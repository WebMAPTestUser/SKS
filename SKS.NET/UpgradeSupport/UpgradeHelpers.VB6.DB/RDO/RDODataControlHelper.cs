using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UpgradeHelpers.VB6.DB.Controls;
using System.Data.Common;

namespace UpgradeHelpers.VB6.DB.RDO
{
    /// <summary>
    /// This class implements the MSRDC functionality.
    /// </summary>
    [ToolboxItem(true)]
    public partial class RDODataControlHelper : InternalRDODataControlHelper
    {
        /// <summary>
        /// Exposes the reposition event.
        /// </summary>
        public event EventHandler Reposition = null;

        /// <summary>
        /// Raises the Reposition event.
        /// </summary>
        protected virtual void OnReposition()
        {
            if (Reposition != null)
                Reposition(this, new EventArgs());
        }

        /// <summary>
        /// Creates a new Control instance.
        /// </summary>
        public RDODataControlHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets and sets the Connection object used by the underlying RDORecordsetHelper 
        /// to make all the database operations.
        /// </summary>
        public DbConnection Connection
        {
            get { return Recordset.ActiveConnection; }
            set { Recordset.ActiveConnection = value; }
        }

        /// <summary>
        /// Holds the lock type for the underlying recordset.
        /// </summary>
        private LockTypeConstants _LockType = LockTypeConstants.rdConcurRowVer;

        /// <summary>
        /// Gets and sets the lock type for the underlying recordset.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets/Sets the lock type for the underlying recordset")]
        public LockTypeConstants LockType
        {
            get
            {
                return _LockType;
            }
            set
            {
                _LockType = value;
            }
        }

        /// <summary>
        /// Holds RemoteData control's data source name.
        /// </summary>
        private string _DataSourceName = string.Empty;

        /// <summary>
        /// Gets/sets RemoteData control's data source name.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets/sets RemoteData control's data source name")]
        [Category("Connection")]
        public string DataSourceName
        {
            get
            {
                return _DataSourceName;
            }
            set
            {
                _DataSourceName = value;
                UpdateConnectionInfo();
            }
        }

        /// <summary>
        /// Updates the connection string property according to the specific information given 
        /// to this control such as Datasource.
        /// </summary>
        protected override void UpdateConnectionInfo()
        {
            if (InDesignMode || OnInitialization)
                return;
            base.UpdateConnectionInfo();
            DbConnectionStringBuilder connbuilder = new DbConnectionStringBuilder();
            if (!string.IsNullOrEmpty(DataSourceName))
            {
                connbuilder.ConnectionString = ConnectionString;
                object o = null;
                if (!String.IsNullOrEmpty(DataSourceName))
                {
                     connbuilder.TryGetValue("DSN", out o);
                    if (o == null)
                        connbuilder.Add("DSN", DataSourceName);
                    else
                        connbuilder["DSN"] = DataSourceName;
                }
                _ConnectionString = connbuilder.ConnectionString;
            }
        }

        /// <summary>
        /// Finishes the initialization process.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();
            OnInitialization = false;
            UpdateConnectionInfo();
        }

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        public void BeginTrans()
        {
            TransactionManager.Enlist(Connection);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void CommitTrans()
        {
            TransactionManager.Commit(Connection);
        }

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        public void RollbackTrans()
        {
            TransactionManager.Rollback(Connection);
        }

        /// <summary>
        /// Updates the underlying recordset.
        /// </summary>
        public void UpdateRow()
        {
            Recordset.Update();
        }
    }
}
