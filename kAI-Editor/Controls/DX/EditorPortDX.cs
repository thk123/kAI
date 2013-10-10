using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

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
        /// The size of the port graphic. 
        /// </summary>
        public static readonly Vector2 sPortSize;        

        // Is the mouse currently hovering over the node. 
        bool mIsHovering;

        // Reference to the containing editor window. 
        kAIBehaviourEditorWindowDX mEditorWindow;

        // The rectangle we are currently registered with the input manager with
        // used to remove ourselves from the input manager when moved. 
        Rectangle mAddedRectangle;

        List<kAIEditorConnexionDX> mConnexions;

        /// <summary>
        /// The port this GUI element is representing.
        /// </summary>
        public kAIPort Port
        {
            get;
            private set;
        }

        /// <summary>
        /// Absolute position of the node.
        /// </summary>
        public NodeCoordinate Position
        {
            get;
            private set;
        }

        static kAIEditorPortDX()
        {
            sPortSize = new Vector2(Properties.Resources.InPort.Width, Properties.Resources.InPort.Height);
        }

        /// <summary>
        /// Creates a new GUI element representing a given port. 
        /// </summary>
        /// <param name="lPort">The port being represented. </param>
        /// <param name="lPosition">The position of the port. </param>
        /// <param name="lEditorWindow">The editor window this node belongs to. </param>
        public kAIEditorPortDX(kAIPort lPort, NodeCoordinate lPosition, kAIBehaviourEditorWindowDX lEditorWindow)
        {
            Port = lPort;
            Position = lPosition;

            mIsHovering = false;

            mEditorWindow = lEditorWindow;

            mAddedRectangle = new Rectangle(Position.GetPositionFixed(), new Size((int)sPortSize.X, (int)sPortSize.Y));

            mEditorWindow.InputManager.AddClickListenArea(mAddedRectangle, 
                new kAIMouseEventResponders{ OnMouseHover = OnHover, OnMouseLeave = OnLeave, RectangleId = Port.OwningNodeID + ":" + Port.PortID},
                Port.OwningNode == null); // if the port is an internal node (ie no owning node) then it doesn't move with the camera

            mConnexions = new List<kAIEditorConnexionDX>();

            if(Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                foreach (kAIPort.kAIConnexion lConnexion in Port.Connexions)
                {
                    mConnexions.Add(new kAIEditorConnexionDX(lConnexion, mEditorWindow));
                }
            }

        }

        /// <summary>
        /// Used to move the port in the event of the attached node moving
        /// or the editor being resized.
        /// </summary>
        /// <param name="ldX">The change in x position. </param>
        /// <param name="ldY">The change in y position. </param>
        public void UpdatePosition(int ldX, int ldY)
        {
            //mPosition.Translate(ldX, ldY);
            Point lOldPoint = Position.GetPositionFixed();
            Position = new NodeCoordinate(lOldPoint.X - ldX, lOldPoint.Y - ldY);
        }

        /// <summary>
        /// Tell the port that it has finished moving so should update its rectangle in the InputManager. 
        /// </summary>
        public void FinalisePosition()
        {
            kAIMouseEventResponders lResponder = mEditorWindow.InputManager.RemoveClickListenArea(mAddedRectangle, Port.OwningNode == null);

            mAddedRectangle = new Rectangle(Position.GetPositionFixed(), new Size((int)sPortSize.X, (int)sPortSize.Y));

            mEditorWindow.InputManager.AddClickListenArea(mAddedRectangle,
                lResponder,
                Port.OwningNode == null); // if the port is an internal node (ie no owning node) then it doesn't move with the camera
        }

        /// <summary>
        /// Draw the port on the screen. 
        /// </summary>
        /// <param name="lContainerEditor">The editor window to draw the port in. </param>
        public void Render2D(kAIBehaviourEditorWindowDX lContainerEditor)
        {
            ShaderResourceView lTexture;

            Point lFormPosition;

            if (Port.OwningNode != null)
            {
                // If we are an external node, we move with our parent node and hence the camera
                lFormPosition = Position.GetFormPosition(lContainerEditor.ParentControl, lContainerEditor.CameraPosition);
            }
            else
            {
                // Otherwise we are an internal node so we stay fixed to the edge of the screen
                lFormPosition = Position.GetPositionFixed();
            }

            Vector2 lLabelPosition;
            Vector2 lStringSize = lContainerEditor.TextRenderer.MeasureString(Port.PortID.ToString()).Size;
            if (Port.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lTexture = mIsHovering ?
                    lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.InPort_Hover) :
                    lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.InPort);

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
                    lXPosition = sPortSize.X + 3;
                }

                // Position    =           Location of the port + the offset determined above to shift it to left or right of the port
                lLabelPosition = new Vector2(lFormPosition.X + lXPosition,
                    //Location of the port + half the port (to get to the middle) - half the hight of the text (to align the middle of the text)
                    lFormPosition.Y + (0.5f * sPortSize.Y) - (0.5f * lStringSize.Y));
            }
            else // PortDirection == PortDirection_Out
            {
                lTexture = mIsHovering ?
                    lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.OutPort_Hover) :
                    lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.OutPort);

                // The offset of the label from the position of the port. 
                float lXPosition;

                if (Port.OwningNode == null)
                {
                    // Is a internal port going out, so on the right hand side
                    // => text is on the right of the port
                    lXPosition = sPortSize.X + 3;
                    
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
                    lFormPosition.Y + (0.5f * sPortSize.Y) - (0.5f * lStringSize.Y));
            }

            lContainerEditor.SpriteRenderer.Draw(lTexture, new Vector2(lFormPosition.X, lFormPosition.Y) , sPortSize, SpriteTextRenderer.CoordinateType.Absolute);
            lContainerEditor.TextRenderer.DrawString(Port.PortID, lLabelPosition, new Color4(Color.White));

            foreach (kAIEditorConnexionDX lConnexion in mConnexions)
            {
                lConnexion.Render2D();
            }
        }

        public void LineRender()
        {
            foreach (kAIEditorConnexionDX lConnexion in mConnexions)
            {
                lConnexion.LineRender();
            }
        }

        void lUnderlyingControl_MouseLeave(object sender, EventArgs e)
        {
            mIsHovering = false;
        }

        void lUnderlyingControl_MouseHover(object sender, EventArgs e)
        {
            mIsHovering = true;
        }

        void OnHover(object sender, MouseEventArgs e)
        {
            mIsHovering = true;
        }

        void OnLeave(object sender, MouseEventArgs e)
        {
            mIsHovering = false;
        }
    }
}
