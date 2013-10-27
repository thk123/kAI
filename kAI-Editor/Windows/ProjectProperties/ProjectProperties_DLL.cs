using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;
using System.Reflection;
using kAI.Editor.Controls;

namespace kAI.Editor.Forms.ProjectProperties
{
    partial class ProjectPropertiesForm : Form
    {
        /// <summary>
        /// Sets the controls on the DLL tab from the project properties. 
        /// </summary>
        private void SetDLLFormFromProject()
        {
            DLL_FlowList.Controls.Clear();

            foreach (Assembly lAssembly in Project.ProjectDLLs)
            {
                DLLEntry lEntry = new DLLEntry(lAssembly);
                lEntry.DLLRemovedEvent += new DLLEntry.DLLRemoved(lEntry_DLLRemovedEvent);
                DLL_FlowList.Controls.Add(lEntry);
            }
        }

        /// <summary>
        /// Sets the project properties from the settings on the DLLs screen. 
        /// </summary>
        private void SetProjectFromDLLForm()
        {
            //TODO: probably not required but for consistency should. 
        }

        void lEntry_DLLRemovedEvent(DLLEntry lSender, Assembly lDLLToRemove)
        {
            Project.RemoveDLL(lDLLToRemove);
            DLL_FlowList.Controls.Remove(lSender);

        }

        private void DLL_Browse(object sender, EventArgs e)
        {
            OpenFileDialog lFolderBrowser = new OpenFileDialog();
            lFolderBrowser.Filter = "DLL Files (*.dll)|*.dll";
            lFolderBrowser.InitialDirectory = Project.XmlBehaviourRoot.GetDirectory().FullName;

            DialogResult lResult = lFolderBrowser.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                DLL_BrowseText.Text = lFolderBrowser.FileName; ;
            }
        }

        private void DLL_AddBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DLL_BrowseText.Text))
            {
                MessageBox.Show("Enter a valid DLL path.");
            }
            else
            {
                try
                {
                    Project.AddDLL(new FileInfo(DLL_BrowseText.Text));
                    SetFormFromProject();
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Enter a valid DLL path.");
                }
            }
            //TODO: More exception handling
           
        }
    }
}
