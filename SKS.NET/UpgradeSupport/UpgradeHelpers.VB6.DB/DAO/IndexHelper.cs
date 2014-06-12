using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UpgradeHelpers.VB6.DB.DAO
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexHelper
    {
        private string _name;
        private bool _primary;
        private bool _unique;
        private bool _ignorenulls;
        private bool _required;
        private bool _clustered;
        private bool _foreign;

        private List<DataColumn> _fields = new List<DataColumn>();

        /// <summary>
        /// 
        /// </summary>
        public IndexHelper()
        {
            _name = "default";
            _primary = false;
            _unique = false;
            _ignorenulls = false;
            _required = false;
            _clustered = false;
            _foreign = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get{ return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Unique
        {
            get { return _unique; }
            set { _unique = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Primary
        {
            get { return _primary; }
            set { _primary = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreNulls
        {
            get { return _ignorenulls; }
            set { _ignorenulls = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Foreign
        {
            get { return _foreign; }
            set { _foreign = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Clustered
        {
            get { return _clustered; }
            set { _clustered = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DataColumn> Fields
        {
            get { return _fields; }
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
    }
}
