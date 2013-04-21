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
    public partial class BehaviourChooser : Form
    {
        /// <summary>
        /// The behaviour that is to be made. 
        /// </summary>
        kAIBehaviour mNewBehaviour;

        /// <summary>
        /// Create a new behaviour chooser form.
        /// </summary>
        public BehaviourChooser()
        {
            InitializeComponent();

            mNewBehaviour = null;

            //TEMP: Clearly need to select dll's in project properties and shouldn't waste time doing it when creating the form. 
            Assembly lAssembly = Assembly.LoadFrom("kAITestBehaviours.dll");
            List<kAIBehaviourTemplate> templates = kAIBehaviourTemplate.ReadBehavioursFromDll(lAssembly);
            BehavioursList.Items.Add(lAssembly.FullName);
            foreach (kAIBehaviourTemplate lTemplate in templates)
            {
                BehavioursList.Items.Add(lTemplate);
            }

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
                kAIBehaviourTemplate lTemplateBehaviour = lSelectedItem.SelectedItem as kAIBehaviourTemplate;
                if (lTemplateBehaviour != null)
                {
                    // Yes, ok we try and instantiate a behaviour based on this template. This can be got by 
                    // whoever called the show dialogue using GetSelectedBehaviour. 
                    mNewBehaviour = lTemplateBehaviour.Instantiate();
                    DialogResult = DialogResult.OK;
                }
            }
            
        }
    }
}
