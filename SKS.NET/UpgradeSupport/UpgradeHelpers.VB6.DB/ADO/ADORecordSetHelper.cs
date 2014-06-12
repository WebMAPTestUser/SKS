using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using UpgradeHelpers.VB6.DB.ADO.Events;
using System.Runtime.Serialization;
using System.ComponentModel;


/// <summary>
/// 
/// </summary>
public static class DBTrace
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="conn"></param>
#if TargetF2
    public static void OpenWithTrace(DbConnection conn)
#else
	public static void OpenWithTrace(this DbConnection conn)
#endif
	{
		conn.ConnectionString = NewConnectionstring(conn.ConnectionString);
#if DBTrace
		File.AppendAllText(LogDBTrace, "Openning connection [" + conn.ConnectionString + "] " + new System.Diagnostics.StackTrace().ToString());
#endif
		conn.Open();
	}

	/// <summary>
	/// CreateConnectionWithTrace
	/// </summary>
	/// <param name="factory"></param>
	/// <returns></returns>
#if TargetF2
    public static DbConnection CreateConnectionWithTrace(DbProviderFactory factory)
#else
	public static DbConnection CreateConnectionWithTrace(this DbProviderFactory factory)
#endif
	{
#if DBTrace
		File.AppendAllText(LogDBTrace, "Creating connection " + new System.Diagnostics.StackTrace().ToString());
#endif
		return factory.CreateConnection();

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="connectionString"></param>
	/// <returns></returns>
	public static string NewConnectionstring(String connectionString)
	{
#if ConnectionPoolOff
		if (!connectionString.Contains("Pooling="))
		{
			if (connectionString.EndsWith(";"))
			{
				connectionString += "Pooling=false;";
			}
			else
			{
				connectionString += ";Pooling=false;";
			}
		}
		return connectionString;
#else
		return connectionString;
#endif

	}

}

namespace UpgradeHelpers.VB6.DB.ADO
{   
    /// <summary>
    /// Determines which records will be affected by the operation.
    /// </summary>
    public enum AffectEnum
    {
        /// <summary>
        /// Affect Current
        /// </summary>
        adAffectCurrent = 1,
        /// <summary>
        /// Affect Group
        /// </summary>
        adAffectGroup,
        /// <summary>
        /// Affect All
        /// </summary>
        adAffectAll,
        /// <summary>
        /// Affect All Chapters
        /// </summary>
        adAffectAllChapters
    }

    /// <summary>
    /// Determines if server-side or client-side cursors are used (which cursor engine used).
    /// </summary>
    public enum CursorLocationEnum
    {
        /// <summary>
        /// Use None
        /// </summary>
        adUseNone = 1,
        /// <summary>
        /// Use Server
        /// </summary>
        adUseServer = 2,
        /// <summary>
        /// Use Client
        /// </summary>
        adUseClient = 3
    }
    /// <summary>Sets or returns the type of locking (concurrency) to use.</summary>
    public enum LockTypeEnum
    {
        /// <summary>
        /// Lock not specified, value -1
        /// </summary>
        adLockUnspecified = -1,
        /// <summary>
        /// Read Only Lock, value 1
        /// </summary>
        adLockReadOnly = 1,
        /// <summary>
        /// Pessimistic Lock
        /// </summary>
        adLockPessimistic = 2,
        /// <summary>
        /// Optimistic Lock
        /// </summary>
        adLockOptimistic = 3,
        /// <summary>
        /// Batch Optimistic Lock
        /// </summary>
        adLockBatchOptimistic = 4
    }

    /// <summary>Determines if the operation will affect the records in a specific position.</summary>
    public enum PositionEnum
    {
        /// <summary>
        /// Begin of File Position
        /// </summary>
        adPosBOF = -2,
        /// <summary>
        /// End Of File Position
        /// </summary>
        adPosEOF = -3,
        /// <summary>
        /// Postion unknown
        /// </summary>
        adPosUnknown = -1
    }

    /// <summary>
    /// Enum to describe the different edition modes for the Recordset
    /// </summary>
    public enum EditModeEnum
    {
        /// <summary>
        /// No edition is in progress
        /// </summary>
        adEditNone = 0,
        /// <summary>
        /// Edition is in progress
        /// </summary>
        adEditInProgress = 1,
        /// <summary>
        /// Addition is in progress
        /// </summary>
        adEditAdd = 2,
        /// <summary>
        /// Delete is in progress
        /// </summary>
        adEditDelete = 4
    }

    /// <summary>
    /// Enum to describe the bookmark prosition to be used by the GetRows method
    /// </summary>
    public enum BookmarkEnum
    {
        /// <summary>
        /// Uses the current position 
        /// </summary>
        adBookmarkCurrent = 0,
        /// <summary>
        /// Starts at the first record
        /// </summary>
        adBookmarkFirst,
        /// <summary>
        /// Starts at the last record
        /// </summary>
        adBookmarkLast
    }

    /// <summary>
    /// Support class for the ADO.Recorset the object that represents the records in a base table or the records that result from running a query.
    /// </summary>
    [Serializable]
    public class ADORecordSetHelper : RecordSetHelper
    {
        #region Class Variables

        #region Events declarations

        /// <summary>Occurs when EOF/BOF hit.</summary>
        public event EndOfRecordsetEventHandler EndOfRecordset;

        /// <summary>Occurs before a field change.</summary>
        public event FieldChangeEventHandler WillChangeField;

        /// <summary>Occurs after a field change.</summary>
        public event FieldChangeCompleteEventHandler FieldChangeComplete;

        /// <summary>Occurs before a record change.</summary>
        public event RecordChangeEventHandler WillChangeRecord;

        /// <summary>Occurs after a record change.</summary>
        public event RecordChangeCompleteEventHandler RecordChangeComplete;

        /// <summary>Occurs before a recordset change.</summary>
        public event RecordSetChangeEventHandler WillChangeRecordset;

        /// <summary>Occurs after a recordset change.</summary>
        public event RecordSetChangeCompleteEventHandler RecordsetChangeComplete;

        /// <summary>Occurs before a different row becomes the current row.</summary>
        public event MoveEventHandler WillMove;

        /// <summary>Occurs after a row becomes the current row.</summary>
        public event MoveCompleteEventHandler MoveComplete;

        #endregion

        private LockTypeEnum lockType = LockTypeEnum.adLockReadOnly;
        private CursorLocationEnum cursorLocation = CursorLocationEnum.adUseClient;
        private ConnectionState state = ConnectionState.Closed;
        private String sort = "";

        /// <summary>
        /// Flag that indicates if the current recordset is a cloned one
        /// </summary>
        protected bool isClone = false;

        private EditModeEnum editMode = EditModeEnum.adEditNone;
        private Queue<DbCommand> commands = null;

        private const string NOT_ALLOWED_OPERATION = "Operation is not allowed when the object is closed.";

        #endregion

