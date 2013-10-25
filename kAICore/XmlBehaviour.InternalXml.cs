using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;

namespace kAI.Core
{
    /// <summary>
    /// An XML behaviour (ie one created by the editor).
    /// </summary>
    public partial class kAIXmlBehaviour : kAIBehaviour
    {
        /// <summary>
        /// Represents the XML for the behaviour when saved out. 
        /// </summary>
        [DataContract(Name = "kAIXmlBehaviour")]
        private class InternalXml
        {
            /// <summary>
            /// Represents a node inside the behaviour when written out. 
            /// </summary>
            [DataContract()]
            struct SerialNode
            {
                [DataMember()]
                public kAINodeID NodeID;

                [DataMember()]
                public string NodeType;

                [DataMember()]
                public string NodeTypeAssembly;

                [DataMember()]
                public string NodeSerialType;

                [DataMember()]
                public string NodeSerialAssembly;

                [DataMember()]
                public object NodeContents;

                public SerialNode(kAINode lNode)
                {
                    NodeID = lNode.NodeID;
                    NodeType = lNode.GetNodeContentsType().FullName;
                    NodeTypeAssembly = lNode.GetNodeContentsType().Assembly.FullName;

                    NodeSerialType = lNode.GetNodeSerialableContentType().FullName;
                    NodeSerialAssembly = lNode.GetNodeSerialableContentType().Assembly.FullName;
                    NodeContents = lNode.GetNodeSerialisableContent();
                }
            }

            [DataContract()]
            struct SerialPort
            {
                [DataMember()]
                public string PortID;

                [DataMember()]
                public kAIPort.ePortDirection PortDirection;

                [DataMember()]
                public string PortDataType;

                [DataMember()]
                public string PortDataTypeAssembly;

                [DataMember()]
                public bool IsGloballyAccesible;

                public SerialPort(InternalPort lPort)
                {
                    PortID = lPort.Port.PortID;
                    PortDirection = lPort.Port.PortDirection;
                    PortDataType = lPort.Port.DataType.DataType.FullName;
                    PortDataTypeAssembly = lPort.Port.DataType.DataType.Assembly.FullName;
                    IsGloballyAccesible = lPort.IsGloballyAccesible;
                }
            }

            [DataContract()]
            struct SerialConnexion
            {
                [DataMember()]
                public kAIPortID StartPortID;

                [DataMember()]
                public kAINodeID StartNodeID;

                [DataMember()]
                public kAIPortID EndPortID;

                [DataMember()]
                public kAINodeID EndNodeID;

                public SerialConnexion(kAIPort.kAIConnexion lConnexion)
                {
                    StartPortID = lConnexion.StartPort.PortID;
                    StartNodeID = lConnexion.StartPort.OwningNodeID;

                    EndPortID = lConnexion.EndPort.PortID;
                    EndNodeID = lConnexion.EndPort.OwningNodeID;
                }
            }

            [DataMember()]
            public string BehaviourID
            {
                get;
                set;
            }

            [DataMember()]
            private List<SerialNode> InternalNodes
            {
                get;
                set;
            }

            [DataMember()]
            private List<SerialPort> InternalPorts
            {
                get;
                set;
            }

            [DataMember()]
            private List<SerialConnexion> InternalConnexions
            {
                get;
                set;
            }

            public InternalXml()
            {
                InternalNodes = new List<SerialNode>();
            }

