using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	partial class frmAdjustStockManual
	{

		#region "Upgrade Support "
		private static frmAdjustStockManual m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmAdjustStockManual DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = new frmAdjustStockManual();
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
		public frmAdjustStockManual()
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
			ReLoadForm(false);
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "_sbStatusBar_Panel1", "sbStatusBar", "txtStockID", "txtOriginalPrice", "_txtValues_0", "txtQuantityPerUnit", "txtProductName", "txtUnit", "_txtValues_1", "txtOriginalQuantity", "_lvStocks_ColumnHeader_1", "_lvStocks_ColumnHeader_2", "_lvStocks_ColumnHeader_3", "_lvStocks_ColumnHeader_4", "_lvStocks_ColumnHeader_5", "_lvStocks_ColumnHeader_6", "_lvStocks_ColumnHeader_7", "_lvStocks_ColumnHeader_8", "lvStocks", "Frame3", "txtCode", "txtName", "cmdProducts", "Label5", "Label4", "Frame1", "cmdClose", "cmdSave", "_lvProducts_ColumnHeader_1", "_lvProducts_ColumnHeader_2", "_lvProducts_ColumnHeader_3", "_lvProducts_ColumnHeader_4", "_lvProducts_ColumnHeader_5", "_lvProducts_ColumnHeader_6", "_lvProducts_ColumnHeader_7", "lvProducts", "Label14", "lblNewQuantity", "Label12", "lblCurrentQuantity", "Label11", "Label10", "Label8", "Label9", "Label7", "Label6", "Label2", "Label1", "Label3", "txtValues", "listViewHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		private System.Windows.Forms.ToolStripStatusLabel _sbStatusBar_Panel1;
		public System.Windows.Forms.StatusStrip sbStatusBar;
		public System.Windows.Forms.TextBox txtStockID;
		public System.Windows.Forms.TextBox txtOriginalPrice;
		private System.Windows.Forms.TextBox _txtValues_0;
		public System.Windows.Forms.TextBox txtQuantityPerUnit;
		public System.Windows.Forms.TextBox txtProductName;
		public System.Windows.Forms.TextBox txtUnit;
		private System.Windows.Forms.TextBox _txtValues_1;
		public System.Windows.Forms.TextBox txtOriginalQuantity;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_4;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_5;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_6;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_7;
		private System.Windows.Forms.ColumnHeader _lvStocks_ColumnHeader_8;
		public System.Windows.Forms.ListView lvStocks;
		public System.Windows.Forms.GroupBox Frame3;
		public System.Windows.Forms.TextBox txtCode;
		public System.Windows.Forms.TextBox txtName;
		public System.Windows.Forms.Button cmdProducts;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.Button cmdClose;
		public System.Windows.Forms.Button cmdSave;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_4;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_5;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_6;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_7;
		public System.Windows.Forms.ListView lvProducts;
		public System.Windows.Forms.Label Label14;
		public System.Windows.Forms.Label lblNewQuantity;
		public System.Windows.Forms.Label Label12;
		public System.Windows.Forms.Label lblCurrentQuantity;
		public System.Windows.Forms.Label Label11;
		public System.Windows.Forms.Label Label10;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.Label Label6;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.TextBox[] txtValues = new System.Windows.Forms.TextBox[2];
		private UpgradeHelpers.VB6.Gui.ListViewHelper listViewHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdjustStockManual));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.sbStatusBar = new System.Windows.Forms.StatusStrip();
			this._sbStatusBar_Panel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtStockID = new System.Windows.Forms.TextBox();
			this.txtOriginalPrice = new System.Windows.Forms.TextBox();
			this._txtValues_0 = new System.Windows.Forms.TextBox();
			this.txtQuantityPerUnit = new System.Windows.Forms.TextBox();
			this.txtProductName = new System.Windows.Forms.TextBox();
			this.txtUnit = new System.Windows.Forms.TextBox();
			this._txtValues_1 = new System.Windows.Forms.TextBox();
			this.txtOriginalQuantity = new System.Windows.Forms.TextBox();
			this.Frame3 = new System.Windows.Forms.GroupBox();
			this.lvStocks = new System.Windows.Forms.ListView();
			this._lvStocks_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_5 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_6 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_7 = new System.Windows.Forms.ColumnHeader();
			this._lvStocks_ColumnHeader_8 = new System.Windows.Forms.ColumnHeader();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.txtCode = new System.Windows.Forms.TextBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cmdProducts = new System.Windows.Forms.Button();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.cmdClose = new System.Windows.Forms.Button();
			this.cmdSave = new System.Windows.Forms.Button();
			this.lvProducts = new System.Windows.Forms.ListView();
			this._lvProducts_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_5 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_6 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_7 = new System.Windows.Forms.ColumnHeader();
			this.Label14 = new System.Windows.Forms.Label();
			this.lblNewQuantity = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.lblCurrentQuantity = new System.Windows.Forms.Label();
			this.Label11 = new System.Windows.Forms.Label();
			this.Label10 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label9 = new System.Windows.Forms.Label();
			this.Label7 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.sbStatusBar.SuspendLayout();
			this.Frame3.SuspendLayout();
			this.lvStocks.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.lvProducts.SuspendLayout();
			this.SuspendLayout();
			this.listViewHelper1 = new UpgradeHelpers.VB6.Gui.ListViewHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).BeginInit();
			// 
			// sbStatusBar
			// 
			this.sbStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sbStatusBar.Location = new System.Drawing.Point(0, 538);
			this.sbStatusBar.Name = "sbStatusBar";
			this.sbStatusBar.ShowItemToolTips = true;
			this.sbStatusBar.Size = new System.Drawing.Size(426, 25);
			this.sbStatusBar.TabIndex = 28;
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
			this._sbStatusBar_Panel1.Size = new System.Drawing.Size(407, 25);
			this._sbStatusBar_Panel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._sbStatusBar_Panel1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			// 
			// txtStockID
			// 
			this.txtStockID.AcceptsReturn = true;
			this.txtStockID.BackColor = System.Drawing.SystemColors.Menu;
			this.txtStockID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtStockID.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtStockID.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtStockID.Location = new System.Drawing.Point(96, 374);
			this.txtStockID.MaxLength = 0;
			this.txtStockID.Name = "txtStockID";
			this.txtStockID.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtStockID.Size = new System.Drawing.Size(81, 20);
			this.txtStockID.TabIndex = 26;
			this.txtStockID.TabStop = false;
			// 
			// txtOriginalPrice
			// 
			this.txtOriginalPrice.AcceptsReturn = true;
			this.txtOriginalPrice.BackColor = System.Drawing.SystemColors.Menu;
			this.txtOriginalPrice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtOriginalPrice.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtOriginalPrice.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtOriginalPrice.Location = new System.Drawing.Point(96, 403);
			this.txtOriginalPrice.MaxLength = 0;
			this.txtOriginalPrice.Name = "txtOriginalPrice";
			this.txtOriginalPrice.ReadOnly = true;
			this.txtOriginalPrice.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtOriginalPrice.Size = new System.Drawing.Size(81, 20);
			this.txtOriginalPrice.TabIndex = 25;
			this.txtOriginalPrice.TabStop = false;
			this.txtOriginalPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _txtValues_0
			// 
			this._txtValues_0.AcceptsReturn = true;
			this._txtValues_0.BackColor = System.Drawing.SystemColors.Window;
			this._txtValues_0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtValues_0.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtValues_0.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtValues_0.Location = new System.Drawing.Point(328, 403);
			this._txtValues_0.MaxLength = 0;
			this._txtValues_0.Name = "_txtValues_0";
			this._txtValues_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtValues_0.Size = new System.Drawing.Size(81, 20);
			this._txtValues_0.TabIndex = 4;
			this._txtValues_0.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._txtValues_0.Enter += new System.EventHandler(this.txtValues_Enter);
			this._txtValues_0.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValues_KeyPress);
			this._txtValues_0.TextChanged += new System.EventHandler(this.txtValues_TextChanged);
			// 
			// txtQuantityPerUnit
			// 
			this.txtQuantityPerUnit.AcceptsReturn = true;
			this.txtQuantityPerUnit.BackColor = System.Drawing.SystemColors.Menu;
			this.txtQuantityPerUnit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtQuantityPerUnit.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtQuantityPerUnit.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtQuantityPerUnit.Location = new System.Drawing.Point(328, 374);
			this.txtQuantityPerUnit.MaxLength = 0;
			this.txtQuantityPerUnit.Name = "txtQuantityPerUnit";
			this.txtQuantityPerUnit.ReadOnly = true;
			this.txtQuantityPerUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtQuantityPerUnit.Size = new System.Drawing.Size(81, 20);
			this.txtQuantityPerUnit.TabIndex = 21;
			this.txtQuantityPerUnit.TabStop = false;
			this.txtQuantityPerUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtProductName
			// 
			this.txtProductName.AcceptsReturn = true;
			this.txtProductName.BackColor = System.Drawing.SystemColors.Menu;
			this.txtProductName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtProductName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtProductName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtProductName.Location = new System.Drawing.Point(96, 344);
			this.txtProductName.MaxLength = 0;
			this.txtProductName.Name = "txtProductName";
			this.txtProductName.ReadOnly = true;
			this.txtProductName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtProductName.Size = new System.Drawing.Size(145, 20);
			this.txtProductName.TabIndex = 20;
			this.txtProductName.TabStop = false;
			// 
			// txtUnit
			// 
			this.txtUnit.AcceptsReturn = true;
			this.txtUnit.BackColor = System.Drawing.SystemColors.Menu;
			this.txtUnit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtUnit.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtUnit.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtUnit.Location = new System.Drawing.Point(328, 344);
			this.txtUnit.MaxLength = 0;
			this.txtUnit.Name = "txtUnit";
			this.txtUnit.ReadOnly = true;
			this.txtUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtUnit.Size = new System.Drawing.Size(81, 20);
			this.txtUnit.TabIndex = 19;
			this.txtUnit.TabStop = false;
			// 
			// _txtValues_1
			// 
			this._txtValues_1.AcceptsReturn = true;
			this._txtValues_1.BackColor = System.Drawing.SystemColors.Window;
			this._txtValues_1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtValues_1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtValues_1.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtValues_1.Location = new System.Drawing.Point(328, 432);
			this._txtValues_1.MaxLength = 0;
			this._txtValues_1.Name = "_txtValues_1";
			this._txtValues_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtValues_1.Size = new System.Drawing.Size(81, 20);
			this._txtValues_1.TabIndex = 5;
			this._txtValues_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._txtValues_1.Enter += new System.EventHandler(this.txtValues_Enter);
			this._txtValues_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValues_KeyPress);
			this._txtValues_1.TextChanged += new System.EventHandler(this.txtValues_TextChanged);
			// 
			// txtOriginalQuantity
			// 
			this.txtOriginalQuantity.AcceptsReturn = true;
			this.txtOriginalQuantity.BackColor = System.Drawing.SystemColors.Menu;
			this.txtOriginalQuantity.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtOriginalQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtOriginalQuantity.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtOriginalQuantity.Location = new System.Drawing.Point(96, 432);
			this.txtOriginalQuantity.MaxLength = 0;
			this.txtOriginalQuantity.Name = "txtOriginalQuantity";
			this.txtOriginalQuantity.ReadOnly = true;
			this.txtOriginalQuantity.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtOriginalQuantity.Size = new System.Drawing.Size(81, 20);
			this.txtOriginalQuantity.TabIndex = 14;
			this.txtOriginalQuantity.TabStop = false;
			this.txtOriginalQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// Frame3
			// 
			this.Frame3.BackColor = System.Drawing.SystemColors.Control;
			this.Frame3.Controls.Add(this.lvStocks);
			this.Frame3.Enabled = true;
			this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame3.Location = new System.Drawing.Point(8, 200);
			this.Frame3.Name = "Frame3";
			this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame3.Size = new System.Drawing.Size(409, 137);
			this.Frame3.TabIndex = 13;
			this.Frame3.Text = "Stocks for the product ";
			this.Frame3.Visible = true;
			// 
			// lvStocks
			// 
			this.lvStocks.BackColor = System.Drawing.SystemColors.Window;
			this.lvStocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvStocks.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvStocks.FullRowSelect = true;
			this.lvStocks.GridLines = true;
			this.lvStocks.HideSelection = false;
			this.lvStocks.HotTracking = true;
			this.lvStocks.LabelEdit = false;
			this.lvStocks.Location = new System.Drawing.Point(8, 16);
			this.lvStocks.Name = "lvStocks";
			this.lvStocks.Size = new System.Drawing.Size(393, 113);
			this.lvStocks.TabIndex = 3;
			this.lvStocks.View = System.Windows.Forms.View.Details;
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_1);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_2);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_3);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_4);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_5);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_6);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_7);
			this.lvStocks.Columns.Add(this._lvStocks_ColumnHeader_8);
			// 
			// _lvStocks_ColumnHeader_1
			// 
			this._lvStocks_ColumnHeader_1.Text = "Stock ID";
			this._lvStocks_ColumnHeader_1.Width = 97;
			// 
			// _lvStocks_ColumnHeader_2
			// 
			this._lvStocks_ColumnHeader_2.Text = "Current Stock";
			this._lvStocks_ColumnHeader_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_2.Width = 97;
			// 
			// _lvStocks_ColumnHeader_3
			// 
			this._lvStocks_ColumnHeader_3.Text = "Initial Stock";
			this._lvStocks_ColumnHeader_3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_3.Width = 97;
			// 
			// _lvStocks_ColumnHeader_4
			// 
			this._lvStocks_ColumnHeader_4.Text = "Price";
			this._lvStocks_ColumnHeader_4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_4.Width = 97;
			// 
			// _lvStocks_ColumnHeader_5
			// 
			this._lvStocks_ColumnHeader_5.Text = "Stock Price";
			this._lvStocks_ColumnHeader_5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_5.Width = 97;
			// 
			// _lvStocks_ColumnHeader_6
			// 
			this._lvStocks_ColumnHeader_6.Text = "Created";
			this._lvStocks_ColumnHeader_6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_6.Width = 97;
			// 
			// _lvStocks_ColumnHeader_7
			// 
			this._lvStocks_ColumnHeader_7.Text = "Modified";
			this._lvStocks_ColumnHeader_7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvStocks_ColumnHeader_7.Width = 97;
			// 
			// _lvStocks_ColumnHeader_8
			// 
			this._lvStocks_ColumnHeader_8.Text = "User";
			this._lvStocks_ColumnHeader_8.Width = 97;
			// 
			// Frame1
			// 
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.txtCode);
			this.Frame1.Controls.Add(this.txtName);
			this.Frame1.Controls.Add(this.cmdProducts);
			this.Frame1.Controls.Add(this.Label5);
			this.Frame1.Controls.Add(this.Label4);
			this.Frame1.Enabled = true;
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(8, 32);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(409, 65);
			this.Frame1.TabIndex = 9;
			this.Frame1.Text = "Search product ";
			this.Frame1.Visible = true;
			// 
			// txtCode
			// 
			this.txtCode.AcceptsReturn = true;
			this.txtCode.BackColor = System.Drawing.SystemColors.Window;
			this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtCode.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtCode.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtCode.Location = new System.Drawing.Point(112, 16);
			this.txtCode.MaxLength = 0;
			this.txtCode.Name = "txtCode";
			this.txtCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtCode.Size = new System.Drawing.Size(97, 20);
			this.txtCode.TabIndex = 0;
			this.txtCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCode_KeyPress);
			this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
			// 
			// txtName
			// 
			this.txtName.AcceptsReturn = true;
			this.txtName.BackColor = System.Drawing.SystemColors.Window;
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtName.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtName.Location = new System.Drawing.Point(112, 40);
			this.txtName.MaxLength = 0;
			this.txtName.Name = "txtName";
			this.txtName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtName.Size = new System.Drawing.Size(145, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// cmdProducts
			// 
			this.cmdProducts.BackColor = System.Drawing.SystemColors.Control;
			this.cmdProducts.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdProducts.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdProducts.Location = new System.Drawing.Point(360, 16);
			this.cmdProducts.Name = "cmdProducts";
			this.cmdProducts.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdProducts.Size = new System.Drawing.Size(25, 21);
			this.cmdProducts.TabIndex = 10;
			this.cmdProducts.TabStop = false;
			this.cmdProducts.Text = "...";
			this.cmdProducts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdProducts.UseVisualStyleBackColor = false;
			this.cmdProducts.Click += new System.EventHandler(this.cmdProducts_Click);
			// 
			// Label5
			// 
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(16, 16);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(89, 17);
			this.Label5.TabIndex = 12;
			this.Label5.Text = "Product code:";
			// 
			// Label4
			// 
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(16, 40);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(89, 17);
			this.Label4.TabIndex = 11;
			this.Label4.Text = "Product name:";
			// 
			// cmdClose
			// 
			this.cmdClose.BackColor = System.Drawing.SystemColors.Control;
			this.cmdClose.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdClose.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdClose.Location = new System.Drawing.Point(344, 504);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdClose.Size = new System.Drawing.Size(73, 25);
			this.cmdClose.TabIndex = 7;
			this.cmdClose.Text = "&Close";
			this.cmdClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdClose.UseVisualStyleBackColor = false;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// cmdSave
			// 
			this.cmdSave.BackColor = System.Drawing.SystemColors.Control;
			this.cmdSave.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdSave.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdSave.Location = new System.Drawing.Point(264, 504);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdSave.Size = new System.Drawing.Size(73, 25);
			this.cmdSave.TabIndex = 6;
			this.cmdSave.Text = "&Save";
			this.cmdSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdSave.UseVisualStyleBackColor = false;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// lvProducts
			// 
			this.lvProducts.BackColor = System.Drawing.SystemColors.Window;
			this.lvProducts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvProducts.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvProducts.FullRowSelect = true;
			this.lvProducts.GridLines = true;
			this.lvProducts.HideSelection = false;
			this.lvProducts.HotTracking = true;
			this.lvProducts.LabelEdit = false;
			this.lvProducts.Location = new System.Drawing.Point(8, 104);
			this.lvProducts.Name = "lvProducts";
			this.lvProducts.Size = new System.Drawing.Size(409, 97);
			this.lvProducts.TabIndex = 2;
			this.lvProducts.View = System.Windows.Forms.View.Details;
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_1);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_2);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_3);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_4);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_5);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_6);
			this.lvProducts.Columns.Add(this._lvProducts_ColumnHeader_7);
			// 
			// _lvProducts_ColumnHeader_1
			// 
			this._lvProducts_ColumnHeader_1.Text = "Code";
			this._lvProducts_ColumnHeader_1.Width = 97;
			// 
			// _lvProducts_ColumnHeader_2
			// 
			this._lvProducts_ColumnHeader_2.Text = "Name";
			this._lvProducts_ColumnHeader_2.Width = 97;
			// 
			// _lvProducts_ColumnHeader_3
			// 
			this._lvProducts_ColumnHeader_3.Text = "Price";
			this._lvProducts_ColumnHeader_3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProducts_ColumnHeader_3.Width = 97;
			// 
			// _lvProducts_ColumnHeader_4
			// 
			this._lvProducts_ColumnHeader_4.Text = "Existence";
			this._lvProducts_ColumnHeader_4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProducts_ColumnHeader_4.Width = 97;
			// 
			// _lvProducts_ColumnHeader_5
			// 
			this._lvProducts_ColumnHeader_5.Text = "Ordered";
			this._lvProducts_ColumnHeader_5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProducts_ColumnHeader_5.Width = 97;
			// 
			// _lvProducts_ColumnHeader_6
			// 
			this._lvProducts_ColumnHeader_6.Text = "Quantity per Unit";
			this._lvProducts_ColumnHeader_6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProducts_ColumnHeader_6.Width = 97;
			// 
			// _lvProducts_ColumnHeader_7
			// 
			this._lvProducts_ColumnHeader_7.Text = "Unit";
			this._lvProducts_ColumnHeader_7.Width = 97;
			// 
			// Label14
			// 
			this.Label14.BackColor = System.Drawing.SystemColors.Control;
			this.Label14.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label14.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label14.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label14.Location = new System.Drawing.Point(216, 472);
			this.Label14.Name = "Label14";
			this.Label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label14.Size = new System.Drawing.Size(89, 17);
			this.Label14.TabIndex = 32;
			this.Label14.Text = "Adjusted quantity";
			// 
			// lblNewQuantity
			// 
			this.lblNewQuantity.BackColor = System.Drawing.SystemColors.Control;
			this.lblNewQuantity.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblNewQuantity.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblNewQuantity.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblNewQuantity.Location = new System.Drawing.Point(312, 472);
			this.lblNewQuantity.Name = "lblNewQuantity";
			this.lblNewQuantity.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblNewQuantity.Size = new System.Drawing.Size(89, 17);
			this.lblNewQuantity.TabIndex = 31;
			// 
			// Label12
			// 
			this.Label12.BackColor = System.Drawing.SystemColors.Control;
			this.Label12.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label12.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label12.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label12.Location = new System.Drawing.Point(16, 472);
			this.Label12.Name = "Label12";
			this.Label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label12.Size = new System.Drawing.Size(89, 17);
			this.Label12.TabIndex = 30;
			this.Label12.Text = "Stock quantity";
			// 
			// lblCurrentQuantity
			// 
			this.lblCurrentQuantity.BackColor = System.Drawing.SystemColors.Control;
			this.lblCurrentQuantity.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblCurrentQuantity.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblCurrentQuantity.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblCurrentQuantity.Location = new System.Drawing.Point(112, 472);
			this.lblCurrentQuantity.Name = "lblCurrentQuantity";
			this.lblCurrentQuantity.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblCurrentQuantity.Size = new System.Drawing.Size(89, 17);
			this.lblCurrentQuantity.TabIndex = 29;
			// 
			// Label11
			// 
			this.Label11.BackColor = System.Drawing.SystemColors.Control;
			this.Label11.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label11.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label11.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label11.Location = new System.Drawing.Point(16, 376);
			this.Label11.Name = "Label11";
			this.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label11.Size = new System.Drawing.Size(65, 17);
			this.Label11.TabIndex = 27;
			this.Label11.Text = "Stock ID:";
			// 
			// Label10
			// 
			this.Label10.BackColor = System.Drawing.SystemColors.Control;
			this.Label10.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label10.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label10.Location = new System.Drawing.Point(232, 376);
			this.Label10.Name = "Label10";
			this.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label10.Size = new System.Drawing.Size(89, 17);
			this.Label10.TabIndex = 24;
			this.Label10.Text = "Quantity per Unit";
			// 
			// Label8
			// 
			this.Label8.BackColor = System.Drawing.SystemColors.Control;
			this.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label8.Location = new System.Drawing.Point(16, 344);
			this.Label8.Name = "Label8";
			this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label8.Size = new System.Drawing.Size(89, 17);
			this.Label8.TabIndex = 23;
			this.Label8.Text = "Product name:";
			// 
			// Label9
			// 
			this.Label9.BackColor = System.Drawing.SystemColors.Control;
			this.Label9.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label9.Location = new System.Drawing.Point(296, 344);
			this.Label9.Name = "Label9";
			this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label9.Size = new System.Drawing.Size(25, 17);
			this.Label9.TabIndex = 22;
			this.Label9.Text = "Unit";
			// 
			// Label7
			// 
			this.Label7.BackColor = System.Drawing.SystemColors.Control;
			this.Label7.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label7.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label7.Location = new System.Drawing.Point(216, 435);
			this.Label7.Name = "Label7";
			this.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label7.Size = new System.Drawing.Size(89, 17);
			this.Label7.TabIndex = 18;
			this.Label7.Text = "Adjusted &Quantity";
			// 
			// Label6
			// 
			this.Label6.BackColor = System.Drawing.SystemColors.Control;
			this.Label6.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label6.Location = new System.Drawing.Point(216, 406);
			this.Label6.Name = "Label6";
			this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label6.Size = new System.Drawing.Size(73, 17);
			this.Label6.TabIndex = 17;
			this.Label6.Text = "Adjusted &Price";
			// 
			// Label2
			// 
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(16, 435);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(81, 17);
			this.Label2.TabIndex = 16;
			this.Label2.Text = "Original Quantity";
			// 
			// Label1
			// 
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(16, 406);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(73, 17);
			this.Label1.TabIndex = 15;
			this.Label1.Text = "Original Price";
			// 
			// Label3
			// 
			this.Label3.BackColor = System.Drawing.SystemColors.Control;
			this.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label3.Location = new System.Drawing.Point(16, 8);
			this.Label3.Name = "Label3";
			this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label3.Size = new System.Drawing.Size(121, 17);
			this.Label3.TabIndex = 8;
			this.Label3.Text = "Select a product first";
			// 
			// frmAdjustStockManual
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(426, 563);
			this.Controls.Add(this.sbStatusBar);
			this.Controls.Add(this.txtStockID);
			this.Controls.Add(this.txtOriginalPrice);
			this.Controls.Add(this._txtValues_0);
			this.Controls.Add(this.txtQuantityPerUnit);
			this.Controls.Add(this.txtProductName);
			this.Controls.Add(this.txtUnit);
			this.Controls.Add(this._txtValues_1);
			this.Controls.Add(this.txtOriginalQuantity);
			this.Controls.Add(this.Frame3);
			this.Controls.Add(this.Frame1);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.lvProducts);
			this.Controls.Add(this.Label14);
			this.Controls.Add(this.lblNewQuantity);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.lblCurrentQuantity);
			this.Controls.Add(this.Label11);
			this.Controls.Add(this.Label10);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.Label7);
			this.Controls.Add(this.Label6);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Label3);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Location = new System.Drawing.Point(8, 30);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmAdjustStockManual";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Inventory Adjust";
			this.Closed += new System.EventHandler(this.frmAdjustStockManual_Closed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAdjustStockManual_FormClosing);
			this.listViewHelper1.SetItemClickMethod(this.lvStocks, "lvStocks_ItemClick");
			this.listViewHelper1.SetCorrectEventsBehavior(this.lvStocks, true);
			this.listViewHelper1.SetItemClickMethod(this.lvProducts, "lvProducts_ItemClick");
			this.listViewHelper1.SetCorrectEventsBehavior(this.lvProducts, true);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).EndInit();
			this.sbStatusBar.ResumeLayout(false);
			this.Frame3.ResumeLayout(false);
			this.lvStocks.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.lvProducts.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		void ReLoadForm(bool addEvents)
		{
			InitializetxtValues();
		}
		void InitializetxtValues()
		{
			this.txtValues = new System.Windows.Forms.TextBox[2];
			this.txtValues[0] = _txtValues_0;
			this.txtValues[1] = _txtValues_1;
		}
		#endregion
	}
}