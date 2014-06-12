using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data.Common;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using UpgradeHelpers.VB6.DB.DAO;


namespace UpgradeHelpers.VB6.DB.Controls
{


	/// <summary>
	/// The ValidatingEvent arguments.
	/// </summary>
	public class ValidatingEventArgs : EventArgs
	{
		/// <summary>
		/// Creates a new ValidatingEventArgs instance.
		/// </summary>
		/// <param name="Action">The action that raises the event.</param>
		/// <param name="Save">Determines if needs to save.</param>
		public ValidatingEventArgs(int Action, int Save)
		{
			this._Action = Action;
			this._Save = Save;
		}
		private int _Action;
		/// <summary>
		/// Gets the event action.
		/// </summary>
		public int Action
		{
			get
			{
				return _Action;
			}
			set
			{
				_Action = value;
			}
		}

		private int _Save;
		/// <summary>
		/// Gets the Save value.
		/// </summary>
		public int Save
		{
			get
			{
				return _Save;
			}
			set
			{
				_Save = value;
			}
		}
	}

    /// <summary>
    /// BOF action values.
    /// </summary>
    public enum BOFActionEnum 
    {
        /// <summary>
        /// MoveFirst
        /// </summary>
        MoveFirst, 
        /// <summary>
        /// Begin Of File
        /// </summary>
        BOF 
    };
    /// <summary>
    /// EOF action values.
    /// </summary>
    public enum EOFActionEnum 
    { 
        /// <summary>
        /// Move Last
        /// </summary>
        MoveLast, 
        /// <summary>
        /// End of file
        /// </summary>
        EOF, 
        /// <summary>
        /// Add Record state
        /// </summary>
        Add 
    };

    /// <summary>
    /// Base class for the supported Data Controls.
    /// </summary>
    /// <typeparam name="T">The type of the specific recordset helper to be handled by the control.</typeparam>
    public partial class DataControlHelper<T> : UserControl, ISupportInitialize where T : RecordSetHelper, new()
    {

        private BindingSource source = new BindingSource();

        private T recordSet;

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
        /// Public ValidatingEvent.
        /// </summary>
        public new event ValidatingEventHandler Validating = null;
        /// <summary>
        /// Raises the validating event.
        /// </summary>
        /// <param name="Action">The action that raises the event.</param>
        /// <param name="Save">Determines if needs to save.</param>
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
        /// Delegate to wire the ValidatingEvent.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void ValidatingEventHandler(object sender, ValidatingEventArgs e);
        /// <summary>
        /// OnIntialization state
        /// </summary>
        protected bool OnInitialization = false;
        
        /// <summary>
        /// Creates a new DataControlHelper.
        /// </summary>
        public DataControlHelper()
        {
            InitializeComponent();
            //base.Enabled = false;
            this.SetStyle(ControlStyles.Selectable, false);
			source.CurrentChanged += new EventHandler(BindingContext_onCurrentChanged);
			source.ListChanged += new ListChangedEventHandler(BindingContext_onListChanged);
        }

