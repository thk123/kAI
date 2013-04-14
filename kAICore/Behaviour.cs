using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    interface kAIIBehaviour
    {
        //public kAIIBehaviour()
        //public kAIIBehaviour MakeBehaviour();

        // Add a port to this behaviour
        void AddPort(kAIPort lNewPort);

        void AddBehaviour(kAIIBehaviour lNewBehaviour);
        void AddPortToBehaviour(kAIPort lNewPort, kAIIBehaviour lExistingBehaviour);

        void ConnectPort(kAIPort lStartPort, kAIPort lEndPort);
        void DisconnectPort(kAIConnexion lConnexion);
        void Update(TimeSpan lDeltaTime);
    }

    public class kAIBehaviour : kAIObject
    {
        // These are the global ports for the behaviour, are connected to by external ports
        public List<kAIPort> GlobalInPorts;
        public List<kAIPort> GlobalOutPorts;

        List<kAINode> InternalNodes;

        public kAINodeID BehaviourID
        {
            get;
            private set;
        }

        public void AddPort(kAIPort lNewPort)
        {
            if (lNewPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
            {
                GlobalOutPorts.Add(lNewPort);
            }
            else
            {
                GlobalInPorts.Add(lNewPort);
            }
        }

        public void AddBehaviour(kAIBehaviour lNewBehaviour)
        {
            // so we get a new behaviour to add
            // first we make a node from this behaviour
                       
        }

        private kAINode CreateNodeFromBehaviour(kAIBehaviour lNewBehaviour)
        {
            kAINodeID lNewNodeID = CreateNodeID(lNewBehaviour);
            // Extract all external ports from this behaviour 
                       
            return null;
        }

        private kAINodeID CreateNodeID(kAIBehaviour lNewBehaviour)
        {
            return lNewBehaviour.BehaviourID;
        }

        public void AddPortToBehaviour(kAIPort lNewPort, kAIBehaviour lExistingBehaviour)
        {
            throw new NotImplementedException();
        }

        public void ConnectPort(kAIPort lStartPort, kAIPort lEndPort)
        {
            throw new NotImplementedException();
        }

        public void DisconnectPort(kAIConnexion lConnexion)
        {
            throw new NotImplementedException();
        }

        public void Update(TimeSpan lDeltaTime)
        {
            LogMessage("Message:", lDeltaTime);
        }

        
    }

    internal class kAINode : kAIObject
    {
        List<kAIPort> InternalInPorts;
        List<kAIPort> InternalOutPorts;

        public kAINode(kAINodeID lNodeID, kAIILogger lLogger = null)
            :base(lLogger)
        {

        }
    }

    internal class kAIBehaviourNode : kAINode
    {
        public kAIBehaviourNode(kAIBehaviour lInternalBehaviour, kAINodeID lNodeID, kAIILogger lLogger = null)
            :base(lNodeID, lLogger)
        {
            BindExternalPorts(lInternalBehaviour);
        }

        private void BindExternalPorts(kAIBehaviour lInternalBehaviour)
        {
            // Get each of the ports and add a new port of the opposite direction
        }
    }

    /*public class kAIAction 
    {
    }

    public class kAINode
    {

    }

    public class kAIBehaviourNode : kAINode
    {
        kAIBehaviour mContainedBehaviour;

        public kAIBehaviourNode(kAIBehaviour containedBehaviour)
        {
            foreach (kAIPort lport in containedBehaviour.GlobalInPorts)
            {
                // we add 
                //kAIPort lExternalInport = new kAIPort(lport.PortID; 
                kAIPort lExternalInPort = new kAIPort(lport.PortID, kAIPort.ePortDirection.PortDirection_In, lport.DataType);
                kAIPort.ConnectPorts(lport, lExternalInPort);
            }
        }
    }

    public class kAIActionNode : kAINode
    {
        kAIAction mContaintedAction;
    }*/

    /// <summary>
    /// A simple wrapper class for node IDs 
    /// </summary>
    public class kAINodeID
    {
        /// <summary>
        /// The string of the node id
        /// </summary>
        public string NodeID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a NodeID from a string.
        /// </summary>
        /// <param name="lNodeID"></param>
        public kAINodeID(string lNodeID)
        {
            NodeID = lNodeID;
        }

        /// <summary>
        /// Implicitly convert between kAINodeIDs and strings.
        /// </summary>
        /// <param name="lNodeID">The existing node ID.</param>
        /// <returns>The string representing the node ID.</returns>
        public static implicit operator string(kAINodeID lNodeID)
        {
            return lNodeID.NodeID;
        }

        /// <summary>
        /// Implicitly convert between kAINodeIDs and strings.
        /// </summary>
        /// <param name="lNodeID">The string of a node id.</param>
        /// <returns>A kAINodeID from the string. </returns>
        public static implicit operator kAINodeID(string lNodeID)
        {
            return new kAINodeID(lNodeID);
        }

        /// <summary>
        /// Checks two NodeID's match.
        /// </summary>
        /// <param name="lNodeIDA">The first node ID.</param>
        /// <param name="lNodeIDB">The second node ID.</param>
        /// <returns>Whether the two nodes match.</returns>
        public static bool operator== (kAINodeID lNodeIDA, kAINodeID lNodeIDB)
        {
            return lNodeIDA.NodeID == lNodeIDB.NodeID;
        }

        /// <summary>
        /// Checks two NodeID's don't match. 
        /// </summary>
        /// <param name="lNodeIDA">The first node ID.</param>
        /// <param name="lNodeIDB">The second node ID.</param>
        /// <returns>Whether the two nodes match.</returns>
        public static bool operator !=(kAINodeID lNodeIDA, kAINodeID lNodeIDB)
        {
            return !(lNodeIDA == lNodeIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAINodeID lNodeID = obj as kAINodeID;
            if (lNodeID != null)
            {
                return lNodeID == this;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Standard hash code.
        /// </summary>
        /// <returns>The hash of the object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns the NodeID.
        /// </summary>
        /// <returns>The NodeID. </returns>
        public override string ToString()
        {
            return NodeID;
        }
    }
}

