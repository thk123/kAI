using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a specific node. 
    /// </summary>
    [Serializable]
    public class kAINodeDebugInfo : kAIDebugInfo
    {
        /// <summary>
        /// The ID of the node. 
        /// </summary>
        public string NodeID
        {
            get;
            private set;
        }

        /// <summary>
        /// The debug info of the contents of the node. 
        /// </summary>
        public kAINodeObjectDebugInfo Contents
        {
            get;
            private set;
        }

        /// <summary>
        /// The debug info for each of the external ports. 
        /// </summary>
        public List<kAIPortDebugInfo> ExternalPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// Create debug info for a specific node. 
        /// </summary>
        /// <param name="lNode">The node to make the debug info for. </param>
        public kAINodeDebugInfo(kAINode lNode)
        {
            NodeID = lNode.NodeID;

            Contents = lNode.NodeContents.GenerateDebugInfo();

            ExternalPorts = new List<kAIPortDebugInfo>();
            foreach (kAIPort lExternalPort in lNode.GetExternalPorts())
            {
                ExternalPorts.Add(lExternalPort.GenerateDebugInfo());
            }
        }
    }
}
