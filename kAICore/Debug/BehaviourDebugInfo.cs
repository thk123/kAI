using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a behaviour. 
    /// </summary>
    [Serializable]
    public class kAIBehaviourDebugInfo : kAINodeObjectDebugInfo
    {
        /// <summary>
        /// The ID of the behaviour.
        /// </summary>
        public string BehaviourID
        {
            get;
            private set;
        }

        /// <summary>
        /// Is the behaviour active. 
        /// </summary>
        public bool Active
        {
            get;
            private set;
        }


        /// <summary>
        /// Construct the debug info from an instance of a behaviour. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour to display debug info for. </param>
        public kAIBehaviourDebugInfo(kAIBehaviour lBehaviour)
            :base(lBehaviour)
        {
            BehaviourID = lBehaviour.BehaviourID;

            Active = lBehaviour.Active;
        }
    }

}
