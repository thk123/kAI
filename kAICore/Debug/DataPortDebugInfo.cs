using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core.Debug
{
    /// <summary>
    /// Debug info for a data port. 
    /// </summary>
    [Serializable]
    public class kAIDataPortDebugInfo : kAIPortDebugInfo
    {
        /// <summary>
        /// The current value stored at this port. 
        /// Will be [NULL] if the value is null. 
        /// </summary>
        public string CurrentData
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct debug info for the data port. 
        /// </summary>
        /// <param name="lPort">The data port. to make the debug info for. </param>
        public kAIDataPortDebugInfo(kAIDataPort lPort)
            :base(lPort)
        {
            object lData = lPort.GetData();
            if (lData == null)
            {
                CurrentData = "[NULL]";
            }
            else
            {
                IEnumerable enumeratignData = lData as IEnumerable;
                if (enumeratignData != null)
                {
                    StringBuilder listData = new StringBuilder();
                    foreach (object o in enumeratignData)
                    {
                        listData.Append(o.ToString() + ", ");
                    }
                    CurrentData = listData.ToString();
                }
                else
                {
                    CurrentData = lPort.GetData().ToString();
                }
            }
        }
    }
}
