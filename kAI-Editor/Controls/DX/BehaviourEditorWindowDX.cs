using System;
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

using kAI.Core;
using System.Drawing;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// The DirectX implementation of the editor window. 
    /// </summary>
    class BehaviourEditorWindowDX : kAIIBehaviourEditorGraphicalImplementator
    {
        // DX stuff:
        DeviceContext context;
        SwapChain swapChain;
        RenderTargetView renderTarget;

        // Control that holds the renderer. 
        Control mParentControl;

        // Nodes currently being rendered.
        List<kAINodeEditorDX> mNodes;

        // Mouse management stuff, abstract me!
        float zoom = 1.0f;
        int tick = 0;
        bool mMouseDown = false;

        // Location of the camera.
        NodeCoordinate mCameraPosition = new NodeCoordinate(0, 0);

        /// <summary>
        /// Create a behaviour editor window using DirectX. 
        /// </summary>
        public BehaviourEditorWindowDX()
        {
            mNodes = new List<kAINodeEditorDX>();
        }

        /// <summary>
        /// Set up the editor window within a given control. 
        /// </summary>
        /// <param name="lParentControl">The control to embed the editor in to. </param>
        public void Init(Control lParentControl)
        {
            mParentControl = lParentControl;

            // Listen for mouse events, needs to be abstracted. 
            SlimDX.RawInput.Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, SlimDX.RawInput.DeviceFlags.None);
            SlimDX.RawInput.Device.MouseInput += new EventHandler<MouseInputEventArgs>(Device_MouseInput);           

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

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            context = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, lParentControl.ClientSize.Width, lParentControl.ClientSize.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

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

            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            // set the shaders
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(lParentControl.Handle, WindowAssociationFlags.IgnoreAltEnter);

            lParentControl.Resize += (o, e) =>
            {
                // Resize the viewport and the buffer
                var lNewViewport = new Viewport(0.0f, 0.0f, lParentControl.ClientSize.Width, lParentControl.ClientSize.Height);
                context.Rasterizer.SetViewports(lNewViewport);
                renderTarget.Dispose();

                swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                {
                    renderTarget = new RenderTargetView(device, resource);
                }

                context.OutputMerger.SetTargets(renderTarget);
            };
        }

        void Device_MouseInput(object sender, MouseInputEventArgs e)
        {

            int value = tick + (e.WheelDelta / 120);
            tick = value;
            zoom = (float)Math.Pow(2.0, (double)value / 10.0);

            if (e.ButtonFlags.HasFlag(MouseButtonFlags.LeftDown))
            {
                mMouseDown = true;
                
            }
            else if (e.ButtonFlags.HasFlag(MouseButtonFlags.LeftUp))
            {
                mMouseDown = false;
            }

            if (mMouseDown)
            {

                mCameraPosition.Translate(-e.X, e.Y);
            }
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
          //  throw new NotImplementedException();
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
            context.ClearRenderTargetView(renderTarget, Color.DarkGray);
            
            // Number of vertices used per mesh
            const int kNumberVertices = 4;

            // If we have some nodes, we draw them
            if(mNodes.Count > 0)
            {

                // Fill a stream with vertices, 12 bytes per vert (x, y, z) * number of verts per node * number of nodes
                var vertices = new DataStream(12 * kNumberVertices * mNodes.Count, true, true);
                foreach (kAINodeEditorDX lNode in mNodes)
                {
                    Size lNodeSize = lNode.Size; // The size in pixels of the node

                    NodeCoordinate lNodePosition = lNode.Position; // The position in global pixels of the node

                    // Get a vector3 of where this position is in normalised space ([-1, 1] x [-1, 1])
                    Vector3 lNodePositionNormalised = lNodePosition.GetNormalisedPositionV3(mParentControl, mCameraPosition);

                    // Get a vector3 representing what the width and height are in normalised space ([-1, 1] x [-1, 1]
                    Vector3 lNodeSizeNormalised = lNodeSize.GetNormalisedSizeFromSizeV3(mParentControl);

                    Vector3 lTopLeft, lTopRight, lBottomLeft, lBottomRight;

                    lTopLeft = lNodePositionNormalised;
                    lTopRight = lNodePositionNormalised + Vector3.Modulate(Vector3.UnitX, lNodeSizeNormalised);
                    lBottomRight = lNodePositionNormalised + lNodeSizeNormalised;
                    lBottomLeft = lNodePositionNormalised + Vector3.Modulate(Vector3.UnitY, lNodeSizeNormalised);

                    // We are a triangle strip so we can draw the quad using only 4 vertices
                    vertices.Write(lTopLeft); 
                    vertices.Write(lBottomLeft);
                    vertices.Write(lTopRight);
                    vertices.Write(lBottomRight);
                }
                
                // Reset the vertices stream to the start for filling in the buffer
                vertices.Position = 0;

                // Fill the buffer to draw from
                var vertexBuffer = new Buffer(context.Device, vertices, 12 * kNumberVertices * mNodes.Count, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

                // Set the vertex buffer
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));
    
                // For each of the nodes, we draw its collection of vertices
                for (int i = 0; i < mNodes.Count; ++i)
                {
                    context.Draw(kNumberVertices, kNumberVertices * i);
                }

                // Free the buffer and stream
                vertexBuffer.Dispose();
                vertices.Dispose();
            }

            // Swap the back and front buffers
            swapChain.Present(0, PresentFlags.None);
        }

        /// <summary>
        /// Dispose all of the SlimDX COM objects. 
        /// </summary>
        public void Destroy()
        {
            context.Device.Dispose();
            swapChain.Dispose();
            context.Dispose();
            
        }

        /// <summary>
        /// When two ports have been connected. 
        /// </summary>
        public event Action<kAI.Core.kAIPort, kAI.Core.kAIPort> OnConnexion;

        

    }
}
