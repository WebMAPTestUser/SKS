using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace UpgradeHelpers.VB6.DB.DAO
{

    /// <summary>
    /// Table Defs Helper, list of TableDef definitions
    /// </summary>
    public class TableDefsHelper : List<TableDefHelper>
    {
        DbConnection _connection;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public TableDefsHelper(DbConnection connection)
        {
            _connection = connection;
        }

        internal void Add(TableDefHelper item, bool newTableToDB)
        {
            TableDefHelper tb = this[item.TableName];
            if (tb == null)
            {
                base.Add(item);
                if (newTableToDB)
                {
                    DbCommand command = _connection.CreateCommand();
                    StringBuilder strFields = new StringBuilder();
                    foreach (DataColumn field in item.Columns)
                    {
                        strFields.Append(string.Format("{0} {1}", field.ColumnName, DbTypesConverter.TypeToProviderType(field.DataType)));
                        if (field.MaxLength > 0)
                        {
                            strFields.Append(string.Format("({0})", field.MaxLength));
                        }
                        strFields.Append(",");
                    }
                    if (strFields.Length > 0)
                    {
                        strFields.Remove(strFields.Length - 1, 1);
                        command.CommandText = string.Format("CREATE TABLE {0} ({1})", item.TableName, strFields);
                        command.ExecuteNonQuery();

                        if (item.Indexes.Count > 0)
                        {
                            foreach (IndexHelper idx in item.Indexes)
                            {
                                strFields.Length = 0;
                                strFields.Capacity = 0;
                                foreach (DataColumn column in idx.Fields)
                                {
                                    strFields.Append(string.Format("{0},",column.ColumnName));
                                }
                                if ( strFields.Length > 0 )
                                {
                                    strFields.Remove(strFields.Length - 1, 1);
                                    DbCommand idxcmd = _connection.CreateCommand();

                                    // idx.Foreign is readonly in DAO, but is automatically set when the Field has the ForeignTable property assigned.

                                    if (idx.Primary)
                                    {
                                        idxcmd.CommandText = string.Format("ALTER TABLE {0} ADD PRIMARY KEY ({1})", item.TableName, strFields);
                                    }
                                    else if (idx.Unique)
                                    {
                                        idxcmd.CommandText = string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", idx.Name, item.TableName, strFields);
                                    }
                                    else
                                    {
                                        idxcmd.CommandText = string.Format("CREATE INDEX {0} ON {1} ({2})", idx.Name, item.TableName, strFields);
                                    }

                                    if (idx.IgnoreNulls && !idx.Primary)
                                    {
                                        idxcmd.CommandText += " WITH DISALLOW NULL";
                                    }
                                    try
                                    {
                                        idxcmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public new void Add(TableDefHelper item)
        {
            Add(item, true);
        }
        /// <summary>
        /// Returns true if tableName is found
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableDefHelper Find(string tableName)
        {
            TableDefHelper found = null;
            foreach (TableDefHelper tb in this)
            {
                if (tb.TableName.CompareTo(tableName) == 0)
                {
                    found = tb;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableDefHelper this[string tableName]
        {
            get
            {
                return Find(tableName);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void Remove(string tableName)
        {
            TableDefHelper tb = Find(tableName);
            if (tb != null)
            {
                DbCommand command = _connection.CreateCommand();
                command.CommandText = string.Format("DROP TABLE {0}", tb.TableName);
                command.ExecuteNonQuery();
                this.Remove(tb);
            }
        }
    }
}
