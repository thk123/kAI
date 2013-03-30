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
        /// The port this port is connected to (null if disconnected).
        /// </summary>
        protected kAIPort mConnectedPort;

        /// <summary>
        /// The node this port belong to (if null, is a global node).
        /// </summary>
        public kAINode OwningNode
        {
            get;
            protected set;
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
        /// Is this port currently connected to another port. 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return mConnectedPort != null;
            }
        }

        /// <summary>
        /// Is the port a global port or connected to some kAINode. 
        /// </summary>
        public bool IsGlobalPort
        {
            get
            {
                return OwningNode == null;
            }
        }

        /// <summary>
        /// Gets a construction of the current connexion, containing what this port is attached to etc.
        /// </summary>
        public kAIConnexion Connexion
        {
            get
            {
                if (IsConnected)
                {
                    return new kAIConnexion(this, mConnectedPort);
                }
                else
                {
                    return null;
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
            :base(lLogger)
        {
            mConnectedPort = null;
            PortID = lPortID;
            PortDirection = lPortDirection;
            DataType = lDataType;
        }

        /// <summary>
        /// Attach a port to a node. 
        /// </summary>
        /// <param name="lOwningNode">The node this port belongs to. </param>
        public void Attach(kAINode lOwningNode)
        {
            if (OwningNode != null)
            {
                LogError("Node already connected", PortID);

                //NOTE: Maybe should remove itself from other node, probably not required as this is not really meant to happen

                return;
            }
            OwningNode = lOwningNode;
        }

        /// <summary>
        /// Connect this port to another port. Will report a warning if something goes wrong.
        /// </summary>
        /// <param name="lOtherEnd">The port to attempt to connect to. </param>
        /// <returns>A result indicating how the connexion went. </returns>
        public ePortConnexionResult Connect(kAIPort lOtherEnd)
        {
            ePortConnexionResult lConnexionResult = CanConnect(lOtherEnd);

            if (lConnexionResult == ePortConnexionResult.PortConnexionResult_OK)
            {
                mConnectedPort = lOtherEnd;
                lOtherEnd.mConnectedPort = this;
            }
            else
            {
                LogWarning("Attempted to connect to invalid port.", lConnexionResult);
            }

            return lConnexionResult;
        }

        /// <summary>
        /// Disconnect this port from whatever it is connected to. 
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                mConnectedPort.mConnectedPort = null;
                mConnectedPort = null;
            }
            else
            {
                LogWarning("Port not connected to anything", PortID);
            }
        }

        /// <summary>
        /// Check whether this port would be a valid connexion for the supplied other end. 
        /// </summary>
        /// <param name="lOtherEnd">The port to test against.</param>
        /// <returns>A result indicating how the connexion went.</returns>
        public ePortConnexionResult CanConnect(kAIPort lOtherEnd)
        {
            bool lDirectionCheck = IsDirectionOpposite(lOtherEnd.PortDirection, PortDirection);
            bool lDataCheck = AreDataTypesCompatible(lOtherEnd.DataType, DataType);

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
            return lPortIDA.PortID == lPortIDB.PortID;
        }

        /// <summary>
        /// Checks two PortID's don't match. 
        /// </summary>
        /// <param name="lPortIDA">The first port ID.</param>
        /// <param name="lPortIDB">The second port ID.</param>
        /// <returns>Whether the two ports match.</returns>
        public static bool operator !=(kAIPortID lPortIDA, kAIPortID lPortIDB)
        {
            return !(lPortIDA == lPortIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAIPortID lPortID = obj as kAIPortID;
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
        /// The start node of this connexion (if not global, null if it is).
        /// </summary>
        public kAINode StartNode
        {
            get
            {
                return StartPort.OwningNode;
            }
        }

        /// <summary>
        /// The end node of this connexion (if global, this is null).
        /// </summary>
        public kAINode EndNode
        {
            get
            {
                return EndPort.OwningNode;
            }
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
