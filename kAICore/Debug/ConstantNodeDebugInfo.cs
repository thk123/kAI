using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// DebugInfo for a constant node. 
    /// </summary>
    [Serializable]
    public class kAIConstantNodeDebugInfo : kAINodeObjectDebugInfo
    {
        /// <summary>
        /// The value of the constant node. 
        /// </summary>
        public string NodeValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct debug info for a constant node. 
        /// </summary>
        /// <param name="lNode">The constant node to make the debug info for. </param>
        public kAIConstantNodeDebugInfo(kAIConstantNode lNode)
            :base(lNode)
        {
            NodeValue = lNode.GetValueString();
        }
    }
}
