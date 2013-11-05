using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using kAI.Core;

namespace kAI.Editor.ObjectProperties
{
    /// <summary>
    /// Represents the properties visible for a port.
    /// </summary>
    class kAIPortProperties : kAIIPropertyEntry
    {
        /// <summary>
        /// The ID of the port. 
        /// </summary>
        [DescriptionAttribute("The unique (either on the node or within the behaviour) ID for this port.")]
        public string PortID
        {
            get
            {
                return mPort.PortID;
            }
            // TODO: set
        }

        /// <summary>
        /// The type of the port. 
        /// </summary>
        [DescriptionAttribute("The type of this port (e.g. trigger or some kind of a data port). ")]
        public string PortType
        {
            get
            {
                return mPort.DataType.ToString();
            }
            //TODO: set
        }

        /// <summary>
        /// The direction of the port. 
        /// </summary>
        [DescriptionAttribute("The direction of this port. ")]
        public string PortDirection
        {
            get
            {
                return mPort.PortDirection.ToString();
            }
            // TODO: set
        }

        /// <summary>
        /// The node this port belongs to (if any).
        /// </summary>
        [TypeConverterAttribute(typeof(kAISimpleConverter<kAINode, kAINodeProperties>))]
        [DescriptionAttribute("The node this port belongs to (if any).")]
        public kAISimpleClickableEntry<kAINode, kAINodeProperties> OwningNode
        {
            get
            {
                if (mPort.OwningNode != null)
                {
                    return new kAISimpleClickableEntry<kAINode, kAINodeProperties>(mPort.OwningNode);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The list of connexions either going into or out of this port. 
        /// </summary>
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        [DescriptionAttribute("The list of connexions either going into or out of this port. ")]
        public kAISimplePropertyList<kAIPort.kAIConnexion, kAIConnexionProperties> Connexions
        {
            get
            {
                return new kAISimplePropertyList<kAIPort.kAIConnexion, kAIConnexionProperties>(mPort.OwningBehaviour.GetConnectedPorts(mPort));
            }
        }

        kAIPort mPort;

        /// <summary>
        /// Create a PortProperties basic off a specific port. 
        /// </summary>
        /// <param name="lPort">The port to show the properties for. </param>
        public kAIPortProperties(kAIPort lPort)
        {
            mPort = lPort;
        }
    }
}
