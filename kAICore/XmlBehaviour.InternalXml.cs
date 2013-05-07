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

            public InternalXml()
            {
                InternalNodes = new List<SerialNode>();
            }

            public InternalXml(kAIXmlBehaviour lBehaviour)
            {
                // the behaviour to save
                BehaviourID = lBehaviour.BehaviourID;

                InternalNodes = new List<SerialNode>();

                foreach (kAINode lNodeBase in lBehaviour.InternalNodes)
                {
                    Type lContentType = lNodeBase.GetNodeSerialableContentType();
                    object lContent = lNodeBase.GetNodeSerialisableContent();

                    lBehaviour.Assert(lContentType == lContent.GetType(), "The content returned from the node does not match the reported type...");

                    InternalNodes.Add(new SerialNode(lNodeBase));
                }

                InternalPorts = new List<SerialPort>();

                foreach (InternalPort lPort in lBehaviour.mInternalPorts.Values)
                {
                    InternalPorts.Add(new SerialPort(lPort));
                }
            }

            /// <summary>
            /// Get each of the internal nodes saved within this serial object. 
            /// </summary>
            /// <param name="lAssemblyGetter">The method to use to get assemblies to resolve types. </param>
            /// <returns>A list of nodes this serial object represents. </returns>
            public IEnumerable<kAINode> GetInternalNodes(GetAssemblyByName lAssemblyGetter)
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

                    kAINode lNewNode = new kAINode(lInternalNode.NodeID, (kAIINodeObject)lNodeContents);

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
        }
    }

}


