namespace kAI.Editor.Controls.WinForms
{
    partial class UILogger
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mMessageBox = new System.Windows.Forms.ListBox();
            this.mCmdTextbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mMessageBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mCmdTextbox);
            this.splitContainer1.Panel2MinSize = 23;
            this.splitContainer1.Size = new System.Drawing.Size(841, 190);
            this.splitContainer1.SplitterDistance = 164;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 3;
            // 
            // mMessageBox
            // 
            this.mMessageBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMessageBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMessageBox.ForeColor = System.Drawing.SystemColors.Control;
            this.mMessageBox.FormattingEnabled = true;
            this.mMessageBox.ItemHeight = 15;
            this.mMessageBox.Location = new System.Drawing.Point(0, 0);
            this.mMessageBox.Name = "mMessageBox";
            this.mMessageBox.Size = new System.Drawing.Size(841, 164);
            this.mMessageBox.TabIndex = 0;
            // 
            // mCmdTextbox
            // 
            this.mCmdTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mCmdTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mCmdTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mCmdTextbox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCmdTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.mCmdTextbox.Location = new System.Drawing.Point(0, 0);
            this.mCmdTextbox.Name = "mCmdTextbox";
            this.mCmdTextbox.Size = new System.Drawing.Size(841, 23);
            this.mCmdTextbox.TabIndex = 1;
            this.mCmdTextbox.Text = " > [enter command or Help() for a list, enter to run]";
            this.mCmdTextbox.Enter += new System.EventHandler(this.mCmdTextbox_Enter);
            this.mCmdTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mCmdTextbox_KeyPress);
            this.mCmdTextbox.Leave += new System.EventHandler(this.mCmdTextbox_Leave);
            // 
            // UILogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UILogger";
            this.Size = new System.Drawing.Size(841, 190);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mFlowPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox mMessageBox;
        internal System.Windows.Forms.TextBox mCmdTextbox;
    }
}
