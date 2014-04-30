namespace kAI.Editor.Forms.ProjectProperties
{
    partial class ProjectPropertiesForm
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
            this.DLL_BrowseText = new System.Windows.Forms.TextBox();
            this.DLL_BrowseBtn = new System.Windows.Forms.Button();
            this.DLL_AddBtn = new System.Windows.Forms.Button();
            this.DLL_FlowList = new System.Windows.Forms.FlowLayoutPanel();
            this.DLL_TitleLabel = new System.Windows.Forms.Label();
            this.TabPage_Types = new System.Windows.Forms.TabPage();
            this.mAllTypesList = new kAI.Editor.Controls.WinForms.SearchableList();
            this.mIncludedTypesList = new System.Windows.Forms.ListBox();
            this.mRemoveBtn = new System.Windows.Forms.Button();
            this.mAddBtn = new System.Windows.Forms.Button();
            this.mProjectTypesTitle = new System.Windows.Forms.Label();
            this.TabPage_Actions = new System.Windows.Forms.TabPage();
            this.mAllFunctionsList = new kAI.Editor.Controls.WinForms.SearchableList();
            this.mProjectFunctionsList = new System.Windows.Forms.ListBox();
            this.mFunctionsRemoveBtn = new System.Windows.Forms.Button();
            this.mFunctionRemoveBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TabPage_Behaviours = new System.Windows.Forms.TabPage();
            this.ProjectBehaviours_Title = new System.Windows.Forms.Label();
            this.Cancel_Btn = new System.Windows.Forms.Button();
            this.Confirm_Btn = new System.Windows.Forms.Button();
            this.ProjectProperties_TabControl.SuspendLayout();
            this.TabPage_General.SuspendLayout();
            this.TabPage_DLL.SuspendLayout();
            this.TabPage_Types.SuspendLayout();
            this.TabPage_Actions.SuspendLayout();
            this.TabPage_Behaviours.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProjectProperties_TabControl
            // 
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_General);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_DLL);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Types);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Actions);
            this.ProjectProperties_TabControl.Controls.Add(this.TabPage_Behaviours);
            this.ProjectProperties_TabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProjectProperties_TabControl.Location = new System.Drawing.Point(0, 0);
            this.ProjectProperties_TabControl.Name = "ProjectProperties_TabControl";
            this.ProjectProperties_TabControl.SelectedIndex = 0;
            this.ProjectProperties_TabControl.Size = new System.Drawing.Size(969, 483);
            this.ProjectProperties_TabControl.TabIndex = 0;
            // 
            // TabPage_General
            // 
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
            this.TabPage_General.Size = new System.Drawing.Size(961, 457);
            this.TabPage_General.TabIndex = 0;
            this.TabPage_General.Text = "General";
            this.TabPage_General.UseVisualStyleBackColor = true;
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
            this.TabPage_DLL.Controls.Add(this.DLL_BrowseText);
            this.TabPage_DLL.Controls.Add(this.DLL_BrowseBtn);
            this.TabPage_DLL.Controls.Add(this.DLL_AddBtn);
            this.TabPage_DLL.Controls.Add(this.DLL_FlowList);
            this.TabPage_DLL.Controls.Add(this.DLL_TitleLabel);
            this.TabPage_DLL.Location = new System.Drawing.Point(4, 22);
            this.TabPage_DLL.Name = "TabPage_DLL";
            this.TabPage_DLL.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_DLL.Size = new System.Drawing.Size(961, 687);
            this.TabPage_DLL.TabIndex = 1;
            this.TabPage_DLL.Text = "DLL\'s";
            this.TabPage_DLL.UseVisualStyleBackColor = true;
            // 
            // DLL_BrowseText
            // 
            this.DLL_BrowseText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DLL_BrowseText.Location = new System.Drawing.Point(13, 661);
            this.DLL_BrowseText.Name = "DLL_BrowseText";
            this.DLL_BrowseText.Size = new System.Drawing.Size(768, 20);
            this.DLL_BrowseText.TabIndex = 3;
            this.DLL_BrowseText.DoubleClick += new System.EventHandler(this.DLL_Browse);
            // 
            // DLL_BrowseBtn
            // 
            this.DLL_BrowseBtn.Location = new System.Drawing.Point(787, 658);
            this.DLL_BrowseBtn.Name = "DLL_BrowseBtn";
            this.DLL_BrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.DLL_BrowseBtn.TabIndex = 4;
            this.DLL_BrowseBtn.Text = "Browse";
            this.DLL_BrowseBtn.UseVisualStyleBackColor = true;
            this.DLL_BrowseBtn.Click += new System.EventHandler(this.DLL_Browse);
            // 
            // DLL_AddBtn
            // 
            this.DLL_AddBtn.Location = new System.Drawing.Point(868, 658);
            this.DLL_AddBtn.Name = "DLL_AddBtn";
            this.DLL_AddBtn.Size = new System.Drawing.Size(75, 23);
            this.DLL_AddBtn.TabIndex = 5;
            this.DLL_AddBtn.Text = "Add";
            this.DLL_AddBtn.UseVisualStyleBackColor = true;
            this.DLL_AddBtn.Click += new System.EventHandler(this.DLL_AddBtn_Click);
            // 
            // DLL_FlowList
            // 
            this.DLL_FlowList.Location = new System.Drawing.Point(13, 61);
            this.DLL_FlowList.Name = "DLL_FlowList";
            this.DLL_FlowList.Size = new System.Drawing.Size(940, 591);
            this.DLL_FlowList.TabIndex = 2;
            // 
            // DLL_TitleLabel
            // 
            this.DLL_TitleLabel.AutoSize = true;
            this.DLL_TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DLL_TitleLabel.Location = new System.Drawing.Point(3, 3);
            this.DLL_TitleLabel.Name = "DLL_TitleLabel";
            this.DLL_TitleLabel.Size = new System.Drawing.Size(308, 55);
            this.DLL_TitleLabel.TabIndex = 1;
            this.DLL_TitleLabel.Text = "Project DLL\'s";
            // 
            // TabPage_Types
            // 
            this.TabPage_Types.Controls.Add(this.mAllTypesList);
            this.TabPage_Types.Controls.Add(this.mIncludedTypesList);
            this.TabPage_Types.Controls.Add(this.mRemoveBtn);
            this.TabPage_Types.Controls.Add(this.mAddBtn);
            this.TabPage_Types.Controls.Add(this.mProjectTypesTitle);
            this.TabPage_Types.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Types.Name = "TabPage_Types";
            this.TabPage_Types.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Types.Size = new System.Drawing.Size(961, 687);
            this.TabPage_Types.TabIndex = 2;
            this.TabPage_Types.Text = "Types";
            this.TabPage_Types.UseVisualStyleBackColor = true;
            // 
            // mAllTypesList
            // 
            this.mAllTypesList.Location = new System.Drawing.Point(18, 90);
            this.mAllTypesList.Name = "mAllTypesList";
            this.mAllTypesList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.mAllTypesList.Size = new System.Drawing.Size(294, 259);
            this.mAllTypesList.TabIndex = 8;
            // 
            // mIncludedTypesList
            // 
            this.mIncludedTypesList.FormattingEnabled = true;
            this.mIncludedTypesList.Location = new System.Drawing.Point(362, 121);
            this.mIncludedTypesList.Name = "mIncludedTypesList";
            this.mIncludedTypesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.mIncludedTypesList.Size = new System.Drawing.Size(307, 225);
            this.mIncludedTypesList.TabIndex = 7;
            // 
            // mRemoveBtn
            // 
            this.mRemoveBtn.Location = new System.Drawing.Point(329, 256);
            this.mRemoveBtn.Name = "mRemoveBtn";
            this.mRemoveBtn.Size = new System.Drawing.Size(27, 23);
            this.mRemoveBtn.TabIndex = 6;
            this.mRemoveBtn.Text = "<-";
            this.mRemoveBtn.UseVisualStyleBackColor = true;
            this.mRemoveBtn.Click += new System.EventHandler(this.mRemoveBtn_Click);
            // 
            // mAddBtn
            // 
            this.mAddBtn.Location = new System.Drawing.Point(329, 215);
            this.mAddBtn.Name = "mAddBtn";
            this.mAddBtn.Size = new System.Drawing.Size(27, 23);
            this.mAddBtn.TabIndex = 4;
            this.mAddBtn.Text = "->";
            this.mAddBtn.UseVisualStyleBackColor = true;
            this.mAddBtn.Click += new System.EventHandler(this.mAddBtn_Click);
            // 
            // mProjectTypesTitle
            // 
            this.mProjectTypesTitle.AutoSize = true;
            this.mProjectTypesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mProjectTypesTitle.Location = new System.Drawing.Point(8, 3);
            this.mProjectTypesTitle.Name = "mProjectTypesTitle";
            this.mProjectTypesTitle.Size = new System.Drawing.Size(317, 55);
            this.mProjectTypesTitle.TabIndex = 2;
            this.mProjectTypesTitle.Text = "Project Types";
            // 
            // TabPage_Actions
            // 
            this.TabPage_Actions.Controls.Add(this.mAllFunctionsList);
            this.TabPage_Actions.Controls.Add(this.mProjectFunctionsList);
            this.TabPage_Actions.Controls.Add(this.mFunctionsRemoveBtn);
            this.TabPage_Actions.Controls.Add(this.mFunctionRemoveBtn);
            this.TabPage_Actions.Controls.Add(this.label2);
            this.TabPage_Actions.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Actions.Name = "TabPage_Actions";
            this.TabPage_Actions.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Actions.Size = new System.Drawing.Size(961, 687);
            this.TabPage_Actions.TabIndex = 3;
            this.TabPage_Actions.Text = "Actions";
            this.TabPage_Actions.UseVisualStyleBackColor = true;
            // 
            // mAllFunctionsList
            // 
            this.mAllFunctionsList.Location = new System.Drawing.Point(18, 90);
            this.mAllFunctionsList.Name = "mAllFunctionsList";
            this.mAllFunctionsList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.mAllFunctionsList.Size = new System.Drawing.Size(294, 259);
            this.mAllFunctionsList.TabIndex = 13;
            // 
            // mProjectFunctionsList
            // 
            this.mProjectFunctionsList.FormattingEnabled = true;
            this.mProjectFunctionsList.Location = new System.Drawing.Point(362, 121);
            this.mProjectFunctionsList.Name = "mProjectFunctionsList";
            this.mProjectFunctionsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.mProjectFunctionsList.Size = new System.Drawing.Size(307, 225);
            this.mProjectFunctionsList.TabIndex = 12;
            // 
            // mFunctionsRemoveBtn
            // 
            this.mFunctionsRemoveBtn.Location = new System.Drawing.Point(329, 256);
            this.mFunctionsRemoveBtn.Name = "mFunctionsRemoveBtn";
            this.mFunctionsRemoveBtn.Size = new System.Drawing.Size(27, 23);
            this.mFunctionsRemoveBtn.TabIndex = 11;
            this.mFunctionsRemoveBtn.Text = "<-";
            this.mFunctionsRemoveBtn.UseVisualStyleBackColor = true;
            this.mFunctionsRemoveBtn.Click += new System.EventHandler(this.mFunctionRemoveBtn_Click);
            // 
            // mFunctionRemoveBtn
            // 
            this.mFunctionRemoveBtn.Location = new System.Drawing.Point(329, 215);
            this.mFunctionRemoveBtn.Name = "mFunctionRemoveBtn";
            this.mFunctionRemoveBtn.Size = new System.Drawing.Size(27, 23);
            this.mFunctionRemoveBtn.TabIndex = 10;
            this.mFunctionRemoveBtn.Text = "->";
            this.mFunctionRemoveBtn.UseVisualStyleBackColor = true;
            this.mFunctionRemoveBtn.Click += new System.EventHandler(this.mFunctionAddBtn);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(394, 55);
            this.label2.TabIndex = 9;
            this.label2.Text = "Project Functions";
            // 
            // TabPage_Behaviours
            // 
            this.TabPage_Behaviours.Controls.Add(this.ProjectBehaviours_Title);
            this.TabPage_Behaviours.Location = new System.Drawing.Point(4, 22);
            this.TabPage_Behaviours.Name = "TabPage_Behaviours";
            this.TabPage_Behaviours.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage_Behaviours.Size = new System.Drawing.Size(961, 687);
            this.TabPage_Behaviours.TabIndex = 4;
            this.TabPage_Behaviours.Text = "Behaviours";
            this.TabPage_Behaviours.UseVisualStyleBackColor = true;
            // 
            // ProjectBehaviours_Title
            // 
            this.ProjectBehaviours_Title.AutoSize = true;
            this.ProjectBehaviours_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectBehaviours_Title.Location = new System.Drawing.Point(3, 3);
            this.ProjectBehaviours_Title.Name = "ProjectBehaviours_Title";
            this.ProjectBehaviours_Title.Size = new System.Drawing.Size(427, 55);
            this.ProjectBehaviours_Title.TabIndex = 2;
            this.ProjectBehaviours_Title.Text = "Project Behaviours";
            // 
            // Cancel_Btn
            // 
            this.Cancel_Btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Btn.Location = new System.Drawing.Point(683, 489);
            this.Cancel_Btn.Name = "Cancel_Btn";
            this.Cancel_Btn.Size = new System.Drawing.Size(138, 23);
            this.Cancel_Btn.TabIndex = 13;
            this.Cancel_Btn.Text = "Cancel";
            this.Cancel_Btn.UseVisualStyleBackColor = true;
            // 
            // Confirm_Btn
            // 
            this.Confirm_Btn.Location = new System.Drawing.Point(827, 489);
            this.Confirm_Btn.Name = "Confirm_Btn";
            this.Confirm_Btn.Size = new System.Drawing.Size(138, 23);
            this.Confirm_Btn.TabIndex = 12;
            this.Confirm_Btn.Text = "Confirm";
            this.Confirm_Btn.UseVisualStyleBackColor = true;
            this.Confirm_Btn.Click += new System.EventHandler(this.Confirm_Btn_Click);
            // 
            // ProjectPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 529);
            this.Controls.Add(this.Cancel_Btn);
            this.Controls.Add(this.Confirm_Btn);
            this.Controls.Add(this.ProjectProperties_TabControl);
            this.Name = "ProjectPropertiesForm";
            this.Text = "Project Properties";
            this.ProjectProperties_TabControl.ResumeLayout(false);
            this.TabPage_General.ResumeLayout(false);
            this.TabPage_General.PerformLayout();
            this.TabPage_DLL.ResumeLayout(false);
            this.TabPage_DLL.PerformLayout();
            this.TabPage_Types.ResumeLayout(false);
            this.TabPage_Types.PerformLayout();
            this.TabPage_Actions.ResumeLayout(false);
            this.TabPage_Actions.PerformLayout();
            this.TabPage_Behaviours.ResumeLayout(false);
            this.TabPage_Behaviours.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel DLL_FlowList;
        private System.Windows.Forms.Label DLL_TitleLabel;
        private System.Windows.Forms.Button Cancel_Btn;
        private System.Windows.Forms.Button Confirm_Btn;
        private System.Windows.Forms.TextBox DLL_BrowseText;
        private System.Windows.Forms.Button DLL_BrowseBtn;
        private System.Windows.Forms.Button DLL_AddBtn;
        private System.Windows.Forms.Label ProjectBehaviours_Title;
        private System.Windows.Forms.ListBox mIncludedTypesList;
        private System.Windows.Forms.Button mRemoveBtn;
        private System.Windows.Forms.Button mAddBtn;
        private System.Windows.Forms.Label mProjectTypesTitle;
        private Controls.WinForms.SearchableList mAllTypesList;
        private Controls.WinForms.SearchableList mAllFunctionsList;
        private System.Windows.Forms.ListBox mProjectFunctionsList;
        private System.Windows.Forms.Button mFunctionsRemoveBtn;
        private System.Windows.Forms.Button mFunctionRemoveBtn;
        private System.Windows.Forms.Label label2;
    }
}