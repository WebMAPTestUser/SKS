namespace UpgradeHelpers.VB6.Gui
{
    partial class MSDataGridHelperDesignerModalView
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
            this.trvLayout = new System.Windows.Forms.TreeView();
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdUp = new System.Windows.Forms.Button();
            this.cmdDown = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.lblLayout = new System.Windows.Forms.Label();
            this.lblProperties = new System.Windows.Forms.Label();
            this.cmdAddSplit = new System.Windows.Forms.Button();
            this.cmdAddColumn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trvLayout
            // 
            this.trvLayout.HideSelection = false;
            this.trvLayout.Location = new System.Drawing.Point(12, 28);
            this.trvLayout.Name = "trvLayout";
            this.trvLayout.Size = new System.Drawing.Size(266, 266);
            this.trvLayout.TabIndex = 1;
            this.trvLayout.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvLayout_AfterSelect);
            this.trvLayout.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trvLayout_KeyUp);
            // 
            // propGrid
            // 
            this.propGrid.Location = new System.Drawing.Point(340, 28);
            this.propGrid.Name = "propGrid";
            this.propGrid.Size = new System.Drawing.Size(268, 295);
            this.propGrid.TabIndex = 8;
            this.propGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propGrid_PropertyValueChanged);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(533, 332);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(74, 20);
            this.cmdCancel.TabIndex = 10;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOk
            // 
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(453, 332);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(74, 20);
            this.cmdOk.TabIndex = 9;
            this.cmdOk.Text = "Ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            // 
            // cmdUp
            // 
            this.cmdUp.Enabled = false;
            this.cmdUp.Location = new System.Drawing.Point(283, 28);
            this.cmdUp.Name = "cmdUp";
            this.cmdUp.Size = new System.Drawing.Size(48, 21);
            this.cmdUp.TabIndex = 4;
            this.cmdUp.Text = "Up";
            this.cmdUp.UseVisualStyleBackColor = true;
            // 
            // cmdDown
            // 
            this.cmdDown.Enabled = false;
            this.cmdDown.Location = new System.Drawing.Point(284, 55);
            this.cmdDown.Name = "cmdDown";
            this.cmdDown.Size = new System.Drawing.Size(47, 21);
            this.cmdDown.TabIndex = 5;
            this.cmdDown.Text = "Down";
            this.cmdDown.UseVisualStyleBackColor = true;
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(284, 82);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(47, 21);
            this.cmdDelete.TabIndex = 6;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // lblLayout
            // 
            this.lblLayout.Location = new System.Drawing.Point(13, 10);
            this.lblLayout.Name = "lblLayout";
            this.lblLayout.Size = new System.Drawing.Size(264, 18);
            this.lblLayout.TabIndex = 0;
            this.lblLayout.Text = "Edit the splits and columns:";
            // 
            // lblProperties
            // 
            this.lblProperties.Location = new System.Drawing.Point(337, 10);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(264, 18);
            this.lblProperties.TabIndex = 7;
            this.lblProperties.Text = "Properties:";
            // 
            // cmdAddSplit
            // 
            this.cmdAddSplit.Location = new System.Drawing.Point(12, 300);
            this.cmdAddSplit.Name = "cmdAddSplit";
            this.cmdAddSplit.Size = new System.Drawing.Size(128, 23);
            this.cmdAddSplit.TabIndex = 2;
            this.cmdAddSplit.Text = "Add a &Split";
            this.cmdAddSplit.UseVisualStyleBackColor = true;
            this.cmdAddSplit.Click += new System.EventHandler(this.cmdAddSplit_Click);
            // 
            // cmdAddColumn
            // 
            this.cmdAddColumn.Location = new System.Drawing.Point(150, 300);
            this.cmdAddColumn.Name = "cmdAddColumn";
            this.cmdAddColumn.Size = new System.Drawing.Size(128, 23);
            this.cmdAddColumn.TabIndex = 3;
            this.cmdAddColumn.Text = "Add a &Column";
            this.cmdAddColumn.UseVisualStyleBackColor = true;
            this.cmdAddColumn.Click += new System.EventHandler(this.cmdAddColumn_Click);
            // 
            // MSDataGridHelperDesignerModalView
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(620, 369);
            this.Controls.Add(this.cmdAddColumn);
            this.Controls.Add(this.cmdAddSplit);
            this.Controls.Add(this.lblProperties);
            this.Controls.Add(this.lblLayout);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdDown);
            this.Controls.Add(this.cmdUp);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.propGrid);
            this.Controls.Add(this.trvLayout);
            this.Name = "MSDataGridHelperDesignerModalView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MSDataGrid Layout Editor";
            this.Load += new System.EventHandler(this.MSDataGridHelperDesignerModalView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trvLayout;
        private System.Windows.Forms.PropertyGrid propGrid;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdUp;
        private System.Windows.Forms.Button cmdDown;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Label lblLayout;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.Button cmdAddSplit;
        private System.Windows.Forms.Button cmdAddColumn;
    }
}