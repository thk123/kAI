using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
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
            return "Attempted to create a port in behaviour \"" + mBehaviour.BehaviourID + "\" whose name already existed: \"" + mExistingPort.PortID + "\"";
        }
    }
}
