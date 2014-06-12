using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UpgradeHelpers.VB6.DB
{
    /// <summary>
    /// It simulates a VB6 Field, contains the Value and FieldMetadata
    /// </summary>
    public class FieldHelper
    {
        private RecordSetHelper _rs;
        private object _column;
        private bool _columnTypeNumeric;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs">The recordset for this Field.</param>
        /// <param name="column">The column index or column string to get the Field in the recordset.</param>
        /// <param name="columnTypeNumeric">Indicates if column is an index or string value.</param>
        public FieldHelper(RecordSetHelper rs, object column, bool columnTypeNumeric)
        {
            this._rs = rs;
            this._column = column;
            this._columnTypeNumeric = columnTypeNumeric;
        }

        /// <summary>
        /// Value for this Field
        /// </summary>
        public virtual object Value
        {
            get
            { 
                if (_columnTypeNumeric)
                    return _rs[(int)_column];
                else
                    return _rs[(String)_column];
            }
            set
            {
                if (_columnTypeNumeric)
                    _rs[(int)_column] = value;
                else
                    _rs[(int)_column] = value;
            }
        }

        /// <summary>
        /// Metadata for this Field
        /// </summary>
        public virtual DataColumn FieldMetadata
        {
            get
            {
                if (_columnTypeNumeric)
                    return _rs.FieldsMetadata[(int)_column];
                else
                    return _rs.FieldsMetadata[(String)_column];
            }
        }
    }
}
