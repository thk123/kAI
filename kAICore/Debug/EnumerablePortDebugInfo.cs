using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a enumerable data port. 
    /// </summary>
    [Serializable]
    public class kAIEnumerablePortDebugInfo : kAIPortDebugInfo
    {
        /// <summary>
        /// The set of values currently at this port. 
        /// </summary>
        public List<string> CurrentValues
        {
            get;
            private set;
        }

        /// <summary>
        /// Create debug info for the enumerable data port. 
        /// </summary>
        /// <param name="lPort">The port to make the debug info for. </param>
        /// <param name="lValues">The string representations of the values currently stored at this port.</param>
        public kAIEnumerablePortDebugInfo(kAIEnumerableDataPort lPort, IEnumerable<string> lValues)
            : base(lPort)
        {
            CurrentValues = new List<string>();
            CurrentValues.AddRange(lValues);
        }
    }
}
