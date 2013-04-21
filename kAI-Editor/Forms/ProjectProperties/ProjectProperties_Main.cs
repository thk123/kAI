using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace kAI.Editor.Forms.ProjectProperties
{
    partial class ProjectPropertiesForm : Form
    {
        /// <summary>
        /// Set the controls on the main page from the project. 
        /// </summary>
        private void SetMainFormFromProject()
        {
            ProjectName_TextBox.Text = Project.ProjectName;
            ProjectDir_TextBox.Text = Project.ProjectRoot.FullName;
            BehaviuorDir_TextBox.Text = Project.XmlBehaviourRoot.FullName;
        }

        /// <summary>
        /// Sets the properties in the project from the main page. 
        /// </summary>
        private void SetProjectFromMainForm()
        {
            Project.ProjectName = ProjectName_TextBox.Text;
            Project.ProjectRoot = new DirectoryInfo(ProjectDir_TextBox.Text);
            Project.XmlBehaviourRoot = new DirectoryInfo(BehaviuorDir_TextBox.Text);
        }


        private void ProjectDir_Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog lFolderBrowser = new FolderBrowserDialog();
            lFolderBrowser.SelectedPath = Project.ProjectRoot.FullName;

            DialogResult lResult = lFolderBrowser.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                ProjectDir_TextBox.Text = lFolderBrowser.SelectedPath;
                Project.ProjectRoot = new DirectoryInfo(lFolderBrowser.SelectedPath);
            }
        }

        private void BehaviourDir_Browse(object sender, EventArgs e)
        {
            FolderBrowserDialog lFolderBrowser = new FolderBrowserDialog();
            lFolderBrowser.SelectedPath = Project.XmlBehaviourRoot.FullName;

            DialogResult lResult = lFolderBrowser.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                ProjectDir_TextBox.Text = lFolderBrowser.SelectedPath;
                Project.XmlBehaviourRoot = new DirectoryInfo(lFolderBrowser.SelectedPath);
            }
        }
    }
}
