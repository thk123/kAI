﻿using System;
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

        /// <summary>
        /// Create a new editor pane for a XML behaviour
        /// </summary>
        public BehaviourEditorWindow(kAIProject lProject)
        {
            InitializeComponent();

            mNextInPortY = 5;
            mNextOutPortY = 5;

            mBehaviour = null;
            mBehaviourLocation = null;

            mNodes = new List<kAIEditorNode>();

            mProject = lProject;
        }

        /// <summary>
        /// Add a new behaviour to the editor window. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour to add. </param>
        public void AddBehaviour(kAIBehaviour lBehaviour)
        {
            kAIEditorNode lNewNode = new kAIEditorNode(lBehaviour);

            Controls.Add(lNewNode);

            nodes.Add(lNewNode);
        }

        /// <summary>
        /// Create a new behaviour and load it in to the editor. 
        /// </summary>
        public void NewBehaviour(kAIBehaviourID lBehaviourID, FileInfo lFile)
        {
            //TEMP: Need to get the name from some dialog box or something. 
            kAIXmlBehaviour lBehaviour = new kAIXmlBehaviour(lBehaviourID);

            mBehaviourLocation = lFile;

            LoadBehaviour(lBehaviour);
        }

        /// <summary>
        /// Load an existing XML behaviour in to the editor. 
        /// </summary>
        /// <param name="lBehaviour">The XML behaviour to load. </param>
        public void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            foreach (kAIPort lGlobalPort in lBehaviour.GlobalPorts)
            {
                AddGlobalPort(lGlobalPort);
            }
        }

        private void AddGlobalPort(kAIPort lNewPort)
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

            Controls.Add(lNewEditorPort);
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
