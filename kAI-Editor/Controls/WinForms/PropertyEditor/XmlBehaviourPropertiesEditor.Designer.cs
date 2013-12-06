namespace kAI.Editor.Controls.WinForms
{
    partial class XmlBehaviourPropertiesEditor
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
            this.mBehaviourNameLabel = new System.Windows.Forms.Label();
            this.mInPortsGroupBox = new System.Windows.Forms.GroupBox();
            this.mInPortListControl = new kAI.Editor.Controls.WinForms.PortListControl();
            this.mInPortsList = new System.Windows.Forms.ListBox();
            this.mGroupOutPorts = new System.Windows.Forms.GroupBox();
            this.mOutPortListControl = new kAI.Editor.Controls.WinForms.PortListControl();
            this.mOutPortsList = new System.Windows.Forms.ListBox();
            this.mConfirmBtn = new System.Windows.Forms.Button();
            this.mCancelBtn = new System.Windows.Forms.Button();
            this.mBehaviourIDText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mBehaviourLocationText = new System.Windows.Forms.TextBox();
            this.mBrowseBtn = new System.Windows.Forms.Button();
            this.mInPortsGroupBox.SuspendLayout();
            this.mGroupOutPorts.SuspendLayout();
            this.SuspendLayout();
            // 
            // mBehaviourNameLabel
            // 
            this.mBehaviourNameLabel.AutoSize = true;
            this.mBehaviourNameLabel.Location = new System.Drawing.Point(16, 13);
            this.mBehaviourNameLabel.Name = "mBehaviourNameLabel";
            this.mBehaviourNameLabel.Size = new System.Drawing.Size(55, 13);
            this.mBehaviourNameLabel.TabIndex = 2;
            this.mBehaviourNameLabel.Text = "Behaviour";
            // 
            // mInPortsGroupBox
            // 
            this.mInPortsGroupBox.Controls.Add(this.mInPortListControl);
            this.mInPortsGroupBox.Controls.Add(this.mInPortsList);
            this.mInPortsGroupBox.Location = new System.Drawing.Point(12, 86);
            this.mInPortsGroupBox.Name = "mInPortsGroupBox";
            this.mInPortsGroupBox.Size = new System.Drawing.Size(300, 409);
            this.mInPortsGroupBox.TabIndex = 3;
            this.mInPortsGroupBox.TabStop = false;
            this.mInPortsGroupBox.Text = "In Ports";
            // 
            // mInPortListControl
            // 
            this.mInPortListControl.Location = new System.Drawing.Point(4, 355);
            this.mInPortListControl.Name = "mInPortListControl";
            this.mInPortListControl.Size = new System.Drawing.Size(280, 50);
            this.mInPortListControl.TabIndex = 1;
            this.mInPortListControl.OnPortAddedClick += new System.EventHandler(this.portListControl1_OnPortAddedClick);
            this.mInPortListControl.OnPortRemovedClick += new System.EventHandler(this.mInPortListControl_OnPortRemovedClick);
            // 
            // mInPortsList
            // 
            this.mInPortsList.FormattingEnabled = true;
            this.mInPortsList.Location = new System.Drawing.Point(4, 20);
            this.mInPortsList.Name = "mInPortsList";
            this.mInPortsList.Size = new System.Drawing.Size(290, 329);
            this.mInPortsList.TabIndex = 0;
            // 
            // mGroupOutPorts
            // 
            this.mGroupOutPorts.Controls.Add(this.mOutPortListControl);
            this.mGroupOutPorts.Controls.Add(this.mOutPortsList);
            this.mGroupOutPorts.Location = new System.Drawing.Point(318, 86);
            this.mGroupOutPorts.Name = "mGroupOutPorts";
            this.mGroupOutPorts.Size = new System.Drawing.Size(300, 409);
            this.mGroupOutPorts.TabIndex = 4;
            this.mGroupOutPorts.TabStop = false;
            this.mGroupOutPorts.Text = "Out Ports";
            // 
            // mOutPortListControl
            // 
            this.mOutPortListControl.Location = new System.Drawing.Point(6, 353);
            this.mOutPortListControl.Name = "mOutPortListControl";
            this.mOutPortListControl.Size = new System.Drawing.Size(280, 50);
            this.mOutPortListControl.TabIndex = 1;
            this.mOutPortListControl.OnPortAddedClick += new System.EventHandler(this.mOutPortListControl_OnPortAddedClick);
            this.mOutPortListControl.OnPortRemovedClick += new System.EventHandler(this.mOutPortListControl_OnPortRemovedClick);
            // 
            // mOutPortsList
            // 
            this.mOutPortsList.FormattingEnabled = true;
            this.mOutPortsList.Location = new System.Drawing.Point(7, 20);
            this.mOutPortsList.Name = "mOutPortsList";
            this.mOutPortsList.Size = new System.Drawing.Size(287, 329);
            this.mOutPortsList.TabIndex = 0;
            // 
            // mConfirmBtn
            // 
            this.mConfirmBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mConfirmBtn.Location = new System.Drawing.Point(543, 501);
            this.mConfirmBtn.Name = "mConfirmBtn";
            this.mConfirmBtn.Size = new System.Drawing.Size(75, 23);
            this.mConfirmBtn.TabIndex = 5;
            this.mConfirmBtn.Text = "Confirm";
            this.mConfirmBtn.UseVisualStyleBackColor = true;
            this.mConfirmBtn.Click += new System.EventHandler(this.mConfirmBtn_Click);
            // 
            // mCancelBtn
            // 
            this.mCancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelBtn.Location = new System.Drawing.Point(462, 501);
            this.mCancelBtn.Name = "mCancelBtn";
            this.mCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.mCancelBtn.TabIndex = 6;
            this.mCancelBtn.Text = "Cancel";
            this.mCancelBtn.UseVisualStyleBackColor = true;
            this.mCancelBtn.Click += new System.EventHandler(this.mCancelBtn_Click);
            // 
            // mBehaviourIDText
            // 
            this.mBehaviourIDText.Location = new System.Drawing.Point(126, 10);
            this.mBehaviourIDText.Name = "mBehaviourIDText";
            this.mBehaviourIDText.Size = new System.Drawing.Size(486, 20);
            this.mBehaviourIDText.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Behaviour Location";
            // 
            // mBehaviourLocationText
            // 
            this.mBehaviourLocationText.Location = new System.Drawing.Point(126, 38);
            this.mBehaviourLocationText.Name = "mBehaviourLocationText";
            this.mBehaviourLocationText.Size = new System.Drawing.Size(411, 20);
            this.mBehaviourLocationText.TabIndex = 9;
            // 
            // mBrowseBtn
            // 
            this.mBrowseBtn.Location = new System.Drawing.Point(544, 36);
            this.mBrowseBtn.Name = "mBrowseBtn";
            this.mBrowseBtn.Size = new System.Drawing.Size(68, 23);
            this.mBrowseBtn.TabIndex = 10;
            this.mBrowseBtn.Text = "Browse";
            this.mBrowseBtn.UseVisualStyleBackColor = true;
            // 
            // XmlBehaviourPropertiesEditor
            // 
            this.AcceptButton = this.mConfirmBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelBtn;
            this.ClientSize = new System.Drawing.Size(626, 536);
            this.Controls.Add(this.mBrowseBtn);
            this.Controls.Add(this.mBehaviourLocationText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mBehaviourIDText);
            this.Controls.Add(this.mCancelBtn);
            this.Controls.Add(this.mConfirmBtn);
            this.Controls.Add(this.mGroupOutPorts);
            this.Controls.Add(this.mInPortsGroupBox);
            this.Controls.Add(this.mBehaviourNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XmlBehaviourPropertiesEditor";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "XmlBehaviourPropertiesEditor";
            this.mInPortsGroupBox.ResumeLayout(false);
            this.mGroupOutPorts.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mBehaviourNameLabel;
        private System.Windows.Forms.GroupBox mInPortsGroupBox;
        private System.Windows.Forms.ListBox mInPortsList;
        private System.Windows.Forms.GroupBox mGroupOutPorts;
        private System.Windows.Forms.ListBox mOutPortsList;
        private PortListControl mInPortListControl;
        private PortListControl mOutPortListControl;
        private System.Windows.Forms.Button mConfirmBtn;
        private System.Windows.Forms.Button mCancelBtn;
        private System.Windows.Forms.TextBox mBehaviourIDText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mBehaviourLocationText;
        private System.Windows.Forms.Button mBrowseBtn;
    }
}