        #region  properties

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        public override int PageCount
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return base.PageCount;
            }
        }

        /// <summary>
        /// Gets a bool value indicating if the current record is the last one in the RecordsetHelper object.
        /// </summary>
        public override bool EOF
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return base.EOF;
            }
        }

        /// <summary>
        /// Property to indicate the editing status of the current record
        /// </summary>
        public EditModeEnum EditMode
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return editMode;
            }
        }

        /// <summary>
        /// Property to handle the Status of the recordset
        /// </summary>
        public DataRowState Status
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return FieldsValues.RowState;
            }
        }

        /// <summary>
        /// Property used to determine if the data needs to be get from a dataview or the table directly
        /// </summary>
        protected override bool UsingView
        {
            get
            {
                return base.UsingView || !String.IsNullOrEmpty(sort) || isClone;
            }
        }

        /// <summary>
        /// Property to get and set the order of the recordset
        /// </summary>
        public String Sort
        {
            get
            {
                return sort;
            }
            set
            {
                if (isDefaultSerializationInProgress) return;
                sort = value;
                if (currentView != null)
                {
                currentView.Sort = sort;
                }
                if (opened)
                {
                    MoveFirst(EventReasonEnum.adRsnRequery);
                }
            }
        }

        /// <summary>
        /// Property to handle the state of the recordset
        /// </summary>
        public ConnectionState State
        {
            //Changes for remove memory leak
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        /// <summary>
        /// This is an override to wire the event necesary to handle the proper state of the recordset
        /// </summary>
        public override DbConnection ActiveConnection
        {
            set
            {
                if (ActiveConnection != null)
                {
                    /* We must remove StateChangeEventHandlers because we are disattaching the connection **/
                    ActiveConnection.StateChange -= new StateChangeEventHandler(ActiveConnection_StateChange);
                }
                base.ActiveConnection = value;
                if (ActiveConnection != null)
                {
                    // Remove memory leak
                    ActiveConnection.StateChange += new StateChangeEventHandler(ActiveConnection_StateChange);
                    CursorLocation = ADOConnectionSettingsHelper.GetCursorLocation(ActiveConnection);
                }
            }
        }

        /// <summary>
        /// Event delegate necesary to handle the proper state of the recordset
        /// </summary>
        /// <param name="sender">The connection object</param>
        /// <param name="e">The arguments of the event</param>
        private void ActiveConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (!(e.CurrentState == ConnectionState.Closed || e.CurrentState == ConnectionState.Broken || e.CurrentState == ConnectionState.Open))
            {
                State = e.CurrentState;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the current record position is before the first record in an ADORecordsetHelper object. Read-only Boolean.
        /// </summary>
        public override bool BOF
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }

                if (this.UsingView)
                    return this.index < 0 || this.currentView.Count == 0;
                else if (this.Tables.Count > 0)
                    return this.index < 0 || this.Tables[0].Rows.Count == 0;
                else
                    return true;
            }
        }

        /// <summary>
        /// Sets the Filter to by applied to the this ADORecordsetHelper. (valid objects are: string, DataViewRowState and DataRow[]).
        /// </summary>
        [DefaultValue(null)]
        public override Object Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                base.Filter = value;
                if (opened)
                {
                    //First reset the index and eof
                    //When the user filters, the current row goes to the
                    //first row if there is one.
                    //Also there might be no rows at all.
                    //AIS-TODO try not setting index to -1 and not eof
                    index = - 1;
                    eof = this.IsEof();
                    if (this.RecordCount > 0)
                    {
                        MoveFirst(EventReasonEnum.adRsnRequery);
                    }
                }
            }
        }

        /// <summary>
        /// Gets/Sets the LockType for this object.
        /// </summary>
        public LockTypeEnum LockType
        {
            get
            {
                return lockType;
            }
            set
            {
                lockType = value;
            }
        }

        /// <summary>
        /// Gets/Sets the CursorLocation for this object.
        /// </summary>
        public CursorLocationEnum CursorLocation
        {
            get
            {
                return cursorLocation;
            }
            set
            {
                cursorLocation = value;
            }
        }

        /// <summary>
        /// Gets the total number of records currently on the ADORecordsetHelper.
        /// </summary>
        public override int RecordCount
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return base.RecordCount;
            }
        }

        /// <summary>
        /// Gets and Sets the position of the current record on the recordset instance.
        /// </summary>
        public int AbsolutePosition
        {
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                /*if (BOF)
                {
                    return (int)PositionEnum.adPosBOF;
                }
                else if (EOF)
                {
                    return (int)PositionEnum.adPosEOF;
                }
                else
                {*/
                return index + 1;
                //}
            }
            set
            {
                throw new NotImplementedException("Ais-Todo: This is not implemented");
            }
        }

        /// <summary>
        /// Array access by column name to set the object
        /// </summary>
        /// <param name="columnName">string column name</param>
        /// <returns>object</returns>
        public override Object this[String columnName]
        {
            get
            {
                return base[columnName];
            }
            set
            {
                base[columnName] = value;
                if (!isBatchEnabled() || editMode != EditModeEnum.adEditNone)
                {
                    editMode = EditModeEnum.adEditInProgress;
                }
                firstChange = firstChange ? false : firstChange;
            }
        }

        /// <summary>
        /// Array access by index
        /// </summary>
        /// <param name="columnIndex">index value</param>
        /// <returns>object</returns>
        public override Object this[int columnIndex]
        {
            get
            {
                return base[columnIndex];
            }
            set
            {
                base[columnIndex] = value;
                if (!isBatchEnabled() || editMode != EditModeEnum.adEditNone)
                {
                    editMode = EditModeEnum.adEditInProgress;
                }
                firstChange = firstChange ? false : firstChange;
            }
        }

        /// <summary>
        /// Sets a bookmark to an specific record inside the ADORecordsetHelper.
        /// </summary>
        public override DataRow Bookmark
        {
            set
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                EventStatusEnum status = EventStatusEnum.adStatusOK;
                OnWillMove(EventReasonEnum.adRsnMove, ref status);
                string[] errors = null;
                try
                {
                    Update();
                    base.Bookmark = value;
                }
                catch (Exception e)
                {
                    errors = new string[] { e.Message };
                }
                OnMoveComplete(EventReasonEnum.adRsnMove, ref status, errors);
            }
            get
            {
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                return base.Bookmark;
            }
        }

        /// <summary>
        /// Gets/Sets the current page number.
        /// </summary>
        public int AbsolutePage
        {
            get
            {
                if (isDefaultSerializationInProgress) return 0;
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
                EventStatusEnum status = EventStatusEnum.adStatusOK;
                OnWillMove(EventReasonEnum.adRsnMove, ref status);
                string[] errors = null;
                OnMoveComplete(EventReasonEnum.adRsnMove, ref status, errors);
                if (BOF)
                {
                    return (int)PositionEnum.adPosBOF;
                }
                else if (EOF)
                {
                    return (int)PositionEnum.adPosEOF;
                }
                else
                {
                    if (PageSize != 0)
                    {
                        return (int)Math.Ceiling((double)AbsolutePosition /PageSize);
                    }
                    return 0;
                }
            }
            set
            {
                if (isDefaultSerializationInProgress) return;
                if (!Opened)
                {
                    throw new InvalidOperationException(NOT_ALLOWED_OPERATION);
                }
				if (value > 0)
				{
					index = (value - 1) * PageSize;
				}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object DataSource
        {
            get
            {
                object result = null;
                if (State != ConnectionState.Closed)
                {
                    if (UsingView)
                    {
                        result = currentView;
                    }
                    else
                    {
                        result = Tables[0];
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String DataMember
        {
            get
            {
                string result = string.Empty;
                if (State != ConnectionState.Closed)
                {
                    if (UsingView)
                    {
                        result = currentView.Table.TableName;
                    }
                    else
                    {
                        result = Tables[0].TableName;
                    }
                }
                return result;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new ADORecordsetHelper instance using the default factory specified on the configuration xml.
        /// </summary>
        public ADORecordSetHelper()
            : base(AdoFactoryManager.Default.Name)
        {
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper instance using the factory specified on the “factoryName” parameter.
        /// </summary>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        public ADORecordSetHelper(String factoryName)
            : base(string.IsNullOrEmpty(factoryName) ? AdoFactoryManager.Default.Name : factoryName)
        {

        }

        /// <summary>
        /// Creates a new ADORecordsetHelper instance using provided parameters.
        /// </summary>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <param name="connString">The connection string to be used by this ADORecordsetHelper.</param>
        public ADORecordSetHelper(String factoryName, String connString)
            : base(factoryName, connString)
        {
            
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper instance using provided parameters.
        /// </summary>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <param name="connString">The connection string to be used by this ADORecordsetHelper.</param>
        /// <param name="sqlSelectString">A string containing the SQL Query to be loaded on the ADORecordsetHelper.</param>
        public ADORecordSetHelper(String factoryName, String connString, String sqlSelectString)
            : base(factoryName, connString, sqlSelectString)
        {
        }

        #endregion

        #region Serialization machinery

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">System.Runtime.Serialization.SerializationInfo, all the data needed to load and store an object.</param>
        /// <param name="context">System.Runtime.Serialization.StreamingContext, describes the source and destination of 
        /// a given serialized stream , and provides an additional caller-defined context.</param>
        public ADORecordSetHelper(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Sort = info.GetString("Sort");
            if (opened)
            {
                AbsolutePage = info.GetInt32("AbsolutePage");
            }
            LockType = (LockTypeEnum)info.GetInt32("LockType");
            bool activeConnectionWasNull = (bool)serializationInfo["ActiveConnectionWasNull"];
            string connectionString = (String) serializationInfo["ConnectionString"];
            serializationInfo = null;
            if (LockType == LockTypeEnum.adLockReadOnly || activeConnectionWasNull)
            {
                //We do not need to trigger the logic that recreates the active connection
                this.connectionString = connectionString;
            }
            else
            {
                ConnectionString = connectionString;
            }

        }

        /// <summary>
        /// Gets Object Data
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //The DataSet serialization process will call ALL public properties including AbsolutePage
            //If recordset is not Open, then it should use isDefaultSerializationInProgress variable
            //to skip the raise of InvalidOperationException(NOT_ALLOWED_OPERATION) in AbsolutePage property
            if (!Opened)
            {
                isDefaultSerializationInProgress = true;
            }
            base.GetObjectData(info, context);
            info.AddValue("Sort", Sort);
            if (opened)
            {
                info.AddValue("AbsolutePage", AbsolutePage);
            }
            info.AddValue("LockType", (int)LockType);

        }
 
        #endregion


        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows()
        {
            return GetRows(-1, null, (int[])null);
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows)
        {
            return GetRows(numrows, null, (int[])null);
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="startfrom">A bookmark representing the row to begin from</param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows, object startfrom)
        {
            return GetRows(numrows, startfrom, new int[] { });
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="startfrom">A bookmark representing the row to begin from</param>
        /// <param name="fieldname">The field name to be get from the row</param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows, object startfrom, string fieldname)
        {
            return GetRows(numrows, startfrom, new string[] { fieldname });
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be r    etrieved.</param>
        /// <param name="startfrom">A bookmark representing the row to begin from</param>
        /// <param name="fieldnames">An array of field names to be get from the recordset</param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows, object startfrom, string[] fieldnames)
        {
            int[] fieldpositions = new int[fieldnames.Length];
            for (int i = 0; i < fieldnames.Length; i++)
            {
                fieldpositions[i] = Tables[0].Columns.IndexOf(fieldnames[i]);
            }
            return GetRows(numrows, startfrom, fieldpositions);
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="startfrom">A bookmark representing the row to begin from</param>
        /// <param name="fieldposition">The field index to be get from the recordset </param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows, object startfrom, int fieldposition)
        {
            return GetRows(numrows, startfrom, new int[] { fieldposition });
        }

        /// <summary>
        /// Returns a two dimmension array representing 'n' rows in a result set.
        /// </summary>
        /// <param name="numrows">Number of rows to be retrieved.</param>
        /// <param name="startfrom">A bookmark representing the row to begin from</param>
        /// <param name="fieldpositions">The field indexes to be get from the recordset</param>
        /// <returns>An array containing a number of rows.</returns>
        public object[,] GetRows(int numrows, object startfrom, int[] fieldpositions)
        {
            object[,] buffer = null;
            if (startfrom != null)
            {
                if (startfrom is DataRow)
                {
                    Bookmark = (DataRow)startfrom;
                }
                else if (startfrom is BookmarkEnum)
                {
                    switch ((BookmarkEnum)startfrom)
                    {
                        case BookmarkEnum.adBookmarkFirst:
                            MoveFirst();
                            break;
                        case BookmarkEnum.adBookmarkLast:
                            MoveLast();
                            break;
                    }
                }
                else if (startfrom is string)
                {
                    throw new InvalidOperationException("String parameter not supported on the GetRows method");
                }
            }
            numrows = numrows == -1 ? (int)(RecordCount - index) : numrows;
            if (!(fieldpositions == null || fieldpositions.Length <= 0))
            {
                buffer = new object[fieldpositions.Length,numrows];
            }
            else
            {
                buffer = new object[Tables[0].Columns.Count,numrows];
            }

            int i = index, colindex = 0, rowindex = 0;
            for (; !EOF && index < i + numrows; index++)
            {
                if (!(fieldpositions == null || fieldpositions.Length <= 0))
                {
                    foreach (int fieldposition in fieldpositions)
                    {
                        buffer[colindex, rowindex] = CurrentRow[fieldposition];
                        colindex++;
                    }
                }
                else
                {
                    foreach (Object data in CurrentRow.ItemArray)
                    {
                        buffer[colindex, rowindex] = data;
                        colindex++;
                    }
                }
                colindex = 0;
                rowindex++;
                eof = index >= (!UsingView ? Tables[0].Rows.Count - 1 : currentView.Count - 1);
            }
            index = eof ? (!UsingView ? Tables[0].Rows.Count - 1 : currentView.Count - 1) : index;
            object[,] result = new object[buffer.GetLength(0),rowindex];
            for (int rindex = 0; rindex < rowindex; rindex++)
            {
                for (int cindex = 0; cindex < result.GetLength(0); cindex++)
                {
                    result[cindex, rindex] = buffer[cindex, rindex];
                }
            }
            return result;
        }

        /// <summary>
        /// Moves the position of the currentRecord in a RecordSet.
        /// </summary>
        /// <param name="records">Amount of records positive or negative to move from the current record.</param>
        public override void Move(int records)
        {
            Move(records, EventReasonEnum.adRsnMove, EventStatusEnum.adStatusOK);
        }

        /// <summary>
        /// Moves the current record to the beginning of the ADORecordsetHelper.
        /// </summary>
        public override void MoveFirst()
        {
            MoveFirst(EventReasonEnum.adRsnMoveFirst);
        }

        /// <summary>
        /// Moves the current record to the end of the ADORecordsetHelper.
        /// </summary>
        /// <param name="Options"></param>
        public void MoveLast(int Options)
        {
            //TODO: ToBeImplemented where Options != 0
            if (Options == 0)
            {
                MoveLast();
            }
        }

        /// <summary>
        /// Moves the current record to the end of the ADORecordsetHelper.
        /// </summary>
        public override void MoveLast()
        {
            Move((UsingView ? currentView.Count - 1 : Tables[0].Rows.Count - 1) - index, EventReasonEnum.adRsnMoveLast, EventStatusEnum.adStatusOK);
        }

        /// <summary>
        /// Moves the current record forward one position.
        /// </summary>
        public override void MoveNext()
        {
            Move(1,EventReasonEnum.adRsnMoveNext,EventStatusEnum.adStatusOK);
        }

        /// <summary>
        /// Moves the current record backwards one position.
        /// </summary>
        public override void MovePrevious()
        {
            Move(-1,EventReasonEnum.adRsnMovePrevious,EventStatusEnum.adStatusOK);
        }

        /// <summary>
        /// Creates a new record for an updatable Recordset.
        /// </summary>
        /// <param name="rows">Array containing the rows to be added to the ADORecordsetHelper.</param>
        /// <param name="values">Array containing the values for the rows to be inserted on the ADORecordsetHelper.</param>
        public void AddNew(object[] rows, object[] values)
        {
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            OnWillMove(EventReasonEnum.adRsnAddNew, ref status);
            string[] errors = null;
            if (status != EventStatusEnum.adStatusCancel)
            {
                OnWillChangeRecord(EventReasonEnum.adRsnAddNew, ref status, 1);
                if (status != EventStatusEnum.adStatusCancel)
                {
                    try
                    {
                        if (rows != null && values != null)
                        {
                            doAddNew();
                            OnRecordChangeComplete(EventReasonEnum.adRsnAddNew, ref status, 1, errors);
                            for (int i = 0; i < rows.Length; i++)
                            {
                                if (i == 0)
                                {
                                    OnWillChangeRecord(EventReasonEnum.adRsnAddNew, ref status, 1);
                                }
                                CurrentRow[rows[i].ToString()] = values[i];
                                if (i == 0)
                                {
                                    OnRecordChangeComplete(EventReasonEnum.adRsnAddNew, ref status, 1, errors);
                                }
                            }
                            Update();
                        }
                    }
                    catch (Exception e)
                    {
                        errors = new string[] { e.Message };
                        status = EventStatusEnum.adStatusErrorsOccurred;
                        throw e;
                    }
                }
                OnMoveComplete(EventReasonEnum.adRsnAddNew, ref status, errors);
            }
        }

        /// <summary>
        /// Creates a new record for an updatable Recordset.
        /// </summary>
        public override void AddNew()
        {
            //Validations. AddNew is not allowed for Recordset with ReadOnly LockType
            if (LockType == LockTypeEnum.adLockReadOnly)
            {
                throw new NotSupportedException("AddNew is not supported for RecordSets with a LockType " + LockType);
            }
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            OnWillMove(EventReasonEnum.adRsnMove, ref status);
            string[] errors = null;
            if (status != EventStatusEnum.adStatusCancel)
            {
                OnWillChangeRecord(EventReasonEnum.adRsnAddNew, ref status, 1);
                if (status != EventStatusEnum.adStatusCancel)
                {
                    try
                    {
                        if (!isBatchEnabled() || editMode != EditModeEnum.adEditNone)
                        {
                            editMode = EditModeEnum.adEditAdd;
                        }
                        base.AddNew();
                        if (!UsingView)
                        {
                            Tables[0].Rows.Add(dbRow);
                            base.newRow = false;
                            MoveLast();
                        }
                    }
                    catch (Exception e)
                    {
                        errors = new string[] { e.Message };
                        status = EventStatusEnum.adStatusErrorsOccurred;
                    }
                    OnRecordChangeComplete(EventReasonEnum.adRsnAddNew, ref status, 1, errors);
                }
                OnMoveComplete(EventReasonEnum.adRsnMove, ref status, errors);
            }
        }

        /// <summary>
        /// Deletes the current record or a group of records.
        /// </summary>
        /// <param name="deleteBehavior">AffectEnum value indicating if the deletion applies to the current group or a group.</param>
        public void Delete(int deleteBehavior)
        {
            Exception exceptionToThrow = null;
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            string[] errors = null;
            OnWillChangeRecord(EventReasonEnum.adRsnDelete, ref status, 1);
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    if (!isBatchEnabled() || editMode != EditModeEnum.adEditNone)
                    {
                        editMode = EditModeEnum.adEditDelete;
                    }
                    DataRow deletingRow;
                    switch (deleteBehavior)
                    {
                        case (int)AffectEnum.adAffectCurrent:
                            deletingRow = UsingView ? currentView[index].Row : Tables[0].Rows[index];
                            break;
                        case (int)AffectEnum.adAffectGroup:
                            deletingRow = UsingView ? currentView[index].Row : Tables[0].Rows[index];
                            break;
                        default:
                            throw new ArgumentException("Value not allowed to delete.");
                    }
                    deletingRow.Delete();
                    Update();
                }
                catch (Exception e)
                {
                    if (!isBatchEnabled() || editMode != EditModeEnum.adEditNone)
                    {
                        editMode = EditModeEnum.adEditInProgress;
                    }
                    errors = new string[] { e.Message };
                    exceptionToThrow = e;
                }
                if (exceptionToThrow != null)
                {
                    throw exceptionToThrow;
                }
                OnRecordChangeComplete(EventReasonEnum.adRsnDelete, ref status, 1, errors);
            }
        }

        /// <summary>
        /// Deletes the current record.
        /// </summary>
        public override void Delete()
        {
            Delete((int)AffectEnum.adAffectCurrent);
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        public void Edit()
        {
            //TODO: ToBeImplemented
            //throw new System.Exception("Method or Property not implemented yet!");
        }

        #region Updated Related

        /// <summary>
        /// Saves any changes you make to the current row of an ADORecordsetHelper object.
        /// </summary>
        public override void Update()
        {
            // This is done to support disconnected recordSet operations.
            if (SupportsDisconnectedRecordsetOperations)
            {
                AcceptChanges();
            }
            else
            {
                Update(true);
            }
        }

        /// <summary>
        /// Saves the current content of the ADORecordsetHelper to the database.
        /// </summary>
        /// <param name="UpdateType">>The UpdateType to be use by this update.</param>
        /// <param name="Force">A Boolean value indicating whether or not to force the changes into the database.</param>
        public void Update(int UpdateType, bool Force)
        {
            //note: No case has been found to use the specialization parameters. 
            //if (UpdateType == 1)
            Update();
        }

        /// <summary>
        /// Updates the provided “Fields” with the “values” received has parameter.
        /// </summary>
        /// <param name="fields">Array containing the fields to be updated.</param>
        /// <param name="values">Array containing the values to be used to update the fields.</param>
        public void Update(object[] fields, object[] values)
        {
            if (fields == null)
            {
                throw new ArgumentException("RecordSetHelper.Update fields parameter cannot be null ");
            }
            if (fields.Length == 0)
            {
                throw new ArgumentException("RecordSetHelper.Update fields parameter lenght cannot be zero ");
            }
            if (values == null)
            {
                throw new ArgumentException("RecordSetHelper.Update values parameter cannot be null ");
            }
            if (values.Length == 0)
            {
                throw new ArgumentException("RecordSetHelper.Update values parameter lenght cannot be zero ");
            }
            if (values.Length != fields.Length)
            {
                throw new ArgumentException("RecordSetHelper.Update fields and values arrays have to be of the same lenght");
            }

            Type elementType = fields[0].GetType();
            bool isString = elementType.Equals(Type.GetType("System.String"));
            bool isInt = elementType.Equals(Type.GetType("System.Int16")) || elementType.Equals(Type.GetType("System.Int32")) || elementType.Equals(Type.GetType("System.Int64"));
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            OnWillChangeField(ref status, fields.Length, iterateFields(fields, values, isString, true));
            if (status != EventStatusEnum.adStatusCancel)
            {
                OnFieldChangeComplete(ref status, fields.Length, iterateFields(fields, values, isString, false), new string[] { });
                Update();
            }
        }

        /// <summary>
        /// Updates the provided "field" with the "value" recieved has parameter.
        /// </summary>
        /// <param name="field">The field to be updated.</param>
        /// <param name="value">The value to update the field with.</param>
        public void Update(object field, object value)
        {
            if (field == null)
            {
                throw new ArgumentException("RecordSetHelper.Update field parameter cannot be null ");
            }
            if (value == null)
            {
                throw new ArgumentException("RecordSetHelper.Update value parameter cannot be null ");
            }
            Type elementType = field.GetType();
            bool isString = elementType.Equals(Type.GetType("System.String"));
            bool isInt = elementType.Equals(Type.GetType("System.Int16")) || elementType.Equals(Type.GetType("System.Int32")) || elementType.Equals(Type.GetType("System.Int64"));
            if (isString)
            {
                this[(String)field] = value;
            }
            else if (isInt)
            {
                this[(int)field] = value;
            }
            try
            {
                UpdateWithNoEvents(CurrentRow);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Cancels any changes made to the current or new row of a ADORecordsetHelper object.
        /// </summary>
        public override void CancelUpdate()
        {
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            string[] errors = null;
            OnWillChangeRecord(EventReasonEnum.adRsnUndoUpdate, ref status, 1);
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    base.CancelUpdate();
                    editMode = EditModeEnum.adEditNone;
                }
                catch (Exception e)
                {
                    errors = new string[] { e.Message };
                }
                OnRecordChangeComplete(EventReasonEnum.adRsnUndoUpdate, ref status, 1, errors);
            }
        }

        /// <summary>
        /// Writes all pending batch updates to disk.
        /// </summary>
        public void UpdateBatch()
        {
            Exception exceptionToThrow = null;
            if (UsingView)
            {
                dbvRow.EndEdit();
                index = findBookmarkIndex(dbvRow.Row);
            }
            if (isBatchEnabled())
            {
                DbConnection connection = GetConnection(connectionString);
                using (DbDataAdapter dbAdapter = CreateAdapter(connection, true))
                {
                    DataTable changes = UsingView ? currentView.Table.GetChanges() : Tables[0].GetChanges();
                    if (changes != null)
                    {
                        EventStatusEnum status = EventStatusEnum.adStatusOK;
                        string[] errors = null;
                        OnWillChangeRecord(EventReasonEnum.adRsnUpdate, ref status, 1);
                        if (status != EventStatusEnum.adStatusCancel)
                        {
                            try
                            {
                                dbAdapter.Update(isClone ? currentView.Table.DataSet : this);
                                editMode = EditModeEnum.adEditNone;
                            }
                            catch (Exception e)
                            {
                                errors = new string[] { e.Message };
                                exceptionToThrow = e;
                            }
                            OnRecordChangeComplete(EventReasonEnum.adRsnUpdate, ref status, 1, errors);
                            if (exceptionToThrow != null)
                            {
                                throw exceptionToThrow;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Cancels execution of any pending process.
        /// </summary>
        public override void Cancel()
        {
            bool wasNewRow = newRow;
            if (CurrentRow.RowState != DataRowState.Unchanged)
            {
                EventStatusEnum status = EventStatusEnum.adStatusOK;
                string[] errors = null;
                if (status != EventStatusEnum.adStatusCancel)
                {
                    try
                    {
                      //  base.Cancel();
                    }
                    catch (Exception e)
                    {
                        errors = new string[] { e.Message };
                    }
                }
            }
        }

        /// <summary>
        /// Cancels a pending batch update.
        /// </summary>
        public override void CancelBatch()
        {
            bool wasNewRow = newRow;
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            string[] errors = null;
            OnWillChangeRecord(wasNewRow ? EventReasonEnum.adRsnUndoAddNew : EventReasonEnum.adRsnUndoUpdate, ref status, 1);
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    base.CancelBatch();
                    index = -1;
                    editMode = EditModeEnum.adEditNone;
                }
                catch (Exception e)
                {
                    errors = new string[] { e.Message };
                }
                OnRecordChangeComplete(wasNewRow ? EventReasonEnum.adRsnUndoAddNew : EventReasonEnum.adRsnUndoUpdate, ref status, 1, errors);
            }
        }

        /// <summary>
        /// Indicates if the ADORecordsetHelper is in batch mode.
        /// </summary>
        /// <returns>True if the ADORecordsetHelper is in batch mode, otherwise false.</returns>
        protected override bool isBatchEnabled()
        {
            return lockType == LockTypeEnum.adLockBatchOptimistic && cursorLocation == CursorLocationEnum.adUseClient;
        }

        /// <summary>
        /// Verifies if the ADORecordset object have been open.
        /// </summary>
        protected override void Validate()
        {
            if (opened)
            {
                if (SupportsDisconnectedRecordsetOperations)
                {
                    /* OK */
                }
                else if (!isClone)
                {
                    throw new InvalidOperationException("The recordSet is already open");
                }
            }
        }

        /// <summary>
        /// Closes an open object and any dependent objects.
        /// </summary>
        public override void Close()
        {
            if (HasChanges() && !isBatchEnabled())
            {
                throw new InvalidOperationException("Cancel or update required");
            }
            base.Close();
            isClone = false;
            State = ConnectionState.Closed;
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            OnRecordsetChangeComplete(EventReasonEnum.adRsnClose, ref status, null);
            /* An ADORecordSetHelper could be in a disconnected stated. In that case the ActiveConnection
             * must be checked for null to avoid a null pointer reference when trying to remove EventHandler */
            if (ActiveConnection!=null)
            {
                ActiveConnection.StateChange -= new StateChangeEventHandler(ActiveConnection_StateChange);
            }
        }

        /// <summary>
        /// This method clones the recordset instance
        /// </summary>
        /// <returns>The cloned recordset</returns>
        public new ADORecordSetHelper Clone()
        {
            return Clone(LockType);
        }

        /// <summary>
        /// This method clones the recordset instance
        /// </summary>
        /// <param name="lockType">The lock type to be used by the cloned recorset</param>
        /// <returns>The cloned recordset</returns>
        public ADORecordSetHelper Clone(LockTypeEnum lockType)
        {
            ADORecordSetHelper result = new ADORecordSetHelper();
            result.DatabaseType = DatabaseType;
            result.ProviderFactory = ProviderFactory;
            result.opened = true;
            result.isClone = true;
            result.LockType = lockType;
            result.ActiveConnection = ActiveConnection;
            result.activeCommand = activeCommand;
            result.currentView = new DataView(Tables[0]);
            result.State = State;
            result.CursorLocation = CursorLocation;
            if (FieldChangeComplete != null)
            {
                result.FieldChangeComplete = FieldChangeComplete;
            }
            if (RecordChangeComplete != null)
            {
                result.RecordChangeComplete = RecordChangeComplete;
            }
            if (WillChangeField != null)
            {
                result.WillChangeField = WillChangeField;
            }
            if (WillChangeRecord != null)
            {
                result.WillChangeRecord = WillChangeRecord;
            }
            if (result.currentView.Count > 0)
            {
                result.index = 0;
                result.eof = false;
            }
            return result;
        }

        #region Open Operations

        /// <summary>
        /// Opens the ADORecordsetHelper and requeries according to the value of “requery” parameter.
        /// </summary>
        /// <param name="requery">Indicates if a requery most be done.</param>
        public override void Open(bool requery)
        {
            // This is done to support disconnected recordSet operations.
            if (SupportsDisconnectedRecordsetOperations)
            {
                //According to tests, in VB6 a disconnected recordset changes its state to LockBatchOptimistic when opened
                if (ActiveConnection == null && activeCommand == null)
                {
                    LockType = LockTypeEnum.adLockBatchOptimistic;
                }                
            }
            base.Open(requery);
        }

        /// <summary>
        /// Performs a check to determine if the recordset is working disconnected
        /// </summary>
        /// <returns></returns>
        private bool SupportsDisconnectedRecordsetOperations
        {
            get
            {
                return (ActiveConnection == null && activeCommand == null) ||
                       (ActiveConnection == null && activeCommand != null && 
                       activeCommand.Connection == null);
            }
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>
        public void Open(LockTypeEnum lockType)
        {
            Validate();
            this.lockType = lockType;
            Open();
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="command">A command containing the query to be execute to load the ADORecordsetHelper object.</param>
        public void Open(DbCommand command)
        {
            Open(command, lockType);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper</param>
        public void Open(DbConnection connection)
        {
            Open(connection, lockType);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="str">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="type">StringParameterType of the str.</param>
        public void Open(String str, StringParameterType type)
        {
            Validate();
            if (type == StringParameterType.Source)
            {
                List<DbParameter> parameters;
                CommandType commandType = getCommandType((string)str, out parameters);
                Open(CreateCommand(str, commandType,parameters), lockType);
            }
            else
            {
                Open(GetConnection(str), lockType);
            }
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="command">A command containing the query to be execute to load the ADORecordsetHelper object.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>
        public void Open(DbCommand command, LockTypeEnum lockType)
        {
            Validate();
            source = command;
            activeCommand = command;
            Open(lockType);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>   
        public void Open(DbConnection connection, LockTypeEnum lockType)
        {
            ActiveConnection = connection;
            Open(lockType);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="str">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>
        /// <param name="type">StringParameterType of the str.</param>
        public void Open(String str, LockTypeEnum lockType, StringParameterType type)
        {
            Validate();
            if (type == StringParameterType.Source)
            {
                List<DbParameter> parameters;
                CommandType commandType = getCommandType(str, out parameters);
                Open(CreateCommand(str, commandType,parameters), lockType);
            }
            else
            {
                Open(GetConnection(str), lockType);
            }
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        public void Open(String SQLstr, DbConnection connection)
        {
            ActiveConnection = connection;
            List<DbParameter> parameters;
            CommandType commandType = getCommandType((string)SQLstr, out parameters);
            Open(CreateCommand(SQLstr, commandType,parameters));
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connectionString">Strings that contains information about how to connect to the database.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>   
        public void Open(String SQLstr, String connectionString, LockTypeEnum lockType)
        {
            Open(SQLstr, GetConnection(connectionString), lockType);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connectionString">Strings that contains information about how to connect to the database.</param>
        public void Open(String SQLstr, String connectionString)
        {
            Open(SQLstr, connectionString, LockTypeEnum.adLockBatchOptimistic);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters. 
        /// NOTE: It is better to provide the CommandType when executing the command
        /// If the command type is not given, performance would be affected due to several
        /// request to the DB schema
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>   
        public void Open(String SQLstr, DbConnection connection, LockTypeEnum lockType)
        {
            ActiveConnection = connection;
            List<DbParameter> parameters;
            CommandType commandType = getCommandType((string)SQLstr, out parameters);
            Open(SQLstr, connection, lockType, commandType, parameters);
        }

        /// <summary>
        /// Opens this ADORecordsetHelper using the provided parameters.
        /// This is the preferred Open method for performance reasons. However this call might required
        /// some extra parameters like CommandType and ParameterList.
        /// For most scenerios just provide a null parameter for the parameter list;
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="lockType">The LockTypeEnum of this ADORecordsetHelper object.</param>   
        /// <param name="commandType">The CommandType of this ADORecordsetHelper object.</param>   
        /// <param name="parameters">The list of parameters.</param>   
        public void Open(String SQLstr, DbConnection connection, LockTypeEnum lockType, CommandType commandType, List<DbParameter> parameters)
        {
            // A RecordSet can be openned with a series of staments separated by ;. However each type an open is done, this collection is reseted */
            commands = null;
            ActiveConnection = connection;
            Open(CreateCommand(SQLstr, commandType, parameters), lockType);
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="recordsAffected">Out parameter indicating the amount of records affected by the execution of the “SQLstr” query.</param>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <returns>The new ADORecordsetHelper object.</returns>
        public static ADORecordSetHelper Open(String SQLstr, DbConnection connection, out long recordsAffected, String factoryName)
        {
            ADORecordSetHelper result = new ADORecordSetHelper(factoryName);
            result.Open(SQLstr, connection);
            recordsAffected = result.RecordCount;
            return result;
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <returns>The new ADORecordsetHelper object.</returns>
        public static ADORecordSetHelper Open(String SQLstr, DbConnection connection, String factoryName)
        {
            long recordsAffected;
            return ADORecordSetHelper.Open(SQLstr, connection, out recordsAffected, factoryName);
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="SQLstr">The string containing the SQL query to be loaded into this ADORecodsetHelper object.</param>
        /// <param name="connection">Connection object to be use by this ADORecordsetHelper.</param>
        /// <param name="factory">The DBProviderFactory to be used on the ADORecordsetHelper.</param>
        /// <returns>The new ADORecordsetHelper object.</returns>
        public static ADORecordSetHelper Open(String SQLstr, DbConnection connection, DbProviderFactory factory)
        {
            return ADORecordSetHelper.Open(SQLstr, connection, factory);
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="command">A command containing the query to be execute to load the ADORecordsetHelper object.</param>
        /// <param name="factoryName">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
        /// <returns></returns>
        public static ADORecordSetHelper Open(DbCommand command, String factoryName)
        {
            long recordsAffected;
            return ADORecordSetHelper.Open(command, out recordsAffected, factoryName);
        }

        /// <summary>
        /// Creates a new ADORecordsetHelper using the provided parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="recordsAffected"></param>
        /// <param name="factoryName"></param>
        /// <returns></returns>
        public static ADORecordSetHelper Open(DbCommand command, out long recordsAffected, string factoryName)
        {
            ADORecordSetHelper recordSet = new ADORecordSetHelper(factoryName);
            recordSet.Open(command);
            recordsAffected = recordSet.RecordCount;
            return recordSet;
        }

        #endregion

        /// <summary>
        /// Updates the data in a Recordset object by re-executing the query on which the object is based.
        /// </summary>
        public void Refresh()
        {
            Requery();
        }

        /// <summary>
        /// Sets the “value” to the column at index “ColumnIndex”.
        /// </summary>
        /// <param name="columnIndex">Index of the column to update.</param>
        /// <param name="value">The new value for the column.</param>
        public override void SetNewValue(int columnIndex, object value)
        {
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            string[] errors = null;
            if (firstChange)
            {
                OnWillChangeRecord(EventReasonEnum.adRsnFirstChange, ref status, 1);
            }
            OnWillChangeField(ref status, 1, new object[] { CurrentRow[columnIndex] });
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    base.SetNewValue(columnIndex, value);
                }
                catch (Exception e)
                {
                    status = EventStatusEnum.adStatusErrorsOccurred;
                    errors = new string[] { e.Message };
                }
                OnFieldChangeComplete(ref status, 1, new object[] { CurrentRow[columnIndex] }, errors);
                if (firstChange)
                {
                    OnRecordChangeComplete(EventReasonEnum.adRsnFirstChange, ref status, 1, errors);
                }
            }
        }

        #region Events

        /// <summary>
        /// The EndOfRecordset event is called when there is an attempt to move to a row past the end of the Recordset.
        /// </summary>
        /// <param name="moredata">Bool value that indicates if more data have been added to the ADORecordsetHelper.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnEndOfRecordset(ref bool moredata, EventStatusEnum status)
        {
            if (EndOfRecordset != null)
            {
                EndOfRecordsetEventArgs eor = new EndOfRecordsetEventArgs(moredata, status);
                EndOfRecordset(this, eor);
                moredata = eor.MoreData;
            }
        }

        /// <summary>
        /// The WillChangeField event is called before a pending operation changes the value of one or more Field objects in the ADORecordsetHelper.
        /// </summary>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numfields">Indicates the number of fields objects contained in the “fieldvalues” array.</param>
        /// <param name="fieldvalues">Array with the new values of the modified fields.</param>
        protected void OnWillChangeField(ref EventStatusEnum status, int numfields, object[] fieldvalues)
        {
            if (WillChangeField != null)
            {
                FieldChangeEventArgs args = new FieldChangeEventArgs(numfields, fieldvalues, status);
                WillChangeField(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// The FieldChangeComplete event is called after the value of one or more Field objects has changed.
        /// </summary>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numfields">Indicates the number of fields objects contained in the “fieldvalues” array.</param>
        /// <param name="fieldvalues">Array with the new values of the modified fields.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnFieldChangeComplete(ref EventStatusEnum status, int numfields, object[] fieldvalues, string[] errors)
        {
            if (FieldChangeComplete != null)
            {
                FieldChangeCompleteEventArgs args = new FieldChangeCompleteEventArgs(numfields, fieldvalues, errors, status);
                FieldChangeComplete(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// The OnWillChangeRecord event is called before one or more records (rows) in the Recordset change.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numRecords">Value indicating the number of records changed (affected).</param>
        protected void OnWillChangeRecord(EventReasonEnum reason, ref EventStatusEnum status, int numRecords)
        {
            if (WillChangeRecord != null)
            {
                RecordChangeEventArgs args = new RecordChangeEventArgs(reason, numRecords, status);
                WillChangeRecord(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// OnRecordChangeComplete event is called after one or more records change.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="numRecords">Value indicating the number of records changed (affected).</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnRecordChangeComplete(EventReasonEnum reason, ref EventStatusEnum status, int numRecords, string[] errors)
        {
            if (RecordChangeComplete != null)
            {
                RecordChangeCompleteEventArgs args = new RecordChangeCompleteEventArgs(reason, numRecords, errors, status);
                RecordChangeComplete(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// OnWillChangeRecordset event is called before a pending operation changes the ADORecordsetHelper.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnWillChangeRecordset(EventReasonEnum reason, ref EventStatusEnum status)
        {
            if (WillChangeRecordset != null)
            {
                RecordSetChangeEventArgs args = new RecordSetChangeEventArgs(reason, status);
                WillChangeRecordset(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// OnRecordsetChangeComplete event is called after the ADORecordsetHelper has changed.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnRecordsetChangeComplete(EventReasonEnum reason, ref EventStatusEnum status, string[] errors)
        {
            if (RecordsetChangeComplete != null)
            {
                RecordSetChangeCompleteEventArgs args = new RecordSetChangeCompleteEventArgs(reason, errors, status);
                RecordsetChangeComplete(this, args);
                status = args.Status;
            }
        }

        /// <summary>
        /// OnWillMove event is called before a pending operation changes the current position in the ADORecordsetHelper.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        protected void OnWillMove(EventReasonEnum reason, ref EventStatusEnum status)
        {
            firstChange = true;
            if (WillMove != null)
            {
                MoveEventArgs args = new MoveEventArgs(reason, status);
                WillMove(this, args);
                status = args.Status;
                firstChange = status == EventStatusEnum.adStatusCancel ? false : true;
            }
        }

        /// <summary>
        /// OnMoveComplete event is called after the current position in the ADORecordsetHelper changes.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        /// <param name="errors">Array containing all the errors occurred during the field change.</param>
        protected void OnMoveComplete(EventReasonEnum reason, ref EventStatusEnum status, string[] errors)
        {
            if (MoveComplete != null)
            {
                MoveCompleteEventArgs args = new MoveCompleteEventArgs(reason, errors, status);
                MoveComplete(this, args);
                status = args.Status;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs the basic move operation on the ADORecordsetHelper, moving the current record forward or backwards.
        /// </summary>
        /// <param name="newIndex">The index of the new position for the current record.</param>
        protected override void BasicMove(int newIndex)
        {
            index = newIndex;
//AIS TODO OLD-CODE
//            eof = index >= (UsingView ? currentView.Count : Tables[0].Rows.Count);
//NEW CODE 
            eof = this.IsEof();
            base.BasicMove(newIndex);
        }

        /// <summary>
        /// Determines if we should be at the end of file (EOF) based on the current index.
        /// </summary>
        /// <returns>Returns true if based on the index variable EOF is true; otherwise false.</returns>
        private bool IsEof()
        {
            bool isEof = this.index < 0;
            if (this.UsingView)
            {
                isEof = (this.index < 0) || (this.index >= this.currentView.Count);
            }
            else if (this.Tables.Count > 0)
            {
                isEof = (this.index < 0) || (this.index >= this.Tables[0].Rows.Count);
            }
            return isEof;
        }

        /// <summary>
        /// Verifies that no more data is pending on the ADORecordsetHelper.
        /// </summary>
        private void EndOfRecordsetLogic()
        {
            bool moredata = false;
            OnEndOfRecordset(ref moredata, EventStatusEnum.adStatusOK);
            if (!moredata)
            {
                eof = true;
                index = UsingView ? currentView.Count - 1 : Tables[0].Rows.Count - 1;
            }
            else
            {
                eof = false;
            }
        }

        /// <summary>
        /// Move the current record according to the value of “records”.
        /// </summary>
        /// <param name="records">The number of records to move forward (if positive), backwards (if negative).</param>
        /// <param name="reason">The reason of the change.</param>
        /// <param name="status">An EventStatusEnum value that indicates the state of the ADORecordsetHelper in the moment that the event rose.</param>
        private void Move(int records, EventReasonEnum reason, EventStatusEnum status)
        {            
			//if ((!UsingView && Tables[0].Rows.Count == 0) || (UsingView && currentView.Count == 0))
			//{
			//	throw new InvalidOperationException("Requested operation requires a current record");
			//}
            OnWillMove(reason, ref status);
            string[] errors = null;
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    if (records != 0 && index >= 0)
                    {
                        Update(false);
                    }
                    BasicMove(index + records);
                    if (eof && reason != EventReasonEnum.adRsnMoveFirst && reason != EventReasonEnum.adRsnMovePrevious)
                    {
                        EndOfRecordsetLogic();
                    }
                    //else
                    //{
                    //    eof = false;
                    //}
                }
                catch (Exception e)
                {
                    errors = new string[] { e.Message };
                    status = EventStatusEnum.adStatusErrorsOccurred;
                }
                OnMoveComplete(reason, ref status, errors);
            }
        }

        /// <summary>
        /// Move the current record to the beginning of the ADORecordsetHelper object.
        /// </summary>
        /// <param name="reason">The reason of the change.</param>
        private void MoveFirst(EventReasonEnum reason)
        {
            if (index == -1)
            {
                index = 0;
            }
            Move((-1 * index), reason, EventStatusEnum.adStatusOK);
        }

        /// <summary>
        /// Saves the current content of the ADORecordsetHelper to the database.
        /// </summary>
        /// <param name="reportMove">Bool flag that indicates if this operation will notify others process raising an event or not.</param>
        private void Update(bool reportMove)
        {
            Exception exceptionToThrow = null;
            EventStatusEnum status = EventStatusEnum.adStatusOK;
            string[] errors = null;
            OnWillChangeRecordset(EventReasonEnum.adRsnMove, ref status);
            if (status != EventStatusEnum.adStatusCancel)
            {
                DataRow theRow = CurrentRow;
                if (newRow)
                {
                    newRow = false;
                }
                if (theRow.RowState != DataRowState.Unchanged)
                {
                    if (!isBatchEnabled())
                    {
                        if (UsingView)
                        {
                            dbvRow.EndEdit();
                            index = findBookmarkIndex(dbvRow.Row);
                        }
                        status = EventStatusEnum.adStatusOK;
                        OnWillChangeRecord(EventReasonEnum.adRsnUpdate, ref status, 1);
                        if (status != EventStatusEnum.adStatusCancel)
                        {
                            try
                            {
                                UpdateWithNoEvents(theRow);
                                editMode = EditModeEnum.adEditNone;
                            }
                            catch (Exception e)
                            {
                                errors = new string[] { e.Message };
                                exceptionToThrow = e;
                            }
                            OnRecordChangeComplete(EventReasonEnum.adRsnUpdate, ref status, 1, errors);
                        }
                    }
                }
                OnRecordsetChangeComplete(EventReasonEnum.adRsnMove, ref status, errors);
                if (exceptionToThrow != null)
                {
                    throw exceptionToThrow;
                }
                if (reportMove)
                {
                    Move(0, EventReasonEnum.adRsnMove, EventStatusEnum.adStatusOK);
                }
            }
        }

        /// <summary>
        /// Opens the ADORecordsetHelper using the object public information.
        /// </summary>
        /// <param name="requery">Flag that indicates if a requery is necessary.</param>
        protected override void OpenRecordset(bool requery)
        {
            EventStatusEnum status = requery ? EventStatusEnum.adStatusOK : EventStatusEnum.adStatusCantDeny;
            OnWillMove(requery ? EventReasonEnum.adRsnRequery : EventReasonEnum.adRsnMove, ref status);
            if (requery)
            {
                OnMoveComplete(EventReasonEnum.adRsnRequery, ref status, null);
            }
            string[] errors = null;
            int records = 0;
            if (status != EventStatusEnum.adStatusCancel)
            {
                try
                {
                    ProcessCompoundStatement();
                    base.OpenRecordset(requery);
                    status = EventStatusEnum.adStatusOK;
                    if (Tables.Count == 1 && Tables[0].Columns.Count > 0)
                    {
                        State = ConnectionState.Open;
                    }
                }
                catch (Exception e)
                {
                    if (RecordsetChangeComplete != null || MoveComplete != null)
                    {
                        status = EventStatusEnum.adStatusErrorsOccurred;
                        errors = new string[] { e.Message };
                    }
                    else
                    {
                        throw e;
                    }
                }
                if (requery && records > 0)
                {
                    OnRecordsetChangeComplete(EventReasonEnum.adRsnClose, ref status, errors);
                    Move(0, EventReasonEnum.adRsnMove, EventStatusEnum.adStatusCantDeny);
                }
                if (!requery)
                {
                    OnMoveComplete(requery ? EventReasonEnum.adRsnRequery : EventReasonEnum.adRsnMove, ref status, errors);
                }
            }
        }

        private void ProcessCompoundStatement()
        {
            if (activeCommand != null)
            {
                string[] commandTexts = activeCommand.CommandText.Split(";".ToCharArray());
                if (commandTexts.Length > 1)
                {
                    foreach (string commandText in commandTexts)
                    {
                        if (commandText.Trim() == String.Empty)
                        {
                            continue;
                        }
                        DbCommand tempcommand = ActiveConnection.CreateCommand();
                        tempcommand.Transaction = activeCommand.Transaction;
                        tempcommand.CommandText = commandText;
                        if (commands == null)
                        {
                            commands = new Queue<DbCommand>(commandTexts.Length);
                        }
                        commands.Enqueue(tempcommand);
                    }
                    activeCommand = commands.Dequeue();
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns a new recordset according to the compound statement on the current recordset
        /// </summary>
        /// <returns>A new open recordset</returns>
        public ADORecordSetHelper NextRecordSet()
        {
            ADORecordSetHelper result = null;
            if (commands.Count > 0)
            {
                result = new ADORecordSetHelper();
                result.ProviderFactory = ProviderFactory;
                result.DatabaseType = DatabaseType;
                result.Open(commands.Dequeue());
                result.commands = commands;
            }
            return result;
        }

        /// <summary>
        /// Overrides the IDisposable.Dispose to cleanup
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (ActiveConnection != null)
                ActiveConnection.StateChange -= new StateChangeEventHandler(ActiveConnection_StateChange);
            base.Dispose(disposing);
        }

    }
}