using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a trigger port. 
    /// </summary>
    [Serializable]
    public class kAITriggerPortDebugInfo : kAIPortDebugInfo
    {
        /// <summary>
        /// The last time the port was triggered. 
        /// </summary>
        public DateTime LastTimeTriggered
        {
            get;
            private set;
        }

        /// <summary>
        /// Create debug info for a specific trigger port. 
        /// </summary>
        /// <param name="lLastTimeTriggered">The last time the port was triggered. </param>
        /// <param name="lPort">The port to make the debug info for. </param>
        public kAITriggerPortDebugInfo(DateTime lLastTimeTriggered, kAITriggerPort lPort)
            : base(lPort)
        {
            LastTimeTriggered = lLastTimeTriggered;
        }
    }
}
