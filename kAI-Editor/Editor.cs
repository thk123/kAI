using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
               

        void addBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TEMP: Clearly need to have a window asking what node etc.
            kAIEditorNode lNewNode = new kAIEditorNode("TestNode");

            MainEditor.Panel2.Controls.Add(lNewNode);

            nodes.Add(lNewNode);
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
