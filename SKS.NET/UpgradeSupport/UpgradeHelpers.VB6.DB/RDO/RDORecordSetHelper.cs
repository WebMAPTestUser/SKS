using System;
using System.Collections.Generic;
using System.Text;
using UpgradeHelpers.VB6.DB;
using System.Data.Common;
using UpgradeHelpers.VB6.DB.RDO.Events;
using System.Data;

namespace UpgradeHelpers.VB6.DB.RDO
{
    /// <summary>
    /// Defines the type of concurrence to be used by the recordset.
    /// </summary>
    public enum LockTypeConstants 
    { 
        /// <summary>
        /// Read Only
        /// </summary>
        rdConcurReadOnly = 1, 
        /// <summary>
        /// Lock
        /// </summary>
        rdConcurLock, 
        /// <summary>
        /// By Row
        /// </summary>
        rdConcurRowVer, 
        /// <summary>
        /// By Value
        /// </summary>
        rdConcurValues, 
        /// <summary>
        /// Batch
        /// </summary>
        rdConcurBatch 
    }

    /// <summary>
    /// This class has the same functionality than the recordset exposed by the RDO library.
    /// </summary>
    public class RDORecordSetHelper : RecordSetHelper
    {
        #region Events
        /// <summary>
        /// Row Status Changed event
        /// </summary>
        public event RowStatusChangedEventHandler RowStatusChanged;
        /// <summary>
        /// Will Update Rows event
        /// </summary>
        public event WillUpdateRowsEventHandler WillUpdateRows;
        /// <summary>
        /// Row Currency Change event
        /// </summary>
        public event RowCurrencyChangeEventHandler RowCurrencyChange;
        /// <summary>
        /// Will Dissociate event
        /// </summary>
        public event WillDissociateEventHandler WillDissociate;
        /// <summary>
        /// Will Associate event
        /// </summary>
        public event WillAssociateEventHandler WillAssociate;
        /// <summary>
        /// Dissociate event
        /// </summary>
        public event DissociateEventHandler Dissociate;
        /// <summary>
        /// Associate event
        /// </summary>
        public event AssociateEventHandler Associate;
        #endregion

        private LockTypeConstants locktype;
        private bool editingMode = false;
        
        /// <summary>
        /// Constructs a new RDORecordSetHelper instance using the specified factory.
        /// </summary>
        /// <param name="factoryname">The name used to identify the factory to be used to create all the necesary ADO .Net objects.</param>
        protected RDORecordSetHelper(string factoryname) : base(factoryname) { }

        /// <summary>
        /// creates a new recordset helper.
        /// </summary>
        public RDORecordSetHelper() : this("") { }

        /// <summary>
        /// Gets and Set the connection to be used to interact with the database.
        /// </summary>
        public override DbConnection ActiveConnection
        {
            set
            {
                bool cancel = false;
                if (value == null)
                    OnWillDissociate(ref cancel);
                else
                    OnWillAssociate(value,ref cancel);
                if (!cancel)
                {
                    base.ActiveConnection = value;
                    if (value == null)
                        OnDissociate();
                    else
                        OnAssociate();
                }
            }
        }

        /// <summary>
        /// Gets and Set the position of the current record on the recordset instance.
        /// </summary>
        public int AbsolutePosition
        {
            get { return index == -1 ? index : index + 1; }
            set
            {
                OnRowCurrencyChange();
                BasicMove(value - 1);
            }
        }

