using System;
using System.Collections.Generic;

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
        kAIINodeSerialObject GetDataContractClass();

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
        void Update(float lDeltaTime);
    }
}