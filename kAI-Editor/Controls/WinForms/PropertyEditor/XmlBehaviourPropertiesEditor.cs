using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
        

        // For a new behaviour
        

        kAIProject mProject;

        kAIIBehaviourModifier lModifier;

        bool mIsBehaviourNew;


        public kAIXmlBehaviour Behaviour
        {
            get;
            private set;
        }

        public XmlBehaviourPropertiesEditor(kAIProject lProject, kAIXmlBehaviour lSourceBehaviour)
        {
            InitializeComponent();

            mIsBehaviourNew = lSourceBehaviour == null;
            mProject = lProject;

            if (mIsBehaviourNew)
            {
                lModifier = new kAIXmlBehaviour.Builder();
                lModifier.SetBehaviourID("NewBehaviour");
                lModifier.SetPath(SetConstructorPath(new FileInfo(lProject.XmlBehaviourRoot.GetDirectory().FullName + "//" + lModifier.BehaviourID + "." + kAIXmlBehaviour.kAIXmlBehaviourExtension)));
            }
            else
            {
                lModifier = new XmlBehaviourModifier(lSourceBehaviour);
            }

            ConfigureControls();

            
        }

        kAIRelativePath SetConstructorPath(FileInfo lRealPath)
        {
            return new kAIRelativePath(lRealPath, mProject.XmlBehaviourRoot.GetDirectory(), kAIProject.kBehaviourRootID);
        }

        void ConfigureControls()
        {
            mBehaviourIDText.Text = lModifier.BehaviourID;
            mBehaviourLocationText.Text = lModifier.BehaviourPath.GetFile().FullName;

            mInPortsList.Items.Clear();
            mOutPortsList.Items.Clear();

            foreach (kAIPort lPort in lModifier.Ports)
            {
                AddPortToList(lPort);
            }

            if (mIsBehaviourNew)
            {
                // nowt to do. 
            }
            else
            {
                mBehaviourIDText.ReadOnly = true;
                mBehaviourLocationText.ReadOnly = true;
                mBrowseBtn.Enabled = false;
            }
        }

        private void AddPortToList(kAIPort lPort)
        {
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                mInPortsList.Items.Add(lPort);
            }
            else
            {
                mOutPortsList.Items.Add(lPort);
            }
        }

        /// <summary>
        /// Checks wether we are allowed to add the port:
        ///     - must be no existing internal port with matching name
        ///     - no external port with matching name
        /// </summary>
        /// <param name="lNewPort">The port we are seeking to add. </param>
        /// <returns>True if no port that would interfer has a matching name. </returns>
        private bool CheckPortToAdd(kAIPort lNewPort)
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

            // Since not all external ports are internal ports (eg, OnDeactivate) and in any case the names
            // don't always match, we check the default external ports on a behaviour. 
            foreach (kAIPortID lPreDefinedExternalPort in kAIBehaviour.sDefaultExternalPortNames)
            {
                if (lPreDefinedExternalPort == lNewPort.PortID)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check the specified port can in fact be removed. 
        /// </summary>
        /// <param name="lPort">The port we wish to remove. </param>
        /// <returns>True if the port is not fixed. </returns>
        private bool CheckPortToRemove(kAIPort lPort)
        {
            foreach (kAIPortID lPortID in kAIBehaviour.sDefaultExternalPortNames)
            {
                if (lPort.PortID == lPortID)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Brings up the dialogue to create or modify a port. 
        /// </summary>
        /// <param name="lDefaultDirection">The direction of the port. </param>
        /// <returns></returns>
        kAIPort CreatePort(kAIPort.ePortDirection lDefaultDirection)
        {
            kAIPort lPort = null;
            bool lResult;
            do
            {
                PortPropertiesEditor lEditor;
                if (lPort == null)
                {
                    lEditor = new PortPropertiesEditor(lDefaultDirection, mProject);
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

                lResult = CheckPortToAdd(lPort);

                if (!lResult)
                {
                    MessageBox.Show("A port with this ID already exists, choose a different name");
                }

            } while (!lResult);

            return lPort;
        }

        // Add a port to the left hand list
        private void portListControl1_OnPortAddedClick(object sender, EventArgs e)
        {
            kAIPort lNewPort = CreatePort(kAIPort.ePortDirection.PortDirection_Out);
            if (lNewPort != null)
            {
                AddPortToList(lNewPort);

                lModifier.AddPort(lNewPort);
            }
        }

        // Remove a port from left list. 
        private void mInPortListControl_OnPortRemovedClick(object sender, EventArgs e)
        {
            if (mInPortsList.SelectedItem != null)
            {
                kAIPort lPortToRemove = (kAIPort)mInPortsList.SelectedItem;

                if(CheckPortToRemove(lPortToRemove))
                {
                    lModifier.RemovePort(lPortToRemove);
                    mInPortsList.Items.Remove(mInPortsList.SelectedItem);
                }
            }
        }

        // Add a port to the right hand list. 
        private void mOutPortListControl_OnPortAddedClick(object sender, EventArgs e)
        {
            kAIPort lNewPort = CreatePort(kAIPort.ePortDirection.PortDirection_In);
            if (lNewPort != null)
            {
                AddPortToList(lNewPort);
                lModifier.AddPort(lNewPort);
            }
        }

        // Remove a port from the right hand list. 
        private void mOutPortListControl_OnPortRemovedClick(object sender, EventArgs e)
        {
            if (mOutPortsList.SelectedItem != null)
            {
                kAIPort lPortToRemove = (kAIPort)mOutPortsList.SelectedItem;

                if(CheckPortToRemove(lPortToRemove))
                {
                    lModifier.RemovePort(lPortToRemove);
                    mOutPortsList.Items.Remove(mOutPortsList.SelectedItem);
                }
            }
        }

        private void mConfirmBtn_Click(object sender, EventArgs e)
        {
            lModifier.SetBehaviourID(mBehaviourIDText.Text);
            lModifier.SetPath(SetConstructorPath(new FileInfo(mBehaviourLocationText.Text)));
            Behaviour = lModifier.Construct();
        }

        private void mCancelBtn_Click(object sender, EventArgs e)
        {

        }
    }

    class XmlBehaviourModifier : kAIIBehaviourModifier
    {

        /// <summary>
        /// The list of actions to perform on the XMLBehaviour if we choose 
        /// </summary>
        Queue<Action<kAIXmlBehaviour>> mActionsOnBehaviour;

        // For an existing behaviour
        kAIXmlBehaviour mSourceBehaviour;

        public kAIBehaviourID BehaviourID
        {
            get { return mSourceBehaviour.BehaviourID; }
        }

        public kAIRelativePath BehaviourPath
        {
            get { return mSourceBehaviour.XmlLocation; }
        }



        public XmlBehaviourModifier(kAIXmlBehaviour lBehaviour)
        {
            mSourceBehaviour = lBehaviour;
            mActionsOnBehaviour = new Queue<Action<kAIXmlBehaviour>>();
        }

        public void SetBehaviourID(kAIBehaviourID lBehaviourID)
        {
            if (mSourceBehaviour.BehaviourID != lBehaviourID)
            {
                throw new InvalidOperationException("Cannot change behaviour ID, post construction");
            }
        }

        public void SetPath(kAIRelativePath lPath)
        {
            if (mSourceBehaviour.XmlLocation.GetFile().FullName != lPath.GetFile().FullName)
            {
                throw new InvalidOperationException("Cannot change behaviour location, post construction");
            }
        }

        public IEnumerable<kAIPort> Ports
        {
            get { return mSourceBehaviour.InternalPorts; }
        }

        public void AddPort(kAIPort lNewPort)
        {
            mActionsOnBehaviour.Enqueue((lBehaviour) =>
            {
                lBehaviour.AddInternalPort(lNewPort, true);
            });
        }

        public void RemovePort(kAIPort lPortToRemove)
        {
            throw new NotSupportedException("Not yet supported");
        }

        public kAIXmlBehaviour Construct()
        {
            // we apply all the actions in order to the actual behaviour. 
            foreach (Action<kAIXmlBehaviour> lAction in mActionsOnBehaviour)
            {
                lAction(mSourceBehaviour);
            }

            return mSourceBehaviour;
        }

    }
}
