namespace kAI.Editor.Controls.WinForms
{
    partial class ConstantNodeCreator
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
            this.mIntConstantRadio = new System.Windows.Forms.RadioButton();
            this.mStringConstantRadio = new System.Windows.Forms.RadioButton();
            this.mFloatConstantRadio = new System.Windows.Forms.RadioButton();
            this.mValueTextBox = new System.Windows.Forms.TextBox();
            this.mCreateBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(409, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "Create new function node";
            // 
            // mIntConstantRadio
            // 
            this.mIntConstantRadio.AutoSize = true;
            this.mIntConstantRadio.Location = new System.Drawing.Point(19, 52);
            this.mIntConstantRadio.Name = "mIntConstantRadio";
            this.mIntConstantRadio.Size = new System.Drawing.Size(103, 17);
            this.mIntConstantRadio.TabIndex = 2;
            this.mIntConstantRadio.TabStop = true;
            this.mIntConstantRadio.Text = "Integer Constant";
            this.mIntConstantRadio.UseVisualStyleBackColor = true;
            // 
            // mStringConstantRadio
            // 
            this.mStringConstantRadio.AutoSize = true;
            this.mStringConstantRadio.Location = new System.Drawing.Point(19, 76);
            this.mStringConstantRadio.Name = "mStringConstantRadio";
            this.mStringConstantRadio.Size = new System.Drawing.Size(97, 17);
            this.mStringConstantRadio.TabIndex = 3;
            this.mStringConstantRadio.TabStop = true;
            this.mStringConstantRadio.Text = "String Constant";
            this.mStringConstantRadio.UseVisualStyleBackColor = true;
            // 
            // mFloatConstantRadio
            // 
            this.mFloatConstantRadio.AutoSize = true;
            this.mFloatConstantRadio.Location = new System.Drawing.Point(19, 100);
            this.mFloatConstantRadio.Name = "mFloatConstantRadio";
            this.mFloatConstantRadio.Size = new System.Drawing.Size(93, 17);
            this.mFloatConstantRadio.TabIndex = 4;
            this.mFloatConstantRadio.TabStop = true;
            this.mFloatConstantRadio.Text = "Float Constant";
            this.mFloatConstantRadio.UseVisualStyleBackColor = true;
            // 
            // mValueTextBox
            // 
            this.mValueTextBox.Location = new System.Drawing.Point(19, 124);
            this.mValueTextBox.Name = "mValueTextBox";
            this.mValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.mValueTextBox.TabIndex = 5;
            // 
            // mCreateBtn
            // 
            this.mCreateBtn.Location = new System.Drawing.Point(19, 150);
            this.mCreateBtn.Name = "mCreateBtn";
            this.mCreateBtn.Size = new System.Drawing.Size(75, 23);
            this.mCreateBtn.TabIndex = 6;
            this.mCreateBtn.Text = "Create";
            this.mCreateBtn.UseVisualStyleBackColor = true;
            this.mCreateBtn.Click += new System.EventHandler(this.mCreateBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(324, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "TEMPORARY";
            // 
            // ConstantNodeCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 412);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mCreateBtn);
            this.Controls.Add(this.mValueTextBox);
            this.Controls.Add(this.mFloatConstantRadio);
            this.Controls.Add(this.mStringConstantRadio);
            this.Controls.Add(this.mIntConstantRadio);
            this.Controls.Add(this.label1);
            this.Name = "ConstantNodeCreator";
            this.Text = "ConstantNodeCreator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton mIntConstantRadio;
        private System.Windows.Forms.RadioButton mStringConstantRadio;
        private System.Windows.Forms.RadioButton mFloatConstantRadio;
        private System.Windows.Forms.TextBox mValueTextBox;
        private System.Windows.Forms.Button mCreateBtn;
        private System.Windows.Forms.Label label2;
    }
}