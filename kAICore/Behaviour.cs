using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class kAIBehaviour : kAIObject
    {
        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// The unique (in this behaviour) name of this behaviour instance.
        /// </summary>
        public kAINodeID NodeID
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<kAIPort> GlobalPorts
        {
            get
            {
                return mExternalPorts.Values;
            }
        }

        /// <summary>
        /// Construct a new instance of this behaviour. 
        /// </summary>
        /// <param name="lNodeID">The unique (to this behaviours enviorment) node ID for this instance. </param>
        /// <param name="lLogger">Optionally, the logger this instance should use when logging anything. </param>
        public kAIBehaviour(kAINodeID lNodeID, kAIILogger lLogger = null)
            : base(lLogger)
        {
            NodeID = lNodeID;

            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();
        }

        /// <summary>
        /// Add a globally accessible port to this behaviour.
        /// </summary>
        /// <param name="lNewPort">The new port to add. </param>
        /// <exception cref="kAIBehaviourPortAlreadyExistsException">
        /// If a port with the same PortID already exists in this behaviour.
        /// </exception>
        public void AddPort(kAIPort lNewPort)
        {
            if (!mExternalPorts.ContainsKey(lNewPort.PortID))
            {
                mExternalPorts.Add(lNewPort.PortID, lNewPort);
            }
            else
            {
                throw new kAIBehaviourPortAlreadyExistsException(this, lNewPort, mExternalPorts[lNewPort.PortID]);
            }
        }

        /// <summary>
        /// Update this behaviour. Depends on the behaviors implementation as to what happens here. 
        /// </summary>
        /// <param name="lDeltaTime">The time in seconds between the last update and this. </param>
        public abstract void Update(float lDeltaTime);        
    }

    /// <summary>
    /// Exception when try to add a global port to a behaviour that already has a port with the same node ID. 
    /// </summary>
    public class kAIBehaviourPortAlreadyExistsException : Exception
    {
        /// <summary>
        /// The port that was already present with the same PortID. 
        /// </summary>
        public kAIPort mExistingPort
        {
            get;
            private set;
        }

        /// <summary>
        /// The port that was being added but whose name was already taken. 
        /// </summary>
        public kAIPort mNewPort
        {
            get;
            private set;
        }

        /// <summary>
        /// The behaviour who the port was being added to. 
        /// </summary>
        public kAIBehaviour mBehaviour
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct the exception. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour which this port was added to. </param>
        /// <param name="lExistingPort">The existing port that has the same name as the new port. </param>
        /// <param name="lNewPort">The new port to be added. </param>
        public kAIBehaviourPortAlreadyExistsException(kAIBehaviour lBehaviour, kAIPort lExistingPort, kAIPort lNewPort)
        {
            mExistingPort = lExistingPort;
            mNewPort = lNewPort;
            mBehaviour = lBehaviour;
        }

        /// <summary>
        /// Gets the message of the exception. 
        /// </summary>
        /// <returns>A string explaining what has happened. </returns>
        public override string ToString()
        {
            return "Attempted to create a port in behaviour \"" + mBehaviour.NodeID + "\" whose name already existed: \"" + mExistingPort.PortID + "\"";
        } 
    }
}

