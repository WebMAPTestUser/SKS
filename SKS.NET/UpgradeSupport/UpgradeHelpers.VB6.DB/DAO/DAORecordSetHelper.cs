using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using UpgradeHelpers.VB6.DB;
using UpgradeHelpers.VB6.DB.Controls;
using UpgradeHelpers.VB6.DB.DAO;

namespace UpgradeHelpers.VB6.DB.DAO
{
	#region Enum declaration

	/// <summary>
	/// Determines the type of the recordset.
	/// It will be used on OpenRecordset method.
	/// </summary>
	public enum DAORecordsetTypeEnum
	{
		/// <summary>
		/// Open table
		/// </summary>
		dbOpenTable = 1,
		/// <summary>
		/// Dynamic
		/// </summary>
		dbOpenDynamic = 16,
		/// <summary>
		/// Dynaset
		/// </summary>
		dbOpenDynaset = 2,
		/// <summary>
		/// Snapshot
		/// </summary>
		dbOpenSnapshot = 4,
		/// <summary>
		/// Forward Only
		/// </summary>
		dbOpenForwardOnly = 8
	}

	/// <summary>
	/// Determines the way a recordset will be accessed setting some restriction or permitions.
	/// It will be used on OpenRecordset, and Execute methods.
	/// </summary>
	public enum DAORecordsetOptionEnum
	{
		/// <summary>Allows user to add new records to the dynaset, but prevents user from reading existing records.</summary>
		dbAppendOnly = 8,
		/// <summary>Applies updates only to those fields that will not affect other records in the dynaset (dynaset- and snapshot-type only).</summary>
		dbConsistent = 32,
		/// <summary>Prevents other users from reading Recordset records (table-type only).</summary>
		dbDenyRead = 2,
		/// <summary>Prevents other users from changing Recordset records.</summary>
		dbDenyWrite = 1,
		/// <summary>Executes the query without first calling the SQLPrepare ODBC function.</summary>
		dbExecDirect = 2048,
		/// <summary>Rolls back updates if an error occurs.</summary>
		dbFailOnError = 128,
		/// <summary>Creates a forward-only scrolling snapshot-type Recordset (snapshot-type only).</summary>
		dbForwardOnly = 256,
		/// <summary>Applies updates to all dynaset fields, even if other records are affected (dynaset- and snapshot-type only).</summary>
		dbInconsistent = 16,
		/// <summary>Opens the Recordset as read-only.</summary>
		dbReadOnly = 4,
		/// <summary>Executes the query asynchronously.</summary>
		dbRunAsync = 1024,
		/// <summary>Generates a run-time error if another user is changing data you are editing (dynaset-type only).</summary>
		dbSeeChanges = 512,
		/// <summary>Sends an SQL statement to an ODBC database (snapshot-type only).</summary>
		dbSQLPassThrough = 64
	}

	/// <summary>Sets or returns the type of locking (concurrency) to use.</summary>
	public enum DAOLockTypeEnum
	{
		/// <summary>Optimistic concurrency based on record ID. Cursor compares record ID in old and new records to determine if changes have been made since the record was last accessed.</summary>
		dbOptimistic = 3,
		/// <summary>Enables batch optimistic updates (ODBCDirect workspaces only).</summary>
		dbOptimisticBatch = 5,
		/// <summary>Optimistic concurrency based on record values. Cursor compares data values in old and new records to determine if changes have been made since the record was last accessed (ODBCDirect workspaces only).</summary>
		dbOptimisticValue = 1,
		/// <summary>Pessimistic concurrency. Cursor uses the lowest level of locking sufficient to ensure that the record can be updated.</summary>
		dbPessimistic = 2
	}

	/// <summary>Sets or returns the type of update to use.</summary>
	public enum DAOUpdateTypeEnum
	{
		/// <summary>All pending changes in the update cache are written to disk.</summary>
		dbUpdateBatch = 4,
		/// <summary>Only the current record's pending changes are written to disk.</summary>
		dbUpdateCurrentRecord = 2,
		/// <summary>(Default) Pending changes are not cached and are written to disk immediately.</summary>
		dbUpdateRegular = 1
	}

	#endregion

