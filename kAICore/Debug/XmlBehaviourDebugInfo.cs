using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for an XML behaviour.
    /// </summary>
    [Serializable]
    public class kAIXmlBehaviourDebugInfo : kAIBehaviourDebugInfo
    {
        /// <summary>
        /// Debug info for each internal port of the behaviour. 
        /// </summary>
        public List<kAIPortDebugInfo> InternalPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// Debug info for each of the internal nodes.
        /// </summary>
        public List<kAINodeDebugInfo> InternalNodes
        {
            get;
            private set;
        }

        /// <summary>
        /// Creae debug info for this xml behaviour. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour to make the debug info for. </param>
        public kAIXmlBehaviourDebugInfo(kAIXmlBehaviour lBehaviour)
            : base(lBehaviour)
        {
            InternalPorts = new List<kAIPortDebugInfo>();
            foreach (kAIPort lInternalPort in lBehaviour.InternalPorts)
            {
                InternalPorts.Add(lInternalPort.GenerateDebugInfo());
            }

            InternalNodes = new List<kAINodeDebugInfo>();
            foreach (kAINode lNode in lBehaviour.InternalNodes)
            {
                InternalNodes.Add(lNode.GenerateDebugInfo());
            }
        }
    }
}
