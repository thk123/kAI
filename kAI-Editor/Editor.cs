using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using kAI.Editor.Forms;
using kAI.Editor.Forms.ProjectProperties;
using kAI.Editor.Controls;




namespace kAI.Editor
{
    /// <summary>
    /// The general form for creating kAI Behaviours
    /// </summary>
    public partial class Editor : Form
    {
        bool mIsProjectLoaded;
        kAIProject mLoadedProject;

        BehaviourEditorWindow mBehaviourEditor;

        List<PropertyControllerBase<bool>> mProjectLoadedControls; // Controls that should only be enabled when a project is loaded.

        /// <summary>
        /// Create a new editor. 
        /// </summary>
        public Editor()
        {
            InitializeComponent();

            mIsProjectLoaded = false;
            mLoadedProject = null;

            mProjectLoadedControls = new List<PropertyControllerBase<bool>>();
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(projectPropertiesToolStripMenuItem));
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(closeProjectToolStripMenuItem));

            // Disable all the project specific controls);
            SetEnabledProjectControls(false);

        }
               

        void addBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mIsProjectLoaded)
            {
                BehaviourChooser lChooser = new BehaviourChooser();
                if (lChooser.ShowDialog() == DialogResult.OK)
                {
                    mBehaviourEditor.AddBehaviour(lChooser.GetSelectedBehaviour());
                }
            }
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectPropertiesForm lNewProjectPropertiesViewer = new ProjectPropertiesForm(mLoadedProject);

            // The user has decided to create a new project
            if (lNewProjectPropertiesViewer.ShowDialog() == DialogResult.OK)
            {
                LoadProject(lNewProjectPropertiesViewer.Project);
            }
        }

        private void LoadProject(kAIProject lProject)
        {
            mLoadedProject = lProject;
            mIsProjectLoaded = true;

            // Remove all old controls from the right hand window
            MainEditor.Panel1.Controls.Clear();

            SetEnabledProjectControls(true);

            Label lTestLabel = new Label();
            lTestLabel.Text = "Test project made";
            MainEditor.Panel1.Controls.Add(lTestLabel);

            mBehaviourEditor = new BehaviourEditorWindow();
            MainEditor.Panel2.Controls.Add(mBehaviourEditor);
        }

        private void CloseProject()
        {
            System.Diagnostics.Debug.Assert(mLoadedProject != null);

            mLoadedProject = null;

            SetEnabledProjectControls(false);
        }

        private void SetEnabledProjectControls(bool lEnabled)
        {
            foreach (var lControls in mProjectLoadedControls)
            {
                lControls.SetProperty(lEnabled);
            }
        }


        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog lOpenDialog = new OpenFileDialog();
            lOpenDialog.Filter = "kAI Project Files |*." + kAIProject.kProjectFileExtension;
            if (lOpenDialog.ShowDialog() == DialogResult.OK)
            {
                kAIProject lProject = kAIProject.Load(new FileInfo(lOpenDialog.FileName));

                LoadProject(lProject);
            }
        }

        private void projectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectPropertiesForm lEditProject = new ProjectPropertiesForm(mLoadedProject);
            lEditProject.ShowDialog();
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseProject();
        }
    }
}
