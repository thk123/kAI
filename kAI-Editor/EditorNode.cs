using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

namespace kAI.Editor
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
        /// <param name="lNodeId">The ID of the node that this node holds.</param>
        public kAIEditorNode(kAINodeID lNodeId)
        {
            InitializeComponent();
            
            // The container is started at zero.
            mContainerOffset = new Point(0, 0);

            // Set the text to be the ID 
            NodeName.Text = lNodeId.ToString();
        }

        /// <summary>
        /// For updating the position of the containing editor. 
        /// </summary>
        /// <param name="lEditorDelta">How much the window has moved. </param>
        public void SetViewPosition(Point lEditorDelta)
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

        // When the mouse is pressed, we store the relative position for dragging.
        private void kAIEditorNode_MouseDown(object sender, MouseEventArgs e)
        {
            mClickOffset = e.Location;
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
