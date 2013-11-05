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
using kAI.Editor.Controls.WinForms.PropertyEditor;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// For modifying an XML beavhiour.
    /// </summary>
    partial class XmlBehaviourPropertiesEditor : Form
    {
        kAIXmlBehaviour mSourceBehaviour;
        kAIProject mProject;

        /// <summary>
        /// The list of actions to perform on the XMLBehaviour if we choose 
        /// </summary>
        Queue<Action<kAIXmlBehaviour>> mActionsOnBehaviour;

        public XmlBehaviourPropertiesEditor(kAIProject lProject, kAIXmlBehaviour lSourceBehaviour)
        {
            InitializeComponent();


            SetXmlBehaviour(lSourceBehaviour);

            foreach (kAIINodeSerialObject lSerialObject in lProject.NodeObjects.Values)
            {
                if (lSerialObject.GetNodeFlavour() == eNodeFlavour.BehaviourXml)
                {
                    mXmlBehaviourDropdown.Items.Add(lSerialObject);
                    if (lSerialObject.GetFriendlyName() == lSourceBehaviour.BehaviourID)
                    {
                        mXmlBehaviourDropdown.SelectedItem = lSerialObject;
                    }
                }
            }

            mProject = lProject;
            mSourceBehaviour = lSourceBehaviour;
        }

        void SetXmlBehaviour(kAIXmlBehaviour lSourceBehaviour)
        {
            mBehaviourNameLabel.Text = lSourceBehaviour.BehaviourID;

            mInPortsList.Items.Clear();
            mOutPortsList.Items.Clear();

            foreach (kAIPort lPort in lSourceBehaviour.GetExternalPorts())
            {
                AddPortToList(lPort);
            }

            mActionsOnBehaviour = new Queue<Action<kAIXmlBehaviour>>();
        }

        private void AddPortToList(kAIPort lPort)
        {
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                mInPortsList.Items.Add(lPort);
            }
            else
            {
                mOutPortsList.Items.Add(lPort);
            }
        }

        private bool CheckPort(kAIPort lNewPort)
        {
            foreach (kAIPort lExistingPort in mInPortsList.Items.Cast<kAIPort>())
            {
                if (lExistingPort.PortID == lNewPort.PortID)
                {
                    return false;
                }
            }

            foreach (kAIPort lExistingPort in mOutPortsList.Items.Cast<kAIPort>())
            {
                if (lExistingPort.PortID == lNewPort.PortID)
                {
                    return false;
                }
            }

            return true;
        }

        private void portListControl1_OnPortAddedClick(object sender, EventArgs e)
        {
            kAIPort lNewPort = CreatePort(kAIPort.ePortDirection.PortDirection_In);
            if (lNewPort != null)
            {
                AddPortToList(lNewPort);
                mActionsOnBehaviour.Enqueue((lBehaviour) =>
                {
                    lBehaviour.AddInternalPort(lNewPort, true);
                });
            }
        }

        kAIPort CreatePort(kAIPort.ePortDirection lDefaultDirection)
        {
            kAIPort lPort = null;
            bool lResult;
            do
            {
                PortPropertiesEditor lEditor;
                if (lPort == null)
                {
                    lEditor = new PortPropertiesEditor(kAIPort.ePortDirection.PortDirection_Out, mProject);
                }
                else
                {
                    lEditor = new PortPropertiesEditor(lPort, mProject);
                }

                if (lEditor.ShowDialog() == DialogResult.Cancel)
                {
                    break;
                }

                lPort = lEditor.Port;

                lResult = CheckPort(lPort);

                if (!lResult)
                {
                    MessageBox.Show("A port with this ID already exists, choose a different name");
                }

            } while (!lResult);

            return lPort;
        }

        private void mInPortListControl_OnPortRemovedClick(object sender, EventArgs e)
        {
            if (mInPortsList.SelectedItem != null)
            {
                mInPortsList.Items.Remove(mInPortsList.SelectedItem);
                mActionsOnBehaviour.Enqueue((lBehaviour) =>
                    {
                        //TOOD
                    });
            }
        }


        private void mOutPortListControl_OnPortAddedClick(object sender, EventArgs e)
        {
            kAIPort lNewPort = CreatePort(kAIPort.ePortDirection.PortDirection_Out);
            if (lNewPort != null)
            {
                AddPortToList(lNewPort);
                mActionsOnBehaviour.Enqueue((lBehaviour) =>
                    {
                        lBehaviour.AddInternalPort(lNewPort, true);
                    });
            }
        }

        private void mOutPortListControl_OnPortRemovedClick(object sender, EventArgs e)
        {
            if (mOutPortsList.SelectedItem != null)
            {
                mOutPortsList.Items.Remove(mOutPortsList.SelectedItem);
            }
        }

        private void mConfirmBtn_Click(object sender, EventArgs e)
        {

            // we apply all the actions in order to the actual behaviour. 
            foreach (Action<kAIXmlBehaviour> lAction in mActionsOnBehaviour)
            {
                lAction(mSourceBehaviour);
            }
        }

        private void mCancelBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
