namespace kAI.Editor.Controls.WinForms
{
    partial class DebugControl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.connectDebugBtn = new System.Windows.Forms.ToolStripButton();
            this.disconnectDebugBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDebugBtn,
            this.disconnectDebugBtn,
            this.toolStripSeparator1,
            this.toolStripTextBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(331, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Resize += new System.EventHandler(this.toolStrip1_Resize);
            // 
            // connectDebugBtn
            // 
            this.connectDebugBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.connectDebugBtn.Image = global::kAI.Editor.Properties.Resources.DebugConnectIcon;
            this.connectDebugBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.connectDebugBtn.Name = "connectDebugBtn";
            this.connectDebugBtn.Size = new System.Drawing.Size(23, 22);
            this.connectDebugBtn.Text = "toolStripButton1";
            this.connectDebugBtn.ToolTipText = "Connect";
            this.connectDebugBtn.Click += new System.EventHandler(this.connectDebugBtn_Click);
            // 
            // disconnectDebugBtn
            // 
            this.disconnectDebugBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.disconnectDebugBtn.Enabled = false;
            this.disconnectDebugBtn.Image = global::kAI.Editor.Properties.Resources.DebugDisconnectIcon;
            this.disconnectDebugBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.disconnectDebugBtn.Name = "disconnectDebugBtn";
            this.disconnectDebugBtn.Size = new System.Drawing.Size(23, 22);
            this.disconnectDebugBtn.Text = "toolStripButton2";
            this.disconnectDebugBtn.ToolTipText = "Disconnect debugger";
            this.disconnectDebugBtn.Click += new System.EventHandler(this.disconnectDebugBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Enabled = false;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(150, 25);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Enabled = false;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(331, 330);
            this.treeView1.TabIndex = 1;
            // 
            // DebugControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DebugControl";
            this.Size = new System.Drawing.Size(331, 355);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton connectDebugBtn;
        private System.Windows.Forms.ToolStripButton disconnectDebugBtn;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}
