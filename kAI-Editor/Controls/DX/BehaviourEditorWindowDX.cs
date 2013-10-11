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
using System.Drawing;

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
            NodeTexture,
            InPort,
            InPort_Hover,
            OutPort,
            OutPort_Hover,

            TextureCount
        }

        // Positions of the first internal ports on the left and right. 
        readonly Point kOutPortStartPosition = new Point(0, 5);
        readonly int kPortDeltaY = (int)kAIEditorPortDX.sPortSize.Y + 5;

        static Random sRandom = new Random();

        /// <summary>
        /// When two ports have been connected. 
        /// </summary>
        public event Action<kAI.Core.kAIPort, kAI.Core.kAIPort> OnConnexion;

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
        NodeCoordinate mCameraPosition;
        Point mLastMousePoint;

        // GUI points for laying out new ports (not readonly as depends on dynamic data). 
        Point mInPortStartPosition;

        // Represents the positions for the next port to be added to the behaviour. 
        Point mCurrentInPosition;
        Point mCurrentOutPosition;

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

        /// <summary>
        /// The input manager for this editor. 
        /// </summary>
        public kAIInputManagerDX InputManager
        {
            get;
            private set;
        }

        /// <summary>
        /// The location in absolute space of the camera. 
        /// TODO: Not sure why this needs to be a property or whatever...?
        /// </summary>
        public NodeCoordinate CameraPosition
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

            mCameraPosition = new NodeCoordinate(0, 0);
        }

        /// <summary>
        /// Set up the editor window within a given control. 
        /// </summary>
        /// <param name="lParentControl">The control to embed the editor in to. </param>
        public void Init(Control lParentControl)
        {
            ParentControl = lParentControl;

            InputManager = new kAIInputManagerDX(lParentControl, this);

            InputManager.OnMouseDown += new EventHandler<MouseEventArgs>(InputManager_OnMouseDown);

            mInPortStartPosition = new Point(ParentControl.Width - (int)kAIEditorPortDX.sPortSize.X, 5);

            mCurrentInPosition = mInPortStartPosition;
            mCurrentOutPosition = kOutPortStartPosition;

            // Do all the DX stuff, should probably be in a different class or something. 
            // TODO: These things => memory leaks, need to be moved
            Device device;
            ShaderSignature inputSignature;
            VertexShader vertexShader;
            PixelShader pixelShader;

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

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, description, out device, out mSwapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(mSwapChain, 0))
                mRenderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            mContext = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, lParentControl.ClientSize.Width, lParentControl.ClientSize.Height);
            mContext.OutputMerger.SetTargets(mRenderTarget);
            mContext.Rasterizer.SetViewports(viewport);

            // load and compile the vertex shader, TODO: Relative paths...
            using (var bytecode = ShaderBytecode.CompileFromFile(@"E:\dev\C#\kAI\kAI-Editor\Assets\triangle.fx", "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile(@"E:\dev\C#\kAI\kAI-Editor\Assets\triangle.fx", "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            // TODO: maybe this doesn't go here...?

            // create the vertex layout and buffer
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };
            var layout = new InputLayout(device, inputSignature, elements);
            // configure the Input Assembler portion of the pipeline with the vertex data

            mContext.InputAssembler.InputLayout = layout;
            mContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

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

                foreach (kAIEditorPortDX lInternalPort in mPorts)
                {
                    if (lInternalPort.Port.PortDirection == kAIPort.ePortDirection.PortDirection_In) 
                    {
                        lInternalPort.UpdatePosition(-lDeltaX, 0);
                        lInternalPort.FinalisePosition();
                    }
                }
            };

            // Create text rendering stuff
            SpriteRenderer = new SpriteRenderer(device);
            TextRenderer = new TextBlockRenderer(SpriteRenderer, "Arial", FontWeight.Normal, SlimDX.DirectWrite.FontStyle.Normal, FontStretch.Normal, 12.0f);

            // Create the shader resources for each of the textures
            mTextures = new ShaderResourceView[(int)eTextureID.TextureCount];
            mTextures[(int)eTextureID.NodeTexture] = new ShaderResourceView(device, Texture2D.FromFile(device, @"E:\dev\C#\kAI\kAI-Editor\Assets\Node.png"));
            mTextures[(int)eTextureID.InPort] = new ShaderResourceView(device, Texture2D.FromFile(device, @"E:\dev\C#\kAI\kAI-Editor\Assets\InPort.png"));
            mTextures[(int)eTextureID.InPort_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, @"E:\dev\C#\kAI\kAI-Editor\Assets\InPort_Hover.png"));
            mTextures[(int)eTextureID.OutPort] = new ShaderResourceView(device, Texture2D.FromFile(device, @"E:\dev\C#\kAI\kAI-Editor\Assets\OutPort.png"));
            mTextures[(int)eTextureID.OutPort_Hover] = new ShaderResourceView(device, Texture2D.FromFile(device, @"E:\dev\C#\kAI\kAI-Editor\Assets\OutPort_Hover.png"));

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
        /// Unload the behaviour from the editor (eg delete all nodes, ports and connexions). 
        /// </summary>
        public void UnloadBehaviour()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Add a node to the render of the behaviour. 
        /// </summary>
        /// <param name="lNode">The node to render.  </param>
        public void AddNode(kAI.Core.kAINode lNode)
        {
            mNodes.Add(new kAIEditorNodeDX(lNode, new NodeCoordinate(sRandom.Next(500), sRandom.Next(500)), new Size(200, 100), this));
        }

        /// <summary>
        /// Remove a specific node from the render of the behaviour. 
        /// </summary>
        /// <param name="lNode">The node to stop rendering. </param>
        public void RemoveNode(kAI.Core.kAINode lNode)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Add a render of a connexion between two ports. 
        /// </summary>
        /// <param name="lConnexion">The connexion to render. </param>
        public void AddConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
          //  throw new NotImplementedException();
        }

        /// <summary>
        /// Stop rendering a connexion between two ports. 
        /// </summary>
        /// <param name="lConnexion">The connexion to stop rendering. </param>
        public void RemoveConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
           // throw new NotImplementedException();
        }

        /// <summary>
        /// Start rendering an internal port. 
        /// </summary>
        /// <param name="lPort">The internal port to render. </param>
        public void AddInternalPort(kAI.Core.kAIPort lPort)
        {
            NodeCoordinate lPos;
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                lPos = new NodeCoordinate(mCurrentInPosition.X, mCurrentInPosition.Y);
                mCurrentInPosition.Offset(0, kPortDeltaY);
            }
            else
            {
                lPos = new NodeCoordinate(mCurrentOutPosition.X, mCurrentOutPosition.Y);
                mCurrentOutPosition.Offset(0, kPortDeltaY);
            }

            kAIEditorPortDX lEditorPort = new kAIEditorPortDX(lPort, lPos, this);
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
            mContext.ClearRenderTargetView(mRenderTarget, Color.DarkGray);

            // 3D render with vertices (abandoned)
            //Render();

            // 2D render using SlimDX SpriteTextRenderer http://sdxspritetext.codeplex.com/
            Render2D();

            // Swap the back and front buffers
            mSwapChain.Present(0, PresentFlags.None);            
        }

        /// <summary>
        /// Dispose all of the SlimDX COM objects. 
        /// </summary>
        public void Destroy()
        {
            TextRenderer.Dispose();
            SpriteRenderer.Dispose();

            for (eTextureID lTexture = (eTextureID)0; lTexture < eTextureID.TextureCount; ++lTexture)
            {
                mTextures[(int)lTexture].Dispose();
            }

            mContext.Device.Dispose();
            mSwapChain.Dispose();
            mContext.Dispose();
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

        void InputManager_OnMouseUp(object sender, MouseEventArgs e)
        {
            InputManager.OnMouseMove -= InputManager_OnMouseMove;
            InputManager.OnMouseUp -= InputManager_OnMouseUp;
        }

        void InputManager_OnMouseMove(object sender, MouseEventArgs e)
        {
            mCameraPosition.Translate(mLastMousePoint.X - e.X, mLastMousePoint.Y - e.Y);
            mLastMousePoint = e.Location;
        }

        void InputManager_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!InputManager.MouseOnSomething)
            {
                mLastMousePoint = e.Location;
                InputManager.OnMouseMove += new EventHandler<MouseEventArgs>(InputManager_OnMouseMove);
                InputManager.OnMouseUp += new EventHandler<MouseEventArgs>(InputManager_OnMouseUp);
            }
        }
    }
}