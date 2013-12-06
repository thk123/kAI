using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

using kAI.Core;
using kAI.Editor.Controls.DX;
using kAI.Editor.Controls.DX.Coordinates;

namespace kAI.Editor.Core
{
    /// <summary>
    /// Represents the information about an xml file such as the visual positions of the nodes. 
    /// </summary>
    [DataContract(Name="XmlBehaviourMetaSave")]
    class kAIXmlBehaviourMetaSaveFile
    {
        /// <summary>
        /// Represents a nodes position. 
        /// </summary>
        [DataContract]
        class kAINodePoint
        {
            /// <summary>
            /// The ID of the node being considered. 
            /// </summary>
            [DataMember()]
            public kAINodeID Node;

            /// <summary>
            /// The position of the node in absolute space. 
            /// </summary>
            [DataMember()]
            public Point Position;
        }

        /// <summary>
        /// The node IDs and their respective positions. 
        /// </summary>
        [DataMember(Name="NodePositions")]
        List<kAINodePoint> mNodePoints;

        /// <summary>
        /// Create a meta save file based off the position of nodes. 
        /// </summary>
        /// <param name="lData">The position of each node with corresponding ID. </param>
        public kAIXmlBehaviourMetaSaveFile(IEnumerable<Tuple<kAINodeID, kAIAbsolutePosition>> lData)
        {
            mNodePoints = new List<kAINodePoint>();

            foreach (Tuple<kAINodeID, kAIAbsolutePosition> lEntry in lData)
            {
                mNodePoints.Add(new kAINodePoint { Node = lEntry.Item1, Position = lEntry.Item2.mPoint });
            }
        }

        /// <summary>
        /// Gets the absolute positions of each of the nodes with corresponding ID. 
        /// </summary>
        /// <returns>The position of the node with corresponding ID. </returns>
        public IEnumerable<Tuple<kAINodeID, kAIAbsolutePosition>> GetPositions()
        {
            foreach (kAINodePoint lNodePoint in mNodePoints)
            {
                yield return new Tuple<kAINodeID, kAIAbsolutePosition>(lNodePoint.Node, new kAIAbsolutePosition(lNodePoint.Position.X, lNodePoint.Position.Y, false));
            }
        }
    }
}
