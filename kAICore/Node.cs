using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace kAI.Core
{
    /// <summary>
    /// AN object that can be put inside a node in an XML Behaviour. 
    /// </summary>
    public interface kAIINodeObject
    {
        /// <summary>
        /// Get a list of ports that can be externally accessed by this object. 
        /// </summary>
        /// <returns>A list of ports that can be accessed. </returns>
        IEnumerable<kAIPort> GetExternalPorts();

        /// <summary>
        /// Data that can be used to serialise and deserialise this object
        /// </summary>
        /// <returns>An object that has the DataContractAttribute that will be used to store this arary. </returns>
        object GetDataContractClass();

        /// <summary>
        /// Get the type of the serialisable object within the node type. 
        /// </summary>
        /// <returns>The type of the serial object is. </returns>
        Type GetDataContractType();
    }

    public interface kAIINodeSerialObject
    {

    }

    /// <summary>
    /// A node within an XML behaviour. 
    /// </summary>
    [DataContract()]
    public class kAINode : kAIObject
    {
        /// <summary>
        /// For events when the activation state of this node changes. 
        /// </summary>
        /// <param name="lSender">The node whose state has changed. </param>
        /// <param name="lNewState">The new state of the node. Eg true, the node is now active. </param>
        public delegate void ActivationChangedEvent(kAINode lSender, bool lNewState);


        /// <summary>
        /// Is the node currently active. 
        /// </summary>
        bool mActive;

        /// <summary>
        /// A dictionary of externally connectible ports. 
        /// </summary>
        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// What object this node contains. 
        /// </summary>
        [DataMember()]
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
        /// Is this node currently active in the containing behaviour. 
        /// </summary>
        public bool Active
        {
            get
            {
                return mActive;
            }
            set
            {
                if (mActive != value)
                {
                    mActive = value;

                    OnActivationStateChanged(this, mActive);
                }
            }
        }


        /// <summary>
        /// Triggered when the activation state of the node changes. 
        /// </summary>
        public event ActivationChangedEvent OnActivationStateChanged;

        /// <summary>
        /// Create a new node containing an object of type T. 
        /// </summary>
        /// <param name="lNodeID">The ID of the node. </param>
        /// <param name="lContents">The contents of the node. </param>
        public kAINode(kAINodeID lNodeID, kAIINodeObject lContents)
        {
            NodeID = lNodeID;

            mActive = false;
            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();

            NodeContents = lContents;
            if(NodeContents != null)
            {
                foreach (kAIPort lPort in lContents.GetExternalPorts())
                {
                    AddGlobalPort(lPort);
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
        public object GetNodeSerialisableContent()
        {
            // Check the type is in fact serialisable
            Assert(NodeContents.GetDataContractType().GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0);

            return NodeContents.GetDataContractClass();
        }

        /// <summary>
        /// Add a externally accessible port to the node. 
        /// </summary>
        /// <param name="lNewPort"></param>
        protected virtual void AddGlobalPort(kAIPort lNewPort)
        {
            mExternalPorts.Add(lNewPort.PortID, lNewPort);
        }
    }
}
