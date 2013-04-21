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
        /// Stores the position of the mouse relative to the top left hand corner of the item so 
        /// can be dragged without jumping at the start.
        /// </summary>
        Point mClickOffset;

        /// <summary>
        /// The current offset position of the editor window containing this behaviour.
        /// </summary>
        Point mContainerOffset;

        /// <summary>
        /// The next vertial position of a in port. 
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
        /// Construct a new editor node with the given node ID. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour this node represents</param>
        public kAIEditorNode(kAIBehaviour lBehaviour)
        {
            InitializeComponent();
            
            // The container is started at zero.
            mContainerOffset = new Point(0, 0);

            foreach(kAIPort lPort in lBehaviour.GlobalPorts)
            {
                AddPort(lPort);
            }

            BehaviourName.Text = lBehaviour.NodeID;
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

            Controls.Add(lNewEditorPort);
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
