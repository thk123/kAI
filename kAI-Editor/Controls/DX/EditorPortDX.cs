using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using SlimDX;

using kAI.Core;
using SlimDX.Direct3D11;
using kAI.Editor.Controls.DX.Coordinates;
using kAI.Editor.Controls.WinForms;
using kAI.Editor.ObjectProperties;
using kAI.Core.Debug;

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

        // Used to determine when we have started dragging from this port
        // we set to true if the mouse is left-pressed on the button, and then start
        // dragging if the mouse then leaves port with the mouse still down
        bool mDidLeftClick;

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
        public kAIAbsolutePosition Position
        {
            get;
            private set;
        }

        kAIPortDebugInfo mDebugInfo;

        /// <summary>
        /// Triggered when this port is selected. 
        /// </summary>
        public event Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry> OnSelected;

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
        public kAIEditorPortDX(kAIPort lPort, kAIAbsolutePosition lPosition, kAIBehaviourEditorWindowDX lEditorWindow)
        {
            Port = lPort;
            Position = lPosition;

            mIsHovering = false;

            mEditorWindow = lEditorWindow;

            MenuItem lRemoveConnexion = new MenuItem("Remove connexion...");
            lRemoveConnexion.Click += new EventHandler(lRemoveConnexion_Click);

            MenuItem lRemoveAllConnexions = new MenuItem("Remove all connexions");
            lRemoveAllConnexions.Click += new EventHandler(lRemoveAllConnexions_Click);

            mAddedRectangle = new Rectangle(Position.mPoint, new Size((int)sPortSize.X, (int)sPortSize.Y));

            mEditorWindow.InputManager.AddClickListenArea(mAddedRectangle,
                new kAIMouseEventResponders
                {
                    OnMouseEnter = OnHover,
                    OnMouseLeave = OnLeave,
                    OnMouseDown = OnMouseDown,
                    OnMouseUp = OnMouseUp,
                    OnMouseClick = OnMouseClick,
                    ContextMenu = new ContextMenu(new MenuItem[] { lRemoveConnexion, lRemoveAllConnexions }),
                    RectangleId = Port.OwningNodeID + ":" + Port.PortID
                },
                Port.OwningNode == null); // if the port is an internal node (ie no owning node) then it doesn't move with the camera

            mConnexions = new List<kAIEditorConnexionDX>();

            if(Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                foreach (kAIPort.kAIConnexion lConnexion in Port.Connexions)
                {
                    mConnexions.Add(new kAIEditorConnexionDX(lConnexion, mEditorWindow));
                }
            }


            Port.OnDisconnected += new kAIPort.ConnexionEvent(Port_OnDisconnected);
        }

        void Port_OnDisconnected(kAIPort lSender, kAIPort lOtherEnd)
        {
            if(Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                kAIObject.Assert(null, lSender == Port);
                int lNumberRemoved = mConnexions.RemoveAll((lConnexion) => { return lConnexion.End == lOtherEnd; });
                kAIObject.Assert(null, lNumberRemoved == 1);
            }
        }

        void lRemoveConnexion_Click(object sender, EventArgs e)
        {
            SelectConnexionDialogue lConnexionSelector = new SelectConnexionDialogue(Port, mEditorWindow.Editor.Behaviour.GetConnectedPorts(Port));

            DialogResult lResult = lConnexionSelector.ShowDialog();

            if (lResult == DialogResult.OK)
            {
                foreach (kAIPort.kAIConnexion lConnexion in lConnexionSelector.GetPortsToDisconnect())
                {
                    mEditorWindow.Editor.RemoveConnexion(lConnexion);
                }
            }
        }

        void lRemoveAllConnexions_Click(object sender, EventArgs e)
        {
            // Must store and then do otherwise we modify the collection inside the behaviour as we iterate.
            List<kAIPort> lPortsToDisconnect = new List<kAIPort>();
            foreach (kAIPort.kAIConnexion lConnexion in mEditorWindow.Editor.Behaviour.GetConnectedPorts(Port))
            {
                kAIPort lOtherEnd = lConnexion.EndPort.PortID == Port.PortID ? lConnexion.StartPort : lConnexion.EndPort;
                lPortsToDisconnect.Add(lOtherEnd);
                
            }

            foreach (kAIPort lPortToDisconnect in lPortsToDisconnect)
            {
                mEditorWindow.Editor.RemoveConnexion(Port, lPortToDisconnect);
            }
        }

        /// <summary>
        /// Gets the absolute position that connexion lines should be drawn to/from for this port. 
        /// </summary>
        /// <returns>An absolute position of where the connexion line should run to/from. </returns>
        public kAIAbsolutePosition GetConnexionPoint()
        {
            kAIAbsolutePosition lConnexionPoint = Position;
            int lXTranslation;

            if (Port.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lXTranslation = 5;
            }
            else // PortDirection == PortDirection_Out
            {
                lXTranslation = (int)sPortSize.X - 5;
            }
            
            lConnexionPoint = lConnexionPoint.Translate(lXTranslation, (int)(sPortSize.Y * 0.5f));

            return lConnexionPoint;
        }

        /// <summary>
        /// Used to move the port in the event of the attached node moving
        /// or the editor being resized.
        /// </summary>
        /// <param name="ldX">The change in x position. </param>
        /// <param name="ldY">The change in y position. </param>
        public void UpdatePosition(int ldX, int ldY)
        {
            Position = Position.Translate(ldX, ldY);
        }

        /// <summary>
        /// Rescan the node for additional ports.
        /// TODO: This feels nasty, maybe the Port could have an event?
        /// </summary>
        public void UpdateConnexions()
        {
            mConnexions.Clear();
            if (Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                foreach (kAIPort.kAIConnexion lConnexion in Port.Connexions)
                {
                    mConnexions.Add(new kAIEditorConnexionDX(lConnexion, mEditorWindow));
                }
            }
        }

        /// <summary>
        /// Tell the port that it has finished moving so should update its rectangle in the InputManager. 
        /// </summary>
        public void FinalisePosition()
        {
            kAIMouseEventResponders lResponder = mEditorWindow.InputManager.RemoveClickListenArea(mAddedRectangle, Port.OwningNode == null);

            mAddedRectangle = new Rectangle(Position.mPoint, new Size((int)sPortSize.X, (int)sPortSize.Y));

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

            kAIRelativePosition lFormPosition = new kAIRelativePosition(Position, lContainerEditor.CameraPosition);

            Vector2 lLabelPosition;
            Color lLabelColour;
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
                    lLabelColour = Color.FromArgb(64, 64, 64);
                }
                else // OwningNode is not null
                {
                    // Is an external port going in, so on the left hand side
                    // => text is on the right of the port
                    lXPosition = sPortSize.X + 3;
                    lLabelColour = Color.White;
                }

                // Position    =           Location of the port + the offset determined above to shift it to left or right of the port
                lLabelPosition = new Vector2(lFormPosition.mPoint.X + lXPosition,
                    //Location of the port + half the port (to get to the middle) - half the hight of the text (to align the middle of the text)
                    lFormPosition.mPoint.Y + (0.5f * sPortSize.Y) - (0.5f * lStringSize.Y));
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
                    lLabelColour = Color.FromArgb(64, 64, 64);
                }
                else // OwningNode is not null
                {
                    // Is an external port going out, so on the right hand side
                    // => text is on the left of the port
                    lXPosition = -(lStringSize.X + 3);
                    lLabelColour = Color.White;
                }

                // Position    =           Location of the port + the offset determined above to shift it to left or right of the port
                lLabelPosition = new Vector2(lFormPosition.mPoint.X + lXPosition, 
                    //Location of the port + half the port (to get to the middle) - half the hight of the text (to align the middle of the text)
                    lFormPosition.mPoint.Y + (0.5f * sPortSize.Y) - (0.5f * lStringSize.Y));
            }

            lContainerEditor.SpriteRenderer.Draw(lTexture, new Vector2(lFormPosition.mPoint.X, lFormPosition.mPoint.Y), sPortSize, SpriteTextRenderer.CoordinateType.Absolute);
            lContainerEditor.TextRenderer.DrawString(Port.PortID, lLabelPosition, new Color4(lLabelColour));

            if (mDebugInfo != null)
            {
                kAITriggerPortDebugInfo lTriggerInfo = mDebugInfo as kAITriggerPortDebugInfo;
                if(lTriggerInfo != null)
                {
                    TimeSpan lTimeSinceLastTrigger = DateTime.Now - lTriggerInfo.LastTimeTriggered;

                    if (lTimeSinceLastTrigger <= new TimeSpan(0, 0, 0, 0, 500))
                    {
                        float lAlpha = 1.0f - ((float)lTimeSinceLastTrigger.Milliseconds / 500.0f);
                        lContainerEditor.SpriteRenderer.Draw(lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.EnabledIcon),
                            new Vector2(lFormPosition.mPoint.X - 8, lFormPosition.mPoint.Y - 8), new Vector2(16, 16), new Color4(lAlpha ,1,1,1), SpriteTextRenderer.CoordinateType.Absolute);
                    }
                }
                else
                {
                    kAIDataPortDebugInfo lDataInfo = mDebugInfo as kAIDataPortDebugInfo;
                    if(lDataInfo != null)
                    {
                        string lDefaultText = Port.DataType.DataType.GetDefault() == null ? "[NULL]" : Port.DataType.DataType.GetDefault().ToString();
                        if (lDataInfo.CurrentData == lDefaultText || lDataInfo.CurrentData == "false")
                        {
                            float lAlpha = 1.0f;
                            lContainerEditor.SpriteRenderer.Draw(lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.DisabledIcon),
                                new Vector2(lFormPosition.mPoint.X - 8, lFormPosition.mPoint.Y - 8), new Vector2(16, 16), new Color4(lAlpha, 1, 1, 1), SpriteTextRenderer.CoordinateType.Absolute);
                        }
                        else
                        {
                            float lAlpha = 1.0f;
                            lContainerEditor.SpriteRenderer.Draw(lContainerEditor.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.EnabledIcon),
                                new Vector2(lFormPosition.mPoint.X - 8, lFormPosition.mPoint.Y - 8), new Vector2(16, 16), new Color4(lAlpha, 1, 1, 1), SpriteTextRenderer.CoordinateType.Absolute);
                        }
                    }
                }
            }

            foreach (kAIEditorConnexionDX lConnexion in mConnexions)
            {
                lConnexion.Render2D();
            }
        }

        /// <summary>
        /// Perform the line render on this port (essentially render all of its connexions). 
        /// </summary>
        public void LineRender()
        {
            foreach (kAIEditorConnexionDX lConnexion in mConnexions)
            {
                lConnexion.LineRender();
            }
        }

        /// <summary>
        /// Update all the routes all the connexions out of this port are taking.
        /// </summary>
        public void InvalidateConnexionPositions()
        {
            foreach (kAIEditorConnexionDX lEditorPort in mConnexions)
            {
                lEditorPort.InvalidatePath();
            }
        }

        /// <summary>
        /// Is the port represented here an internal port or an external port. 
        /// </summary>
        /// <returns>True if the port is an internal port (e.g. has now owning node). </returns>
        private bool IsInternalPort()
        {
            return Port.OwningNode == null;
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
            
            if (e.Button == MouseButtons.Left && mDidLeftClick)
            {
                mEditorWindow.ConnexionCreator.PortDown(Port);
            }
        }

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mDidLeftClick = true;
            }
        }

        void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!mDidLeftClick)
            {
                mEditorWindow.ConnexionCreator.PortUp(Port);
                mDidLeftClick = false;
            }
        }

        void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (OnSelected != null)
            {
                OnSelected(new kAIPortProperties(Port, mDebugInfo));
            }
        }

        internal void SetDebugInfo(kAIPortDebugInfo lDebugInfo)
        {
            mDebugInfo = lDebugInfo;
        }

        internal void ClearDebugInfo()
        {
            mDebugInfo = null;
        }
    }
}
