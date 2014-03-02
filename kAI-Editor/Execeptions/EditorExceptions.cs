using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;
using kAI.Editor.Core;
namespace kAI.Editor.Exceptions
{
    abstract class EditorException : Exception
    {

    }

    class kAIEditorNodeNotFoundException : EditorException
    {
        /// <summary>
        /// The ID of the node we were trying to fin. 
        /// </summary>
        public kAINodeID NodeID
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a EditorPort not not
        /// </summary>
        /// <param name="lPortId"></param>
        public kAIEditorNodeNotFoundException(kAINodeID lNodeId)
        {
            NodeID = lNodeId;
        }
    }
}
