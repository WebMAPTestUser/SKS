namespace UpgradeHelpers.VB6.Gui
{
    partial class DataHelper
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataHelper));
            this.b_last = new System.Windows.Forms.Button();
            this.b_next = new System.Windows.Forms.Button();
            this.b_prev = new System.Windows.Forms.Button();
            this.b_first = new System.Windows.Forms.Button();
            this.l_caption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // b_last
            // 
            this.b_last.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.b_last.Image = ((System.Drawing.Image)(resources.GetObject("b_last.Image")));
            this.b_last.Location = new System.Drawing.Point(169, 1);
            this.b_last.Margin = new System.Windows.Forms.Padding(0);
            this.b_last.Name = "b_last";
            this.b_last.Size = new System.Drawing.Size(19, 26);
            this.b_last.TabIndex = 4;
            this.b_last.TabStop = false;
            this.b_last.UseVisualStyleBackColor = true;
            this.b_last.Click += new System.EventHandler(this.b_last_Click);
            // 
            // b_next
            // 
            this.b_next.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.b_next.Image = ((System.Drawing.Image)(resources.GetObject("b_next.Image")));
            this.b_next.Location = new System.Drawing.Point(150, 1);
            this.b_next.Margin = new System.Windows.Forms.Padding(0);
            this.b_next.Name = "b_next";
            this.b_next.Size = new System.Drawing.Size(19, 26);
            this.b_next.TabIndex = 3;
            this.b_next.TabStop = false;
            this.b_next.Text = ">";
            this.b_next.UseVisualStyleBackColor = true;
            this.b_next.Click += new System.EventHandler(this.b_next_Click);
            // 
            // b_prev
            // 
            this.b_prev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.b_prev.Image = ((System.Drawing.Image)(resources.GetObject("b_prev.Image")));
            this.b_prev.Location = new System.Drawing.Point(19, 1);
            this.b_prev.Margin = new System.Windows.Forms.Padding(0);
            this.b_prev.Name = "b_prev";
            this.b_prev.Size = new System.Drawing.Size(19, 26);
            this.b_prev.TabIndex = 2;
            this.b_prev.TabStop = false;
            this.b_prev.Text = "<";
            this.b_prev.UseVisualStyleBackColor = true;
            this.b_prev.Click += new System.EventHandler(this.b_prev_Click);
            // 
            // b_first
            // 
            this.b_first.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.b_first.Image = ((System.Drawing.Image)(resources.GetObject("b_first.Image")));
            this.b_first.Location = new System.Drawing.Point(0, 1);
            this.b_first.Margin = new System.Windows.Forms.Padding(0);
            this.b_first.Name = "b_first";
            this.b_first.Size = new System.Drawing.Size(19, 26);
            this.b_first.TabIndex = 1;
            this.b_first.TabStop = false;
            this.b_first.UseVisualStyleBackColor = true;
            this.b_first.Click += new System.EventHandler(this.b_first_Click);
            // 
            // l_caption
            // 
            this.l_caption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_caption.AutoEllipsis = true;
            this.l_caption.BackColor = System.Drawing.Color.White;
            this.l_caption.Location = new System.Drawing.Point(39, 1);
            this.l_caption.Name = "l_caption";
            this.l_caption.Size = new System.Drawing.Size(110, 27);
            this.l_caption.TabIndex = 4;
            this.l_caption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DataHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.l_caption);
            this.Controls.Add(this.b_last);
            this.Controls.Add(this.b_next);
            this.Controls.Add(this.b_prev);
            this.Controls.Add(this.b_first);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DataHelper";
            this.Size = new System.Drawing.Size(192, 32);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button b_last;
        private System.Windows.Forms.Button b_next;
        private System.Windows.Forms.Button b_prev;
        private System.Windows.Forms.Button b_first;
        private System.Windows.Forms.Label l_caption;
    }
}
