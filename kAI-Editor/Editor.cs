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
using kAI.Core;

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

        /// <summary>
        /// Create a new editor. 
        /// </summary>
        public Editor()
        {
            InitializeComponent();

            mIsProjectLoaded = false;
            mLoadedProject = null;            
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
            ProjectProperties lNewProjectPropertiesViewer = new ProjectProperties(mLoadedProject);

            // The user has decided to create a new project
            if (lNewProjectPropertiesViewer.ShowDialog() == DialogResult.OK)
            {
                LoadProject(lNewProjectPropertiesViewer.mProperties);
            }
        }

        private void LoadProject(kAIProject lProject)
        {
            mLoadedProject = lProject;
            mIsProjectLoaded = true;

            // Remove all old controls from the right hand window
            MainEditor.Panel1.Controls.Clear();

            Label lTestLabel = new Label();
            lTestLabel.Text = "Test project made";
            MainEditor.Panel1.Controls.Add(lTestLabel);

            mBehaviourEditor = new BehaviourEditorWindow();
            MainEditor.Panel2.Controls.Add(mBehaviourEditor);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog lOpenDialog = new OpenFileDialog();
            lOpenDialog.Filter = "kAI Project Files |*." + kAIProject.kProjectFileExtension;
            if (lOpenDialog.ShowDialog() == DialogResult.OK)
            {
                DataContractSerializer lDeserialiser = new DataContractSerializer(typeof(kAIProject));
                mLoadedProject = (kAIProject)lDeserialiser.ReadObject(lOpenDialog.OpenFile());
            }
        }
       
    }
}
