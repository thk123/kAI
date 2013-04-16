namespace kAI.Editor
{
    partial class Editor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainEditor = new System.Windows.Forms.SplitContainer();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.behaviourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewBehaviourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBehaviourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.MainEditor)).BeginInit();
            this.MainEditor.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainEditor
            // 
            this.MainEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainEditor.Location = new System.Drawing.Point(0, 24);
            this.MainEditor.Name = "MainEditor";
            this.MainEditor.Size = new System.Drawing.Size(1226, 616);
            this.MainEditor.SplitterDistance = 249;
            this.MainEditor.TabIndex = 1;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newProjectToolStripMenuItem.Text = "New Project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // behaviourToolStripMenuItem
            // 
            this.behaviourToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewBehaviourToolStripMenuItem,
            this.addBehaviourToolStripMenuItem});
            this.behaviourToolStripMenuItem.Name = "behaviourToolStripMenuItem";
            this.behaviourToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.behaviourToolStripMenuItem.Text = "Behaviour";
            // 
            // createNewBehaviourToolStripMenuItem
            // 
            this.createNewBehaviourToolStripMenuItem.Name = "createNewBehaviourToolStripMenuItem";
            this.createNewBehaviourToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.createNewBehaviourToolStripMenuItem.Text = "Create New Behaviour";
            // 
            // addBehaviourToolStripMenuItem
            // 
            this.addBehaviourToolStripMenuItem.Name = "addBehaviourToolStripMenuItem";
            this.addBehaviourToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addBehaviourToolStripMenuItem.Text = "Add Behaviour";
            this.addBehaviourToolStripMenuItem.Click += new System.EventHandler(this.addBehaviourToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.behaviourToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1226, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 640);
            this.Controls.Add(this.MainEditor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.Text = "kAI Editor";
            ((System.ComponentModel.ISupportInitialize)(this.MainEditor)).EndInit();
            this.MainEditor.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainEditor;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem behaviourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewBehaviourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addBehaviourToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;

    }
}