        /// <summary>
        /// Creates a new DataControlHelper.
        /// </summary>
        /// <param name="container">The container to add the new instance.</param>
		public DataControlHelper(IContainer container)
			: this()
        {
            container.Add(this);
            
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~DataControlHelper()
        {
            try
            {
                if (Recordset != null)
                    Recordset.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Gets and sets the Text property.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return l_caption.Text;
            }
            set
            {
                l_caption.Text = value;
            }
        }
   
         private String _RecordSource = null;
        /// <summary>
         /// Returns/sets the underlying table name, SQL Statement or stored procedure name object used to populate the data control
        /// </summary>
        [Browsable(true),Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Returns/sets the underlying table name, SQL Statement or stored procedure name object used to populate the data control")]
        public string RecordSource
        {
            get
            {
                return _RecordSource;
            }
            set
            {
                _RecordSource = value;
            }
        }

        private string _FactoryName;
        /// <summary>
        /// Returns/sets the factory name used to create the ADO machinery objects
        /// </summary>
        [Browsable(true), Category("General Configuration"),DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Returns/sets the factory name used to create the ADO machinery objects")]
        public string FactoryName
        {
            get { return _FactoryName; }
            set { _FactoryName = value; }
        }

        /// <summary>
        /// Gets and sets the control's back color.
        /// </summary>
        [Browsable(true), Category("Appearance")]
        public new System.Drawing.Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.l_caption.BackColor = value;
                this.b_first.BackColor = System.Drawing.SystemColors.Control;
                this.b_last.BackColor = System.Drawing.SystemColors.Control;
                this.b_next.BackColor = System.Drawing.SystemColors.Control;
                this.b_prev.BackColor = System.Drawing.SystemColors.Control;
            }
        }


        /// <summary>
        /// Gets and sets the control's font.
        /// </summary>
        [Browsable(true), Category("Appearance")]
        public new System.Drawing.Font Font
        {
            get
            {
                return l_caption.Font;
            }
            set
            {
                l_caption.Font = value;
            }
        }

        /// <summary>
        /// Gets and sets the control's fore color.
        /// </summary>
        [Browsable(true), Category("Appearance")]
        public new System.Drawing.Color ForeColor
        {
            get
            {
                return l_caption.ForeColor;
            }
            set
            {
                l_caption.ForeColor = value;
            }
        }

        
        private BOFActionEnum _BOFAction = BOFActionEnum.MoveFirst;
        /// <summary>
        /// Get/Set BOF action value.
        /// </summary>
        [Browsable(true),Category("General Configuration"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BOFActionEnum BOFAction
        {
            get
            {
                return _BOFAction;
            }
            set
            {
                _BOFAction = value;
            }
        }
        private EOFActionEnum _EOFAction = EOFActionEnum.MoveLast;
        /// <summary>
        /// Gets and sets the EOF action.
        /// </summary>
        [Browsable(true), Category("General Configuration"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EOFActionEnum EOFAction
        {
            get
            {
                return _EOFAction;
            }
            set
            {
                _EOFAction = value;
            }
        }
        
        private CommandType _QueryType = CommandType.Text;
        /// <summary>
        /// Gets/Sets the recordset's command type.
        /// </summary>
        [Browsable(true), Category("General Configuration"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets/Sets the recordset's command type")]
        public CommandType QueryType
        {
            get
            {
                return _QueryType;
            }
            set
            {
                _QueryType = value;
            }
        }
        private int _QueryTimeout = 30;
        /// <summary>
        /// Gets/sets the underlying command's timeout.
        /// </summary>
        [Browsable(true), Category("General Configuration"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets/sets the underlying command's timeout ")]
        public int QueryTimeout
        {
            get
            {
                return _QueryTimeout;
            }
            set
            {
                _QueryTimeout = value;
            }
        }

        private bool _Enabled = true;
        /// <summary>
        /// Indicates whether the control is enabled.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates whether the control is enabled")]
        [Category("Behavior")]
        public new bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
                if (!_Enabled)
                    base.Enabled = _Enabled;
                else if (OnInitialization || Recordset != null)
                    base.Enabled = _Enabled;
            }
        }

        /// <summary>
        /// Gets/Sets the control's underlying recordset.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T Recordset
        {
            get
            {
                if (recordSet == null)
                {
                    recordSet = new T();
                    InitRecordset();
                }
                return recordSet;
            }
            set
            {
                if (recordSet != value)
                {
                    DeInitRecordset();
                    recordSet = value;
                    if (recordSet != null)
                    {
                        InitRecordset();
                        if (Enabled)
                            base.Enabled = true;
                    }
                    else
                        base.Enabled = false;
                    
                }
            }
        }
        /// <summary>
        /// Inits the recordset.
        /// </summary>
        protected virtual void InitRecordset()
        {
            
            if (Recordset.Opened)
            {
                RecordSource = Recordset.SqlQuery;
                ConnectionString = Recordset.ConnectionString;
				//AIS: apparently unnecessary ReBind() invocation for DAO
                //ReBind();
                BindDataSet();
            }
            else if (!(string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(_FactoryName)||string.IsNullOrEmpty(_RecordSource)))
            {
                Recordset.ProviderFactory = AdoFactoryManager.GetFactory(_FactoryName);
                Recordset.DatabaseType = AdoFactoryManager.GetFactoryDbType(_FactoryName);

                
                Recordset.ConnectionString = ConnectionString;
                CommandType cmdtype = Recordset.getCommandType(_RecordSource);
                _QueryType = _QueryType == cmdtype ? _QueryType : cmdtype;
                if (Recordset.ProviderFactory != null)
                {
                    DbCommand command = Recordset.CreateCommand(_RecordSource, _QueryType);
                    _QueryType = command.CommandType;
                    command.CommandTimeout = _QueryTimeout;
                    Recordset.Source = command;
                }
            }
            Recordset.AfterMove += new EventHandler(Recordset_onAfterMove);
            Recordset.AfterQuery += new EventHandler(Recordset_onAfterQuery);
        }

        /// <summary>
        /// De inits the recordset.
        /// </summary>
        protected virtual void DeInitRecordset()
        {
   
            if (Recordset != null)
            {
                if (Recordset.Opened)
                {
                    UnBindDataSet();
                    Recordset.Close();
                }
                Recordset.AfterMove -= new EventHandler(Recordset_onAfterMove);
                Recordset.AfterQuery -= new EventHandler(Recordset_onAfterQuery);
            }
        }

       
        private void b_first_Click(object sender, EventArgs e)
        {
            if (this.Recordset != null)
            {
                if (this.Recordset.RecordCount > 0)
                    this.Recordset.MoveFirst();

                ValidateButtonStatus();
            }
        }

        private void b_prev_Click(object sender, EventArgs e)
        {
            if (this.Recordset != null)
            {
                if (this.Recordset.RecordCount > 0)
                {
                    if (Recordset.BOF)
                    {
                        if (_BOFAction == BOFActionEnum.MoveFirst)
                        
                            this.Recordset.MoveFirst();
                    }
                    else
                        this.Recordset.MovePrevious();
                }
                ValidateButtonStatus();
            }
        }

        private void b_next_Click(object sender, EventArgs e)
        {
            if (Recordset != null)
            {
                if (Recordset.EOF)
                {
                    switch (_EOFAction)
                    {
                        case EOFActionEnum.Add:
                                Recordset.AddNew();
                                Recordset.Update();
                                break;
                        case EOFActionEnum.MoveLast:
                            Recordset.MoveLast();
                            break;
                        default:
                            break;
                    }
                }
                else
                    Recordset.MoveNext();
                ValidateButtonStatus();
            }
        }

        private void b_last_Click(object sender, EventArgs e)
        {
            if (this.Recordset != null)
            {
                if (this.Recordset.RecordCount > 0)
                    this.Recordset.MoveLast();

                ValidateButtonStatus();
            }
        }

        private void ValidateButtonStatus()
        {
            b_prev.Enabled = this.Recordset.CanMovePrevious;
            b_next.Enabled = !this.Recordset.EOF;
        }

        /// <summary>
        /// Try to update the information to connect to the database.
        /// </summary>
        protected virtual void UpdateConnectionInfo()
        {
            
            if(OnInitialization)
                return;
            if (!IsConnectionAvailable())
                return;
            DbConnectionStringBuilder connbuilder = new DbConnectionStringBuilder();
            connbuilder.ConnectionString = ConnectionString;
            object o = null;
            if (!String.IsNullOrEmpty(UserName))
            {
                connbuilder.TryGetValue("User ID", out o);
                if (o == null)
                    connbuilder.Add("User ID",UserName);
                else
                    connbuilder["User ID"] = UserName;
            }
            if (!String.IsNullOrEmpty(Password))
            { 
                connbuilder.TryGetValue("Password", out o);
                if (o == null)
                    connbuilder.Add("Password", Password);
                else
                    connbuilder["Password"] = Password;
            }
            _ConnectionString = connbuilder.ConnectionString;
        }

        /// <summary>
        /// It will try to recreate the resultset based on the values of the properties.
        /// </summary>
        private void RefreshResultSet()
        {
            if (OnInitialization)
            {
                return;
            }
            if (!IsConnectionAvailable())
            {
                return;
            }
            DeInitRecordset();
            InitRecordset();
            Recordset.Requery();
        }

        private void Recordset_onAfterMove(object sender, EventArgs e)
        {
            ReBind();
        }

        private void Recordset_onAfterQuery(object sender, EventArgs e)
        {
            BindDataSet();
        }

		/// <summary>
		/// This method is called each time you change the currency of the BindingSource object
		/// BindingSource is like a list and the Currency is like the index to the current record inside
		/// that list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void BindingContext_onCurrentChanged(object sender, EventArgs e)
        {
			if (Recordset.disableEventsWhileDeleting) return;
			if (Recordset.CurrentPosition != -1 && Recordset.CurrentRow != null && Recordset.CurrentRow.RowState == DataRowState.Deleted)
			{
				Recordset.Update();
			}
			int bindingSourcePosition = ((BindingSource)sender).Position;
			if (bindingSourcePosition != Recordset.CurrentPosition)
            {
				if (bindingSourcePosition > Recordset.RecordCount ||
					(Recordset.RecordCount == 0 && bindingSourcePosition == 0 && Recordset.CurrentPosition == -1))
                {
					Recordset.CurrentPosition = bindingSourcePosition;
				}
				else if (bindingSourcePosition >= 0)
				{
					int senderPosition = bindingSourcePosition;
					//Recordset.Update();
                    Recordset.CurrentPosition = senderPosition;
                    source.Position = senderPosition;
                }
            }
        }
		bool specialListChangedFlag = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BindingContext_onListChanged(object sender, ListChangedEventArgs e)
		{
			if (Recordset.disableEventsWhileDeleting) return;
			if (specialListChangedFlag) return;
			try
			{
				specialListChangedFlag = true;
				if (e.ListChangedType != ListChangedType.Reset)
				{
					if (e.NewIndex < this.Recordset.RecordCount)
					{
						recordSet.Update();
					}
				}
			}
			finally
			{
				specialListChangedFlag = false;
			}
		}


        internal string _ConnectionString = string.Empty;
        /// <summary>
        /// Properties to change the way the recorset is connected to a datasource.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies the connection string.")]
        [Category("Connection")]
        [Editor(typeof(ConnectionStringEditor), typeof(UITypeEditor))] 
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
                UpdateConnectionInfo();
            }
        }

        private string _UserName = string.Empty;
        /// <summary>
        /// Specifies user ID.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies user ID")]
        [Category("Connection")]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                UpdateConnectionInfo();
            }
        }


        private string _Password = string.Empty;
        /// <summary>
        /// Password used during creation of the connection.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Password used during creation of the connection")]
        [Category("Connection")]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                UpdateConnectionInfo();
            }
        }
       
        private void ReBind()
        {
            if (source.Position != Recordset.CurrentPosition)
                source.Position = Recordset.CurrentPosition;
        }

        private void BindDataSet()
        {
			UpdateBindingSource();
            if (Recordset.Opened && Recordset.Tables.Count != 0)
            {
                source.CurrentChanged += new EventHandler(BindingContext_onCurrentChanged);
            }
        }

        private void UnBindDataSet()
        {
            
            if (source.Count >= 0)
            {
                source.CurrentChanged -= new EventHandler(BindingContext_onCurrentChanged);
				source.ListChanged -= new ListChangedEventHandler(BindingContext_onListChanged);
            }
        }

        /// <summary>
        /// Refreshes the control's underlying data.
        /// </summary>
        public override void Refresh()
        {
            
            base.Refresh();
            RefreshResultSet();
            BindDataSet();
        }


        /// <summary>
        /// Ends the initialization process.
        /// </summary>
        public virtual void EndInit()
        {
            if (IsConnectionAvailable())
            {
                UpdateConnectionInfo();
                RefreshResultSet();
                source = new BindingSource(Recordset, Recordset.Tables[0].TableName);
                source.CurrentChanged += new EventHandler(BindingContext_onCurrentChanged);
            }
        }

        #region ISupportInitialize Members

        /// <summary>
        /// Begins the initialization process.
        /// </summary>
        public virtual void BeginInit()
        {
            OnInitialization = true;
        }

        #endregion


		#region "Check if Required"
		/// <summary>
		/// Connect
		/// </summary>
		public string Connect
		{
			get { return null; }
			set { _RecordSource = value; }
		}

		private string _databaseName;

		/// <summary>
		/// DatabaseName
		/// </summary>
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		private int _defaultType;

		/// <summary>
		/// DefaultType
		/// </summary>
		public int DefaultType
		{
			get { return _defaultType; }
			set { _defaultType = value; }
		}

		private bool _exclusive;

		/// <summary>
		/// Exclusive
		/// </summary>
		public bool Exclusive
		{
			get { return _exclusive; }
			set { _exclusive = value; }
		}

		private int _options;

		/// <summary>
		/// Options
		/// </summary>
		public int Options
		{
			get { return _options; }
			set { _options = value; }
		}

		private bool _readOnly;

		/// <summary>
		/// ReadOnly
		/// </summary>
		public bool ReadOnly
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}

