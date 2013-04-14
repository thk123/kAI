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
    /// Represents a port in the editor view
    /// </summary>
    public partial class kAIEditorPort : UserControl
    {
        Image mBackgroundImage;
        Image mHoverImage;

        kAIPort mPort;

        /// <summary>
        /// Create a editor port from a given port. 
        /// </summary>
        /// <param name="lPort"></param>
        public kAIEditorPort(kAIPort lPort)
        {
            InitializeComponent();


            mPort = lPort;


            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {

                mBackgroundImage = Properties.Resources.InPort;
                mHoverImage = Properties.Resources.InPort_Hover;
            }
            else
            {
                mBackgroundImage = Properties.Resources.OutPort;
                mHoverImage = Properties.Resources.OutPort_Hover;
            }

            ResetImage();
        }

        void ResetImage()
        {
            BackgroundImage = mBackgroundImage;
        }

        private void kAIEditorPort_MouseEnter(object sender, EventArgs e)
        {
            BackgroundImage = mHoverImage;
        }

        private void kAIEditorPort_MouseLeave(object sender, EventArgs e)
        {
            BackgroundImage = mBackgroundImage;
        }
    }
}