	/// <summary>
	/// Support class for the DAO.Recorset the object that represents the records in a base table or the records that result from running a query.
	/// </summary>
	public class DAORecordSetHelper : RecordSetHelper
	{
		#region Class Variables
		/// <summary>Lock editing operations for the current object.</summary>
		private EditModeEnum editMode = EditModeEnum.dbEditNone;
		/// <summary>Indicates the result of a seek or find operation.</summary>
		private bool noMatch = false;
		/// <summary>Indicates the type of this recordset.</summary>
		private DAORecordsetTypeEnum daoRSType = DAORecordsetTypeEnum.dbOpenDynamic; //default
		/// <summary>Indicates the options for this recorset.</summary>
		private DAORecordsetOptionEnum daoRSOption;
		/// <summary>Indicates the lock for this recordset.</summary>
		private DAOLockTypeEnum daoLockType = DAOLockTypeEnum.dbOptimistic; //default


		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new DAORecordSet instance using the default factory specified on the configuration xml.
		/// </summary>
		public DAORecordSetHelper()
			: this("")
		{ }

		/// <summary>
		/// Creates a new DAORecordSet instance using the factory specified on the “factoryName” parameter.
		/// </summary>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		public DAORecordSetHelper(String factoryName)
			: base(factoryName)
		{ }

