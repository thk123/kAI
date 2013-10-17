namespace kAI.Editor.Controls.WinForms
{
    partial class SelectConnexionDialogue
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
            this.mDeleteConnsBtn = new System.Windows.Forms.Button();
            this.mCancelBtn = new System.Windows.Forms.Button();
            this.mConnexionsChecked = new System.Windows.Forms.CheckedListBox();
            this.mPortNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mDeleteConnsBtn
            // 
            this.mDeleteConnsBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mDeleteConnsBtn.Location = new System.Drawing.Point(325, 235);
            this.mDeleteConnsBtn.Name = "mDeleteConnsBtn";
            this.mDeleteConnsBtn.Size = new System.Drawing.Size(106, 23);
            this.mDeleteConnsBtn.TabIndex = 0;
            this.mDeleteConnsBtn.Text = "Delete Connexions";
            this.mDeleteConnsBtn.UseVisualStyleBackColor = true;
            // 
            // mCancelBtn
            // 
            this.mCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelBtn.Location = new System.Drawing.Point(244, 235);
            this.mCancelBtn.Name = "mCancelBtn";
            this.mCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.mCancelBtn.TabIndex = 1;
            this.mCancelBtn.Text = "Cancel";
            this.mCancelBtn.UseVisualStyleBackColor = true;
            // 
            // mConnexionsChecked
            // 
            this.mConnexionsChecked.FormattingEnabled = true;
            this.mConnexionsChecked.Location = new System.Drawing.Point(12, 41);
            this.mConnexionsChecked.Name = "mConnexionsChecked";
            this.mConnexionsChecked.Size = new System.Drawing.Size(418, 184);
            this.mConnexionsChecked.TabIndex = 2;
            // 
            // mPortNameLabel
            // 
            this.mPortNameLabel.AutoSize = true;
            this.mPortNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPortNameLabel.Location = new System.Drawing.Point(12, 9);
            this.mPortNameLabel.Name = "mPortNameLabel";
            this.mPortNameLabel.Size = new System.Drawing.Size(113, 25);
            this.mPortNameLabel.TabIndex = 3;
            this.mPortNameLabel.Text = "Port:Name";
            // 
            // SelectConnexionDialogue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 270);
            this.Controls.Add(this.mPortNameLabel);
            this.Controls.Add(this.mConnexionsChecked);
            this.Controls.Add(this.mCancelBtn);
            this.Controls.Add(this.mDeleteConnsBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectConnexionDialogue";
            this.Text = "Select Connexion to Delete...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mDeleteConnsBtn;
        private System.Windows.Forms.Button mCancelBtn;
        private System.Windows.Forms.CheckedListBox mConnexionsChecked;
        private System.Windows.Forms.Label mPortNameLabel;
    }
}