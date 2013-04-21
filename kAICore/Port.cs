using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// Represents a connectible port, can be from a behaviour, action, input, trigger etc.
    /// </summary>
    public class kAIPort : kAIObject
    {
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
        /// For when this node gets triggered by something connecting to it (or an external event).
        /// </summary>
        /// <param name="lSender">The port that just got triggered. </param>
        public delegate void TriggerEvent(kAIPort lSender);
        //TODO: Maybe provide the origin of the trigger?

        /// <summary>
        /// Occurs when this node gets triggered. 
        /// </summary>
        public event TriggerEvent OnTriggered;

        //TODO: Data drive ports
        /*public delegate void DataEvent(kAIPort lSender, Type lObjectData, object lData);
        public event DataEvent OnDataSet;
        public event DataEvent OnDataChanged;
        public event DataEvent OnDataUnset;*/

       
 


        /// <summary>
        /// The set of ports this port connects to (not is connected from).
        /// </summary>
        protected List<kAIPort> mConnectingPorts;

        /// <summary>
        /// Who knows at this stage, too early to tell. The idea was the "other side" to this port. 
        /// </summary>
        protected kAIPort mBoundPort;


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
            private set;
        }

        /// <summary>
        /// The direction of this port. 
        /// </summary>
        public ePortDirection PortDirection
        {
            get;
            private set;
        }

        /// <summary>
        /// The data type of this port.
        /// </summary>
        public kAIPortType DataType
        {
            get;
            private set;
        }

        /// <summary>
        /// Is this port currently connected to 1 or more ports. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
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
                List<kAIConnexion> lConnexions = new List<kAIConnexion>();
                foreach (kAIPort lPort in mConnectingPorts)
                {
                    if (PortDirection == ePortDirection.PortDirection_Out)
                    {
                        lConnexions.Add(new kAIConnexion(this, lPort));
                    }
                    else
                    {
                        lConnexions.Add(new kAIConnexion(lPort, this));
                    }
                }

                return lConnexions;
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
            :base(lLogger)
        {
            mConnectingPorts = new List<kAIPort>();
            PortID = lPortID;
            PortDirection = lPortDirection;
            DataType = lDataType;
        }

        /// <summary>
        /// Tell this port it just got triggered. 
        /// </summary>
        public void Trigger()
        {
            if (DataType == kAIPortType.TriggerType)
            {
                if (PortDirection == ePortDirection.PortDirection_Out)
                {
                    foreach (kAIPort lConnectedPorts in mConnectingPorts)
                    {
                        lConnectedPorts.Trigger();
                    }
                }
                else
                {
                    OnTriggered(this);
                }
            }
        }

        /// <summary>
        /// Create a connexion between this port and another port. 
        /// </summary>
        /// <param name="lOtherEnd">The port to connect to. </param>
        /// <returns>The result of doing the connexion. </returns>
        internal ePortConnexionResult MakeConnexion(kAIPort lOtherEnd)
        {
            return kAIPort.ConnectPorts(this, lOtherEnd);
        }

        /// <summary>
        /// Break a connexion between this port and another port connected to it. 
        /// </summary>
        /// <param name="lOtherEnd">The other port. </param>
        internal void BreakConnexion(kAIPort lOtherEnd)
        {
            kAIPort.DisconnectPorts(this, lOtherEnd);
        }

        /// <summary>
        /// Break all the connexions between this port and all connected ports. 
        /// </summary>
        internal void BreakAllConnexions()
        {
            foreach (kAIPort lPort in mConnectingPorts)
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
            mConnectingPorts.Add(lOtherEnd);
            OnConnect(lOtherEnd);
            return ePortConnexionResult.PortConnexionResult_OK;
        }

        /// <summary>
        /// Called when this port gets connected to another port.
        /// </summary>
        /// <param name="lOtherEnd">The port this port has been connected to. </param>
        protected virtual void OnConnect(kAIPort lOtherEnd) 
        {
            OnConnected(this, lOtherEnd);
        }

        /// <summary>
        /// Disconnect this port from the specified port. 
        /// </summary>
        /// <param name="lOtherEnd">The other port. </param>
        private void Disconnect(kAIPort lOtherEnd)
        {
            mConnectingPorts.Remove(lOtherEnd);
            OnDisconnect(lOtherEnd);
        }

        /// <summary>
        /// Called when this port is disconnected.
        /// </summary>
        protected virtual void OnDisconnect(kAIPort lOtherEnd) 
        {
            OnDisconnected(this, lOtherEnd);
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
                lResult = lPortA.Connect(lPortB);
                if (lResult == ePortConnexionResult.PortConnexionResult_OK)
                {
                    lResult = lPortB.Connect(lPortA);


                    if (lResult != ePortConnexionResult.PortConnexionResult_OK)
                    {
                        //Something went wrong when connecting, so we disconnect the first port.
                        lPortA.Disconnect(lPortB);
                    }
                }
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
            foreach (kAIPort lOtherEnd in lPortA.mConnectingPorts)
            {
                if (lOtherEnd == lPortB)
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
            if (ArePortsConnected(lPortA, lPortB))
            {
                lPortA.Disconnect(lPortB);
                lPortB.Disconnect(lPortA);
            }
            else
            {
                lPortA.LogError("Ports not connected", lPortA, lPortB);
            }
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
        /// Checks whether two port data types can be connected. 
        /// </summary>
        /// <param name="lDataA">The first data type.</param>
        /// <param name="lDataB">The second data type. </param>
        /// <returns>A boolean indicating if the data types are compatible, true indicating they are. </returns>
        private static bool AreDataTypesCompatible(kAIPortType lDataA, kAIPortType lDataB)
        {
            return lDataA.DataType == lDataA.DataType;
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
        /// The type of a trigger. 
        /// </summary>
        public static readonly kAIPortType TriggerType = typeof(Boolean);

        /// <summary>
        /// Standard constructor for a System.Type
        /// </summary>
        /// <param name="lType">The type to use. </param>
        public kAIPortType(Type lType)
        {
            DataType = lType;
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
    }

    /// <summary>
    /// A simple wrapper class for port IDs 
    /// </summary>
    public class kAIPortID
    {
        /// <summary>
        /// The string of the port id
        /// </summary>
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
        public static bool operator== (kAIPortID lPortIDA, kAIPortID lPortIDB)
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

    /// <summary>
    /// Represents a connexion between two ports. 
    /// </summary>
    public class kAIConnexion
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
        /// <param name="lStartPort">The start port of the connexion. </param>
        /// <param name="lEndPort">The end point of the connexion. </param>
        public kAIConnexion(kAIPort lStartPort, kAIPort lEndPort)
        {
            StartPort = lStartPort;
            EndPort = lEndPort;
        }
    }
}
