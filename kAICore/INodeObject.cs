using System;
using System.Collections.Generic;

namespace kAI.Core
{
    /// <summary>
    /// An object that can be put inside a node in an XML Behaviour. 
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
        /// <param name="lOwningBehaviour">
        /// The XML behaviour this node is to be embedded into. 
        /// Can be null if we are serialising for the project.
        /// </param>
        /// <returns>An object that has the DataContractAttribute that will be used to store this arary. </returns>
        kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour);

        /// <summary>
        /// Get the type of the serialisable object within the node type.  
        /// </summary>
        /// <returns>The type of the serial object is. </returns>
        Type GetDataContractType();

        /// <summary>
        /// Gets the example name for this node object (to be modified if a match already exists). 
        /// </summary>
        /// <returns>A template string for the name.</returns>
        string GetNameTemplate();

        /// <summary>
        /// Update the node. 
        /// </summary>
        /// <param name="lDeltaTime">The time passed since last update. </param>
        /// <param name="lUserData">The user data.</param>
        void Update(float lDeltaTime, object lUserData);
    }

    /// <summary>
    /// Base class for an object that can be embedded in to a node. 
    /// </summary>
    public abstract class kAINodeObject : kAIObject, kAIINodeObject
    {
        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// The list of externally connectible ports. 
        /// </summary>
        public IEnumerable<kAIPort> GlobalPorts
        {
            get
            {
                return mExternalPorts.Values;
            }
        }

        /// <summary>
        /// Create a node object. 
        /// </summary>
        /// <param name="lLogger">Optionally, the logger this object should use. </param>
        public kAINodeObject(kAIILogger lLogger = null)
            : base(lLogger)
        {
            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();
        }

        /// <summary>
        /// Add a globally accessible port to this behaviour.
        /// </summary>
        /// <param name="lNewPort">The new port to add. </param>
        /// <exception cref="kAIBehaviourPortAlreadyExistsException">
        /// If a port with the same PortID already exists in this behaviour.
        /// </exception>
        protected void AddExternalPort(kAIPort lNewPort)
        {
            if (!mExternalPorts.ContainsKey(lNewPort.PortID))
            {
                mExternalPorts.Add(lNewPort.PortID, lNewPort);
            }
            else
            {
                //ThrowException(new kAIBehaviourPortAlreadyExistsException(this, lNewPort, mExternalPorts[lNewPort.PortID]));
            }
        }

        /// <summary>
        /// Gets a externally accesible port by name. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <returns>Returns the port if it exists. </returns>
        public kAIPort GetPort(kAIPortID lPortID)
        {
            return mExternalPorts[lPortID];
        }

        /// <summary>
        /// The list of externally connectible ports. 
        /// </summary>
        /// <returns>A list of ports that can be connected to externally. </returns>
        public IEnumerable<kAIPort> GetExternalPorts()
        {
            return GlobalPorts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lOwningBehaviour"></param>
        /// <returns></returns>
        public abstract kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Type GetDataContractType();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string GetNameTemplate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lDeltaTime"></param>
        /// <param name="lUserData"></param>
        public abstract void Update(float lDeltaTime, object lUserData);
    }
}