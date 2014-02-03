namespace kAI.Editor.Controls.WinForms
{
    partial class SwitchNodeCreator
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
            this.mSearchableList = new kAI.Editor.Controls.WinForms.SearchableList();
            this.mCreateBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mSearchableList
            // 
            this.mSearchableList.Location = new System.Drawing.Point(12, 12);
            this.mSearchableList.Name = "mSearchableList";
            this.mSearchableList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.mSearchableList.Size = new System.Drawing.Size(294, 421);
            this.mSearchableList.TabIndex = 0;
            // 
            // mCreateBtn
            // 
            this.mCreateBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCreateBtn.Location = new System.Drawing.Point(231, 425);
            this.mCreateBtn.Name = "mCreateBtn";
            this.mCreateBtn.Size = new System.Drawing.Size(75, 23);
            this.mCreateBtn.TabIndex = 1;
            this.mCreateBtn.Text = "Create";
            this.mCreateBtn.UseVisualStyleBackColor = true;
            this.mCreateBtn.Click += new System.EventHandler(this.mCreateBtn_Click);
            // 
            // SwitchNodeCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 470);
            this.Controls.Add(this.mCreateBtn);
            this.Controls.Add(this.mSearchableList);
            this.Name = "SwitchNodeCreator";
            this.Text = "SwitchNodeCreator";
            this.ResumeLayout(false);

        }

        #endregion

        private SearchableList mSearchableList;
        private System.Windows.Forms.Button mCreateBtn;
    }
}