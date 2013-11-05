using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

namespace kAI.Editor.Controls
{
    /// <summary>
    /// Represents a port in the editor view
    /// </summary>
    public partial class kAIEditorPortWinForms : UserControl/*, kAIIEditorPortGraphicalImplementator*/
    {
        /// <summary>
        /// The background image to use (i.e. when not hovered).
        /// </summary>
        Image mBackgroundImage;

        /// <summary>
        /// The image to use when hovered. 
        /// </summary>
        Image mHoverImage;

        /// <summary>
        /// The port this control is representing. 
        /// </summary>
        public kAIPort Port
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a editor port from a given port. 
        /// </summary>
        /// <param name="lPort">The port this control is repesentings</param>
        public kAIEditorPortWinForms(kAIPort lPort)
        {
            InitializeComponent();

            // Based on the direction of the port, choose the image. 
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

            Port = lPort;
        }

        

        

        /// <summary>
        /// Set the image to the non-hovered state. 
        /// </summary>
        void ResetImage()
        {
            BackgroundImage = mBackgroundImage;
        }

        private void kAIEditorPortWinForms_MouseEnter(object sender, EventArgs e)
        {
            BackgroundImage = mHoverImage;
        }

        private void kAIEditorPortWinForms_MouseLeave(object sender, EventArgs e)
        {
            BackgroundImage = mBackgroundImage;
        }

    }
}
