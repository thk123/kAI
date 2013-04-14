using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    public class kAIFSMBehaviour : kAIBehaviour
    {
        struct kAINodeState
        {
            enum eFSMNodeState
            {
                eFSMNodeState_Active,
                eFSMNodeState_Inactive,
            }

            public kAINode Node
            {
                get;
                set;
            }

            public eFSMNodeState NodeState
            {
                get;
                set;
            }
        }

        List<kAINodeState> mNodeStates;

        ///public kAIFSMBehaviour(kAIBehaviourID lBehaviourID, 
    }
}
