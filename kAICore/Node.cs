using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace kAI.Core
{
    /// <summary>
    /// Base (non-generic) class for a node within an XML behaviour
    /// </summary>
    [DataContract()]
    public abstract class kAINodeBase : kAIObject
    {
        private static List<Type> kNodeTypes = new List<Type>();
        public static IEnumerable<Type> NodeTypes
        {
            get
            {
                return kNodeTypes;
            }
        }

        internal static void AddNodeSerialType(Type lSerialType)
        {
            kNodeTypes.Add(lSerialType);
        }

        /// <summary>
        /// For events when the activation state of this node changes. 
        /// </summary>
        /// <param name="lSender">The node whose state has changed. </param>
        /// <param name="lNewState">The new state of the node. Eg true, the node is now active. </param>
        public delegate void ActivationChangedEvent(kAINodeBase lSender, bool lNewState);

        /// <summary>
        /// Triggered when the activation state of the node changes. 
        /// </summary>
        public event ActivationChangedEvent OnActivationStateChanged;

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
        /// Is the node currently active. 
        /// </summary>
        bool mActive;

        /// <summary>
        /// A dictionary of externally connectible ports. 
        /// </summary>
        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// Create a new node with the given ID. 
        /// </summary>
        /// <param name="lNodeID">The unique (in the containing behaviour) ID of the node. </param>
        public kAINodeBase(kAINodeID lNodeID)
        {
            NodeID = lNodeID;
            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();
        }

        /// <summary>
        /// Gets the type of the contents of this node. 
        /// </summary>
        /// <returns>Returns a type of the containing node (will inherit from kAIINodeObject).</returns>
        public abstract Type GetNodeContentsType();

        public abstract Type GetNodeSerialableContentType();

        public abstract object GetNodeSerialisableContent();

        /// <summary>
        /// Get a list of externally accessible ports from this node. 
        /// </summary>
        /// <returns>The list of ports. </returns>
        public abstract IEnumerable<kAIPort> GetExternalPorts();


        /// <summary>
        /// Add a externally accessible port to the node. 
        /// </summary>
        /// <param name="lNewPort"></param>
        protected virtual void AddGlobalPort(kAIPort lNewPort)
        {
            mExternalPorts.Add(lNewPort.PortID, lNewPort);
        }

    }

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

    /// <summary>
    /// A node within an XML behaviour. 
    /// </summary>
    /// <typeparam name="T">The type of node (eg kAIBehaviour)</typeparam>
    [DataContract()]
    public class kAINode<T> : kAINodeBase where  T : kAIINodeObject
    {
        /// <summary>
        /// What object this node contains. 
        /// </summary>
        [DataMember()]
        public T NodeContents
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new node containing an object of type T. 
        /// </summary>
        /// <param name="lNodeID">The ID of the node. </param>
        /// <param name="lContents">The contents of the node. </param>
        public kAINode(kAINodeID lNodeID, T lContents)
            :base(lNodeID)
        {
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
        public override Type GetNodeContentsType()
        {
            return NodeContents.GetType();
        }

        /// <summary>
        /// Get a list of ports that can be externally accessed by this object. 
        /// </summary>
        /// <returns>A list of ports that can be accessed. </returns>
        public override IEnumerable<kAIPort> GetExternalPorts()
        {
            return NodeContents.GetExternalPorts();
        }

        /// <summary>
        /// Get the type of the serialisable object within the node. 
        /// </summary>
        /// <returns>The type of the serialisable object. </returns>
        public override Type GetNodeSerialableContentType()
        {
            return NodeContents.GetDataContractType();
        }

        /// <summary>
        /// Get the object to be serialised. 
        /// </summary>
        /// <returns>A DataContract object to serialse when embedding the content of this node. </returns>
        public override object GetNodeSerialisableContent()
        {
            // Check the type is in fact serialisable
            Assert(NodeContents.GetDataContractType().GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0);

            return NodeContents.GetDataContractClass();
        }
    }
}
