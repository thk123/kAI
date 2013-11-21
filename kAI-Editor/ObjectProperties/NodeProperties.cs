using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace kAI.Editor.ObjectProperties
{
    /// <summary>
    /// Represents the properties visible for a node.
    /// </summary>
    class kAINodeProperties : kAIIPropertyEntry
    {
        /// <summary>
        /// Represents the contents of the node. 
        /// </summary>
        public class kAINodeContentsProperties : kAIIPropertyEntry
        {
            // The object being represented. 
            kAIINodeObject mNodeObject;

            /// <summary>
            /// The type of the contents. 
            /// </summary>
            public string ContentsType
            {
                get
                {
                    return mNodeObject.GetType().Name;
                }
            }

            /// <summary>
            /// The name of the contents. 
            /// </summary>
            [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
            [DescriptionAttribute("The contents of this node. ")]
            public object Contents
            {
                get 
                {
                    if (sNodeContentsProperties.ContainsKey(mNodeObject.GetType()))
                    {
                        return sNodeContentsProperties[mNodeObject.GetType()](mNodeObject);
                    }
                    else
                    {
                        return mNodeObject.GetDataContractClass(null).GetFriendlyName();
                    }
                }
            }

            /// <summary>
            /// Create something representing a specific content of a node. 
            /// </summary>
            /// <param name="lContents">The thing inside the node. </param>
            public kAINodeContentsProperties(kAIINodeObject lContents)
            {
                mNodeObject = lContents;
            }

            /// <summary>
            /// Gets the name of the contents. 
            /// </summary>
            /// <returns>The friendly name of the thing inside the node. </returns>
            public override string ToString()
            {
                return mNodeObject.GetDataContractClass(null).GetFriendlyName();
            }
        }

        // The node being represented by this property set. 
        kAINode mNode;

        /// <summary>
        /// The name of the node. 
        /// </summary>
        [DescriptionAttribute("The unique (within this behaviour) ID for this node.")]
        public string NodeID
        {
            get 
            {
                return mNode.NodeID;
            }
            set
            {
                mNode.NodeID = value;
            }
        }

        /// <summary>
        /// The contents of the node. 
        /// </summary>
        [TypeConverterAttribute(typeof(kAISimpleExpandableConverter<kAINodeContentsProperties>))]
        [DescriptionAttribute("The contents of this node. ")]
        public kAINodeContentsProperties NodeContents
        {
            get
            {
                return new kAINodeContentsProperties(mNode.NodeContents);
            }
        }

        /// <summary>
        /// The set of ports belonging to this node. 
        /// </summary>
        [TypeConverterAttribute(typeof(kAISimpleExpandableConverter<kAISimplePropertyList<kAIPort, kAIPortProperties>>))]
        [DescriptionAttribute("The set of ports belonging to this node.")]
        public kAISimplePropertyList<kAIPort, kAIPortProperties> Ports
        {
            get
            {
                return new kAISimplePropertyList<kAIPort, kAIPortProperties>(mNode.GetExternalPorts());
            }
        }


        public kAINodeProperties(kAINode lNode)
        {
            mNode = lNode;
        }

        static kAINodeProperties()
        {
            sNodeContentsProperties = new Dictionary<Type, Func<kAIINodeObject, kAIIPropertyEntry>>();
            sNodeContentsProperties.Add(typeof(kAIConstantFloatNode), (lEntry) => { return new ConstantProperties((kAIConstantNode)lEntry); });
            sNodeContentsProperties.Add(typeof(kAIConstantIntNode), (lEntry) => { return new ConstantProperties((kAIConstantNode)lEntry); });
            sNodeContentsProperties.Add(typeof(kAIConstantStringNode), (lEntry) => { return new ConstantProperties((kAIConstantNode)lEntry); });
        }

        static Dictionary<Type, Func<kAIINodeObject, kAIIPropertyEntry>> sNodeContentsProperties;
    }
}
