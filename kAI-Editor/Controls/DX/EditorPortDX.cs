using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using SlimDX;

using kAI.Core;
using SlimDX.Direct3D11;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// The GUI element for a port. 
    /// </summary>
    class kAIEditorPortDX
    {
        /// <summary>
        /// The port this GUI element is representing.
        /// </summary>
        public kAIPort Port
        {
            get;
            private set;
        }

        NodeCoordinate mPosition;

        bool mIsHovering;

        /// <summary>
        /// The size of the port graphic. 
        /// </summary>
        public static readonly Vector2 PortSize;

        static kAIEditorPortDX()
        {
            PortSize = new Vector2(Properties.Resources.InPort.Width, Properties.Resources.InPort.Height);
        }

        /// <summary>
        /// Creates a new GUI element representing a given port. 
        /// </summary>
        /// <param name="lPort">The port being represented. </param>
        /// <param name="lPosition">The position of the port. </param>
        public kAIEditorPortDX(kAIPort lPort, NodeCoordinate lPosition)
        {
            Port = lPort;
            mPosition = lPosition;

            mIsHovering = false;
        }

        /// <summary>
        /// Used to move the port in the event of the attached node moving
        /// or the editor being resized.
        /// </summary>
        /// <param name="ldX">The change in x position. </param>
        /// <param name="ldY">The change in y position. </param>
        public void UpdatePosition(int ldX, int ldY)
        {
            mPosition.Translate(ldX, ldY);
        }

        /// <summary>
        /// Draw the port on the screen. 
        /// </summary>
        /// <param name="lContainerEditor">The editor window to draw the port in. </param>
        public void Render2D(BehaviourEditorWindowDX lContainerEditor)
        {
            ShaderResourceView lTexture;

            Point lFormPosition;

            if (Port.OwningNode != null)
            {
                // If we are an external node, we move with our parent node and hence the camera
                lFormPosition = mPosition.GetFormPosition(lContainerEditor.ParentControl, lContainerEditor.CameraPosition);
            }
            else
            {
                // Otherwise we are an internal node so we stay fixed to the edge of the screen
                lFormPosition = mPosition.GetPositionFixed();
            }

            Vector2 lLabelPosition;
            Vector2 lStringSize = lContainerEditor.TextRenderer.MeasureString(Port.PortID.ToString()).Size;
            if (Port.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lTexture = mIsHovering ?
                    lContainerEditor.GetTexture(BehaviourEditorWindowDX.eTextureID.InPort_Hover) :
                    lContainerEditor.GetTexture(BehaviourEditorWindowDX.eTextureID.InPort);

                // The offset of the label from the position of the port. 
                float lXPosition;

                if (Port.OwningNode == null)
                {
                    // Is a internal port going in, so on the right hand side
                    // => text is on the left of the port
                    lXPosition = -(lStringSize.X + 3);
                }
                else // OwningNode is not null
                {
                    // Is an external port going in, so on the left hand side
                    // => text is on the right of the port
                    lXPosition = PortSize.X + 3;
                }

                // Position    =           Location of the port + the offset determined above to shift it to left or right of the port
                lLabelPosition = new Vector2(lFormPosition.X + lXPosition,
                    //Location of the port + half the port (to get to the middle) - half the hight of the text (to align the middle of the text)
                    lFormPosition.Y + (0.5f * PortSize.Y) - (0.5f * lStringSize.Y));
            }
            else // PortDirection == PortDirection_Out
            {
                lTexture = mIsHovering ?
                    lContainerEditor.GetTexture(BehaviourEditorWindowDX.eTextureID.OutPort_Hover) :
                    lContainerEditor.GetTexture(BehaviourEditorWindowDX.eTextureID.OutPort);

                // The offset of the label from the position of the port. 
                float lXPosition;

                if (Port.OwningNode == null)
                {
                    // Is a internal port going out, so on the right hand side
                    // => text is on the right of the port
                    lXPosition = PortSize.X + 3;
                    
                }
                else // OwningNode is not null
                {
                    // Is an external port going out, so on the right hand side
                    // => text is on the left of the port
                    lXPosition = -(lStringSize.X + 3);
                }

                // Position    =           Location of the port + the offset determined above to shift it to left or right of the port
                lLabelPosition = new Vector2(lFormPosition.X + lXPosition, 
                    //Location of the port + half the port (to get to the middle) - half the hight of the text (to align the middle of the text)
                    lFormPosition.Y + (0.5f * PortSize.Y) - (0.5f * lStringSize.Y));
            }

            lContainerEditor.SpriteRenderer.Draw(lTexture, new Vector2(lFormPosition.X, lFormPosition.Y) , PortSize, SpriteTextRenderer.CoordinateType.Absolute);
            lContainerEditor.TextRenderer.DrawString(Port.PortID, lLabelPosition, new Color4(Color.White));
        }
    }
}