		private int _recordsetType;

		/// <summary>
		/// 
		/// </summary>
		public int RecordsetType
		{
			get { return _recordsetType; }
			set { _recordsetType = value; }
		}

		#endregion

    }

    internal class ConnectionStringEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            try
            {
                Assembly editorAssembly = Assembly.Load(new AssemblyName("Microsoft.Data.ConnectionUI.Dialog"));
                if (editorAssembly != null)
                {
                    Type dialogType = editorAssembly.GetType("Microsoft.Data.ConnectionUI.DataConnectionDialog", false);
                    if (dialogType != null)
                    {
                        object dcd = editorAssembly.CreateInstance("Microsoft.Data.ConnectionUI.DataConnectionDialog");
                        Type dataSourceType = editorAssembly.GetType("Microsoft.Data.ConnectionUI.DataSource", false);
                        if (dataSourceType != null)
                        {
                            dataSourceType.InvokeMember("AddStandardDataSources", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { dcd });

                        } // if
                        object res = dialogType.InvokeMember("Show", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, new object[] { dcd });
                        if (res != null)
                        {
                            Int32 val = (Int32)res;
                            if (val == (int)System.Windows.Forms.DialogResult.OK)
                            {
                                //GetConnectionString
                                PropertyInfo pinfo = dialogType.GetProperty("DisplayConnectionString");
                                if (pinfo != null)
                                {
                                    String connectionString = pinfo.GetValue(dcd, null) as String;
                                    return connectionString;
                                } // if
                            }
                        } // if
                    } // if
                    
                } // if
                //else
            } // try
            catch
            {
                
            } // catch
                return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}