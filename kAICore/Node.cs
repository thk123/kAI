using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace kAI.Core
{
    /// <summary>
    /// The serial version of a node object.
    /// </summary>
    /// <note> 
    /// All classes implementing this interface MUST add themselves to
    /// <see cref="kAINode.NodeSerialTypes"/> so they can be saved and 
    /// loaded. 
    /// </note>
    /// <seealso cref="kAIINodeObject"/>
    public interface kAIINodeSerialObject
    {
        /// <summary>
        /// Get the name for this object (its type depends on the type of the node. 
        /// </summary>
        /// <returns></returns>
        string GetFriendlyName();

        /// <summary>
        /// Gets the flavour of this node serial object. 
        /// </summary>
        /// <returns>What type is this serial object. </returns>
        eNodeFlavour GetNodeFlavour();

        /// <summary>
        /// Create the node object this serial type is representing. Can be used to load it 
        /// or embed it into an XML behaviour. 
        /// </summary>
        /// <param name="lAssemblyResolver">The method to use to resolve assembly names to get types. </param>
        /// <returns>An instantiated INodeObject this serial object was representing. </returns>
        kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver);
    }


    /// <summary>
    /// The types a node object can be. 
    /// </summary>
    public enum eNodeFlavour
    {
        /// <summary>
        /// The node is a XML based behaviour object. 
        /// </summary>
        BehaviourXml,

        /// <summary>
        /// The node is a code based behaviour object.
        /// </summary>
        BehaviourCode,

        /// <summary>
        /// The type is unknown. 
        /// </summary>
        UnknownType,
    }

    /// <summary>
    /// Contains methods relating to the enum <see cref="eNodeFlavour"/>.
    /// </summary>
    public static class NodeFlavour
    {
        /// <summary>
        /// Is this a behaviour flavour (either code or XML). 
        /// </summary>
        /// <param name="lFlavour">The flavour to test. </param>
        /// <returns>A boolean indicating if it is a behaviour. </returns>
        public static bool IsBehaviourFlavour(this eNodeFlavour lFlavour)
        {
            return lFlavour == eNodeFlavour.BehaviourCode || lFlavour == eNodeFlavour.BehaviourXml;
        }
    }

    /// <summary>
    /// A node within an XML behaviour. 
    /// </summary>
    public class kAINode : kAIObject
    {
        /// <summary>
        /// For events when the activation state of this node changes. 
        /// </summary>
        /// <param name="lSender">The node whose state has changed. </param>
        /// <param name="lNewState">The new state of the node. Eg true, the node is now active. </param>
        public delegate void ActivationChangedEvent(kAINode lSender, bool lNewState);

        /// <summary>
        /// Get the serial objects of the types that can be embedded within a node. 
        /// </summary>
        public static IEnumerable<Type> NodeSerialTypes
        {
            get
            {
                yield return typeof(kAIXmlBehaviour.SerialObject);
                yield return typeof(kAICodeBehaviour.SerialObject);
            }
        }

        /// <summary>
        /// A dictionary of externally connectible ports. 
        /// </summary>
        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// What object this node contains. 
        /// </summary>
        public kAIINodeObject NodeContents
        {
            get;
            private set;
        }

        /// <summary>
        /// The unique ID(to the containing behaviour) for this node. 
        /// </summary>
        public kAINodeID NodeID
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new node containing an object of type T. 
        /// </summary>
        /// <param name="lNodeID">The ID of the node. </param>
        /// <param name="lContents">The contents of the node. </param>
        /// <param name="lNodeOwner">The behaviour this node resides in. </param>
        public kAINode(kAINodeID lNodeID, kAIINodeObject lContents, kAIXmlBehaviour lNodeOwner)
        {
            NodeID = lNodeID;

            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();

            NodeContents = lContents;
            if(NodeContents != null)
            {
                foreach (kAIPort lPort in lContents.GetExternalPorts())
                {
                    

                    AddGlobalPort(lPort, lNodeOwner);
                }
            }
        }

        /// <summary>
        /// Returns the type of the ports contents. 
        /// </summary>
        /// <returns>Type of the contents. </returns>
        public Type GetNodeContentsType()
        {
            return NodeContents.GetType();
        }

        /// <summary>
        /// Get a list of ports that can be externally accessed by this object. 
        /// </summary>
        /// <returns>A list of ports that can be accessed. </returns>
        public IEnumerable<kAIPort> GetExternalPorts()
        {
            return NodeContents.GetExternalPorts();
        }

        /// <summary>
        /// Get the type of the serialisable object within the node. 
        /// </summary>
        /// <returns>The type of the serialisable object. </returns>
        public Type GetNodeSerialableContentType()
        {
            return NodeContents.GetDataContractType();
        }

        /// <summary>
        /// Get the object to be serialised. 
        /// </summary>
        /// <returns>A DataContract object to serialse when embedding the content of this node. </returns>
        public kAIINodeSerialObject GetNodeSerialisableContent()
        {
            kAIINodeSerialObject lSerialData = NodeContents.GetDataContractClass();

            // Check the type actually matches the reported type
            Assert(NodeContents.GetDataContractType() == lSerialData.GetType());

            // Check the type is in fact serialisable
            Assert(NodeContents.GetDataContractType().GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0);         

            return lSerialData;
        }

        /// <summary>
        /// Add a externally accessible port to the node. 
        /// </summary>
        /// <param name="lNewPort">The global port to add to this node. </param>
        /// <param name="lNodeOwner">The behaviour this node resides in. </param>
        protected virtual void AddGlobalPort(kAIPort lNewPort, kAIXmlBehaviour lNodeOwner)
        {
            // Set the ID of the port so it can be connected. 
            lNewPort.OwningNode = this;
            lNewPort.OwningBehaviour = lNodeOwner;

            mExternalPorts.Add(lNewPort.PortID, lNewPort);
        }
    }
}
