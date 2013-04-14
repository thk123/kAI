using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

namespace kAI.Editor
{
    /// <summary>
    /// The general form for creating kAI Behaviours
    /// </summary>
    public partial class Editor : Form
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
        /// Create a new editor. 
        /// </summary>
        public Editor()
        {
            InitializeComponent();

            // We register these events so can deal with dragging about. 
            MainEditor.Panel2.MouseDown += new MouseEventHandler(Panel2_MouseDown);
            MainEditor.Panel2.MouseMove += new MouseEventHandler(Panel2_MouseMove);

            nodes = new List<kAIEditorNode>();
        }

        /// <summary>
        /// Add a new behaviour to the editor window. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour to add. </param>
        void AddBehaviour(kAIBehaviour lBehaviour)
        {
            kAIEditorNode lNewNode = new kAIEditorNode(lBehaviour);

            MainEditor.Panel2.Controls.Add(lNewNode);

            nodes.Add(lNewNode);
        }

        void addBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BehaviourChooser lChooser = new BehaviourChooser();
            if (lChooser.ShowDialog() == DialogResult.OK)
            {
                AddBehaviour(lChooser.GetSelectedBehaviour());
            }
        }

        // Used for dragging the window about
        void Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            mLastPosition = e.Location;   
        }

        // Used for dragging the window about. 
        void Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point lDeltaPos = Util.SubtractPoints(e.Location, mLastPosition);

                Console.WriteLine("Position: " + mLastPosition.ToString());

                foreach (kAIEditorNode lNode in nodes)
                {
                    lNode.SetViewPosition(lDeltaPos);
                }

                mLastPosition = e.Location;
            }
        }
    }
}
