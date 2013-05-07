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
    partial class BehaviourEditorWindow : UserControl
    {
        /// <summary>
        /// The behaviour currently being shown in this editor. 
        /// </summary>
        kAIXmlBehaviour mBehaviour;

        FileInfo mBehaviourLocation;

        kAIProject mProject;

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
        kAIEditorPort mStartingPort;

        /// <summary>
        /// Create a new editor pane for a XML behaviour
        /// </summary>
        public BehaviourEditorWindow(kAIProject lProject)
        {
            InitializeComponent();

            ClearBehaviour();

            mProject = lProject;
        }

        /// <summary>
        /// Add a new behaviour to the editor window. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour to add. </param>
        public void AddBehaviour(kAIBehaviour lBehaviour)
        {         
            kAINode lBehaviourNode = new kAINode("Node", lBehaviour);
            kAIEditorNode lNewNode = new kAIEditorNode(lBehaviourNode);

            Controls.Add(lNewNode);

            mBehaviour.AddNode(lBehaviourNode);

            mNodes.Add(lNewNode);
        }

        public void AddNode(kAINode lNode)
        {
            kAIEditorNode lEditorNode = new kAIEditorNode(lNode);
            Controls.Add(lEditorNode);

            mNodes.Add(lEditorNode);

            lEditorNode.OnPortClicked += new kAIEditorNode.PortClicked(lEditorNode_OnPortClicked);
        }

        /// <summary>
        /// Create a new behaviour and load it in to the editor. 
        /// </summary>
        /// <returns>The new XML behaviour that was created.</returns>
        public kAIXmlBehaviour NewBehaviour(kAIBehaviourID lBehaviourID, FileInfo lFile)
        {
            //TEMP: Need to get the name from some dialog box or something. 
            kAIXmlBehaviour lBehaviour = new kAIXmlBehaviour(lBehaviourID, lFile);

            mBehaviourLocation = lFile;

            LoadBehaviour(lBehaviour);

            return lBehaviour;
        }

        /// <summary>
        /// Load an existing XML behaviour in to the editor. 
        /// </summary>
        /// <param name="lBehaviour">The XML behaviour to load. </param>
        public void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            if (mBehaviour != null)
            {
                ClearBehaviour();
            }

            foreach (kAIPort lGlobalPort in lBehaviour.InternalPorts)
            {
                AddInternalPort(lGlobalPort);
            }

            foreach (kAINode lInternalNode in lBehaviour.InternalNodes)
            {
                AddNode(lInternalNode);
            }
            mBehaviour = lBehaviour;
        }

        /// <summary>
        /// Save the behaviour.
        /// </summary>
        public void SaveBehaviour()
        {
            mBehaviour.Save();
        }

        /// <summary>
        /// Unloads the currently unloaded behaviour. 
        /// </summary>
        public void ClearBehaviour()
        {
            if (mBehaviour != null)
            {
                mBehaviour.Save();
            }

            mNextInPortY = 5;
            mNextOutPortY = 5;

            mBehaviour = null;
            mBehaviourLocation = null;

            mIsMakingConnexion = false;
            mStartingPort = null;

            mNodes = new List<kAIEditorNode>();

            Controls.Clear();
        }

        private void AddInternalPort(kAIPort lNewPort)
        {
            kAIEditorPort lNewEditorPort = new kAIEditorPort(lNewPort);
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

        void lNewEditorPort_Click(object sender, EventArgs e)
        {
            kAIEditorPort lPortClicked = sender as kAIEditorPort;
            if(lPortClicked != null)
            {
                if (mIsMakingConnexion)
                {
                    // Assert mOtherPort != null

                    FormConnexion(mStartingPort, lPortClicked);
                }
                else
                {
                    mIsMakingConnexion = true;
                    mStartingPort = lPortClicked;
                }
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        void lEditorNode_OnPortClicked(kAIEditorPort lPortClicked, kAINode lOwningNode)
        {
            if (mIsMakingConnexion)
            {
                FormConnexion(mStartingPort, lPortClicked);
            }
            else
            {
                mIsMakingConnexion = true;
                mStartingPort = lPortClicked;
            }
        }

        void FormConnexion(kAIEditorPort lStartPort, kAIEditorPort lEndPort)
        {
            mBehaviour.AddConnexion(lStartPort.Port, lEndPort.Port);

            Graphics lG = Graphics.FromHwnd(Handle);
            lG.DrawBezier(new Pen(Color.Black), new Point(0, 0), new Point(100, 10), new Point(100, 100), new Point(200, 200));
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
    }
}
