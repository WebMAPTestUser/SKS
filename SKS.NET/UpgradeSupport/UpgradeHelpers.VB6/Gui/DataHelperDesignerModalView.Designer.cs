namespace UpgradeHelpers.VB6.Gui
{
    partial class DataHelperDesignerModalView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.dbGridProperties = new System.Windows.Forms.DataGridView();
            this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.DHComboBox = new System.Windows.Forms.ComboBox();
            this.cmdClean = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dbGridProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Enabled = false;
            this.cmdOk.Location = new System.Drawing.Point(56, 175);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(70, 22);
            this.cmdOk.TabIndex = 0;
            this.cmdOk.Text = "Ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(132, 175);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(70, 22);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // dbGridProperties
            // 
            this.dbGridProperties.AllowUserToAddRows = false;
            this.dbGridProperties.AllowUserToDeleteRows = false;
            this.dbGridProperties.AllowUserToResizeRows = false;
            this.dbGridProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dbGridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dbGridProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Property,
            this.Value});
            this.dbGridProperties.Location = new System.Drawing.Point(8, 42);
            this.dbGridProperties.MultiSelect = false;
            this.dbGridProperties.Name = "dbGridProperties";
            this.dbGridProperties.RowHeadersVisible = false;
            this.dbGridProperties.Size = new System.Drawing.Size(270, 127);
            this.dbGridProperties.TabIndex = 2;
            this.dbGridProperties.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dbGridProperties_CellValueChanged);
            this.dbGridProperties.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dbGridProperties_KeyPress);
            // 
            // Property
            // 
            this.Property.HeaderText = "Property";
            this.Property.Name = "Property";
            this.Property.ReadOnly = true;
            this.Property.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Property.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Property.Width = 114;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "DataHelper";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DHComboBox
            // 
            this.DHComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DHComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DHComboBox.FormattingEnabled = true;
            this.DHComboBox.Location = new System.Drawing.Point(121, 18);
            this.DHComboBox.Name = "DHComboBox";
            this.DHComboBox.Size = new System.Drawing.Size(156, 21);
            this.DHComboBox.TabIndex = 4;
            this.DHComboBox.SelectedIndexChanged += new System.EventHandler(this.DHComboBox_SelectedIndexChanged);
            // 
            // cmdClean
            // 
            this.cmdClean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClean.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdClean.Location = new System.Drawing.Point(208, 175);
            this.cmdClean.Name = "cmdClean";
            this.cmdClean.Size = new System.Drawing.Size(70, 22);
            this.cmdClean.TabIndex = 5;
            this.cmdClean.Text = "Clean";
            this.cmdClean.UseVisualStyleBackColor = true;
            this.cmdClean.Click += new System.EventHandler(this.cmdClean_Click);
            // 
            // DataHelperDesignerModalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 203);
            this.Controls.Add(this.cmdClean);
            this.Controls.Add(this.DHComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dbGridProperties);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DataHelperDesignerModalView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DataHelper binding property";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataHelperDesignerModalView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dbGridProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.DataGridView dbGridProperties;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DHComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button cmdClean;
    }
}