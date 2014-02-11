using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a port.
    /// </summary>
    [Serializable]
    public abstract class kAIPortDebugInfo : kAIDebugInfo
    {
        /// <summary>
        /// The ID of the port. 
        /// </summary>
        public string PortID
        {
            get;
            private set;
        }

        /// <summary>
        /// Make debug info for a specific port. 
        /// </summary>
        /// <param name="lPort">The port to make the debug info for. </param>
        public kAIPortDebugInfo(kAIPort lPort)
        {
            PortID = lPort.FQPortID;
        }
    }
}
