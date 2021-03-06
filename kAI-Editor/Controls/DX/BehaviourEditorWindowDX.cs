﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.RawInput;
using SlimDX.Multimedia;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using Buffer = SlimDX.Direct3D11.Buffer;
using Factory = SlimDX.DXGI.Factory;
using SlimDX.DirectWrite;
using SpriteTextRenderer;

using kAI.Core;
using kAI.Editor.Controls.DX.Coordinates;

using kAI.Core.Debug;
using System.Drawing;
using kAI.Editor.Core;
using kAI.Editor.Exceptions;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// The DirectX implementation of the editor window. 
    /// </summary>
    class kAIBehaviourEditorWindowDX : kAIIBehaviourEditorGraphicalImplementator
    {
        /// <summary>
        /// IDs of the textures.
        /// </summary>
        /// <seealso cref="kAIBehaviourEditorWindowDX.GetTexture"/>
        public enum eTextureID
        {
            InPort,
            InPort_Hover,
            OutPort,
            OutPort_Hover,
            InPortData,
            InPortData_Hover,
            OutPortData,
            OutPortData_Hover,
            NodeLowerTexture,
            NodeUpperTexture, 
            EnabledIcon,
            DisabledIcon,
            TextureCount
        }

        // Positions of the first internal ports on the left and right. 
        readonly Point kOutPortStartPosition = new Point(0, 5);
        readonly int kPortDeltaY = (int)kAIEditorPortDX.sPortSize.Y + 5;

        /// <summary>
        /// When two ports have been connected. 
        /// </summary>
        public event Action<kAI.Core.kAIPort, kAI.Core.kAIPort> OnConnexion;

        /// <summary>
        /// When an object is selected. 
        /// </summary>
        public event Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry> ObjectSelected;

        // DX stuff:
        DeviceContext mContext;
        SwapChain mSwapChain;
        RenderTargetView mRenderTarget;

        // The loaded textures.
        ShaderResourceView[] mTextures;

        // Nodes currently being rendered.
        List<kAIEditorNodeDX> mNodes;

        // Internal ports currently being rendered.
        List<kAIEditorPortDX> mPorts;

        // Location of the camera.
        // TODO: abstract the camera stuff
        kAIAbsolutePosition mCameraPosition;
        Point mLastMousePoint;

        bool mIsDragging;

        // GUI points for laying out new ports (not readonly as depends on dynamic data). 
        Point mInPortStartPosition;

        // Represents the positions for the next port to be added to the behaviour. 
        Point mCurrentInPosition;
        Point mCurrentOutPosition;

        ShaderSignature inputSignature;
        Device device;

        VertexShader vertexShader;
        PixelShader pixelShader;

        // Counters used for debugging the R-Tree rectangles
        int mRTreeDebugDepthShow = 0;
        int mRTreeDebugCounter = 0;

        /// <summary>
        /// The SpriteRenderer to use for rendering standard 2D textures. 
        /// </summary>
        public SpriteRenderer SpriteRenderer
        {
            get;
            private set;
        }

        /// <summary>
        /// The TextRenderer to use for rendering standard text. 
        /// </summary>
        public TextBlockRenderer TextRenderer
        {
            get;
            private set;
        }

        /// <summary>
        /// Control that holds the renderer. 
        /// </summary>
        public Control ParentControl
        {
            get;
            private set;
        }

        public kAIBehaviourEditorWindow Editor
        {
            get;
            private set;
        }

        /// <summary>
        /// The input manager for this editor. 
        /// </summary>
        public kAIInputManagerDX InputManager
        {
            get;
            private set;
        }

        public kAIPortConnexionCreator ConnexionCreator
        {
            get;
            private set;
        }

        /// <summary>
        /// The location in absolute space of the camera. 
        /// TODO: Not sure why this needs to be a property or whatever...?
        /// </summary>
        public kAIAbsolutePosition CameraPosition
        {
            get
            {
                return mCameraPosition;
            }
        }        

        /// <summary>
        /// Create a behaviour editor window using DirectX. 
        /// </summary>
        public kAIBehaviourEditorWindowDX()
        {
            mNodes = new List<kAIEditorNodeDX>();
            mPorts = new List<kAIEditorPortDX>();
        }

        /// <summary>
        /// Set up the editor window within a given control. 
        /// </summary>
        /// <param name="lParentControl">The control to embed the editor in to. </param>
        /// <param name="lWindow">The editor that is controlling the implementation. </param>
        public void Init(Control lParentControl, kAIBehaviourEditorWindow lWindow)
        {
            const string lAssetsFolder = @"Assets\";
            
            ParentControl = lParentControl;
            Editor = lWindow;

            int lHalfWidth = ParentControl.Width / 2;
            int lHalfHeight = ParentControl.Height / 2;
            mCameraPosition = new kAIAbsolutePosition(-lHalfWidth, -lHalfHeight, false);

            InputManager = new kAIInputManagerDX(lParentControl, this);

            ConnexionCreator = new kAIPortConnexionCreator(Editor);

            InputManager.OnMouseDown += new EventHandler<MouseEventArgs>(InputManager_OnMouseDown);

            mInPortStartPosition = new Point(ParentControl.Width - (int)kAIEditorPortDX.sPortSize.X, 5);

            mCurrentInPosition = mInPortStartPosition;
            mCurrentOutPosition = kOutPortStartPosition;

            // Do all the DX stuff, should probably be in a different class or something. 
            // TODO: These things => memory leaks, need to be moved
            

            var description = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = lParentControl.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out mSwapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(mSwapChain, 0))
                mRenderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            mContext = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, lParentControl.ClientSize.Width, lParentControl.ClientSize.Height);
            mContext.OutputMerger.SetTargets(mRenderTarget);
            mContext.Rasterizer.SetViewports(viewport);

            // load and compile the vertex shader, TODO: Relative paths...
            using (var bytecode = ShaderBytecode.CompileFromFile(lAssetsFolder + "triangle.fx", "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile(lAssetsFolder + "triangle.fx", "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            // set the shaders
            mContext.VertexShader.Set(vertexShader);
            mContext.PixelShader.Set(pixelShader);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = mSwapChain.GetParent<Factory>())
                factory.SetWindowAssociation(lParentControl.Handle, WindowAssociationFlags.IgnoreAltEnter);

            lParentControl.Resize += (o, e) =>
            {
                // Resize the viewport and the buffer
                var lNewViewport = new Viewport(0.0f, 0.0f, lParentControl.ClientSize.Width, lParentControl.ClientSize.Height);
                mContext.Rasterizer.SetViewports(lNewViewport);
                mRenderTarget.Dispose();

                mSwapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(mSwapChain, 0))
                {
                    mRenderTarget = new RenderTargetView(device, resource);
                }

                mContext.OutputMerger.SetTargets(mRenderTarget);
                SpriteRenderer.RefreshViewport();

                int lNewX = ParentControl.Width - (int)kAIEditorPortDX.sPortSize.X;
                int lDeltaX = lNewX - mCurrentInPosition.X;

                // Move the point to take in to account the new size
                mInPortStartPosition = new Point(lNewX, 5);
                mCurrentInPosition.X = mInPortStartPosition.X;

                foreach (kAIEditorPortDX lInternalPort in mPorts.FindAll((lPort) =>
                    { return lPort.Port.PortDirection == kAIPort.ePortDirection.PortDirection_In; }))
                {
                    lInternalPort.UpdatePosition(lDeltaX, 0);
                    lInternalPort.FinalisePosition();
                }

                InvalidateConnexionPositions();
            };

            // Create text rendering stuff
            SpriteRenderer = new SpriteRenderer(device, 7001);
            TextRenderer = new TextBlockRenderer(SpriteRenderer, "Arial", FontWeight.Normal, SlimDX.DirectWrite.FontStyle.Normal, FontStretch.Normal, 12.0f);

            // Create the shader resources for each of the textures
            mTextures = new ShaderResourceView[(int)eTextureID.TextureCount];
            //mTextures[(int)eTextureID.NodeTexture] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "Node.png"));
            mTextures[(int)eTextureID.InPort] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "InPort.png"));
            mTextures[(int)eTextureID.InPort_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "InPort_Hover.png"));
            mTextures[(int)eTextureID.OutPort] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "OutPort.png"));
            mTextures[(int)eTextureID.OutPort_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "OutPort_Hover.png"));

            mTextures[(int)eTextureID.InPortData] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "InPortData.png"));
            mTextures[(int)eTextureID.InPortData_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "InPortData_Hover.png"));
            mTextures[(int)eTextureID.OutPortData] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "OutPortData.png"));
            mTextures[(int)eTextureID.OutPortData_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "OutPortData_Hover.png"));//_hober


            mTextures[(int)eTextureID.NodeLowerTexture] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "NodeLower.png"));
            mTextures[(int)eTextureID.NodeUpperTexture] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "NodeUpper.png"));
            mTextures[(int)eTextureID.DisabledIcon] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "DisabledIcon.png"));
            mTextures[(int)eTextureID.EnabledIcon] = new ShaderResourceView(device, Texture2D.FromFile(device, lAssetsFolder + "EnabledIcon.png"));

        }

        /// <summary>
        /// Get a specific texture from the id. 
        /// </summary>
        /// <param name="lTextureID">The ID of the texture. </param>
        /// <returns>The texture ready to be drawn. </returns>
        public ShaderResourceView GetTexture(eTextureID lTextureID)
        {
            int lTexIdInt = (int)lTextureID;
            kAIObject.Assert(null, lTexIdInt < (int)eTextureID.TextureCount, "Invalid texture id");

            return mTextures[lTexIdInt];
        }

        /// <summary>
        /// Tells all the connexions to recalculate their path. 
        /// </summary>
        public void InvalidateConnexionPositions()
        {
            foreach (kAIEditorPortDX lInternalPort in mPorts.FindAll((lPort) => 
                { return lPort.Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out; }))
            {
                lInternalPort.InvalidateConnexionPositions();
            }

            foreach (kAIEditorNodeDX lNode in mNodes)
            {
                lNode.InvalidateConnexionPositions();
            }
        }

        /// <summary>
        /// Create a path between two ports (at the moment, is just a straight line,
        /// could be extended to avoid objects etc.). 
        /// </summary>
        /// <param name="lConnexion">The connexion to create a path between</param>
        /// <returns>A list of absolute points to connect. </returns>
        public List<kAIAbsolutePosition> GetPointPath(kAIPort.kAIConnexion lConnexion)
        {
            List<kAIAbsolutePosition> lPoints = new List<kAIAbsolutePosition>();

            kAIEditorPortDX lStart = GetPort(lConnexion.StartPort);
            kAIEditorPortDX lEnd = GetPort(lConnexion.EndPort);

            kAIAbsolutePosition lStartPosition = lStart.GetConnexionPoint();
            kAIAbsolutePosition lEndPosition = lEnd.GetConnexionPoint();
            lPoints.Add(lStartPosition);

            kAIAbsolutePosition lStartIn;
            kAIAbsolutePosition lEndIn;

            if (lStart.Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                lStartIn = lStartPosition.Translate(10, 0);
                lEndIn = lEndPosition.Translate(-10, 0);
            }
            else
            {
                lStartIn = lStartPosition.Translate(-10, 0);
                lEndIn = lEndPosition.Translate(10, 0);
            }

            lPoints.Add(lStartIn);

            if(!lStartPosition.mIsFixed && !lEndPosition.mIsFixed )
            {
                kAIAbsolutePosition lMid1 = kAIAbsolutePosition.MidYFirstX(lStartIn, lEndIn);
                kAIAbsolutePosition lMid2 = kAIAbsolutePosition.MidYSecondX(lStartIn, lEndIn);


                lPoints.Add(lMid1);
                lPoints.Add(lMid2);
            }
            lPoints.Add(lEndIn);
            lPoints.Add(lEndPosition);

            return lPoints;
        }

        public bool CanConnect()
        {
            return CanDrag();
        }

        /// <summary>
        /// Can a node begin dragging now.
        /// </summary>
        /// <returns>True if the node is allowed to drag. </returns>
        public bool CanDrag()
        {
            bool lCanDrag = true;
            lCanDrag &= !mIsDragging; // Can't already be dragging a node
            lCanDrag &= !ConnexionCreator.CreatingLink; // Can't be making a link

            return lCanDrag;
        }

        /// <summary>
        /// Inform the editor that a node is being dragged (eg to stop other nodes being dragged). 
        /// </summary>
        public void StartDrag()
        {
            mIsDragging = true;
        }

        /// <summary>
        /// Inform the editor the dragging is complete. 
        /// </summary>
        public void StopDrag()
        {
            mIsDragging = false;
        }

        /// <summary>
        /// Unload the behaviour from the editor (eg delete all nodes, ports and connexions). 
        /// </summary>
        public void UnloadBehaviour()
        {
            mPorts.Clear();
            mNodes.Clear();

            InputManager.ClearInputRectangles();

            mCurrentInPosition = mInPortStartPosition;
            mCurrentOutPosition = kOutPortStartPosition;
        }

        /// <summary>
        /// Add a node to the render of the behaviour. 
        /// </summary>
        /// <param name="lNode">The node to render. </param>
        /// <param name="lPoint">The position relateive to the form for where to add this node. </param>
        public void AddNode(kAI.Core.kAINode lNode, kAIAbsolutePosition lPoint)
        {
            kAIEditorNodeDX lNewNode = new kAIEditorNodeDX(lNode, lPoint, new kAIAbsoluteSize(200, 0), this);
            lNewNode.OnSelected += new Action<ObjectProperties.kAIIPropertyEntry>(lObject_OnSelected);
            mNodes.Add(lNewNode);
        }

        /// <summary>
        /// Add a node to the render of the behaviour. 
        /// </summary>
        /// <param name="lNode"></param>
        /// <param name="lPoint"></param>
        public void AddNode(kAI.Core.kAINode lNode, Point lPoint)
        {
            AddNode(lNode, new kAIAbsolutePosition(new kAIRelativePosition(lPoint), mCameraPosition, false));
        }

        /// <summary>
        /// Remove a specific node from the render of the behaviour. 
        /// </summary>
        /// <param name="lNode">The node to stop rendering. </param>
        public void RemoveNode(kAI.Core.kAINode lNode)
        {
            mNodes.Remove(GetNode(lNode));
        }

        /// <summary>
        /// Add a render of a connexion between two ports. 
        /// </summary>
        /// <param name="lConnexion">The connexion to render. </param>
        public void AddConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
            kAIPort lOutPort = lConnexion.StartPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out ?
                lConnexion.StartPort : lConnexion.EndPort;

            kAIEditorPortDX lStartEditorPort = GetPort(lOutPort);
            lStartEditorPort.UpdateConnexions();
        }

        /// <summary>
        /// Stop rendering a connexion between two ports. 
        /// </summary>
        /// <param name="lConnexion">The connexion to stop rendering. </param>
        public void RemoveConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
            kAIPort lOutPort = lConnexion.StartPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out ?
                lConnexion.StartPort : lConnexion.EndPort;

            kAIEditorPortDX lStartEditorPort = GetPort(lOutPort);
            lStartEditorPort.UpdateConnexions();
        }

        /// <summary>
        /// Start rendering an internal port. 
        /// </summary>
        /// <param name="lPort">The internal port to render. </param>
        public void AddInternalPort(kAI.Core.kAIPort lPort)
        {
            kAIAbsolutePosition lPos;
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lPos = new kAIAbsolutePosition(mCurrentInPosition.X, mCurrentInPosition.Y, true);
                mCurrentInPosition.Offset(0, kPortDeltaY);
            }
            else
            {
                lPos = new kAIAbsolutePosition(mCurrentOutPosition.X, mCurrentOutPosition.Y, true);
                mCurrentOutPosition.Offset(0, kPortDeltaY);
            }

            kAIEditorPortDX lEditorPort = new kAIEditorPortDX(lPort, lPos, this);
            lEditorPort.OnSelected += lObject_OnSelected;
            mPorts.Add(lEditorPort);
        }

        /// <summary>
        /// Stop rendering an internal port. 
        /// </summary>
        /// <param name="lPort">The port to stop rendering. </param>
        public void RemoveInternalPort(kAIPort lPort)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the absolute positions of each of the nodes. 
        /// </summary>
        /// <returns>A enumeration of each of the node's IDs and its absolute position. </returns>
        public IEnumerable<Tuple<kAINodeID, kAIAbsolutePosition>> GetNodePositions()
        {
            foreach (kAIEditorNodeDX lEdiorNode in mNodes)
            {
                yield return new Tuple<kAINodeID, kAIAbsolutePosition>(lEdiorNode.Node.NodeID, lEdiorNode.Position);
            }
        }

        /// <summary>
        /// Sets the position of a specific node. 
        /// </summary>
        /// <param name="lNodeID">The node id to position. </param>
        /// <param name="lPoint">The absolute position where the node should be positioned. </param>
        public void SetNodePosition(kAINodeID lNodeID, kAIAbsolutePosition lPoint)
        {
            GetNode(lNodeID).SetPosition(lPoint);
        }

        /// <summary>
        /// Start rendering an external port for a specific node. 
        /// </summary>
        /// <param name="lParentNode">The owning node of the port. </param>
        /// <param name="lPort">The port to render. </param>
        public void AddExternalPort(kAI.Core.kAINode lParentNode, kAI.Core.kAIPort lPort)
        {
          //  throw new NotImplementedException();
        }

        /// <summary>
        /// Stop rendering an external port. 
        /// </summary>
        /// <param name="lParentNode">The node the port belongs to. </param>
        /// <param name="lPort">The port to stop rendering. </param>
        public void RemoveExternalPort(kAINode lParentNode, kAIPort lPort)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Perform the draw of DirectX
        /// </summary>
        public void EditorUpdate()
        {
            // clear the render target to a stylish grey
            mContext.ClearRenderTargetView(mRenderTarget, Color.LightGray);

            // 3D render with vertices (abandoned)
            //Render();

            // 2D render using SlimDX SpriteTextRenderer http://sdxspritetext.codeplex.com/
            Render2D();

            LineRender();            

            // Swap the back and front buffers
            mSwapChain.Present(0, PresentFlags.None);    
    
        }

        /// <summary>
        /// Dispose all of the SlimDX COM objects. 
        /// </summary>
        public void Destroy()
        {
            TextRenderer.Dispose();
            TextRenderer = null;

            SpriteRenderer.Dispose();
            SpriteRenderer = null;

            for (eTextureID lTexture = (eTextureID)0; lTexture < eTextureID.TextureCount; ++lTexture)
            {
                mTextures[(int)lTexture].Resource.Dispose();
                mTextures[(int)lTexture].Dispose();
            }
            mTextures = null;

            mRenderTarget.Dispose();
            mRenderTarget = null;

            vertexShader.Dispose();
            vertexShader = null;

            pixelShader.Dispose();
            pixelShader = null;

            mContext.Device.Dispose();
            mSwapChain.Dispose();
            mSwapChain = null;

            mContext.Dispose();
            mContext = null;
        }

        private void LineRender()
        {
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0), new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0) };
            var layout = new InputLayout(mContext.Device, inputSignature, elements);
            // configure the Input Assembler portion of the pipeline with the vertex data

            // Configure the Contexts input assembler (must be done to undo what the 2D render does).
            mContext.InputAssembler.InputLayout = layout;

            mContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;

            // TODO: would somehow like to set the geometry shader to some kind of beizure curve
            mContext.GeometryShader.Set(null);
            mContext.VertexShader.Set(vertexShader);
            mContext.PixelShader.Set(pixelShader);

            foreach (kAIEditorPortDX lPort in mPorts)
            {
                lPort.LineRender();
            }

            foreach (kAIEditorNodeDX lNode in mNodes)
            {
                lNode.LineRender();
            }
            
            //RenderRTreeRects();

            layout.Dispose();
        }

        void RenderRTreeRects()
        {
            int lMaxDepth = 0;

            foreach (Tuple<Rectangle, int, int> lRect in InputManager.GetRectangles())
            {

                List<kAIAbsolutePosition> lRectangleVerts = new List<kAIAbsolutePosition>
                { 
                    new kAIAbsolutePosition(lRect.Item1.Left, lRect.Item1.Top, false), 
                    new kAIAbsolutePosition(lRect.Item1.Right, lRect.Item1.Top, false),
                    new kAIAbsolutePosition(lRect.Item1.Right, lRect.Item1.Bottom, false),
                    new kAIAbsolutePosition(lRect.Item1.Left, lRect.Item1.Bottom, false),
                    new kAIAbsolutePosition(lRect.Item1.Left, lRect.Item1.Top, false)
                };


                if (lRect.Item2 == mRTreeDebugDepthShow)
                {
                    switch (lRect.Item3)
                    {
                        case -1:
                        case 0:
                            RenderLine(lRectangleVerts, Color.Red);
                            break;

                        case 1:
                            RenderLine(lRectangleVerts, Color.Green);
                            break;

                        case 2:
                            RenderLine(lRectangleVerts, Color.Blue);
                            break;

                        case 3:
                            RenderLine(lRectangleVerts, Color.White);
                            break;

                        default:
                            RenderLine(lRectangleVerts, Color.Yellow);
                            break;
                    }

                }

                lMaxDepth = Math.Max(lRect.Item2, lMaxDepth);
            }
            
            if (mRTreeDebugCounter == 5000)
            {
                ++mRTreeDebugDepthShow;
                mRTreeDebugDepthShow %= (lMaxDepth + 1);
                mRTreeDebugCounter = 0;
            }
            else
            {
                ++mRTreeDebugCounter;
            }
        }

        public void RenderLine(List<kAIAbsolutePosition> lPoints, Color4 colour = default(Color4))
        {
            // TODO: move the line renderer in to own class then can cause exception if function called at the wrong time
            DataStream lVertices = new DataStream(32 * (lPoints.Count), true, true);

            foreach (kAIAbsolutePosition lPoint in lPoints)
            {
                // TODO: These points aren't correct
                kAINormalisedPosition lNormalised = new kAINormalisedPosition(lPoint, CameraPosition, ParentControl);
                lVertices.Write(new Vector4(lNormalised.GetAsV3(), 1.0f));
                lVertices.Write(colour.ToVector4());
            }

            lVertices.Position = 0;

            Buffer lVertexBuffer = new Buffer(device, lVertices, 32 * (lPoints.Count), ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            mContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lVertexBuffer, 32, 0));
            mContext.Draw(lPoints.Count , 0);

            lVertices.Dispose();
            lVertexBuffer.Dispose();
        }

        /// <summary>
        /// Deprecated: Performs the 3D render using vertices, does not work very well.
        /// </summary>
        private void Render()
        {
            // Number of vertices used per mesh
            const int kNumberVertices = 4;
            // If we have some nodes, we draw them
            if (mNodes.Count > 0)
            {
                // Fill a stream with vertices, 12 bytes per vert (x, y, z) * number of verts per node * number of nodes
                var vertices = new DataStream(12 * kNumberVertices * mNodes.Count, true, true);
                foreach (kAIEditorNodeDX lNode in mNodes)
                {

                    lNode.Render(vertices, ParentControl, CameraPosition);
                }

                // Reset the vertices stream to the start for filling in the buffer
                vertices.Position = 0;

                // Fill the buffer to draw from
                var vertexBuffer = new Buffer(mContext.Device, vertices, 12 * kNumberVertices * mNodes.Count, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                // Set the vertex buffer
                mContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));

                // For each of the nodes, we draw its collection of vertices
                for (int i = 0; i < mNodes.Count; ++i)
                {
                    mContext.Draw(kNumberVertices, kNumberVertices * i);
                }

                // Free the buffer and stream
                vertexBuffer.Dispose();
                vertices.Dispose();

            }
        }

        /// <summary>
        /// Performs the 2D render using SpriteTextRenderer dll. 
        /// </summary>
        private void Render2D()
        {
            foreach (kAIEditorNodeDX lNode in mNodes)
            {
                lNode.Render2D(this);
            }

            foreach (kAIEditorPortDX lPort in mPorts)
            {
                lPort.Render2D(this);
            }

            SpriteRenderer.Flush();
        }

        /// <summary>
        /// Get the editor port of some port somewhere in the behaviour (can be internal or external). 
        /// </summary>
        /// <param name="lPort">The port to find. </param>
        /// <returns>The EditorPort that represents this port. </returns>
        private kAIEditorPortDX GetPort(kAIPort lPort)
        {
            return GetPort(lPort.FQPortID);
        }

        private kAIEditorPortDX GetPort(kAIFQPortID lPortId)
        {
            if (lPortId.NodeID == kAINodeID.InvalidNodeID)
            {
                return mPorts.Find((lPort) => { return lPort.Port.PortID == lPortId.PortID; });
            }
            else
            {
                kAIEditorNodeDX lOwningNode = GetNode(lPortId.NodeID);
                return lOwningNode.GetExternalPort(lPortId.PortID);
            }
            
        }

        private kAIEditorNodeDX GetNode(kAINode lNode)
        {
            return GetNode(lNode.NodeID);
        }

        private kAIEditorNodeDX GetNode(kAINodeID lNodeID)
        {
            foreach (kAIEditorNodeDX lEditorNode in mNodes)
            {
                if (lEditorNode.Node.NodeID == lNodeID)
                {
                    return lEditorNode;
                }
            }

            throw new kAIEditorNodeNotFoundException(lNodeID);
        }

        void InputManager_OnMouseUp(object sender, MouseEventArgs e)
        {
            InputManager.OnMouseMove -= InputManager_OnMouseMove;
            InputManager.OnMouseUp -= InputManager_OnMouseUp;
        }

        void InputManager_OnMouseMove(object sender, MouseEventArgs e)
        {
            mCameraPosition = mCameraPosition.Translate(mLastMousePoint.X - e.X, mLastMousePoint.Y - e.Y);
            mLastMousePoint = e.Location;
       } 

        void InputManager_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!InputManager.MouseOnSomething)
            {
                if (e.Button == MouseButtons.Left)
                {
                    mLastMousePoint = e.Location;
                    InputManager.OnMouseMove += new EventHandler<MouseEventArgs>(InputManager_OnMouseMove);
                    InputManager.OnMouseUp += new EventHandler<MouseEventArgs>(InputManager_OnMouseUp);
                }
            }
        }

        void lObject_OnSelected(ObjectProperties.kAIIPropertyEntry obj)
        {
            if (ObjectSelected != null)
            {
                ObjectSelected(obj);
            }
        }


            
        public void SetDebugInfo(kAI.Core.Debug.kAIXmlBehaviourDebugInfo lDebugInfo)
        {
            foreach (kAINodeDebugInfo lNodeDebugInfo in lDebugInfo.InternalNodes)
            {
                try
                {
                    GetNode(lNodeDebugInfo.NodeID).SetDebugInfo(lNodeDebugInfo);
                }
                catch (kAIEditorNodeNotFoundException e)
                {
                    GlobalServices.Logger.LogError("Could not find node with ID: " + e.NodeID + ", disconnecting debugger...");
                    GlobalServices.Debugger.DisconnectDebugger();
                    return;
                }                
            }

            foreach (kAIPortDebugInfo lPortDebugInfo in lDebugInfo.InternalPorts)
            {
                GetPort(lPortDebugInfo.PortID).SetDebugInfo(lPortDebugInfo);
            }
        }

        public void ClearDebugInfo()
        {
            foreach (kAIEditorNodeDX lNode in mNodes)
            {
                lNode.ClearDebugInfo();
            }

            foreach (kAIEditorPortDX lPort in mPorts)
            {
                lPort.ClearDebugInfo();
            }
        }
    }
}
