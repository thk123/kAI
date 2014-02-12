namespace kAI.Editor.Controls.WinForms.Debug
{
    partial class DebugWindow
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
            this.mDebuggableBehaviours = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // mDebuggableBehaviours
            // 
            this.mDebuggableBehaviours.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mDebuggableBehaviours.FormattingEnabled = true;
            this.mDebuggableBehaviours.Location = new System.Drawing.Point(0, 0);
            this.mDebuggableBehaviours.Name = "mDebuggableBehaviours";
            this.mDebuggableBehaviours.Size = new System.Drawing.Size(579, 327);
            this.mDebuggableBehaviours.TabIndex = 0;
            this.mDebuggableBehaviours.SelectedIndexChanged += new System.EventHandler(this.mDebuggableBehaviours_SelectedIndexChanged);
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(579, 327);
            this.Controls.Add(this.mDebuggableBehaviours);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DebugWindow";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.Text = "DebugWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox mDebuggableBehaviours;
    }
}