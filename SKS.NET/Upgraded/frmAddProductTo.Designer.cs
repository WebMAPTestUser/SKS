using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	partial class frmAddProductTo
	{

		#region "Upgrade Support "
		private static frmAddProductTo m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmAddProductTo DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = new frmAddProductTo();
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
		public frmAddProductTo()
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
		private string[] visualControls = new string[]{"components", "ToolTipMain", "chkAll", "cmdRemove", "_sbStatusBar_Panel1", "sbStatusBar", "cmdClose", "cmdSave", "cmdProducts", "txtName", "txtCode", "_lvProducts_ColumnHeader_1", "_lvProducts_ColumnHeader_2", "_lvProducts_ColumnHeader_3", "_lvProducts_ColumnHeader_4", "_lvProducts_ColumnHeader_5", "_lvProducts_ColumnHeader_6", "_lvProducts_ColumnHeader_7", "lvProducts", "Label4", "Label5", "Frame1", "_lvProductsBy_ColumnHeader_1", "_lvProductsBy_ColumnHeader_2", "_lvProductsBy_ColumnHeader_3", "_lvProductsBy_ColumnHeader_4", "lvProductsBy", "lblProductsRelated", "listViewHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.CheckBox chkAll;
		public System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.ToolStripStatusLabel _sbStatusBar_Panel1;
		public System.Windows.Forms.StatusStrip sbStatusBar;
		public System.Windows.Forms.Button cmdClose;
		public System.Windows.Forms.Button cmdSave;
		public System.Windows.Forms.Button cmdProducts;
		public System.Windows.Forms.TextBox txtName;
		public System.Windows.Forms.TextBox txtCode;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_4;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_5;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_6;
		private System.Windows.Forms.ColumnHeader _lvProducts_ColumnHeader_7;
		public System.Windows.Forms.ListView lvProducts;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.GroupBox Frame1;
		private System.Windows.Forms.ColumnHeader _lvProductsBy_ColumnHeader_1;
		private System.Windows.Forms.ColumnHeader _lvProductsBy_ColumnHeader_2;
		private System.Windows.Forms.ColumnHeader _lvProductsBy_ColumnHeader_3;
		private System.Windows.Forms.ColumnHeader _lvProductsBy_ColumnHeader_4;
		public System.Windows.Forms.ListView lvProductsBy;
		public System.Windows.Forms.Label lblProductsRelated;
		private UpgradeHelpers.VB6.Gui.ListViewHelper listViewHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddProductTo));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.chkAll = new System.Windows.Forms.CheckBox();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.sbStatusBar = new System.Windows.Forms.StatusStrip();
			this._sbStatusBar_Panel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.cmdClose = new System.Windows.Forms.Button();
			this.cmdSave = new System.Windows.Forms.Button();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.cmdProducts = new System.Windows.Forms.Button();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtCode = new System.Windows.Forms.TextBox();
			this.lvProducts = new System.Windows.Forms.ListView();
			this._lvProducts_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_5 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_6 = new System.Windows.Forms.ColumnHeader();
			this._lvProducts_ColumnHeader_7 = new System.Windows.Forms.ColumnHeader();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.lvProductsBy = new System.Windows.Forms.ListView();
			this._lvProductsBy_ColumnHeader_1 = new System.Windows.Forms.ColumnHeader();
			this._lvProductsBy_ColumnHeader_2 = new System.Windows.Forms.ColumnHeader();
			this._lvProductsBy_ColumnHeader_3 = new System.Windows.Forms.ColumnHeader();
			this._lvProductsBy_ColumnHeader_4 = new System.Windows.Forms.ColumnHeader();
			this.lblProductsRelated = new System.Windows.Forms.Label();
			this.sbStatusBar.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.lvProducts.SuspendLayout();
			this.lvProductsBy.SuspendLayout();
			this.SuspendLayout();
			this.listViewHelper1 = new UpgradeHelpers.VB6.Gui.ListViewHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).BeginInit();
			// 
			// chkAll
			// 
			this.chkAll.Appearance = System.Windows.Forms.Appearance.Normal;
			this.chkAll.BackColor = System.Drawing.SystemColors.Control;
			this.chkAll.CausesValidation = true;
			this.chkAll.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkAll.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.chkAll.Cursor = System.Windows.Forms.Cursors.Default;
			this.chkAll.Enabled = true;
			this.chkAll.ForeColor = System.Drawing.SystemColors.ControlText;
			this.chkAll.Location = new System.Drawing.Point(112, 454);
			this.chkAll.Name = "chkAll";
			this.chkAll.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkAll.Size = new System.Drawing.Size(81, 17);
			this.chkAll.TabIndex = 13;
			this.chkAll.TabStop = false;
			this.chkAll.Text = "Check All";
			this.chkAll.Visible = true;
			this.chkAll.CheckStateChanged += new System.EventHandler(this.chkAll_CheckStateChanged);
			// 
			// cmdRemove
			// 
			this.cmdRemove.BackColor = System.Drawing.SystemColors.Control;
			this.cmdRemove.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdRemove.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdRemove.Location = new System.Drawing.Point(8, 448);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdRemove.Size = new System.Drawing.Size(97, 25);
			this.cmdRemove.TabIndex = 12;
			this.cmdRemove.TabStop = false;
			this.cmdRemove.Text = "&Remove Checked";
			this.cmdRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdRemove.UseVisualStyleBackColor = false;
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// sbStatusBar
			// 
			this.sbStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sbStatusBar.Location = new System.Drawing.Point(0, 488);
			this.sbStatusBar.Name = "sbStatusBar";
			this.sbStatusBar.ShowItemToolTips = true;
			this.sbStatusBar.Size = new System.Drawing.Size(424, 23);
			this.sbStatusBar.TabIndex = 11;
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
			this._sbStatusBar_Panel1.Size = new System.Drawing.Size(405, 23);
			this._sbStatusBar_Panel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._sbStatusBar_Panel1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			// 
			// cmdClose
			// 
			this.cmdClose.BackColor = System.Drawing.SystemColors.Control;
			this.cmdClose.Cursor = System.Windows.Forms.Cursors.Default;
			this.cmdClose.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdClose.Location = new System.Drawing.Point(328, 448);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdClose.Size = new System.Drawing.Size(89, 25);
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
			this.cmdSave.Location = new System.Drawing.Point(224, 448);
			this.cmdSave.Name = "cmdSave";
			this.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdSave.Size = new System.Drawing.Size(89, 25);
			this.cmdSave.TabIndex = 6;
			this.cmdSave.Text = "&Save";
			this.cmdSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdSave.UseVisualStyleBackColor = false;
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			// 
			// Frame1
			// 
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.cmdProducts);
			this.Frame1.Controls.Add(this.txtName);
			this.Frame1.Controls.Add(this.txtCode);
			this.Frame1.Controls.Add(this.lvProducts);
			this.Frame1.Controls.Add(this.Label4);
			this.Frame1.Controls.Add(this.Label5);
			this.Frame1.Enabled = true;
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(8, 8);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(409, 233);
			this.Frame1.TabIndex = 4;
			this.Frame1.Text = "Search product ";
			this.Frame1.Visible = true;
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
			this.cmdProducts.TabIndex = 5;
			this.cmdProducts.TabStop = false;
			this.cmdProducts.Text = "...";
			this.cmdProducts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdProducts.UseVisualStyleBackColor = false;
			this.cmdProducts.Click += new System.EventHandler(this.cmdProducts_Click);
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
			this.txtCode.Leave += new System.EventHandler(this.txtCode_Leave);
			this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
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
			this.lvProducts.Location = new System.Drawing.Point(8, 64);
			this.lvProducts.Name = "lvProducts";
			this.lvProducts.Size = new System.Drawing.Size(393, 161);
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
			this.Label4.TabIndex = 9;
			this.Label4.Text = "Product name:";
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
			this.Label5.TabIndex = 8;
			this.Label5.Text = "Product code:";
			// 
			// lvProductsBy
			// 
			this.lvProductsBy.BackColor = System.Drawing.SystemColors.Window;
			this.lvProductsBy.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvProductsBy.CheckBoxes = true;
			this.lvProductsBy.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvProductsBy.FullRowSelect = true;
			this.lvProductsBy.GridLines = true;
			this.lvProductsBy.HideSelection = false;
			this.lvProductsBy.HotTracking = true;
			this.lvProductsBy.LabelEdit = false;
			this.lvProductsBy.Location = new System.Drawing.Point(8, 272);
			this.lvProductsBy.Name = "lvProductsBy";
			this.lvProductsBy.Size = new System.Drawing.Size(409, 169);
			this.lvProductsBy.TabIndex = 3;
			this.lvProductsBy.View = System.Windows.Forms.View.Details;
			this.lvProductsBy.Columns.Add(this._lvProductsBy_ColumnHeader_1);
			this.lvProductsBy.Columns.Add(this._lvProductsBy_ColumnHeader_2);
			this.lvProductsBy.Columns.Add(this._lvProductsBy_ColumnHeader_3);
			this.lvProductsBy.Columns.Add(this._lvProductsBy_ColumnHeader_4);
			// 
			// _lvProductsBy_ColumnHeader_1
			// 
			this._lvProductsBy_ColumnHeader_1.Text = "Code";
			this._lvProductsBy_ColumnHeader_1.Width = 97;
			// 
			// _lvProductsBy_ColumnHeader_2
			// 
			this._lvProductsBy_ColumnHeader_2.Text = "Name";
			this._lvProductsBy_ColumnHeader_2.Width = 97;
			// 
			// _lvProductsBy_ColumnHeader_3
			// 
			this._lvProductsBy_ColumnHeader_3.Text = "Price";
			this._lvProductsBy_ColumnHeader_3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProductsBy_ColumnHeader_3.Width = 61;
			// 
			// _lvProductsBy_ColumnHeader_4
			// 
			this._lvProductsBy_ColumnHeader_4.Text = "Quantity per Unit";
			this._lvProductsBy_ColumnHeader_4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._lvProductsBy_ColumnHeader_4.Width = 94;
			// 
			// lblProductsRelated
			// 
			this.lblProductsRelated.BackColor = System.Drawing.SystemColors.Control;
			this.lblProductsRelated.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblProductsRelated.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblProductsRelated.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblProductsRelated.Location = new System.Drawing.Point(8, 248);
			this.lblProductsRelated.Name = "lblProductsRelated";
			this.lblProductsRelated.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblProductsRelated.Size = new System.Drawing.Size(409, 17);
			this.lblProductsRelated.TabIndex = 10;
			this.lblProductsRelated.Text = "Products";
			// 
			// frmAddProductTo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(424, 511);
			this.Controls.Add(this.chkAll);
			this.Controls.Add(this.cmdRemove);
			this.Controls.Add(this.sbStatusBar);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdSave);
			this.Controls.Add(this.Frame1);
			this.Controls.Add(this.lvProductsBy);
			this.Controls.Add(this.lblProductsRelated);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Location = new System.Drawing.Point(8, 30);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmAddProductTo";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Create New Product Item";
			this.Closed += new System.EventHandler(this.frmAddProductTo_Closed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAddProductTo_FormClosing);
			this.Load += new System.EventHandler(this.frmAddProductTo_Load);
			this.listViewHelper1.SetItemClickMethod(this.lvProducts, "lvProducts_ItemClick");
			this.listViewHelper1.SetCorrectEventsBehavior(this.lvProducts, true);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).EndInit();
			this.sbStatusBar.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.lvProducts.ResumeLayout(false);
			this.lvProductsBy.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}