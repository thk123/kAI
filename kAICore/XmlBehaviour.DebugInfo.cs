using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    
    public partial class kAIXmlBehaviour
    {
        /// <summary>
        /// DEPRECATED: Represents an XML Behaviours information for presentation and debugging. 
        /// </summary>
        [Serializable]
        public class kAIDebugInfo
        {
            /// <summary>
            /// The set of active XML Behaviours corresponding debug info. 
            /// </summary>
            public List<kAIDebugInfo> ActiveBehaviours
            {
                get;
                private set;
            }

            /// <summary>
            /// The list of all active behaviours. 
            /// </summary>
            public List<string> ActiveNodes
            {
                get;
                private set;
            }

            string xmlBehaviourID;

            /// <summary>
            /// Create a debug info based off an XML Behaviour.
            /// </summary>
            /// <param name="lBehaviour"></param>
            internal kAIDebugInfo(kAIXmlBehaviour lBehaviour)
            {
                //mBehaviour = lBehaviour;
                ActiveNodes = new List<string>();
                ActiveBehaviours = new List<kAIDebugInfo>();

                xmlBehaviourID = lBehaviour.BehaviourID;
            }

            /// <summary>
            /// Update the debug info to have latest information. 
            /// </summary>
            internal void Update()
            {
                ActiveNodes.Clear();
                ActiveBehaviours.Clear();

                /*foreach (kAINode lNode in mBehaviour.InternalNodes)
                {
                    if (lNode.NodeContents is kAIBehaviour)
                    {
                        if (((kAIBehaviour)lNode.NodeContents).Active)
                        {
                            ActiveNodes.Add(lNode.NodeID);

                            if (lNode.NodeContents is kAIXmlBehaviour)
                            {
                                ActiveBehaviours.Add(((kAIXmlBehaviour)lNode.NodeContents).DebugInfo);
                            }
                        }   
                    }
                }*/
            }
        }
    }
}