            public InternalXml(kAIXmlBehaviour lBehaviour)
            {
                // the behaviour to save
                BehaviourID = lBehaviour.BehaviourID;

                InternalNodes = new List<SerialNode>();

                InternalConnexions = new List<SerialConnexion>();

                foreach (kAINode lNodeBase in lBehaviour.InternalNodes)
                {
                    Type lContentType = lNodeBase.GetNodeSerialableContentType();
                    object lContent = lNodeBase.GetNodeSerialisableContent();

                    lBehaviour.Assert(lContentType == lContent.GetType(), "The content returned from the node does not match the reported type...");

                    InternalNodes.Add(new SerialNode(lNodeBase));

                    foreach (kAIPort lPort in lNodeBase.GetExternalPorts())
                    {
                        if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                        {
                            // TODO: some how we need, for each of the adjacent ports, get their node owners
                            // probably at the point where we make the connexion, we need to store this information

                            foreach (kAIPort.kAIConnexion lConnexion in lPort.Connexions)
                            {
                                InternalConnexions.Add(new SerialConnexion(lConnexion));
                            }
                        }
                    }
                }

                InternalPorts = new List<SerialPort>();

                foreach (InternalPort lPort in lBehaviour.mInternalPorts.Values)
                {
                    InternalPorts.Add(new SerialPort(lPort));

                    if (lPort.Port.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                    {
                        foreach (kAIPort.kAIConnexion lConnexion in lPort.Port.Connexions)
                        {
                            InternalConnexions.Add(new SerialConnexion(lConnexion));
                        }
                    }
                }
            }

            /// <summary>
            /// Get each of the internal nodes saved within this serial object. 
            /// </summary>
            /// <param name="lAssemblyGetter">The method to use to get assemblies to resolve types. </param>
            /// <param name="lOwningBehaviour">The XML behaviour these nodes reside in. </param>
            /// <returns>A list of nodes this serial object represents. </returns>
            public IEnumerable<kAINode> GetInternalNodes(GetAssemblyByName lAssemblyGetter, kAIXmlBehaviour lOwningBehaviour)
            {
                foreach (SerialNode lInternalNode in InternalNodes)
                {

                    // Get the type of the thing we want to construct
                    Assembly lNodeTypeAssembly = lAssemblyGetter(lInternalNode.NodeTypeAssembly);
                    Type lNodeType = lNodeTypeAssembly.GetType(lInternalNode.NodeType);

                    // Get the type of the data we have about it. 
                    Assembly lNodeSerialTypeAssembly = lAssemblyGetter(lInternalNode.NodeSerialAssembly);
                    Type lNodeSerialType = lNodeSerialTypeAssembly.GetType(lInternalNode.NodeSerialType);

                    object lData = lInternalNode.NodeContents;

                    MethodInfo lLoader = lNodeType.GetMethod("Load", BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);

                    object lNodeContents = lLoader.Invoke(null, new object[] { lData, lAssemblyGetter });

                    kAINode lNewNode = new kAINode(lInternalNode.NodeID, (kAIINodeObject)lNodeContents, lOwningBehaviour);

                    yield return lNewNode;
                }
            }

            /// <summary>
            /// Get each of the internal ports saved within this serial object. 
            /// </summary>
            /// <param name="lAssemblyGetter">The method to use to get assemblies to resolve types. </param>
            /// <returns>A list of ports this serial object represents. </returns>
            public IEnumerable<InternalPort> GetInternalPorts(GetAssemblyByName lAssemblyGetter)
            {
                foreach (SerialPort lInternalPort in InternalPorts)
                {
                    Assembly lPortTypeAssembly = lAssemblyGetter(lInternalPort.PortDataTypeAssembly);
                    Type lPortType = lPortTypeAssembly.GetType(lInternalPort.PortDataType);
                    kAIPort lPort = new kAIPort(lInternalPort.PortID, lInternalPort.PortDirection, lPortType);

                    InternalPort lWrappedPort = new InternalPort { Port = lPort, IsGloballyAccesible = lInternalPort.IsGloballyAccesible };

                    yield return lWrappedPort;
                }
            }

            /// <summary>
            /// Gets each of the links contained within this behaviour (ie from/to ports that are either internal ports of this behaviour or
            /// external ports of nodes contained within it). 
            /// </summary>
            /// <param name="lBehaviour">The partially constructed behaviour that has the ports we wish to connect. </param>
            /// <returns>All the connexions. </returns>
            public IEnumerable<kAIPort.kAIConnexion> GetInternalConnexions(kAIXmlBehaviour lBehaviour)
            {
                foreach (SerialConnexion lInternalConnexion in InternalConnexions)
                {
                    // First get the start port
                    kAIPort lStartPort = lBehaviour.GetInternalPort(lInternalConnexion.StartPortID, lInternalConnexion.StartNodeID);

                    // Now get the end port
                    kAIPort lEndPort = lBehaviour.GetInternalPort(lInternalConnexion.EndPortID, lInternalConnexion.EndNodeID);

                    yield return new kAIPort.kAIConnexion(lStartPort, lEndPort);
                }
            }
        }
    }

}