		/// <summary>
		/// Creates a new DAORecordSet instance using the factory specified on the “factoryName” and the configuration provided by the other parameters.
		/// </summary>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <param name="rsType">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="rsOption">The DAORecordsetOptionEnum of this DAORecordsetHelper object.</param>
		/// <param name="lockType">The DAOLockTypeEnum of this DAORecordsetHelper object.</param>
		public DAORecordSetHelper(String factoryName, DAORecordsetTypeEnum rsType, DAORecordsetOptionEnum rsOption, DAOLockTypeEnum lockType)
			: base(factoryName)
		{
			this.daoRSType = rsType;
			this.daoRSOption = rsOption;
			this.daoLockType = lockType;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets and sets a bookmark to an specific record inside the ADORecordsetHelper.
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
		/// Gets and Sets the position of the current record on the recordset instance.
		/// </summary>
		public int AbsolutePosition
		{
			get
			{
				return index;
			}
			set
			{
                BasicMove(value);
			}
		}

		/// <summary>
		/// Returns a value that indicates whether the current record position is before the first record in a DAORecordsetHelper object. Read-only Boolean.
		/// </summary>
		public override bool BOF
		{
			get
			{
				return index < 0;
			}
		}

		/// <summary>
		/// Gets or Sets if lock is in effect while editing.  
		/// </summary>
		public bool LockEdits
		{
			get
			{
				return editMode == EditModeEnum.dbEditNone;
			}
			set
			{
				if (value)
				{
					editMode = EditModeEnum.dbEditInProgress;
				}
				else
					editMode = EditModeEnum.dbEditNone;
			}
		}

		/// <summary>
		/// Indicates whether a particular record was found by using the Seek method or one of the Find methods.
		/// </summary>
		public bool NoMatch
		{
			get
			{
				return noMatch;
			}
		}

		/// <summary>
		/// Gets or sets the type for this DAORecordSetHelper object.
		/// </summary>
		public DAORecordsetTypeEnum Type
		{
			get
			{
				return daoRSType;
			}
			set
			{
				daoRSType = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new record.
		/// </summary>
		public override void AddNew()
		{
			doAddNew();
			editMode = EditModeEnum.dbEditAdd;
		}

		/// <summary>
		/// Sets the recordset on edit mode.
		/// </summary>
		public void Edit()
		{
			editMode = EditModeEnum.dbEditNone;
		}

		/// <summary>
		/// Returns a two dimmension array representing 'n' rows in a result set.
		/// </summary>
		/// <param name="numrows">Number of rows to be retrieved.</param>
		/// <returns>A delimited string containing a number of rows.</returns>
		public object[,] GetRows(int numrows)
		{
			object[,] buffer = new object[Tables[0].Columns.Count, numrows];
			int i = index, colindex = 0, rowindex = 0;
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
			object[,] result = new object[Tables[0].Columns.Count, rowindex];
			for (int rindex = 0; rindex < rowindex; rindex++)
				for (int cindex = 0; cindex < Tables[0].Columns.Count; cindex++)
					result[cindex, rindex] = buffer[cindex, rindex];
			return result;
		}

		/// <summary>
		/// Moves the position of the currentRecord in a RecordSet.
		/// </summary>
		/// <param name="rows">The number of rows the position will move. If rows is greater than 0, the position is moved forward (toward the end of the file). If rows is less than 0, the position is moved backward (toward the beginning of the file).</param>
		/// <param name="startBookmark">The start value to begin the move.</param>
		public void Move(int rows, DataRow startBookmark)
		{
			int tempBookmark = findBookmarkIndex(startBookmark);
			index = tempBookmark;
			Move(rows);
		}

		/// <summary>
		/// Locates the first record in DAORecordsetHelper object that satisfies the specified criteria and makes that record the current record.
		/// </summary>
		/// <param name="criteria">A String used to locate the record. It is like the WHERE clause in an SQL statement, but without the word WHERE.</param>
		public void FindFirst(string criteria)
		{
			noMatch = true;
			// Save the current position. 
			int indexPositionBeforeSearch = index;

			// Apply the filter criteria to the data. 
			DataView result = Tables[0].DefaultView;
			result.RowFilter = criteria;

			if (result.Count > 0)
			{
				int tempIndex = findBookmarkIndex(result[0].Row);
				index = tempIndex;
				noMatch = false;
			}
		}

		/// <summary>
		/// Locates the last record in DAORecordsetHelper object that satisfies the specified criteria and makes that record the current record.
		/// </summary>
		/// <param name="criteria">A String used to locate the record. It is like the WHERE clause in an SQL statement, but without the word WHERE.</param>
		public void FindLast(string criteria)
		{
			noMatch = true;
			// Save the current position. 
			int indexPositionBeforeSearch = index;

			// Apply the filter criteria to the data. 
			DataView result = Tables[0].DefaultView;
			result.RowFilter = criteria;

			if (result.Count > 0)
			{
				int tempIndex = findBookmarkIndex(result[result.Count - 1].Row);
				index = tempIndex;
				noMatch = false;
			}
		}

		/// <summary>
		/// Opens the DAORecordsetHelper object by executing the query in the “command” parameter and load all results.
		/// </summary>
		/// <param name="command">A command containing the query to be execute to load the DAORecordsetHelper object.</param>
		public void Open(DbCommand command)
		{
			Validate();
			source = command;
			activeCommand = command;
			Open();
		}

		/// <summary>
		/// Opens the DAORecordsetHelper object by executing the query on the “SQLstr” using the connection object provided has parameter and load all results.
		/// </summary>
		/// <param name="SQLstr">The string containing the SQL query to be loaded into this DAORecodsetHelper object.</param>
		/// <param name="connection">Connection object to be use by this DAORecordsetHelper.</param>
		public void Open(String SQLstr, DbConnection connection)
		{
			ActiveConnection = connection;
			List<DbParameter> parameters;
			CommandType commandType = getCommandType((string)SQLstr, out parameters);
			Open(CreateCommand(SQLstr, commandType, parameters));
		}

		/// <summary>
		/// Creates a new DAORecordsetHelper object using the “factoryName” and opens it by executing the query on the “SQLstr” using the connection object provided has parameter and load all results.
		/// </summary>
		/// <param name="SQLStr">The string containing the SQL query to be loaded into this DAORecodsetHelper object.</param>
		/// <param name="connection">Connection object to be use by this DAORecordsetHelper.</param>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <returns></returns>
		public static DAORecordSetHelper Open(string SQLStr, DbConnection connection, string factoryName)
		{
			DAORecordSetHelper recordSet = new DAORecordSetHelper(factoryName);
			recordSet.Open(SQLStr, connection);
			return recordSet;
		}

		/// <summary>
		/// Creates a new DAORecordsetHelper object using the “factoryName”, “type” and opens it by executing the query on the “SQLstr” using the connection object provided has parameter and load all results.
		/// </summary>
		/// <param name="SQLStr">The string containing the SQL query to be loaded into this DAORecodsetHelper object.</param>
		/// <param name="type">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="connection">Connection object to be use by this DAORecordsetHelper.</param>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <returns>The new DAORecordsetHelper object.</returns>
		public static DAORecordSetHelper Open(string SQLStr, DAORecordsetTypeEnum type, DbConnection connection, string factoryName)
		{
			// Type not used.
			return Open(SQLStr, connection, factoryName);
		}

		/// <summary>
		/// Creates a new DAORecordsetHelper object using the “factoryName”, “type”, "options" and opens it by executing the query on the “SQLstr” using the connection object provided has parameter and load all results.
		/// </summary>
		/// <param name="SQLStr">The string containing the SQL query to be loaded into this DAORecodsetHelper object.</param>
		/// <param name="type">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="options">The DAORecordsetOptionEnum of this DAORecordsetHelper object.</param>
		/// <param name="connection">Connection object to be use by this DAORecordsetHelper.</param>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <returns>The new DAORecordsetHelper object.</returns>
		public static DAORecordSetHelper Open(string SQLStr, DAORecordsetTypeEnum type, DAORecordsetOptionEnum options, DbConnection connection, string factoryName)
		{
			// Type not used.
			// Options not used.
			return Open(SQLStr, connection, factoryName);
		}

		/// <summary>
		/// Creates a new DAORecordsetHelper object using the “factoryName”, “type”, "options", "lockType"  and opens it by executing the query on the “SQLstr” using the connection object provided has parameter and load all results.
		/// </summary>
		/// <param name="SQLStr">The string containing the SQL query to be loaded into this DAORecodsetHelper object.</param>
		/// <param name="type">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="options">The DAORecordsetOptionEnum of this DAORecordsetHelper object.</param>
		/// <param name="lockType">The DAOLockTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="connection">Connection object to be use by this DAORecordsetHelper.</param>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <returns>The new DAORecordsetHelper object.</returns>
		public static DAORecordSetHelper Open(string SQLStr, DAORecordsetTypeEnum type, DAORecordsetOptionEnum options, DAOLockTypeEnum lockType, DbConnection connection, string factoryName)
		{
			DAORecordSetHelper recordSet = new DAORecordSetHelper(factoryName, type, options, lockType);
			recordSet.Open(SQLStr, connection);
			return recordSet;
		}

		/// <summary>
		/// Creates a new DAORecordsetHelper object using the “factoryName” and opens it by executing the query in the “command” parameter and load all results.
		/// </summary>
		/// <param name="command">A command containing the query to be execute to load the DAORecordsetHelper object.</param>
		/// <param name="factoryName">The name of the factory to by use by this DAORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <returns>The new DAORecordsetHelper object.</returns>
		public static DAORecordSetHelper Open(DbCommand command, String factoryName)
		{
			DAORecordSetHelper recordSet = new DAORecordSetHelper(factoryName);
			recordSet.Open(command);
			return recordSet;
		}

		/// <summary>
		/// Save the current content of the DAORecordsetHelper to the database.
		/// </summary>
		/// <param name="UpdateType">The DAOUpdateTypeEnum to be use by this update.</param>
		/// <param name="Force">A Boolean value indicating whether or not to force the changes into the database.</param>
		public void Update(DAOUpdateTypeEnum UpdateType, bool Force)
		{
			//note: No case has been found to use the specialization parameters. 
			//if (UpdateType == DAOUpdateTypeEnum.dbUpdateRegular)
			Update();
		}



#if TargetF2
        private object _updateLambda;

        /// <summary>
        /// Extension lambda for overriding default Update logic. This is helpful for
        /// Recordset loaded from Stored Procedures
        /// </summary>
        public object UpdateLambda
        {
            get { return _updateLambda; }
            set { _updateLambda = value; }
        }
        private void UpdateLambdaF2(DataRow theRow){}
#else
		/// <summary>
		/// Extension lambda for overriding default Update logic. This is helpful for
		/// Recordset loaded from Stored Procedures
		/// </summary>
		public Action<DAORecordSetHelper, DataRow> UpdateLambda { get; set; }
#endif


#if TargetF2
        private Action<DAORecordSetHelper> _deleteLambda;

       /// <summary>
	   /// Extension lambda for overriding default Update logic. This is helpful for
	   /// Recordset loaded from Stored Procedures
	   /// </summary>
        public Action<DAORecordSetHelper> DeleteLambda
        {
            get { return _deleteLambda; }
            set { _deleteLambda = value; }
        }
#else
		/// <summary>
		/// Extension lambda for overriding default Update logic. This is helpful for
		/// Recordset loaded from Stored Procedures
		/// </summary>
		public Action<DAORecordSetHelper, DataRow> DeleteLambda { get; set; }
#endif
		/// <summary>
		/// List the posible update cases used with the UpdateInfo
		/// </summary>
		public enum UpdateType
		{
			/// <summary>
			/// Row Adde
			/// </summary>
			Added,
			/// <summary>
			/// Row deleted
			/// </summary>
			Deleted,
			/// <summary>
			/// Row Modified
			/// </summary>
			Modified
		}
		/// <summary>
		/// Event argument class for sending update information to listeners like DataControl helper classes
		/// </summary>
		public class UpdateInfo : EventArgs
		{
			/// <summary>
			/// The update type for the row
			/// </summary>
			public UpdateType UpdateType;
			/// <summary>
			/// Row Index
			/// </summary>
			public int index;
		}

		/// <summary>
		/// Event triggered before an update takes placed
		/// </summary>
		public event EventHandler BeforeUpdate;


		/// <summary>
		/// Event triggered after the updated has been completed
		/// </summary>
		public event EventHandler<UpdateInfo> AfterUpdate;

		/// <summary>
		/// Saves any changes made to the DataRow recieved as parameter.
		/// </summary>
		/// <param name="theRow">The row to be save on the Database.</param>
		protected override void UpdateWithNoEvents(DataRow theRow)
		{
			OnBeforeUpdate();

#if TargetF2
			UpdateInfo info = null; 
#else
			UpdateInfo info = new UpdateInfo() {UpdateType = MatchRowState(theRow), index = this.index};
#endif

            if (UpdateLambda != null || DeleteLambda != null)
			{
				if (UpdateLambda != null && theRow.RowState == DataRowState.Modified)
				{
#if TargetF2
                    UpdateLambdaF2(theRow);
#else
					UpdateLambda(this, theRow);
#endif
				}
				else if (DeleteLambda != null && theRow.RowState == DataRowState.Deleted)
				{
#if TargetF2
                    UpdateLambdaF2(theRow);
#else
					DeleteLambda(this, theRow);
#endif
				}
				else
				{
#if TargetF2
                    UpdateLambdaF2(theRow);
#else
					UpdateLambda(this, theRow);
#endif
				}
			}
			else
			{
				base.UpdateWithNoEvents(theRow);
			}
			OnAfterUpdate(info);
		}

		private UpdateType MatchRowState(DataRow theRow)
		{
			switch (theRow.RowState)
			{
				case DataRowState.Added: return UpdateType.Added;

				case DataRowState.Deleted: return UpdateType.Deleted;
				case DataRowState.Modified: return UpdateType.Modified;
				default:
					throw new NotImplementedException("Not ready for State" + theRow.RowState);
			}
		}

		private void OnAfterUpdate(UpdateInfo info)
		{
			if (AfterUpdate != null)
			{
				AfterUpdate(this, info);
			}
		}

		private void OnBeforeUpdate()
		{
			if (BeforeUpdate != null)
			{
				BeforeUpdate(this, new EventArgs());
			}
		}

		private bool CheckNullState(DataRow row)
		{
			bool empty = true;
			foreach (object col in row.ItemArray)
			{
				if (col != null && !Convert.IsDBNull(col) && !String.IsNullOrEmpty(Convert.ToString(col)))
				{
					empty = false;
					break;
				}
			}
			if (empty)
			{
				index = (index > 0 ? index-- : index);
			}
			return empty;
		}
		/// <summary>
		/// Saves the changes done to the current record on the recordset.
		/// </summary>
		/// <remarks>If the recordset is not batch enabled this method saves the changes on the database.</remarks>
		public override void Update()
		{
			DataRow theRow = CurrentRow;
			if (theRow == null)
			{
				return;
			}
			if (newRow)
			{
				Tables[0].Rows.Add(theRow);
				index = Tables[0].Rows.IndexOf(theRow);
				newRow = false;
				dbRow = null;
			}
			int Action = -1;
			int Save = 0;

			if (theRow.RowState != DataRowState.Unchanged)
			{
				if (theRow.RowState == DataRowState.Added)
				{
					Save = -1;
					editMode = EditModeEnum.dbEditAdd;
					//AIS-TODO: Check if data row is empty do not call database operation
					if (CheckNullState(theRow))
					{
						return;
					}
				}
				OnValidating(ref Save, ref Action);
				if (Action == 0)
				{
					editMode = EditModeEnum.dbEditNone;
					return;
				}
				if (!isBatchEnabled())
				{
					UpdateWithNoEvents(theRow);
				}
			}
			editMode = EditModeEnum.dbEditNone;
		}

		/// <summary>
		/// Moves the current index to the desire position provided has parameter.
		/// </summary>
		/// <param name="newIndex">The new index for the DAORecordsetHelper object.</param>
		protected override void BasicMove(int newIndex)
		{
			index = newIndex < 0 ? 0 : newIndex;
			eof = index >= (UsingView ? currentView.Count : Tables[0].Rows.Count);
			base.BasicMove(newIndex);
		}

		/// <summary>
		/// Moves the current location to the last position in the DAORecordset.
		/// </summary>
		public override void MoveLast()
		{
			BasicMove((UsingView ? currentView.Count - 1 : Tables[0].Rows.Count - 1));
		}

		/// <summary>
		/// Creates and open a new DAORecordsetHelper using the same information of the current object.
		/// </summary>
		/// <returns>A new DAORecordset.</returns>
		public DAORecordSetHelper OpenRS()
		{
			DAORecordSetHelper newRecordSet = new DAORecordSetHelper();
			newRecordSet.activeCommand = this.activeCommand;
			newRecordSet.ActiveConnection = this.ActiveConnection;
			newRecordSet.DatabaseType = this.DatabaseType;
			newRecordSet.ProviderFactory = this.ProviderFactory;
			newRecordSet.Open();
			return newRecordSet;
		}

		/// <summary>
		/// Creates and open a new DAORecordsetHelper using the same information of the current object and the type provided has parameter.
		/// </summary>
		/// <param name="rsType">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <returns>A new DAORecordset.</returns>
		public DAORecordSetHelper OpenRS(DAORecordsetTypeEnum rsType)
		{
			DAORecordSetHelper newRecordSet = new DAORecordSetHelper();
			newRecordSet.daoRSType = rsType;
			newRecordSet.activeCommand = this.activeCommand;
			newRecordSet.ActiveConnection = this.ActiveConnection;
			newRecordSet.DatabaseType = this.DatabaseType;
			newRecordSet.ProviderFactory = this.ProviderFactory;
			newRecordSet.Open();
			return newRecordSet;
		}

		/// <summary>
		/// Creates and open a new DAORecordsetHelper using the same information of the current object and the type provided has parameter.
		/// </summary>
		/// <param name="rsType">The DAORecordsetTypeEnum of this DAORecordsetHelper object.</param>
		/// <param name="rsOptions">The DAORecordsetOptionEnum of this DAORecordsetHelper object.</param>
		/// <returns>A new DAORecordset.</returns>
		public DAORecordSetHelper OpenRS(DAORecordsetTypeEnum rsType, DAORecordsetOptionEnum rsOptions)
		{
			DAORecordSetHelper newRecordSet = new DAORecordSetHelper();
			newRecordSet.daoRSType = rsType;
			newRecordSet.daoRSOption = rsOptions;
			newRecordSet.activeCommand = this.activeCommand;
			newRecordSet.ActiveConnection = this.ActiveConnection;
			newRecordSet.DatabaseType = this.DatabaseType;
			newRecordSet.ProviderFactory = this.ProviderFactory;
			newRecordSet.Open();
			return newRecordSet;
		}

		/// <summary>
		/// Updates the data in a Recordset object by re-executing the query on which the object is based.
		/// </summary>
		public override void Requery()
		{
            if (Tables.Count > 0)
            {
			Tables[0].Rows.Clear();
            }
			base.Requery();
		}

		/// <summary>
		/// Deletes the current record.
		/// </summary>
		public override void Delete()
		{

			try
			{
                this.disableEventsWhileDeleting = false;
				base.Delete();
			}
			finally
			{
				this.disableEventsWhileDeleting = false;
			}
			Update();
		}
		#endregion

		/// <summary>
		/// Event Validating
		/// </summary>
		public event DAODataControlHelper.ValidatingEventHandler Validating;

		/// <summary>
		/// Fires DAORecordSetHelper Validating event that is listened by DAODataControlHelper
		/// </summary>
		/// <param name="Action"></param>
		/// <param name="Save"></param>
		protected virtual void OnValidating(ref int Action, ref int Save)
		{
			if (Validating != null)
			{
				ValidatingEventArgs vArgs = new ValidatingEventArgs(Action, Save);
				Validating(this, vArgs);
				Action = vArgs.Action;
				Save = vArgs.Save;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public DataRow LastModified
		{
			get
			{
				DataTable table = this.Tables[0];
				if (table.Rows.Count > 0)
				{
					return table.Rows[table.Rows.Count - 1];
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int EditMode
		{
			get
			{
				return (int)editMode;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void Refresh()
		{
			Requery();
		}
	}
}
