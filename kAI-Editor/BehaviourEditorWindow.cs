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
    ///
    public partial class BehaviourEditorWindow : UserControl
    {
        /// <summary>
        /// Stores the last point of the mouse, used when dragging to create a delta. 
        /// </summary>
        Point mLastPosition;

        /// <summary>
        /// TEMP: The list of nodes currently in the editor. 
        /// </summary>
        List<kAIEditorNode> nodes;

        /// <summary>
        /// Create a new editor pane for a XML behaviour
        /// </summary>
        public BehaviourEditorWindow()
        {
            InitializeComponent();

            nodes = new List<kAIEditorNode>();
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
                foreach (kAIEditorNode lNode in nodes)
                {
                    lNode.SetViewPosition(lDeltaPos);
                }

                mLastPosition = e.Location;
            }
        }
    }
}
