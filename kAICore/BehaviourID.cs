using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace kAI.Core
{
    /// <summary>
    /// A simple wrapper class for behaviour IDs.
    /// Behaviours should have unique names. 
    /// </summary>
    [DataContract()]
    public class kAIBehaviourID
    {
        /// <summary>
        /// The string of the behaviour ID
        /// </summary>
        [DataMember()]
        public string BehaviourID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a BehaviourID from a string.
        /// </summary>
        /// <param name="lBehaviourID"></param>
        public kAIBehaviourID(string lBehaviourID)
        {
            BehaviourID = lBehaviourID;
        }

        /// <summary>
        /// Implicitly convert between kAIBehaviourIDs and strings.
        /// </summary>
        /// <param name="lBehaviourID">The existing behaviour ID.</param>
        /// <returns>The string representing the behaviour ID.</returns>
        public static implicit operator string(kAIBehaviourID lBehaviourID)
        {
            return lBehaviourID.BehaviourID;
        }

        /// <summary>
        /// Implicitly convert between kAIBehaviourIDs and strings.
        /// </summary>
        /// <param name="lBehaviourID">The string of a behaviour id.</param>
        /// <returns>A kAIBehaviourID from the string. </returns>
        public static implicit operator kAIBehaviourID(string lBehaviourID)
        {
            return new kAIBehaviourID(lBehaviourID);
        }

        /// <summary>
        /// Checks two BehaviourID's match.
        /// </summary>
        /// <param name="lBehaviourIDA">The first behaviour ID.</param>
        /// <param name="lBehaviourIDB">The second behaviour ID.</param>
        /// <returns>Whether the two behaviours match.</returns>
        public static bool operator ==(kAIBehaviourID lBehaviourIDA, kAIBehaviourID lBehaviourIDB)
        {
            return lBehaviourIDA.BehaviourID == lBehaviourIDB.BehaviourID;
        }

        /// <summary>
        /// Checks two BehaviourID's don't match. 
        /// </summary>
        /// <param name="lBehaviourIDA">The first behaviour ID.</param>
        /// <param name="lBehaviourIDB">The second behaviour ID.</param>
        /// <returns>Whether the two behaviours match.</returns>
        public static bool operator !=(kAIBehaviourID lBehaviourIDA, kAIBehaviourID lBehaviourIDB)
        {
            return !(lBehaviourIDA == lBehaviourIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAIBehaviourID lBehaviourID = obj as kAIBehaviourID;
            if (lBehaviourID != null)
            {
                return lBehaviourID == this;
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
            return BehaviourID.GetHashCode();
        }

        /// <summary>
        /// Returns the BehaviourID.
        /// </summary>
        /// <returns>The BehaviourID. </returns>
        public override string ToString()
        {
            return BehaviourID;
        }
    }
}
