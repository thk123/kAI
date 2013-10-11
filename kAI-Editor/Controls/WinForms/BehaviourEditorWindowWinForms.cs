using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using kAI.Editor.Forms;
using kAI.Editor.Forms.ProjectProperties;
using kAI.Editor.Controls;

namespace kAI.Editor.Controls
{
    /// <summary>
    /// The control that handles editing behaviours. 
    /// </summary>
    partial class BehaviourEditorWindowWinForms : UserControl, kAIIBehaviourEditorGraphicalImplementator
    {
        List<kAIEditorNode> mNodes;

        /// <summary>
        /// Stores the last point of the mouse, used when dragging to create a delta. 
        /// </summary>
        Point mLastPosition;

        /// <summary>
        /// The next vertical position of a in port. 
        /// </summary>
        int mNextInPortY;

        /// <summary>
        /// The next vertical position of an out port. 
        /// </summary>
        int mNextOutPortY;

        /// <summary>
        /// The vertical distance between ports. 
        /// </summary>
        const int kPortVerticalSeperation = 10;

        bool mIsMakingConnexion;
        kAIEditorPortWinForms mStartingPort;

        public event Action<kAIPort, kAIPort> OnConnexion;

        /// <summary>
        /// Create a new editor pane for a XML behaviour implemented using WinForms
        /// </summary>
        public BehaviourEditorWindowWinForms()
        {
            InitializeComponent();

            UnloadBehaviour();
        }

