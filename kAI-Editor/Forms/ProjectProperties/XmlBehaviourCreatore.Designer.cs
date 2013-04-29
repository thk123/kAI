namespace kAI.Editor.Forms.ProjectProperties
{
    partial class XmlBehaviourCreator
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BehaviourName_Text = new System.Windows.Forms.TextBox();
            this.BehaviourLocation_Text = new System.Windows.Forms.TextBox();
            this.BehaviourLocation_Btn = new System.Windows.Forms.Button();
            this.Create_Btn = new System.Windows.Forms.Button();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(528, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "New kAI Xml Behaviour";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Behaviour Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Behaviour Location:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 324);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "TODO: Add external ports";
            // 
            // BehaviourName_Text
            // 
            this.BehaviourName_Text.Location = new System.Drawing.Point(147, 68);
            this.BehaviourName_Text.Name = "BehaviourName_Text";
            this.BehaviourName_Text.Size = new System.Drawing.Size(393, 20);
            this.BehaviourName_Text.TabIndex = 4;
            this.BehaviourName_Text.TextChanged += new System.EventHandler(this.BehaviourName_Text_TextChanged);
            // 
            // BehaviourLocation_Text
            // 
            this.BehaviourLocation_Text.Location = new System.Drawing.Point(147, 94);
            this.BehaviourLocation_Text.Name = "BehaviourLocation_Text";
            this.BehaviourLocation_Text.Size = new System.Drawing.Size(393, 20);
            this.BehaviourLocation_Text.TabIndex = 5;
            this.BehaviourLocation_Text.DoubleClick += new System.EventHandler(this.BehaviourLocation_Browse);
            // 
            // BehaviourLocation_Btn
            // 
            this.BehaviourLocation_Btn.Location = new System.Drawing.Point(546, 92);
            this.BehaviourLocation_Btn.Name = "BehaviourLocation_Btn";
            this.BehaviourLocation_Btn.Size = new System.Drawing.Size(76, 23);
            this.BehaviourLocation_Btn.TabIndex = 6;
            this.BehaviourLocation_Btn.Text = "Browse";
            this.BehaviourLocation_Btn.UseVisualStyleBackColor = true;
            // 
            // Create_Btn
            // 
            this.Create_Btn.Location = new System.Drawing.Point(546, 553);
            this.Create_Btn.Name = "Create_Btn";
            this.Create_Btn.Size = new System.Drawing.Size(75, 23);
            this.Create_Btn.TabIndex = 7;
            this.Create_Btn.Text = "Create";
            this.Create_Btn.UseVisualStyleBackColor = true;
            this.Create_Btn.Click += new System.EventHandler(this.Create_Btn_Click);
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Btn.Location = new System.Drawing.Point(465, 553);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Btn.TabIndex = 8;
            this.Cancel_Btn.Text = "Cancel";
            this.Cancel_Btn.UseVisualStyleBackColor = true;
            // 
            // XmlBehaviourCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 588);
            this.Controls.Add(this.Cancel_Btn);
            this.Controls.Add(this.Create_Btn);
            this.Controls.Add(this.BehaviourLocation_Btn);
            this.Controls.Add(this.BehaviourLocation_Text);
            this.Controls.Add(this.BehaviourName_Text);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "XmlBehaviourCreator";
            this.Text = "XmlBehaviourCreatore";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox BehaviourName_Text;
        private System.Windows.Forms.TextBox BehaviourLocation_Text;
        private System.Windows.Forms.Button BehaviourLocation_Btn;
        private System.Windows.Forms.Button Create_Btn;
        private System.Windows.Forms.Button Cancel_Btn;
    }
}