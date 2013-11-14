using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;

namespace kAI.Editor.Controls.WinForms.PropertyEditor
{
    /// <summary>
    /// For editing or creating a new port. 
    /// </summary>
    partial class PortPropertiesEditor : Form
    {
        /// <summary>
        /// The port that has been created or modified.
        /// </summary>
        public kAIPort Port
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a window for a new port. 
        /// </summary>
        /// <param name="lDefaultDirection">The direction of the port. </param>
        /// <param name="lProject">The project we are making a port for. </param>
        public PortPropertiesEditor(kAIPort.ePortDirection lDefaultDirection, kAIProject lProject)
            : this(null, lProject)
        {
            mPortDirectionDropdown.SelectedItem = lDefaultDirection;
        }

        /// <summary>
        /// Create a window for modifying an existing port. 
        /// </summary>
        /// <param name="lPort">The port to modify. </param>
        /// <param name="lProject">The project we are making a port for. </param>
        public PortPropertiesEditor(kAIPort lPort, kAIProject lProject)
        {
            InitializeComponent();

            mPortDirectionDropdown.Items.AddRange(new object[] { kAIPort.ePortDirection.PortDirection_In, kAIPort.ePortDirection.PortDirection_Out });
            
            mPortTypeDropdown.Items.Add(kAIPortType.TriggerType);
            mPortTypeDropdown.Items.Add(new kAIPortType(typeof(float)));

            if (lPort != null)
            {
                SetFromPort(lPort);
            }
            else
            {
                Port = null;
                mPortIDTextBox.Text = "PortID...";
                mPortDirectionDropdown.SelectedItem = kAIPort.ePortDirection.PortDirection_Out;
                mPortTypeDropdown.SelectedItem = kAIPortType.TriggerType;
            }
        }

        void SetFromPort(kAIPort lPort)
        {
            mPortIDTextBox.Text = lPort.PortID;

            mPortDirectionDropdown.SelectedItem = lPort.PortDirection;

            mPortTypeDropdown.SelectedItem = lPort.DataType;
            Port = lPort;
        }

        private void mConfirmBtn_Click(object sender, EventArgs e)
        {
            // apply to the port
            if (Port == null)
            {
                if ((kAIPortType)mPortTypeDropdown.SelectedItem == kAIPortType.TriggerType)
                {
                    Port = new kAITriggerPort(mPortIDTextBox.Text, (kAIPort.ePortDirection)mPortDirectionDropdown.SelectedItem);
                }
                else
                {
                    Port = kAIDataPort.CreateDataPort((kAIPortType)mPortTypeDropdown.SelectedItem, mPortIDTextBox.Text, (kAIPort.ePortDirection)mPortDirectionDropdown.SelectedItem);
                }
                
            }
            else
            {
                // TODO: modify ports!                
                if ((kAIPortType)mPortTypeDropdown.SelectedItem == kAIPortType.TriggerType)
                {
                    Port = new kAITriggerPort(mPortIDTextBox.Text, (kAIPort.ePortDirection)mPortDirectionDropdown.SelectedItem);
                }
                else
                {
                    Port = kAIDataPort.CreateDataPort((kAIPortType)mPortTypeDropdown.SelectedItem, mPortIDTextBox.Text, (kAIPort.ePortDirection)mPortDirectionDropdown.SelectedItem);
                }
            }

        }

        private void mCancelBtn_Click(object sender, EventArgs e)
        {
            // do nothing
        }
    }
}
