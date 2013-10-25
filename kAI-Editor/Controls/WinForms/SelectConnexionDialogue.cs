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
    public partial class SelectConnexionDialogue : Form
    {
        public SelectConnexionDialogue(kAIPort lSender, IEnumerable<kAIPort.kAIConnexion> lConnexions)
        {
            InitializeComponent();

            mPortNameLabel.Text = lSender.PortID.ToString();

            foreach (kAIPort.kAIConnexion lConnexion in lConnexions)
            {
                mConnexionsChecked.Items.Add(lConnexion);
            }
        }

        public IEnumerable<kAIPort.kAIConnexion> GetPortsToDisconnect()
        {
            return mConnexionsChecked.CheckedItems.Cast<kAIPort.kAIConnexion>();
        }
    }
}
