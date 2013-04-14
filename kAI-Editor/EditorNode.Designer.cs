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
            this.NodeName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NodeName
            // 
            this.NodeName.AutoSize = true;
            this.NodeName.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.NodeName.Location = new System.Drawing.Point(4, 4);
            this.NodeName.Name = "NodeName";
            this.NodeName.Size = new System.Drawing.Size(75, 13);
            this.NodeName.TabIndex = 0;
            this.NodeName.Text = "NODE_NAME";
            // 
            // kAIEditorNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.NodeName);
            this.Name = "kAIEditorNode";
            this.Size = new System.Drawing.Size(192, 79);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.kAIEditorNode_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.kAIEditorNode_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NodeName;
    }
}
