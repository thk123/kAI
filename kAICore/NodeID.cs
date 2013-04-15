using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
	/// <summary>
    /// A simple wrapper class for node IDs.
    /// Every object within an kAIXmlBehaviour has a unique node  ID. 
    /// </summary>
    public class kAINodeID
    {
        /// <summary>
        /// The string of the node ID
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