using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using kAI.Editor.Forms;
using kAI.Editor.Forms.ProjectProperties;
using kAI.Editor.Controls;
using System.Reflection;

namespace kAI.Editor
{
    /// <summary>
    /// A form for choosing from a list of behaviours a specific behaviour. 
    /// </summary>
    partial class kAINodeChooser : Form
    {
        /// <summary>
        /// The behaviour that is to be made. 
        /// </summary>
        kAIBehaviour mNewBehaviour;

        kAIProject mProject;

        /// <summary>
        /// Create a new behaviour chooser form.
        /// </summary>
        public kAINodeChooser(kAIProject lProject)
        {
            InitializeComponent();

            mNewBehaviour = null;

            foreach (kAIINodeSerialObject lTemplate in lProject.NodeObjects.Values)
            {
                BehavioursList.Items.Add(lTemplate);
            }

            mProject = lProject;

        }

        /// <summary>
        /// Get the selected behaviour, shouldn't be called before the dialogue box has closed. 
        /// </summary>
        /// <returns>An instance of the new behaviour to make. </returns>
        public kAIBehaviour GetSelectedBehaviour()
        {
            return mNewBehaviour;
        }

        private void BehavioursList_DoubleClick(object sender, EventArgs e)
        {
            // What have we double clicked on
            ListBox lSelectedItem = sender as ListBox;
            if (lSelectedItem != null)
            {
                // Was the item a behaviour template
                kAIINodeSerialObject lTemplateBehaviour = lSelectedItem.SelectedItem as kAIINodeSerialObject;
                if (lTemplateBehaviour != null)
                {
                    try
                    {
                        mNewBehaviour = lTemplateBehaviour.Instantiate(mProject.GetAssemblyByName) as kAIBehaviour;
                    }
                    catch (System.Exception ex)
                    {
                        GlobalServices.Logger.LogError("Failed to instantiate node: " + ex.ToString());
                    }
                    

                    if (mNewBehaviour != null)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        GlobalServices.Logger.LogError("Failed to instantiate node: Reason Unknown");
                    }                    
                }
            }
        }
    }
}
