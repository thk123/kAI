namespace kAI.Editor.Controls
{
    partial class DLLEntry
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.DLL_NameLabel = new System.Windows.Forms.Label();
            this.RemoveDLL_Btn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.DLL_NameLabel);
            this.flowLayoutPanel1.Controls.Add(this.RemoveDLL_Btn);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(673, 26);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // DLL_NameLabel
            // 
            this.DLL_NameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.DLL_NameLabel.AutoSize = true;
            this.DLL_NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DLL_NameLabel.Location = new System.Drawing.Point(3, 0);
            this.DLL_NameLabel.Name = "DLL_NameLabel";
            this.DLL_NameLabel.Size = new System.Drawing.Size(60, 29);
            this.DLL_NameLabel.TabIndex = 0;
            this.DLL_NameLabel.Text = "label1";
            // 
            // RemoveDLL_Btn
            // 
            this.RemoveDLL_Btn.Location = new System.Drawing.Point(69, 3);
            this.RemoveDLL_Btn.Name = "RemoveDLL_Btn";
            this.RemoveDLL_Btn.Size = new System.Drawing.Size(27, 23);
            this.RemoveDLL_Btn.TabIndex = 1;
            this.RemoveDLL_Btn.Text = "X";
            this.RemoveDLL_Btn.UseVisualStyleBackColor = true;
            this.RemoveDLL_Btn.Click += new System.EventHandler(this.RemoveDLL_Btn_Click);
            // 
            // DLLEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DLLEntry";
            this.Size = new System.Drawing.Size(673, 26);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label DLL_NameLabel;
        private System.Windows.Forms.Button RemoveDLL_Btn;
    }
}