        public void Init(Control lParent, kAIBehaviourEditorWindow lEditor)
        {
            lParent.Controls.Add(this);
            Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Resets the form to be ready for a new behaviour.
        /// </summary>
        public void UnloadBehaviour()
        {
            mNextInPortY = 5;
            mNextOutPortY = 5;

            mIsMakingConnexion = false;
            mStartingPort = null;

            mNodes = new List<kAIEditorNode>();

            Controls.Clear();
        }

        /// <summary>
        /// Add a control representing a given node within the behaviour.
        /// </summary>
        /// <param name="lNode">The node that is being added. </param>
        public void AddNode(kAINode lNode)
        {
            kAIEditorNode lEditorNode = new kAIEditorNode(lNode);
            Controls.Add(lEditorNode);

            mNodes.Add(lEditorNode);

            lEditorNode.OnPortClicked += new kAIEditorNode.PortClicked(lEditorNode_OnPortClicked);
        }

        /// <summary>
        /// Remove a given node control from within the behaviour. 
        /// </summary>
        /// <param name="lNode">The node that is being remvoed. </param>
        public void RemoveNode(kAINode lNode)
        {
            kAIEditorNode lControlNode = null;
            foreach (kAIEditorNode lEditorNode in mNodes)
            {
                if (lEditorNode.Node == lNode)
                {
                    lControlNode = lEditorNode;
                }
            }

            kAIObject.Assert(null, lControlNode, "Could not find removable node");

            foreach (kAIPort lExternalPort in lNode.GetExternalPorts())
            {
                // TODO: Won't remove existing connexions going into the port...
                if(lExternalPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                {
                    foreach (kAIPort.kAIConnexion lConnexion in lExternalPort.Connexions)
                    {
                        RemoveConnexion(lConnexion);
                    }
                }
            }

            mNodes.Remove(lControlNode);

            Controls.Remove(lControlNode);
        }

        /// <summary>
        /// Add a connexion between two ports. 
        /// </summary>
        /// <param name="lConnexion">A structure representing the two ports to connect. </param>
        public void AddConnexion(kAIPort.kAIConnexion lConnexion)
        {
            //mBehaviour.AddConnexion(lStartPort.Port, lEndPort.Port);

            //kAIEditorPortWinForms lStartPort = mNodes.Find((port) => { return port.Port == lConnexion.StartPort; });

            kAIEditorPortWinForms lStartPort = GetControlPortFromPort(lConnexion.StartPort);
            kAIEditorPortWinForms lEndPort = GetControlPortFromPort(lConnexion.EndPort);

            Graphics lG = Graphics.FromHwnd(Handle);
            lG.DrawCurve(new Pen(Color.Black), new Point[] { lStartPort.GetControlPosition(this), lEndPort.GetControlPosition(this) });
        }

        private kAIEditorPortWinForms GetControlPortFromPort(kAIPort lPort)
        {
            // TODO: this is shit

            kAIEditorPortWinForms lControlPort = Controls.OfType<kAIEditorPortWinForms>().FirstOrDefault((control) => { return control.Port == lPort; });

            kAIObject.Assert(null, lControlPort, "Could not find port");

            return lControlPort;
        }

        public bool CanConnect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a connexion between two ports.
        /// </summary>
        /// <param name="lConnexion">A structure representing the two ports whose connexion we wish to remove. </param>
        public void RemoveConnexion(kAIPort.kAIConnexion lConnexion)
        {
            // TODO
        }

        public void AddInternalPort(kAIPort lNewPort)
        {
            kAIEditorPortWinForms lNewEditorPort = new kAIEditorPortWinForms(lNewPort);
            if (lNewPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                Point lNewLocation = new Point();
                lNewLocation.Y = mNextInPortY;
                lNewLocation.X = 0;

                lNewEditorPort.Location = lNewLocation;
                mNextInPortY += lNewEditorPort.Height + kPortVerticalSeperation;
            }
            else
            {
                Point lNewLocation = new Point();
                lNewLocation.Y = mNextOutPortY;
                lNewLocation.X = Width - lNewEditorPort.Width;

                lNewEditorPort.Location = lNewLocation;
                mNextOutPortY += lNewEditorPort.Height + kPortVerticalSeperation;
            }

            lNewEditorPort.Click += new EventHandler(lNewEditorPort_Click);

            Controls.Add(lNewEditorPort);
        }

        public void RemoveInternalPort()
        {
            throw new NotImplementedException();
        }

        public void AddExternalPort(kAINode lParentNode, kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public void RemoveExternalPort()
        {
            throw new NotImplementedException();
        }

        //TODO: The following methods are like the same...
        void lNewEditorPort_Click(object sender, EventArgs e)
        {
            kAIEditorPortWinForms lPortClicked = sender as kAIEditorPortWinForms;

            kAIObject.Assert(null, lPortClicked, "Didn't click on a port");

            if (mIsMakingConnexion)
            {
                kAIObject.Assert(null, mStartingPort, "No starting node to end with");
                // Assert mOtherPort != null
                EndDrag(lPortClicked);
            }
            else
            {
                mIsMakingConnexion = true;
                mStartingPort = lPortClicked;
            }
        }

        void lEditorNode_OnPortClicked(kAIEditorPortWinForms lPortClicked, kAINode lOwningNode)
        {
            if (mIsMakingConnexion)
            {
                EndDrag(lPortClicked);
            }
            else
            {
                mIsMakingConnexion = true;
                mStartingPort = lPortClicked;
            }
        }

        void EndDrag(kAIEditorPortWinForms lEndPort)
        {
            OnConnexion(mStartingPort.Port, lEndPort.Port);
            //AddConnexion(mStartingPort, lEndPort);
            lEndPort = null;
            mIsMakingConnexion = false;
        }

        private void BehaviourEditorWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mLastPosition = e.Location;
        }

        private void BehaviourEditorWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point lDeltaPos = Util.SubtractPoints(e.Location, mLastPosition);

                // Update each nodes position. 
                foreach (kAIEditorNode lNode in mNodes)
                {
                    lNode.SetViewPosition(lDeltaPos);
                }

                mLastPosition = e.Location;
            }
        }

        public void EditorUpdate()
        {
            // Do nothing
        }

        public void Destroy()
        {
            // Do nothing
        }

        #region kAIIBehaviourEditorGraphicalImplementator Members


        public void RemoveInternalPort(kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public void RemoveExternalPort(kAINode lParentNode, kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}