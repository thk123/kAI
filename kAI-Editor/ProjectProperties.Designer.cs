namespace kAI.Editor
{
    partial class ProjectProperties
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
            this.ProjectProperties_TabControl = new System.Windows.Forms.TabControl();
            this.TabPage_General = new System.Windows.Forms.TabPage();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.Confirm_Btn = new System.Windows.Forms.Button();
            this.BehaviourDir_BrowseBtn = new System.Windows.Forms.Button();
            this.BehaviuorDir_TextBox = new System.Windows.Forms.TextBox();
            this.ProjectDir_BrowseBtn = new System.Windows.Forms.Button();
            this.ProjectDir_TextBox = new System.Windows.Forms.TextBox();
            this.ProjectName_TextBox = new System.Windows.Forms.TextBox();
            this.BehaviourDir_Label = new System.Windows.Forms.Label();
            this.ProjectDir_Label = new System.Windows.Forms.Label();
            this.ProjectName_Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TabPage_DLL = new System.Windows.Forms.TabPage();
            this.TabPage_Types = new System.Windows.Forms.TabPage();
            this.TabPage_Actions = new System.Windows.Forms.TabPage();
            this.TabPage_Behaviours = new System.Windows.Forms.TabPage();
            this.ProjectProperties_TabControl.SuspendLayout();
            this.TabPage_General.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectProperties_TabControl
            // 
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_General);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_DLL);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Types);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Actions);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Behaviours);
            this.ProjectProperties_TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProjectProperties_TabControl.Location = new System.Drawing.Point(0, 0);
            this.ProjectProperties_TabControl.Name = "ProjectProperties_TabControl";
            this.ProjectProperties_TabControl.SelectedIndex = 0;
            this.ProjectProperties_TabControl.Size = new System.Drawing.Size(969, 747);
            this.ProjectProperties_TabControl.TabIndex = 0;
            // 
            // TabPage_General
            // 
            this.TabPage_General.Controls.Add(this.Cancel_Btn);
            this.TabPage_General.Controls.Add(this.Confirm_Btn);
            this.TabPage_General.Controls.Add(this.BehaviourDir_BrowseBtn);
            this.TabPage_General.Controls.Add(this.BehaviuorDir_TextBox);
            this.TabPage_General.Controls.Add(this.ProjectDir_BrowseBtn);
            this.TabPage_General.Controls.Add(this.ProjectDir_TextBox);
            this.TabPage_General.Controls.Add(this.ProjectName_TextBox);
            this.TabPage_General.Controls.Add(this.BehaviourDir_Label);
            this.TabPage_General.Controls.Add(this.ProjectDir_Label);
            this.TabPage_General.Controls.Add(this.ProjectName_Label);
            this.TabPage_General.Controls.Add(this.label1);
            this.TabPage_General.Location = new System.Drawing.Point(4, 22);
            this.TabPage_General.Name = "TabPage_General";
            this.TabPage_General.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_General.Size = new System.Drawing.Size(961, 721);
            this.TabPage_General.TabIndex = 0;
            this.TabPage_General.Text = "General";
            this.TabPage_General.UseVisualStyleBackColor = true;
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Btn.Location = new System.Drawing.Point(671, 690);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(138, 23);
            this.Cancel_Btn.TabIndex = 11;
            this.Cancel_Btn.Text = "Cancel";
            this.Cancel_Btn.UseVisualStyleBackColor = true;
            // 
            // Confirm_Btn
            // 
            this.Confirm_Btn.Location = new System.Drawing.Point(815, 690);
            this.Confirm_Btn.Name = "Confirm_Btn";
            this.Confirm_Btn.Size = new System.Drawing.Size(138, 23);
            this.Confirm_Btn.TabIndex = 10;
            this.Confirm_Btn.Text = "Confirm";
            this.Confirm_Btn.UseVisualStyleBackColor = true;
            this.Confirm_Btn.Click += new System.EventHandler(this.Confirm_Btn_Click);
            // 
            // BehaviourDir_BrowseBtn
            // 
            this.BehaviourDir_BrowseBtn.Location = new System.Drawing.Point(858, 144);
            this.BehaviourDir_BrowseBtn.Name = "BehaviourDir_BrowseBtn";
            this.BehaviourDir_BrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.BehaviourDir_BrowseBtn.TabIndex = 9;
            this.BehaviourDir_BrowseBtn.Text = "Browse";
            this.BehaviourDir_BrowseBtn.UseVisualStyleBackColor = true;
            this.BehaviourDir_BrowseBtn.Click += new System.EventHandler(this.BehaviourDir_Browse);
            // 
            // BehaviuorDir_TextBox
            // 
            this.BehaviuorDir_TextBox.Location = new System.Drawing.Point(308, 146);
            this.BehaviuorDir_TextBox.Name = "BehaviuorDir_TextBox";
            this.BehaviuorDir_TextBox.Size = new System.Drawing.Size(544, 20);
            this.BehaviuorDir_TextBox.TabIndex = 8;
            this.BehaviuorDir_TextBox.DoubleClick += new System.EventHandler(this.BehaviourDir_Browse);
            // 
            // ProjectDir_BrowseBtn
            // 
            this.ProjectDir_BrowseBtn.Location = new System.Drawing.Point(858, 113);
            this.ProjectDir_BrowseBtn.Name = "ProjectDir_BrowseBtn";
            this.ProjectDir_BrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.ProjectDir_BrowseBtn.TabIndex = 7;
            this.ProjectDir_BrowseBtn.Text = "Browse";
            this.ProjectDir_BrowseBtn.UseVisualStyleBackColor = true;
            this.ProjectDir_BrowseBtn.Click += new System.EventHandler(this.ProjectDir_Browse);
            // 
            // ProjectDir_TextBox
            // 
            this.ProjectDir_TextBox.Location = new System.Drawing.Point(308, 115);
            this.ProjectDir_TextBox.Name = "ProjectDir_TextBox";
            this.ProjectDir_TextBox.Size = new System.Drawing.Size(544, 20);
            this.ProjectDir_TextBox.TabIndex = 6;
            this.ProjectDir_TextBox.DoubleClick += new System.EventHandler(this.ProjectDir_Browse);
            // 
            // ProjectName_TextBox
            // 
            this.ProjectName_TextBox.Location = new System.Drawing.Point(308, 83);
            this.ProjectName_TextBox.Name = "ProjectName_TextBox";
            this.ProjectName_TextBox.Size = new System.Drawing.Size(625, 20);
            this.ProjectName_TextBox.TabIndex = 5;
            // 
            // BehaviourDir_Label
            // 
            this.BehaviourDir_Label.AutoSize = true;
            this.BehaviourDir_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BehaviourDir_Label.Location = new System.Drawing.Point(48, 141);
            this.BehaviourDir_Label.Name = "BehaviourDir_Label";
            this.BehaviourDir_Label.Size = new System.Drawing.Size(179, 24);
            this.BehaviourDir_Label.TabIndex = 4;
            this.BehaviourDir_Label.Text = "Behaviour Directory:";
            // 
            // ProjectDir_Label
            // 
            this.ProjectDir_Label.AutoSize = true;
            this.ProjectDir_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectDir_Label.Location = new System.Drawing.Point(48, 112);
            this.ProjectDir_Label.Name = "ProjectDir_Label";
            this.ProjectDir_Label.Size = new System.Drawing.Size(152, 24);
            this.ProjectDir_Label.TabIndex = 3;
            this.ProjectDir_Label.Text = "Project Directory:";
            // 
            // ProjectName_Label
            // 
            this.ProjectName_Label.AutoSize = true;
            this.ProjectName_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectName_Label.Location = new System.Drawing.Point(48, 83);
            this.ProjectName_Label.Name = "ProjectName_Label";
            this.ProjectName_Label.Size = new System.Drawing.Size(129, 24);
            this.ProjectName_Label.TabIndex = 2;
            this.ProjectName_Label.Text = "Project Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(405, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project Properties";
            // 
            // TabPage_DLL
            // 
            this.TabPage_DLL.Location = new System.Drawing.Point(4, 22);
            this.TabPage_DLL.Name = "TabPage_DLL";
            this.TabPage_DLL.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_DLL.Size = new System.Drawing.Size(961, 721);
            this.TabPage_DLL.TabIndex = 1;
            this.TabPage_DLL.Text = "DLL\'s";
            this.TabPage_DLL.UseVisualStyleBackColor = true;
            // 
            // TabPage_Types
            // 
            this.TabPage_Types.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Types.Name = "TabPage_Types";
            this.TabPage_Types.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Types.Size = new System.Drawing.Size(961, 721);
            this.TabPage_Types.TabIndex = 2;
            this.TabPage_Types.Text = "Types";
            this.TabPage_Types.UseVisualStyleBackColor = true;
            // 
            // TabPage_Actions
            // 
            this.TabPage_Actions.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Actions.Name = "TabPage_Actions";
            this.TabPage_Actions.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Actions.Size = new System.Drawing.Size(961, 721);
            this.TabPage_Actions.TabIndex = 3;
            this.TabPage_Actions.Text = "Actions";
            this.TabPage_Actions.UseVisualStyleBackColor = true;
            // 
            // TabPage_Behaviours
            // 
            this.TabPage_Behaviours.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Behaviours.Name = "TabPage_Behaviours";
            this.TabPage_Behaviours.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Behaviours.Size = new System.Drawing.Size(961, 721);
            this.TabPage_Behaviours.TabIndex = 4;
            this.TabPage_Behaviours.Text = "Behaviours";
            this.TabPage_Behaviours.UseVisualStyleBackColor = true;
            // 
            // ProjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 747);
            this.Controls.Add(this.ProjectProperties_TabControl);
            this.Name = "ProjectProperties";
            this.Text = "Project Properties";
            this.ProjectProperties_TabControl.ResumeLayout(false);
            this.TabPage_General.ResumeLayout(false);
            this.TabPage_General.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl ProjectProperties_TabControl;
        private System.Windows.Forms.TabPage TabPage_General;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage TabPage_DLL;
        private System.Windows.Forms.TabPage TabPage_Types;
        private System.Windows.Forms.TabPage TabPage_Actions;
        private System.Windows.Forms.TabPage TabPage_Behaviours;
        private System.Windows.Forms.Label BehaviourDir_Label;
        private System.Windows.Forms.Label ProjectDir_Label;
        private System.Windows.Forms.Label ProjectName_Label;
        private System.Windows.Forms.Button BehaviourDir_BrowseBtn;
        private System.Windows.Forms.TextBox BehaviuorDir_TextBox;
        private System.Windows.Forms.Button ProjectDir_BrowseBtn;
        private System.Windows.Forms.TextBox ProjectDir_TextBox;
        private System.Windows.Forms.TextBox ProjectName_TextBox;
        private System.Windows.Forms.Button Cancel_Btn;
        private System.Windows.Forms.Button Confirm_Btn;
    }
}