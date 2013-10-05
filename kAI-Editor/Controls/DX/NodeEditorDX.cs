﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

using SlimDX;
using SpriteTextRenderer;
using SlimDX.Direct3D11;



namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// Represents a renderable node within a behaviour for DirectX.
    /// </summary>
    class kAINodeEditorDX
    {
        /// <summary>
        /// The position of the node in absolute pixels. 
        /// </summary>
        public NodeCoordinate Position
        {
            get;
            private set;

        }

        /// <summary>
        /// The size of the node in absolute pixels. 
        /// </summary>
        public Size Size
        {
            get;
            private set;
        }        

        kAINode mNode;

        // GUI points (for laying out elements of the node). 
        readonly Point kNodeNamePosition = new Point(5, 5);

        /// <summary>
        /// Create a new node for rendering. 
        /// </summary>
        /// <param name="lNode">The node this renderable node represents. </param>
        /// <param name="lPoint">The location of the node in absolute pixels. </param>
        /// <param name="lSize">The size of the node in absolute pixels. </param>
        public kAINodeEditorDX(kAINode lNode, NodeCoordinate lPoint, Size lSize)
        {
            Position = lPoint;
            Size = lSize;
            mNode = lNode;
        }

        /// <summary>
        /// Deprecated: 3D render using vertices to create a quad representing the node. 
        /// </summary>
        /// <param name="lVertexStream">The vertex stream to fill with vertices. </param>
        /// <param name="lParentControl">The control the nodes are within. </param>
        /// <param name="lCameraPos">The position of the camera. </param>
        public void Render(DataStream lVertexStream, Control lParentControl, NodeCoordinate lCameraPos)
        {
            // Get a vector3 of where this position is in normalised space ([-1, 1] x [-1, 1])
            Vector3 lNodePositionNormalised = Position.GetNormalisedPositionV3(lParentControl, lCameraPos);

            // Get a vector3 representing what the width and height are in normalised space ([-1, 1] x [-1, 1]
            Vector3 lNodeSizeNormalised = Size.GetNormalisedSizeFromSizeV3(lParentControl);

            Vector3 lTopLeft, lTopRight, lBottomLeft, lBottomRight;

            lTopLeft = lNodePositionNormalised;
            lTopRight = lNodePositionNormalised + Vector3.Modulate(Vector3.UnitX, lNodeSizeNormalised);
            lBottomRight = lNodePositionNormalised + lNodeSizeNormalised;
            lBottomLeft = lNodePositionNormalised + Vector3.Modulate(Vector3.UnitY, lNodeSizeNormalised);

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
        public void Render2D(BehaviourEditorWindowDX lEditorWindow)
        {
            // Get the position for the square 
            Point lFormPosition = Position.GetFormPosition(lEditorWindow.ParentControl, lEditorWindow.CameraPosition);

            // Render the box for the node
            lEditorWindow.SpriteRenderer.Draw(lEditorWindow.GetTexture(BehaviourEditorWindowDX.eTextureID.NodeTexture), new Vector2(lFormPosition.X, lFormPosition.Y), new Vector2(Size.Width, Size.Height), CoordinateType.Absolute);

            // Render the node label.
            lEditorWindow.TextRenderer.DrawString(mNode.NodeID.ToString(), new Vector2(lFormPosition.X + kNodeNamePosition.X, lFormPosition.Y + kNodeNamePosition.Y), new Color4(Color.White));

        }
    }
}