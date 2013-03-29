using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// Represents a connectible element in a behaviour
    /// </summary>
    public class kAINode : kAIObject
    {
        /// <summary>
        /// The ID (unique to its environment) of this node. 
        /// </summary>
        public kAINodeID NodeID
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of ports this node has that things can connect to.
        /// </summary>
        public List<kAIPort> InPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of ports this node has that can be connected from. 
        /// </summary>
        public List<kAIPort> OutPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// Does one or more of the out ports connect to something. 
        /// </summary>
        public bool IsConnectedToSomething
        {
            get
            {
                foreach (kAIPort port in OutPorts)
                {
                    if(port.IsConnected)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Is there one or more in ports that have something connected to it. 
        /// </summary>
        public bool IsConnectedFromSomething
        {
            get
            {
                foreach (kAIPort port in InPorts)
                {
                    if (port.IsConnected)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Is one of the ports (either in or out) connected to something. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return IsConnectedToSomething || IsConnectedFromSomething;
            }
        }

        /// <summary>
        /// Standard constructor for a kAINode
        /// </summary>
        /// <param name="lNodeID">The identifier for this node. </param>
        /// <param name="lInPorts">A list of ports that can be connected to.</param>
        /// <param name="lOutPorts">A list of ports that can be connected from.</param>
        /// <param name="lLogger">Optionally, the logger used for this instance.</param>
        public kAINode(kAINodeID lNodeID, List<kAIPort> lInPorts, List<kAIPort> lOutPorts, kAIILogger lLogger = null)
            : base(lLogger)
        {
            NodeID = lNodeID;

            // Check the supplied in ports, to check actually in, and add them to the in list. 
            InPorts = new List<kAIPort>();
            foreach (kAIPort lPort in lInPorts)
            {
                if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
                {
                    InPorts.Add(lPort);
                }
                else
                {
                    LogError("Attempted to add an out port in the in ports list", lPort);
                }
            }

            // Check the supplied out ports, to check actually out, and add them to the out list. 
            OutPorts = new List<kAIPort>();
            foreach (kAIPort lPort in lOutPorts)
            {
                if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                {
                    OutPorts.Add(lPort);
                }
                else
                {
                    LogError("Attempted to add an in port in the out ports list", lPort);
                }
            }
        }

        /// <summary>
        /// Same as standard constructor, but don't sort the ports first. 
        /// </summary>
        /// <param name="lNodeID">The ID of the node. </param>
        /// <param name="lAllPorts">All the ports this node has. </param>
        /// <param name="lLogger">Optionally, the logger used for this instance.</param>
        public kAINode(kAINodeID lNodeID, List<kAIPort> lAllPorts, kAIILogger lLogger = null)
            : base(lLogger)
        {
            NodeID = lNodeID;

            foreach (kAIPort lPort in lAllPorts)
            {
                if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
                {
                    InPorts.Add(lPort);
                }
                else
                {
                    OutPorts.Add(lPort);
                }
            }
        }

        /// <summary>
        /// Get a specific port by ID.
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <returns>The port requested, null if the port cannot be found. </returns>
        public kAIPort GetPort(kAIPortID lPortID)
        {
            foreach (kAIPort lPort in InPorts)
            {
                if (lPort.PortID == lPortID)
                {
                    return lPort;
                }
            }

            foreach (kAIPort lPort in OutPorts)
            {
                if (lPort.PortID == lPortID)
                {
                    return lPort;
                }
            }

            LogWarning("Could not find port", lPortID);

            return null;
        }

        /// <summary>
        /// Gets a specific port with a specified direction. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lDirection">The direction of the port. </param>
        /// <returns></returns>
        public kAIPort GetPort(kAIPortID lPortID, kAIPort.ePortDirection lDirection)
        {
            switch (lDirection)
            {
                case kAIPort.ePortDirection.PortDirection_In:
                    {
                        foreach (kAIPort lPort in InPorts)
                        {
                            if (lPort.PortID == lPortID)
                            {
                                return lPort;
                            }
                        }
                    }
                    break;
                case kAIPort.ePortDirection.PortDirection_Out:
                    {
                        foreach (kAIPort lPort in OutPorts)
                        {
                            if (lPort.PortID == lPortID)
                            {
                                return lPort;
                            }
                        }
                    }
                    break;
                default:
                    LogError("Did not recognise direction", lDirection);
                    break;
            }

            LogWarning("Could not find port", lPortID);

            return null;
        }

        /// <summary>
        /// Add a new port to this behaviour. 
        /// </summary>
        /// <param name="lNewPort">The new port to add. </param>
        public void AddPort(kAIPort lNewPort)
        {
            if (!DoesPortExist(lNewPort.PortID))
            {
                switch (lNewPort.PortDirection)
                {
                    case kAIPort.ePortDirection.PortDirection_In:
                        InPorts.Add(lNewPort);
                        break;
                    case kAIPort.ePortDirection.PortDirection_Out:
                        OutPorts.Add(lNewPort);
                        break;
                    default:
                        LogError("Did not recognise direction", lNewPort.PortDirection);
                        break;
                }
            }
        }

        /// <summary>
        /// Remove a port from this node. 
        /// </summary>
        /// <param name="lPortID">The ID of the port to remove. </param>
        public void RemovePort(kAIPortID lPortID)
        {
            kAIPort lPort = GetPort(lPortID);
            if(lPort != null)
            {
                lPort.Disconnect();

                switch (lPort.PortDirection)
                {
                    case kAIPort.ePortDirection.PortDirection_In:
                        InPorts.Remove(lPort);
                        break;
                    case kAIPort.ePortDirection.PortDirection_Out:
                        OutPorts.Remove(lPort);
                        break;
                    default:
                        LogError("Did not recognise direction", lPort.PortDirection);
                        break;
                }
            }
            else
            {
                LogError("Couldn't find port.", lPortID);
            }
        }

        /// <summary>
        /// Find out if a given port exists. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <returns>A boolean indicating if the port exists on this node, where true means it does.</returns>
        public bool DoesPortExist(kAIPortID lPortID)
        {
            foreach (kAIPort lPort in InPorts)
            {
                if (lPort.PortID == lPortID)
                {
                    return true;
                }
            }

            foreach (kAIPort lPort in OutPorts)
            {
                if (lPort.PortID == lPortID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Connect a port on this node to another port on another node. 
        /// </summary>
        /// <param name="lStartPortID">The ID of the port to start at. </param>
        /// <param name="lOtherNode">The other node to connect to. </param>
        /// <param name="lOtherPortID">The ID of the port on the other node to connect to. </param>
        /// <returns>A result indicating how the connexion went. </returns>
        public kAIPort.ePortConnexionResult ConnectToPort(kAIPortID lStartPortID, kAINode lOtherNode, kAIPortID lOtherPortID)
        {
            kAIPort lStartPort = GetPort(lStartPortID);
            if(lStartPortID != null)
            {
                kAIPort lEndPort = lOtherNode.GetPort(lOtherPortID);
                if (lEndPort != null)
                {
                    return lStartPort.Connect(lEndPort);
                }
                else
                {
                    return kAIPort.ePortConnexionResult.PortConnexionResult_NoSuchEndPort;
                }
            }
            else
            {
                return kAIPort.ePortConnexionResult.PortConnexionResult_NoSuchStartPort;
            }
        }

        /// <summary>
        /// Connect a port on this node to another port (can be global).
        /// </summary>
        /// <param name="lStartPortID">The ID of the port on this node. </param>
        /// <param name="lEndPort">The other port. </param>
        /// <returns>A result indicating how the connexion went. </returns>
        public kAIPort.ePortConnexionResult ConnectToPort(kAIPortID lStartPortID, kAIPort lEndPort)
        {
            kAIPort lStartPort = GetPort(lStartPortID);
            if (lStartPortID != null)
            {
                return lStartPort.Connect(lEndPort);
            }
            else
            {
                return kAIPort.ePortConnexionResult.PortConnexionResult_NoSuchStartPort;
            }
        }

        /// <summary>
        /// Disconnect all the ports on this node. 
        /// </summary>
        public void Disconnect()
        {
            foreach (kAIPort lPort in InPorts)
            {
                lPort.Disconnect();
            }

            foreach (kAIPort lPort in OutPorts)
            {
                lPort.Disconnect();
            }
        }
    }

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
        public static bool operator ==(kAINodeID lNodeIDA, kAINodeID lNodeIDB)
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
            kAINodeID lPortID = obj as kAINodeID;
            if (lPortID != null)
            {
                return lPortID == this;
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
        /// <returns>The PortID. </returns>
        public override string ToString()
        {
            return NodeID;
        }
    }
}
