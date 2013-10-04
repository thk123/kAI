using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace kAI.Editor.Controls
{
    /// <summary>
    /// An DLL entry in the project properties control. 
    /// </summary>
    public partial class DLLEntry : UserControl
    {
        /// <summary>
        /// Delegate for the DLLRemovedEvent.
        /// </summary>
        /// <param name="lSender">The entry that sent the request. </param>
        /// <param name="lDLLToRemove">The assembly this entry represented. </param>
        public delegate void DLLRemoved(DLLEntry lSender, Assembly lDLLToRemove);

        /// <summary>
        /// Triggered when the user clicks the delete button on this assembly.
        /// </summary>
        public event DLLRemoved DLLRemovedEvent;

        /// <summary>
        /// The Assembly behind this entry. 
        /// </summary>
        Assembly mAssembly;

        /// <summary>
        /// Create a DLL entry representing a given assembly. 
        /// </summary>
        /// <param name="lAssembly">The assembly this entry represents. </param>
        public DLLEntry(Assembly lAssembly)
        {
            InitializeComponent();
            mAssembly = lAssembly;
            SetDllEntry(lAssembly);
        }

        /// <summary>
        /// Sets the data of this control according to the DLL. 
        /// </summary>
        /// <param name="lDLLEntry">The DLL to use to set the controls of the control. </param>
        private void SetDllEntry(Assembly lDLLEntry)
        {
            DLL_NameLabel.Text = lDLLEntry.GetName().Name;
        }

        private void RemoveDLL_Btn_Click(object sender, EventArgs e)
        {
            DLLRemovedEvent(this, mAssembly);
        }
    }
}
