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
using UpgradeHelpers.VB6.Utils;


namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Class DataHelper, inherits from UserControl, ISupportInitialize and IExtenderProvider.
    /// Used to Connecto to Access or by DSN to any DB.
    /// </summary>
    [ProvideProperty("DataHelperBinding", typeof(Control))]
    public partial class DataHelper : UserControl, System.ComponentModel.ISupportInitialize, IExtenderProvider
    {

        /// <summary>
        /// Enum to indicate how a control should be unbind from the datasource.
        /// </summary>
        private enum UnBindActionEnum
        {
            UnBindMember = 0,
            CleanMember = 1,
            CallFunction = 3,
            RemoveEventHandler = 4
        }

        /// <summary>
        /// Enum to indicate which component is being emulated.
        /// </summary>
        public enum EmulationTypeEnum
        {
            /// <summary>
            /// VB Data
            /// </summary>
            VBData = 0,
            /// <summary>
            /// MS RDC
            /// </summary>
            MSRDC = 1
        }

        /// <summary>
        /// Enum for the property CursorDriver [MSRDC].
        /// </summary>
        public enum CursorDriverEnum
        {
            /// <summary>
            /// Use if needed
            /// </summary>
            rdUseIfNeeded = 0,
            /// <summary>
            /// Use Odbc
            /// </summary>
            rdUseOdbc = 1,
            /// <summary>
            /// Use Server
            /// </summary>
            rdUseServer = 2,
            /// <summary>
            /// Client Batch
            /// </summary>
            rdUseClientBatch = 3
        }

        /// <summary>
        /// Enum for the property BOFAction.
        /// </summary>
        public enum BOFActionEnum
        {
            /// <summary>
            /// Move First
            /// </summary>
            rdMoveFirst = 0,
            /// <summary>
            /// Moves to Begin of File
            /// </summary>
            rdBOF = 1
        }

        /// <summary>
        /// Enum for the property EOFAction.
        /// </summary>
        public enum EOFActionEnum
        {
            /// <summary>
            /// Move to Last
            /// </summary>
            rdMoveLast = 0,
            /// <summary>
            /// Moves to End of File
            /// </summary>
            rdEOF = 1,
            /// <summary>
            /// Add New
            /// </summary>
            rdAddNew = 2
        }

        /// <summary>
        /// Enum for the property LockType.
        /// </summary>
        public enum LockTypeEnum
        {
            /// <summary>
            /// Read Only
            /// </summary>
            rdConcurReadOnly = 1,
            /// <summary>
            /// Lock
            /// </summary>
            rdConcurLock = 2,
            /// <summary>
            /// Row Lock
            /// </summary>
            rdConcurRowver = 3,
            /// <summary>
            /// Value Lock
            /// </summary>
            rdConcurValues = 4,
            /// <summary>
            /// Batch Lock
            /// </summary>
            rdConcurBatch = 5
        }

        /// <summary>
        /// Enum for the property Prompt.
        /// </summary>
        public enum PromptEnum
        {
            /// <summary>
            /// Driver
            /// </summary>
            rdDriverPrompt = 0,
            /// <summary>
            /// No Prompt
            /// </summary>
            rdDriverNoPrompt = 1,
            /// <summary>
            /// Complete
            /// </summary>
            rdDriverComplete = 2,
            /// <summary>
            /// Complete Required
            /// </summary>
            rdDriverCompleteRequired = 3
        }

        /// <summary>
        /// Enum for the Validate Events.
        /// </summary>
        public enum DataValidateEnum
        {
            /// <summary>
            /// Cancel
            /// </summary>
            vbDataActionCancel = 0,
            /// <summary>
            /// Move First
            /// </summary>
            vbDataActionMoveFirst = 1,
            /// <summary>
            /// Move Previous
            /// </summary>
            vbDataActionMovePrevious = 2,
            /// <summary>
            /// Move Next
            /// </summary>
            vbDataActionMoveNext = 3,
            /// <summary>
            /// Move Last
            /// </summary>
            vbDataActionMoveLast = 4,
            /// <summary>
            /// Add New
            /// </summary>
            vbDataActionAddNew = 5,
            /// <summary>
            /// Update
            /// </summary>
            vbDataActionUpdate = 6,
            /// <summary>
            /// Delete
            /// </summary>
            vbDataActionDelete = 7,
            /// <summary>
            /// Find
            /// </summary>
            vbDataActionFind = 8,
            /// <summary>
            /// BookMark
            /// </summary>
            vbDataActionBookmark = 9,
            /// <summary>
            /// Close
            /// </summary>
            vbDataActionClose = 10,
            /// <summary>
            /// Unload
            /// </summary>
            vbDataActionUnload = 11
        }

        /// <summary>
        /// Class to store information about how a member for a control should be unbind from the datasource.
        /// </summary>
        private class ControlUnbindingInformation
        {
            public string Member = string.Empty;
            public UnBindActionEnum UnBindAction = UnBindActionEnum.UnBindMember;

            /// <summary>
            /// Property UnbindFunctionParameters is a list that holds the unbind parameters.
            /// </summary>
            private List<object> _UnBindFunctionParameters = new List<object>();
            public object[] UnBindFunctionParameters
            {
                get
                {
                    object[] result = new object[_UnBindFunctionParameters.Count];
                    _UnBindFunctionParameters.CopyTo(result);
                    return result;
                }
            }

            /// <summary>
            /// Constructor that Sets the internal variables.
            /// </summary>
            /// <param name="Member">set the internal member</param>
            /// <param name="UnBindAction">set the internal UnbindAction value</param>
            public ControlUnbindingInformation(string Member, UnBindActionEnum UnBindAction)
            {
                this.Member = Member;
                this.UnBindAction = UnBindAction;
            }

            /// <summary>
            ///     Constructor that set the internal variables and adds a range of UnbindFunctionParameters
            /// </summary>
            /// <param name="Member">Sets the internal member value.</param>
            /// <param name="UnBindAction">Sets the internal UnbindAction value.</param>
            /// <param name="UnBindFunctionParameters">Adds a UnbindFunctionsParameters.</param>
            public ControlUnbindingInformation(string Member, UnBindActionEnum UnBindAction, object[] UnBindFunctionParameters)
            {
                this.Member = Member;
                this.UnBindAction = UnBindAction;
                this._UnBindFunctionParameters.AddRange(UnBindFunctionParameters);
            }
        }

        /// <summary>
        /// Class to store information for a control about how the binding was invoked by the user and 
        /// how the members should be unbind.
        /// </summary>
        private class ControlBindingInformation
        {
            /// <summary>
            /// This contains the list of the parameters originally sent by the user code to bind 
            /// the control, using these and the control type it is possible to figure out which 
            /// BindControl function was/should be used.
            /// </summary>
            private List<object> _BindingParameters = new List<object>();
            public List<object> BindingInvocationParameters
            {
                get
                {
                    return _BindingParameters;
                }
            }

            /// <summary>
            /// This contains the list of members that should be used to unbind this control and 
            /// how the unbinding should be done.
            /// </summary>
            private List<ControlUnbindingInformation> _UnBindingMemberInformation = new List<ControlUnbindingInformation>();
            public List<ControlUnbindingInformation> UnBindingMemberInformation
            {
                get
                {
                    return _UnBindingMemberInformation;
                }
            }
            /// <summary>
            /// Add Invocation Parameters to Binding Invocation Parameters List
            /// done
            /// </summary>
            /// <param name="invocationParameters">Invocation parameters object array.</param>
            public ControlBindingInformation(params object[] invocationParameters)
            {
                BindingInvocationParameters.AddRange(invocationParameters);
            }
        }


        /// <summary>
        /// Collection of controls bound so far.
        /// </summary>
        private Dictionary<object, ControlBindingInformation> _BoundControls = new Dictionary<object, ControlBindingInformation>();
        private Dictionary<object, ControlBindingInformation> BoundControls
        {
            get
            {
                return _BoundControls;
            }
        }

        /// <summary>
        /// List of controls pending to be bound when a recordset is available.
        /// </summary>
        private Dictionary<object, ControlBindingInformation> _ControlsPendingToBind = new Dictionary<object, ControlBindingInformation>();
        private Dictionary<object, ControlBindingInformation> ControlsPendingToBind
        {
            get
            {
                return _ControlsPendingToBind;
            }
        }

        /// <summary>
        /// On Reposition Event, using the Reposition EventHandler.
        /// </summary>
        public event EventHandler Reposition = null;
        /// <summary>
        /// On Reposition event
        /// </summary>
        protected virtual void OnReposition()
        {
            if (Reposition != null)
                Reposition(this, new EventArgs());
        }
        /// <summary>
        /// ValidatingEventHandler holds the event declaration.
        /// </summary>
        public new event ValidatingEventHandler Validating = null;
        /// <summary>
        /// On Validating Event, using the Validating EventHandler
        /// parameters int and by ref Action and Save.
        /// </summary>
        /// <param name="Action">Action value to be returned.</param>
        /// <param name="Save">Save value to be returned.</param>
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
        /// Delegate ValidatingEventHandler, used to process event ValidatingEventHandler.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">ValidatingEventArgs to process.</param>
        public delegate void ValidatingEventHandler(object sender, ValidatingEventArgs e);

        /// <summary>
        /// Class ValidatingEventArgs, used to process event ValidatingEventHandler parameters.
        /// </summary>
        public class ValidatingEventArgs : EventArgs
        {
            /// <summary>
            /// Constructor ValidatingEventArgs, recive Action and Save.
            /// </summary>
            public ValidatingEventArgs(int Action, int Save)
            {
                this._Action = Action;
                this.Save = Save;
            }

            private int _Action;
            /// <summary>
            /// Integer Property Action.
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
            /// Integer Property Save.
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
        /// Delegate AddNewRow EventHandler, used to process event ValidatingEventHandler.
        /// </summary>
        public delegate void AddNewRowEventHandler(object sender, AddNewRowEventArgs e);
        
        /// <summary>
        /// Class AddNewRow Event Args, used to process parameters for AddNewRowEventHandler.
        /// </summary>
        public class AddNewRowEventArgs : EventArgs
        {
            /// <summary>
            /// Constructor for AddNewRowEventArgs, do not set anything.
            /// </summary>
            public AddNewRowEventArgs()
            {
            }

            private DataRow _newRow = null;
            /// <summary>
            /// Boolean Property NewRow.
            /// </summary>
            public DataRow NewRow
            {
                get { return _newRow; }
                set { _newRow = value; }
            }
        }

        /// <summary>
        /// OnInitialization state variable.
        /// </summary>
        private bool OnInitialization = false;
        
        // Methods to implement inherited from ISupportInitialize.

        /// <summary>
        /// BeginInit, sets the OnInitialization state to true.
        /// </summary>
        public void BeginInit()
        {
            OnInitialization = true;
        }

        /// <summary>
        /// EndInit, sets the OnInitialization state to false. Refreshes the Connection Info.
        /// </summary>
        public void EndInit()
        {
            OnInitialization = false;
            UpdateConnectionInfo();
            RefreshResultSet();
        }

        /// <summary>
        /// Constructor, initialize controls and sets the style to Selectable in false.
        /// </summary>
        public DataHelper()
        {
            InitializeComponent();
            //base.Enabled = false;
            this.SetStyle(ControlStyles.Selectable, false);
        }

        /// <summary>
        /// Destructor, Unbinds used controls and clean the lists.  
        /// In the case there is a Recordset used will be disposed.
        /// </summary>
        ~DataHelper()
        {
            try
            {
                foreach (Control ctrl in BoundControls.Keys)
                    InternalUnbindControl(ctrl);

                BoundControls.Clear();

                if (Recordset != null)
                    Recordset.Dispose();
            }
            catch { }
        }

        #region DesignTime property
        private static WeakDictionary<Control, DataHelperBindingInfo> _DataHelperBinding = new WeakDictionary<Control, DataHelperBindingInfo>();

        /// <summary>
        /// Prevents multiple properties for the controls when 
        /// several instances of this type are present in the form.
        /// </summary>
        private static List<Object> ListOfExtendedControls = new List<Object>();
        private List<Object> MyListOfExtendedControls = new List<Object>();

        /// <summary>
        /// Returns true if the control will expose the property, only one instance of the property 
        /// will be added to a control no matter how many instances of this ExtenderProvider are 
        /// added to the form.
        /// </summary>
        /// <param name="ctrl">Control to check.</param>
        /// <returns></returns>
        public bool CanExtend(object ctrl)
        {
            bool res = (ctrl is ListBox) || (ctrl is ComboBox) || (ctrl is TextBox) || (ctrl is PictureBox) || ctrl.GetType().Name.Equals("C1TrueDBGrid", StringComparison.CurrentCultureIgnoreCase);
            if (res)
            {
                if (!ListOfExtendedControls.Contains(ctrl))
                {
                    ListOfExtendedControls.Add(ctrl);
                    MyListOfExtendedControls.Add(ctrl);
                }
                else if (ListOfExtendedControls.Contains(ctrl) && !MyListOfExtendedControls.Contains(ctrl))
                    res = false;
            }

            return res;
        }

        /// <summary>
        /// Gets method of the property to be added. A custom designer has been added.
        /// </summary>
        /// <param name="ctrl">Control to check.</param>
        /// <returns></returns>
        [Editor(typeof(DataHelperDesignerEditor), typeof(System.Drawing.Design.UITypeEditor)), Description("Allows to bind the control to a DataHelper"), Category("Data")]
        public DataHelperBindingInfo GetDataHelperBinding(Control ctrl)
        {
            if (_DataHelperBinding.ContainsKey(ctrl))
                return _DataHelperBinding[ctrl];
            else
                return getDefaultDataHelperBindingInfo(ctrl);
        }

        /// <summary>
        /// Sets method of the property to be added.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <param name="value">Helper to bind.</param>
        public void SetDataHelperBinding(Control ctrl, DataHelperBindingInfo value)
        {
            KeyValuePair<string, string>[] parameters = null;

            //Unbind the control if it is bound
            UnBindControl(ctrl);

            _DataHelperBinding[ctrl] = value;

            if ((value != null) && (value.BindingControl != null))
            {
                parameters = value.BindingParameters.ToArray();
                if (value.BindingParameters.Count == 0)
                    value.BindingControl.BindControl(ctrl);
                else if ((value.BindingParameters.Count == 2) && (ctrl is ListBox))
                    value.BindingControl.BindControl((ListBox)ctrl, parameters[0].Value, parameters[1].Value);
                else if ((value.BindingParameters.Count == 2) && (ctrl is ComboBox))
                    value.BindingControl.BindControl((ComboBox)ctrl, parameters[0].Value, parameters[1].Value);
                else if (value.BindingParameters.Count == 2)
                    value.BindingControl.BindControl(ctrl, parameters[0].Value, parameters[1].Value);
                else if (value.BindingParameters.Count == 1)
                    value.BindingControl.BindControl(ctrl, parameters[0].Value);
            }
        }

        /// <summary>
        /// Returns a default DataHelperBindingInfo class based on the control.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <returns></returns>
        internal static DataHelperBindingInfo getDefaultDataHelperBindingInfo(Control ctrl)
        {
            DataHelperBindingInfo res = null;

            if ((ctrl is ListBox) || (ctrl is ComboBox))
            {
                res = new DataHelperBindingInfo(null, new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("ValueMember", string.Empty), 
                    new KeyValuePair<string, string>("DisplayMember", string.Empty) 
                });
            }
            else if ((ctrl is TextBox) || (ctrl is PictureBox))
            {
                res = new DataHelperBindingInfo(null, new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Text", string.Empty)
                });
            }
            else
            {
                res = new DataHelperBindingInfo(null, new KeyValuePair<string, string>[] { });
            }
            return res;
        }

        #endregion

        /// <summary>
        /// Text Property, used to set the Caption.
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


        private EmulationTypeEnum _EmulationType = EmulationTypeEnum.VBData;
        /// <summary>
        /// Emulation Type Property.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MIGRATION PROPERTY]: To indicate which component we are emulating")]
        [Category("DataConnection Emulation Type")]
        public EmulationTypeEnum EmulationType
        {
            get
            {
                return _EmulationType;
            }
            set
            {
                _EmulationType = value;
            }
        }

        private string _Connect = "Access";
        /// <summary>
        /// Connect Property, used to set the Caption.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA, MSRDC PROPERTY]: Indicates the source of an open database, a database used in a pass-through query, or an attached table")]
        [Category("VB.DATA DataConnection")]
        public string Connect
        {
            get
            {
                return _Connect;
            }
            set
            {
                _Connect = value;
                UpdateConnectionInfo();
            }
        }
        private string _DatabaseName = string.Empty;
        /// <summary>
        /// DatabaseName: Returns/sets the name and location of the source of data for a Data control.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA PROPERTY]: Returns/sets the name and location of the source of data for a Data control")]
        [Category("VB.DATA DataConnection")]
        public string DatabaseName
        {
            get
            {
                return _DatabaseName;
            }
            set
            {
                _DatabaseName = value;
                UpdateConnectionInfo();
            }
        }

        private int _DefaultCursorType = 0;
        /// <summary>
        /// DefaultCursorType: Get/Set the Default Cursor Type.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA PROPERTY]: Not used")]
        public int DefaultCursorType
        {
            get
            {
                return _DefaultCursorType;
            }
            set
            {
                _DefaultCursorType = value;
            }
        }
        private int _DefaultType = 2;
        /// <summary>
        /// DefaultType Get/Set the Default/Set originally is 2.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA PROPERTY]: Not used")]
        public int DefaultType
        {
            get
            {
                return _DefaultType;
            }

            set
            {
                _DefaultType = value;
            }
        }

        private bool _Exclusive = false;
        /// <summary>
        /// Exclusive: Get/Set the Exclusive value, default is false.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA PROPERTY]: Not used")]
        public bool Exclusive
        {
            get
            {
                return _Exclusive;
            }
            set
            {
                _Exclusive = value;
            }
        }
        private int _Options = 0;
        /// <summary>
        /// Options: Get/Set Options value, default is 0.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA, MSRDC PROPERTY]: Not used")]
        public int Options
        {
            get
            {
                return _Options;
            }
            set
            {
                _Options = value;
            }
        }

        private bool _ReadOnly = false;

        /// <summary>
        /// ReadOnly: Get/Set ReadOnly value, default is false.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA PROPERTY]: Not used")]
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                _ReadOnly = value;
            }
        }

        private int _RecordsetType = 1;

        /// <summary>
        /// RecordsetType: Get/Set Recordset Type value, default is 1.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA, MSRDC PROPERTY]: Not used")]
        public int RecordsetType
        {
            get
            {
                return _RecordsetType;
            }
            set
            {
                _RecordsetType = value;
            }
        }

        private string _RecordSource = string.Empty;
        /// <summary>
        /// RecordSource: Returns/sets the underlying table, SQL Statement, or 
        /// QueryDef object for a Data control.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VB.DATA, MSRDC PROPERTY]: Returns/sets the underlying table, SQL Statement, or QueryDef object for a Data control")]
        [Category("VB.DATA DataConnection")]
        public string RecordSource
        {
            get
            {
                return _RecordSource;
            }
            set
            {
                _RecordSource = value;
                UpdateConnectionInfo();
            }
        }

        /// <summary>
        /// BackColor: Get/Set Back Color.
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

        private int _Appearance = 0;

        /// <summary>
        /// Appearance: Get/Set Appearance value, default is 0. If is set to 0, 
        /// BorderStyle is FixedSingle.
        /// </summary>
        [Browsable(true), Category("Appearance")]
        [Description("[VB.DATA, MSRDC PROPERTY]: Not used")]
        public int Appearance
        {
            get
            {
                return _Appearance;
            }
            set
            {
                _Appearance = value;
                if (_Appearance == 0)
                    this.BorderStyle = BorderStyle.FixedSingle;
                else
                    this.BorderStyle = BorderStyle.Fixed3D;
            }
        }
        /// <summary>
        /// Font: Get/Set Font value for caption.
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
        /// ForeColor: Get/Set ForeColor value for caption.
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

        private CursorDriverEnum _CursorDriver = CursorDriverEnum.rdUseIfNeeded;

        /// <summary>
        /// CursorDriver: Get/Set CursorDriver value, default is CursorDriverEnum.rdUseIfNeeded, 
        /// the property is integer but is mapped to the internal var to the specific enum.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int CursorDriver
        {
            get
            {
                return (int)_CursorDriver;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _CursorDriver = CursorDriverEnum.rdUseOdbc;
                        break;
                    case 2:
                        _CursorDriver = CursorDriverEnum.rdUseServer;
                        break;
                    case 3:
                        _CursorDriver = CursorDriverEnum.rdUseClientBatch;
                        break;
                    default:
                        _CursorDriver = CursorDriverEnum.rdUseIfNeeded;
                        break;
                }
            }
        }

        private BOFActionEnum _BOFAction = BOFActionEnum.rdMoveFirst;

        /// <summary>
        /// BOFAction: Get/Set BOFAction value, default is BOFActionEnum.rdMoveFirst. 
        /// When is set if value is 0, the internal var will be rdMoveFirst, otherwise is rdBOF.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VS.DATA, MSRDC PROPERTY]: Not used")]
        public int BOFAction
        {
            get
            {
                return (int)_BOFAction;
            }
            set
            {
                _BOFAction = (value == 0) ? BOFActionEnum.rdMoveFirst : BOFActionEnum.rdBOF;
            }
        }

        private EOFActionEnum _EOFAction = EOFActionEnum.rdMoveLast;

        /// <summary>
        /// EOFAction: Get/Set EOFAction, default is EOFActionEnum.rdMoveLast. 
        /// When is set the value is mapped to the specific enum value.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[VS.DATA, MSRDC PROPERTY]: Not used")]
        public int EOFAction
        {
            get
            {
                return (int)_EOFAction;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _EOFAction = EOFActionEnum.rdEOF;
                        break;
                    case 2:
                        _EOFAction = EOFActionEnum.rdAddNew;
                        break;
                    default:
                        _EOFAction = EOFActionEnum.rdMoveLast;
                        break;
                }
            }
        }

        private LockTypeEnum _LockType = LockTypeEnum.rdConcurRowver;
        /// <summary>
        /// LockType: Get/Set LockType value, default is LockTypeEnum.rdConcurRowver. 
        /// When is set the int value is mapped to the specific enum value in LockTypeEnum.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int LockType
        {
            get
            {
                return (int)_LockType;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _LockType = LockTypeEnum.rdConcurReadOnly;
                        break;
                    case 2:
                        _LockType = LockTypeEnum.rdConcurLock;
                        break;
                    case 4:
                        _LockType = LockTypeEnum.rdConcurValues;
                        break;
                    case 5:
                        _LockType = LockTypeEnum.rdConcurBatch;
                        break;
                    default:
                        _LockType = LockTypeEnum.rdConcurRowver;
                        break;
                }
            }
        }

        private int _QueryType = 0;
        /// <summary>
        /// QueryType: Get/Set QueryType value, default is 0. 
        /// No matter what value is sent, will be set to 0.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int QueryType
        {
            get
            {
                return _QueryType;
            }
            set
            {
                _QueryType = 0;
            }
        }

        private PromptEnum _Prompt = PromptEnum.rdDriverCompleteRequired;
        /// <summary>
        /// Prompt: Get/Set Prompt value, default is PromptEnum.rdDriverCompleteRequired. 
        /// When is set the int value is mapped to the PromptEnum value.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int Prompt
        {
            get
            {
                return (int)_Prompt;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        _Prompt = PromptEnum.rdDriverPrompt;
                        break;
                    case 1:
                        _Prompt = PromptEnum.rdDriverNoPrompt;
                        break;
                    case 2:
                        _Prompt = PromptEnum.rdDriverComplete;
                        break;
                    default:
                        _Prompt = PromptEnum.rdDriverCompleteRequired;
                        break;
                }
            }
        }

        private int _QueryTimeout = 30;
        /// <summary>
        /// QueryTimeOut: Get/Set Query time out value, default is 30.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
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

        private int _RowsetSize = 100;
        /// <summary>
        /// RowsetSize: Get/Set Rowset size, default is 100.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int RowsetSize
        {
            get
            {
                return _RowsetSize;
            }
            set
            {
                _RowsetSize = value;
            }
        }

        private int _LoginTimeout = 15;
        /// <summary>
        /// LoginTimeout: Get/Set Login time out value, default is 15.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int LoginTimeout
        {
            get
            {
                return _LoginTimeout;
            }
            set
            {
                _LoginTimeout = value;
            }
        }
        private int _KeysetSize = 0;
        /// <summary>
        /// KeysetSize: Get/Set Key set size value, default is 0.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int KeysetSize
        {
            get
            {
                return _KeysetSize;
            }
            set
            {
                _KeysetSize = value;
            }
        }

        private int _MaxRows = 0;
        /// <summary>
        /// MaxRows: Get/Set Max rows value, default is 0.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int MaxRows
        {
            get
            {
                return _MaxRows;
            }
            set
            {
                _MaxRows = value;
            }
        }

        private int _ErrorThreshold = -1;
        /// <summary>
        /// ErrorThreshold: Get/Set Error Threshold value, default is -1.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int ErrorThreshold
        {
            get
            {
                return _ErrorThreshold;
            }
            set
            {
                _ErrorThreshold = value;
            }
        }

        private int _BatchSize = 15;
        /// <summary>
        /// BatchSize: Get/Set Batch size value, default is 15.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Not used")]
        public int BatchSize
        {
            get
            {
                return _BatchSize;
            }
            set
            {
                _BatchSize = value;
            }
        }

        private string _DataSourceName = string.Empty;
        /// <summary>
        /// DataSourceName: Returns/sets RemoteData control's data source name. 
        /// Updates ConnectionInfo after is set.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Returns/sets RemoteData control's data source name")]
        [Category("MSRDC DataConnection")]
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

        private string _UserName = string.Empty;
        /// <summary>
        /// UserName: Get/Set User name id. Updates connection info when is set.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Specifies user ID")]
        [Category("MSRDC DataConnection")]
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
        /// Password: Get/Set Password value. Updates connection info when is set.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Password used during creation of rdoEnvironment object")]
        [Category("MSRDC DataConnection")]
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

        private string _LogMessages = string.Empty;
        /// <summary>
        /// LogMessages: Get/Set LogMessages string.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: LogMessages")]
        public string LogMessages
        {
            get
            {
                return _LogMessages;
            }
            set
            {
                _LogMessages = value;
            }
        }

        /// <summary>
        /// Caption: Determines RemoteData control's caption.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("[MSRDC PROPERTY]: Determines RemoteData control's caption")]
        public string Caption
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        private bool _Enabled = true;
        /// <summary>
        /// Enabled: Get/Set Enabled value, default is true. Used to control the enabling of the control.
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
                else if (Recordset != null)
                    base.Enabled = _Enabled;
            }
        }

        private RecordsetClass _Recordset = null;
        /// <summary>
        /// RecordSet: Used to set the RecordSet, default is null.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RecordsetClass Recordset
        {
            get
            {
                return _Recordset;
            }
            set
            {
                if (_Recordset != value)
                {
                    if (_Recordset != null)
                    {
                        TemporallyUnBindControls();
                        _Recordset.AfterQuery -= new EventHandler(Recordset_onAfterQuery);
                        _Recordset.AfterMove -= new EventHandler(Recordset_onAfterMove);
                        _Recordset.Validating -= new ValidatingEventHandler(Recordset_Validating);
                        _Recordset.AddNewRecord -= new AddNewRowEventHandler(Recordset_AddNewRecord);
                        _Recordset.CancelCurrentEdit -= new EventHandler(Recordset_CancelCurrentEdit);
                        _Recordset.EndCurrentEdit -= new EventHandler(Recordset_EndCurrentEdit);
                        UnBindDataSet();
                    }
                    _Recordset = value;
                    if (_Recordset != null)
                    {
                        CheckControlsPendingToBind();
                        _Recordset.AfterQuery += new EventHandler(Recordset_onAfterQuery);
                        _Recordset.AfterMove += new EventHandler(Recordset_onAfterMove);
                        _Recordset.Validating += new ValidatingEventHandler(Recordset_Validating);
                        _Recordset.AddNewRecord += new AddNewRowEventHandler(Recordset_AddNewRecord);
                        _Recordset.CancelCurrentEdit += new EventHandler(Recordset_CancelCurrentEdit);
                        _Recordset.EndCurrentEdit += new EventHandler(Recordset_EndCurrentEdit);
                        BindDataSet();
                        OnReposition();

                        if (Enabled)
                            base.Enabled = true;
                    }
                    else
                        base.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Bind all controls found in the collection ControlsPendingToBind.
        /// </summary>
        private void CheckControlsPendingToBind()
        {
            foreach (Control ctrl in ControlsPendingToBind.Keys)
            {
                if (ControlsPendingToBind[ctrl].BindingInvocationParameters.Count == 0)
                    BindControl(ctrl);
                else if ((ctrl is ListBox) && (ControlsPendingToBind[ctrl].BindingInvocationParameters.Count == 2))
                {
                    BindControl((ListBox)ctrl, ControlsPendingToBind[ctrl].BindingInvocationParameters[0].ToString(), ControlsPendingToBind[ctrl].BindingInvocationParameters[1].ToString());
                }
                else if ((ctrl is ComboBox) && (ControlsPendingToBind[ctrl].BindingInvocationParameters.Count == 2))
                {
                    BindControl((ComboBox)ctrl, ControlsPendingToBind[ctrl].BindingInvocationParameters[0].ToString(), ControlsPendingToBind[ctrl].BindingInvocationParameters[1].ToString());
                }
                else if (ControlsPendingToBind[ctrl].BindingInvocationParameters.Count == 1)
                {
                    BindControl(ctrl, ControlsPendingToBind[ctrl].BindingInvocationParameters[0].ToString());
                }
                else if (ControlsPendingToBind[ctrl].BindingInvocationParameters.Count == 2)
                {
                    BindControl(ctrl, ControlsPendingToBind[ctrl].BindingInvocationParameters[0].ToString(), ControlsPendingToBind[ctrl].BindingInvocationParameters[1].ToString());
                }
            }

            ControlsPendingToBind.Clear();
        }

        /// <summary>
        /// UnBind all controls and leave them in the list of ControlsPendingToBind.
        /// </summary>
        private void TemporallyUnBindControls()
        {
            foreach (Control ctrl in BoundControls.Keys)
            {
                ControlsPendingToBind.Add(ctrl, BoundControls[ctrl]);
                InternalUnbindControl(ctrl);
            }

            BoundControls.Clear();
        }
        /// <summary>
        /// Internal method to move first.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void b_first_Click(object sender, EventArgs e)
        {
            if (Recordset != null)
            {
                if (Recordset.RecordCount > 0)
                    Recordset.MoveFirst();

                ValidateButtonStatus();
            }
        }
        /// <summary>
        /// Internal method to move back.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void b_prev_Click(object sender, EventArgs e)
        {
            if (Recordset != null)
            {
                if (Recordset.RecordCount > 0)
                {
                    if (_BOFAction == BOFActionEnum.rdMoveFirst)
                    {
                        if (Recordset.RP > 0)
                            Recordset.MovePrevious();
                    }
                    else
                        Recordset.MovePrevious();
                }

                ValidateButtonStatus();
            }
        }
        /// <summary>
        /// Internal method to move next.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void b_next_Click(object sender, EventArgs e)
        {
            if (Recordset != null)
            {
                switch (_EOFAction)
                {
                    case EOFActionEnum.rdAddNew:
                        if ((Recordset.RecordCount > 0) && (Recordset.RP < (Recordset.RecordCount - 1)))
                            Recordset.MoveNext();
                        else
                        {
                            Recordset.AddNew();
                            Recordset.Update();
                        }
                        break;
                    case EOFActionEnum.rdMoveLast:
                        if (Recordset.RecordCount > 0)
                        {
                            if (Recordset.RP < (Recordset.RecordCount - 1))
                                Recordset.MoveNext();
                        }
                        break;
                    default:
                        if (Recordset.RecordCount > 0)
                        {
                            Recordset.MoveNext();
                        }
                        break;
                }

                ValidateButtonStatus();
            }
        }
        /// <summary>
        /// Internal method to move last.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void b_last_Click(object sender, EventArgs e)
        {
            if (Recordset != null)
            {
                if (Recordset.RecordCount > 0)
                    Recordset.MoveLast();

                ValidateButtonStatus();
            }
        }

        /// <summary>
        /// To decide if a button should be enabled/disabled.
        /// </summary>
        private void ValidateButtonStatus()
        {
            b_prev.Enabled = Recordset.RP_CanMovePrevious;
            b_next.Enabled = Recordset.RP_CanMoveNext;
        }


        /// <summary>
        /// Try to update the information to connect to the database based on the values of the properties:
        ///     - Connect
        ///     - DatabaseName
        ///     - RecordSource
        /// </summary>
        private void UpdateConnectionInfo()
        {
            if (DesignMode || OnInitialization)
                return;

            switch (EmulationType)
            {
                //In case we are emulating a VB.Data we have to handle a set of properties
                case EmulationTypeEnum.VBData:
                    switch (Connect.ToLower())
                    {
                        case "access":
                        case "access 2000;":
                            UpdateConnectionInfo_Access();
                            break;
                    }
                    break;
                //In case we are emulating a MSRDC we have to handle another set of properties
                case EmulationTypeEnum.MSRDC:
                    UpdateConnectionThroughDSN();
                    break;
            }
        }

        /// <summary>
        /// Try to update the information to connect to an Access database based 
        /// on the values of the properties:
        ///     - DatabaseName
        ///     - RecordSource
        /// </summary>
        private void UpdateConnectionInfo_Access()
        {
            string DBFile = string.Empty;

            if (!String.IsNullOrEmpty(DatabaseName) && !(String.IsNullOrEmpty(RecordSource)))
            {
                if (!Path.IsPathRooted(DatabaseName))
                    DBFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DatabaseName);
                else
                    DBFile = DatabaseName;

                if (!File.Exists(DBFile))
                    throw new Exception("Couldn't find file '" + DatabaseName + "'");

                ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + DBFile + "\"";
                if (RecordSource.Split(new char[] { ' ' }).Length > 1)
                    SqlSelectQuery = RecordSource;
                else
                    SqlSelectQuery = "SELECT * FROM " + RecordSource;
            }
        }

        /// <summary>
        /// Try to update the information to connect to a database using DSN as is done 
        /// by the MSRDC Control using the following properties:
        ///     - Datasourcename
        ///     - Username
        ///     - Password
        /// </summary>
        private void UpdateConnectionThroughDSN()
        {
            if (!String.IsNullOrEmpty(DataSourceName))
                ConnectionString = "Dsn=" + DataSourceName;

            if (!String.IsNullOrEmpty(ConnectionString) && !String.IsNullOrEmpty(UserName))
                ConnectionString += ";uid=" + UserName;

            if (!String.IsNullOrEmpty(ConnectionString) && !String.IsNullOrEmpty(Password))
                ConnectionString += ";pwd=" + Password;

        }

        /// <summary>
        /// It will try to recreate the resultset based on the values of the properties:
        /// - ConnnectionString
        /// - SqlSelectQuery
        /// </summary>
        private void RefreshResultSet()
        {
            if (DesignMode || OnInitialization)
                return;

            if (Recordset != null)
                Recordset.CloseRecordSet();

            if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(SqlSelectQuery))
                Recordset = null;
            else
            {
                switch (EmulationType)
                {
                    case EmulationTypeEnum.VBData:
                        Recordset = new OleDbRecordsetClass(ConnectionString, SqlSelectQuery);
                        break;
                    case EmulationTypeEnum.MSRDC:
                        Recordset = new ODBCRecordsetClass(ConnectionString, SqlSelectQuery);
                        break;
                }
            }
        }
        /// <summary>
        /// BindControl when only has a control parameter.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <returns></returns>
        public int BindControl(Control ctrl)
        {
            int i = -1;
            ControlBindingInformation ctrlBindingInfo = new ControlBindingInformation();

            try
            {
                //The control can't be bound right now, so it will wait until the recordset becomes available
                if (Recordset == null)
                {
                    ControlsPendingToBind.Add(ctrl, ctrlBindingInfo);
                    i = ControlsPendingToBind.Count;
                    return i;
                }

                if (ctrl.GetType().Name.Equals("C1TrueDBGrid", StringComparison.CurrentCultureIgnoreCase))
                {
                    i = BindC1TrueDBGrid(ctrl);
                }
                else
                    if (Utils.ReflectionHelper.ExistMember(ctrl, "DataSource"))
                    {
                        Utils.ReflectionHelper.SetMember(ctrl, "DataSource", Recordset.DataSet);
                        ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DataSource", UnBindActionEnum.CleanMember));

                        if (Utils.ReflectionHelper.ExistMember(ctrl, "DataMember"))
                        {
                            Utils.ReflectionHelper.SetMember(ctrl, "DataMember", "Table");
                            ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DataMember", UnBindActionEnum.CleanMember));
                        }

                        BoundControls.Add(ctrl, ctrlBindingInfo);
                        i = BoundControls.Count;
                    }
            }
            catch { }

            return i;
        }
        /// <summary>
        /// BindControl with Control and Column name, 
        /// then is called the BindControl with propertyName "Text"
        /// </summary>
        /// <param name="ctrl">Control to bind.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns></returns>
        public int BindControl(Control ctrl, String columnName)
        {
            return BindControl(ctrl, "Text", columnName);
        }

        /// <summary>
        /// BindControl when has the Control, the property name and the column name.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns></returns>
        public int BindControl(Control ctrl, string propertyName, string columnName)
        {
            int i = -1;
            ControlBindingInformation ctrlBindingInfo = new ControlBindingInformation(propertyName, columnName);

            try
            {
                //The control can't be bound right now, so it will wait until the recordset becomes available
                if (Recordset == null)
                {
                    ControlsPendingToBind.Add(ctrl, ctrlBindingInfo);
                    i = ControlsPendingToBind.Count;
                    return i;
                }

                ctrl.DataBindings.Add(propertyName, Recordset.DataSet, "Table." + columnName);
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation(propertyName, UnBindActionEnum.UnBindMember));

                BoundControls.Add(ctrl, ctrlBindingInfo);
                i = BoundControls.Count;
            }
            catch { }
            return i;
        }
        /// <summary>
        /// BindControl using a ListBox, columnValue and column Display.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <param name="colValue">String column value.</param>
        /// <param name="colDisplay">String column display.</param>
        /// <returns></returns>
        public int BindControl(ListBox ctrl, String colValue, String colDisplay)
        {
            int i = -1;
            ControlBindingInformation ctrlBindingInfo = new ControlBindingInformation(colValue, colDisplay);

            try
            {
                //The control can't be bound right now, so it will wait until the recordset becomes available
                if (Recordset == null)
                {
                    ControlsPendingToBind.Add(ctrl, ctrlBindingInfo);
                    i = ControlsPendingToBind.Count;
                    return i;
                }

                ctrl.DataSource = Recordset.DataSet;
                ctrl.ValueMember = "Table." + colValue;
                ctrl.DisplayMember = "Table." + colDisplay;

                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DataSource", UnBindActionEnum.CleanMember));
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("ValueMember", UnBindActionEnum.CleanMember));
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DisplayMember", UnBindActionEnum.CleanMember));
                BoundControls.Add(ctrl, ctrlBindingInfo);
                i = BoundControls.Count;
            }
            catch { }
            return i;
        }
        /// <summary>
        /// BindCntrol using a ComboBox, columnValue and ColumnDisplay.
        /// </summary>
        /// <param name="ctrl">Control to process.</param>
        /// <param name="colValue">String column value.</param>
        /// <param name="colDisplay">String column to display.</param>
        /// <returns></returns>
        public int BindControl(ComboBox ctrl, String colValue, String colDisplay)
        {
            int i = -1;
            ControlBindingInformation ctrlBindingInfo = new ControlBindingInformation(colValue, colDisplay);

            try
            {
                //The control can't be bound right now, so it will wait until the recordset becomes available
                if (Recordset == null)
                {
                    ControlsPendingToBind.Add(ctrl, ctrlBindingInfo);
                    i = ControlsPendingToBind.Count;
                    return i;
                }

                ctrl.DataSource = Recordset.DataSet;
                ctrl.ValueMember = "Table." + colValue;
                ctrl.DisplayMember = "Table." + colDisplay;

                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DataSource", UnBindActionEnum.CleanMember));
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("ValueMember", UnBindActionEnum.CleanMember));
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("DisplayMember", UnBindActionEnum.CleanMember));
                BoundControls.Add(ctrl, ctrlBindingInfo);
                i = BoundControls.Count;
            }
            catch { }
            return i;
        }

        /// <summary>
        /// UnbindControl, removes the Control.
        /// </summary>
        /// <param name="Ctrl">Control to process.</param>
        public void UnBindControl(Control Ctrl)
        {
            if (BoundControls.ContainsKey(Ctrl))
            {
                InternalUnbindControl(Ctrl);
                BoundControls.Remove(Ctrl);
            }
            else if (ControlsPendingToBind.ContainsKey(Ctrl))
                ControlsPendingToBind.Remove(Ctrl);
        }
        /// <summary>
        /// Internal UnbindControl to remove the control.
        /// </summary>
        /// <param name="Ctrl">Control to process.</param>
        private void InternalUnbindControl(Control Ctrl)
        {
            EventInfo eInfo = null;
            EventHandler eHandler = null;

            if (BoundControls.ContainsKey(Ctrl))
            {
                try
                {
                    foreach (ControlUnbindingInformation unbindInfo in BoundControls[Ctrl].UnBindingMemberInformation)
                    {
                        switch (unbindInfo.UnBindAction)
                        {
                            case UnBindActionEnum.CleanMember:
                                Utils.ReflectionHelper.SetMember(Ctrl, unbindInfo.Member, null);
                                break;
                            case UnBindActionEnum.UnBindMember:
                                Ctrl.DataBindings.Remove(Ctrl.DataBindings[unbindInfo.Member]);
                                break;
                            case UnBindActionEnum.CallFunction:
                                Utils.ReflectionHelper.Invoke(Ctrl, unbindInfo.Member, unbindInfo.UnBindFunctionParameters);
                                break;
                            case UnBindActionEnum.RemoveEventHandler:
                                eHandler = (EventHandler)unbindInfo.UnBindFunctionParameters[0];
                                eInfo = Ctrl.GetType().GetEvent(unbindInfo.Member);
                                if (eInfo != null)
                                {
                                    eInfo.RemoveEventHandler(Ctrl, eHandler);
                                }
                                break;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Function created to bind a C1TrueDBGrid to this DataHelper.
        /// </summary>
        /// <param name="ctrl">The C1TrueDBGrid control.</param>
        /// <returns>The index in the list of controls bound so far.</returns>
        private int BindC1TrueDBGrid(Control ctrl)
        {
            ControlBindingInformation ctrlBindingInfo = new ControlBindingInformation();
            EventInfo eInfo = null;
            EventHandler eHandler = null;

            //Calls the function SetDataBinding to bind the control to this DataHelper
            Utils.ReflectionHelper.Invoke(ctrl, "SetDataBinding", new object[] { Recordset.DataSet, "Table", false });
            ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("SetDataBinding", UnBindActionEnum.CallFunction));

            eHandler = new EventHandler(C1TrueDBGrid_Updated);

            //Bind an eventHandler to the event AfterUpdate to force the updating inline
            eInfo = ctrl.GetType().GetEvent("AfterUpdate");
            if (eInfo != null)
            {
                eInfo.AddEventHandler(ctrl, eHandler);
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("AfterUpdate", UnBindActionEnum.RemoveEventHandler, new object[] { eHandler }));
            }

            //Bind an eventHandler to the event AfterDelete to force the updating inline
            eInfo = ctrl.GetType().GetEvent("AfterDelete");
            if (eInfo != null)
            {
                eInfo.AddEventHandler(ctrl, eHandler);
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("AfterDelete", UnBindActionEnum.RemoveEventHandler, new object[] { eHandler }));
            }

            //Bind an eventHandler to the event AfterInsert to force the updating inline
            eInfo = ctrl.GetType().GetEvent("AfterInsert");
            if (eInfo != null)
            {
                eInfo.AddEventHandler(ctrl, eHandler);
                ctrlBindingInfo.UnBindingMemberInformation.Add(new ControlUnbindingInformation("AfterInsert", UnBindActionEnum.RemoveEventHandler, new object[] { eHandler }));
            }

            BoundControls.Add(ctrl, ctrlBindingInfo);
            return BoundControls.Count;
        }

        /// <summary>
        /// Event handler to force updates for the C1TrueDBGrid.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void C1TrueDBGrid_Updated(object sender, EventArgs e)
        {
            Recordset.Update();
        }
        /// <summary>
        /// Event for Recordset On After Move.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void Recordset_onAfterMove(object sender, EventArgs e)
        {
            ReBind();
            OnReposition();
        }
        /// <summary>
        /// Event for Recordset On After Query.
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">EventArgs.</param>
        private void Recordset_onAfterQuery(object sender, EventArgs e)
        {
            BindDataSet();
        }

        /// <summary>
        /// To flag that a new row has been added.
        /// </summary>
        private bool newRow = false;
        /// <summary>
        /// A signal sent by the recordset that a new row should be added to the BindingContext.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">AddNewRowEventArgs.</param>
        private void Recordset_AddNewRecord(object sender, AddNewRowEventArgs e)
        {
            IgnoreChangeInPosition = true;
            if (BindingContext[Recordset.DataSet, "Table"].IsBindingSuspended)
                BindingContext[Recordset.DataSet, "Table"].ResumeBinding();

            if (newRow)
                BindingContext[Recordset.DataSet, "Table"].CancelCurrentEdit();

            BindingContext[Recordset.DataSet, "Table"].AddNew();
            e.NewRow = ((DataRowView)BindingContext[Recordset.DataSet, "Table"].Current).Row;
            newRow = true;

            IgnoreChangeInPosition = false;
        }

        /// <summary>
        /// A signal sent by the recordset that the new record addes should be deleted from 
        /// the BindingContext.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void Recordset_CancelCurrentEdit(object sender, EventArgs e)
        {
            if (newRow)
            {
                IgnoreChangeInPosition = true;

                newRow = false;
                BindingContext[Recordset.DataSet, "Table"].CancelCurrentEdit();

                IgnoreChangeInPosition = false;
            }

        }

        /// <summary>
        /// A signal sent by the recordset that the changes of the new row should be applied.
        /// </summary>
        /// <param name="sender">Object sent.</param>
        /// <param name="e">EventArgs sent.</param>
        private void Recordset_EndCurrentEdit(object sender, EventArgs e)
        {
            if (newRow)
            {
                newRow = false;
                BindingContext[Recordset.DataSet, "Table"].EndCurrentEdit();
            }
        }

        /// <summary>
        /// BindingContext On Current Changed.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">EventArgs.</param>
        private void BindingContext_onCurrentChanged(object sender, EventArgs e)
        {
            //AIS-Bug 6780 FSABORIO
            if (!IgnoreChangeInPosition && (BindingContext[Recordset.DataSet, "Table"].Position != Recordset.AbsolutePosition))
            {
                if (BindingContext[Recordset.DataSet, "Table"].Position >= 0)
                    Recordset.AbsolutePosition = BindingContext[Recordset.DataSet, "Table"].Position;
            }
        }
        /// <summary>
        /// Recordset Validating event.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">DataHelper.ValidatingEventArgs.</param>
        private void Recordset_Validating(object sender, DataHelper.ValidatingEventArgs e)
        {
            int Action = e.Action;
            int Save = e.Save;
            OnValidating(ref Action, ref Save);
            e.Save = Save;
            e.Action = Action;
        }

        private string _ConnectionString = string.Empty;
        /// <summary>
        /// Properties to change the way the recorset is connected to a datasource and the query
        /// that is used to retrieve the data.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Returns/Sets the .NET specific query used to open the connection")]
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        private string _SqlSelectQuery = string.Empty;
        /// <summary>
        /// SqlSelectQuery: Get/Set Sql Select Query value. default is empty.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Returns/Sets the specific query used to return values from the connection")]
        [Category("MSRDC DataConnection")]
        public string SqlSelectQuery
        {
            get
            {
                return _SqlSelectQuery;
            }
            set
            {
                _SqlSelectQuery = value;
            }
        }

        // Methods provided to allow the user to specify the queries to use to update information.


        /// <summary>
        /// Method to set the insert query to use, this allows the user to override 
        /// the query that is generated by default
        /// </summary>
        /// <param name="InsertQuery">Query to use to insert values. It can include parameters.</param>
        /// <param name="parameters">Information of the parameters to set when the Command is created.</param>
        public void SetInsertQuery(String InsertQuery, List<DBParameterInfo> parameters)
        {
            if (Recordset != null)
                Recordset.SetInsertQuery(InsertQuery, parameters);
        }
        /// <summary>
        /// Method to set the update query to use, this allows the user to 
        /// override the query that is generated by default.
        /// </summary>
        /// <param name="UpdateQuery">Query to use to update values. It can include parameters.</param>
        /// <param name="parameters">Information of the parameters to set when the Command is created.</param>
        public void SetUpdateQuery(String UpdateQuery, List<DBParameterInfo> parameters)
        {
            if (Recordset != null)
                Recordset.SetUpdateQuery(UpdateQuery, parameters);
        }
        /// <summary>
        /// Method to set the delete query to use, this allows the user to 
        /// override the query that is generated by default.
        /// </summary>
        /// <param name="DeleteQuery">Query to use to delete values. It can include parameters.</param>
        /// <param name="parameters">Information of the parameters to set when the Command is created.</param>
        public void SetDeleteQuery(String DeleteQuery, List<DBParameterInfo> parameters)
        {
            if (Recordset != null)
                Recordset.SetDeleteQuery(DeleteQuery, parameters);
        }
        //AIS-Bug 6780 FSABORIO
        /// <summary>
        /// ReBind function to set position for RecordSet.DataSet.
        /// </summary>
        private bool IgnoreChangeInPosition = false;
        private void ReBind()
        {
            IgnoreChangeInPosition = true;

            //AIS-Bug 6780 FSABORIO
            int PosToSet = Recordset.AbsolutePosition;
            if (BindingContext[Recordset.DataSet, "Table"].Position != PosToSet)
            {
                if (PosToSet == -1)
                {
                    //AIS-Bug 6780 FSABORIO
                    if (!BindingContext[Recordset.DataSet, "Table"].IsBindingSuspended)
                        BindingContext[Recordset.DataSet, "Table"].SuspendBinding();
                }
                else
                {
                    //AIS-Bug 6780 FSABORIO
                    if (BindingContext[Recordset.DataSet, "Table"].IsBindingSuspended)
                        BindingContext[Recordset.DataSet, "Table"].ResumeBinding();

                    BindingContext[Recordset.DataSet, "Table"].Position = PosToSet;
                }
            }

            IgnoreChangeInPosition = false;
        }
        /// <summary>
        /// BindDataSet unbinds DataSet and sets the CurrentChanged Event Handler.
        /// </summary>
        private void BindDataSet()
        {
            UnBindDataSet();
            BindingContext[Recordset.DataSet, "Table"].CurrentChanged += new EventHandler(BindingContext_onCurrentChanged);
        }
        /// <summary>
        /// UnBindDataSet removes the CurrentChanged Event Handler.
        /// </summary>
        private void UnBindDataSet()
        {
            BindingContext[Recordset.DataSet, "Table"].CurrentChanged -= new EventHandler(BindingContext_onCurrentChanged);
        }
        /// <summary>
        /// Refreshes the ResultSet.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            RefreshResultSet();
        }

        /// <summary>
        /// Type to pass parameter information for the update queries.
        /// This is provided just in case that the CreateAdapter 
        /// method can't create update queries by default, so 
        /// methods will be provided for the user to specify them directly.
        /// </summary>
        public class DBParameterInfo : System.Data.IDataParameter
        {
            private DbType _DbType = DbType.String;
            private OdbcType OdbcType = OdbcType.VarChar;
            /// <summary>
            /// DbType: Get/Set the DBType also sets the OdbcType depending on value to set.
            /// </summary>
            public DbType DbType
            {
                get { return _DbType; }
                set
                {
                    _DbType = value;
                    switch (DbType)
                    {
                        case DbType.AnsiString:
                            OdbcType = OdbcType.VarChar;
                            break;
                        case DbType.AnsiStringFixedLength:
                            OdbcType = OdbcType.VarChar;
                            break;
                        case DbType.Binary:
                            OdbcType = OdbcType.Binary;
                            break;
                        case DbType.Boolean:
                            OdbcType = OdbcType.Bit;
                            break;
                        case DbType.Byte:
                            OdbcType = OdbcType.TinyInt;
                            break;
                        case DbType.Currency:
                            OdbcType = OdbcType.BigInt;
                            break;
                        case DbType.Date:
                            OdbcType = OdbcType.Date;
                            break;
                        case DbType.DateTime:
                            OdbcType = OdbcType.DateTime;
                            break;
                        case DbType.Decimal:
                            OdbcType = OdbcType.Decimal;
                            break;
                        case DbType.Double:
                            OdbcType = OdbcType.Double;
                            break;
                        case DbType.Guid:
                            OdbcType = OdbcType.UniqueIdentifier;
                            break;
                        case DbType.Int16:
                            OdbcType = OdbcType.SmallInt;
                            break;
                        case DbType.Int32:
                            OdbcType = OdbcType.Int;
                            break;
                        case DbType.Int64:
                            OdbcType = OdbcType.BigInt;
                            break;
                        case DbType.Object:
                            OdbcType = OdbcType.VarBinary;
                            break;
                        case DbType.SByte:
                            OdbcType = OdbcType.TinyInt;
                            break;
                        case DbType.Single:
                            OdbcType = OdbcType.Double;
                            break;
                        case DbType.String:
                            OdbcType = OdbcType.NVarChar;
                            break;
                        case DbType.StringFixedLength:
                            OdbcType = OdbcType.NVarChar;
                            break;
                        case DbType.Time:
                            OdbcType = OdbcType.Time;
                            break;
                        case DbType.UInt16:
                            OdbcType = OdbcType.SmallInt;
                            break;
                        case DbType.UInt32:
                            OdbcType = OdbcType.Int;
                            break;
                        case DbType.UInt64:
                            OdbcType = OdbcType.BigInt;
                            break;
                        case DbType.VarNumeric:
                            OdbcType = OdbcType.Numeric;
                            break;
                        case DbType.Xml:
                            OdbcType = OdbcType.NVarChar;
                            break;
                    }
                }
            }
            private ParameterDirection _Direction = ParameterDirection.Input;
            /// <summary>
            /// ParameterDirection: Get/Set ParameterDirection value, 
            /// default value is ParameterDirection.Input.
            /// </summary>
            public ParameterDirection Direction
            {
                get { return _Direction; }
                set { _Direction = value; }
            }

            private bool _IsNullable = true;
            /// <summary>
            /// IsNullable: Get/Set IsNullable value, default is true.
            /// </summary>
            public bool IsNullable
            {
                get { return _IsNullable; }
                set { _IsNullable = value; }
            }

            private string _ParameterName = string.Empty;
            /// <summary>
            /// ParameterName: Get/Set Parameter name value, default is empty.
            /// </summary>
            public string ParameterName
            {
                get { return _ParameterName; }
                set { _ParameterName = value; }
            }

            private string _SourceColumn = string.Empty;
            /// <summary>
            /// SourceColumn: Get/Set Source column value, default is empty.
            /// </summary>
            public string SourceColumn
            {
                get { return _SourceColumn; }
                set { _SourceColumn = value; }
            }

            private DataRowVersion _SourceVersion = DataRowVersion.Default;
            /// <summary>
            /// SourceVersion: Get/Set Source version value, default is DataRowVersion.Default.
            /// </summary>
            public DataRowVersion SourceVersion
            {
                get { return _SourceVersion; }
                set { _SourceVersion = value; }
            }

            private Object _Value = null;
            /// <summary>
            /// Value: Get/Set Value object, default is null.
            /// </summary>
            public Object Value
            {
                get { return _Value; }
                set { _Value = value; }
            }

            private bool _SourceColumnNullMapping = false;
            /// <summary>
            /// SourceColumnNullMapping: Get/Set Source column null mapping value, default is false.
            /// </summary>
            public bool SourceColumnNullMapping
            {
                get { return _SourceColumnNullMapping; }
                set { _SourceColumnNullMapping = value; }
            }
            /// <summary>
            /// Constructor to set the internal parameters.
            /// </summary>
            /// <param name="ParameterName">string</param>
            /// <param name="dbType">DbType</param>
            /// <param name="SourceColumn">string</param>
            public DBParameterInfo(string ParameterName, DbType dbType, string SourceColumn)
            {
                this.ParameterName = ParameterName;
                this.DbType = dbType;
                this.SourceColumn = SourceColumn;
            }
            /// <summary>
            /// Constructor to set internal parameters, int size is not used.
            /// </summary>
            /// <param name="ParameterName">String.</param>
            /// <param name="dbType">DbType.</param>
            /// <param name="size">Int not used.</param>
            /// <param name="SourceColumn">String.</param>
            public DBParameterInfo(string ParameterName, DbType dbType, int size, string SourceColumn)
            {
                this.ParameterName = ParameterName;
                this.DbType = dbType;
                this.SourceColumn = SourceColumn;
            }

            /// <summary>
            /// Returns this instance as a OleDbParameter.
            /// </summary>
            /// <returns></returns>
            protected internal OleDbParameter getOleDbParameter()
            {
                OleDbParameter res = new OleDbParameter();
                res.DbType = DbType;
                res.Direction = Direction;
                res.IsNullable = IsNullable;
                res.ParameterName = ParameterName;
                res.SourceColumn = SourceColumn;
                res.SourceVersion = SourceVersion;
                res.Value = Value;
                res.SourceColumnNullMapping = SourceColumnNullMapping;
                return res;
            }

            /// <summary>
            /// Returns this instance as a OleDbParameter.
            /// </summary>
            /// <returns></returns>
            protected internal OdbcParameter getOdbcParameter()
            {
                OdbcParameter res = new OdbcParameter();
                res.DbType = DbType;
                res.Direction = Direction;
                res.IsNullable = IsNullable;
                res.ParameterName = ParameterName;
                res.SourceColumn = SourceColumn;
                res.SourceVersion = SourceVersion;
                res.SourceColumnNullMapping = SourceColumnNullMapping;
                res.Value = Value;
                return res;
            }
        }

        /// <summary>
        /// Definition of the generic interface used as recordset.
        /// </summary>
        public interface RecordsetClass
        {
            /// <summary>
            /// After query event
            /// </summary>
            event EventHandler AfterQuery;
            /// <summary>
            /// After move event
            /// </summary>
            event EventHandler AfterMove;
            /// <summary>
            /// Validating event
            /// </summary>
            event ValidatingEventHandler Validating;
            /// <summary>
            /// Add new row event
            /// </summary>
            event AddNewRowEventHandler AddNewRecord;
            /// <summary>
            /// Cancel current edit event
            /// </summary>
            event EventHandler CancelCurrentEdit;
            /// <summary>
            /// End current edit event
            /// </summary>
            event EventHandler EndCurrentEdit;

            /// <summary>
            /// Gets the Record Position
            /// </summary>
            int RP { get;}
            /// <summary>
            /// Can move next?
            /// </summary>
            bool RP_CanMoveNext { get;}
            /// <summary>
            /// Can move previous?
            /// </summary>
            bool RP_CanMovePrevious { get;}
            /// <summary>
            /// Moves to First Record
            /// </summary>
            void MoveFirst();
            /// <summary>
            /// Moves to Last Record
            /// </summary>
            /// <param name="Options">specify option in the move</param>
            void MoveLast(int Options);

            /// <summary>
            /// Moves to Last Record with options
            /// </summary>
            /// <param name="Options">specifying an object of options</param>
            void MoveLast(object Options);
            /// <summary>
            /// Moves to Last Record
            /// </summary>
            void MoveLast();
            /// <summary>
            /// Moves Next
            /// </summary>
            void MoveNext();
            /// <summary>
            /// Moves previous
            /// </summary>
            void MovePrevious();

            /// <summary>
            /// Add new record
            /// </summary>
            void AddNew();
            /// <summary>
            /// Delete record
            /// </summary>
            void Delete();
            /// <summary>
            /// Edit record
            /// </summary>
            void Edit();
            /// <summary>
            /// Updates record
            /// </summary>
            void Update();
            /// <summary>
            /// Updates record with type and persistent
            /// </summary>
            /// <param name="UpdateType">kind of update</param>
            /// <param name="Force">is forced or not</param>
            void Update(int UpdateType, bool Force);


            /// <summary>
            /// Disposes internal objects
            /// </summary>
            void Dispose();
            /// <summary>
            /// Close internal recordset
            /// </summary>
            void CloseRecordSet();
            /// <summary>
            /// Sets the Connection String and connects
            /// </summary>
            /// <param name="constr"></param>
            /// <returns></returns>
            bool Connection(String constr);
            /// <summary>
            /// Connects using the internal connection string
            /// </summary>
            /// <returns></returns>
            bool Connection();

            /// <summary>
            /// Clones the internal Recordset class
            /// </summary>
            /// <returns></returns>
            RecordsetClass Clone();
            /// <summary>
            /// Finds first record using the column name and object criteria
            /// </summary>
            /// <param name="columnName">column name</param>
            /// <param name="criteria">object criteria</param>
            void FindFirst(String columnName, Object criteria);
            /// <summary>
            /// Finds Last record using column name and object criteria
            /// </summary>
            /// <param name="columnName">column name</param>
            /// <param name="criteria">object criteria</param>
            void FindLast(String columnName, Object criteria);
            /// <summary>
            /// Finds Previous, uses the actual position and get the previous record matching the criteria
            /// </summary>
            /// <param name="columnName">Column name</param>
            /// <param name="criteria">object criteria</param>
            void FindPrevious(String columnName, Object criteria);
            /// <summary>
            /// Finds next, uses the actual position and get the next record matching the criteria
            /// </summary>
            /// <param name="columnName">Column name</param>
            /// <param name="criteria">object criteria</param>
            void FindNext(String columnName, Object criteria);
            /// <summary>
            /// Refreshes the internal data record
            /// </summary>
            void Refresh();
            /// <summary>
            /// Query the database and refill the internal data set
            /// </summary>
            /// <returns></returns>
            bool Requery();
            /// <summary>
            /// Returns the Record Source query
            /// </summary>
            string RecordSource { get;}

            /// <summary>
            /// Cancel operations
            /// </summary>
            void Cancel();
            /// <summary>
            /// Close the internal recordset
            /// </summary>
            void Close();

            /// <summary>
            /// Return no match state
            /// </summary>
            bool NoMatch { get;}
            /// <summary>
            /// Get/Set the position in the data set
            /// </summary>
            int AbsolutePosition { get;set;}
            /// <summary>
            /// Return the Begin of File state
            /// </summary>
            bool BOF { get;}
            /// <summary>
            /// Return the End of File state
            /// </summary>
            bool EOF { get;}
            /// <summary>
            /// Access the dataset fields as an array using field name
            /// </summary>
            /// <param name="columnName">Field name</param>
            /// <returns>Object with field information</returns>
            Object this[String columnName] { get;set;}
            /// <summary>
            /// Access the dataset fields as an array using an index
            /// </summary>
            /// <param name="columnIndex">column index</param>
            /// <returns>Object with field information</returns>
            Object this[int columnIndex] { get;set;}
            /// <summary>
            /// Gets the number of records found using the actual query string
            /// </summary>
            long RecordsFound { get;}
            /// <summary>
            /// Gets the number of records in the dataset
            /// </summary>
            long RecordCount { get;}
            /// <summary>
            /// Gets Loading Finished state
            /// </summary>
            bool IsLoadingFinnished { get;}
            /// <summary>
            /// Returns the internal Fields Class
            /// </summary>
            FieldsClass Fields { get;}
            /// <summary>
            /// Returns the internal Query String
            /// </summary>
            String SqlQuery { get;set;}
            /// <summary>
            /// Gets the Bookmark data row 
            /// </summary>
            DataRow Bookmark { get;set;}
            /// <summary>
            /// Gets the Data Set
            /// </summary>
            DataSet DataSet { get;}

            /// <summary>
            /// Gets the Name of the Data Set
            /// </summary>
            string Name { get;}

            /// <summary>
            /// Sets the internal Insert query specifying the parameters
            /// </summary>
            /// <param name="InsertQuery">query string</param>
            /// <param name="parameters">list of DBParameterInfo parameters</param>
            void SetInsertQuery(String InsertQuery, List<DBParameterInfo> parameters);
            /// <summary>
            /// Sets the internal Update query specifying the parameters
            /// </summary>
            /// <param name="UpdateQuery">query string</param>
            /// <param name="parameters">list of DBParameterInfo parameters</param>
            void SetUpdateQuery(String UpdateQuery, List<DBParameterInfo> parameters);
            /// <summary>
            /// Sets the internal Delete query specifying the parameters
            /// </summary>
            /// <param name="DeleteQuery">query string</param>
            /// <param name="parameters">list of DBParameterInfo parameters</param>
            void SetDeleteQuery(String DeleteQuery, List<DBParameterInfo> parameters);
        }

        /// <summary>
        /// Implementation of RecordsetClass using OleDb.
        /// </summary>
        private class OleDbRecordsetClass : RecordsetClass
        {
            private OleDbConnection dbConnection = null;
            private OleDbDataAdapter dbAdapter = null;
            private OleDbCommandBuilder cmdBuilder = null;
            private DataSet dbDataSet = null;
            private DataTable dbTable = null;
            private DataRow dbRow = null;
            private int _AbsolutePosition = -1;
            private int _RecordPosition = -1;
            private bool newRow = false;
            private bool newRowChanged = false;
            private bool found = false;
            private String sqlSelectQuery = string.Empty;
            private String TableName = string.Empty;
            private OleDbCommand updateCommand = null;
            private OleDbCommand deleteCommand = null;
            private OleDbCommand insertCommand = null;
            private long foundRecordsCounter = 0;
            private bool operationFinished = false;
            private FieldsClass classFields = null;

            public event EventHandler AfterQuery = null;
            public event EventHandler AfterMove = null;
            public event ValidatingEventHandler Validating = null;
            public event AddNewRowEventHandler AddNewRecord = null;
            public event EventHandler CancelCurrentEdit = null;
            public event EventHandler EndCurrentEdit = null;

            /// <summary>
            /// RP: Record Position.
            /// </summary>
            public int RP
            {
                get { return _RecordPosition; }
            }
            /// <summary>
            /// RP_CanMoveFirst: Record can move first.
            /// </summary>
            private bool RP_CanMoveFirst
            {
                get
                {
                    return RecordCount > 0;
                }
            }
            /// <summary>
            /// RP_CanMoveLast: Record can move last.
            /// </summary>
            private bool RP_CanMoveLast
            {
                get
                {
                    return RP_CanMoveFirst;
                }
            }
            /// <summary>
            /// RP_CanMoveNext: Record can move next.
            /// </summary>
            public bool RP_CanMoveNext
            {
                get
                {
                    return (RecordCount > 0) && (_RecordPosition < RecordCount);
                }
            }
            /// <summary>
            /// RP_CanMovePrevious: Record can move previous.
            /// </summary>
            public bool RP_CanMovePrevious
            {
                get
                {
                    return _RecordPosition >= 0;
                }
            }
            /// <summary>
            /// RP_CanMoveAtPosition: Can move to specific position.
            /// </summary>
            /// <param name="position">Int position</param>
            /// <returns></returns>
            private bool RP_CanMoveAtPosition(int position)
            {
                return (position >= 0) && (position < RecordCount);
            }
            /// <summary>
            /// RP_MoveFirst: move to first position.
            /// </summary>
            private void RP_MoveFirst()
            {
                if (RP_CanMoveFirst)
                {
                    _RecordPosition = 0;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveLast: if can move to last, move it.
            /// </summary>
            private void RP_MoveLast()
            {
                if (RP_CanMoveLast)
                {
                    _RecordPosition = (int)RecordCount - 1;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveNext: if can move next, move it.
            /// </summary>
            private void RP_MoveNext()
            {
                if (RP_CanMoveNext)
                {
                    _RecordPosition++;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MovePrevious: if can move previous, move it.
            /// </summary>
            private void RP_MovePrevious()
            {
                if (RP_CanMovePrevious)
                {
                    _RecordPosition--;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveAtPosition: if can move to a specific position then move it.
            /// </summary>
            /// <param name="position"></param>
            private void RP_MoveAtPosition(int position)
            {
                if (RP_CanMoveAtPosition(position))
                {
                    _RecordPosition = position;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_Reset: sets the current position to -1.
            /// </summary>
            private void RP_Reset()
            {
                _RecordPosition = -1;
            }
            /// <summary>
            /// OnAfterQuery event calls the AfterQuery delegate asigned if not is null.
            /// </summary>
            protected virtual void OnAfterQuery()
            {
                if (AfterQuery != null) AfterQuery(this, new EventArgs());
            }
            /// <summary>
            /// OnAfterMove event calls the AfterMove delegate asigned if not is null.
            /// </summary>
            protected virtual void OnAfterMove()
            {
                if (AfterMove != null)
                    AfterMove(this, new EventArgs());
            }
            /// <summary>
            /// OnValidating event calls the Validating delegate is not is null.
            /// </summary>
            /// <param name="Action">Int returned.</param>
            /// <param name="Save">Int returned.</param>
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
            /// OnAddNewRecord calls the AddNewRecord delegate in case is not null, and 
            /// also assigns the NewRow value.
            /// </summary>
            /// <param name="NewRow"></param>
            protected virtual void OnAddNewRecord(out DataRow NewRow)
            {
                if (AddNewRecord != null)
                {
                    AddNewRowEventArgs arg = new AddNewRowEventArgs();
                    AddNewRecord(this, arg);
                    NewRow = arg.NewRow;
                }
                else
                    NewRow = null;
            }
            /// <summary>
            /// OnCancelCurrentEdit calls the CancelCurrentEdit delegate in case is not null.
            /// </summary>
            protected virtual void OnCancelCurrentEdit()
            {
                if (CancelCurrentEdit != null) CancelCurrentEdit(this, new EventArgs());
            }
            /// <summary>
            /// OnEndCurrentEdit calls the EndCurrentEdit delegate in case is not null.
            /// </summary>
            protected virtual void OnEndCurrentEdit()
            {
                if (EndCurrentEdit != null) EndCurrentEdit(this, new EventArgs());
            }

            /// <summary>
            /// Validates the Action to take considering the original action.
            /// </summary>
            /// <param name="oldAction">The original Action.</param>
            /// <param name="Action">The new Action to take.</param>
            /// <returns>The new action that should be taken.</returns>
            private DataValidateEnum ValidateNewAction(DataValidateEnum oldAction, int Action)
            {
                if (Enum.IsDefined(typeof(DataValidateEnum), Action))
                {
                    DataValidateEnum newAction = (DataValidateEnum)Action;
                    if (newAction != oldAction)
                    {
                        switch (oldAction)
                        {
                            case DataValidateEnum.vbDataActionAddNew:
                            case DataValidateEnum.vbDataActionMoveFirst:
                            case DataValidateEnum.vbDataActionMoveLast:
                            case DataValidateEnum.vbDataActionMoveNext:
                            case DataValidateEnum.vbDataActionMovePrevious:
                                if ((newAction == DataValidateEnum.vbDataActionCancel)
                                    || (newAction == DataValidateEnum.vbDataActionAddNew)
                                    || (newAction == DataValidateEnum.vbDataActionMoveFirst)
                                    || (newAction == DataValidateEnum.vbDataActionMoveLast)
                                    || (newAction == DataValidateEnum.vbDataActionMoveNext)
                                    || (newAction == DataValidateEnum.vbDataActionMovePrevious))
                                    return newAction;

                                throw new InvalidOperationException("New action is not allowed");
                            default:
                                if (newAction == DataValidateEnum.vbDataActionCancel)
                                    return newAction;

                                throw new InvalidOperationException("New action is not allowed");
                        }
                    }
                }
                else
                    throw new InvalidCastException("Invalid action value");

                return oldAction;
            }
            /// <summary>
            /// Constructor initialize variables.
            /// </summary>
            public OleDbRecordsetClass()
            {
                dbConnection = new OleDbConnection();
                dbAdapter = new OleDbDataAdapter();
                dbDataSet = new DataSet();
                newRow = false;
                newRowChanged = false;
                foundRecordsCounter = 0;
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// Initialize variables and sets the connection string.
            /// </summary>
            /// <param name="connString">string</param>
            public OleDbRecordsetClass(String connString)
            {
                dbConnection = new OleDbConnection(connString);
                dbAdapter = new OleDbDataAdapter();
                dbDataSet = new DataSet();
                newRow = false;
                newRowChanged = false;
                foundRecordsCounter = 0;
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// Initialize variables and sets the connectionstring and select query string.
            /// </summary>
            /// <param name="connString">Connection String.</param>
            /// <param name="sqlSelectString">Query String.</param>
            public OleDbRecordsetClass(String connString, String sqlSelectString)
            {
                this.dbConnection = new OleDbConnection(connString);
                this.SqlQuery = sqlSelectString;
                this.operationFinished = false;
                this.dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                this.dbDataSet = new DataSet();
                this.dbTable = new DataTable();
                this.dbTable = dbDataSet.Tables.Add("Table");
                this.dbAdapter.Fill(this.dbTable);
                CheckUpdateCommandsFromMetaData();
                this.operationFinished = true;
                this.newRow = false;
                this.newRowChanged = false;
                this.foundRecordsCounter = 0;
                this.OnAfterQuery();
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// Sets the Initial Position.
            /// </summary>
            private void SetInitialPosition()
            {
                if (RP_CanMoveFirst)
                {
                    _BOF = false;
                    _EOF = false;
                    MoveFirst();
                }
                else
                {
                    _BOF = true;
                    _EOF = true;
                    RP_Reset();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }

            /// <summary>
            /// Check if it is possible to create the update commands using meta data.
            /// </summary>
            private void CheckUpdateCommandsFromMetaData()
            {
                if (this.dbAdapter.InsertCommand == null)
                    CreateInsertCommandFromMetaData();

                if (this.dbAdapter.UpdateCommand == null)
                    CreateUpdateCommandFromMetaData();

                if (this.dbAdapter.DeleteCommand == null)
                    CreateDeleteCommandFromMetaData();
            }
            /// <summary>
            /// Builds sql statement from metadata to delete data.
            /// </summary>
            private void CreateDeleteCommandFromMetaData()
            {
                int j = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string wherePart = "";
                        string sql = "";
                        DbType dType;
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
                        {
                            if (wherePart.Length > 0)
                                wherePart += " AND ";

                            if (dColumn.AllowDBNull)
                            {
                                wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";

                                dType = getDBType(dColumn.DataType);
                                pInfo = new DBParameterInfo("p" + (++j), DbType.Int32, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                pInfo.SourceColumnNullMapping = true;
                                pInfo.Value = 1;
                                listGeneral.Add(pInfo);

                                pInfo = new DBParameterInfo("p" + (++j), dType, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listGeneral.Add(pInfo);
                            }
                            else
                            {
                                wherePart += "(" + dColumn.ColumnName + " = ?)";

                                pInfo = new DBParameterInfo("q" + (++j), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listGeneral.Add(pInfo);
                            }
                        }
                        sql = "DELETE FROM " + TableName + " WHERE (" + wherePart + ")";
                        this.SetDeleteQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// Builds sql insert statement from metadata.
            /// </summary>
            private void CreateInsertCommandFromMetaData()
            {
                int i = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string fieldsPart = "";
                        string valuesPart = "";
                        string sql = "";
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
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

                                pInfo = new DBParameterInfo("p" + (++i), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Current;
                                listGeneral.Add(pInfo);
                            }
                        }

                        sql = "INSERT INTO " + TableName + " (" + fieldsPart + ") VALUES (" + valuesPart + ")";
                        this.SetInsertQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// Builds sql update statement from metadata.
            /// </summary>
            private void CreateUpdateCommandFromMetaData()
            {
                int i = 0, j = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string updatePart = "";
                        string wherePart = "";
                        string sql = "";
                        DbType dType;
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        List<DBParameterInfo> listWhere = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
                        {
                            if (!dColumn.ReadOnly)
                            {
                                if (updatePart.Length > 0)
                                    updatePart += " , ";

                                updatePart += dColumn.ColumnName + " = ?";

                                pInfo = new DBParameterInfo("p" + (++i), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Current;
                                listGeneral.Add(pInfo);
                            }

                            if (wherePart.Length > 0)
                                wherePart += " AND ";

                            if (dColumn.AllowDBNull)
                            {
                                wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";

                                dType = getDBType(dColumn.DataType);
                                pInfo = new DBParameterInfo("q" + (++j), DbType.Int32, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                pInfo.SourceColumnNullMapping = true;
                                pInfo.Value = 1;
                                listWhere.Add(pInfo);

                                pInfo = new DBParameterInfo("q" + (++j), dType, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listWhere.Add(pInfo);
                            }
                            else
                            {
                                wherePart += "(" + dColumn.ColumnName + " = ?)";

                                pInfo = new DBParameterInfo("q" + (++j), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listWhere.Add(pInfo);
                            }
                        }
                        listGeneral.AddRange(listWhere);
                        sql = "UPDATE " + TableName + " SET " + updatePart + " WHERE " + wherePart;
                        this.SetUpdateQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// Gets DBType from a .NET Type.
            /// </summary>
            /// <param name="type">Type.</param>
            /// <returns>DBType.</returns>
            private DbType getDBType(Type type)
            {
                switch (type.Name)
                {
                    case "Byte":
                        return DbType.Byte;
                    case "Boolean":
                        return DbType.Boolean;
                    case "DateTime":
                        return DbType.DateTime;
                    case "Decimal":
                        return DbType.Decimal;
                    case "Double":
                        return DbType.Double;
                    case "Guid":
                        return DbType.Guid;
                    case "Int16":
                        return DbType.Int16;
                    case "Int32":
                        return DbType.Int32;
                    case "Int64":
                        return DbType.Int64;
                    case "Object":
                        return DbType.Object;
                    case "SByte":
                        return DbType.SByte;
                    case "Single":
                        return DbType.Single;
                    case "String":
                        return DbType.String;
                    case "UInt16":
                        return DbType.UInt16;
                    case "UInt32":
                        return DbType.UInt32;
                    case "UInt64":
                        return DbType.UInt64;
                }

                return DbType.String;
            }

            /// <summary>
            /// Destructor, close the recordset.
            /// </summary>
            ~OleDbRecordsetClass()
            {
                if (dbConnection != null)
                {
                    CloseRecordSet();
                }
            }

            /// <summary>
            /// Methods to move the current record set.
            /// </summary>
            public void MoveFirst()
            {

                int Action = (int)DataValidateEnum.vbDataActionMoveFirst;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveFirst, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }
                else
                    DiscardAddNewChanges();

                if (RP_CanMoveFirst)
                {
                    _BOF = false;
                    _EOF = false;
                    RP_MoveFirst();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }

            public void MoveLast(int Options)
            {
                //TODO: ToBeImplemented where Options != 0
                if (Options == 0)
                    MoveLast();
            }

            public void MoveLast()
            {

                int Action = (int)DataValidateEnum.vbDataActionMoveLast;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveLast, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }
                else
                    DiscardAddNewChanges();

                if (RP_CanMoveLast)
                {
                    _BOF = false;
                    _EOF = false;
                    RP_MoveLast();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// MoveNext.
            /// </summary>
            public void MoveNext()
            {

                int Action = (int)DataValidateEnum.vbDataActionMoveNext;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveNext, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }
                else
                    DiscardAddNewChanges();

                if (RP_CanMoveNext || ((_AbsolutePosition != RP)))
                {
                    _BOF = false;
                    _EOF = false;
                    if (_AbsolutePosition != RP)
                    {
                        if (RP_CanMoveAtPosition(RP))
                            RP_MoveAtPosition(RP);
                        else if (RP_CanMoveLast)
                        {
                            RP_MoveLast();
                        }
                        else
                            RP_Reset();
                    }
                    else
                        RP_MoveNext();

                    _EOF = !RP_CanMoveNext;
                    if (!_EOF)
                        _AbsolutePosition = RP;
                    else
                        _AbsolutePosition = -1;

                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// MovePrevious.
            /// </summary>
            public void MovePrevious()
            {

                int Action = (int)DataValidateEnum.vbDataActionMovePrevious;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMovePrevious, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }
                else
                    DiscardAddNewChanges();

                //FSQ20080207 - Bug 4041
                if (RP_CanMovePrevious)
                {
                    _EOF = false;
                    RP_MovePrevious();
                    _BOF = !RP_CanMovePrevious;
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }

            /// <summary>
            /// The Data Control discard changes made by AddNew if 
            /// any operation moves to another record
            /// </summary>
            private void DiscardAddNewChanges()
            {
                if (newRow)
                {
                    if (newRowChanged)
                    {
                        Update();
                    }
                    else
                    {
                        dbRow.Table.ColumnChanged -= new DataColumnChangeEventHandler(NewRow_ColumnChanged);
                        dbRow = null;

                        OnCancelCurrentEdit();
                        newRow = false;
                    }
                }
            }

            /// <summary>
            /// AddNew data.
            /// </summary>
            public void AddNew()
            {

                int Action = (int)DataValidateEnum.vbDataActionAddNew;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionAddNew, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }
                else
                    DiscardAddNewChanges();

                OnAfterMove();

                OnAddNewRecord(out dbRow);

                if (dbRow == null)
                    throw new InvalidOperationException("No records were added");

                dbRow.Table.ColumnChanged += new DataColumnChangeEventHandler(NewRow_ColumnChanged);
                newRow = true;
                newRowChanged = false;
            }
            /// <summary>
            /// event NewRow_ColumnChanged.
            /// </summary>
            /// <param name="sender">Object sender.</param>
            /// <param name="e">DataColumnChangeEventArgs.</param>
            private void NewRow_ColumnChanged(object sender, DataColumnChangeEventArgs e)
            {
                newRowChanged = true;
            }
            /// <summary>
            /// Delete Record.
            /// </summary>
            public void Delete()
            {
                int Action = (int)DataValidateEnum.vbDataActionDelete;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionDelete, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                DataRow deletingRow = dbDataSet.Tables[0].Rows[RP];
                deletingRow.Delete();
                dbAdapter.Update(dbDataSet.Tables[0].GetChanges(DataRowState.Deleted));
                //FSQ20080207 - Bug 4041
                dbDataSet.Clear();
                dbAdapter.Fill(dbDataSet, "Table");
                _AbsolutePosition = -1;
            }
            /// <summary>
            /// Edit Record. Not implemented already.
            /// </summary>
            public void Edit()
            {
                //TODO: ToBeImplemented
                //throw new System.Exception("Method or Property not implemented yet!");
            }
            /// <summary>
            /// Updates Record.
            /// </summary>
            public void Update()
            {
                int Action = (int)DataValidateEnum.vbDataActionUpdate;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionUpdate, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (newRow)
                {
                    dbRow.Table.ColumnChanged -= new DataColumnChangeEventHandler(NewRow_ColumnChanged);
                    dbRow = null;

                    OnEndCurrentEdit();

                    newRow = false;
                    newRowChanged = false;
                    _BOF = false;

                    if (_AbsolutePosition == -1)
                        RP_MoveLast();

                    OnAfterMove();
                }
                
                int i = dbAdapter.Update(dbDataSet);
            }
            /// <summary>
            /// Updates Data, by now only UpdateType in 1 is supported.
            /// </summary>
            /// <param name="UpdateType">Int Update Type.</param>
            /// <param name="Force">Boolean force, not used.</param>
            public void Update(int UpdateType, bool Force)
            {
                //For now only this type is supported
                if (UpdateType == 1)
                    Update();
            }

            /// <summary>
            /// Creates an Adapter using a query and an the connection.
            /// </summary>
            /// <param name="query">Query string.</param>
            /// <param name="con">OleDbConnection connection.</param>
            /// <returns></returns>
            private OleDbDataAdapter CreateAdapter(String query, OleDbConnection con)
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.MissingMappingAction = MissingMappingAction.Passthrough;
                if (updateCommand == null || deleteCommand == null || insertCommand == null)
                {
                    cmdBuilder = new OleDbCommandBuilder(adapter);
                    try
                    {
                        adapter.UpdateCommand = (updateCommand != null) ? updateCommand : updateCommand = cmdBuilder.GetUpdateCommand();
                        adapter.InsertCommand = (insertCommand != null) ? insertCommand : insertCommand = cmdBuilder.GetInsertCommand();
                        adapter.DeleteCommand = (deleteCommand != null) ? deleteCommand : deleteCommand = cmdBuilder.GetDeleteCommand();
                    }
                    catch { }
                }
                else
                {
                    adapter.UpdateCommand = updateCommand;
                    adapter.InsertCommand = insertCommand;
                    adapter.DeleteCommand = deleteCommand;
                }


                return adapter;
            }
            /// <summary>
            /// Disposes the class and close the recordset.
            /// </summary>
            public void Dispose()
            {
                CloseRecordSet();
            }
            /// <summary>
            /// Close the recordset.
            /// </summary>
            public void CloseRecordSet()
            {
                try
                {
                    int Action = (int)DataValidateEnum.vbDataActionClose;
                    int Save = 0;
                    OnValidating(ref Action, ref Save);
                    //switch (ValidateNewAction(DataValidateEnum.vbDataActionClose, Action))
                    //{
                    //}

                    if (Save != 0)
                    {
                        Edit();
                        Update();
                    }

                    if (dbTable != null) dbTable.Dispose();
                    if (dbDataSet != null) dbDataSet.Dispose();
                    if (dbAdapter != null) dbAdapter.Dispose();
                    if (dbConnection != null) dbConnection.Dispose();
                    dbConnection = null;
                }
                catch { }
                finally
                {
                    Utils.MemoryHelper.ReleaseMemory();
                }
            }
            /// <summary>
            /// Sets the ConnectionString to the dbConnection.
            /// </summary>
            /// <param name="constr">Connection String</param>
            /// <returns></returns>
            public bool Connection(String constr)
            {
                dbConnection.ConnectionString = constr;
                dbConnection.Open();
                dbConnection.Close();
                return true;
            }
            /// <summary>
            /// Sets the connection if the connection string is set.
            /// </summary>
            /// <returns></returns>
            public bool Connection()
            {
                if (dbConnection.ConnectionString != null)
                    return Connection(dbConnection.ConnectionString);
                else
                    throw new ArgumentException("ConnectionString must be set prior method invocation");
            }
            /// <summary>
            /// Opens the recordset if the SqlQuery is not null.
            /// </summary>
            /// <returns></returns>
            private bool OpenRecordSet()
            {
                if (SqlQuery != null)
                    return OpenRecordSet(SqlQuery);
                else
                    throw new ArgumentException("sqlQuery must be set prior method invocation");
            }
            /// <summary>
            /// Opens the recordset using the SQLstr value.
            /// </summary>
            /// <param name="SQLstr">Query string.</param>
            /// <returns></returns>
            private bool OpenRecordSet(String SQLstr)
            {
                SqlQuery = SQLstr;
                operationFinished = false;
                dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                dbTable = dbDataSet.Tables.Add("Table");
                dbAdapter.Fill(dbTable);
                CheckUpdateCommandsFromMetaData();
                operationFinished = true;
                newRow = false;
                newRowChanged = false;
                foundRecordsCounter = 0;

                SetInitialPosition();
                return true;
            }
            /// <summary>
            /// Returns an instance of the current instance.
            /// </summary>
            /// <returns></returns>
            public RecordsetClass Clone()
            {
                return new OleDbRecordsetClass(this.dbConnection.ConnectionString, SqlQuery);
            }
            /// <summary>
            /// Method to Find using a column name and a criteria of search.
            /// </summary>
            /// <param name="columnName">Column name to use.</param>
            /// <param name="criteria">Criteria object to use.</param>
            public void FindFirst(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                int memory;
                found = false;
                _AbsolutePosition = 0;

                while (!found && _AbsolutePosition < dbDataSet.Tables[0].Rows.Count)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition++;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition++;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition++;
                }
                if (found)
                {
                    foundRecordsCounter = 0;
                    memory = _AbsolutePosition;
                    while (!NoMatch)
                    {
                        foundRecordsCounter++;
                        FindNext(columnName, criteria);
                    }
                    _AbsolutePosition = memory;
                    found = true;
                }
                else
                    foundRecordsCounter = 0;

                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Method FindLast finds the last row matching the criteria, using the column name.
            /// </summary>
            /// <param name="columnName">Column name string.</param>
            /// <param name="criteria">Criteria Object.</param>
            public void FindLast(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                int memory;
                found = false;
                _AbsolutePosition = dbDataSet.Tables[0].Rows.Count - 1;

                while (!found && _AbsolutePosition < -1)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition--;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition--;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition--;
                }
                if (found)
                {
                    foundRecordsCounter = 0;
                    memory = _AbsolutePosition;
                    while (!NoMatch)
                    {
                        foundRecordsCounter++;
                        FindPrevious(columnName, criteria);
                    }
                    _AbsolutePosition = memory;
                    found = true;
                }
                else
                    foundRecordsCounter = 0;

                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Finds Previous method search back from the actual position using 
            /// the column name and criteria.
            /// </summary>
            /// <param name="columnName">Column name string.</param>
            /// <param name="criteria">Criteria object.</param>
            public void FindPrevious(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                _AbsolutePosition--;
                found = false;

                while (!found && _AbsolutePosition > -1)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition--;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition--;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition--;
                }
                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Finds next method search for the next record from the actual position using 
            /// the column name and criteria.
            /// </summary>
            /// <param name="columnName"></param>
            /// <param name="criteria"></param>
            public void FindNext(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                _AbsolutePosition++;
                found = false;

                while (!found && _AbsolutePosition < dbDataSet.Tables[0].Rows.Count)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition++;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition++;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition++;
                }
                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Requerys the recordset.
            /// </summary>
            public void Refresh()
            {
                Requery();
            }
            /// <summary>
            /// Requery using the internal query string.
            /// </summary>
            /// <returns></returns>
            public bool Requery()
            {
                return Requery(SqlQuery);
            }
            /// <summary>
            /// Returns the internal query string
            /// </summary>
            public string RecordSource
            {
                get
                {
                    return SqlQuery;
                }
            }
            /// <summary>
            /// Performs the query and fills the internal data structures.
            /// </summary>
            /// <param name="SQLstr"></param>
            /// <returns></returns>
            private bool Requery(String SQLstr)
            {
                SqlQuery = SQLstr;
                operationFinished = false;
                dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                dbTable = dbDataSet.Tables.Add("Table");
                dbAdapter.Fill(dbTable);
                CheckUpdateCommandsFromMetaData();
                operationFinished = true;
                newRow = false;
                newRowChanged = false;
                foundRecordsCounter = 0;

                SetInitialPosition();
                return true;
            }
            /// <summary>
            /// NoMatch: get if record is found.
            /// </summary>
            public bool NoMatch
            {
                get { return found; }
            }
            /// <summary>
            /// AbsolutePosition: Get/Set Absolute position value.
            /// </summary>
            public int AbsolutePosition
            {
                get { return _AbsolutePosition; }
                set
                {
                    RP_MoveAtPosition(value);
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }
            
            //FSQ20080207 - Bug 4041
            /// <summary>
            /// BOF: Get the BOF state.
            /// </summary>
            private bool _BOF = false;
            public bool BOF
            {
                get { return _BOF; }
            }
            /// <summary>
            /// EOF: Get the EOF state.
            /// </summary>
            private bool _EOF = false;
            public bool EOF
            {
                get { return _EOF; }
            }
            /// <summary>
            /// Helps to access the value from a column table as an array.
            /// </summary>
            /// <param name="columnName"></param>
            /// <returns></returns>
            public Object this[String columnName]
            {
                get
                {
                    ValidateCurrentPosition();
                    return dbDataSet.Tables[0].Rows[RP][columnName];
                }
                set
                {
                    ValidateCurrentPosition();
                    if (newRow)
                    {
                        if (!dbRow[columnName].Equals(value))
                            dbRow[columnName] = value;
                    }
                    else
                        dbDataSet.Tables[0].Rows[RP][columnName] = value;
                }
            }
            /// <summary>
            /// Access the table by a column index.
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public Object this[int columnIndex]
            {
                get
                {
                    ValidateCurrentPosition();
                    return dbDataSet.Tables[0].Rows[RP][columnIndex];
                }
                set
                {
                    ValidateCurrentPosition();
                    if (newRow)
                    {
                        if (!dbRow[columnIndex].Equals(value))
                            dbRow[columnIndex] = value;
                    }
                    else
                        dbDataSet.Tables[0].Rows[RP][columnIndex] = value;
                }
            }
            /// <summary>
            /// Validates that current postion is not a EOF, BOF and not a new row.
            /// </summary>
            private void ValidateCurrentPosition()
            {
                if ((BOF || EOF) && !newRow)
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RecordsFound: Get the state for found records.
            /// </summary>
            public long RecordsFound
            {
                get { return foundRecordsCounter; }
            }
            /// <summary>
            /// RecourdCount: Get the rows count.
            /// </summary>
            public long RecordCount
            {
                get { return dbDataSet.Tables[0].Rows.Count; }
            }
            /// <summary>
            /// IsLoadingfinnished: Get the operation finished state.
            /// </summary>
            public bool IsLoadingFinnished
            {
                get { return operationFinished; }
            }
            /// <summary>
            /// Fields: Get the FieldsClass internal instance.
            /// </summary>
            public FieldsClass Fields
            {
                get
                {
                    return classFields;
                }
            }
            /// <summary>
            /// SqlQuery: Get/Set the query string and sets the TableName.
            /// </summary>
            public String SqlQuery
            {
                get { return sqlSelectQuery; }
                set
                {
                    sqlSelectQuery = value;
                    TableName = getTableName(sqlSelectQuery);
                }
            }

            /// <summary>
            /// Try to find the name of the table from the select query.
            /// </summary>
            /// <param name="sqlSelectQuery">The query to parse.</param>
            /// <returns>The name of the table or empty string.</returns>
            private string getTableName(string sqlSelectQuery)
            {
                Match mtch;
                if (!string.IsNullOrEmpty(sqlSelectQuery))
                {
                    if ((mtch = Regex.Match(sqlSelectQuery.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+([\w.]+)[ \t]*.*$", RegexOptions.IgnoreCase)) != Match.Empty)
                        return mtch.Groups[1].Value.Trim();
                }

                return string.Empty;
            }
            /// <summary>
            /// Bookmark: Get/Set the BookMark.
            /// </summary>
            public DataRow Bookmark
            {
                get
                {
                    return dbDataSet.Tables[0].Rows[RP];
                }
                set
                {
                    int Action = (int)DataValidateEnum.vbDataActionBookmark;
                    int Save = 0;
                    OnValidating(ref Action, ref Save);
                    switch (ValidateNewAction(DataValidateEnum.vbDataActionBookmark, Action))
                    {
                        case DataValidateEnum.vbDataActionCancel:
                            return;
                    }

                    if (Save != 0)
                    {
                        Edit();
                        Update();
                    }

                    RP_MoveAtPosition(dbDataSet.Tables[0].Rows.IndexOf(value));
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }
            /// <summary>
            /// DataSet: Get the internal DataSet.
            /// </summary>
            public DataSet DataSet
            {
                get { return dbDataSet; }
            }

            /// <summary>
            /// Methods to override the commands created by default to update the recordset.
            /// </summary>
            public void SetInsertQuery(String InsertQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.InsertCommand = new OleDbCommand(InsertQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.InsertCommand.Parameters.Add(paramInfo.getOleDbParameter());
                    }
                }
            }
            /// <summary>
            /// Sets the UpdateQuery in the adapter.
            /// </summary>
            /// <param name="UpdateQuery">Query String.</param>
            /// <param name="parameters">DBParameterInfo List.</param>
            public void SetUpdateQuery(String UpdateQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.UpdateCommand = new OleDbCommand(UpdateQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.UpdateCommand.Parameters.Add(paramInfo.getOleDbParameter());
                    }
                }
            }
            /// <summary>
            /// Sets the Delete Query in the adapter.
            /// </summary>
            /// <param name="DeleteQuery">Query String.</param>
            /// <param name="parameters">DbParameterInfo List.</param>
            public void SetDeleteQuery(String DeleteQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.DeleteCommand = new OleDbCommand(DeleteQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.DeleteCommand.Parameters.Add(paramInfo.getOleDbParameter());
                    }
                }
            }
            /// <summary>
            /// Move to Last Record.
            /// </summary>
            /// <param name="Options">only used when is Type.Missing.</param>
            public void MoveLast(object Options)
            {
                //TODO: ToBeImplemented where Options isnot Type.Missing
                if (Options.Equals(Type.Missing))
                    MoveLast();
            }
            /// <summary>
            /// Cancel. Not implemented.
            /// </summary>
            public void Cancel()
            {
                //Nothing to do
            }
            /// <summary>
            /// Close. Not implemented.
            /// </summary>
            public void Close()
            {
                //Nothing to do
            }
            /// <summary>
            /// Name: Get the internal query.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.SqlQuery;
                }
            }
        }

        /// <summary>
        /// Implementation of RecordsetClass using ODBC.
        /// </summary>
        private class ODBCRecordsetClass : RecordsetClass
        {
            private OdbcConnection dbConnection = null;
            private OdbcDataAdapter dbAdapter = null;
            private OdbcCommandBuilder cmdBuilder = null;
            private DataSet dbDataSet = null;
            private DataTable dbTable = null;
            private DataRow dbRow = null;
            private int _AbsolutePosition = -1;
            private int _RecordPosition = -1;
            private bool newRow = false;
            private bool found = false;
            private String sqlSelectQuery = string.Empty;
            private String TableName = string.Empty;
            private OdbcCommand updateCommand = null;
            private OdbcCommand deleteCommand = null;
            private OdbcCommand insertCommand = null;
            private long foundRecordsCounter = 0;
            private bool operationFinished = false;
            private FieldsClass classFields = null;

            public event EventHandler AfterQuery = null;
            public event EventHandler AfterMove = null;
            public event ValidatingEventHandler Validating = null;
            public event AddNewRowEventHandler AddNewRecord = null;
            public event EventHandler CancelCurrentEdit = null;
            public event EventHandler EndCurrentEdit = null;

            /// <summary>
            /// RP: Get Record Position.
            /// </summary>
            public int RP
            {
                get { return _RecordPosition; }
            }
            /// <summary>
            /// RP_CanMoveFirst: Get if there are records so is possible to move first.
            /// </summary>
            private bool RP_CanMoveFirst
            {
                get
                {
                    return RecordCount > 0;
                }
            }
            /// <summary>
            /// RP_CanMoveLast: Get if there are records so is possible to move last.
            /// </summary>
            private bool RP_CanMoveLast
            {
                get
                {
                    return RP_CanMoveFirst;
                }
            }
            /// <summary>
            /// RP_CanMoveNext: Check if is possible to move next.
            /// </summary>
            public bool RP_CanMoveNext
            {
                get
                {
                    return (RecordCount > 0) && (_RecordPosition < RecordCount);
                }
            }
            /// <summary>
            /// RP_CanMovePrevious: Check if is possible to move back.
            /// </summary>
            public bool RP_CanMovePrevious
            {
                get
                {
                    return _RecordPosition >= 0;
                }
            }
            /// <summary>
            /// RP_CanMoveAtPosition: Check if possible to move to a specific position.
            /// </summary>
            /// <param name="position">Int position.</param>
            /// <returns></returns>
            private bool RP_CanMoveAtPosition(int position)
            {
                return (position >= 0) && (position < RecordCount);
            }
            /// <summary>
            /// RP_MoveFirst: if can move to first position, move it.
            /// </summary>
            private void RP_MoveFirst()
            {
                if (RP_CanMoveFirst)
                {
                    _RecordPosition = 0;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveLast: if can move to last position, move it.
            /// </summary>
            private void RP_MoveLast()
            {
                if (RP_CanMoveLast)
                {
                    _RecordPosition = (int)RecordCount - 1;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveNext: if can move to next position, move it.
            /// </summary>
            private void RP_MoveNext()
            {
                if (RP_CanMoveNext)
                {
                    _RecordPosition++;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MovePrevious: if can move to previous position, move it.
            /// </summary>
            private void RP_MovePrevious()
            {
                if (RP_CanMovePrevious)
                {
                    _RecordPosition--;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_MoveAtPosition: if can move to a specific position, move it.
            /// </summary>
            /// <param name="position"></param>
            private void RP_MoveAtPosition(int position)
            {
                if (RP_CanMoveAtPosition(position))
                {
                    _RecordPosition = position;
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RP_Reset: Reset the position.
            /// </summary>
            private void RP_Reset()
            {
                _RecordPosition = -1;
            }
            /// <summary>
            /// OnAfterQuery: if delegate AfterQuery is not null, call it.
            /// </summary>
            protected virtual void OnAfterQuery()
            {
                if (AfterQuery != null) AfterQuery(this, new EventArgs());
            }
            /// <summary>
            /// OnAfterMove: if delegate AfterMove is not null, call it.
            /// </summary>
            protected virtual void OnAfterMove()
            {
                if (AfterMove != null)
                    AfterMove(this, new EventArgs());
            }
            /// <summary>
            /// OnValidating: if delegate Validating is not null, 
            /// creates the ValidatingEventArgs using parameters and call the delegate.
            /// </summary>
            /// <param name="Action">Int Action returned.</param>
            /// <param name="Save">Int Save returned.</param>
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
            /// OnAddNewRecord: if delegate AddNewRecord is not null, 
            /// creates the AddNewRowEventArgs, call the delegate and return the NewRow.
            /// </summary>
            /// <param name="NewRow"></param>
            protected virtual void OnAddNewRecord(out DataRow NewRow)
            {
                if (AddNewRecord != null)
                {
                    AddNewRowEventArgs args = new AddNewRowEventArgs();
                    AddNewRecord(this, args);
                    NewRow = args.NewRow;
                }
                else
                    NewRow = null;
            }
            /// <summary>
            /// OnCancelCurrentEdit: if delegate CancelCurrentEdit is not null, call it.
            /// </summary>
            protected virtual void OnCancelCurrentEdit()
            {
                if (CancelCurrentEdit != null) CancelCurrentEdit(this, new EventArgs());
            }
            /// <summary>
            /// OnEdnCurrentEdit: if delegate EndCurrentEdit is not null, call it.
            /// </summary>
            protected virtual void OnEndCurrentEdit()
            {
                if (EndCurrentEdit != null) EndCurrentEdit(this, new EventArgs());
            }

            /// <summary>
            /// Validates the Action to take considering the original action.
            /// </summary>
            /// <param name="oldAction">The original Action.</param>
            /// <param name="Action">The new Action to take.</param>
            /// <returns>The new action that should be taken.</returns>
            private DataValidateEnum ValidateNewAction(DataValidateEnum oldAction, int Action)
            {
                if (Enum.IsDefined(typeof(DataValidateEnum), Action))
                {
                    DataValidateEnum newAction = (DataValidateEnum)Action;
                    if (newAction != oldAction)
                    {
                        switch (oldAction)
                        {
                            case DataValidateEnum.vbDataActionAddNew:
                            case DataValidateEnum.vbDataActionMoveFirst:
                            case DataValidateEnum.vbDataActionMoveLast:
                            case DataValidateEnum.vbDataActionMoveNext:
                            case DataValidateEnum.vbDataActionMovePrevious:
                                if ((newAction == DataValidateEnum.vbDataActionCancel)
                                    || (newAction == DataValidateEnum.vbDataActionAddNew)
                                    || (newAction == DataValidateEnum.vbDataActionMoveFirst)
                                    || (newAction == DataValidateEnum.vbDataActionMoveLast)
                                    || (newAction == DataValidateEnum.vbDataActionMoveNext)
                                    || (newAction == DataValidateEnum.vbDataActionMovePrevious))
                                    return newAction;

                                throw new InvalidOperationException("New action is not allowed");
                            default:
                                if (newAction == DataValidateEnum.vbDataActionCancel)
                                    return newAction;

                                throw new InvalidOperationException("New action is not allowed");
                        }
                    }
                }
                else
                    throw new InvalidCastException("Invalid action value");

                return oldAction;
            }
            /// <summary>
            /// ODBCRecordsetClass Constructor: initialize internal variables.
            /// </summary>
            public ODBCRecordsetClass()
            {
                dbConnection = new OdbcConnection();
                dbAdapter = new OdbcDataAdapter();
                dbDataSet = new DataSet();
                newRow = false;
                foundRecordsCounter = 0;
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// ODBCRecordsetClass Constructor: initialize internal variables, 
            /// the connection is initialized using the connection string.
            /// </summary>
            /// <param name="connString"></param>
            public ODBCRecordsetClass(String connString)
            {
                dbConnection = new OdbcConnection(connString);
                dbAdapter = new OdbcDataAdapter();
                dbDataSet = new DataSet();
                newRow = false;
                foundRecordsCounter = 0;
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// ODBCRecordsetClass Constructor: initialize internal variables, 
            /// the connection is initialized using the connection string, and 
            /// the query uses the sqlSelectString.
            /// </summary>
            /// <param name="connString"></param>
            /// <param name="sqlSelectString"></param>
            public ODBCRecordsetClass(String connString, String sqlSelectString)
            {
                this.dbConnection = new OdbcConnection(connString);
                this.SqlQuery = sqlSelectString;
                this.operationFinished = false;
                this.dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                this.dbDataSet = new DataSet();
                this.dbTable = new DataTable();
                this.dbTable = dbDataSet.Tables.Add("Table");
                this.dbAdapter.Fill(this.dbTable);
                CheckUpdateCommandsFromMetaData();
                this.operationFinished = true;
                this.newRow = false;
                this.foundRecordsCounter = 0;
                this.OnAfterQuery();
                classFields = new FieldsClass(this);

                SetInitialPosition();
            }
            /// <summary>
            /// Sets Initial Position, and move to first position.
            /// </summary>
            private void SetInitialPosition()
            {
                if (RP_CanMoveFirst)
                {
                    _BOF = false;
                    _EOF = false;
                    MoveFirst();
                }
                else
                {
                    _BOF = true;
                    _EOF = true;
                    RP_Reset();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }

            /// <summary>
            /// Check if it is possible to create the update commands using meta data.
            /// </summary>
            private void CheckUpdateCommandsFromMetaData()
            {
                if (this.dbAdapter.InsertCommand == null)
                    CreateInsertCommandFromMetaData();

                if (this.dbAdapter.UpdateCommand == null)
                    CreateUpdateCommandFromMetaData();

                if (this.dbAdapter.DeleteCommand == null)
                    CreateDeleteCommandFromMetaData();
            }
            /// <summary>
            /// Creates a SQL delete query string from metadata.
            /// </summary>
            private void CreateDeleteCommandFromMetaData()
            {
                int j = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string wherePart = "";
                        string sql = "";
                        DbType dType;
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
                        {
                            if (wherePart.Length > 0)
                                wherePart += " AND ";

                            if (dColumn.AllowDBNull)
                            {
                                wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";

                                dType = getDBType(dColumn.DataType);
                                pInfo = new DBParameterInfo("p" + (++j), DbType.Int32, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                pInfo.SourceColumnNullMapping = true;
                                pInfo.Value = 1;
                                listGeneral.Add(pInfo);

                                pInfo = new DBParameterInfo("p" + (++j), dType, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listGeneral.Add(pInfo);
                            }
                            else
                            {
                                wherePart += "(" + dColumn.ColumnName + " = ?)";

                                pInfo = new DBParameterInfo("q" + (++j), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listGeneral.Add(pInfo);
                            }
                        }
                        sql = "DELETE FROM " + TableName + " WHERE (" + wherePart + ")";
                        this.SetDeleteQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// Creates a SQL insert query string from metadata.
            /// </summary>
            private void CreateInsertCommandFromMetaData()
            {
                int i = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string fieldsPart = "";
                        string valuesPart = "";
                        string sql = "";
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
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

                                pInfo = new DBParameterInfo("p" + (++i), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Current;
                                listGeneral.Add(pInfo);
                            }
                        }

                        sql = "INSERT INTO " + TableName + " (" + fieldsPart + ") VALUES (" + valuesPart + ")";
                        this.SetInsertQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// Creates a SQL update query string from metadata.
            /// </summary>
            private void CreateUpdateCommandFromMetaData()
            {
                int i = 0, j = 0;
                try
                {
                    if (!string.IsNullOrEmpty(TableName))
                    {
                        string updatePart = "";
                        string wherePart = "";
                        string sql = "";
                        DbType dType;
                        List<DBParameterInfo> listGeneral = new List<DBParameterInfo>();
                        List<DBParameterInfo> listWhere = new List<DBParameterInfo>();
                        DBParameterInfo pInfo = null;

                        foreach (System.Data.DataColumn dColumn in dbDataSet.Tables[0].Columns)
                        {
                            if (!dColumn.ReadOnly)
                            {
                                if (updatePart.Length > 0)
                                    updatePart += " , ";

                                updatePart += dColumn.ColumnName + " = ?";

                                pInfo = new DBParameterInfo("p" + (++i), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Current;
                                listGeneral.Add(pInfo);
                            }

                            if (wherePart.Length > 0)
                                wherePart += " AND ";

                            if (dColumn.AllowDBNull)
                            {
                                wherePart += "((? = 1 AND " + dColumn.ColumnName + " IS NULL) OR (" + dColumn.ColumnName + " = ?))";

                                dType = getDBType(dColumn.DataType);
                                pInfo = new DBParameterInfo("q" + (++j), DbType.Int32, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                pInfo.SourceColumnNullMapping = true;
                                pInfo.Value = 1;
                                listWhere.Add(pInfo);

                                pInfo = new DBParameterInfo("q" + (++j), dType, dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listWhere.Add(pInfo);
                            }
                            else
                            {
                                wherePart += "(" + dColumn.ColumnName + " = ?)";

                                pInfo = new DBParameterInfo("q" + (++j), getDBType(dColumn.DataType), dColumn.MaxLength, dColumn.ColumnName);
                                pInfo.SourceVersion = DataRowVersion.Original;
                                listWhere.Add(pInfo);
                            }
                        }
                        listGeneral.AddRange(listWhere);
                        sql = "UPDATE " + TableName + " SET " + updatePart + " WHERE " + wherePart;
                        this.SetUpdateQuery(sql, listGeneral);
                    }
                }
                catch { }
            }
            /// <summary>
            /// getDBType: Return DbType map from type.Name.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            private DbType getDBType(Type type)
            {
                switch (type.Name)
                {
                    case "Byte":
                        return DbType.Byte;
                    case "Boolean":
                        return DbType.Boolean;
                    case "DateTime":
                        return DbType.DateTime;
                    case "Decimal":
                        return DbType.Decimal;
                    case "Double":
                        return DbType.Double;
                    case "Guid":
                        return DbType.Guid;
                    case "Int16":
                        return DbType.Int16;
                    case "Int32":
                        return DbType.Int32;
                    case "Int64":
                        return DbType.Int64;
                    case "Object":
                        return DbType.Object;
                    case "SByte":
                        return DbType.SByte;
                    case "Single":
                        return DbType.Single;
                    case "String":
                        return DbType.String;
                    case "UInt16":
                        return DbType.UInt16;
                    case "UInt32":
                        return DbType.UInt32;
                    case "UInt64":
                        return DbType.UInt64;
                }

                return DbType.String;
            }

            /// <summary>
            /// Destructor.
            /// </summary>
            ~ODBCRecordsetClass()
            {
                if (dbConnection != null)
                {
                    CloseRecordSet();
                }
            }
            /// <summary>
            /// Moves to First Position.
            /// </summary>
            public void MoveFirst()
            {
                int Action = (int)DataValidateEnum.vbDataActionMoveFirst;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveFirst, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                if (RP_CanMoveFirst)
                {
                    _BOF = false;
                    _EOF = false;
                    RP_MoveFirst();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// Moves to Last Position, parameter Options must be 0 by now.
            /// </summary>
            /// <param name="Options">int</param>
            public void MoveLast(int Options)
            {
                //TODO: ToBeImplemented where Options != 0
                if (Options == 0)
                    MoveLast();
            }
            /// <summary>
            /// Moves to Last Position.
            /// </summary>
            public void MoveLast()
            {
                int Action = (int)DataValidateEnum.vbDataActionMoveLast;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveLast, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                if (RP_CanMoveLast)
                {
                    _BOF = false;
                    _EOF = false;
                    RP_MoveLast();
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// Moves to next record.
            /// </summary>
            public void MoveNext()
            {
                int Action = (int)DataValidateEnum.vbDataActionMoveNext;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMoveNext, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                if (RP_CanMoveNext || ((_AbsolutePosition != RP)))
                {
                    _BOF = false;
                    _EOF = false;
                    if (_AbsolutePosition != RP)
                    {
                        if (RP_CanMoveAtPosition(RP))
                            RP_MoveAtPosition(RP);
                        else if (RP_CanMoveLast)
                        {
                            RP_MoveLast();
                        }
                        else
                            RP_Reset();
                    }
                    else
                        RP_MoveNext();

                    _EOF = !RP_CanMoveNext;
                    if (!_EOF)
                        _AbsolutePosition = RP;
                    else
                        _AbsolutePosition = -1;

                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }
            /// <summary>
            /// Moves to Previous record.
            /// </summary>
            public void MovePrevious()
            {
                int Action = (int)DataValidateEnum.vbDataActionMovePrevious;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionMovePrevious, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionAddNew:
                        AddNew();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                //FSQ20080207 - Bug 4041
                if (RP_CanMovePrevious)
                {
                    _EOF = false;
                    RP_MovePrevious();
                    _BOF = !RP_CanMovePrevious;
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
                else
                    throw new Exception("No current record");
            }

            /// <summary>
            /// Add New Record.
            /// </summary>
            public void AddNew()
            {
                int Action = (int)DataValidateEnum.vbDataActionAddNew;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionAddNew, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                    case DataValidateEnum.vbDataActionMoveFirst:
                        MoveFirst();
                        return;
                    case DataValidateEnum.vbDataActionMoveLast:
                        MoveLast();
                        return;
                    case DataValidateEnum.vbDataActionMoveNext:
                        MoveNext();
                        return;
                    case DataValidateEnum.vbDataActionMovePrevious:
                        MovePrevious();
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                OnAddNewRecord(out dbRow);

                if (dbRow == null)
                    throw new InvalidOperationException("No records were added");

                dbRow.Table.ColumnChanged += new DataColumnChangeEventHandler(NewRow_ColumnChanged);
                newRow = true;
                OnAfterMove();
            }
            /// <summary>
            /// Delete actual record.
            /// </summary>
            public void Delete()
            {
                int Action = (int)DataValidateEnum.vbDataActionDelete;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionDelete, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                Exception pendingException = null;

                DataRow deletingRow = dbDataSet.Tables[0].Rows[RP];
                deletingRow.Delete();
                try
                {
                    dbAdapter.Update(dbDataSet.Tables[0].GetChanges(DataRowState.Deleted));
                }
                catch (Exception e)
                {
                    pendingException = e;
                }
                //FSQ20080207 - Bug 4041
                dbDataSet.Clear();
                dbAdapter.Fill(dbDataSet, "Table");
                _AbsolutePosition = -1;

                if (pendingException != null)
                    throw pendingException;
            }
            /// <summary>
            /// Edit Record. Not Implemented already.
            /// </summary>
            public void Edit()
            {
                //TODO: ToBeImplemented
                //throw new System.Exception("Method or Property not implemented yet!");
            }
            /// <summary>
            /// Updates actual record.
            /// </summary>
            public void Update()
            {
                int Action = (int)DataValidateEnum.vbDataActionUpdate;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionUpdate, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (newRow)
                {
                    dbRow.Table.ColumnChanged -= new DataColumnChangeEventHandler(NewRow_ColumnChanged);
                    dbRow = null;

                    OnEndCurrentEdit();

                    newRow = false;
                    _BOF = false;

                    if (_AbsolutePosition == -1)
                        RP_MoveLast();

                    OnAfterMove();
                }
                dbAdapter.Update(dbDataSet);
            }
            /// <summary>
            /// Event New Row Column Changed change internal status.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void NewRow_ColumnChanged(object sender, DataColumnChangeEventArgs e)
            {
            }
            /// <summary>
            /// Updates Record, UpdateType must be 1, Force is not used by now.
            /// </summary>
            /// <param name="UpdateType">Int UpdateType.</param>
            /// <param name="Force">Bool Force.</param>
            public void Update(int UpdateType, bool Force)
            {
                //For now only this type is supported
                if (UpdateType == 1)
                    Update();
            }
            /// <summary>
            /// Creates the Odbc Data Adapter using the query string and OdbcConnection.
            /// </summary>
            /// <param name="query">String.</param>
            /// <param name="con">OdbcConnection.</param>
            /// <returns></returns>
            private OdbcDataAdapter CreateAdapter(String query, OdbcConnection con)
            {
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, con);
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.MissingMappingAction = MissingMappingAction.Passthrough;
                if (updateCommand == null || deleteCommand == null || insertCommand == null)
                {
                    cmdBuilder = new OdbcCommandBuilder(adapter);
                    try
                    {
                        adapter.UpdateCommand = (updateCommand != null) ? updateCommand : updateCommand = cmdBuilder.GetUpdateCommand();
                        adapter.InsertCommand = (insertCommand != null) ? insertCommand : insertCommand = cmdBuilder.GetInsertCommand();
                        adapter.DeleteCommand = (deleteCommand != null) ? deleteCommand : deleteCommand = cmdBuilder.GetDeleteCommand();
                    }
                    catch { }
                }
                else
                {
                    adapter.UpdateCommand = updateCommand;
                    adapter.InsertCommand = insertCommand;
                    adapter.DeleteCommand = deleteCommand;
                }


                return adapter;
            }
            /// <summary>
            /// Disposes the class and call to close the recordset.
            /// </summary>
            public void Dispose()
            {
                CloseRecordSet();
            }
            /// <summary>
            /// Close the RecordSet.
            /// </summary>
            public void CloseRecordSet()
            {
                try
                {
                    int Action = (int)DataValidateEnum.vbDataActionClose;
                    int Save = 0;
                    OnValidating(ref Action, ref Save);
                    //switch (ValidateNewAction(DataValidateEnum.vbDataActionClose, Action))
                    //{
                    //}

                    if (Save != 0)
                    {
                        Edit();
                        Update();
                    }

                    if (dbTable != null) dbTable.Dispose();
                    if (dbDataSet != null) dbDataSet.Dispose();
                    if (dbAdapter != null) dbAdapter.Dispose();
                    if (dbConnection != null) dbConnection.Dispose();
                    dbConnection = null;
                }
                catch { }
            }
            /// <summary>
            /// Opens the Connection using the connection string.
            /// </summary>
            /// <param name="constr">String.</param>
            /// <returns></returns>
            public bool Connection(String constr)
            {
                dbConnection.ConnectionString = constr;
                dbConnection.Open();
                dbConnection.Close();
                return true;
            }
            /// <summary>
            /// Opens the connection if there is a ConnectionString already assigned.
            /// </summary>
            /// <returns></returns>
            public bool Connection()
            {
                if (dbConnection.ConnectionString != null)
                    return Connection(dbConnection.ConnectionString);
                else
                    throw new ArgumentException("ConnectionString must be set prior method invocation");
            }
            /// <summary>
            /// Opens the Recordset if the internal query string is assigned.
            /// </summary>
            /// <returns></returns>
            private bool OpenRecordSet()
            {
                if (SqlQuery != null)
                    return OpenRecordSet(SqlQuery);
                else
                    throw new ArgumentException("sqlQuery must be set prior method invocation");
            }
            /// <summary>
            /// Opens the RecordSet using the SQLstr parameter.
            /// </summary>
            /// <param name="SQLstr"></param>
            /// <returns></returns>
            private bool OpenRecordSet(String SQLstr)
            {
                SqlQuery = SQLstr;
                operationFinished = false;
                dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                dbTable = dbDataSet.Tables.Add("Table");
                dbAdapter.Fill(dbTable);
                CheckUpdateCommandsFromMetaData();
                operationFinished = true;
                newRow = false;
                foundRecordsCounter = 0;

                SetInitialPosition();
                return true;
            }
            /// <summary>
            /// Returns and instance of OleDbRecordsetClass using 
            /// the actual connection string and SqlQuery.
            /// </summary>
            /// <returns></returns>
            public RecordsetClass Clone()
            {
                return new OleDbRecordsetClass(this.dbConnection.ConnectionString, this.SqlQuery);
            }
            /// <summary>
            /// Finds the First record using the column name and the criteria.
            /// </summary>
            /// <param name="columnName">String column name.</param>
            /// <param name="criteria">Criteria object.</param>
            public void FindFirst(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                int memory;
                found = false;
                _AbsolutePosition = 0;

                while (!found && _AbsolutePosition < dbDataSet.Tables[0].Rows.Count)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition++;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition++;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition++;
                }
                if (found)
                {
                    foundRecordsCounter = 0;
                    memory = _AbsolutePosition;
                    while (!NoMatch)
                    {
                        foundRecordsCounter++;
                        FindNext(columnName, criteria);
                    }
                    _AbsolutePosition = memory;
                    found = true;
                }
                else
                    foundRecordsCounter = 0;

                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Finds last record using the column name and criteria.
            /// </summary>
            /// <param name="columnName">String column name.</param>
            /// <param name="criteria">Criteria object.</param>
            public void FindLast(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                int memory;
                found = false;
                _AbsolutePosition = dbDataSet.Tables[0].Rows.Count - 1;

                while (!found && _AbsolutePosition < -1)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition--;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition--;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition--;
                }
                if (found)
                {
                    foundRecordsCounter = 0;
                    memory = _AbsolutePosition;
                    while (!NoMatch)
                    {
                        foundRecordsCounter++;
                        FindPrevious(columnName, criteria);
                    }
                    _AbsolutePosition = memory;
                    found = true;
                }
                else
                    foundRecordsCounter = 0;

                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Finds previous record using the column name and criteria.
            /// </summary>
            /// <param name="columnName">String column name.</param>
            /// <param name="criteria">Criteria object.</param>
            public void FindPrevious(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                _AbsolutePosition--;
                found = false;

                while (!found && _AbsolutePosition > -1)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition--;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition--;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition--;
                }
                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Finds the next record matching the criteria using the column name.
            /// </summary>
            /// <param name="columnName">String column name.</param>
            /// <param name="criteria">Criteria object.</param>
            public void FindNext(String columnName, Object criteria)
            {
                int Action = (int)DataValidateEnum.vbDataActionFind;
                int Save = 0;
                OnValidating(ref Action, ref Save);
                switch (ValidateNewAction(DataValidateEnum.vbDataActionFind, Action))
                {
                    case DataValidateEnum.vbDataActionCancel:
                        return;
                }

                if (Save != 0)
                {
                    Edit();
                    Update();
                }

                _AbsolutePosition++;
                found = false;

                while (!found && _AbsolutePosition < dbDataSet.Tables[0].Rows.Count)
                {
                    if (criteria == null)
                    {
                        if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                            found = true;
                        else
                            _AbsolutePosition++;
                    }
                    else if (Convert.IsDBNull(dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName]))
                    {
                        _AbsolutePosition++;
                    }
                    else if (dbDataSet.Tables[0].Rows[_AbsolutePosition][columnName] == criteria)
                    {
                        found = true;
                    }
                    else
                        _AbsolutePosition++;
                }
                RP_MoveAtPosition(_AbsolutePosition);
                OnAfterMove();
            }
            /// <summary>
            /// Calls the requery method.
            /// </summary>
            public void Refresh()
            {
                Requery();
            }
            /// <summary>
            /// Calls the Requery with the actual SqlQuery string.
            /// </summary>
            /// <returns></returns>
            public bool Requery()
            {
                return Requery(SqlQuery);
            }
            /// <summary>
            /// RecordSource: Get the SqlQuery string.
            /// </summary>
            public string RecordSource
            {
                get
                {
                    return SqlQuery;
                }
            }
            /// <summary>
            /// Fills the internal adapter with the internal query.
            /// </summary>
            /// <param name="SQLstr"></param>
            /// <returns></returns>
            private bool Requery(String SQLstr)
            {
                SqlQuery = SQLstr;
                operationFinished = false;
                dbAdapter = CreateAdapter(SqlQuery, dbConnection);
                dbTable = dbDataSet.Tables.Add("Table");
                dbAdapter.Fill(dbTable);
                CheckUpdateCommandsFromMetaData();
                operationFinished = true;
                newRow = false;
                foundRecordsCounter = 0;

                SetInitialPosition();
                return true;
            }

            /// <summary>
            /// NoMatch: Get the found state.
            /// </summary>
            public bool NoMatch
            {
                get { return found; }
            }
            /// <summary>
            /// AbsolutePosition: Get/Set AbsolutePosition value.
            /// </summary>
            public int AbsolutePosition
            {
                get { return _AbsolutePosition; }
                set
                {
                    RP_MoveAtPosition(value);
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }

            //FSQ20080207 - Bug 4041
            /// <summary>
            /// BOF: Get BOF state.
            /// </summary>
            private bool _BOF = false;
            public bool BOF
            {
                get { return _BOF; }
            }
            /// <summary>
            /// EOF: Get EOF state.
            /// </summary>
            private bool _EOF = false;
            public bool EOF
            {
                get { return _EOF; }
            }
            /// <summary>
            /// Helps to access the value from a column table as an array.
            /// </summary>
            /// <param name="columnName">String.</param>
            /// <returns></returns>
            public Object this[String columnName]
            {
                get
                {
                    ValidateCurrentPosition();
                    return dbDataSet.Tables[0].Rows[RP][columnName];
                }
                set
                {
                    ValidateCurrentPosition();
                    if (newRow)
                    {
                        if (!dbRow[columnName].Equals(value))
                            dbRow[columnName] = value;
                    }
                    else
                        dbDataSet.Tables[0].Rows[RP][columnName] = value;
                }
            }
            /// <summary>
            /// Helps to access the value as an index column.
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public Object this[int columnIndex]
            {
                get
                {
                    ValidateCurrentPosition();
                    return dbDataSet.Tables[0].Rows[RP][columnIndex];
                }
                set
                {
                    ValidateCurrentPosition();
                    if (newRow)
                    {
                        if (!dbRow[columnIndex].Equals(value))
                            dbRow[columnIndex] = value;
                    }
                    else
                        dbDataSet.Tables[0].Rows[RP][columnIndex] = value;
                }
            }
            /// <summary>
            /// Validates that current postion is not a EOF, BOF and not a new row.
            /// </summary>
            private void ValidateCurrentPosition()
            {
                if ((BOF || EOF) && !newRow)
                    throw new Exception("No current record");
            }
            /// <summary>
            /// RecordsFound: Get the state for found records.
            /// </summary>
            public long RecordsFound
            {
                get { return foundRecordsCounter; }
            }
            /// <summary>
            /// RecourdCount: Get the rows count.
            /// </summary>
            public long RecordCount
            {
                get { return dbDataSet.Tables[0].Rows.Count; }
            }
            /// <summary>
            /// IsLoadingfinnished: Get the operation finished state.
            /// </summary>
            public bool IsLoadingFinnished
            {
                get { return operationFinished; }
            }
            /// <summary>
            /// Fields: Get the FieldsClass internal instance.
            /// </summary>
            public FieldsClass Fields
            {
                get
                {
                    return classFields;
                }
            }
            /// <summary>
            /// SqlQuery: Get/Set the query string and sets the TableName.
            /// </summary>
            public String SqlQuery
            {
                get { return sqlSelectQuery; }
                set
                {
                    sqlSelectQuery = value;
                    TableName = getTableName(sqlSelectQuery);
                }
            }

            /// <summary>
            /// Tries to find the name of the table from the select query.
            /// </summary>
            /// <param name="sqlSelectQuery">The query to parse.</param>
            /// <returns>The name of the table or empty string.</returns>
            private string getTableName(string sqlSelectQuery)
            {
                Match mtch;
                if (!string.IsNullOrEmpty(sqlSelectQuery))
                {
                    if ((mtch = Regex.Match(sqlSelectQuery.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' '), @"^.+[ \t]+FROM[ \t]+([\w.]+)[ \t]*.*$", RegexOptions.IgnoreCase)) != Match.Empty)
                        return mtch.Groups[1].Value.Trim();
                }

                return string.Empty;
            }
            /// <summary>
            /// Bookmark: Get/Set the BookMark.
            /// </summary>
            public DataRow Bookmark
            {
                get
                {
                    return dbDataSet.Tables[0].Rows[RP];
                }
                set
                {
                    int Action = (int)DataValidateEnum.vbDataActionBookmark;
                    int Save = 0;
                    OnValidating(ref Action, ref Save);
                    switch (ValidateNewAction(DataValidateEnum.vbDataActionBookmark, Action))
                    {
                        case DataValidateEnum.vbDataActionCancel:
                            return;
                    }

                    if (Save != 0)
                    {
                        Edit();
                        Update();
                    }

                    RP_MoveAtPosition(dbDataSet.Tables[0].Rows.IndexOf(value));
                    _AbsolutePosition = RP;
                    OnAfterMove();
                }
            }
            /// <summary>
            /// DataSet: Get the internal DataSet.
            /// </summary>
            public DataSet DataSet
            {
                get { return dbDataSet; }
            }

            /// <summary>
            /// Methods to override the commands created by default to update the recordset.
            /// </summary>
            public void SetInsertQuery(String InsertQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.InsertCommand = new OdbcCommand(InsertQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.InsertCommand.Parameters.Add(paramInfo.getOdbcParameter());
                    }
                }
            }
            /// <summary>
            /// Method to set the update query to use, this allows the user to 
            /// override the query that is generated by default.
            /// </summary>
            /// <param name="UpdateQuery">Query to use to update values. It can include parameters.</param>
            /// <param name="parameters">Information of the parameters to set when the Command is created.</param>
            public void SetUpdateQuery(String UpdateQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.UpdateCommand = new OdbcCommand(UpdateQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.UpdateCommand.Parameters.Add(paramInfo.getOdbcParameter());
                    }
                }
            }
            /// <summary>
            /// Method to set the delete query to use, this allows the user to 
            /// override the query that is generated by default.
            /// </summary>
            /// <param name="DeleteQuery">Query to use to delete values. It can include parameters.</param>
            /// <param name="parameters">Information of the parameters to set when the Command is created.</param>
            public void SetDeleteQuery(String DeleteQuery, List<DBParameterInfo> parameters)
            {
                this.dbAdapter.DeleteCommand = new OdbcCommand(DeleteQuery, this.dbConnection);
                if (parameters != null)
                {
                    foreach (DBParameterInfo paramInfo in parameters)
                    {
                        this.dbAdapter.DeleteCommand.Parameters.Add(paramInfo.getOdbcParameter());
                    }
                }
            }

            /// <summary>
            /// Moves to Last Record.
            /// </summary>
            /// <param name="Options">only used when is Type.Missing</param>
            public void MoveLast(object Options)
            {
                //TODO: ToBeImplemented where Options isnot Type.Missing
                if (Options.Equals(Type.Missing))
                    MoveLast();
            }
            /// <summary>
            /// Cancel. Not implemented.
            /// </summary>
            public void Cancel()
            {
                //Nothing to do
            }
            /// <summary>
            /// Close. Not implemented.
            /// </summary>
            public void Close()
            {
                //Nothing to do
            }
            /// <summary>
            /// Name: Get the internal query.
            /// </summary>
            public string Name
            {
                get
                {
                    return this.SqlQuery;
                }
            }
        }


        /// <summary>
        /// Class used for the DAO.Fields handling.
        /// </summary>
        public class FieldsClass
        {
            private RecordsetClass RecordSet = null;
            /// <summary>
            /// internal Field Class
            /// </summary>
            public FieldClass Field = null;

            /// <summary>
            /// Constructor. Initialize Recordset.
            /// </summary>
            public FieldsClass(RecordsetClass recordSet)
            {
                RecordSet = recordSet;
            }


            /// <summary>
            /// Returns the Field Class, accessed by the columnname.
            /// </summary>
            /// <param name="columnName"></param>
            /// <returns></returns>
            public FieldClass this[String columnName]
            {
                get
                {
                    Field = new FieldClass(RecordSet.DataSet.Tables[0].Columns[columnName], RecordSet);
                    return Field;
                }
            }
            /// <summary>
            /// Returns the Field Class, accessed by the column index.
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public FieldClass this[int columnIndex]
            {
                get
                {
                    Field = new FieldClass(RecordSet.DataSet.Tables[0].Columns[columnIndex], RecordSet);
                    return Field;
                }
            }
            /// <summary>
            /// Count: Get the column count.
            /// </summary>
            public int Count
            {
                get { return RecordSet.DataSet.Tables[0].Columns.Count; }
            }


            /// <summary>
            /// Append is not implemented.
            /// </summary>
            /// <param name="Object"></param>
            public void Append(Object Object)
            {
                throw new Exception("This method has not been implemented");
            }
            /// <summary>
            /// Refresh is not implemented.
            /// </summary>
            public void Refresh()
            {
                throw new Exception("This method has not been implemented");
            }
            /// <summary>
            /// Delete is not implemented.
            /// </summary>
            /// <param name="Name"></param>
            public void Delete(String Name)
            {
                throw new Exception("This method has not been implemented");
            }

        }

        /// <summary>
        /// Class used for the DAO.Field handling, is more for mapping purposes.
        /// </summary>
        public class FieldClass
        {
            private DataColumn Field = null;
            private RecordsetClass RecordSet = null;

            ///<sumary>
            ///Constructor, Initialize internal variables.
            ///</sumary>
            public FieldClass(DataColumn field, RecordsetClass recordSet)
            {
                Field = field;
                RecordSet = recordSet;
            }

            /// <summary>
            /// Value: Get/Set RecordSet[Field.Ordinal].
            /// </summary>
            public Object Value
            {
                get { return RecordSet[Field.Ordinal]; }
                set { RecordSet[Field.Ordinal] = value; }
            }
            /// <summary>
            /// AllowZeroLength: Not implemented.
            /// </summary>
            public bool AllowZeroLength
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// Attributes: Not Implemented.
            /// </summary>
            public long Attributes
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// CollatingOrder: Not implemented.
            /// </summary>
            public long CollatingOrder
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// DataUpdatable: Not implemented.
            /// </summary>
            public bool DataUpdatable
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// DefaultValue: Not implemented.
            /// </summary>
            public Object DefaultValue
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// FieldSize: Not implemented.
            /// </summary>
            public long FieldSize
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// ForeignName: Not implemented.
            /// </summary>
            public string ForeignName
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// Name: Not implemented.
            /// </summary>
            public string Name
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// OrdinalPosition: Not implemented.
            /// </summary>
            public int OrdinalPosition
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// OriginalValue: Not implemented.
            /// </summary>
            public Object OriginalValue
            {
                get { throw new Exception("This property has not been implemented"); }
            }

            //TODO: Properties Class
            /*public Properties Properties
            {
                get { throw new Exception("This property has not been implemented"); }
            }*/
            /// <summary>
            /// Required: Not implemented.
            /// </summary>
            public bool Required
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// Size: Not implemented.
            /// </summary>
            public long Size
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// SourceField: Not implemented.
            /// </summary>
            public string SourceField
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// SourceTable: Not implemented.
            /// </summary>
            public string SourceTable
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// Type: Not implemented.
            /// </summary>
            public int Type
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// ValidateOnSet: Not implemented.
            /// </summary>
            public bool ValidateOnSet
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// ValidationRule: Not implemented.
            /// </summary>
            public string ValidationRule
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// ValidationText: Not implemented.
            /// </summary>
            public string ValidationText
            {
                get { throw new Exception("This property has not been implemented"); }
                set { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// VisibleValue: Not implemented.
            /// </summary>
            public Object VisibleValue
            {
                get { throw new Exception("This property has not been implemented"); }
            }
            /// <summary>
            /// AppendChunk is not implemented.
            /// </summary>
            /// <param name="Val"></param>
            public void AppendChunk(Object Val)
            {
                throw new Exception("This method has not been implemented");
            }
            /// <summary>
            /// CreateProperty is not implemented.
            /// </summary>
            public void CreateProperty()
            {
                throw new Exception("This method has not been implemented");
            }
            /// <summary>
            /// GetChunk is not implemented.
            /// </summary>
            /// <param name="Offset"></param>
            /// <param name="Bytes"></param>
            public void GetChunk(long Offset, long Bytes)
            {
                throw new Exception("This method has not been implemented");
            }
        }
    }

}