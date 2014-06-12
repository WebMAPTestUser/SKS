// Author: mrojas
// Project: UpgradeHelpers.VB6.DB
// Path: UpgradeHelpers\VB6\DB
// Creation date: 8/6/2009 2:29 PM
// Last modified: 8/21/2009 10:02 AM

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace UpgradeHelpers.VB6.DB
{
	/// <summary>
	/// String Parameter Type Enum
	/// </summary>
	public enum StringParameterType
	{
		/// <summary>
		/// Source
		/// </summary>
		Source,
		/// <summary>
		/// Connection String
		/// </summary>
		ConnectionString
	}

	/// <summary>
	/// Database Type Enum
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// Oracle
		/// </summary>
		Oracle,
		/// <summary>
		/// SqlServer
		/// </summary>
		SQLServer,
		/// <summary>
		/// MS Access
		/// </summary>
		Access,
		/// <summary>
		/// Not defined
		/// </summary>
		Undefined
	}

	/// <summary>
	/// This is base class for the ADO/RDO/DAORecordsetHelper; 
	/// it provides common functionality for the database access helpers using ADO.NET.
	/// </summary>
	[Serializable]
	[ToolboxItem(false)]
	public class RecordSetHelper : DataSet, ISerializable
	{
		private const string TITLE_DIALOG_RecordSetError = "RecordSet error";
		internal event EventHandler AfterMove;
		internal event EventHandler AfterQuery;

		/// <summary>
		/// Internal variable added to indicate that the recordset is disconnected
		/// </summary>
		protected bool disconnected = false;

		#region Class Variables

		//[NonSerialized]
		private DbProviderFactory providerFactory;
		private DbConnection activeConnection;

		/// <summary>
		/// active command
		/// </summary>
		protected DbCommand activeCommand;

		/// <summary>
		/// current view
		/// </summary>
		protected DataView currentView;

		/// <summary>
		/// new database row
		/// </summary>
		protected DataRow dbRow = null;

		/// <summary>
		/// New Datarow view when adding to a sorted or filtered collection
		/// </summary>
		protected DataRowView dbvRow = null;

		/// <summary>
		/// Connection String
		/// </summary>
		protected String connectionString = string.Empty;

		/// <summary>
		/// actual index
		/// </summary>
		protected int index = -1;

		/// <summary>
		/// new row state
		/// </summary>
		protected bool newRow = false;

		/// <summary>
		/// open state
		/// </summary>
		protected bool opened = false;

		/// <summary>
		/// string for select query
		/// </summary>
		protected String sqlSelectQuery = string.Empty;

		/// <summary>
		/// string for update query
		/// </summary>
		protected String sqlUpdateQuery = string.Empty;

		/// <summary>
		/// string for delete query
		/// </summary>
		protected String sqlDeleteQuery = string.Empty;

		/// <summary>
		/// string for insert query
		/// </summary>
		protected String sqlInsertQuery = string.Empty;

		/// <summary>
		/// actual object source
		/// </summary>
		protected Object source;

		/// <summary>
		/// operation finished state
		/// </summary>
		protected bool operationFinished = false;

		/// <summary>
		/// is filtered?
		/// </summary>
		private bool filtered = false;

		/// <summary>
		/// is first End Of File?
		/// </summary>
		protected bool firstEOF = true;

		/// <summary>
		/// is first change?
		/// </summary>
		protected bool firstChange = true;

		/// <summary>
		/// is end of file
		/// </summary>
		protected bool eof = true;

		/// <summary>
		/// is deserilized
		/// </summary>
		protected bool isDeserialized = false;

		/// <summary>
		/// has auto increment columns
		/// </summary>
		protected bool isDefaultSerializationInProgress = true;

		/// <summary>
		/// has auto increment columns
		/// </summary>
		protected bool hasAutoincrementCols = false;

		/// <summary>
		/// auto increment column name
		/// </summary>
		protected string autoIncrementCol = String.Empty;

		/// <summary>
		/// actual Connection State
		/// </summary>
		protected ConnectionState connectionStateAtEntry = ConnectionState.Closed;

		/// <summary>
		/// page size
		/// </summary>
		protected int pagesize;

		/// <summary>
		/// actual filter object
		/// </summary>
		protected object filter;

		private DatabaseType dbType;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new RecordsetHelper with the default initialization.
		/// </summary>
		protected RecordSetHelper()
			: base()
		{
			isDefaultSerializationInProgress = false;
		}

		/// <summary>
		/// Creates a new RecordsetHelper using the provided DBProviderFactory.
		/// </summary>
		/// <param name="factory">DBProviderFactory instance to be used by internal variable.</param>
		protected RecordSetHelper(DbProviderFactory factory)
			: base()
		{
			isDefaultSerializationInProgress = false;
			providerFactory = factory;
		}

		/// <summary>
		/// Creates a new RecordsetHelper using the provided factory.
		/// </summary>
		/// <param name="factoryname">The name for the factory to be used by this ADORecordsetHelper object (the name must exist on the configuration xml file).</param>
		protected RecordSetHelper(string factoryname)
			: this(AdoFactoryManager.GetFactory(factoryname))
		{
			isDefaultSerializationInProgress = false;
			index = -1;
			newRow = false;
			DatabaseType = AdoFactoryManager.GetFactoryDbType(factoryname);
		}

		/// <summary>
		/// Creates a new RecordsetHelper using the provided parameters.
		/// </summary>
		/// <param name="factoryname">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <param name="connString">The connection string to be used by this ADORecordsetHelper.</param>
		protected RecordSetHelper(String factoryname, String connString)
			: this(factoryname)
		{
			isDefaultSerializationInProgress = false;
			connectionString = connString;
		}

		/// <summary>
		/// Creates a new RecordsetHelper using the provided parameters.
		/// </summary>
		/// <param name="factoryname">The name of the factory to by use by this ADORecordsetHelper object (the name most exist on the configuration xml file).</param>
		/// <param name="connString">The connection string to be used by this ADORecordsetHelper.</param>
		/// <param name="sqlSelectString">A string containing the SQL Query to be loaded on the ADORecordsetHelper.</param>
		protected RecordSetHelper(String factoryname, String connString, String sqlSelectString)
			: this(factoryname, connString)
		{
			isDefaultSerializationInProgress = false;
			this.SqlQuery = sqlSelectString;
			Open();
		}

		#endregion

		#region Open related

		/// <summary>
		/// Opens the connection and initialize the RecordsetHelper object.
		/// </summary>
		public virtual void Open()
		{
			try
			{
				Open(false);
			} // try
			catch (Exception ex)
			{
				if (InDesignMode)
				{
					MessageBox.Show("There was a problem opening the recordset. Please verify connection string", TITLE_DIALOG_RecordSetError);
				}
				else
				{
					throw ex;
				} // if
			} // catch
		}

		/// <summary>
		/// Opens the RecordsetHelper and requeries according to the value of “requery” parameter.
		/// </summary>
		/// <param name="requery">Indicates if a requery most be done.</param>
		public virtual void Open(bool requery)
		{
			if (!requery)
			{
				Validate();
			}
			if (activeCommand == null && (source is String))
			{
				List<DbParameter> parameters;
				CommandType commandType = getCommandType((string)source, out parameters);
				activeCommand = CreateCommand((string)source, commandType, parameters);
			}
			if (ActiveConnection == null && activeCommand != null && activeCommand.Connection != null)
			{
				ActiveConnection = activeCommand.Connection;
			}

			OpenRecordset(requery);
		}

		private static bool loadSchema = true;

		/// <summary>
		/// This is property is used when the record set uses more than one table,
		/// uses CommandBuilder, primary keys or, any other metadata information.
		/// </summary>
		public static bool LoadSchema
		{
			get
			{
				return loadSchema;
			}
			set
			{
				loadSchema = value;
			}
		}
		
		private bool _loadSchemaOnly = false;
		/// <summary>
		/// Used to signal to load only the schema and not fill any data, useful to retrieve meta information
		/// </summary>
		public bool LoadSchemaOnly 
		{ 
			get
			{
				return _loadSchemaOnly;
			}
			set
			{
				_loadSchemaOnly = value;
			}
		}

		/// <summary>
		/// Opens the RecordsetHelper and requeries according to the value of “requery” parameter.
		/// </summary>
		/// <param name="requery">Indicates if a requery most be done.</param>
		protected virtual void OpenRecordset(bool requery)
		{
			firstEOF = true;
			if (ActiveConnection != null && activeCommand != null)
			{
				DbDataAdapter dbAdapter = null;
				try
				{
					dbAdapter = CreateAdapter(ActiveConnection, false);
					sqlSelectQuery = activeCommand.CommandText;
					operationFinished = false;
					if (LoadSchema)
					{
						using (DataTable tmpTable = new DataTable())
						{
							if ((this.Tables != null) &&
								(this.Tables.Count > 0) &&
								(this.Tables[0].Columns != null) &&
								(this.Tables[0].Columns.Count > 0)
								)
							{
								dbAdapter.FillSchema(tmpTable, SchemaType.Source);
							}
							else
							{
							}
						}
					}
					if (!LoadSchemaOnly)
						dbAdapter.Fill(this);
					else
						dbAdapter.FillSchema(this, SchemaType.Source);
				}
				finally
				{
					if (!IsCachingAdapter)
						dbAdapter.Dispose();
				}
			}
			if (Tables.Count > 0)
			{
				FixAutoincrementColumns(Tables[0]);
				operationFinished = true;
				currentView = Tables[0].DefaultView;
				currentView.AllowDelete = true;
				currentView.AllowEdit = true;
				currentView.AllowNew = true;
				if (Tables[0].Rows.Count == 0)
				{
					index = -1;
					eof = true;
				}
				else
				{
					index = 0;
					eof = false;
				}
			}
			else
			{
				index = -1;
				eof = true;
			}
			newRow = false;
			opened = true;
			OnAfterQuery();
		}

		/// <summary>
		/// Populates a recordsetHelper with the information defined in a XmlDocument.
		/// </summary>
		/// <param name="document">XmlDocument to load into the RecordsetHelper.</param>
		public void Open(XmlDocument document)
		{
			StringReader sreader = null;
			XmlTextReader reader = null;
			try
			{
				disconnected = true;
				sreader = new StringReader(document.OuterXml);
				reader = new XmlTextReader(sreader);
				ReadXml(reader, XmlReadMode.ReadSchema);
				MoveFirst();
				AcceptChanges();
			}
			catch
			{
			}
			finally
			{
				if (sreader != null)
				{
					sreader.Close();
				}
				if (reader != null)
				{
					reader.Close();
				}
			}
		}

		/// <summary>
		/// Updates the data in a Recordset object by re-executing the query on which the object is based.
		/// </summary>
		public virtual void Requery()
		{
			Open(true);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets and Set the percent of the current position of the total of records retrieved.
		/// </summary>
		public virtual float PercentPosition
		{
			get
			{
				float result = -1;
				if (index != -1)
				{
					result = ((index + 1f) * 100f) / RecordCount;
				}
				return result;
			}
			set
			{
				if (index != -1)
				{
					BasicMove(System.Convert.ToInt32(value * RecordCount / 100) - 1);
				}
			}
		}

		/// <summary>
		/// Indicates if this RecordsetHelper have been open.
		/// </summary>
		internal bool Opened
		{
			get
			{
				return opened;
			}
		}

		/// <summary>
		/// Gets or Sets the current Record position inside the RecordsetHelper.
		/// </summary>
		internal int CurrentPosition
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		/// <summary>
		/// Gets or sets the DatabaseType being use by this object. 
		/// </summary>
		public DatabaseType DatabaseType
		{
			get
			{
				return dbType;
			}
			set
			{
				dbType = value;
			}
		}

		/// <summary>
		/// Returns a value that indicates whether the current record position is before the first record in a RecordsetHelper object. Read-only Boolean.
		/// </summary>
		public virtual bool BOF
		{
			get
			{
				return index == 0;
			}
		}

		/// <summary>
		/// Gets a string with the SQL query being use to obtain the RecordsetHelper data.
		/// </summary>
		public string RecordSource
		{
			get
			{
				return sqlSelectQuery;
			}
		}

		/// <summary>
		/// Gets or sets the DBProviderFactory to be use by this object.
		/// </summary>
		public DbProviderFactory ProviderFactory
		{
			get
			{
				return providerFactory;
			}
			set
			{
				providerFactory = value;
			}
		}

		/// <summary>
		/// Gets or sets the ActiveConnection (this connection is the one used in all RecordsetHelper operations).
		/// </summary>
		public virtual DbConnection ActiveConnection
		{
			get
			{
				return activeConnection;
			}
			set
			{
				if (value != null)
					Validate();
				if (activeCommand != null)
				{
					activeCommand.Connection = value;
				}
				activeConnection = value;
				connectionString = ActiveConnection != null ? ActiveConnection.ConnectionString : String.Empty;
				connectionStateAtEntry = ActiveConnection != null ? ActiveConnection.State : ConnectionState.Closed;
			}
		}

		/// <summary>
		/// Returns a copy of the current ActiveCommand of this RecordsetHelper.
		/// </summary>
		/// <returns>A copy of the current ActiveCommand.</returns>
		public DbCommand CopySourceCommand()
		{
			DbCommand result = null;
			if (opened)
			{
				result = ActiveConnection.CreateCommand();
				result.CommandText = activeCommand.CommandText;
				result.CommandType = activeCommand.CommandType;
				DbParameter[] paramArray = new DbParameter[activeCommand.Parameters.Count];
				activeCommand.Parameters.CopyTo(paramArray, 0);
				result.Parameters.AddRange(paramArray);
				return result;
			}
			else
			{
				throw new InvalidOperationException("The recordSet has to be opened to perform this operation");
			}
		}

		/// <summary>
		/// Gets or sets the connection string being use by this RecordsetHelper object.
		/// </summary>
		public String ConnectionString
		{
			get
			{
				return connectionString;
			}
			set
			{
				connectionString = value;
				if (providerFactory != null)
				{
					try
					{
						Validate();
#if TargetF2
                        DbConnection connection = DBTrace.CreateConnectionWithTrace(providerFactory);
#else
						DbConnection connection = providerFactory.CreateConnectionWithTrace();
#endif
						connection.ConnectionString = value;
						ActiveConnection = connection;
#if TargetF2
                        DBTrace.OpenWithTrace(ActiveConnection);
#else
						ActiveConnection.OpenWithTrace();
#endif
					} // try
					catch (Exception ex)
					{
						if (InDesignMode)
						{
							MessageBox.Show(
								string.Format(
									"Problem while trying to set the active connection. Please verify ConnectionString {0} and Factory {1} settings. Error details {2}",
									connectionString,
									providerFactory,
									ex.Message),
								TITLE_DIALOG_RecordSetError);
						} // if

						if (!disconnected)
							throw ex;
					} // catch
				}
				_defaultValues = null;
			}
		}

		/// <summary>
		/// Gets the design mode flag.
		/// </summary>
		protected bool InDesignMode
		{
			get
			{
				return Process.GetCurrentProcess().ProcessName == "devenv";
			}
		}

		/// <summary>
		/// Gets a bool value indicating if the current record is the last one in the RecordsetHelper object.
		/// </summary>
		public virtual bool EOF
		{
			get
			{
				return eof;
			}
		}

		/// <summary>
		/// Gets a DataRow object containing the field values of the current record.
		/// </summary>
		public DataRow FieldsValues
		{
			get
			{
				return UsingView ? currentView[index].Row : Tables[0].Rows[index];
			}
		}

		/// <summary>
		/// Gets a DataColumnCollection object that contains the information of all columns on the RecordsetHelper.
		/// </summary>
		public DataColumnCollection FieldsMetadata
		{
			get
			{
				if (Tables.Count <= 0)
				{
					Tables.Add();
				}
				return Tables[0].Columns;
			}
		}

		/// <summary>
		/// Gets or sets the SQL query used for select operations in this RecordsetHelper.
		/// </summary>
		public String SqlSelectQuery
		{
			get
			{
				return sqlSelectQuery;
			}
			set
			{
				sqlSelectQuery = value;
			}
		}

		/// <summary>
		/// Gets or sets the SQL query used for update operations in this RecordsetHelper.
		/// </summary>
		public String SqlUpdateQuery
		{
			get
			{
				return sqlUpdateQuery;
			}
			set
			{
				sqlUpdateQuery = value;
			}
		}

		/// <summary>
		/// Gets or sets the SQL query used for delete operations in this RecordsetHelper.
		/// </summary>
		public String SqlDeleteQuery
		{
			get
			{
				return sqlDeleteQuery;
			}
			set
			{
				sqlDeleteQuery = value;
			}
		}

		/// <summary>
		/// Gets or sets the SQL query used for insert operations in this RecordsetHelper.
		/// </summary>
		public String SqlInsertQuery
		{
			get
			{
				return sqlInsertQuery;
			}
			set
			{
				sqlInsertQuery = value;
			}
		}

		/// <summary>
		/// Sets or sets the source to obtain the necessary queries. Can be DBCommand or String.
		/// </summary>
		public Object Source
		{
			set
			{
				if (value is DbCommand)
				{
					if (((DbCommand)value).Connection != null && ActiveConnection != ((DbCommand)value).Connection)
					{
						ActiveConnection = ((DbCommand)value).Connection;
					}
					activeCommand = (DbCommand)value;
				}
				else if (value is String)
				{
					List<DbParameter> parameters;
					CommandType commandType = getCommandType((string)value, out parameters);
					activeCommand = CreateCommand((string)value, commandType, parameters);
				}
				else
				{
					throw new ArgumentException("Invalid type for the Source property");
				}
				source = value;
			}
		}

		/// <summary>
		/// Gets the current total number of records on the RecordsetHelper.
		/// </summary>
		public virtual int RecordCount
		{
			get
			{
				int count = 0;
				if (UsingView)
				{
					count = currentView.Count;
				}
				else if (Tables.Count > 0)
				{
					count = Tables[0].Rows.Count;
				}
				if (newRow && !UsingView)
				{
					return count + 1;
				}
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating if any operation is pending.
		/// </summary>
		public bool IsLoadingFinished
		{
			get
			{
				return operationFinished;
			}
		}


		/// <summary>
		/// Gets a value that indicates whether the named column contains a null value.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <returns>true if the column contains a null value; otherwise, false.</returns>
		public bool IsNull(string columnName)
		{
			if (this.CurrentRow == null)
			{
				throw new InvalidOperationException("No current row selected.");
			}
			return this.CurrentRow.IsNull(columnName);
		}

		/// <summary>
		/// Gets or sets the SQL query used for select operations in this RecordsetHelper.
		/// </summary>
		public String SqlQuery
		{
			get
			{
				return sqlSelectQuery;
			}
			set
			{
				sqlSelectQuery = value;
			}
		}

		/// <summary>
		/// Looks for a column with the given name and returns the column index
		/// or -1 if not found
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public int GetColumnIndexByName(String columnName)
		{
			if (UsingView)
			{
				return currentView.Table.Columns.IndexOf(columnName);
			}
			else
			{
				if (Tables.Count > 0)
				{
					return Tables[0].Columns.IndexOf(columnName);
				}
				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the row value at “ColumnName” index.
		/// </summary>
		/// <param name="columnName">Name of the column to look for.</param>
		/// <returns>The value at the given index.</returns>
		public virtual Object this[String columnName]
		{
			get
			{
				return CurrentRow[columnName];
			}
			set
			{
				int columnIndex = GetColumnIndexByName(columnName);
				if (columnIndex > -1)
				{
					SetNewValue(columnIndex, value);
				}
				else
				{
					throw new Exception(string.Format("Column {0} not found", columnName));
				}
			}
		}

		/// <summary>
		/// Gets or sets the row value at “ColumnIndex” index.
		/// </summary>
		/// <param name="columnIndex">index of the column to look for.</param>
		/// <returns>The value at the given index.</returns>
		public virtual Object this[int columnIndex]
		{
			get
			{
				return CurrentRow[columnIndex];
			}
			set
			{
				SetNewValue(columnIndex, value);
			}
		}

		/// <summary>
		/// Gets or sets the row value at “ColumnName” index.
		/// </summary>
		/// <param name="columnName">Name of the column to look for.</param>
		/// <returns>The value at the given index.</returns>
		public virtual FieldHelper GetField(String columnName)
		{
			FieldHelper newField = new FieldHelper(this, columnName, false);
			return newField;
		}

		/// <summary>
		/// Gets or sets the row value at “ColumnIndex” index.
		/// </summary>
		/// <param name="columnIndex">index of the column to look for.</param>
		/// <returns>The value at the given index.</returns>
		public virtual FieldHelper GetField(int columnIndex)
		{
			FieldHelper newField = new FieldHelper(this, columnIndex, true);
			return newField;
		}

		/// <summary>
		/// Sets the Filter to by applied to the this ADORecordsetHelper. (valid objects are: string, DataViewRowState and DataRow[]).
		/// </summary>
		[DefaultValue(null)]
		public virtual Object Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
				filtered = filter != null && opened;
				if (filtered)
				{
					if (filter is String)
					{
						SetFilter((String)filter);
					}
					else if (filter is DataViewRowState)
					{
						SetFilter((DataViewRowState)filter);
					}
					else if (filter is DataRow[])
					{
						SetFilter((DataRow[])filter);
					}
					else
					{
						throw new ArgumentException("Filter value not allowed");
					}
				}
			}
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Property used to determine if the data needs to be get from a dataview or the table directly
		/// </summary>
		protected virtual bool UsingView
		{
			get
			{
				return filtered;
			}
		}

		/// <summary>
		/// Gets a DataRow with the information of the current record on the RecordsetHelper.
		/// </summary>
		internal virtual DataRow CurrentRow
		{
			get
			{
				DataRow theRow = null;
				if (newRow)
				{
					theRow = dbRow;
				}
				else if (UsingView)
				{
					dbvRow = currentView[index];
					theRow = dbvRow.Row;
				}
				else
				{
					if (index < Tables[0].Rows.Count)
					{
						theRow = Tables[0].Rows[index];
					}
				}
				return theRow;
			}
		}

		/// <summary>
		/// Sets a bookmark to an specific record inside the RecordsetHelper.
		/// </summary>
		public virtual DataRow Bookmark
		{
			get
			{
				return UsingView ? currentView[index].Row : Tables[0].Rows[index];
			}
			set
			{
				index = findBookmarkIndex(value);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Saves the information on the RecordsetHelper to a XML document.
		/// </summary>
		/// <param name="document">The XML document to save the data to.</param>
		public void Save(XmlDocument document)
		{
			using (StringWriter writer = new StringWriter())
			{
				WriteXml(writer, XmlWriteMode.WriteSchema);
				document.LoadXml(writer.ToString());
				writer.Close();
			}
		}

		/// <summary>
		/// Moves the current record to the beginning of the RecordsetHelper.
		/// </summary>
		public virtual void MoveFirst()
		{
			BasicMove(0);
		}

		/// <summary>
		/// Moves the current record to the end of the RecordsetHelper.
		/// </summary>
		public virtual void MoveLast()
		{
			BasicMove((UsingView ? currentView.Count : Tables[0].Rows.Count));
		}

		/// <summary>
		/// Moves the current record forward one position.
		/// </summary>
		public virtual void MoveNext()
		{
			BasicMove(index + 1);
		}

		/// <summary>
		/// Moves the current record backwards one position.
		/// </summary>
		public virtual void MovePrevious()
		{
			BasicMove(index - 1);
		}

		/// <summary>
		/// Moves the position of the currentRecord in a RecordSet
		/// </summary>
		/// <param name="records">Amount of records positive or negative to move from the current record.</param>
		public virtual void Move(int records)
		{
			BasicMove(index + records);
		}

		/// <summary>
		/// Gets Command Source query string
		/// </summary>
		/// <returns>string</returns>
		public String getSource()
		{
			if (source is DbCommand)
			{
				return ((DbCommand)source).CommandText;
			}
			return (String)source;
		}

		/// <summary>
		/// /// Deletes the current record.
		/// </summary>
		public virtual void Delete()
		{
			CurrentRow.Delete();
		}

		private bool _cachingAdapter = false;

		/// <summary>
		/// Return true if the recordsethelper is caching the adapters
		/// </summary>
		public bool IsCachingAdapter
		{
			get { return _cachingAdapter; }
		}

		private Dictionary<KeyValuePair<DbConnection, string>, DbDataAdapter> dataAdaptersCached = new Dictionary<KeyValuePair<DbConnection, string>, DbDataAdapter>();

		/// <summary>
		/// Start caching the adapters used for connections. Use carefully because it needs an explicit call to StopCachingAdapter
		/// </summary>
		public void StartCachingAdapter()
		{
			clearDataAdaptersCached();
			_cachingAdapter = true;
		}

		/// <summary>
		/// Stop caching the adapters used for connections
		/// </summary>
		public void StopCachingAdapter()
		{
			clearDataAdaptersCached();
			_cachingAdapter = false;
		}

		/// <summary>
		/// Clear data adapters cached
		/// </summary>
		private void clearDataAdaptersCached()
		{
			try
			{
				foreach (KeyValuePair<DbConnection, string> key in new List<KeyValuePair<DbConnection, string>>(dataAdaptersCached.Keys))
				{
					try
					{
						dataAdaptersCached[key].Dispose();
						dataAdaptersCached[key] = null;
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			finally
			{
				dataAdaptersCached.Clear();
			}
		}

		/// <summary>
		/// Using connection parameter creates a Database Data Adapter
		/// </summary>
		/// <param name="connection">DbConnection parameter</param>
		/// <param name="updating">if updating creates all internal query strings</param>
		/// <returns></returns>
		protected virtual DbDataAdapter CreateAdapter(DbConnection connection, bool updating)
		{
			Debug.Assert(connection != null, "Error during CreateAdapter call. Connection String must never be null");
			DbDataAdapter realAdapter = ProviderFactory.CreateDataAdapter();
			DbDataAdapter adapter = ProviderFactory.CreateDataAdapter();
			bool isOracleProvider = ProviderFactory.GetType().FullName.Equals("Oracle.DataAccess.Client.OracleClientFactory");
			bool isMSAccessProvider = (ProviderFactory is System.Data.OleDb.OleDbFactory &&
									  connection.ConnectionString.Contains("Provider=Microsoft.Jet"));
			DbCommandBuilder cmdBuilder = null;
			KeyValuePair<DbConnection, string> key = new KeyValuePair<DbConnection, string>();
			try
			{
				cmdBuilder = ProviderFactory.CreateCommandBuilder();

				if (activeCommand.Connection == null || activeCommand.Connection.ConnectionString.Equals(""))
				{
					//What should we use here. ActiveConnection or the connection we are sending as parameter
					//it seams more valid to use the parameter
					activeCommand.Connection = connection;
				}
				if (String.IsNullOrEmpty(activeCommand.CommandText))
				{
					activeCommand.CommandText = sqlSelectQuery;
				}
				DbTransaction transaction = TransactionManager.GetTransaction(connection);
				if (transaction != null)
				{
					activeCommand.Transaction = transaction;
				}

				if (_cachingAdapter)
				{
					key = new KeyValuePair<DbConnection, string>(activeCommand.Connection, activeCommand.CommandText);
					if (dataAdaptersCached.ContainsKey(key))
					{
						return dataAdaptersCached[key];
					}
				}

				adapter.SelectCommand = activeCommand;
				realAdapter.SelectCommand = adapter.SelectCommand;
				cmdBuilder.DataAdapter = adapter;
				if (updating)
				{
					if (DatabaseType == DatabaseType.Access || DatabaseType == DatabaseType.SQLServer || getTableName(activeCommand.CommandText).Contains(" "))
					{
						cmdBuilder.QuotePrefix = "[";
						cmdBuilder.QuoteSuffix = "]";
					}
					CreateUpdateCommand(adapter, cmdBuilder);
					try
					{
						adapter.InsertCommand = (string.IsNullOrEmpty(sqlInsertQuery)) ? cmdBuilder.GetInsertCommand(true) : CreateCommand(sqlInsertQuery, CommandType.Text, null);
					}
					catch (Exception)
					{
						adapter.InsertCommand = CreateInsertCommandFromMetaData();
					}
					try
					{
						adapter.DeleteCommand = (string.IsNullOrEmpty(sqlDeleteQuery)) ? cmdBuilder.GetDeleteCommand(true) : CreateCommand(sqlDeleteQuery, CommandType.Text, null);
					}
					catch (Exception)
					{
						adapter.DeleteCommand = CreateDeleteCommandFromMetaData();
					}
					if ((ProviderFactory is System.Data.SqlClient.SqlClientFactory) ||
						(ProviderFactory is System.Data.OracleClient.OracleClientFactory) ||
						(ProviderFactory.GetType().FullName.Equals("Oracle.DataAccess.Client.OracleClientFactory")))
					{
						//EVG20080326: Oracle.DataAccess Version 10.102.2.20 Bug. It returns "::" instead ":" before each parameter name, wich is invalid.
						if (isOracleProvider)
						{
							adapter.InsertCommand.CommandText = adapter.InsertCommand.CommandText.Replace("::", ":");
							adapter.DeleteCommand.CommandText = adapter.DeleteCommand.CommandText.Replace("::", ":");
							adapter.UpdateCommand.CommandText = adapter.UpdateCommand.CommandText.Replace("::", ":");
						}
						CompleteInsertCommand(adapter);
					}
					else if (ProviderFactory is System.Data.OleDb.OleDbFactory)
					{
						((System.Data.OleDb.OleDbDataAdapter)realAdapter).RowUpdated += new System.Data.OleDb.OleDbRowUpdatedEventHandler(RecordSetHelper_RowUpdatedOleDb);
					}
					realAdapter.InsertCommand = CloneCommand(adapter.InsertCommand);
					realAdapter.DeleteCommand = CloneCommand(adapter.DeleteCommand);
					realAdapter.UpdateCommand = CloneCommand(adapter.UpdateCommand);
					if (realAdapter.InsertCommand != null)
					{
						realAdapter.InsertCommand.Transaction = realAdapter.SelectCommand.Transaction;
					}
					if (realAdapter.DeleteCommand != null)
					{
						realAdapter.DeleteCommand.Transaction = realAdapter.SelectCommand.Transaction;
					}
					if (realAdapter.UpdateCommand != null)
					{
						realAdapter.UpdateCommand.Transaction = realAdapter.SelectCommand.Transaction;
					}
				}

				if (_cachingAdapter)
				{
					dataAdaptersCached.Add(key, realAdapter);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				adapter.Dispose();
				if (cmdBuilder != null)
				{
					cmdBuilder.Dispose();
				}
			}
			return realAdapter;
		}

		/// <summary>
		/// Clone a command
		/// </summary>
		/// <param name="dbCommand"></param>
		/// <returns></returns>
		private DbCommand CloneCommand(DbCommand dbCommand)
		{

			DbCommand res = providerFactory.CreateCommand();
			res.CommandText = dbCommand.CommandText;
			res.CommandTimeout = dbCommand.CommandTimeout;
			res.CommandType = dbCommand.CommandType;
			res.Connection = dbCommand.Connection;
			res.Transaction = dbCommand.Transaction;

			foreach (DbParameter param in dbCommand.Parameters)
			{
				DbParameter newParam = res.CreateParameter();
				newParam.DbType = param.DbType;
				newParam.Direction = param.Direction;
				newParam.ParameterName = param.ParameterName;
				newParam.Size = param.Size;
				newParam.SourceColumn = param.SourceColumn;
				newParam.SourceColumnNullMapping = param.SourceColumnNullMapping;
				newParam.SourceVersion = param.SourceVersion;
				newParam.Value = param.Value;

				res.Parameters.Add(newParam);
			}

			return res;
		}

		/// <summary>
		/// Creates the update command for the database update operations of the recordset
		/// </summary>
		/// <param name="adapter">The data adapter that will contain the update command</param>
		/// <param name="cmdBuilder">The command builder to get the update command from.</param>
		protected virtual void CreateUpdateCommand(DbDataAdapter adapter, DbCommandBuilder cmdBuilder)
		{
			try
			{
				adapter.UpdateCommand = (string.IsNullOrEmpty(sqlUpdateQuery)) ? cmdBuilder.GetUpdateCommand(true) : CreateCommand(sqlUpdateQuery, CommandType.Text, null);
			}
			catch (Exception)
			{
				adapter.UpdateCommand = CreateUpdateCommandFromMetaData();
			}
		}

		/// <summary>
		/// Creates a new record for an updatable Recordset.
		/// </summary>
		public virtual void AddNew()
		{
			doAddNew();
		}
		/// <summary>
		/// This flag is used to stop the propagation of events while performing a delete.
		/// It was found that deleting a DataRow raised several events on the binding source
		/// and these events update the current row which must remain the same until the update logic is executed
		/// </summary>
		internal bool disableEventsWhileDeleting;


		/// <summary>
		/// Saves any changes you make to the current row of a ADORecordsetHelper object.
		/// </summary>
		public virtual void Update()
		{
		}

		/// <summary>
		/// Cancels any changes made to the current or new row of a ADORecordsetHelper object.
		/// </summary>
		public virtual void CancelUpdate()
		{
			DoCancelUpdate();
		}

		/// <summary>
		/// Cancels any changes made to the current or new row of a ADORecordsetHelper object.
		/// </summary>
		private void DoCancelUpdate()
		{
			DataRow theRow = CurrentRow;
			if (theRow.RowState != DataRowState.Unchanged)
			{
				theRow.RejectChanges();
			}
			newRow = false;
			dbRow = null;
		}

		/// <summary>
		/// Cancels execution of any pending process.
		/// </summary>
		public virtual void Cancel()
		{
			DoCancelUpdate();
		}

		/// <summary>
		/// Cancels a pending batch update.
		/// </summary>
		public virtual void CancelBatch()
		{
			Tables[0].RejectChanges();
			newRow = false;
			dbRow = null;
		}

		/// <summary>
		/// Verifies if a parameter with the provided name exists on the command received, otherwise a new parameter using the specified name.
		/// </summary>
		/// <param name="command">The command object to look into.</param>
		/// <param name="name">The name of the parameter to look for.</param>
		/// <returns>The parameter named with “name”.</returns>
		public static DbParameter commandParameterBinding(DbCommand command, string name)
		{
			if (!command.Parameters.Contains(name))
			{
				DbParameter parameter = command.CreateParameter();
				parameter.ParameterName = name;
				command.Parameters.Add(parameter);
			}
			return command.Parameters[name];
		}

		/// <summary>
		/// Closes an open object and any dependent objects.
		/// </summary>
		public virtual void Close()
		{
			try
			{
				if (Tables.Count > 0)
				{
					Tables[0].Rows.Clear();
				}
				if (activeCommand != null)
				{
					activeCommand.Connection = null;
					activeCommand.Dispose();
				}
				if (ActiveConnection != null)
				{
					if (ActiveConnection.State == ConnectionState.Open && connectionStateAtEntry == ConnectionState.Closed)
					{
						ActiveConnection.Close();
					}
				}
				opened = false;
				base.Dispose();
			}
			catch
			{
			}
		}

		/// <summary>
		/// Looks in all records for a field that matches the “criteria”. 
		/// </summary>
		/// <param name="criteria">A String used to locate the record. It is like the WHERE clause in an SQL statement, but without the word WHERE.</param>
		public void Find(String criteria)
		{
			DataView result = Tables[0].DefaultView;
			result.RowFilter = criteria;
			if (result.Count > 0)
			{
				object[] values = result[0].Row.ItemArray;
				bool bfound = false;
				MoveFirst();
				while ((!bfound) && !this.EOF)
				{
					for (int i = 0; i < values.Length; i++)
					{
						bfound = (this.CurrentRow.ItemArray[i].Equals(values[i]));
						if (!bfound)
						{
							break;
						}
					}
					if (!bfound)
					{
						MoveNext();
					}
				}
			}
		}

		/// <summary>
		/// Looks in all records for a field that matches the “criteria”. 
		/// </summary>
		/// <param name="rowName">A String used to locate the row from the record.</param>
		/// <param name="pCriteria">A String used to locate the record. It is like the WHERE clause in an SQL statement, but without the word WHERE.</param>
		public void Find(String rowName, String pCriteria)
		{
			if (Tables[0].Rows.Count > 0)
			{
				bool bfound = false;
				MoveFirst();
				int i = 0;
				while ((!bfound) && !this.EOF)
				{
					if (i < Tables[0].Rows.Count)
					{
						bfound = (Tables[0].Rows[i][rowName].Equals(pCriteria));
					}
					if (!bfound)
					{
						MoveNext();
					}
					i++;
				}
			}
		}

		/// <summary>
		/// Oracle event for row update
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">OracleRowUpdated event args</param>
		protected void RecordSetHelper_RowUpdatedOracle(object sender, System.Data.OracleClient.OracleRowUpdatedEventArgs e)
		{
			if (e.StatementType == StatementType.Insert)
			{
				Dictionary<String, String> identities = IdentityColumnsManager.GetIndentityInformation(getTableName(activeCommand.CommandText));
				if (identities != null)
				{
					DbCommand oCmd = providerFactory.CreateCommand();
					oCmd.Connection = ActiveConnection;
					oCmd.Transaction = TransactionManager.GetTransaction(ActiveConnection);
					foreach (KeyValuePair<String, String> identityInfo in identities)
					{
						oCmd.CommandText = "Select " + identityInfo.Value + ".Currval from dual";
						e.Row[identityInfo.Key] = oCmd.ExecuteScalar();
					}
					e.Row.AcceptChanges();
				}
			}
		}

		/// <summary>
		/// OleDb Row Updated event
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">Row updated event args</param>
		protected void RecordSetHelper_RowUpdatedOleDb(object sender, System.Data.OleDb.OleDbRowUpdatedEventArgs e)
		{
			//This behavior depends on the database we are interacting with
			if (e.StatementType == StatementType.Insert && e.Status == UpdateStatus.Continue)
			{
				Dictionary<String, String> identities = IdentityColumnsManager.GetIndentityInformation(getTableName(activeCommand.CommandText));
				if (identities != null)
				{
					DbCommand oCmd = e.Command.Connection.CreateCommand();
					oCmd.Transaction = e.Command.Transaction;
					foreach (KeyValuePair<String, String> identityInfo in identities)
					{
						switch (DatabaseType)
						{
							case DatabaseType.Oracle:
								oCmd.CommandText = "Select " + identityInfo.Value + ".Currval from dual";
								break;
							case DatabaseType.SQLServer:
								oCmd.CommandText = "SELECT SCOPE_IDENTITY()";
								break;
							case DatabaseType.Access:
								oCmd.CommandText = "SELECT @@IDENTITY";
								break;
						}
						e.Row[identityInfo.Key] = oCmd.ExecuteScalar();
					}
					e.Row.AcceptChanges();
				}
			}
		}

		#region Serialization machinery

		internal Hashtable serializationInfo;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info">SerializationInfo</param>
		/// <param name="context">StreamingContext</param>
		protected RecordSetHelper(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			isDeserialized = true;
			isDefaultSerializationInProgress = false;
			int old_index = info.GetInt16("Index");
			index = old_index;
			newRow = info.GetBoolean("NewRow");
			filtered = info.GetBoolean("Filtered");
			opened = info.GetBoolean("Opened");
			firstEOF = info.GetBoolean("FirstEof");
			firstChange = info.GetBoolean("FirstChange");
			bool old_eof = info.GetBoolean("EOF");
			eof = old_eof;
			string factoryName = info.GetString("FactoryName");
			providerFactory = AdoFactoryManager.GetFactory(factoryName);
			sqlSelectQuery = info.GetString("RecordSource");
			if (opened)
			{
				//NOTE: all OpenRecordset logic can be reused. However after
				//executing the openrecordset logic the eof and index variables
				//will be reset and we need them to keep the original values;
				OpenRecordset(false);
				eof = old_eof;
				index = old_index;
			}
			//These properties must be handle in the classes that extend the recordset helper
			serializationInfo = new Hashtable();
			serializationInfo.Add("ActiveConnectionWasNull", info.GetBoolean("ActiveConnectionWasNull"));
			serializationInfo.Add("ConnectionString", info.GetString("ConnectionString"));
		}

		/// <summary>
		/// Gets Object Data
		/// </summary>
		/// <param name="info">SerializationInfo</param>
		/// <param name="context">StreamingContext</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Index", index);
			info.AddValue("NewRow", newRow);
			info.AddValue("Filtered", filtered);
			info.AddValue("Opened", opened);
			info.AddValue("FirstEof", firstEOF);
			info.AddValue("FirstChange", firstChange);
			info.AddValue("EOF", eof);
			info.AddValue("FactoryName", FactoryName);
			info.AddValue("RecordSource", RecordSource);
			info.AddValue("ActiveConnectionWasNull", ActiveConnection == null);
			info.AddValue("ConnectionString", ConnectionString);


		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public string FactoryName
		{
			get
			{
				if (ProviderFactory == null) return String.Empty;
				return AdoFactoryManager.GetFactoryNameFromProviderType(ProviderFactory.GetType());
			}
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Verifies if the ADORecordset object have been open.
		/// </summary>
		protected virtual void Validate()
		{
		}

		/// <summary>
		/// Infers the command type from an SQL string getting the schema metadata from the database.
		/// </summary>
		/// <param name="sqlCommand">The sql string to be analyzed.</param>
		/// <param name="parameters">List of DbParameters</param>
		/// <returns>The command type</returns>
		protected CommandType getCommandType(String sqlCommand, out List<DbParameter> parameters)
		{
			CommandType commandType = CommandType.Text;
			parameters = null;
			sqlCommand = sqlCommand.Trim();
			if (sqlCommand.StartsWith("select", StringComparison.InvariantCultureIgnoreCase) ||
				sqlCommand.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase) ||
				sqlCommand.StartsWith("update", StringComparison.InvariantCultureIgnoreCase) ||
				sqlCommand.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase) ||
				sqlCommand.StartsWith("exec", StringComparison.InvariantCultureIgnoreCase) ||
				sqlCommand.StartsWith("begin", StringComparison.InvariantCultureIgnoreCase))
			{
				commandType = CommandType.Text;
				return commandType;
			}
			else
			{
				string[] completeCommandParts = sqlCommand.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				sqlCommand = completeCommandParts[0];
				String[] commandParts = sqlCommand.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				String objectQuery = String.Empty;
				DbConnection connection = GetConnection(connectionString);
				if (!(connection.State == ConnectionState.Open))
				{
#if TargetF2
                    DBTrace.OpenWithTrace(connection);
#else
					connection.OpenWithTrace();
#endif
				}
				DataRow[] existingObjects;
				DataTable dbObjects = connection.GetSchema("Tables");
				if (dbObjects.Rows.Count > 0)
				{
					//this is an sql server connection
					if (dbObjects.Columns.Contains("table_catalog") && dbObjects.Columns.Contains("table_schema"))
					{
						if (commandParts.Length == 3)
						{
							objectQuery = "table_catalog = \'" + commandParts[0] + "\' AND table_schema = \'" + commandParts[1] + "\' AND table_name = \'" + commandParts[2] + "\'";
						}
						else if (commandParts.Length == 2)
						{
							objectQuery = "table_schema = \'" + commandParts[0] + "\' AND table_name = \'" + commandParts[1] + "\'";
						}
						else
						{
							objectQuery = "table_name = \'" + commandParts[0] + "\'";
						}
					}
					else if (dbObjects.Columns.Contains("OWNER"))
					{
						if (commandParts.Length == 2)
						{
							objectQuery = "OWNER = \'" + commandParts[0] + "\' AND TABLE_NAME = \'" + commandParts[1] + "\'";
						}
						else
						{
							objectQuery = "TABLE_NAME = \'" + commandParts[0] + "\'";
						}
					}
					existingObjects = dbObjects.Select(objectQuery);
					if (existingObjects.Length > 0)
					{
						commandType = CommandType.TableDirect;
						return commandType;
					}
				}
				dbObjects = connection.GetSchema("Procedures");
				// The query for looking for stored procedures information is version sensitive.
				// The database version can be verified in SQLServer using a query like "Select @@version"
				// That version can be mapped to the specific SQL Server Product Version with a table like the provided here: http://www.sqlsecurity.com/FAQs/SQLServerVersionDatabase/tabid/63/Default.aspx 
				// The following code verifies columns for SQL Server version 2003, other versions might have a different schema and require changes
				if (dbObjects.Columns.Contains("procedure_catalog") && dbObjects.Columns.Contains("procedure_schema"))
				{
					if (commandParts.Length == 3)
					{
						objectQuery = "procedure_catalog = \'" + commandParts[0] + "\' AND procedure_schema = \'" + commandParts[1] + " AND procedure_name = " + commandParts[2] +
																		 "\'";
					}
					else if (commandParts.Length == 2)
					{
						objectQuery = "procedure_schema = \'" + commandParts[0] + "\' AND procedure_name = \'" + commandParts[1] + "\'";
					}
					else
					{
						objectQuery = "procedure_name = \'" + commandParts[0] + "\'";
					}
				}
				else if (dbObjects.Rows.Count > 0)
				{
					//this is an sql server connection
					if (dbObjects.Columns.Contains("specific_catalog") && dbObjects.Columns.Contains("specific_schema"))
					{
						if (commandParts.Length == 3)
						{
							objectQuery = "specific_catalog = \'" + commandParts[0] + "\' AND specific_schema = \'" + commandParts[1] + " AND specific_name = " + commandParts[2] +
										  "\'";
						}
						else if (commandParts.Length == 2)
						{
							objectQuery = "specific_schema = \'" + commandParts[0] + "\' AND specific_name = \'" + commandParts[1] + "\'";
						}
						else
						{
							objectQuery = "specific_name = \'" + commandParts[0] + "\'";
						}
					}
					else if (dbObjects.Columns.Contains("OWNER"))
					{
						if (commandParts.Length == 2)
						{
							objectQuery = "OWNER = \'" + commandParts[0] + "\' AND OBJECT_NAME = \'" + commandParts[1] + "\'";
						}
						else
						{
							objectQuery = "OBJECT_NAME = \'" + commandParts[0] + "\'";
						}
					}
					existingObjects = dbObjects.Select(objectQuery);
					if (existingObjects.Length > 0)
					{
						commandType = CommandType.StoredProcedure;
						if (dbObjects.Columns.Contains("specific_catalog") && dbObjects.Columns.Contains("specific_schema"))
						{
							DataTable procedureParameters = connection.GetSchema("ProcedureParameters");
							DataRow[] theparameters =
								procedureParameters.Select(
									"specific_catalog = \'" + existingObjects[0]["specific_catalog"] + "\' AND specific_schema = \'" + existingObjects[0]["specific_schema"] +
									"' AND specific_name = '" + existingObjects[0]["specific_name"] + "\'",
									"ordinal_position ASC");
							if (theparameters.Length > 0)
							{
								parameters = new List<DbParameter>(theparameters.Length);
								foreach (DataRow paraminfo in theparameters)
								{
									DbParameter theParameter = providerFactory.CreateParameter();
#if CLR_AT_LEAST_3_5 
                                    theParameter.ParameterName = paraminfo.Field<string>("parameter_name");
                                    theParameter.DbType = MapToDbType(paraminfo.Field<string>("data_type"));
#else
									theParameter.ParameterName = paraminfo["parameter_name"] as string;
									theParameter.DbType = MapToDbType(paraminfo["data_type"] as string);
#endif
									parameters.Add(theParameter);
								}
							}
						}
					}
				}
			}
			return commandType;
		}

		/// <summary>
		/// Infers the command type from an sql string getting the schema metadata from the database.
		/// </summary>
		/// <param name="sql">The sql string to be analyzed</param>
		internal CommandType getCommandType(String sql)
		{
			List<DbParameter> parameters;
			return getCommandType(sql, out parameters);
		}

		/// <summary>
		/// Returns the ActiveConnection object if it has been initialized otherwise creates a new DBConnection object.
		/// </summary>
		/// <param name="connectionString">The connection string to be used by the connection.</param>
		/// <returns>A DBConnection containing with the connection string set. </returns>
		protected virtual DbConnection GetConnection(String connectionString)
		{
			if (ActiveConnection != null && ActiveConnection.ConnectionString.Equals(connectionString, StringComparison.InvariantCultureIgnoreCase))
			{
				return ActiveConnection;
			}
#if TargetF2
            DbConnection connection = DBTrace.CreateConnectionWithTrace(providerFactory);
#else
			DbConnection connection = providerFactory.CreateConnectionWithTrace();
#endif
			connection.ConnectionString = connectionString;
			return connection;
		}

		/// <summary>
		/// Converts from System.Type to DbType.
		/// </summary>
		/// <param name="type">The System.Type to be converted.</param>
		/// <returns>The equivalent DBType.</returns>
		public static DbType GetDBType(Type type)
		{
			DbType result = DbType.String;
			switch (type.Name)
			{
				case "Byte":
					result = DbType.Byte;
					break;
				case "Byte[]":
					result = DbType.Binary;
					break;
				case "Boolean":
					result = DbType.Boolean;
					break;
				case "DateTime":
					result = DbType.DateTime;
					break;
				case "Decimal":
					result = DbType.Decimal;
					break;
				case "Double":
					result = DbType.Double;
					break;
				case "Guid":
					result = DbType.Guid;
					break;
				case "Int16":
					result = DbType.Int16;
					break;
				case "Int32":
					result = DbType.Int32;
					break;
				case "Int64":
					result = DbType.Int64;
					break;
				case "Object":
					result = DbType.Object;
					break;
				case "SByte":
					result = DbType.SByte;
					break;
				case "Single":
					result = DbType.Single;
					break;
				case "String":
					result = DbType.String;
					break;
				case "UInt16":
					result = DbType.UInt16;
					break;
				case "UInt32":
					result = DbType.UInt32;
					break;
				case "UInt64":
					result = DbType.UInt64;
					break;
			}

			return result;
		}

		/// <summary>
		/// Turns the DB type string to corresponding CLR type string.
		/// </summary>
		/// <param name="strDBType"></param>
		/// <returns></returns>
		public static DbType MapToDbType(string strDBType)
		{
			switch (strDBType)
			{
				case "xml":
					return DbType.Xml;

				case "nvarchar":
				case "varchar":
				case "sysname":
				case "nchar":
				case "char":
				case "ntext":
				case "text":
					return DbType.String;

				case "int":
					return DbType.Int32;

				case "bigint":
					return DbType.Int64;

				case "bit":
					return DbType.Boolean;

				case "long":
					return DbType.Int32;

				case "real":
				case "float":
					return DbType.Double;

				case "datetime":
				case "datetime2":
				case "date":
					return DbType.DateTime;

				case "datetimeoffset":
					return DbType.DateTimeOffset;

				case "time":
				case "timespan":
					return DbType.Time;

				case "tinyint":
					return DbType.Byte;

				case "smallint":
					return DbType.Int16;

				case "uniqueidentifier":
					return DbType.Guid;

				case "numeric":
				case "decimal":
					return DbType.Decimal;

				case "binary":
				case "image":
				case "varbinary":
					return DbType.Binary;

				case "sql_variant":
					return DbType.Object;
			}
			throw new ArgumentException("Given DB type does not map to any known types.", "strDBType");
		}

		/// <summary>
		/// Converts from DbType to System.Type.
		/// </summary>
		/// <param name="dbType">The DBType to be converted.</param>
		/// <returns>The equivalent System.Type.</returns>
		public static System.Type GetType(DbType dbType)
		{
			System.Type result = System.Type.GetType("System.String");
			switch (dbType)
			{
				case DbType.Byte:
					result = System.Type.GetType("System.Byte");
					break;
				case DbType.Binary:
					result = System.Type.GetType("System.Byte[]");
					break;
				case DbType.Boolean:
					result = System.Type.GetType("System.Boolean");
					break;
				case DbType.DateTime:
					result = System.Type.GetType("System.DateTime");
					break;
				case DbType.Decimal:
					result = System.Type.GetType("System.Decimal");
					break;
				case DbType.Double:
					result = System.Type.GetType("System.Double");
					break;
				case DbType.Guid:
					result = System.Type.GetType("System.Guid");
					break;
				case DbType.Int16:
					result = System.Type.GetType("System.Int16");
					break;
				case DbType.Int32:
					result = System.Type.GetType("System.Int32");
					break;
				case DbType.Int64:
					result = System.Type.GetType("System.Int64");
					break;
				case DbType.Object:
					result = System.Type.GetType("System.Object");
					break;
				case DbType.SByte:
					result = System.Type.GetType("System.SByte");
					break;
				case DbType.Single:
					result = System.Type.GetType("System.Single");
					break;
				case DbType.String:
					result = System.Type.GetType("System.String");
					break;
				case DbType.UInt16:
					result = System.Type.GetType("System.UInt16");
					break;
				case DbType.UInt32:
					result = System.Type.GetType("System.UInt32");
					break;
				case DbType.UInt64:
					result = System.Type.GetType("System.UInt64");
					break;
			}

			return result;

		}

		/// <summary>
		/// Creates a DBCommand object using de provided parameters.
		/// </summary>
		/// <param name="commandText">A string containing the SQL query.</param>
		/// <param name="commandType">The desire type for the command.</param>
		/// <returns>A new DBCommand object containing the SLQ code received has parameter.</returns>
		internal DbCommand CreateCommand(String commandText, CommandType commandType)
		{
			List<DbParameter> parameters = null;
			return CreateCommand(commandText, commandType, parameters);
		}

		/// <summary>
		/// Creates a DBCommand object using de provided parameters.
		/// </summary>
		/// <param name="commandText">A string containing the SQL query.</param>
		/// <param name="commandType">The desire type for the command.</param>
		/// <param name="parameters">A list with the parameters to be included on the DBCommand object.</param>
		/// <returns>A new DBCommand object.</returns>
		protected virtual DbCommand CreateCommand(String commandText, CommandType commandType, List<DbParameter> parameters)
		{
			Debug.Assert(providerFactory != null, "Providerfactory must not be null");
			DbCommand command = providerFactory.CreateCommand();
			switch (commandType)
			{
				case CommandType.StoredProcedure:
					string[] commandParts = commandText.Split(" ".ToCharArray());
					command.CommandType = commandType;
					command.CommandText = commandParts[0];
					if (parameters != null && (parameters.Count == commandParts.Length - 1))
					{
						for (int i = 1; i < commandParts.Length; i++)
						{
							DbParameter parameter = parameters[i - 1];
							object value = commandParts[i];
							//conversions might be needed for various types
							//currently there is only a convertion for Guid types. New convertions will be added as needed
							if (parameter.DbType == DbType.Guid)
							{
								//Remove single quotes if present to avoid exception in Guid constructor
								string strValue = commandParts[i].Replace("'", "");
								value = new Guid(strValue);
							}
							parameter.Value = value;
						}
						command.Parameters.AddRange(parameters.ToArray());
					}
					break;
				case CommandType.TableDirect:
					//ODBC and SQL Client providers do not support table direct commands
					string providerType = providerFactory.GetType().ToString();
					if (providerType.StartsWith("System.Data.Odbc") || providerType.StartsWith("System.Data.SqlClient"))
					{
						command.CommandType = CommandType.Text;
						command.CommandText = "Select * from " + commandText;
					}
					else
					{
						goto default;
					}
					break;
				default:
					command.CommandType = commandType;
					command.CommandText = commandText;
					break;
			}
			return command;
		}

		/// <summary>
		/// Sets the primary key to a DataTable object.
		/// </summary>
		/// <param name="dataTable">The DataTable that holds the currently loaded data.</param>
		private void FixAutoincrementColumns(DataTable dataTable)
		{
			if (ActiveConnection is System.Data.SqlClient.SqlConnection)
			{
				foreach (DataColumn col in dataTable.PrimaryKey)
				{
					if (col.AutoIncrement)
					{
						col.AutoIncrementSeed = 0;
						col.AutoIncrementStep = -1;
						col.ReadOnly = false;
						hasAutoincrementCols = true;
						// todo check multiple autoincrement cases
						autoIncrementCol = col.ColumnName;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Indicates if is possible to move previous one record.
		/// </summary>     
		internal bool CanMovePrevious
		{
			get
			{
				return index >= 0;
			}
		}

		/// <summary>
		/// This is the atomic Move operation it sets the index on the proper position and updates the eof flag.
		/// </summary>
		/// <param name="newIndex">The new position for the index</param>
		protected virtual void BasicMove(int newIndex)
		{
			OnAfterMove();
		}

		private bool requiresDefaultValues = false;

		/// <summary>
		/// Executes the atomic addNew Operation creating the new row and setting the newRow flag.
		/// </summary>
		protected virtual void doAddNew()
		{
			if (UsingView)
			{
				dbvRow = currentView.AddNew();
				dbRow = dbvRow.Row;
			}
			else
			{
				dbRow = Tables[0].NewRow();
				requiresDefaultValues = true;
			}
			newRow = true;
		}


		/// <summary>
		///  Send to DB query to compute.
		/// </summary>
		/// <param name="expression">The query to compute</param>
		/// <returns>The value computed</returns>
		private object computeValue(string expression)
		{
			object result = null;
			using (DbCommand cmd = this.ActiveConnection.CreateCommand())
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = @"Select " + expression;
				cmd.Transaction = TransactionManager.GetTransaction(this.ActiveConnection);
				result = cmd.ExecuteScalar();
			}
			return result;
		}

		private List<KeyValuePair<bool, object>> _defaultValues = null; //isComputed - value

		/// <summary>
		/// Sets default values to a fields to avoid insert null in the DB when the field does not accept it.
		/// </summary>
		private void AssignDefaultValues(DataRow dbRow)
		{
			DbDataAdapter adapter = null;
			DataTable schemaTable = null;
			try
			{
				requiresDefaultValues = false;
				if (_defaultValues == null) //no default values loaded for this table
				{
					try{
						adapter = CreateAdapter(GetConnection(ConnectionString), true);
						schemaTable = this.ActiveConnection.GetSchema("Columns", new string[] { this.ActiveConnection.Database, "dbo", getTableName(adapter.SelectCommand.CommandText, true).Replace("dbo.", string.Empty) });
					}catch
					{
						return;
					}

					//Preloaded with the number  of elements required
                    _defaultValues = new List<KeyValuePair<bool, object>>();
                    for (int i = 0; i < this.Tables[0].Columns.Count; i++)
                    {
                        _defaultValues.Add(new KeyValuePair<bool, object>());
                    }
					string defaultValue = String.Empty;
					bool isComputed = false;
					bool isUnknown = false;
					string partialResult = String.Empty;
					object originalValue = null;
					int thisColumnIndex = -1;
					for (int i = 0; i < schemaTable.Rows.Count; i++)
					{
						thisColumnIndex = this.Tables[0].Columns.IndexOf(Convert.ToString(schemaTable.Rows[i]["COLUMN_NAME"]));
						if (thisColumnIndex < 0) continue;

						//13 Maximun length
						if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(string)) this.Tables[0].Columns[thisColumnIndex].MaxLength = Convert.ToInt32(schemaTable.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]);

						originalValue = dbRow[thisColumnIndex];
						if (schemaTable.Rows[i]["COLUMN_DEFAULT"] != System.DBNull.Value) //Has default value
						{
							defaultValue = (string)schemaTable.Rows[i]["COLUMN_DEFAULT"]; //8 Default Value
							if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(bool))
							{
								dbRow[thisColumnIndex] = Convert.ToBoolean(Convert.ToDouble((defaultValue).Trim(new char[] { '(', ')', '\'' })));
							}
							else
							{
								try
								{
									partialResult = defaultValue.Trim(new char[] { '(', ')', '\'' });
									if (this.Tables[0].Columns[thisColumnIndex].MaxLength != -1) //is string																			
										dbRow[thisColumnIndex] = partialResult.Length > this.Tables[0].Columns[thisColumnIndex].MaxLength ? partialResult.Substring(0, this.Tables[0].Columns[thisColumnIndex].MaxLength) : partialResult;
									else
										dbRow[thisColumnIndex] = partialResult;
								}
								catch
								{
									try
									{
										dbRow[thisColumnIndex] = computeValue(defaultValue);
										isComputed = true;
									}
									catch
									{
										isUnknown = true;
									}
								}
							}
						}
						else
						{
							var isNullable = schemaTable.Rows[i]["IS_NULLABLE"];
							bool tmpRes = false;
							if (isNullable != null 
								&& (string.Equals(Convert.ToString(isNullable), "No", StringComparison.InvariantCultureIgnoreCase)
								|| (bool.TryParse(Convert.ToString(isNullable), out tmpRes) && !tmpRes))) //Not Allow Null and has not default value
							{
								//Add more if necesary
								if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(string))
									dbRow[thisColumnIndex] = string.Empty;
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(Int16))
									dbRow[thisColumnIndex] = default(Int16);
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(Int32))
									dbRow[thisColumnIndex] = default(Int32);
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(bool))
									dbRow[thisColumnIndex] = default(bool);
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(decimal))
									dbRow[thisColumnIndex] = default(decimal);
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(byte))
									dbRow[thisColumnIndex] = default(byte);
								else if (this.Tables[0].Columns[thisColumnIndex].DataType == typeof(char))
									dbRow[thisColumnIndex] = default(char);
							}
							else
								dbRow[thisColumnIndex] = DBNull.Value;
						}
						if (isComputed)
						{
							_defaultValues[thisColumnIndex] = new KeyValuePair<bool, object>(true, defaultValue);
							isComputed = false;
						}
						else if (isUnknown)
						{
							_defaultValues[thisColumnIndex] = new KeyValuePair<bool, object>(false, DBNull.Value);
							isUnknown = false;
						}
						else
							_defaultValues[thisColumnIndex] = new KeyValuePair<bool, object>(false, dbRow[thisColumnIndex]);

						if (originalValue != DBNull.Value)
							dbRow[thisColumnIndex] = originalValue;
					}
				}
				else //already _defaultValues has been created
				{
					try
					{
						dbRow.BeginEdit();
						for (int i = 0; i < _defaultValues.Count; i++)
						{
							if (dbRow[i] == DBNull.Value)
							{
								if (!_defaultValues[i].Key)
									dbRow[i] = _defaultValues[i].Value;
								else
									dbRow[i] = computeValue((string)_defaultValues[i].Value);
							}
						}
					}
					finally
					{
						dbRow.EndEdit();
					};
				}
			}
			finally
			{
				if (!IsCachingAdapter && adapter != null)
					adapter.Dispose();
			}
		}

		/// <summary>
		/// Saves any changes made to the DataRow recieved as parameter.
		/// </summary>
		/// <param name="theRow">The row to be save on the Database.</param>
		protected virtual void UpdateWithNoEvents(DataRow theRow)
		{
			if (theRow.RowState != DataRowState.Unchanged)
			{
				if (!isBatchEnabled())
				{
					DbConnection connection = GetConnection(ConnectionString);
					DbDataAdapter dbAdapter = null;
					try
					{
						dbAdapter = CreateAdapter(connection, true);
						if (requiresDefaultValues)
							AssignDefaultValues(theRow);

						dbAdapter.Update(new DataRow[] { theRow });
					}
					finally
					{
						if (!IsCachingAdapter)
							dbAdapter.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// Indicates if the ADORecordsetHelper is in batch mode.
		/// </summary>
		/// <returns>True if the ADORecordsetHelper is in batch mode, otherwise false.</returns>
		protected virtual bool isBatchEnabled()
		{
			return false;
		}

		/// <summary>
		/// iterate fields, to assign current row the values for each specific fields
		/// </summary>
		/// <param name="fields">array of fields</param>
		/// <param name="values">array of values</param>
		/// <param name="isString">is string the field items</param>
		/// <param name="currentValues">has the current values</param>
		/// <returns>the row with the assigned values on each field</returns>
		protected object[] iterateFields(object[] fields, object[] values, bool isString, bool currentValues)
		{
			object[] thevalues = values;
			for (int i = 0; i < fields.Length; i++)
			{
				if (!currentValues)
				{
					if (isString)
					{
						CurrentRow[(String)fields[i]] = values[i];
					}
					else
					{
						CurrentRow[(int)fields[i]] = values[i];
					}
				}
				else
				{
					thevalues = new object[fields.Length];
					if (isString)
					{
						thevalues[i] = CurrentRow[(String)fields[i]];
					}
					else
					{
						thevalues[i] = CurrentRow[(int)fields[i]];
					}
				}
			}
			return thevalues;
		}

		/// <summary>
		/// Check if the query has multiple tables
		/// </summary>
		/// <param name="sqlQuery">query string</param>
		/// <returns>boolean</returns>
		protected virtual bool HasMultipleTables(String sqlQuery)
		{
			for (int i = 0; i < sqlQuery.Length; i++)
			{
				if (sqlQuery.Substring(i, 4).ToUpper().Equals("FROM"))
				{
					sqlQuery = sqlQuery.Remove(0, i + 4);
					break;
				}
			}

			for (int i = 0; i < sqlQuery.Length; i++)
			{
				if (sqlQuery.Substring(i, 5).ToUpper().Equals("WHERE"))
				{
					sqlQuery = sqlQuery.Remove(i).Trim();
					break;
				}
			}

			String[] tables = sqlQuery.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (tables.Length > 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a delete command using the information contained in the RecordsetHelper.    
		/// </summary>
		/// <returns>A DBCommand object containing a delete command.</returns>
		protected DbCommand CreateDeleteCommandFromMetaData()
		{
			DbCommand result = null;
			String tableName = getTableName(activeCommand.CommandText);
			int j = 0;
			try
			{
				if (!string.IsNullOrEmpty(tableName))
				{
					string wherePart = "";
					string sql = "";
					List<DbParameter> listGeneral = new List<DbParameter>();
					DbParameter pInfo = null;

					foreach (System.Data.DataColumn dColumn in Tables[0].Columns)
					{
						if (wherePart.Length > 0)
						{
							wherePart += " AND ";
						}

						if (dColumn.AllowDBNull)
						{
							wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";

							pInfo = CreateParameterFromColumn("p" + (++j), dColumn);
							pInfo.DbType = DbType.Int32;
							pInfo.SourceVersion = DataRowVersion.Original;
							pInfo.SourceColumnNullMapping = true;
							pInfo.Value = 1;
							listGeneral.Add(pInfo);

							pInfo = CreateParameterFromColumn("p" + (++j), dColumn);
							pInfo.SourceVersion = DataRowVersion.Original;
							listGeneral.Add(pInfo);
						}
						else
						{
							wherePart += "(" + dColumn.ColumnName + " = ?)";
							pInfo = CreateParameterFromColumn("q" + (++j), dColumn);
							pInfo.SourceVersion = DataRowVersion.Original;
							listGeneral.Add(pInfo);
						}
					}
					sql = "DELETE FROM " + tableName + " WHERE (" + wherePart + ")";
					result = ProviderFactory.CreateCommand();
					result.CommandText = sql;
					listGeneral.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
					result.Connection = activeCommand.Connection;
				}
			}
			catch
			{
			}
			return result;
		}

		/// <summary>
		/// Creates an update command using the information contained in the RecordsetHelper.
		/// </summary>
		/// <returns>A DBCommand object containing an update command.</returns>
		protected DbCommand CreateUpdateCommandFromMetaData()
		{
			int i = 0, j = 0;
			DbCommand result = null;
			String tableName = getTableName(activeCommand.CommandText);
			try
			{
				if (!string.IsNullOrEmpty(tableName))
				{
					string updatePart = "";
					string wherePart = "";
					string sql = "";
					List<DbParameter> listGeneral = new List<DbParameter>();
					List<DbParameter> listWhere = new List<DbParameter>();
					DbParameter param = null;

					foreach (System.Data.DataColumn dColumn in Tables[0].Columns)
					{
						if (Tables[0].PrimaryKey != null && !(Array.Exists<DataColumn>(
							Tables[0].PrimaryKey,
							delegate(DataColumn col) { return col.ColumnName.Equals(dColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase); })
							  || dColumn.ReadOnly))
						{
							if (updatePart.Length > 0)
							{
								updatePart += " , ";
							}

							updatePart += dColumn.ColumnName + " = ?";
							listGeneral.Add(CreateParameterFromColumn("p" + (++i), dColumn));
						}
						if (wherePart.Length > 0)
						{
							wherePart += " AND ";
						}
						if (dColumn.AllowDBNull)
						{
							wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";
							param = CreateParameterFromColumn("q" + (++j), dColumn);
							param.DbType = DbType.Int32;
							param.SourceVersion = DataRowVersion.Original;
							param.SourceColumnNullMapping = true;
							param.Value = 1;
							listWhere.Add(param);
							param = CreateParameterFromColumn("q" + (++j), dColumn);
							param.SourceVersion = DataRowVersion.Original;
							listWhere.Add(param);
						}
						else
						{
							wherePart += "(" + dColumn.ColumnName + " = ?)";
							param = CreateParameterFromColumn("q" + (++j), dColumn);
							param.SourceVersion = DataRowVersion.Original;
							listWhere.Add(param);
						}
					}
					listGeneral.AddRange(listWhere);
					sql = "UPDATE " + tableName + " SET " + updatePart + " WHERE " + wherePart;
					result = ProviderFactory.CreateCommand();
					result.CommandText = sql;
					listGeneral.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
					result.Connection = activeCommand.Connection;
				}
			}
			catch
			{
			}
			return result;
		}

		/// <summary>
		/// Creates an insert command using the information contained in the RecordsetHelper.
		/// </summary>
		/// <returns>A DBCommand object containing an insert command.</returns>
		protected DbCommand CreateInsertCommandFromMetaData()
		{
			DbCommand result = null;
			int i = 0;
			String tableName = getTableName(activeCommand.CommandText);
			try
			{

				if (!string.IsNullOrEmpty(tableName))
				{
					List<DbParameter> parameters = new List<DbParameter>();
					string fieldsPart = "";
					string valuesPart = "";
					string sql = "";
					foreach (System.Data.DataColumn dColumn in Tables[0].Columns)
					{
						if (!dColumn.ReadOnly)
						{
							if (fieldsPart.Length > 0)
							{
								fieldsPart += ", ";
								valuesPart += ", ";
							}

							fieldsPart += dColumn.ColumnName;
							valuesPart += "?";
							parameters.Add(CreateParameterFromColumn("p" + (++i), dColumn));
						}
					}
					sql = "INSERT INTO " + tableName + " (" + fieldsPart + ") VALUES (" + valuesPart + ")";
					result = ProviderFactory.CreateCommand();
					result.CommandText = sql;
					parameters.ForEach(delegate(DbParameter p) { result.Parameters.Add(p); });
					result.Connection = activeCommand.Connection;
				}
			}
			catch
			{
			}
			return result;
		}

		/// <summary>
		/// Assigns the InsertCommand to the adaptar parameter
		/// </summary>
		/// <param name="adapter">DbDataAdapter</param>
		protected void CompleteInsertCommand(DbDataAdapter adapter)
		{
			String extraCommandText = "";
			String extraCommandText1 = "";
			Dictionary<String, String> identities = IdentityColumnsManager.GetIndentityInformation(getTableName(activeCommand.CommandText));
			if (identities != null)
			{
				foreach (KeyValuePair<String, String> identityInfo in identities)
				{
					DbParameter outPar;
					adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
					//outPar.ParameterName = (isOracle ? ":" : "@") + identityInfo.Key;

					if (DatabaseType == DatabaseType.Oracle)
					{
						outPar = adapter.InsertCommand.Parameters[":" + identityInfo.Key];
						//todo: check for null
						outPar.Direction = ParameterDirection.Output;
						outPar.DbType = GetDBType(Tables[0].Columns[identityInfo.Key].DataType);

						if (String.IsNullOrEmpty(extraCommandText))
						{
							extraCommandText = " RETURNING " + identityInfo.Key;
							extraCommandText1 = " INTO :" + identityInfo.Key;
						}
						else
						{
							extraCommandText += ", " + identityInfo.Key;
							extraCommandText1 += ", :" + identityInfo.Key;
						}
					}
					else if (DatabaseType != DatabaseType.Undefined)
					{
						extraCommandText = MsInsertCommandCompletion(adapter, identityInfo.Key, extraCommandText);
					}
				}
			}
			else
			{
				extraCommandText = MsInsertCommandCompletion(adapter, autoIncrementCol, extraCommandText);
			}
			adapter.InsertCommand.CommandText += extraCommandText + extraCommandText1;
		}

		/// <summary>
		/// SqlServer Identity value for last insert execution.
		/// </summary>
		/// <param name="adapter">DbDataAdapter to set</param>
		/// <param name="identityInfo">Name of Identity field</param>
		/// <param name="extraCommandText">used to set the query to get the identity value</param>
		/// <returns>returns the entire query in the adapter</returns>
		protected string MsInsertCommandCompletion(DbDataAdapter adapter, String identityInfo, string extraCommandText)
		{
			if (!String.IsNullOrEmpty(identityInfo))
			{
				DbParameter outPar;
				outPar = providerFactory.CreateParameter();
				outPar.ParameterName = "@" + identityInfo;
				outPar.DbType = GetDBType(Tables[0].Columns[identityInfo].DataType);
				outPar.Direction = ParameterDirection.Output;
				outPar.SourceColumn = identityInfo;
				extraCommandText += " SELECT @" + identityInfo + " = SCOPE_IDENTITY()";
				adapter.InsertCommand.Parameters.Add(outPar);
			}
			return extraCommandText;
		}

		/// <summary>
		/// Analyzes an SQL Query and obtain the name of the table.
		/// </summary>
		/// <param name="sqlSelectQuery">The SQL query containing the name of the table.</param>
		/// <param name="useParam"> When use the first table name in the query, by default is false.</param>
		/// <returns>The SQL query containing the name of the table.</returns>
		protected string getTableName(string sqlSelectQuery, bool useParam = false)
		{
			Match mtch;
			String query = activeCommand.CommandText;
			if (!string.IsNullOrEmpty(query))
			{
				if (activeCommand.CommandType == CommandType.Text)
				{
					if (useParam)
						mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"FROM\s+([^ ,]+)(?:\s*,\s*([^ ,]+))*\s+", RegexOptions.IgnoreCase);
					else
                        mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+([\w.]+)[ \t]*.*$", RegexOptions.IgnoreCase);

					if (mtch != Match.Empty)
					{
						return mtch.Groups[1].Value.Trim();
					}
					else if (useParam)
					{
                        mtch = Regex.Match(query.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+([\w.]+)[ \t]*.*$", RegexOptions.IgnoreCase);
						if (mtch != Match.Empty)
							return mtch.Groups[1].Value.Trim();
					}
				}
				else if (activeCommand.CommandType == CommandType.TableDirect)
				{
					return query;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Creates a Dbparameter obtaining the information from a DataColumn object.
		/// </summary>
		/// <param name="paramName">The name for the parameter.</param>
		/// <param name="dColumn">The DataColumn object to extract the information from.</param>
		/// <returns>A new DBParameter object containing the desired configuration.</returns>
		protected DbParameter CreateParameterFromColumn(string paramName, System.Data.DataColumn dColumn)
		{
			DbParameter parameter;
			parameter = ProviderFactory.CreateParameter();
			parameter.ParameterName = paramName;
			parameter.DbType = GetDBType(dColumn.DataType);
			parameter.SourceColumn = dColumn.ColumnName;
			parameter.SourceVersion = DataRowVersion.Current;
			return parameter;
		}

		/// <summary>
		/// Finds the index in the RecordsetHelper for the “value”.
		/// </summary>
		/// <param name="value">The DataRow to look for.</param>
		/// <returns>The index number if is found, otherwise -1.</returns>
		protected int findBookmarkIndex(DataRow value)
		{
			if (!UsingView)
			{
				return Tables[0].Rows.IndexOf(value);
			}
			int result = -1;
			for (int i = 0; i < currentView.Count; i++)
			{
				if (currentView[i].Row == value)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Sets a new value for a specific index column.
		/// </summary>
		/// <param name="columnIndex">Index of the column to be updated.</param>
		/// <param name="value">New value for column.</param>
		public virtual void SetNewValue(int columnIndex, object value)
		{
			CurrentRow[columnIndex] = value;
		}

		/// <summary>
		/// Sets the filter for the RecordsetHelper.
		/// </summary>
		/// <param name="filter">The filter to apply to this RecordsetHelper.</param>
		protected virtual void SetFilter(String filter)
		{
			currentView.RowFilter = filter;
		}

		/// <summary>
		/// Sets the filter for the RecordsetHelper.
		/// </summary>
		/// <param name="filter">The filter to apply to this RecordsetHelper.</param>
		protected virtual void SetFilter(DataViewRowState filter)
		{
			currentView.RowStateFilter = filter;
		}

		/// <summary>
		/// Sets the filter for the RecordsetHelper.
		/// </summary>
		/// <param name="filter">The filter to apply to this RecordsetHelper.</param>
		protected virtual void SetFilter(DataRow[] filter)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// Sets the AfterMove EventHandler.
		/// </summary>
		private void OnAfterMove()
		{
			if (AfterMove != null)
			{
				AfterMove(this, new EventArgs());
			}
		}

		/// <summary>
		/// Sets the AfterQuery eventHandler.
		/// </summary>
		private void OnAfterQuery()
		{
			if (AfterQuery != null)
			{
				AfterQuery(this, new EventArgs());
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.MarshalByValueComponent"/> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. 
		///                 </param>
		protected override void Dispose(bool disposing)
		{
			if (opened)
			{
				Close();
			}
			base.Dispose(disposing);
		}

		#region Paging

		// This region provides the logic to deal with pages. However, its functionality difer from the one 
		// provided by old ADODB technology.
		// 
		// Next members provide functionality to determine the current page, the size of the page, but real paging
		// must be implemented using these members.

		/// <summary>
		/// Gets/Sets the number of rows per page.
		/// </summary>
		public int PageSize
		{
			get
			{
				return pagesize;
			}
			set
			{
				pagesize = value;
			}
		}

		/// <summary>
		/// Gets the number of pages.
		/// </summary>
		public virtual int PageCount
		{
			get
			{
				if (this.PageSize == 0)
				{
					return 0;
				}
				else
				{
					return (int)System.Math.Ceiling((float)this.RecordCount / (float)this.PageSize);
				}
			}
		}

		#endregion
	}
}