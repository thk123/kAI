using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace kAI.Core
{
    /// <summary>
    /// An XML behaviour (ie one created by the editor).
    /// </summary>
    [DataContract(Name = "kAIXmlBehaviour")]
    public class kAIXmlBehaviour : kAIBehaviour
    {
        /// <summary>
        /// The nodes within the XML behaviour. 
        /// </summary>
        [DataMember()]
        public IEnumerable<kAINodeBase> InternalNodes
        {
            get
            {
                return mNodes.Values;
            }
        }

        Dictionary<kAINodeID, kAINodeBase> mNodes;

        /// <summary>
        /// Create a new XML behaviour 
        /// </summary>
        /// <param name="lBehaviourID">The name of the new behaviour. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        public kAIXmlBehaviour(kAIBehaviourID lBehaviourID, kAIILogger lLogger = null)
            : base(lBehaviourID, lLogger)
        {
            mNodes = new Dictionary<kAINodeID, kAINodeBase>();
        }

        /// <summary>
        /// Add a node inside this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        public void AddNode(kAINodeBase lNode)
        {
            lNode.Active = false;
            mNodes.Add(lNode.NodeID, lNode);
        }

        /// <summary>
        /// Update this behaviour, updating an active nodes and processing any events. 
        /// </summary>
        /// <param name="lDeltaTime">The time passed since last update. </param>
        public override void Update(float lDeltaTime)
        {
        }
    }
}
