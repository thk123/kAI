using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace kAI.Core
{
    /// <summary>
    /// Represents a connectible port, can be from a behaviour, action, input, trigger etc.
    /// </summary>
    public abstract class kAIPort : kAIObject
    {
        /// <summary>
        /// Represents a connexion between two ports. 
        /// </summary>
        public class kAIConnexion : kAIObject
        {
            /// <summary>
            /// The start port of this connexion. 
            /// </summary>
            public kAIPort StartPort
            {
                get;
                private set;
            }

            /// <summary>
            /// The end port of this connexion. 
            /// </summary>
            public kAIPort EndPort
            {
                get;
                private set;
            }

            /// <summary>
            /// Create a standard connexion between two ports. 
            /// </summary>
            /// <param name="lPortA">The start port of the connexion. </param>
            /// <param name="lPortB">The end point of the connexion. </param>
            /// <param name="lLogger">Optionally, the logger the component should use. </param>
            public kAIConnexion(kAIPort lPortA, kAIPort lPortB, kAIILogger lLogger = null)
                :base(lLogger)
            {
                kAIPort lStartPort, lEndPort;
                OrderPorts(lPortA, lPortB, out lStartPort, out lEndPort);

                StartPort = lStartPort;
                EndPort = lEndPort;
            }

            /// <summary>
            /// Gets a string representation of a connexion. 
            /// </summary>
            /// <returns>Format: StartPortUI->EndPortUI</returns>
            public override string ToString()
            {
                return StartPort.ToString() + "->" + EndPort.ToString();
            }
        }

        /// <summary>
        /// For when connexions are made or destroyed. 
        /// </summary>
        /// <param name="lSender">The node that has been connected to or disconnected from. </param>
        /// <param name="lOtherEnd">The other end of the connexion.</param>
        public delegate void ConnexionEvent(kAIPort lSender, kAIPort lOtherEnd);

        /// <summary>
        /// Occurs when something conects to this node. 
        /// </summary>
        public event ConnexionEvent OnConnected;

        /// <summary>
        /// Occurs when something gets disconnected from this node. 
        /// </summary>
        public event ConnexionEvent OnDisconnected;

        /// <summary>
        /// The set of ports this port connects to (not is connected from).
        /// </summary>
        protected Dictionary<kAIPortID, kAIPort> mConnectingPorts
        {
            get;
            private set;
        }

        /// <summary>
        /// The node that this port belongs to (null if global). 
        /// </summary>
        protected kAINode mOwningNode
        {
            get;
            private set;
        }

        /// <summary>
        /// The XML behaviour this port is embedded in. 
        /// Can be null if it is an external node to a behaviour and that behaviour is at the root
        /// </summary>
        protected kAIXmlBehaviour mOwningBehaviour
        {
            get;
            private set;
        }

        /// <summary>
        /// Get and sets the owning node of this port (maybe hide and just have the ID).
        /// You can only assign the owning node once and this must be done before connecting
        /// the port to anything else. 
        /// </summary>
        public kAINode OwningNode
        {
            get
            {
                return mOwningNode;
            }
            internal set
            {
                Assert(OwningNode == null, "Cannot change the assigned parent node of a port post facto");
                Assert(mConnectingPorts.Count == 0, "This port is already connected to things, so cannot change the parent node of the port");

                mOwningNode = value;
            }
        }

        /// <summary>
        /// Has the owning behaviour of this node been set. 
        /// </summary>
        protected bool mOwningBehaviourSet
        {
            get;
            private set;
        }

        /// <summary>
        /// Get and sets the owning behaviour
        /// You can only assign the owning node once and this must be done before triggering
        /// or releasing the port.
        /// </summary>
        public kAIXmlBehaviour OwningBehaviour
        {
            get
            {
                return mOwningBehaviour;
            }
            internal set
            {
                Assert(!mOwningBehaviourSet, "Cannot change the assigned owning behaviour");

                mOwningBehaviour = value;
                mOwningBehaviourSet = true;
            }
        }

        /// <summary>
        /// Is this port a global port or is it belonging to a node. 
        /// </summary>
        protected bool IsGlobal
        {
            get
            {
                return mOwningNode != null;
            }
        }

        /// <summary>
        /// Gets the ID of the owning node or <see cref="kAINodeID.InvalidNodeID"/> if the node is global.
        /// </summary>
        public kAINodeID OwningNodeID
        {
            get
            {
                if (mOwningNode == null)
                {
                    return kAINodeID.InvalidNodeID;
                }
                else
                {
                    return mOwningNode.NodeID;
                }
            }
        }


        /// <summary>
        /// An enum representing the direction of a port.
        /// </summary>
        public enum ePortDirection
        {
            /// <summary>
            /// This port is an 'In' port, ie stuff is connected to it. 
            /// </summary>
            PortDirection_In,

            /// <summary>
            /// This port is an 'Out' port, ie stuff connects from it. 
            /// </summary>
            PortDirection_Out,
        };

        /// <summary>
        /// Represents a result of a connexion between two <see cref="kAIPort"/>s.
        /// </summary>
        public enum ePortConnexionResult
        {
            /// <summary>
            /// The connexion is valid
            /// </summary>
            PortConnexionResult_OK,

            /// <summary>
            /// The directions are incompatible, eg both 'In' or both 'Out'.
            /// </summary>
            /// <seealso cref="ePortDirection"/>
            PortConnexionResult_InvalidDirection,

            /// <summary>
            /// The ports are of a different data type so can't be connected. 
            /// </summary>
            /// <seealso cref="kAIPortType"/>
            PortConnexionResult_InvalidDataType,

            /// <summary>
            /// The connexion failed because couldn't find a start point with the specified ID.
            /// </summary>
            PortConnexionResult_NoSuchStartPort,

            /// <summary>
            /// The connexion failed because couldn't find a end point with the specified ID.
            /// </summary>
            PortConnexionResult_NoSuchEndPort,
        };

        /// <summary>
        /// The ID of the port, used to connect to it and identify it.
        /// </summary>
        public kAIPortID PortID
        {
            get;
            internal set;
        }

        /// <summary>
        /// The direction of this port. 
        /// </summary>
        public ePortDirection PortDirection
        {
            get;
            internal set;
        }

        /// <summary>
        /// The data type of this port.
        /// </summary>
        public kAIPortType DataType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Is this port currently connected to 1 or more ports. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                Assert(PortDirection == ePortDirection.PortDirection_Out);
                return mConnectingPorts.Count > 0;
            }
        }

        /// <summary>
        /// Gets a list of all of the connexions leaving this port. 
        /// </summary>
        public IEnumerable<kAIConnexion> Connexions
        {
            get
            {
                Assert(PortDirection == ePortDirection.PortDirection_Out);;
                foreach (kAIPort lPort in mConnectingPorts.Values)
                {
                    yield return new kAIConnexion(this, lPort);
                }
            }
        }

        /// <summary>
        /// Standard constructor for a new global port. 
        /// </summary>
        /// <param name="lPortID">The indetifier for this port, must be unique for this global space. </param>
        /// <param name="lPortDirection">The direction of the port. </param>
        /// <param name="lDataType">The data type of the port.</param>
        /// <param name="lLogger">Optionaly, the logger this instance should use. </param>
        public kAIPort(kAIPortID lPortID, ePortDirection lPortDirection, kAIPortType lDataType, kAIILogger lLogger = null)
            : base(lLogger)
        {
            mConnectingPorts = new Dictionary<kAIPortID, kAIPort>();
            PortID = lPortID;
            PortDirection = lPortDirection;
            DataType = lDataType;

            OwningNode = null;

            mOwningBehaviourSet = false;
        }        
        
        /// <summary>
        /// Release this port (first stage of the drill down update). 
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// Create a port going the opposite direction but of the same type as this port.
        /// This is used when creating ports that are externally accessible ports on XML behaviours. 
        /// </summary>
        /// <returns>The opposite port. </returns>
        internal abstract kAIPort CreateOppositePort();

        /// <summary>
        /// Determine if this port is connected to another specified port. 
        /// This port must be an outbound port. 
        /// </summary>
        /// <param name="lPort">The port to check against. </param>
        /// <returns>True if there is a connexion from this port to the other port. </returns>
        public bool IsConnectedTo(kAIPort lPort)
        {
            Assert(PortDirection == ePortDirection.PortDirection_Out, "In ports are not connected to things, things connect to them.");
            return mConnectingPorts.ContainsKey(lPort.PortID);
        }

        /// <summary>
        /// Break a connexion between this port and another port connected to it. 
        /// </summary>
        /// <param name="lOtherEnd">The other port. </param>
        public void BreakConnexion(kAIPort lOtherEnd)
        {
            kAIPort.DisconnectPorts(this, lOtherEnd);
        }

        /// <summary>
        /// Break all the connexions between this port and all connected ports. 
        /// </summary>
        public void BreakAllConnexions()
        {
            Assert(PortDirection == ePortDirection.PortDirection_Out, "In ports are not connected to things, things connect to them.");
            foreach (kAIPort lPort in mConnectingPorts.Values)
            {
                BreakConnexion(lPort);
            }
        }

        /// <summary>
        /// Connect this port to another port. Will report a warning if something goes wrong.
        /// </summary>
        /// <param name="lOtherEnd">The port to attempt to connect to. </param>
        /// <returns>A result indicating how the connexion went. </returns>
        private ePortConnexionResult Connect(kAIPort lOtherEnd)
        {
            if (PortDirection == ePortDirection.PortDirection_Out)
            {
                mConnectingPorts.Add(lOtherEnd.PortID, lOtherEnd);
            }

            OnConnect(lOtherEnd);
            return ePortConnexionResult.PortConnexionResult_OK;
        }

        /// <summary>
        /// Called when this port gets connected to another port.
        /// </summary>
        /// <param name="lOtherEnd">The port this port has been connected to. </param>
        protected virtual void OnConnect(kAIPort lOtherEnd)
        {
            if (OnConnected != null)
            {
                OnConnected(this, lOtherEnd);
            }
        }

        /// <summary>
        /// Disconnect this port from the specified port. 
        /// </summary>
        /// <param name="lOtherEnd">The other port. </param>
        private void Disconnect(kAIPort lOtherEnd)
        {
            // If we are the outbound port, we are storing what we are connected to, so we remove it
            if(PortDirection == ePortDirection.PortDirection_Out)
            {
                mConnectingPorts.Remove(lOtherEnd.PortID);
                
            }

            // Either way, we dirgger the event. 
            OnDisconnect(lOtherEnd);
        }

        /// <summary>
        /// Called when this port is disconnected.
        /// </summary>
        protected virtual void OnDisconnect(kAIPort lOtherEnd)
        {
            if (OnDisconnected != null)
            {
                OnDisconnected(this, lOtherEnd);
            }
        }

        /// <summary>
        /// Returns the URP of this port in the format:
        /// ":PortID" if the port is global
        /// "NodeID:PortID" if the port belongs to some node. 
        /// </summary>
        /// <returns>The URP of this port. </returns>
        public override string ToString()
        {
            string lInsideBehaviour;
            if (mOwningBehaviour == null)
            {
                lInsideBehaviour = "GLOBAL";
            }
            else
            {
                lInsideBehaviour = mOwningBehaviour.BehaviourID;
            }
            if (OwningNode == null)
            {
                return ":" + PortID + " [" + lInsideBehaviour + "]";
            }
            else
            {
                return OwningNode.NodeID + ":" + PortID + " [" + lInsideBehaviour + "]";
            }
        }

        /// <summary>
        /// Check the the state is not in a release of another port (if we are, we cannot trigger additional ports). 
        /// </summary>
        /// <returns>True if the state is valid for triggering a port. </returns>
        private bool CheckState()
        {
            if (mOwningBehaviour == null)
            {
                // This is only true when we are an external port of a behaviour that is at the root#
                // i.e. not embedded in some other behaviour. This means there is no release phase 
                 return true;
            }
            else
            {
                return !mOwningBehaviour.InReleasePhase;
            }
        }


        /// <summary>
        /// Check whether this port would be a valid connexion for the supplied other end. 
        /// </summary>
        /// <param name="lPortA">The first port.</param>
        /// <param name="lPortB">The second port. </param>
        /// <returns>A result indicating how the connexion went.</returns>
        internal static ePortConnexionResult CanConnect(kAIPort lPortA, kAIPort lPortB)
        {
            bool lDirectionCheck = IsDirectionOpposite(lPortA.PortDirection, lPortB.PortDirection);
            bool lDataCheck = AreDataTypesCompatible(lPortA.DataType, lPortB.DataType);

            if (lDirectionCheck && lDataCheck)
            {
                return ePortConnexionResult.PortConnexionResult_OK;
            }
            else if (!lDirectionCheck)
            {
                return ePortConnexionResult.PortConnexionResult_InvalidDirection;
            }
            else // (!lDataCheck)
            {
                return ePortConnexionResult.PortConnexionResult_InvalidDataType;
            }
        }

        /// <summary>
        /// Connect two ports. 
        /// </summary>
        /// <param name="lPortA">The first port. </param>
        /// <param name="lPortB">The second port. </param>
        /// <returns>The result of attempting this connexion. </returns>
        internal static ePortConnexionResult ConnectPorts(kAIPort lPortA, kAIPort lPortB)
        {

            ePortConnexionResult lResult = CanConnect(lPortA, lPortB);
            if (lResult == ePortConnexionResult.PortConnexionResult_OK)
            {
                // We can connect so the ports are opposite directions
                // So we now connect from the outward bound port to the inward bound port
                kAIPort lStartPort, lEndPort;
                OrderPorts(lPortA, lPortB, out lStartPort, out lEndPort);

                return lStartPort.Connect(lEndPort);
            }

            return lResult;
        }

        /// <summary>
        /// Checks whether two ports are connected.
        /// </summary>
        /// <param name="lPortA">The first port.</param>
        /// <param name="lPortB">The second port. </param>
        /// <returns>A boolean indicating if the two elements are connected. </returns>
        internal static bool ArePortsConnected(kAIPort lPortA, kAIPort lPortB)
        {
            kAIPort lEndPort; 
            kAIPort lStarPort;
            OrderPorts(lPortA, lPortB, out lStarPort, out lEndPort);

            foreach (kAIPort lOtherEnd in lStarPort.mConnectingPorts.Values)
            {
                if (lEndPort.PortID == lOtherEnd.PortID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Disconnect two ports. 
        /// </summary>
        /// <param name="lPortA">The first port. </param>
        /// <param name="lPortB">The second port. </param>
        internal static void DisconnectPorts(kAIPort lPortA, kAIPort lPortB)
        {
            Assert(null, ArePortsConnected(lPortA, lPortB), "Attempted to disconnect two ports that aren't disconnected. ");
            lPortA.Disconnect(lPortB);
            lPortB.Disconnect(lPortA);
        }

        /// <summary>
        /// Disconnect two ports. 
        /// </summary>
        /// <param name="lConnexion">The connexion to break. </param>
        internal static void DisconnectPorts(kAIConnexion lConnexion)
        {
            Assert(null, ArePortsConnected(lConnexion.StartPort, lConnexion.EndPort), "Attempted to disconnect two ports that aren't disconnected. ");
            lConnexion.StartPort.Disconnect(lConnexion.EndPort);
            lConnexion.EndPort.Disconnect(lConnexion.StartPort);
        }

        /// <summary>
        /// Checks to see if two directions are opposite. 
        /// </summary>
        /// <param name="lDirectionA">The first direction.</param>
        /// <param name="lDirectionB">The second direction. </param>
        /// <returns>A boolean indicating if the directions are opposite, where true is they are opposite. </returns>
        private static bool IsDirectionOpposite(ePortDirection lDirectionA, ePortDirection lDirectionB)
        {
            return (lDirectionA == kAIPort.ePortDirection.PortDirection_In && lDirectionB == kAIPort.ePortDirection.PortDirection_Out) ||
                    (lDirectionA == kAIPort.ePortDirection.PortDirection_Out && lDirectionB == kAIPort.ePortDirection.PortDirection_In);
        }

        /// <summary>
        /// Order two arbitary points in to the start port and the end port. 
        /// </summary>
        /// <param name="lPortA">One of the ports. </param>
        /// <param name="lPortB">The other port. </param>
        /// <param name="lStartPort">The out direction port. </param>
        /// <param name="lEndPort">The in direction port. </param>
        private static void OrderPorts(kAIPort lPortA, kAIPort lPortB, out kAIPort lStartPort, out kAIPort lEndPort)
        {
            Assert(null, IsDirectionOpposite(lPortA.PortDirection, lPortB.PortDirection), "Both ports are the same direction so can't order them. ");
            lStartPort = lPortA.PortDirection == ePortDirection.PortDirection_Out ? lPortA : lPortB;
            lEndPort = lPortA.PortDirection == ePortDirection.PortDirection_In ? lPortA : lPortB;
        }

        /// <summary>
        /// Checks whether two port data types can be connected. 
        /// </summary>
        /// <param name="lDataA">The first data type.</param>
        /// <param name="lDataB">The second data type. </param>
        /// <returns>A boolean indicating if the data types are compatible, true indicating they are. </returns>
        private static bool AreDataTypesCompatible(kAIPortType lDataA, kAIPortType lDataB)
        {
            return lDataA == lDataB;
        }
    }

    /// <summary>
    /// A class indicating the type of a given port. (Currently just a wrapper for System.Type)
    /// </summary>
    public class kAIPortType
    {
        /// <summary>
        /// The internal type of this port type. 
        /// </summary>
        public Type DataType
        {
            get;
            set;
        }

        /// <summary>
        /// We use this to differentiate between trigger ports and ports that are type boolean. 
        /// </summary>
        bool mIsTrigger;

        /// <summary>
        /// The type of a trigger. 
        /// </summary>
        public static readonly kAIPortType TriggerType = new kAIPortType(typeof(Boolean), true);

        /// <summary>
        /// Standard constructor for a System.Type
        /// </summary>
        /// <param name="lType">The type to use. </param>
        /// <param name="lIsTrigger">Is the port a trigger.</param>
        public kAIPortType(Type lType, bool lIsTrigger = false)
        {
            DataType = lType;
            mIsTrigger = lIsTrigger;
        }


        /// <summary>
        /// Converts a kAIPortType to a System.Type implicitly.
        /// </summary>
        /// <param name="lPortType">The port type to convert. </param>
        /// <returns>A System.Type version of this port type. </returns>
        public static implicit operator Type(kAIPortType lPortType)
        {
            return lPortType.DataType;
        }

        /// <summary>
        /// Converts a System.Type to a kAIPortType implicitly. 
        /// </summary>
        /// <param name="lType">The type to convert. </param>
        /// <returns>A kaiPortType from this Type.</returns>
        public static implicit operator kAIPortType(Type lType)
        {
            return new kAIPortType(lType);
        }

        /// <summary>
        /// Checks equality of two port types. 
        /// </summary>
        /// <param name="lPortTypeA">The first port type. </param>
        /// <param name="lPortTypeB">The second port type. </param>
        /// <returns>True if theu are equal. </returns>
        public static bool operator ==(kAIPortType lPortTypeA, kAIPortType lPortTypeB)
        {
            return lPortTypeA.Equals(lPortTypeB);
        }

        /// <summary>
        /// Checks inequality of two port types. 
        /// </summary>
        /// <param name="lPortTypeA">The first port type. </param>
        /// <param name="lPortTypeB">The second port type. </param>
        /// <returns>True if theu are inequal. </returns>
        public static bool operator !=(kAIPortType lPortTypeA, kAIPortType lPortTypeB)
        {
            return !lPortTypeA.Equals(lPortTypeB);
        }
    

        /// <summary>
        /// Check whether two data types are equal. 
        /// </summary>
        /// <param name="obj">The other obejct. </param>
        /// <returns>True if the data types are equivalent. </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (((object)obj == null))
                return false;

            kAIPortType lDataType = obj as kAIPortType;
            if (((object)lDataType != null))
            {
                return lDataType.DataType == DataType && lDataType.mIsTrigger == mIsTrigger;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generate a hash code based on the type. 
        /// </summary>
        /// <returns>The hash code the type would return. </returns>
        public override int GetHashCode()
        {
            return (DataType.GetHashCode() / 2) + (mIsTrigger.GetHashCode() / 2);
        }

        /// <summary>
        /// Gets a string representation of this type. 
        /// </summary>
        /// <returns>Trigger if a trigger port, the data type otherwise. </returns>
        public override string ToString()
        {
            if (mIsTrigger)
            {
                return "Trigger";
            }
            else
            {
                return "Data: " + DataType.Name;
            }
        }
    }

    /// <summary>
    /// A simple wrapper class for port IDs 
    /// </summary>
    [DataContract()]
    public class kAIPortID
    {
        /// <summary>
        /// The string of the port id
        /// </summary>
        [DataMember()]
        public string PortID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a PortID from a string.
        /// </summary>
        /// <param name="lPortID"></param>
        public kAIPortID(string lPortID)
        {
            PortID = lPortID;
        }

        /// <summary>
        /// Implicitly convert between kAIPortIDs and strings.
        /// </summary>
        /// <param name="lPortID">The existing port ID.</param>
        /// <returns>The string representing the port ID.</returns>
        public static implicit operator string(kAIPortID lPortID)
        {
            return lPortID.PortID;
        }

        /// <summary>
        /// Implicitly convert between kAIPortIDs and strings.
        /// </summary>
        /// <param name="lPortID">The string of a port id.</param>
        /// <returns>A kAIPortID from the string. </returns>
        public static implicit operator kAIPortID(string lPortID)
        {
            return new kAIPortID(lPortID);
        }

        /// <summary>
        /// Checks two PortID's match.
        /// </summary>
        /// <param name="lPortIDA">The first port ID.</param>
        /// <param name="lPortIDB">The second port ID.</param>
        /// <returns>Whether the two ports match.</returns>
        public static bool operator ==(kAIPortID lPortIDA, kAIPortID lPortIDB)
        {
            return lPortIDA.Equals(lPortIDB);
        }

        /// <summary>
        /// Checks two PortID's don't match. 
        /// </summary>
        /// <param name="lPortIDA">The first port ID.</param>
        /// <param name="lPortIDB">The second port ID.</param>
        /// <returns>Whether the two ports match.</returns>
        public static bool operator !=(kAIPortID lPortIDA, kAIPortID lPortIDB)
        {
            return !lPortIDA.Equals(lPortIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (((object)obj == null))
                return false;

            kAIPortID lPortID = obj as kAIPortID;
            if (((object)lPortID != null))
            {
                return lPortID.PortID == PortID;
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
            return PortID.GetHashCode();
        }

        /// <summary>
        /// Returns the PortID.
        /// </summary>
        /// <returns>The PortID. </returns>
        public override string ToString()
        {
            return PortID;
        }
    }
}
