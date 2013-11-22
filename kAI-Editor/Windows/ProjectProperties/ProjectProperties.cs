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
using System.Reflection;

using kAI.Editor.Core;

namespace kAI.Editor.Forms.ProjectProperties
{
    /// <summary>
    /// A form showing the settings for this project. 
    /// </summary>
    partial class ProjectPropertiesForm : Form
    {
        /// <summary>
        /// A boolean indicating if this is a new project or not. 
        /// </summary>
        bool mIsNewProject;

        /// <summary>
        /// The project this screen is showing. 
        /// </summary>
        public kAIProject Project
        {
            get;
            private set;
        }


        

        /// <summary>
        /// Create the project properties form with the relevant project. 
        /// </summary>
        public ProjectPropertiesForm(kAIProject lProject)
        {
            InitializeComponent();

            // If we have a null entry, then we are making a new project, use the default values but don't let us close unless sound
            if (lProject == null)
            {
                lProject = new kAIProject();
                mIsNewProject = true;
            }
            else
            {
                mIsNewProject = false;
            }

            Project = lProject;

            // Set the values in the form based on the project. 
            SetFormFromProject();
        }

        /// <summary>
        /// Set all the forms values according to the projects value
        /// </summary>
        private void SetFormFromProject()
        {
            SetMainFormFromProject();
            SetDLLFormFromProject();
            SetTypesFormFromProject();
            SetFunctionsFormFromProject();
            
        }

        /// <summary>
        /// Set all the projects values according to the current values in the form.
        /// </summary>
        private void SetProjectFromForm()
        {
            SetProjectFromMainForm();
            SetProjectFromDLLForm();
            SetProjectFromTypesForm();
            SetProjectFromFunctionsForm();
        }


        private void Confirm_Btn_Click(object sender, EventArgs e)
        {
            SetProjectFromForm();
            if (mIsNewProject)
            {
                // Validate folders are valid and create if not
                if (!Project.ProjectRoot.Exists)
                {
                    Project.ProjectRoot.Create();
                }
                else
                {
                    FileInfo[] lFiles = Project.ProjectRoot.GetFiles("*." + kAIProject.kProjectFileExtension);
                    if (lFiles.Length > 0)
                    {
                        MessageBox.Show("A kAI project already exists in this directory, must be a new directory. \n" 
                            + Path.GetFileNameWithoutExtension(lFiles[0].Name));
                        return;
                    }
                }

                if (!Project.XmlBehaviourRoot.GetDirectory().Exists)
                {
                    Project.XmlBehaviourRoot.GetDirectory().Create();
                }
            }

            Project.Save();

            DialogResult = DialogResult.OK;
        }

        

         
    }
}
