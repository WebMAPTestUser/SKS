using VB6 = Microsoft.VisualBasic.Compatibility.VB6.Support;

namespace SKS
{
	partial class frmSales
	{

		#region "Upgrade Support "
		private static frmSales m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmSales DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = new frmSales();
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
		public frmSales()
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
			isInitializingComponent = true;
			InitializeComponent();
			isInitializingComponent = false;
			ReLoadForm(false);
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "lvwSales", "ctrLine", "cboCashier", "cboMonth", "cboYear", "_Toolbar1_Button1", "Toolbar1", "Label9", "Label2", "Label5", "Label8", "lblSellable", "Label4", "lblTotalSales", "Label1", "listViewHelper1", "listBoxComboBoxHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.ListView lvwSales;
		public System.Windows.Forms.PictureBox ctrLine;
		public System.Windows.Forms.ComboBox cboCashier;
		public System.Windows.Forms.ComboBox cboMonth;
		public System.Windows.Forms.ComboBox cboYear;
		private System.Windows.Forms.ToolStripButton _Toolbar1_Button1;
		public System.Windows.Forms.ToolStrip Toolbar1;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label lblSellable;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label lblTotalSales;
		public System.Windows.Forms.Label Label1;
		private UpgradeHelpers.VB6.Gui.ListViewHelper listViewHelper1;
		private UpgradeHelpers.VB6.Gui.ListControlHelper listBoxComboBoxHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSales));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.lvwSales = new System.Windows.Forms.ListView();
			this.ctrLine = new System.Windows.Forms.PictureBox();
			this.cboCashier = new System.Windows.Forms.ComboBox();
			this.cboMonth = new System.Windows.Forms.ComboBox();
			this.cboYear = new System.Windows.Forms.ComboBox();
			this.Toolbar1 = new System.Windows.Forms.ToolStrip();
			this._Toolbar1_Button1 = new System.Windows.Forms.ToolStripButton();
			this.Label9 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label8 = new System.Windows.Forms.Label();
			this.lblSellable = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.lblTotalSales = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.Toolbar1.SuspendLayout();
			this.SuspendLayout();
			this.listViewHelper1 = new UpgradeHelpers.VB6.Gui.ListViewHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).BeginInit();
			this.listBoxComboBoxHelper1 = new UpgradeHelpers.VB6.Gui.ListControlHelper(this.components);
			((System.ComponentModel.ISupportInitialize) this.listBoxComboBoxHelper1).BeginInit();
			// 
			// lvwSales
			// 
			this.lvwSales.BackColor = System.Drawing.SystemColors.Window;
			this.lvwSales.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lvwSales.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvwSales.HideSelection = true;
			this.lvwSales.LabelEdit = true;
			this.lvwSales.Location = new System.Drawing.Point(0, 120);
			this.lvwSales.Name = "lvwSales";
			this.lvwSales.Size = new System.Drawing.Size(521, 193);
			this.lvwSales.TabIndex = 12;
			// 
			// ctrLine
			// 
			this.ctrLine.BackColor = System.Drawing.SystemColors.Control;
			this.ctrLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ctrLine.CausesValidation = true;
			this.ctrLine.Cursor = System.Windows.Forms.Cursors.Default;
			this.ctrLine.Dock = System.Windows.Forms.DockStyle.None;
			this.ctrLine.Enabled = true;
			this.ctrLine.Location = new System.Drawing.Point(0, 88);
			this.ctrLine.Name = "ctrLine";
			this.ctrLine.Size = new System.Drawing.Size(601, 2);
			this.ctrLine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
			this.ctrLine.TabIndex = 11;
			this.ctrLine.TabStop = true;
			this.ctrLine.Visible = true;
			// 
			// cboCashier
			// 
			this.cboCashier.BackColor = System.Drawing.SystemColors.Window;
			this.cboCashier.CausesValidation = true;
			this.cboCashier.Cursor = System.Windows.Forms.Cursors.Default;
			this.cboCashier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCashier.Enabled = true;
			this.cboCashier.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cboCashier.ForeColor = System.Drawing.SystemColors.WindowText;
			this.cboCashier.IntegralHeight = true;
			this.cboCashier.Location = new System.Drawing.Point(384, 56);
			this.cboCashier.Name = "cboCashier";
			this.cboCashier.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cboCashier.Size = new System.Drawing.Size(137, 21);
			this.cboCashier.Sorted = false;
			this.cboCashier.TabIndex = 9;
			this.cboCashier.TabStop = true;
			this.cboCashier.Visible = true;
			this.cboCashier.Items.AddRange(new object[]{"View All"});
			this.cboCashier.SelectedIndexChanged += new System.EventHandler(this.cboCashier_SelectedIndexChanged);
			// 
			// cboMonth
			// 
			this.cboMonth.BackColor = System.Drawing.SystemColors.Window;
			this.cboMonth.CausesValidation = true;
			this.cboMonth.Cursor = System.Windows.Forms.Cursors.Default;
			this.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMonth.Enabled = true;
			this.cboMonth.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cboMonth.ForeColor = System.Drawing.SystemColors.WindowText;
			this.cboMonth.IntegralHeight = true;
			this.cboMonth.Location = new System.Drawing.Point(216, 56);
			this.cboMonth.Name = "cboMonth";
			this.cboMonth.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cboMonth.Size = new System.Drawing.Size(73, 21);
			this.cboMonth.Sorted = false;
			this.cboMonth.TabIndex = 8;
			this.cboMonth.TabStop = true;
			this.cboMonth.Visible = true;
			this.cboMonth.Items.AddRange(new object[]{"(none)", "1 - Jan", "2 - Feb", "3 - Mar", "4 - Apr", "5 - May", "6 - Jun", "7 - Jul", "8 - Aug", "9 - Sep", "10 - Oct", "11 - Nov", "12 - Dec"});
			this.cboMonth.SelectedIndexChanged += new System.EventHandler(this.cboMonth_SelectedIndexChanged);
			// 
			// cboYear
			// 
			this.cboYear.BackColor = System.Drawing.SystemColors.Window;
			this.cboYear.CausesValidation = true;
			this.cboYear.Cursor = System.Windows.Forms.Cursors.Default;
			this.cboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboYear.Enabled = true;
			this.cboYear.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cboYear.ForeColor = System.Drawing.SystemColors.WindowText;
			this.cboYear.IntegralHeight = true;
			this.cboYear.Location = new System.Drawing.Point(48, 56);
			this.cboYear.Name = "cboYear";
			this.cboYear.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cboYear.Size = new System.Drawing.Size(73, 21);
			this.cboYear.Sorted = false;
			this.cboYear.TabIndex = 0;
			this.cboYear.TabStop = true;
			this.cboYear.Visible = true;
			this.cboYear.SelectedIndexChanged += new System.EventHandler(this.cboYear_SelectedIndexChanged);
			// 
			// Toolbar1
			// 
			this.Toolbar1.Dock = System.Windows.Forms.DockStyle.Top;
			this.Toolbar1.Location = new System.Drawing.Point(0, 0);
			this.Toolbar1.Name = "Toolbar1";
			this.Toolbar1.ShowItemToolTips = true;
			this.Toolbar1.Size = new System.Drawing.Size(603, 28);
			this.Toolbar1.TabIndex = 13;
			this.Toolbar1.Items.Add(this._Toolbar1_Button1);
			// 
			// _Toolbar1_Button1
			// 
			this._Toolbar1_Button1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
			this._Toolbar1_Button1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._Toolbar1_Button1.Size = new System.Drawing.Size(24, 22);
			this._Toolbar1_Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			// 
			// Label9
			// 
			this.Label9.AutoSize = true;
			this.Label9.BackColor = System.Drawing.SystemColors.Control;
			this.Label9.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label9.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label9.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label9.Location = new System.Drawing.Point(272, 104);
			this.Label9.Name = "Label9";
			this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label9.Size = new System.Drawing.Size(7, 13);
			this.Label9.TabIndex = 10;
			this.Label9.Text = "|";
			// 
			// Label2
			// 
			this.Label2.AutoSize = true;
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label2.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label2.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label2.Location = new System.Drawing.Point(320, 56);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(40, 13);
			this.Label2.TabIndex = 7;
			this.Label2.Text = "Cashier:";
			// 
			// Label5
			// 
			this.Label5.AutoSize = true;
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label5.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label5.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label5.Location = new System.Drawing.Point(8, 56);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(26, 13);
			this.Label5.TabIndex = 6;
			this.Label5.Text = "Year:";
			// 
			// Label8
			// 
			this.Label8.AutoSize = true;
			this.Label8.BackColor = System.Drawing.SystemColors.Control;
			this.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label8.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label8.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label8.Location = new System.Drawing.Point(160, 56);
			this.Label8.Name = "Label8";
			this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label8.Size = new System.Drawing.Size(34, 13);
			this.Label8.TabIndex = 5;
			this.Label8.Text = "Month:";
			// 
			// lblSellable
			// 
			this.lblSellable.AutoSize = true;
			this.lblSellable.BackColor = System.Drawing.SystemColors.Control;
			this.lblSellable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblSellable.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblSellable.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblSellable.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.lblSellable.Location = new System.Drawing.Point(424, 104);
			this.lblSellable.Name = "lblSellable";
			this.lblSellable.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblSellable.Size = new System.Drawing.Size(12, 13);
			this.lblSellable.TabIndex = 4;
			this.lblSellable.Text = "---";
			// 
			// Label4
			// 
			this.Label4.AutoSize = true;
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label4.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label4.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label4.Location = new System.Drawing.Point(296, 104);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(109, 13);
			this.Label4.TabIndex = 3;
			this.Label4.Text = "Most Sellable Item:";
			// 
			// lblTotalSales
			// 
			this.lblTotalSales.AutoSize = true;
			this.lblTotalSales.BackColor = System.Drawing.SystemColors.Control;
			this.lblTotalSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblTotalSales.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblTotalSales.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblTotalSales.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.lblTotalSales.Location = new System.Drawing.Point(168, 104);
			this.lblTotalSales.Name = "lblTotalSales";
			this.lblTotalSales.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblTotalSales.Size = new System.Drawing.Size(12, 13);
			this.lblTotalSales.TabIndex = 2;
			this.lblTotalSales.Text = "---";
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
			this.Label1.Location = new System.Drawing.Point(8, 104);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(145, 13);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Total Sales for the Month:";
			// 
			// frmSales
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(603, 369);
			this.Controls.Add(this.lvwSales);
			this.Controls.Add(this.ctrLine);
			this.Controls.Add(this.cboCashier);
			this.Controls.Add(this.cboMonth);
			this.Controls.Add(this.cboYear);
			this.Controls.Add(this.Toolbar1);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label5);
			this.Controls.Add(this.Label8);
			this.Controls.Add(this.lblSellable);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.lblTotalSales);
			this.Controls.Add(this.Label1);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Icon = (System.Drawing.Icon) resources.GetObject("frmSales.Icon");
			this.Location = new System.Drawing.Point(4, 23);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmSales";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Sales";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.listBoxComboBoxHelper1.SetItemData(this.cboCashier, new int[]{0});
			this.listBoxComboBoxHelper1.SetItemData(this.cboMonth, new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0});
			this.Closed += new System.EventHandler(this.frmSales_Closed);
			this.Load += new System.EventHandler(this.frmSales_Load);
			this.Resize += new System.EventHandler(this.frmSales_Resize);
			((System.ComponentModel.ISupportInitialize) this.listViewHelper1).EndInit();
			((System.ComponentModel.ISupportInitialize) this.listBoxComboBoxHelper1).EndInit();
			this.Toolbar1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		void ReLoadForm(bool addEvents)
		{
			//This form is an MDI child.
			//This code simulates the VB6 
			// functionality of automatically
			// loading and showing an MDI
			// child's parent.
			this.MdiParent = SKS.frmMain.DefInstance;
			SKS.frmMain.DefInstance.Show();
			//The MDI form in the VB6 project had its
			//AutoShowChildren property set to True
			//To simulate the VB6 behavior, we need to
			//automatically Show the form whenever it
			//is loaded.  If you do not want this behavior
			//then delete the following line of code
			//UPGRADE_TODO: (2018) Remove the next line of code to stop form from automatically showing. More Information: http://www.vbtonet.com/ewis/ewi2018.aspx
			this.Show();
		}
		#endregion
	}
}