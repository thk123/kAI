using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor;

using kAI.Editor.Core;
using System.IO;

namespace kAI.Editor.Forms.ProjectProperties
{
    partial class XmlBehaviourCreator : Form
    {
        kAIProject mProject;

        DirectoryInfo mContainingDirectory;
        public kAIBehaviourID BehaviourID
        {
            get;
            private set;
        }

        public FileInfo BehaviourPath
        {
            get
            {
                return new FileInfo(mContainingDirectory.FullName + "//" + BehaviourID + "." + kAIXmlBehaviour.kAIXmlBehaviourExtension);
            }
        }

        public XmlBehaviourCreator(kAIProject lProject, kAIBehaviourID lBehaviourName)
        {
            InitializeComponent();
            mProject = lProject;
            BehaviourID = lBehaviourName;
            mContainingDirectory = lProject.XmlBehaviourRoot;

            SetValuesFromData();
        }

        private void SetValuesFromData()
        {
            BehaviourName_Text.Text = BehaviourID;
            BehaviourLocation_Text.Text = mContainingDirectory.FullName;   
        }

        private void SetDataFromValues()
        {
            BehaviourID = BehaviourName_Text.Text;
            mContainingDirectory = new DirectoryInfo(BehaviourName_Text.Text);
        }

        private void BehaviourLocation_Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog lFolderPicker = new FolderBrowserDialog();
            //TODO: Maybe set the root folder to be the project, we don't want anything outside it?
            lFolderPicker.SelectedPath = mContainingDirectory.FullName;

            if (lFolderPicker.ShowDialog() == DialogResult.OK)
            {
                BehaviourLocation_Text.Text = lFolderPicker.SelectedPath;
            }

            SetValuesFromData();
        }

        private void Create_Btn_Click(object sender, EventArgs e)
        {
            // Make sure the name matches
            SetDataFromValues();

            if (mProject.CheckBehaviourName(BehaviourID))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("A behaviour with this name exists already, each behaviour must have a unique name");
            }
        }

        private void BehaviourName_Text_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
