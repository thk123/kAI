namespace kAI.Editor.Controls.WinForms.PropertyEditor
{
    partial class PortPropertiesEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.mPortIDTextBox = new System.Windows.Forms.TextBox();
            this.mPortIDLabel = new System.Windows.Forms.Label();
            this.mPortDirectionLabel = new System.Windows.Forms.Label();
            this.mPortDirectionDropdown = new System.Windows.Forms.ComboBox();
            this.mPortTypeDropdown = new System.Windows.Forms.ComboBox();
            this.mPortTypeLabel = new System.Windows.Forms.Label();
            this.mConfirmBtn = new System.Windows.Forms.Button();
            this.mCancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port Creator";
            // 
            // mPortIDTextBox
            // 
            this.mPortIDTextBox.Location = new System.Drawing.Point(96, 64);
            this.mPortIDTextBox.Name = "mPortIDTextBox";
            this.mPortIDTextBox.Size = new System.Drawing.Size(260, 20);
            this.mPortIDTextBox.TabIndex = 1;
            // 
            // mPortIDLabel
            // 
            this.mPortIDLabel.AutoSize = true;
            this.mPortIDLabel.Location = new System.Drawing.Point(16, 67);
            this.mPortIDLabel.Name = "mPortIDLabel";
            this.mPortIDLabel.Size = new System.Drawing.Size(43, 13);
            this.mPortIDLabel.TabIndex = 2;
            this.mPortIDLabel.Text = "Port ID:";
            // 
            // mPortDirectionLabel
            // 
            this.mPortDirectionLabel.AutoSize = true;
            this.mPortDirectionLabel.Location = new System.Drawing.Point(16, 99);
            this.mPortDirectionLabel.Name = "mPortDirectionLabel";
            this.mPortDirectionLabel.Size = new System.Drawing.Size(74, 13);
            this.mPortDirectionLabel.TabIndex = 3;
            this.mPortDirectionLabel.Text = "Port Direction:";
            // 
            // mPortDirectionDropdown
            // 
            this.mPortDirectionDropdown.FormattingEnabled = true;
            this.mPortDirectionDropdown.Location = new System.Drawing.Point(96, 96);
            this.mPortDirectionDropdown.Name = "mPortDirectionDropdown";
            this.mPortDirectionDropdown.Size = new System.Drawing.Size(260, 21);
            this.mPortDirectionDropdown.TabIndex = 4;
            // 
            // mPortTypeDropdown
            // 
            this.mPortTypeDropdown.FormattingEnabled = true;
            this.mPortTypeDropdown.Location = new System.Drawing.Point(96, 130);
            this.mPortTypeDropdown.Name = "mPortTypeDropdown";
            this.mPortTypeDropdown.Size = new System.Drawing.Size(260, 21);
            this.mPortTypeDropdown.TabIndex = 6;
            // 
            // mPortTypeLabel
            // 
            this.mPortTypeLabel.AutoSize = true;
            this.mPortTypeLabel.Location = new System.Drawing.Point(16, 133);
            this.mPortTypeLabel.Name = "mPortTypeLabel";
            this.mPortTypeLabel.Size = new System.Drawing.Size(56, 13);
            this.mPortTypeLabel.TabIndex = 5;
            this.mPortTypeLabel.Text = "Port Type:";
            // 
            // mConfirmBtn
            // 
            this.mConfirmBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mConfirmBtn.Location = new System.Drawing.Point(281, 175);
            this.mConfirmBtn.Name = "mConfirmBtn";
            this.mConfirmBtn.Size = new System.Drawing.Size(75, 23);
            this.mConfirmBtn.TabIndex = 7;
            this.mConfirmBtn.Text = "Create";
            this.mConfirmBtn.UseVisualStyleBackColor = true;
            this.mConfirmBtn.Click += new System.EventHandler(this.mConfirmBtn_Click);
            // 
            // mCancelBtn
            // 
            this.mCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelBtn.Location = new System.Drawing.Point(200, 175);
            this.mCancelBtn.Name = "mCancelBtn";
            this.mCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.mCancelBtn.TabIndex = 8;
            this.mCancelBtn.Text = "Cancel";
            this.mCancelBtn.UseVisualStyleBackColor = true;
            this.mCancelBtn.Click += new System.EventHandler(this.mCancelBtn_Click);
            // 
            // PortPropertiesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 218);
            this.Controls.Add(this.mCancelBtn);
            this.Controls.Add(this.mConfirmBtn);
            this.Controls.Add(this.mPortTypeDropdown);
            this.Controls.Add(this.mPortTypeLabel);
            this.Controls.Add(this.mPortDirectionDropdown);
            this.Controls.Add(this.mPortDirectionLabel);
            this.Controls.Add(this.mPortIDLabel);
            this.Controls.Add(this.mPortIDTextBox);
            this.Controls.Add(this.label1);
            this.Name = "PortPropertiesEditor";
            this.Text = "PortPropertiesEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mPortIDTextBox;
        private System.Windows.Forms.Label mPortIDLabel;
        private System.Windows.Forms.Label mPortDirectionLabel;
        private System.Windows.Forms.ComboBox mPortDirectionDropdown;
        private System.Windows.Forms.ComboBox mPortTypeDropdown;
        private System.Windows.Forms.Label mPortTypeLabel;
        private System.Windows.Forms.Button mConfirmBtn;
        private System.Windows.Forms.Button mCancelBtn;
    }
}