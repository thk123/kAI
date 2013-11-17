namespace kAI.Editor.Controls.WinForms
{
    partial class FunctionNodeCreator
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
            this.lSearchableList = new kAI.Editor.Controls.WinForms.SearchableList();
            this.mFunctionList = new kAI.Editor.Controls.WinForms.SearchableList();
            this.mInParamsGroup = new System.Windows.Forms.GroupBox();
            this.mOutParamsGroup = new System.Windows.Forms.GroupBox();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.mCreateBtn = new System.Windows.Forms.Button();
            this.mInParamsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(409, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Create new function node";
            // 
            // lSearchableList
            // 
            this.lSearchableList.Location = new System.Drawing.Point(0, 0);
            this.lSearchableList.Name = "lSearchableList";
            this.lSearchableList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.lSearchableList.Size = new System.Drawing.Size(294, 258);
            this.lSearchableList.TabIndex = 0;
            // 
            // mFunctionList
            // 
            this.mFunctionList.Location = new System.Drawing.Point(19, 51);
            this.mFunctionList.Name = "mFunctionList";
            this.mFunctionList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.mFunctionList.Size = new System.Drawing.Size(298, 258);
            this.mFunctionList.TabIndex = 1;
            // 
            // mInParamsGroup
            // 
            this.mInParamsGroup.Controls.Add(this.flowLayoutPanel1);
            this.mInParamsGroup.Location = new System.Drawing.Point(324, 52);
            this.mInParamsGroup.Name = "mInParamsGroup";
            this.mInParamsGroup.Size = new System.Drawing.Size(362, 257);
            this.mInParamsGroup.TabIndex = 2;
            this.mInParamsGroup.TabStop = false;
            this.mInParamsGroup.Text = "In Parameters";
            // 
            // mOutParamsGroup
            // 
            this.mOutParamsGroup.Location = new System.Drawing.Point(324, 316);
            this.mOutParamsGroup.Name = "mOutParamsGroup";
            this.mOutParamsGroup.Size = new System.Drawing.Size(362, 242);
            this.mOutParamsGroup.TabIndex = 3;
            this.mOutParamsGroup.TabStop = false;
            this.mOutParamsGroup.Text = "Out Data";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(356, 231);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // mCreateBtn
            // 
            this.mCreateBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCreateBtn.Enabled = false;
            this.mCreateBtn.Location = new System.Drawing.Point(611, 564);
            this.mCreateBtn.Name = "mCreateBtn";
            this.mCreateBtn.Size = new System.Drawing.Size(75, 23);
            this.mCreateBtn.TabIndex = 4;
            this.mCreateBtn.Text = "Create";
            this.mCreateBtn.UseVisualStyleBackColor = true;
            this.mCreateBtn.Click += new System.EventHandler(this.mCreateBtn_Click);
            // 
            // FunctionNodeCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 600);
            this.Controls.Add(this.mCreateBtn);
            this.Controls.Add(this.mOutParamsGroup);
            this.Controls.Add(this.mInParamsGroup);
            this.Controls.Add(this.mFunctionList);
            this.Controls.Add(this.label1);
            this.Name = "FunctionNodeCreator";
            this.Text = "FunctionNodeCreator";
            this.mInParamsGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private kAI.Editor.Controls.WinForms.SearchableList lSearchableList;
        private SearchableList mFunctionList;
        private System.Windows.Forms.GroupBox mInParamsGroup;
        private System.Windows.Forms.GroupBox mOutParamsGroup;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.ServiceProcess.ServiceController serviceController1;
        private System.Windows.Forms.Button mCreateBtn;
    }
}