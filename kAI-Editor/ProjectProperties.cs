using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;

namespace kAI.Editor
{
    /// <summary>
    /// 
    /// </summary>
    partial class ProjectProperties : Form
    {
        public kAIProject mProperties
        {
            get;
            set;
        }

        bool mIsNewProject;

        /// <summary>
        /// 
        /// </summary>
        public ProjectProperties(kAIProject lProperties)
        {
            InitializeComponent();

            // If we have a null entry, then we are making a new project, use the defualt values but don't let us close unless sound
            if (lProperties == null)
            {
                lProperties = new kAIProject();
                mIsNewProject = true;
            }
            else
            {
                mIsNewProject = false;
            }

            mProperties = lProperties;

            SetFormFromProperties();
        }

        private void SetFormFromProperties()
        {
            ProjectName_TextBox.Text = mProperties.ProjectName;
            ProjectDir_TextBox.Text = mProperties.ProjectRoot.FullName;
            BehaviuorDir_TextBox.Text = mProperties.XmlBehaviourRoot.FullName;
        }

        private void SetPropertiesFromForm()
        {
            mProperties.ProjectName = ProjectName_TextBox.Text;
            mProperties.ProjectRoot = new DirectoryInfo(ProjectDir_TextBox.Text);
            mProperties.XmlBehaviourRoot = new DirectoryInfo(BehaviuorDir_TextBox.Text);
        }

        private void ProjectDir_Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog lFolderBrowser = new FolderBrowserDialog();
            lFolderBrowser.SelectedPath = mProperties.ProjectRoot.FullName;

            DialogResult lResult = lFolderBrowser.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                ProjectDir_TextBox.Text = lFolderBrowser.SelectedPath;
                mProperties.ProjectRoot = new DirectoryInfo(lFolderBrowser.SelectedPath);
            }
        }

        private void BehaviourDir_Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog lFolderBrowser = new FolderBrowserDialog();
            lFolderBrowser.SelectedPath = mProperties.XmlBehaviourRoot.FullName;

            DialogResult lResult = lFolderBrowser.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                ProjectDir_TextBox.Text = lFolderBrowser.SelectedPath;
                mProperties.XmlBehaviourRoot = new DirectoryInfo(lFolderBrowser.SelectedPath);
            }
        }

        private void Confirm_Btn_Click(object sender, EventArgs e)
        {
            SetPropertiesFromForm();
            if (mIsNewProject)
            {
                // Validate folders are valid and create if not
                if (!mProperties.ProjectRoot.Exists)
                {
                    mProperties.ProjectRoot.Create();
                }
                else
                {
                    FileInfo[] lFiles = mProperties.ProjectRoot.GetFiles("*." + kAIProject.kProjectFileExtension);
                    if (lFiles.Length > 0)
                    {
                        MessageBox.Show("A kAI project already exists in this directory, must be a new directory. \n" 
                            + Path.GetFileNameWithoutExtension(lFiles[0].Name));
                        return;
                    }
                }

                if (!mProperties.XmlBehaviourRoot.Exists)
                {
                    mProperties.XmlBehaviourRoot.Create();
                }

                
            }

            XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(kAIProject));
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;
            lSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            XmlWriter lWriter = XmlWriter.Create(mProperties.ProjectRoot.FullName + "\\" + mProperties.ProjectName + ".kAIProj", lSettings);
            lProjectSerialiser.WriteObject(lWriter, mProperties);
            lWriter.Close();

            DialogResult = DialogResult.OK;
        }
    }
}
