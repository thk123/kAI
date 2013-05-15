using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using kAI.Editor.Forms;
using kAI.Editor.Forms.ProjectProperties;
using kAI.Editor.Controls;

namespace kAI.Editor.Controls
{
    /// <summary>
    /// A node in a behaviour web. 
    /// </summary>
    public partial class kAIEditorNode : UserControl
    {
        /// <summary>
        /// Handler for a a port clicked event. 
        /// </summary>
        /// <param name="lPortClicked">The port that was clicked. </param>
        /// <param name="lOwningNode">The node that this port belongs to (if any). </param>
        public delegate void PortClicked(kAIEditorPort lPortClicked, kAINode lOwningNode);

        /// <summary>
        /// Stores the position of the mouse relative to the top left hand corner of the item so 
        /// can be dragged without jumping at the start.
        /// </summary>
        Point mClickOffset;

        /// <summary>
        /// The current offset position of the editor window containing this behaviour.
        /// </summary>
        Point mContainerOffset;

        /// <summary>
        /// The next vertical position of a in port. 
        /// </summary>
        int mNextInPortY;

        /// <summary>
        /// The next vertical position of an out port. 
        /// </summary>
        int mNextOutPortY;

        /// <summary>
        /// The node this control is representing. 
        /// </summary>
        public kAINode Node
        {
            get;
            private set;
        }

        /// <summary>
        /// The vertical distance between ports. 
        /// </summary>
        const int kPortVerticalSeperation = 10;

        /// <summary>
        /// The position of this node in the behaviour web (independent of current view).
        /// </summary>
        public Point GlobalPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// The position of the window containing this, so can be dragged about. 
        /// </summary>
        Point ContainerPosition
        {
            get
            {
                return mContainerOffset;
            }
        }


        /// <summary>
        /// Triggered when one of the ports within this node is clicked. 
        /// </summary>
        public event PortClicked OnPortClicked;

        /// <summary>
        /// Construct a new editor node with the given node ID. 
        /// </summary>
        /// <param name="lNode">The behaviour this node represents</param>
        public kAIEditorNode(kAINode lNode)
        {
            InitializeComponent();
            
            // The container is started at zero.
            mContainerOffset = new Point(0, 0);

            foreach(kAIPort lPort in lNode.GetExternalPorts())
            {
                AddPort(lPort);
            }

            BehaviourName.Text = lNode.NodeID;

            Node = lNode;
        }

        /// <summary>
        /// Add a port to this node.
        /// </summary>
        /// <param name="lNewPort">The port to add. </param>
        public void AddPort(kAIPort lNewPort)
        {
            kAIEditorPort lNewEditorPort = new kAIEditorPort(lNewPort);
            if (lNewPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                Point lNewLocation = new Point();
                lNewLocation.Y = mNextInPortY;
                lNewLocation.X = MainNode.Location.X - lNewEditorPort.Width;

                lNewEditorPort.Location = lNewLocation;
                mNextInPortY += lNewEditorPort.Height + kPortVerticalSeperation;
            }
            else
            {
                Point lNewLocation = new Point();
                lNewLocation.Y = mNextOutPortY;
                lNewLocation.X = MainNode.Location.X + MainNode.Width;

                lNewEditorPort.Location = lNewLocation;
                mNextOutPortY += lNewEditorPort.Height + kPortVerticalSeperation;
            }

            // If the window is too short for this node, we extend it. 
            ExtendWindow(lNewEditorPort.Location.Y + lNewEditorPort.Height);

            lNewEditorPort.Click += new EventHandler(lNewEditorPort_Click);

            Controls.Add(lNewEditorPort);
        }

        void lNewEditorPort_Click(object sender, EventArgs e)
        {
            kAIEditorPort lClickedPort = sender as kAIEditorPort;
            if (lClickedPort != null)
            {
                if (OnPortClicked != null)
                {
                    OnPortClicked(lClickedPort, Node);
                }
            }
        }

        /// <summary>
        /// For updating the position of the containing editor. 
        /// </summary>
        /// <param name="lEditorDelta">How much the window has moved. </param>
        internal void SetViewPosition(Point lEditorDelta)
        {
            mContainerOffset = Util.AddPoints(mContainerOffset, lEditorDelta);
            UpdatePosition();
        }

        /// <summary>
        /// Update the drawn position of this element based on its position and the position of the editor. 
        /// </summary>
        private void UpdatePosition()
        {
            Location = Util.AddPoints(GlobalPosition, ContainerPosition);
        }

        /// <summary>
        /// If the window is no longer big enough to hold this new vertical limit, we extend the window.
        /// </summary>
        /// <param name="lNewVerticalMax">The new lower limit of the window.</param>
        private void ExtendWindow(int lNewVerticalMax)
        {
            if (Height < lNewVerticalMax)
            {
                // We take half the separation so as to give equal weight.
                Height = lNewVerticalMax + (kPortVerticalSeperation / 2);
            }
        }

        // When the mouse is pressed, we store the relative position for dragging.
        private void kAIEditorNode_MouseDown(object sender, MouseEventArgs e)
        {
            mClickOffset = Util.AddPoints(e.Location, ContainerPosition);
        }

        // When the mouse moves, we move the objects actual position and update the drawn position.
        private void kAIEditorNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Location = Util.AddPoints(mContainerOffset, Util.AddPoints(Location, Util.SubtractPoints(e.Location, mClickOffset)));

                GlobalPosition = Util.AddPoints(Location, Util.SubtractPoints(e.Location, mClickOffset));
                UpdatePosition();
            }
        }        
    }
}
