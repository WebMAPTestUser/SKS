using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UpgradeHelpers.VB6.DB.DAO
{

    /// <summary>
    /// 
    /// </summary>
    public class TableDefHelper:DataTable   
    {
        private String _sourceTableName;
        private bool _fromDatabase = false;

        private IndexesHelper _indexes = new IndexesHelper();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceTableName"></param>
        /// <param name="fromDatabase"></param>
        internal TableDefHelper(string name, string sourceTableName, bool fromDatabase)
            : base(name)
        {
            _sourceTableName = sourceTableName;
            _fromDatabase = fromDatabase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceTableName"></param>
        public TableDefHelper(string name, string sourceTableName) : this(name, sourceTableName, false) { }

        
        /// <summary>
        /// 
        /// </summary>
        public IndexesHelper Indexes
        {
            get { return _indexes; }
            set { _indexes = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dbtype"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public DataColumn CreateField(string columnName, DbType dbtype, object size)
        {
            DataColumn field = new DataColumn();
            field.ColumnName = columnName;
            field.DataType = DbTypesConverter.DbTypeToType(dbtype);
            if (size != null)
            {
                field.MaxLength = int.Parse(size.ToString());
            }
            return field;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IndexHelper CreateIndex(string name)
        {
            IndexHelper index = new IndexHelper();
            index.Name = name;
            return index;
        }


        /// <summary>
        /// 
        /// </summary>
		public string Name { get { return _sourceTableName; } }
    }
}
