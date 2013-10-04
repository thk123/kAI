using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;

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
        }
    }
}
