using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	partial class frmOrderRequest
	{

		#region "Upgrade Support "
		private static frmOrderRequest m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmOrderRequest DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = new frmOrderRequest();
					m_InitializingDefInstance = false;
				}
				return m_vb6FormDefInstance;
			}
			set
			{
				m_vb6FormDefInstance = value;
			}
		}

		#endregion
		#region "Windows Form Designer generated code "
		public frmOrderRequest()
			: base()
		{
			if (m_vb6FormDefInstance == null)
			{
				if (m_InitializingDefInstance)
				{
					m_vb6FormDefInstance = this;
				}
				else
				{
					try
					{
						//For the start-up form, the first instance created is the default instance.
						if (System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType == this.GetType())
						{
							m_vb6FormDefInstance = this;
						}
					}
					catch
					{
					}
				}
			}
			//This call is required by the Windows Form Designer.
			InitializeComponent();
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "txtSubTotal", "txtTotal", "txtTotalTax", "txtFreightCharge", "txtSalesTax", "txtEntry", "fgProducts", "_sbStatusBar_Panel1", "sbStatusBar", "dtRequired", "cmdSave", "cmdClose", "cmdAddProducts", "txtContactLastName", "txtContactName", "cmdCustomers", "txtCompanyName", "_lvCustomers_ColumnHeader_1", "_lvCustomers_ColumnHeader_2", "_lvCustomers_ColumnHeader_3", "_lvCustomers_ColumnHeader_4", "_lvCustomers_ColumnHeader_5", "_lvCustomers_ColumnHeader_6", "_lvCustomers_ColumnHeader_7", "lvCustomers", "Label3", "Label4", "Label2", "Frame1", "txtCustomerContact", "txtCustomerCompany", "Label5", "Label1", "Frame2", "Text3", "dtPromised", "Label13", "Label12", "Label11", "Label10", "Label9", "Label8", "Label7", "Label6", "listViewHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.TextBox txtSubTotal;
		public System.Windows.Forms.TextBox txtTotal;
		public System.Windows.Forms.TextBox txtTotalTax;
		public System.Windows.Forms.TextBox txtFreightCharge;
		public System.Windows.Forms.TextBox txtSalesTax;
		public System.Windows.Forms.TextBox txtEntry;
		public UpgradeHelpers.Windows.Forms.DataGridViewFlex fgProducts;
		private System.Windows.Forms.ToolStripStatusLabel _sbStatusBar_Panel1;
		public System.Windows.Forms.StatusStrip sbStatusBar;
		public System.Windows.Forms.DateTimePicker dtRequired;
		public System.Windows.Forms.Button cmdSave;
		public System.Windows.Forms.Button cmdClose;
		public System.Windows.Forms.Button cmdAddProducts;
		public System.Windows.Forms.TextBox txtContactLastName;
		public System.Windows.Forms.TextBox txtContactName;
		public System.Windows.Forms.Button cmdCustomers;
		public System.Windows.Forms.TextBox txtCompanyName;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_4;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_5;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_6;
		private System.Windows.Forms.ColumnHeader _lvCustomers_ColumnHeader_7;
		public System.Windows.Forms.ListView lvCustomers;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.TextBox txtCustomerContact;
		public System.Windows.Forms.TextBox txtCustomerCompany;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.GroupBox Frame2;
		public System.Windows.Forms.TextBox Text3;
		public System.Windows.Forms.DateTimePicker dtPromised;
		public System.Windows.Forms.Label Label13;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.Label Label6;
		private UpgradeHelpers.VB6.Gui.ListViewHelper listViewHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOrderRequest));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.txtSubTotal = new System.Windows.Forms.TextBox();
			this.txtTotal = new System.Windows.Forms.TextBox();
			this.txtTotalTax = new System.Windows.Forms.TextBox();
			this.txtFreightCharge = new System.Windows.Forms.TextBox();
			this.txtSalesTax = new System.Windows.Forms.TextBox();
			this.txtEntry = new System.Windows.Forms.TextBox();
			this.fgProducts = new UpgradeHelpers.Windows.Forms.DataGridViewFlex(this.components);
			this.sbStatusBar = new System.Windows.Forms.StatusStrip();
			this._sbStatusBar_Panel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.dtRequired = new System.Windows.Forms.DateTimePicker();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.cmdAddProducts = new System.Windows.Forms.Button();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.txtContactLastName = new System.Windows.Forms.TextBox();
			this.txtContactName = new System.Windows.Forms.TextBox();
			this.cmdCustomers = new System.Windows.Forms.Button();
			this.txtCompanyName = new System.Windows.Forms.TextBox();
			this.lvCustomers = new System.Windows.Forms.ListView();
			this._lvCustomers_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_5 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_6 = new System.Windows.Forms.ColumnHeader();
			this._lvCustomers_ColumnHeader_7 = new System.Windows.Forms.ColumnHeader();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Frame2 = new System.Windows.Forms.GroupBox();
			this.txtCustomerContact = new System.Windows.Forms.TextBox();
			this.txtCustomerCompany = new System.Windows.Forms.TextBox();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.Text3 = new System.Windows.Forms.TextBox();
			this.dtPromised = new System.Windows.Forms.DateTimePicker();
			this.Label13 = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.Label11 = new System.Windows.Forms.Label();
			this.Label10 = new System.Windows.Forms.Label();
			this.Label9 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label7 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.sbStatusBar.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.lvCustomers.SuspendLayout();
			this.Frame2.SuspendLayout();
			this.SuspendLayout();
			this.listViewHelper1 = new UpgradeHelpers.VB6.Gui.ListViewHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).BeginInit();
			// 
			// txtSubTotal
			// 
			this.txtSubTotal.AcceptsReturn = true;
			this.txtSubTotal.BackColor = System.Drawing.SystemColors.Menu;
			this.txtSubTotal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtSubTotal.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtSubTotal.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtSubTotal.Location = new System.Drawing.Point(360, 520);
			this.txtSubTotal.MaxLength = 0;
			this.txtSubTotal.Name = "txtSubTotal";
			this.txtSubTotal.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtSubTotal.Size = new System.Drawing.Size(145, 20);
			this.txtSubTotal.TabIndex = 33;
			this.txtSubTotal.TabStop = false;
			this.txtSubTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtTotal
			// 
			this.txtTotal.AcceptsReturn = true;
			this.txtTotal.BackColor = System.Drawing.SystemColors.Menu;
			this.txtTotal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtTotal.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtTotal.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtTotal.Location = new System.Drawing.Point(96, 544);
			this.txtTotal.MaxLength = 0;
			this.txtTotal.Name = "txtTotal";
			this.txtTotal.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtTotal.Size = new System.Drawing.Size(145, 20);
			this.txtTotal.TabIndex = 31;
			this.txtTotal.TabStop = false;
			this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtTotalTax
			// 
			this.txtTotalTax.AcceptsReturn = true;
			this.txtTotalTax.BackColor = System.Drawing.SystemColors.Menu;
			this.txtTotalTax.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtTotalTax.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtTotalTax.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtTotalTax.Location = new System.Drawing.Point(360, 496);
			this.txtTotalTax.MaxLength = 0;
			this.txtTotalTax.Name = "txtTotalTax";
			this.txtTotalTax.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtTotalTax.Size = new System.Drawing.Size(145, 20);
			this.txtTotalTax.TabIndex = 29;
			this.txtTotalTax.TabStop = false;
			this.txtTotalTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtFreightCharge
			// 
			this.txtFreightCharge.AcceptsReturn = true;
			this.txtFreightCharge.BackColor = System.Drawing.SystemColors.Window;
			this.txtFreightCharge.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtFreightCharge.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtFreightCharge.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtFreightCharge.Location = new System.Drawing.Point(96, 520);
			this.txtFreightCharge.MaxLength = 0;
			this.txtFreightCharge.Name = "txtFreightCharge";
			this.txtFreightCharge.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtFreightCharge.Size = new System.Drawing.Size(145, 20);
			this.txtFreightCharge.TabIndex = 8;
			this.txtFreightCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtFreightCharge.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFreightCharge_KeyPress);
			this.txtFreightCharge.TextChanged += new System.EventHandler(this.txtFreightCharge_TextChanged);
			// 
			// txtSalesTax
			// 
			this.txtSalesTax.AcceptsReturn = true;
			this.txtSalesTax.BackColor = System.Drawing.SystemColors.Window;
			this.txtSalesTax.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtSalesTax.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtSalesTax.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtSalesTax.Location = new System.Drawing.Point(96, 496);
			this.txtSalesTax.MaxLength = 0;
			this.txtSalesTax.Name = "txtSalesTax";
			this.txtSalesTax.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtSalesTax.Size = new System.Drawing.Size(145, 20);
			this.txtSalesTax.TabIndex = 7;
			this.txtSalesTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtSalesTax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSalesTax_KeyPress);
			this.txtSalesTax.TextChanged += new System.EventHandler(this.txtSalesTax_TextChanged);
			// 
			// txtEntry
			// 
			this.txtEntry.AcceptsReturn = true;
			this.txtEntry.BackColor = System.Drawing.SystemColors.Window;
			this.txtEntry.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtEntry.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtEntry.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtEntry.Location = new System.Drawing.Point(96, 472);
			this.txtEntry.MaxLength = 0;
			this.txtEntry.Name = "txtEntry";
			this.txtEntry.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtEntry.Size = new System.Drawing.Size(145, 19);
			this.txtEntry.TabIndex = 26;
			this.txtEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEntry_KeyDown);
			this.txtEntry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEntry_KeyPress);
			this.txtEntry.Leave += new System.EventHandler(this.txtEntry_Leave);
			// 
			// fgProducts
			// 
			this.fgProducts.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.fgProducts.ColumnsCount = 0;
			this.fgProducts.FixedColumns = 0;
			this.fgProducts.FixedRows = 0;
			this.fgProducts.Location = new System.Drawing.Point(8, 288);
			this.fgProducts.Name = "fgProducts";
			this.fgProducts.Size = new System.Drawing.Size(505, 177);
			this.fgProducts.TabIndex = 6;
			this.fgProducts.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgProducts_CellLeave);
			this.fgProducts.Click += new System.EventHandler(this.fgProducts_Click);
			this.fgProducts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fgProducts_KeyPress);
			// 
			// sbStatusBar
			// 
			this.sbStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sbStatusBar.Location = new System.Drawing.Point(0, 630);
			this.sbStatusBar.Name = "sbStatusBar";
			this.sbStatusBar.ShowItemToolTips = true;
			this.sbStatusBar.Size = new System.Drawing.Size(535, 25);
			this.sbStatusBar.TabIndex = 25;
			this.sbStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[]{this._sbStatusBar_Panel1});
			// 
			// _sbStatusBar_Panel1
			// 
			this._sbStatusBar_Panel1.AutoSize = true;
			this._sbStatusBar_Panel1.AutoSize = false;
			this._sbStatusBar_Panel1.BorderSides = (System.Windows.Forms.ToolStripStatusLabelBorderSides) (System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom);
			this._sbStatusBar_Panel1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this._sbStatusBar_Panel1.DoubleClickEnabled = true;
			this._sbStatusBar_Panel1.Margin = new System.Windows.Forms.Padding(0);
			this._sbStatusBar_Panel1.Size = new System.Drawing.Size(517, 25);
			this._sbStatusBar_Panel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._sbStatusBar_Panel1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			// 
			// dtRequired
			// 
			this.dtRequired.Checked = false;
			this.dtRequired.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtRequired.Location = new System.Drawing.Point(120, 248);
			this.dtRequired.Name = "dtRequired";
			this.dtRequired.Size = new System.Drawing.Size(97, 20);
			this.dtRequired.TabIndex = 4;
			this.dtRequired.ValueChanged += new System.EventHandler(this.dtRequired_ValueChanged);
			// 
			// cmdSave
			// 
			this.cmdSave.BackColor = System.Drawing.SystemColors.Control;
			this.cmdSave.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdSave.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdSave.Location = new System.Drawing.Point(328, 568);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdSave.Size = new System.Drawing.Size(89, 25);
			this.cmdSave.TabIndex = 9;
			this.cmdSave.Text = "&Save";
			this.cmdSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdSave.UseVisualStyleBackColor = false;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.BackColor = System.Drawing.SystemColors.Control;
			this.cmdClose.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdClose.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdClose.Location = new System.Drawing.Point(432, 568);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdClose.Size = new System.Drawing.Size(89, 25);
			this.cmdClose.TabIndex = 10;
			this.cmdClose.Text = "&Close";
			this.cmdClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdClose.UseVisualStyleBackColor = false;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// cmdAddProducts
			// 
			this.cmdAddProducts.BackColor = System.Drawing.SystemColors.Control;
			this.cmdAddProducts.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdAddProducts.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdAddProducts.Location = new System.Drawing.Point(488, 264);
			this.cmdAddProducts.Name = "cmdAddProducts";
			this.cmdAddProducts.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdAddProducts.Size = new System.Drawing.Size(25, 21);
			this.cmdAddProducts.TabIndex = 22;
			this.cmdAddProducts.TabStop = false;
			this.cmdAddProducts.Text = "...";
			this.cmdAddProducts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdAddProducts.UseVisualStyleBackColor = false;
			this.cmdAddProducts.Click += new System.EventHandler(this.cmdAddProducts_Click);
			// 
			// Frame1
			// 
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.txtContactLastName);
			this.Frame1.Controls.Add(this.txtContactName);
			this.Frame1.Controls.Add(this.cmdCustomers);
			this.Frame1.Controls.Add(this.txtCompanyName);
			this.Frame1.Controls.Add(this.lvCustomers);
			this.Frame1.Controls.Add(this.Label3);
			this.Frame1.Controls.Add(this.Label4);
			this.Frame1.Controls.Add(this.Label2);
			this.Frame1.Enabled = true;
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(8, 8);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(505, 169);
			this.Frame1.TabIndex = 13;
			this.Frame1.Text = "Search customer";
			this.Frame1.Visible = true;
			// 
			// txtContactLastName
			// 
			this.txtContactLastName.AcceptsReturn = true;
			this.txtContactLastName.BackColor = System.Drawing.SystemColors.Window;
			this.txtContactLastName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtContactLastName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtContactLastName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtContactLastName.Location = new System.Drawing.Point(336, 48);
			this.txtContactLastName.MaxLength = 0;
			this.txtContactLastName.Name = "txtContactLastName";
			this.txtContactLastName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtContactLastName.Size = new System.Drawing.Size(145, 20);
			this.txtContactLastName.TabIndex = 2;
			this.txtContactLastName.TextChanged += new System.EventHandler(this.txtContactLastName_TextChanged);
			// 
			// txtContactName
			// 
			this.txtContactName.AcceptsReturn = true;
			this.txtContactName.BackColor = System.Drawing.SystemColors.Window;
			this.txtContactName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtContactName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtContactName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtContactName.Location = new System.Drawing.Point(88, 48);
			this.txtContactName.MaxLength = 0;
			this.txtContactName.Name = "txtContactName";
			this.txtContactName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtContactName.Size = new System.Drawing.Size(145, 20);
			this.txtContactName.TabIndex = 1;
			this.txtContactName.TextChanged += new System.EventHandler(this.txtContactName_TextChanged);
			// 
			// cmdCustomers
			// 
			this.cmdCustomers.BackColor = System.Drawing.SystemColors.Control;
			this.cmdCustomers.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdCustomers.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdCustomers.Location = new System.Drawing.Point(456, 16);
			this.cmdCustomers.Name = "cmdCustomers";
			this.cmdCustomers.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdCustomers.Size = new System.Drawing.Size(25, 21);
			this.cmdCustomers.TabIndex = 14;
			this.cmdCustomers.TabStop = false;
			this.cmdCustomers.Text = "...";
			this.cmdCustomers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdCustomers.UseVisualStyleBackColor = false;
			this.cmdCustomers.Click += new System.EventHandler(this.cmdCustomers_Click);
			// 
			// txtCompanyName
			// 
			this.txtCompanyName.AcceptsReturn = true;
			this.txtCompanyName.BackColor = System.Drawing.SystemColors.Window;
			this.txtCompanyName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtCompanyName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtCompanyName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtCompanyName.Location = new System.Drawing.Point(88, 16);
			this.txtCompanyName.MaxLength = 0;
			this.txtCompanyName.Name = "txtCompanyName";
			this.txtCompanyName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtCompanyName.Size = new System.Drawing.Size(145, 20);
			this.txtCompanyName.TabIndex = 0;
			this.txtCompanyName.TextChanged += new System.EventHandler(this.txtCompanyName_TextChanged);
			// 
			// lvCustomers
			// 
			this.lvCustomers.BackColor = System.Drawing.SystemColors.Window;
			this.lvCustomers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvCustomers.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvCustomers.FullRowSelect = true;
			this.lvCustomers.GridLines = true;
			this.lvCustomers.HideSelection = false;
			this.lvCustomers.HotTracking = true;
			this.lvCustomers.LabelEdit = false;
			this.lvCustomers.Location = new System.Drawing.Point(8, 80);
			this.lvCustomers.Name = "lvCustomers";
			this.lvCustomers.Size = new System.Drawing.Size(489, 81);
			this.lvCustomers.TabIndex = 3;
			this.lvCustomers.View = System.Windows.Forms.View.Details;
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_1);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_2);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_3);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_4);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_5);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_6);
			this.lvCustomers.Columns.Add(this._lvCustomers_ColumnHeader_7);
			// 
			// _lvCustomers_ColumnHeader_1
			// 
			this._lvCustomers_ColumnHeader_1.Text = "Customer ID";
			this._lvCustomers_ColumnHeader_1.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_2
			// 
			this._lvCustomers_ColumnHeader_2.Text = "Company Name";
			this._lvCustomers_ColumnHeader_2.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_3
			// 
			this._lvCustomers_ColumnHeader_3.Text = "Contact Name";
			this._lvCustomers_ColumnHeader_3.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_4
			// 
			this._lvCustomers_ColumnHeader_4.Text = "Contact Last Name";
			this._lvCustomers_ColumnHeader_4.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_5
			// 
			this._lvCustomers_ColumnHeader_5.Text = "City";
			this._lvCustomers_ColumnHeader_5.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_6
			// 
			this._lvCustomers_ColumnHeader_6.Text = "State";
			this._lvCustomers_ColumnHeader_6.Width = 97;
			// 
			// _lvCustomers_ColumnHeader_7
			// 
			this._lvCustomers_ColumnHeader_7.Text = "Country";
			this._lvCustomers_ColumnHeader_7.Width = 97;
			// 
			// Label3
			// 
			this.Label3.BackColor = System.Drawing.SystemColors.Control;
			this.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label3.Location = new System.Drawing.Point(240, 48);
			this.Label3.Name = "Label3";
			this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label3.Size = new System.Drawing.Size(97, 17);
			this.Label3.TabIndex = 17;
			this.Label3.Text = "Contact last name:";
			// 
			// Label4
			// 
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(8, 16);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(89, 17);
			this.Label4.TabIndex = 16;
			this.Label4.Text = "Company name:";
			// 
			// Label2
			// 
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(8, 48);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(89, 17);
			this.Label2.TabIndex = 15;
			this.Label2.Text = "Contact name:";
			// 
			// Frame2
			// 
			this.Frame2.BackColor = System.Drawing.SystemColors.Control;
			this.Frame2.Controls.Add(this.txtCustomerContact);
			this.Frame2.Controls.Add(this.txtCustomerCompany);
			this.Frame2.Controls.Add(this.Label5);
			this.Frame2.Controls.Add(this.Label1);
			this.Frame2.Enabled = true;
			this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame2.Location = new System.Drawing.Point(8, 184);
			this.Frame2.Name = "Frame2";
			this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame2.Size = new System.Drawing.Size(505, 49);
			this.Frame2.TabIndex = 12;
			this.Frame2.Text = "Customer";
			this.Frame2.Visible = true;
			// 
			// txtCustomerContact
			// 
			this.txtCustomerContact.AcceptsReturn = true;
			this.txtCustomerContact.BackColor = System.Drawing.SystemColors.Menu;
			this.txtCustomerContact.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtCustomerContact.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtCustomerContact.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtCustomerContact.Location = new System.Drawing.Point(288, 16);
			this.txtCustomerContact.MaxLength = 0;
			this.txtCustomerContact.Name = "txtCustomerContact";
			this.txtCustomerContact.ReadOnly = true;
			this.txtCustomerContact.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtCustomerContact.Size = new System.Drawing.Size(209, 20);
			this.txtCustomerContact.TabIndex = 21;
			this.txtCustomerContact.TabStop = false;
			// 
			// txtCustomerCompany
			// 
			this.txtCustomerCompany.AcceptsReturn = true;
			this.txtCustomerCompany.BackColor = System.Drawing.SystemColors.Menu;
			this.txtCustomerCompany.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtCustomerCompany.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtCustomerCompany.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtCustomerCompany.Location = new System.Drawing.Point(72, 16);
			this.txtCustomerCompany.MaxLength = 0;
			this.txtCustomerCompany.Name = "txtCustomerCompany";
			this.txtCustomerCompany.ReadOnly = true;
			this.txtCustomerCompany.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtCustomerCompany.Size = new System.Drawing.Size(145, 20);
			this.txtCustomerCompany.TabIndex = 20;
			this.txtCustomerCompany.TabStop = false;
			// 
			// Label5
			// 
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(8, 16);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(57, 17);
			this.Label5.TabIndex = 19;
			this.Label5.Text = "Company:";
			// 
			// Label1
			// 
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(232, 16);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(57, 17);
			this.Label1.TabIndex = 18;
			this.Label1.Text = "Contact:";
			// 
			// Text3
			// 
			this.Text3.AcceptsReturn = true;
			this.Text3.BackColor = System.Drawing.SystemColors.Window;
			this.Text3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Text3.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.Text3.ForeColor = System.Drawing.SystemColors.WindowText;
			this.Text3.Location = new System.Drawing.Point(120, 152);
			this.Text3.MaxLength = 0;
			this.Text3.Name = "Text3";
			this.Text3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text3.Size = new System.Drawing.Size(145, 20);
			this.Text3.TabIndex = 11;
			// 
			// dtPromised
			// 
			this.dtPromised.Checked = false;
			this.dtPromised.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtPromised.Location = new System.Drawing.Point(352, 248);
			this.dtPromised.Name = "dtPromised";
			this.dtPromised.Size = new System.Drawing.Size(97, 20);
			this.dtPromised.TabIndex = 5;
			this.dtPromised.ValueChanged += new System.EventHandler(this.dtPromised_ValueChanged);
			// 
			// Label13
			// 
			this.Label13.BackColor = System.Drawing.SystemColors.Control;
			this.Label13.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label13.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label13.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label13.Location = new System.Drawing.Point(16, 472);
			this.Label13.Name = "Label13";
			this.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label13.Size = new System.Drawing.Size(81, 17);
			this.Label13.TabIndex = 35;
			this.Label13.Text = "Line quantity:";
			// 
			// Label12
			// 
			this.Label12.BackColor = System.Drawing.SystemColors.Control;
			this.Label12.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label12.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label12.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label12.Location = new System.Drawing.Point(16, 520);
			this.Label12.Name = "Label12";
			this.Label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label12.Size = new System.Drawing.Size(89, 17);
			this.Label12.TabIndex = 34;
			this.Label12.Text = "Freight Charge:";
			// 
			// Label11
			// 
			this.Label11.BackColor = System.Drawing.SystemColors.Control;
			this.Label11.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label11.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label11.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label11.Location = new System.Drawing.Point(16, 544);
			this.Label11.Name = "Label11";
			this.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label11.Size = new System.Drawing.Size(89, 17);
			this.Label11.TabIndex = 32;
			this.Label11.Text = "Total:";
			// 
			// Label10
			// 
			this.Label10.BackColor = System.Drawing.SystemColors.Control;
			this.Label10.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label10.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label10.Location = new System.Drawing.Point(280, 496);
			this.Label10.Name = "Label10";
			this.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label10.Size = new System.Drawing.Size(89, 17);
			this.Label10.TabIndex = 30;
			this.Label10.Text = "Total Tax:";
			// 
			// Label9
			// 
			this.Label9.BackColor = System.Drawing.SystemColors.Control;
			this.Label9.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label9.Location = new System.Drawing.Point(280, 520);
			this.Label9.Name = "Label9";
			this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label9.Size = new System.Drawing.Size(89, 17);
			this.Label9.TabIndex = 28;
			this.Label9.Text = "Sub Total:";
			// 
			// Label8
			// 
			this.Label8.BackColor = System.Drawing.SystemColors.Control;
			this.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label8.Location = new System.Drawing.Point(16, 496);
			this.Label8.Name = "Label8";
			this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label8.Size = new System.Drawing.Size(89, 17);
			this.Label8.TabIndex = 27;
			this.Label8.Text = "Sales Tax:";
			// 
			// Label7
			// 
			this.Label7.BackColor = System.Drawing.SystemColors.Control;
			this.Label7.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label7.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label7.Location = new System.Drawing.Point(256, 248);
			this.Label7.Name = "Label7";
			this.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label7.Size = new System.Drawing.Size(105, 17);
			this.Label7.TabIndex = 24;
			this.Label7.Text = "Promised by date:";
			// 
			// Label6
			// 
			this.Label6.BackColor = System.Drawing.SystemColors.Control;
			this.Label6.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label6.Location = new System.Drawing.Point(8, 248);
			this.Label6.Name = "Label6";
			this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label6.Size = new System.Drawing.Size(105, 17);
			this.Label6.TabIndex = 23;
			this.Label6.Text = "Required by date:";
			// 
			// frmOrderRequest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(535, 655);
			this.Controls.Add(this.txtSubTotal);
			this.Controls.Add(this.txtTotal);
			this.Controls.Add(this.txtTotalTax);
			this.Controls.Add(this.txtFreightCharge);
			this.Controls.Add(this.txtSalesTax);
			this.Controls.Add(this.txtEntry);
			this.Controls.Add(this.fgProducts);
			this.Controls.Add(this.sbStatusBar);
			this.Controls.Add(this.dtRequired);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdAddProducts);
			this.Controls.Add(this.Frame1);
			this.Controls.Add(this.Frame2);
			this.Controls.Add(this.Text3);
			this.Controls.Add(this.dtPromised);
			this.Controls.Add(this.Label13);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.Label11);
			this.Controls.Add(this.Label10);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.Label7);
			this.Controls.Add(this.Label6);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Location = new System.Drawing.Point(8, 30);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmOrderRequest";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Create Order";
			this.Closed += new System.EventHandler(this.frmOrderRequest_Closed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOrderRequest_FormClosing);
			this.Load += new System.EventHandler(this.frmOrderRequest_Load);
			this.listViewHelper1.SetItemClickMethod(this.lvCustomers, "lvCustomers_ItemClick");
			this.listViewHelper1.SetCorrectEventsBehavior(this.lvCustomers, true);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).EndInit();
			this.sbStatusBar.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.lvCustomers.ResumeLayout(false);
			this.Frame2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}