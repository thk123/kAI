namespace kAI.Editor.Controls
{
    partial class BehaviourTree
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
            this.BehaviourTree_Tree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // BehaviourTree_Tree
            // 
            this.BehaviourTree_Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BehaviourTree_Tree.Location = new System.Drawing.Point(0, 0);
            this.BehaviourTree_Tree.Name = "BehaviourTree_Tree";
            this.BehaviourTree_Tree.Size = new System.Drawing.Size(330, 516);
            this.BehaviourTree_Tree.TabIndex = 0;
            // 
            // BehaviourTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.BehaviourTree_Tree);
            this.Name = "BehaviourTree";
            this.Size = new System.Drawing.Size(330, 516);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView BehaviourTree_Tree;
    }
}