        /// <summary>
        /// Gets and Set the percentage of the current position of the total of records retrieved.
        /// </summary>
        public override float PercentPosition
        {
            set
            {
                if (index != -1)
                {
                    OnRowCurrencyChange();
                    base.PercentPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets and Set the lock type to be used by the recordset.
        /// </summary>
        public LockTypeConstants LockType
        {
            get { return locktype; }
            set { locktype = value; }
        }
        /// <summary>
        /// Bookmark a Data Row
        /// </summary>
        public override DataRow Bookmark
        {
            get
            {
                return base.Bookmark;
            }
            set
            {
                CancelUpdate();
                base.Bookmark = value;
            }
        }

        /// <summary>
        /// Gets the first 256 characters of the sql statement used to open the recordset.
        /// </summary>
        public string Name
        {
            get
            {
                String source = getSource();
                return (source.Length > 256) ? source.Substring(0, 256) : source;
            }
        }

        #region Open methods
        
        /// <summary>
        /// Opens the recordset.
        /// </summary>
        private new void Open()
        {
            base.Open();
            if (Tables.Count > 0)
            {
                Tables[0].RowChanging += new DataRowChangeEventHandler(RDORecordSetHelper_RowChanging);
            }
        }

        /// <summary>
        /// Creates a new RDORecordSetHelper and opens it.
        /// </summary>
        /// <param name="SQLstr">The sql statement used to populate the recordset.</param>
        /// <param name="connection">The connection used to interact with the database.</param>
        /// <param name="locktype">The lock type used by the recordset.</param>
        /// <param name="factoryName">The name used to identify the factory to be used to create all the necesary ADO .Net objects.</param>
        /// <returns>A new opened recordset.</returns>
        public static RDORecordSetHelper Open(String SQLstr, DbConnection connection, LockTypeConstants locktype, String factoryName)
        {
            if (factoryName == "")
                factoryName = AdoFactoryManager.Default.Name;
            RDORecordSetHelper result = new RDORecordSetHelper(factoryName);
            result.Source = SQLstr;
            result.LockType = locktype;
            result.ActiveConnection = connection;
            result.Open();
            return result;
        }

        /// <summary>
        /// Creates a new RDORecordSetHelper and opens it.
        /// </summary>
        /// <param name="SQLstr">The sql statement used to populate the recordset.</param>
        /// <param name="connection">The connection used to interact with the database.</param>
        /// <param name="factoryName">The name used to identify the factory to be used to create all the necesary ADO .Net objects.</param>
        /// <returns>A new opened recordset.</returns>
        public static RDORecordSetHelper Open(String SQLstr, DbConnection connection, String factoryName)
        {
            return RDORecordSetHelper.Open(SQLstr, connection,LockTypeConstants.rdConcurReadOnly, factoryName);
        }

        /// <summary>
        ///  Creates a new RDORecordSetHelper and opens it.
        /// </summary>
        /// <param name="command">The sql statement used to populate the recordset.</param>
        /// <param name="locktype">The lock type used by the recordset.</param>
        /// <param name="factoryName">The name used to identify the factory to be used to create all the necesary ADO .Net objects.</param>
        /// <returns>A new opened recordset.</returns>
        public static RDORecordSetHelper Open(DbCommand command, LockTypeConstants locktype, String factoryName)
        {
            RDORecordSetHelper result = new RDORecordSetHelper(factoryName);
            result.Source = command;
            result.LockType = locktype;
            result.Open();
            return result;
        }

        /// <summary>
        ///  Creates a new RDORecordSetHelper and opens it.
        /// </summary>
        /// <param name="command">The sql statement used to populate the recordset.</param>
        /// <param name="factoryName">The name used to identify the factory to be used to create all the necesary ADO .Net objects.</param>
        /// <returns>A new opened recordset.</returns>
        public static RDORecordSetHelper Open(DbCommand command, String factoryName)
        {
            return RDORecordSetHelper.Open(command, LockTypeConstants.rdConcurReadOnly, factoryName);
        }

        #endregion

        #region Data Handling methods

        /// <summary>
        /// Creates a new record on the recordset.
        /// </summary>
        public override void  AddNew()
        {
            OnRowStatusChanged();
            OnRowCurrencyChange();
            base.AddNew();
        }

        /// <summary>
        /// Sets the recordset on edition mode.
        /// </summary>
        public void Edit()
        {
            editingMode = true;
        }

        /// <summary>
        /// Deletes the current record of the recordset.
        /// </summary>
        public override void Delete()
        {
            base.Delete();
            if(!isBatchEnabled())
                Update();
        }

        /// <summary>
        /// Saves the changes done to the current record on the recordset.
        /// </summary>
        /// <remarks>If the recordset is not batch enabled this method saves the changes on the database.</remarks>
        public override void  Update()
        {
            int returncode = 0;
            OnWillUpdateRows(ref returncode);
            DataRow theRow = CurrentRow;
            if (newRow)
            {
                Tables[0].Rows.Add(theRow);
                index = Tables[0].Rows.IndexOf(theRow);
                newRow = false;
                dbRow = null;
            }
            if (theRow.RowState != DataRowState.Unchanged)
            {
                if (!isBatchEnabled())
                {
                    UpdateWithNoEvents(theRow);
                    MoveFirst();
                }
            }
        }
        
        /// <summary>
        /// Saves a batch of changes to the database.
        /// </summary>
        public void BatchUpdate()
        {
            if (isBatchEnabled())
            {
                Update();
                DbConnection connection = GetConnection(connectionString);
                using (DbDataAdapter dbAdapter = CreateAdapter(connection, true))
                {
                    DataTable changes = Tables[0].GetChanges();
                    if (changes != null)
                    {
                        dbAdapter.Update(changes);
                    }
                }
            }
            else
                throw new InvalidOperationException("The current RecordSet is not set for batch processing.");
        }

        /// <summary>
        /// Cancels the changes done to the current recordset.
        /// </summary>
        public override void CancelUpdate()
        {
            bool wasNewRow = newRow;
            OnRowStatusChanged();
            if (wasNewRow)
                base.Cancel();
            else
            {
                editingMode = false;
                base.CancelUpdate();
            }
        }

        /// <summary>
        /// Releses the resources used by the recordset.
        /// </summary>
        public override void Close()
        {
            CancelBatch();
            base.Close();
        }
        #endregion

        #region Misc Methods
        
        /// <summary>
        /// Determines if the recordset is batch enabled.
        /// </summary>
        /// <returns></returns>
        protected override bool isBatchEnabled()
        {
            return locktype == LockTypeConstants.rdConcurBatch;
        }

        /// <summary>
        /// Returns a delimited string for 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="columnDelimiter">Expression used to separate the columns.</param>
        /// <param name="rowDelimiter">Expression used to separate the rows.</param>
        /// <param name="nullExpr">Expression used to replace nulls.</param>
        /// <returns>A delimited string containing a number of rows.</returns>
        public String GetClipString(int numrows, String columnDelimiter, String rowDelimiter, String nullExpr)
        {
            StringBuilder builder = new StringBuilder();
            OnRowCurrencyChange();
            int i = index;
            for (; !EOF && index < i + numrows; index++)
            {
                foreach (Object data in CurrentRow.ItemArray)
                {
                    builder.Append(data == DBNull.Value ? nullExpr : Convert.ToString(data));
                    builder.Append(columnDelimiter);
                }
                builder.Append(rowDelimiter);
                eof = index >= Tables[0].Rows.Count - 1;
            }        
            return builder.ToString();
        }

        /// <summary>
        /// Returns a delimited string for 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="columnDelimiter">Expression used to separate the columns.</param>
        /// <param name="rowDelimiter">Expression used to separate the rows.</param>
        /// <returns>A delimited string containing a number of rows.</returns>
        public String GetClipString(int numrows, String columnDelimiter, String rowDelimiter)
        {
            return GetClipString(numrows, columnDelimiter, rowDelimiter, String.Empty);
        }

        /// <summary>
        /// Returns a delimited string for 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="columnDelimiter">Expression used to separate the columns.</param>
        /// <returns>A delimited string containing a number of rows.</returns>
        public String GetClipString(int numrows, String columnDelimiter)
        {
            return GetClipString(numrows, columnDelimiter, '\n'.ToString());
        }

        /// <summary>
        /// Returns a delimited string for 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <returns>A delimited string containing a number of rows.</returns>
        public String GetClipString(int numrows)
        {
            return GetClipString(numrows, ' '.ToString());
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <returns>A delimited string containing a number of rows.</returns>
        public object[,] GetRows(int numrows)
        {
            object[,] buffer =  new object[Tables[0].Columns.Count,numrows];
            OnRowCurrencyChange();
            int i = index,colindex = 0, rowindex = 0;
            for (; !EOF && index < i + numrows; index++)
            {
                foreach (Object data in CurrentRow.ItemArray)
                {
                    buffer[colindex, rowindex] = data;
                    colindex++;
                }
                colindex = 0;
                rowindex++;
                eof = index >= Tables[0].Rows.Count - 1;
            }
            object[,] result = new object[Tables[0].Columns.Count, rowindex ];
            for (int rindex = 0; rindex < rowindex; rindex++)
                for (int cindex = 0; cindex < Tables[0].Columns.Count; cindex++)
                    result[cindex, rindex] = buffer[cindex, rindex];
            return result;
        }

        #endregion

        #region Move Methods

        /// <summary>
        /// Used to handle the common move call.
        /// </summary>
        private delegate void MoveAction();

        /// <summary>
        /// Moves the current record pointer 'n' number of records.
        /// </summary>
        /// <param name="records">The number of records to move the record pointer.</param>
        public override void Move(int records)
        {
            OnRowCurrencyChange();
            base.Move(records);
        }

        /// <summary>
        /// Moves the record pointer to the first record.
        /// </summary>
        public override void MoveFirst()
        {
            DoMove(base.MoveFirst);
        }

        /// <summary>
        /// Moves the record pointer to the last record.
        /// </summary>
        public override void MoveLast()
        {
            DoMove(base.MoveLast);
        }

        /// <summary>
        /// Moves the record pointer to the next record.
        /// </summary>
        public override void MoveNext()
        {
            DoMove(base.MoveNext);
        }

        /// <summary>
        /// Moves the record pointer to the previous record.
        /// </summary>
        public override void MovePrevious()
        {
            DoMove(base.MovePrevious);
        }

        /// <summary>
        /// Actually executes the move method.
        /// </summary>
        private void DoMove(MoveAction action)
        {
            OnRowCurrencyChange();
            action();
        }

        /// <summary>
        /// Moves between the rows of the current recordset.
        /// </summary>
        protected override void BasicMove(int newIndex)
        {
            index = newIndex < 0 ? 0 : newIndex;
            eof = index > (UsingView ? currentView.Count - 1 : Tables[0].Rows.Count - 1);
            index = eof ? (UsingView ? currentView.Count - 1 : Tables[0].Rows.Count - 1) : index;
            base.BasicMove(newIndex);
        }
        #endregion

        #region event triggers 

        /// <summary>
        /// Method to trigger the row status changed event when an update happens.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void RDORecordSetHelper_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Change && e.Action != DataRowAction.Nothing && e.Action != DataRowAction.Rollback && e.Action != DataRowAction.Commit)
            {
                OnRowStatusChanged();
            }
        }

        /// <summary>
        /// Fires event RowStatusChanged.
        /// </summary>
        private void OnRowStatusChanged()
        {
            if (RowStatusChanged != null)
                RowStatusChanged(this, new EventArgs());
        }

        /// <summary>
        /// Fires event RowCurrencyChange.
        /// </summary>
        private void OnRowCurrencyChange()
        {
            if(newRow || editingMode)
                CancelUpdate();
            if (RowCurrencyChange != null)
                RowCurrencyChange(this, new EventArgs());
        }

        /// <summary>
        /// Fires event WillUpdateRows.
        /// </summary>
        private void OnWillUpdateRows(ref int returncode)
        {
            if (WillUpdateRows != null)
            {
                WillUpdateRowsEventArgs e = new WillUpdateRowsEventArgs(returncode);
                WillUpdateRows(this, e);
                returncode = e.Returncode;
            }
        }

        /// <summary>
        /// Fires event Associate.
        /// </summary>
        private void OnAssociate()
        {
            if (Associate != null)
                Associate(this, new EventArgs());
        }

        /// <summary>
        /// Fires event Dissociate.
        /// </summary>
        private void OnDissociate()
        {
            if (Dissociate != null)
                Dissociate(this, new EventArgs());
        }

        /// <summary>
        /// Fires event WillAssociate.
        /// </summary>
        private void OnWillAssociate(DbConnection connection, ref bool cancel)
        {
            if (WillAssociate != null)
            {
                WillAssociateEventArgs e =new WillAssociateEventArgs(connection, cancel);
                WillAssociate(this, e);
                cancel = e.Cancel;
            }
        }

        /// <summary>
        /// Fires event WillDissociate.
        /// </summary>
        private void OnWillDissociate(ref bool cancel)
        {
            if (WillDissociate != null)
            {
                WillDissociateEventArgs e = new WillDissociateEventArgs(cancel);
                WillDissociate(this, e);
            }
        }

        #endregion
    }
}
