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
        BehaviourTree mBehaviourTree;

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

        /// <summary>
        /// Load a given project in to the editor. 
        /// </summary>
        /// <param name="lProject"></param>
        private void LoadProject(kAIProject lProject)
        {
            mLoadedProject = lProject;
            mIsProjectLoaded = true;

            // Remove all old controls from the right hand window
            MainEditor.Panel1.Controls.Clear();

            SetEnabledProjectControls(true);

            mBehaviourTree = new BehaviourTree(lProject);
            mBehaviourTree.Dock = DockStyle.Fill;
            MainEditor.Panel1.Controls.Add(mBehaviourTree);

            mBehaviourEditor = new BehaviourEditorWindow();
            MainEditor.Panel2.Controls.Add(mBehaviourEditor);
        }

        /// <summary>
        /// Close the currently open project. 
        /// </summary>
        private void CloseProject()
        {
            System.Diagnostics.Debug.Assert(mLoadedProject != null);

            mLoadedProject = null;

            SetEnabledProjectControls(false);
        }

        /// <summary>
        /// Either enable or disable all the project controls (eg controls that only make sense when a project is open). 
        /// </summary>
        /// <param name="lEnabled">Should the project controls be enabled. </param>
        private void SetEnabledProjectControls(bool lEnabled)
        {
            foreach (var lControls in mProjectLoadedControls)
            {
                lControls.SetProperty(lEnabled);
            }
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

            mBehaviourTree.UpdateTree(mLoadedProject);
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseProject();
        }
    }
}
