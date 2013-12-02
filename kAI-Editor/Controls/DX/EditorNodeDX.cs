using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.ObjectProperties;
using kAI.Editor.Controls.DX.Coordinates;

using SlimDX;
using SpriteTextRenderer;
using SlimDX.Direct3D11;
using kAI.Editor.Core;


namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// Represents a renderable node within a behaviour for DirectX.
    /// </summary>
    class kAIEditorNodeDX
    {
        // GUI points (for laying out elements of the node). 
        readonly Point kNodeNamePosition = new Point(5, 5);
        readonly Point kInPortStartPosition = new Point(-(int)kAIEditorPortDX.sPortSize.X, 30);
        readonly Point kOutPortStartPosition;
        readonly int kPortDeltaY = (int)kAIEditorPortDX.sPortSize.Y + 5;

        // The set of ports this node has
        List<kAIEditorPortDX> mExternalPorts;

        // A reference to the editor window. 
        kAIBehaviourEditorWindowDX mEditorWindow;
        
        // Represents the positions for the next port to be added to the behaviour. 
        Point mCurrentInPosition;
        Point mCurrentOutPosition;

        // Used for dragging the node about.
        bool mBeingDragged;

        // The rectangle we were at, so we can remove it. 
        Rectangle mAddedRectangle;

        /// <summary>
        /// The node that is being represented.  
        /// </summary>
        public kAINode Node
        {
            get;
            private set;
        }

        kAIAbsolutePosition mPosition;

        /// <summary>
        /// The position of the node in absolute pixels. 
        /// </summary>
        public kAIAbsolutePosition Position
        {
            get
            {
                return mPosition;
            }
            private set
            {
                // We store the old position
                Point lOldPoint = mPosition.mPoint;
                mPosition = value;

                // Work out the delta between the two points
                int lDX = mPosition.mPoint.X - lOldPoint.X;
                int lDY = mPosition.mPoint.Y - lOldPoint.Y;

                // And use these to move the ports accordingly. 
                foreach (kAIEditorPortDX lExternalPort in mExternalPorts)
                {
                    lExternalPort.UpdatePosition(lDX, lDY);
                }
            }
        }

        /// <summary>
        /// The size of the node in absolute pixels. 
        /// </summary>
        public kAIAbsoluteSize Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Triggered when this object is selected. 
        /// </summary>
        public event Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry> OnSelected;

        /// <summary>
        /// Create a new node for rendering. 
        /// </summary>
        /// <param name="lNode">The node this renderable node represents. </param>
        /// <param name="lPoint">The location of the node in absolute pixels. </param>
        /// <param name="lSize">The size of the node in absolute pixels. </param>
        /// <param name="lEditorWindow">The editor window this node belongs to. </param>
        public kAIEditorNodeDX(kAINode lNode, kAIAbsolutePosition lPoint, kAIAbsoluteSize lSize, kAIBehaviourEditorWindowDX lEditorWindow)
        {
            mPosition = lPoint;
            Size = lSize;
            Node = lNode;
            mBeingDragged = false;

            mEditorWindow = lEditorWindow;

            kOutPortStartPosition = new Point(lSize.mSize.Width, 30);

            mCurrentInPosition = kInPortStartPosition;
            mCurrentOutPosition = kOutPortStartPosition;

            IEnumerable<kAIPort> lExternalPorts = lNode.GetExternalPorts();
            mExternalPorts = new List<kAIEditorPortDX>();
            foreach (kAIPort lExternalPort in lExternalPorts)
            {
                AddExternalPort(lExternalPort);
            }

            MenuItem lDeleteNode = new MenuItem("Delete Node");
            lDeleteNode.Click += new EventHandler(lDeleteNode_Click);

            mAddedRectangle = new Rectangle(Position.mPoint, Size.mSize);

            lEditorWindow.InputManager.AddClickListenArea(mAddedRectangle,
                new kAIMouseEventResponders
                {
                    OnMouseDown = OnMouseDown,
                    OnMouseClick = OnMouseClick,
                    OnMouseDoubleClick = OnMouseDoubleClick,
                    ContextMenu = new ContextMenu(new MenuItem[] { lDeleteNode }),
                    RectangleId = Node.NodeID
                },
                false);

            lNode.OnExternalPortAdded += new kAINode.ExternalPortAdded(lNode_OnExternalPortAdded);
        }     

        /// <summary>
        /// Deprecated: 3D render using vertices to create a quad representing the node. 
        /// </summary>
        /// <param name="lVertexStream">The vertex stream to fill with vertices. </param>
        /// <param name="lParentControl">The control the nodes are within. </param>
        /// <param name="lCameraPos">The position of the camera. </param>
        public void Render(DataStream lVertexStream, Control lParentControl, kAIAbsolutePosition lCameraPos)
        {
            // Get a vector3 of where this position is in normalised space ([-1, 1] x [-1, 1])
            kAINormalisedPosition lNodePositionNormalised = new kAINormalisedPosition(Position, lCameraPos, lParentControl);
            

            // Get a vector3 representing what the width and height are in normalised space ([-1, 1] x [-1, 1]
            kAINormalisedSize lNodeSizeNormalised = new kAINormalisedSize(Size, lParentControl);

            Vector3 lTopLeft, lTopRight, lBottomLeft, lBottomRight;

            lTopLeft = lNodePositionNormalised.GetAsV3();
            lTopRight = lNodePositionNormalised.GetAsV3() + Vector3.Modulate(Vector3.UnitX, lNodeSizeNormalised.GetAsV3());
            lBottomRight = lNodePositionNormalised.GetAsV3() + lNodeSizeNormalised.GetAsV3();
            lBottomLeft = lNodePositionNormalised.GetAsV3() + Vector3.Modulate(Vector3.UnitY, lNodeSizeNormalised.GetAsV3());

            // We are a triangle strip so we can draw the quad using only 4 vertices
            lVertexStream.Write(lTopLeft);
            lVertexStream.Write(lBottomLeft);
            lVertexStream.Write(lTopRight);
            lVertexStream.Write(lBottomRight);
        }

        /// <summary>
        /// Render the node using the SlimDX Sprite and Text Renderer. 
        /// </summary>
        /// <param name="lEditorWindow">The parent window to render into. </param>
        public void Render2D(kAIBehaviourEditorWindowDX lEditorWindow)
        {
            // Get the position for the square 
            // Point lFormPosition = Position.GetFormPosition(lEditorWindow.ParentControl, lEditorWindow.CameraPosition);
            kAIRelativePosition lFormPosition = new kAIRelativePosition(Position, lEditorWindow.CameraPosition);
            kAIRelativeSize lFormSize = new kAIRelativeSize(Size);

            // Render the box for the node
            lEditorWindow.SpriteRenderer.Draw(lEditorWindow.GetTexture(kAIBehaviourEditorWindowDX.eTextureID.NodeTexture), new Vector2(lFormPosition.mPoint.X, lFormPosition.mPoint.Y), new Vector2(lFormSize.mSize.Width, lFormSize.mSize.Height), CoordinateType.Absolute);

            // Render the node label.
            lEditorWindow.TextRenderer.DrawString(Node.NodeID.ToString(), new Vector2(lFormPosition.mPoint.X + kNodeNamePosition.X, lFormPosition.mPoint.Y + kNodeNamePosition.Y), new Color4(Color.White));

            foreach (kAIEditorPortDX lEditorPort in mExternalPorts)
            {
                lEditorPort.Render2D(lEditorWindow);
            }
        }


        /// <summary>
        /// Perform the line render pass (currently just connexions). 
        /// </summary>
        public void LineRender()
        {
            foreach (kAIEditorPortDX lPort in mExternalPorts)
            {
                lPort.LineRender();
            }
        }

        /// <summary>
        /// Inform all connexions that their path may be invalid. 
        /// </summary>
        public void InvalidateConnexionPositions()
        {
            foreach(kAIEditorPortDX lPort in mExternalPorts.Where((lPort) => { return lPort.Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out; }))
            {
                lPort.InvalidateConnexionPositions();
            }
        }        

        /// <summary>
        /// Add an external port to this node. 
        /// </summary>
        /// <param name="lPort"></param>
        void AddExternalPort(kAIPort lPort)
        {
            kAIObject.Assert(null, lPort.OwningNode == Node, "Tried to set as an external port a port which is not related to this node");

            kAIAbsolutePosition lPositionForPort;
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lPositionForPort = Position.Add(new kAIAbsolutePosition(mCurrentInPosition.X, mCurrentInPosition.Y, false));
                mCurrentInPosition.Offset(0, kPortDeltaY);
            }
            else
            {
                lPositionForPort = Position.Add(new kAIAbsolutePosition(mCurrentOutPosition.X, mCurrentOutPosition.Y, false));
                mCurrentOutPosition.Offset(0, kPortDeltaY);
            }
            kAIEditorPortDX lEditorPort = new kAIEditorPortDX(lPort, lPositionForPort, mEditorWindow);
            lEditorPort.OnSelected += new Action<kAIIPropertyEntry>(lEditorPort_OnSelected);
            mExternalPorts.Add(lEditorPort);
        }

        void lEditorPort_OnSelected(kAIIPropertyEntry obj)
        {
            if (OnSelected != null)
            {
                OnSelected(obj);
            }
        }

        /// <summary>
        /// Get the EditorPort for a specific port. 
        /// </summary>
        /// <param name="lPort">The port to retrieve.</param>
        /// <returns>The EditorPort (visual representation) of the specified port. </returns>
        public kAIEditorPortDX GetExternalPort(kAIPort lPort)
        {
            foreach (kAIEditorPortDX lEditorPort in mExternalPorts)
            {
                if (lEditorPort.Port.PortID == lPort.PortID)
                {
                    return lEditorPort;
                }
            }

            kAIObject.Assert(null, false, "Could not find specified external port on this node.");
            return null;
        }

        public void SetPosition(kAIAbsolutePosition lAbsolutePosition)
        {
            Position = lAbsolutePosition;
            FinalisePosition();
        }

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!mBeingDragged && mEditorWindow.CanDrag())
            {
                mEditorWindow.StartDrag();

                // We start dragging so we now listen to these events. 
                mEditorWindow.InputManager.OnMouseMove += new EventHandler<MouseEventArgs>(InputManager_OnMouseMove);
                mEditorWindow.InputManager.OnMouseUp += new EventHandler<MouseEventArgs>(InputManager_OnMouseUp);

                mBeingDragged = true;
            }
        }

        void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (OnSelected != null)
            {
                OnSelected(new kAINodeProperties(Node));
            }
        }

        void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            // If it was a kAI-Behaviour, we load it.
            kAIINodeSerialObject lObject = Node.GetNodeSerialisableContent(null);
            
            if (lObject.GetNodeFlavour() == eNodeFlavour.BehaviourXml)
            {
                mEditorWindow.Editor.LoadBehaviour(kAIXmlBehaviour.Load(lObject, GlobalServices.LoadedProject.GetAssemblyByName));
            }
        }

        void InputManager_OnMouseMove(object sender, MouseEventArgs e)
        {
            // Update the position to the location of the mouse
            kAIRelativePosition lRelativePosition = new kAIRelativePosition(e.Location);
            Position = new kAIAbsolutePosition(lRelativePosition, mEditorWindow.CameraPosition, false);
        }

        void InputManager_OnMouseUp(object sender, MouseEventArgs e)
        {
            kAIObject.Assert(null, mBeingDragged, "Stopping dragging an object that wasn't being dragged");
            // We have finished dragging so we stop listenign to these events. 
            mEditorWindow.InputManager.OnMouseMove -= InputManager_OnMouseMove;
            mEditorWindow.InputManager.OnMouseUp -= InputManager_OnMouseUp;

            FinalisePosition();

            mBeingDragged = false;

            mEditorWindow.StopDrag();
        }

        void FinalisePosition()
        {
            Rectangle lNewRectanlge = new Rectangle(Position.mPoint, Size.mSize);
            if(mAddedRectangle != lNewRectanlge)
            {
                // Remove the old rectangle. 
                kAIMouseEventResponders lResponder = mEditorWindow.InputManager.RemoveClickListenArea(mAddedRectangle, false);
                kAIObject.Assert(null, lResponder, "Could not find node!");

                // Create and add the new rectangle. 
                mAddedRectangle = lNewRectanlge;
                mEditorWindow.InputManager.AddClickListenArea(mAddedRectangle, lResponder, false);

                // Tell the ports we are done moving (so they can remove their old rectangles and add the new ones). 
                foreach (kAIEditorPortDX lExternalPort in mExternalPorts)
                {
                    lExternalPort.FinalisePosition();
                }

                // Since we have moved some ports we may need to recalculate the lines used to represent the connexions. 
                mEditorWindow.InvalidateConnexionPositions();
            }
        }



        void lDeleteNode_Click(object sender, EventArgs e)
        {
            mEditorWindow.InputManager.RemoveClickListenArea(mAddedRectangle, false);
            mEditorWindow.Editor.RemoveNode(Node);
        }


        void lNode_OnExternalPortAdded(kAINode lSender, kAIPort lNewPort)
        {
            AddExternalPort(lNewPort);
        }  
    }
}
