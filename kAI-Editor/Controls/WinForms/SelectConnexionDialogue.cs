using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// A dialogue box for selecting one or more connexions from a given port. 
    /// </summary>
    public partial class SelectConnexionDialogue : Form
    {
        /// <summary>
        /// Create a SelectionConnexionDialogue for a given port. 
        /// </summary>
        /// <param name="lSender">The port to look at. </param>
        /// <param name="lConnexions">The connexions going either into or out of this port. </param>
        public SelectConnexionDialogue(kAIPort lSender, IEnumerable<kAIPort.kAIConnexion> lConnexions)
        {
            InitializeComponent();

            mPortNameLabel.Text = lSender.PortID.ToString();

            foreach (kAIPort.kAIConnexion lConnexion in lConnexions)
            {
                mConnexionsChecked.Items.Add(lConnexion);
            }
        }

        /// <summary>
        /// Get the currently selected connexions. 
        /// </summary>
        /// <returns>A enumeration of checked connexions. </returns>
        public IEnumerable<kAIPort.kAIConnexion> GetPortsToDisconnect()
        {
            return mConnexionsChecked.CheckedItems.Cast<kAIPort.kAIConnexion>();
        }
    }
}
