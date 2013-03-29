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
        };

        /// <summary>
        /// The ID of the port, used to connect to it and identify it.
        /// </summary>
        public string PortID
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
                return true;
            }
        }

        /// <summary>
        /// Standard constructor for a new global port. 
        /// </summary>
        /// <param name="lPortID">The indetifier for this port, must be unique for this global space. </param>
        /// <param name="lPortDirection">The direction of the port. </param>
        /// <param name="lDataType">The data type of the port.</param>
        public kAIPort(string lPortID, ePortDirection lPortDirection, kAIPortType lDataType)
        {
            mConnectedPort = null;
            PortID = lPortID;
            PortDirection = lPortDirection;
            DataType = lDataType;
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
            }
            else
            {
                LogWarning("Attempted to connect to invalid port.", lConnexionResult);
            }

            return lConnexionResult;
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
}
