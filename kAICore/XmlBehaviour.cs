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
    public class kAIXmlBehaviour : kAIBehaviour
    {
        /// <summary>
        /// This is the object that is used when embedding this behaviour in another XML behaviour to define the node.
        /// </summary>
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            /// <summary>
            /// The ID of the behaviour.
            /// </summary>
            [DataMember()]
            public kAIBehaviourID BehaviourID;

            /// <summary>
            /// The location of the XML file that stores this behaviour.
            /// </summary>
            [DataMember()]
            public FileInfo XmlBehaviourFile;

            /// <summary>
            /// Create the serialisable object that is used to embed this behaviour into a node. 
            /// </summary>
            /// <param name="lXmlBehaviour">The XML Behaviour to base this off. </param>
            public SerialObject(kAIXmlBehaviour lXmlBehaviour)
            {
                BehaviourID = lXmlBehaviour.BehaviourID;
                XmlBehaviourFile = lXmlBehaviour.XmlLocation;


                lXmlBehaviour.Assert(XmlBehaviourFile != null, "Attempted to add a file that hasn't been saved.");
            }

            /// <summary>
            /// Gets the name of this serial object. 
            /// </summary>
            /// <returns>The behaviourID of this XML behaviour. </returns>
            public string GetFriendlyName()
            {
                return BehaviourID;
            }

            /// <summary>
            /// Gets the type of this node serial type.
            /// </summary>
            /// <returns>The type -- BehaviourXML. </returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.BehaviourXml;
            }

            /// <summary>
            /// Create this <see cref="kAIINodeObject"/> this serial object is representing.
            /// </summary>
            /// <param name="lAssemblyResolver">The method to use to resolve assembly names to get types. </param>
            /// <returns>An instantiated kAIXMLBehaviour. </returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return kAIXmlBehaviour.Load(this, lAssemblyResolver);
            }

            /// <summary>
            /// Gets the string representation of this behaviour.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return GetFriendlyName();
            }
        }

        /// <summary>
        /// Represents the XML for the behaviour when saved out. 
        /// </summary>
        [DataContract(Name = "kAIXmlBehaviour")]
        private class kAIXmlBehaviour_InternalXml
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

            public kAIXmlBehaviour_InternalXml()
            {
                InternalNodes = new List<SerialNode>();
            }

            public kAIXmlBehaviour_InternalXml(kAIXmlBehaviour lBehaviour)
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

        /// <summary>
        /// Wraps an internal port with extra information, 
        /// specifically whether is has an associated external port. 
        /// </summary>
        private struct InternalPort
        {
            /// <summary>
            /// The port in question.
            /// </summary>
            public kAIPort Port;

            /// <summary>
            /// Does this port have an external port linked with it. 
            /// </summary>
            public bool IsGloballyAccesible;

        }

        //TODO: move me to the project with a specific method exposed. 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lFullName"></param>
        /// <returns></returns>
        public delegate Assembly GetAssemblyByName(string lFullName);

        /// <summary>
        /// Extension for XML behaviours. 
        /// </summary>
        public const string kAIXmlBehaviourExtension = "xml";

        //Internal port IDs
        private readonly kAIPortID kOnActivatePortID = "OnActivate";
        private readonly kAIPortID kDeactivatePortID = "Deactivate";

        /// <summary>
        /// The location of the XML file.
        /// </summary>
        public FileInfo XmlLocation
        {
            get;
            private set;
        }
            


        /// <summary>
        /// The nodes within the XML behaviour. 
        /// </summary>
        public IEnumerable<kAINode> InternalNodes
        {
            get
            {
                return mInternalNodes.Values;
            }
        }

        /// <summary>
        /// Get the ports within this XML behaviour. 
        /// </summary>
        public IEnumerable<kAIPort> InternalPorts
        {
            get
            {
                foreach(InternalPort lInternalPort in mInternalPorts.Values)
                {
                    yield return lInternalPort.Port;
                }
            }
        }

        /// <summary>
        /// Nodes within this XML behaviour. 
        /// </summary>
        Dictionary<kAINodeID, kAINode> mInternalNodes;

        /// <summary>
        /// Ports within this XML behaviour.
        /// </summary>
        Dictionary<kAIPortID, InternalPort> mInternalPorts;

        /// <summary>
        /// Create a new XML behaviour 
        /// </summary>
        /// <param name="lBehaviourID">The name of the new behaviour. </param>
        /// <param name="lFile">Where this behaviour should be saved. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        public kAIXmlBehaviour(kAIBehaviourID lBehaviourID, FileInfo lFile, kAIILogger lLogger = null)
            : this(lBehaviourID, lLogger)
        {
            XmlLocation = lFile;

            // Create the internal ports 
            kAIPort lOnActivatePort = new kAIPort(kOnActivatePortID, kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType, lLogger);
            AddInternalPort(lOnActivatePort, false);

            kAIPort lDeactivatePort = new kAIPort(kDeactivatePortID, kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType, lLogger);
            lDeactivatePort.OnTriggered += new kAIPort.TriggerEvent(lDeactivatePort_OnTriggered);
            AddInternalPort(lDeactivatePort, false);
        }

        /// <summary>
        /// Create a kAIXMLBehaviour based off a loaded save file for it. 
        /// </summary>
        /// <param name="lSource">The loaded XML file. </param>
        /// <param name="lAssemblyGetter">The method to use to resolve assembly names to get types. </param>
        /// <param name="lSourceFile">The source file this behaviour is loaded from. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        private kAIXmlBehaviour(kAIXmlBehaviour_InternalXml lSource, GetAssemblyByName lAssemblyGetter, FileInfo lSourceFile, kAIILogger lLogger = null)
            : this(lSource.BehaviourID, lLogger)
        {
            XmlLocation = lSourceFile;

            foreach (kAINode lNode in lSource.GetInternalNodes(lAssemblyGetter))
            {
                AddNode(lNode);
            }

            foreach (InternalPort lPort in lSource.GetInternalPorts(lAssemblyGetter))
            {
                AddInternalPort(lPort);
            }
        }

        private kAIXmlBehaviour(kAIBehaviourID lBehaviourID, kAIILogger lLogger = null)
            : base(lBehaviourID, lLogger)
        {
            mInternalNodes = new Dictionary<kAINodeID, kAINode>();
            mInternalPorts = new Dictionary<kAIPortID, InternalPort>();
        }

        /// <summary>
        /// Add a node inside this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        public void AddNode(kAINode lNode)
        {
            mInternalNodes.Add(lNode.NodeID, lNode);
        }

        /// <summary>
        /// Save this XML behaviour in to an XML file. 
        /// </summary>
        public void Save()
        {
            kAIXmlBehaviour_InternalXml lSaveableBehaviour = new kAIXmlBehaviour_InternalXml(this);
            
            XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(kAIXmlBehaviour_InternalXml), kAINode.NodeSerialTypes);

            // Settings for writing the XML file 
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;

            // Create the writer and write the file. 
            XmlWriter lWriter = XmlWriter.Create(XmlLocation.FullName, lSettings);
            lProjectSerialiser.WriteObject(lWriter, lSaveableBehaviour);
            lWriter.Close();
            
        }

        /// <summary>
        /// Update this behaviour, updating an active nodes and processing any events. 
        /// </summary>
        /// <param name="lDeltaTime">The time passed since last update. </param>
        public override void Update(float lDeltaTime)
        {
        }

        /// <summary>
        /// Gets the <see cref="kAIINodeSerialObject"/>of this XML Behaviour. 
        /// </summary>
        /// <returns>The serial object representing this XML behaviour. </returns>
        public override kAIINodeSerialObject GetDataContractClass()
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Get the type of the serial object this <see cref="kAIINodeObject"/> produces. Will inherit from 
        /// <see cref="kAIINodeSerialObject"/>.
        /// </summary>
        /// <returns>The type of the serial object. </returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        /// <summary>
        /// Add internally accessible port. 
        /// </summary>
        /// <param name="lNewPort">The new port to add. </param>
        /// <param name="lExpose">Should this port have a corresponding externally accesible port. </param>
        private void AddInternalPort(kAIPort lNewPort, bool lExpose = false)
        {
            AddInternalPort(new InternalPort { Port = lNewPort, IsGloballyAccesible = lExpose });
        }

        private void AddInternalPort(InternalPort lInternalPort)
        {
            if (mInternalPorts.ContainsKey(lInternalPort.Port.PortID))
            {
                throw new kAIBehaviourPortAlreadyExistsException(this, mInternalPorts[lInternalPort.Port.PortID].Port, lInternalPort.Port);
            }
            else
            {
                mInternalPorts.Add(lInternalPort.Port.PortID, lInternalPort);

                if (lInternalPort.IsGloballyAccesible)
                {
                    // TODO: If InPort, then create an out port and add an event listener for the inport 
                    // to trigger the out port.
                    // else create an inport and listen to its trigger for when to trigger ours
                }

            }
        }

        /// <summary>
        /// Load an XML Behaviour from a file. 
        /// </summary>
        /// <param name="lSerialObject">the serialised version of this XML behaviour.</param>
        /// <param name="lAssemblyGetter">The method to use resolve unknown types. </param>
        /// <returns>An instantiated behaviour based on the provided XML. </returns>
        public static kAIXmlBehaviour Load(kAIINodeSerialObject lSerialObject, GetAssemblyByName lAssemblyGetter)
        {
            SerialObject lRealSerial = lSerialObject as SerialObject;
            if (lRealSerial != null)
            {
                XmlObjectSerializer lProjectDeserialiser = new DataContractSerializer(typeof(kAIXmlBehaviour_InternalXml), kAINode.NodeSerialTypes);

                Stream lXmlStream = lRealSerial.XmlBehaviourFile.OpenRead();

                kAIXmlBehaviour_InternalXml lXmlFile = (kAIXmlBehaviour_InternalXml)lProjectDeserialiser.ReadObject(lXmlStream);

                lXmlStream.Close();

                return new kAIXmlBehaviour(lXmlFile, lAssemblyGetter, lRealSerial.XmlBehaviourFile);
            }
            else
            {
                //TODO: Error!
                return null;
            }
        }

        /// <summary>
        /// Called once this behaviour gets activated. 
        /// </summary>
        protected override void OnActivate()
        {
            // Trigger the activate port. 
            mInternalPorts[kOnActivatePortID].Port.Trigger();
        }

        // The deactivate port was triggered => this behaviour wants to be deactivated. 
        void lDeactivatePort_OnTriggered(kAIPort lSender)
        {
            Deactivate();
        }
    }
}
