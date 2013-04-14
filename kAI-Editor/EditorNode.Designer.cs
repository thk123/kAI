namespace kAI.Editor
{
    partial class kAIEditorNode
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
            this.MainNode = new System.Windows.Forms.Panel();
            this.BehaviourName = new System.Windows.Forms.Label();
            this.MainNode.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainNode
            // 
            this.MainNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.MainNode.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.MainNode.Controls.Add(this.BehaviourName);
            this.MainNode.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.MainNode.Location = new System.Drawing.Point(16, 0);
            this.MainNode.Name = "MainNode";
            this.MainNode.Size = new System.Drawing.Size(150, 150);
            this.MainNode.TabIndex = 0;
            this.MainNode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.kAIEditorNode_MouseDown);
            this.MainNode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.kAIEditorNode_MouseMove);
            // 
            // BehaviourName
            // 
            this.BehaviourName.AutoSize = true;
            this.BehaviourName.Location = new System.Drawing.Point(4, 4);
            this.BehaviourName.Name = "BehaviourName";
            this.BehaviourName.Size = new System.Drawing.Size(28, 13);
            this.BehaviourName.TabIndex = 0;
            this.BehaviourName.Text = "Test";
            // 
            // kAIEditorNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.MainNode);
            this.Name = "kAIEditorNode";
            this.Size = new System.Drawing.Size(182, 150);
            this.MainNode.ResumeLayout(false);
            this.MainNode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainNode;
        internal System.Windows.Forms.Label BehaviourName;

    }
}
