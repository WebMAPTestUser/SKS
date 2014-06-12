using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	partial class frmOrderReception
	{

		#region "Upgrade Support "
		private static frmOrderReception m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmOrderReception DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = new frmOrderReception();
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
		public frmOrderReception()
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
		private string[] visualControls = new string[]{"components", "ToolTipMain", "txtNotes", "txtSubTotal", "txtTotal", "txtTotalTax", "txtFreightCharge", "txtSalesTax", "txtEntry", "fgProducts", "_sbStatusBar_Panel1", "sbStatusBar", "cmdSave", "cmdClose", "cmdAddProducts", "txtProviderName", "txtContactLastName", "txtContactName", "cmdProviders", "_lvProviders_ColumnHeader_1", "_lvProviders_ColumnHeader_2", "_lvProviders_ColumnHeader_3", "_lvProviders_ColumnHeader_4", "_lvProviders_ColumnHeader_5", "_lvProviders_ColumnHeader_6", "_lvProviders_ColumnHeader_7", "lvProviders", "Label3", "Label4", "Label2", "Frame1", "txtProviderContact", "txtProviderCompany", "Label5", "Label1", "Frame2", "Text3", "Label12", "Label11", "Label10", "Label9", "Label8", "Label6", "listViewHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.TextBox txtNotes;
		public System.Windows.Forms.TextBox txtSubTotal;
		public System.Windows.Forms.TextBox txtTotal;
		public System.Windows.Forms.TextBox txtTotalTax;
		public System.Windows.Forms.TextBox txtFreightCharge;
		public System.Windows.Forms.TextBox txtSalesTax;
		public System.Windows.Forms.TextBox txtEntry;
		public UpgradeHelpers.Windows.Forms.DataGridViewFlex fgProducts;
		private System.Windows.Forms.ToolStripStatusLabel _sbStatusBar_Panel1;
		public System.Windows.Forms.StatusStrip sbStatusBar;
		public System.Windows.Forms.Button cmdSave;
		public System.Windows.Forms.Button cmdClose;
		public System.Windows.Forms.Button cmdAddProducts;
		public System.Windows.Forms.TextBox txtProviderName;
		public System.Windows.Forms.TextBox txtContactLastName;
		public System.Windows.Forms.TextBox txtContactName;
		public System.Windows.Forms.Button cmdProviders;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_4;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_5;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_6;
		private System.Windows.Forms.ColumnHeader _lvProviders_ColumnHeader_7;
		public System.Windows.Forms.ListView lvProviders;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.TextBox txtProviderContact;
		public System.Windows.Forms.TextBox txtProviderCompany;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.GroupBox Frame2;
		public System.Windows.Forms.TextBox Text3;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label6;
		private UpgradeHelpers.VB6.Gui.ListViewHelper listViewHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOrderReception));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.txtNotes = new System.Windows.Forms.TextBox();
			this.txtSubTotal = new System.Windows.Forms.TextBox();
			this.txtTotal = new System.Windows.Forms.TextBox();
			this.txtTotalTax = new System.Windows.Forms.TextBox();
			this.txtFreightCharge = new System.Windows.Forms.TextBox();
			this.txtSalesTax = new System.Windows.Forms.TextBox();
			this.txtEntry = new System.Windows.Forms.TextBox();
			this.fgProducts = new UpgradeHelpers.Windows.Forms.DataGridViewFlex(this.components);
			this.sbStatusBar = new System.Windows.Forms.StatusStrip();
			this._sbStatusBar_Panel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.cmdSave = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.cmdAddProducts = new System.Windows.Forms.Button();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.txtProviderName = new System.Windows.Forms.TextBox();
			this.txtContactLastName = new System.Windows.Forms.TextBox();
			this.txtContactName = new System.Windows.Forms.TextBox();
			this.cmdProviders = new System.Windows.Forms.Button();
			this.lvProviders = new System.Windows.Forms.ListView();
			this._lvProviders_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_5 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_6 = new System.Windows.Forms.ColumnHeader();
			this._lvProviders_ColumnHeader_7 = new System.Windows.Forms.ColumnHeader();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Frame2 = new System.Windows.Forms.GroupBox();
			this.txtProviderContact = new System.Windows.Forms.TextBox();
			this.txtProviderCompany = new System.Windows.Forms.TextBox();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.Text3 = new System.Windows.Forms.TextBox();
			this.Label12 = new System.Windows.Forms.Label();
			this.Label11 = new System.Windows.Forms.Label();
			this.Label10 = new System.Windows.Forms.Label();
			this.Label9 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.sbStatusBar.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.lvProviders.SuspendLayout();
			this.Frame2.SuspendLayout();
			this.SuspendLayout();
			this.listViewHelper1 = new UpgradeHelpers.VB6.Gui.ListViewHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).BeginInit();
			// 
			// txtNotes
			// 
			this.txtNotes.AcceptsReturn = true;
			this.txtNotes.BackColor = System.Drawing.SystemColors.Window;
			this.txtNotes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtNotes.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtNotes.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtNotes.Location = new System.Drawing.Point(56, 240);
			this.txtNotes.MaxLength = 0;
			this.txtNotes.Multiline = true;
			this.txtNotes.Name = "txtNotes";
			this.txtNotes.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtNotes.Size = new System.Drawing.Size(425, 44);
			this.txtNotes.TabIndex = 4;
			this.txtNotes.TextChanged += new System.EventHandler(this.txtNotes_TextChanged);
			// 
			// txtSubTotal
			// 
			this.txtSubTotal.AcceptsReturn = true;
			this.txtSubTotal.BackColor = System.Drawing.SystemColors.Menu;
			this.txtSubTotal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtSubTotal.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtSubTotal.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtSubTotal.Location = new System.Drawing.Point(352, 504);
			this.txtSubTotal.MaxLength = 0;
			this.txtSubTotal.Name = "txtSubTotal";
			this.txtSubTotal.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtSubTotal.Size = new System.Drawing.Size(145, 20);
			this.txtSubTotal.TabIndex = 31;
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
			this.txtTotal.Location = new System.Drawing.Point(88, 528);
			this.txtTotal.MaxLength = 0;
			this.txtTotal.Name = "txtTotal";
			this.txtTotal.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtTotal.Size = new System.Drawing.Size(145, 20);
			this.txtTotal.TabIndex = 29;
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
			this.txtTotalTax.Location = new System.Drawing.Point(352, 480);
			this.txtTotalTax.MaxLength = 0;
			this.txtTotalTax.Name = "txtTotalTax";
			this.txtTotalTax.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtTotalTax.Size = new System.Drawing.Size(145, 20);
			this.txtTotalTax.TabIndex = 27;
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
			this.txtFreightCharge.Location = new System.Drawing.Point(88, 504);
			this.txtFreightCharge.MaxLength = 0;
			this.txtFreightCharge.Name = "txtFreightCharge";
			this.txtFreightCharge.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtFreightCharge.Size = new System.Drawing.Size(145, 20);
			this.txtFreightCharge.TabIndex = 7;
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
			this.txtSalesTax.Location = new System.Drawing.Point(88, 480);
			this.txtSalesTax.MaxLength = 0;
			this.txtSalesTax.Name = "txtSalesTax";
			this.txtSalesTax.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtSalesTax.Size = new System.Drawing.Size(145, 20);
			this.txtSalesTax.TabIndex = 6;
			this.txtSalesTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtSalesTax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSalesTax_KeyPress);
			this.txtSalesTax.TextChanged += new System.EventHandler(this.txtSalesTax_TextChanged);
			// 
			// txtEntry
			// 
			this.txtEntry.AcceptsReturn = true;
			this.txtEntry.BackColor = System.Drawing.SystemColors.Window;
			this.txtEntry.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtEntry.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtEntry.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtEntry.Location = new System.Drawing.Point(416, 296);
			this.txtEntry.MaxLength = 0;
			this.txtEntry.Name = "txtEntry";
			this.txtEntry.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtEntry.Size = new System.Drawing.Size(73, 19);
			this.txtEntry.TabIndex = 24;
			this.txtEntry.Visible = false;
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
			this.fgProducts.TabIndex = 5;
			this.fgProducts.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.fgProducts_CellLeave);
			this.fgProducts.Click += new System.EventHandler(this.fgProducts_Click);
			this.fgProducts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fgProducts_KeyPress);
			// 
			// sbStatusBar
			// 
			this.sbStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sbStatusBar.Location = new System.Drawing.Point(0, 584);
			this.sbStatusBar.Name = "sbStatusBar";
			this.sbStatusBar.ShowItemToolTips = true;
			this.sbStatusBar.Size = new System.Drawing.Size(523, 25);
			this.sbStatusBar.TabIndex = 23;
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
			this._sbStatusBar_Panel1.Size = new System.Drawing.Size(505, 25);
			this._sbStatusBar_Panel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._sbStatusBar_Panel1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			// 
			// cmdSave
			// 
			this.cmdSave.BackColor = System.Drawing.SystemColors.Control;
			this.cmdSave.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdSave.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdSave.Location = new System.Drawing.Point(320, 552);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdSave.Size = new System.Drawing.Size(89, 25);
			this.cmdSave.TabIndex = 8;
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
			this.cmdClose.Location = new System.Drawing.Point(424, 552);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdClose.Size = new System.Drawing.Size(89, 25);
			this.cmdClose.TabIndex = 9;
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
			this.cmdAddProducts.TabIndex = 21;
			this.cmdAddProducts.TabStop = false;
			this.cmdAddProducts.Text = "...";
			this.cmdAddProducts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdAddProducts.UseVisualStyleBackColor = false;
			this.cmdAddProducts.Click += new System.EventHandler(this.cmdAddProducts_Click);
			// 
			// Frame1
			// 
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.txtProviderName);
			this.Frame1.Controls.Add(this.txtContactLastName);
			this.Frame1.Controls.Add(this.txtContactName);
			this.Frame1.Controls.Add(this.cmdProviders);
			this.Frame1.Controls.Add(this.lvProviders);
			this.Frame1.Controls.Add(this.Label3);
			this.Frame1.Controls.Add(this.Label4);
			this.Frame1.Controls.Add(this.Label2);
			this.Frame1.Enabled = true;
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(8, 8);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(505, 169);
			this.Frame1.TabIndex = 12;
			this.Frame1.Text = "Search supplier";
			this.Frame1.Visible = true;
			// 
			// txtProviderName
			// 
			this.txtProviderName.AcceptsReturn = true;
			this.txtProviderName.BackColor = System.Drawing.SystemColors.Window;
			this.txtProviderName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtProviderName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtProviderName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtProviderName.Location = new System.Drawing.Point(88, 16);
			this.txtProviderName.MaxLength = 0;
			this.txtProviderName.Name = "txtProviderName";
			this.txtProviderName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtProviderName.Size = new System.Drawing.Size(145, 20);
			this.txtProviderName.TabIndex = 0;
			this.txtProviderName.TextChanged += new System.EventHandler(this.txtProviderName_TextChanged);
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
			// cmdProviders
			// 
			this.cmdProviders.BackColor = System.Drawing.SystemColors.Control;
			this.cmdProviders.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdProviders.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdProviders.Location = new System.Drawing.Point(456, 16);
			this.cmdProviders.Name = "cmdProviders";
			this.cmdProviders.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdProviders.Size = new System.Drawing.Size(25, 21);
			this.cmdProviders.TabIndex = 13;
			this.cmdProviders.TabStop = false;
			this.cmdProviders.Text = "...";
			this.cmdProviders.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdProviders.UseVisualStyleBackColor = false;
			this.cmdProviders.Click += new System.EventHandler(this.cmdProviders_Click);
			// 
			// lvProviders
			// 
			this.lvProviders.BackColor = System.Drawing.SystemColors.Window;
			this.lvProviders.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvProviders.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvProviders.FullRowSelect = true;
			this.lvProviders.GridLines = true;
			this.lvProviders.HideSelection = false;
			this.lvProviders.HotTracking = true;
			this.lvProviders.LabelEdit = false;
			this.lvProviders.Location = new System.Drawing.Point(8, 80);
			this.lvProviders.Name = "lvProviders";
			this.lvProviders.Size = new System.Drawing.Size(489, 81);
			this.lvProviders.TabIndex = 3;
			this.lvProviders.View = System.Windows.Forms.View.Details;
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_1);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_2);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_3);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_4);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_5);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_6);
			this.lvProviders.Columns.Add(this._lvProviders_ColumnHeader_7);
			// 
			// _lvProviders_ColumnHeader_1
			// 
			this._lvProviders_ColumnHeader_1.Text = "Supplier ID";
			this._lvProviders_ColumnHeader_1.Width = 97;
			// 
			// _lvProviders_ColumnHeader_2
			// 
			this._lvProviders_ColumnHeader_2.Text = "Supplier Name";
			this._lvProviders_ColumnHeader_2.Width = 97;
			// 
			// _lvProviders_ColumnHeader_3
			// 
			this._lvProviders_ColumnHeader_3.Text = "Contact Name";
			this._lvProviders_ColumnHeader_3.Width = 97;
			// 
			// _lvProviders_ColumnHeader_4
			// 
			this._lvProviders_ColumnHeader_4.Text = "Contact Last Name";
			this._lvProviders_ColumnHeader_4.Width = 97;
			// 
			// _lvProviders_ColumnHeader_5
			// 
			this._lvProviders_ColumnHeader_5.Text = "City";
			this._lvProviders_ColumnHeader_5.Width = 97;
			// 
			// _lvProviders_ColumnHeader_6
			// 
			this._lvProviders_ColumnHeader_6.Text = "State";
			this._lvProviders_ColumnHeader_6.Width = 97;
			// 
			// _lvProviders_ColumnHeader_7
			// 
			this._lvProviders_ColumnHeader_7.Text = "Country";
			this._lvProviders_ColumnHeader_7.Width = 97;
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
			this.Label3.TabIndex = 16;
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
			this.Label4.TabIndex = 15;
			this.Label4.Text = "Supplier Name:";
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
			this.Label2.TabIndex = 14;
			this.Label2.Text = "Contact name:";
			// 
			// Frame2
			// 
			this.Frame2.BackColor = System.Drawing.SystemColors.Control;
			this.Frame2.Controls.Add(this.txtProviderContact);
			this.Frame2.Controls.Add(this.txtProviderCompany);
			this.Frame2.Controls.Add(this.Label5);
			this.Frame2.Controls.Add(this.Label1);
			this.Frame2.Enabled = true;
			this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame2.Location = new System.Drawing.Point(8, 184);
			this.Frame2.Name = "Frame2";
			this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame2.Size = new System.Drawing.Size(505, 49);
			this.Frame2.TabIndex = 11;
			this.Frame2.Text = "Supplier";
			this.Frame2.Visible = true;
			// 
			// txtProviderContact
			// 
			this.txtProviderContact.AcceptsReturn = true;
			this.txtProviderContact.BackColor = System.Drawing.SystemColors.Menu;
			this.txtProviderContact.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtProviderContact.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtProviderContact.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtProviderContact.Location = new System.Drawing.Point(288, 16);
			this.txtProviderContact.MaxLength = 0;
			this.txtProviderContact.Name = "txtProviderContact";
			this.txtProviderContact.ReadOnly = true;
			this.txtProviderContact.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtProviderContact.Size = new System.Drawing.Size(209, 20);
			this.txtProviderContact.TabIndex = 20;
			this.txtProviderContact.TabStop = false;
			// 
			// txtProviderCompany
			// 
			this.txtProviderCompany.AcceptsReturn = true;
			this.txtProviderCompany.BackColor = System.Drawing.SystemColors.Menu;
			this.txtProviderCompany.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtProviderCompany.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtProviderCompany.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtProviderCompany.Location = new System.Drawing.Point(72, 16);
			this.txtProviderCompany.MaxLength = 0;
			this.txtProviderCompany.Name = "txtProviderCompany";
			this.txtProviderCompany.ReadOnly = true;
			this.txtProviderCompany.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtProviderCompany.Size = new System.Drawing.Size(145, 20);
			this.txtProviderCompany.TabIndex = 19;
			this.txtProviderCompany.TabStop = false;
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
			this.Label5.TabIndex = 18;
			this.Label5.Text = "Name:";
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
			this.Label1.TabIndex = 17;
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
			this.Text3.TabIndex = 10;
			// 
			// Label12
			// 
			this.Label12.BackColor = System.Drawing.SystemColors.Control;
			this.Label12.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label12.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label12.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label12.Location = new System.Drawing.Point(8, 504);
			this.Label12.Name = "Label12";
			this.Label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label12.Size = new System.Drawing.Size(89, 17);
			this.Label12.TabIndex = 32;
			this.Label12.Text = "Freight Charge:";
			// 
			// Label11
			// 
			this.Label11.BackColor = System.Drawing.SystemColors.Control;
			this.Label11.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label11.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label11.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label11.Location = new System.Drawing.Point(8, 528);
			this.Label11.Name = "Label11";
			this.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label11.Size = new System.Drawing.Size(89, 17);
			this.Label11.TabIndex = 30;
			this.Label11.Text = "Total:";
			// 
			// Label10
			// 
			this.Label10.BackColor = System.Drawing.SystemColors.Control;
			this.Label10.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label10.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label10.Location = new System.Drawing.Point(272, 480);
			this.Label10.Name = "Label10";
			this.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label10.Size = new System.Drawing.Size(89, 17);
			this.Label10.TabIndex = 28;
			this.Label10.Text = "Total Tax:";
			// 
			// Label9
			// 
			this.Label9.BackColor = System.Drawing.SystemColors.Control;
			this.Label9.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label9.Location = new System.Drawing.Point(272, 504);
			this.Label9.Name = "Label9";
			this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label9.Size = new System.Drawing.Size(89, 17);
			this.Label9.TabIndex = 26;
			this.Label9.Text = "Sub Total:";
			// 
			// Label8
			// 
			this.Label8.BackColor = System.Drawing.SystemColors.Control;
			this.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label8.Location = new System.Drawing.Point(8, 480);
			this.Label8.Name = "Label8";
			this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label8.Size = new System.Drawing.Size(89, 17);
			this.Label8.TabIndex = 25;
			this.Label8.Text = "Sales Tax:";
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
			this.Label6.Size = new System.Drawing.Size(33, 17);
			this.Label6.TabIndex = 22;
			this.Label6.Text = "Notes:";
			// 
			// frmOrderReception
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(523, 609);
			this.Controls.Add(this.txtNotes);
			this.Controls.Add(this.txtSubTotal);
			this.Controls.Add(this.txtTotal);
			this.Controls.Add(this.txtTotalTax);
			this.Controls.Add(this.txtFreightCharge);
			this.Controls.Add(this.txtSalesTax);
			this.Controls.Add(this.txtEntry);
			this.Controls.Add(this.fgProducts);
			this.Controls.Add(this.sbStatusBar);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdAddProducts);
			this.Controls.Add(this.Frame1);
			this.Controls.Add(this.Frame2);
			this.Controls.Add(this.Text3);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.Label11);
			this.Controls.Add(this.Label10);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.Label6);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Location = new System.Drawing.Point(8, 30);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmOrderReception";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Add Stock Order";
			this.Closed += new System.EventHandler(this.frmOrderReception_Closed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOrderReception_FormClosing);
			this.Load += new System.EventHandler(this.frmOrderReception_Load);
			this.listViewHelper1.SetItemClickMethod(this.lvProviders, "lvProviders_ItemClick");
			this.listViewHelper1.SetCorrectEventsBehavior(this.lvProviders, true);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).EndInit();
			this.sbStatusBar.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.lvProviders.ResumeLayout(false);
			this.Frame2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}