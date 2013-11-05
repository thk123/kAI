using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// A control for managing the PortList in <see cref="kAIXmlBehaviourPropertiesEditor"/>.
    /// </summary>
    partial class PortListControl : UserControl
    {
        /// <summary>
        /// Triggered when the Add Port button is clicked.
        /// </summary>
        public event EventHandler OnPortAddedClick;

        /// <summary>
        /// Triggered when the Remove Port button is clicked. 
        /// </summary>
        public event EventHandler OnPortRemovedClick;

        public PortListControl()
        {
            InitializeComponent();

            mAddBtn.Click += new EventHandler(mAddBtn_Click);
            mRemoveBtn.Click += new EventHandler(mRemoveBtn_Click);
        }

        void mRemoveBtn_Click(object sender, EventArgs e)
        {
            if (OnPortRemovedClick != null)
            {
                OnPortRemovedClick(sender, e);
            }
        }

        void mAddBtn_Click(object sender, EventArgs e)
        {
            if (OnPortAddedClick != null)
            {
                OnPortAddedClick(sender, e);
            }
        }
    }
}
