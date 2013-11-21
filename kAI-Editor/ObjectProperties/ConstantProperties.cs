using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using kAI.Core;

namespace kAI.Editor.ObjectProperties
{
    
    class ConstantProperties : kAIIPropertyEntry
    {
        kAIConstantNode mNode;

        [DescriptionAttribute("The value of this constant node")]
        public string Value
        {
            get
            {
                return mNode.GetValueString();
            }
            set
            {
                if (mNode.SetValueFromString(value))
                {

                }
            }
        }

        public ConstantProperties(kAIConstantNode lNode)
        {
            mNode = lNode;
        }

        /// <summary>
        /// Gets the name of the contents. 
        /// </summary>
        /// <returns>The friendly name of the thing inside the node. </returns>
        public override string ToString()
        {
            return mNode.GetDataContractClass(null).GetFriendlyName();
        }
    }
}
