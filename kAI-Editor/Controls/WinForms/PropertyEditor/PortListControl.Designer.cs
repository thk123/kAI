namespace kAI.Editor.Controls.WinForms
{
    partial class PortListControl
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
            this.mAddBtn = new System.Windows.Forms.Button();
            this.mRemoveBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mAddBtn
            // 
            this.mAddBtn.Location = new System.Drawing.Point(4, 15);
            this.mAddBtn.Name = "mAddBtn";
            this.mAddBtn.Size = new System.Drawing.Size(75, 23);
            this.mAddBtn.TabIndex = 0;
            this.mAddBtn.Text = "Add";
            this.mAddBtn.UseVisualStyleBackColor = true;
            // 
            // mRemoveBtn
            // 
            this.mRemoveBtn.Location = new System.Drawing.Point(202, 15);
            this.mRemoveBtn.Name = "mRemoveBtn";
            this.mRemoveBtn.Size = new System.Drawing.Size(75, 23);
            this.mRemoveBtn.TabIndex = 1;
            this.mRemoveBtn.Text = "Remove";
            this.mRemoveBtn.UseVisualStyleBackColor = true;
            // 
            // PortListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mRemoveBtn);
            this.Controls.Add(this.mAddBtn);
            this.Name = "PortListControl";
            this.Size = new System.Drawing.Size(280, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mAddBtn;
        private System.Windows.Forms.Button mRemoveBtn;
    }
}
