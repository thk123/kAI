namespace kAI.Editor
{
    partial class BehaviourChooser
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
            this.BehavioursList = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BehavioursList
            // 
            this.BehavioursList.FormattingEnabled = true;
            this.BehavioursList.Location = new System.Drawing.Point(12, 40);
            this.BehavioursList.Name = "BehavioursList";
            this.BehavioursList.Size = new System.Drawing.Size(756, 628);
            this.BehavioursList.TabIndex = 0;
            this.BehavioursList.DoubleClick += new System.EventHandler(this.BehavioursList_DoubleClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(756, 20);
            this.textBox1.TabIndex = 1;
            // 
            // BehaviourChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 676);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BehavioursList);
            this.Name = "BehaviourChooser";
            this.Text = "BehaviourChooser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox BehavioursList;
        private System.Windows.Forms.TextBox textBox1;
    }
}