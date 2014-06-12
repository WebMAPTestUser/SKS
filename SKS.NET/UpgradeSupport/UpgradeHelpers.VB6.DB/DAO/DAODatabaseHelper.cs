using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections.Specialized;



namespace UpgradeHelpers.VB6.DB.DAO
{
    /// <summary>
    /// 
    /// </summary>
    public class DAODatabaseHelper
    {

        private DbConnection _connection;
        private TableDefsHelper _tableDefs;

		static Dictionary<string, WeakReference> previousCommands = new Dictionary<string, WeakReference>();

		/// <summary>
		/// QueryDefs
		/// </summary>
		/// <param name="QueryDefName"></param>
		/// <returns></returns>
		public DbCommand QueryDefs(string QueryDefName)
		{
			lock (previousCommands)
			{
				WeakReference previousRef;
				DbCommand previous;
				previousCommands.TryGetValue(QueryDefName, out previousRef);
				if (previousRef == null || !previousRef.IsAlive)
				{
					DbCommand command = this.CreateCommand();
					command.CommandText = QueryDefName;
					command.CommandType = CommandType.StoredProcedure;
					previousCommands[QueryDefName] = new WeakReference(command);
					previous = command;
				}
				else
				{
					previous = previousRef.Target as DbCommand;
				}
				previous.Transaction = TransactionManager.GetTransaction(Connection);
				return previous;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public DbConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public TableDefHelper this[string table]
        {
            get
            {
				string tableStr = table;
				int index = -1;
				if ((index = table.IndexOf("QCCSTemp.")) != -1)
				{
					tableStr = table.Substring(index + "QCCSTemp.".Length);
				}
				return _tableDefs[tableStr];                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TableDefsHelper TableDefs
        {
            get
            {
                return _tableDefs;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public DAODatabaseHelper(DbConnection connection)
        {
            _connection = connection;
            DbTypesConverter.ProviderTypeMap = BuildProviderTypeMap();

            _tableDefs = new TableDefsHelper(connection);
            DataTable dbTables = _connection.GetSchema("Tables");
            foreach (DataRow r in dbTables.Rows)
            {
                TableDefHelper tableDef = new TableDefHelper(r["TABLE_NAME"].ToString(), r["TABLE_NAME"].ToString(), true);
                AddColumnsToTableDef(tableDef);
                AddIndexesToTableDef(tableDef);
                _tableDefs.Add(tableDef,false);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableDef"></param>
        private void AddIndexesToTableDef(TableDefHelper tableDef)
        {
            DataTable dbIndexes = _connection.GetSchema("Indexes");
            foreach (DataRow r in dbIndexes.Select("TABLE_NAME = '" + tableDef.TableName + "'"))
            {
                DataColumn newCol = new DataColumn();
                newCol.ColumnName = r["COLUMN_NAME"].ToString();
                newCol.AllowDBNull = r["NULLS"].ToString() == "1";
                newCol.DataType = DbTypesConverter.ProviderTypeToType(r["TYPE"].ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableDef"></param>
        private void AddColumnsToTableDef(TableDefHelper tableDef)
        {
            DataTable dbColumns = _connection.GetSchema("Columns");
            foreach (DataRow r in dbColumns.Select("TABLE_NAME = '" + tableDef.TableName + "'"))
            {
                /*System.Diagnostics.Debug.WriteLine("-------------");
                foreach (DataColumn c in r.Table.Columns)
                {
                    System.Diagnostics.Debug.WriteLine("\t" + c.ColumnName + " " + r[c.ColumnName]);
                }*/

                DataColumn field = new DataColumn();
                field.ColumnName = r["COLUMN_NAME"].ToString();
                field.DataType = DbTypesConverter.ProviderTypeToType(r["DATA_TYPE"].ToString());
                if ( r["DATA_TYPE"].ToString() == "130" ) // strings
                    field.MaxLength = int.Parse(r["CHARACTER_MAXIMUM_LENGTH"].ToString());
                tableDef.Columns.Add(field);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableDefHelper CreateTableDef(string name)
        {
            return new TableDefHelper(name, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceTableName"></param>
        /// <returns></returns>
        public TableDefHelper CreateTableDef(string name, string sourceTableName)
        {
            return new TableDefHelper(name, sourceTableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DAORecordSetHelper OpenRecordset(string name)
        {
            return OpenRecordset(name, AdoFactoryManager.Default.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="factoryName"></param>
        /// <returns></returns>
      
        public DAORecordSetHelper OpenRecordset(string name, string factoryName)
        {
            DAORecordSetHelper rs = new DAORecordSetHelper(factoryName);
            rs.ActiveConnection = _connection;
            rs.Source = name;
            rs.Open();
            return rs;
        }

        private Dictionary<string, KeyValuePair<string, string>> BuildProviderTypeMap()
        {
            DataTable dataTypesSchema = _connection.GetSchema("DataTypes");
            Dictionary<string, KeyValuePair<string, string>> dict = new Dictionary<string, KeyValuePair<string, string>>();

            foreach (DataRow row in dataTypesSchema.Rows)
            {
                dict[row["NativeDataType"].ToString()] = new KeyValuePair<string, string>(row["TypeName"].ToString(), row["DataType"].ToString());

                /*System.Diagnostics.Debug.WriteLine("------------------");
                foreach (DataColumn c in row.Table.Columns)
                {
                    System.Diagnostics.Debug.WriteLine("\t" + c.ColumnName + " " + row[c.ColumnName]);
                }*/
            }

            return dict;
        }

		/// <summary>
		/// CreateCommand
		/// </summary>
		/// <returns></returns>
		public DbCommand CreateCommand()
		{
			return (_connection != null) ? _connection.CreateCommand() : AdoFactoryManager.GetFactory().CreateCommand();
		}
    }
}
