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
        /// Nodes within this XML behaviour. 
        /// </summary>
        Dictionary<kAINodeID, kAINode> mInternalNodes;

        /// <summary>
        /// Ports within this XML behaviour.
        /// </summary>
        Dictionary<kAIPortID, InternalPort> mInternalPorts;


        /// <summary>
        /// The location of the XML file.
        /// </summary>
        public FileInfo XmlLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// Is this behaviour currently releasing ports (which means other ports inside it cannot be triggered). 
        /// </summary>
        public bool InReleasePhase
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
        /// Base constructor, not to be used, just makes dictionaries for internal ports and nodes. 
        /// </summary>
        /// <param name="lBehaviourID">The name of the new behaviour. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        private kAIXmlBehaviour(kAIBehaviourID lBehaviourID, kAIILogger lLogger = null)
            : base(lBehaviourID, lLogger)
        {
            mInternalNodes = new Dictionary<kAINodeID, kAINode>();
            mInternalPorts = new Dictionary<kAIPortID, InternalPort>();
        }

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
        private kAIXmlBehaviour(InternalXml lSource, GetAssemblyByName lAssemblyGetter, FileInfo lSourceFile, kAIILogger lLogger = null)
            : this(lSource.BehaviourID, lLogger)
        {
            XmlLocation = lSourceFile;

            foreach (kAINode lNode in lSource.GetInternalNodes(lAssemblyGetter, this))
            {
                AddNode(lNode);
            }

            foreach (InternalPort lPort in lSource.GetInternalPorts(lAssemblyGetter))
            {
                AddInternalPort(lPort);
            }

            foreach (kAIPort.kAIConnexion lConnexion in lSource.GetInternalConnexions(this))
            {
                AddConnexion(lConnexion.StartPort, lConnexion.EndPort);
            }
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
        /// Remove a node from within this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to remove. </param>
        public void RemoveNode(kAINode lNode)
        {
            mInternalNodes.Remove(lNode.NodeID);
        }

        /// <summary>
        /// Find out if a node with a given ID is already contained within this behaviour. 
        /// </summary>
        /// <param name="lNodeID">The NodeID to check. </param>
        /// <returns>True if the NodeID is already used. </returns>
        public bool ContainsNodeID(kAINodeID lNodeID)
        {
            return mInternalNodes.ContainsKey(lNodeID);
        }

        /// <summary>
        /// Add a internal connexion within this behaviour. 
        /// </summary>
        /// <param name="lStartPort">The starting port of this connexion. </param>
        /// <param name="lEndPort">The ending port of this connexion. </param>
        public void AddConnexion(kAIPort lStartPort, kAIPort lEndPort)
        {
            // Design justification:
            // Need to represent the connexions within the XML behaviour and ports
            // need to be able to quickly inform their connexions when triggered
            // Since the average behaviour is sparse (that is relatively few connexions 
            // per number of nodes (since most nodes won't be connected to most other nodes)
            // This => uses a adjacency lists (as opposed to matrix)
            // Also, since each port needs to be able to tell connected ports quickly
            // It makes sense for each of the lists of adjacencies is stored in the ports
            // Also since connexions go from Out to In, only store at one end
            // (Trade off: expensive for an in port to remove all of its connexions)

            kAIPort.ConnectPorts(lStartPort, lEndPort);
        }

        /// <summary>
        /// Get all the ports that are either connected from or to this port.
        /// For outbound ports, that is just lPort.Connexions. However, for 
        /// in bound ports this must be computed by checking all internal nodes
        /// and ports for things that are connected to it. 
        /// </summary>
        /// <param name="lPort">The port to find what is connected to it. </param>
        /// <returns>A list of ports that are either conected to or from the specified port. </returns>
        public IEnumerable<kAIPort.kAIConnexion> GetConnectedPorts(kAIPort lPort)
        {
            if (lPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                return lPort.Connexions;
            }
            else
            {
                List<kAIPort.kAIConnexion> lConnexions = new List<kAIPort.kAIConnexion>();

                foreach (InternalPort lInternalPortWrapper in mInternalPorts.Values)
                {
                    kAIPort lInternalPort = lInternalPortWrapper.Port;
                    if (lInternalPort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                    {
                        if (lInternalPort.IsConnectedTo(lPort))
                        {
                            lConnexions.Add(new kAIPort.kAIConnexion(lInternalPort, lPort));
                        }
                    }
                }

                foreach (kAINode lInternalNode in mInternalNodes.Values)
                {
                    foreach (kAIPort lInternalNodePort in lInternalNode.GetExternalPorts())
                    {
                        if (lInternalNodePort.PortDirection == kAIPort.ePortDirection.PortDirection_Out)
                        {
                            if (lInternalNodePort.IsConnectedTo(lPort))
                            {
                                lConnexions.Add(new kAIPort.kAIConnexion(lInternalNodePort, lPort));
                            }
                        }
                    }
                }

                return lConnexions;
            }
        }

        /// <summary>
        /// Save this XML behaviour in to an XML file. 
        /// </summary>
        public void Save()
        {
            InternalXml lSaveableBehaviour = new InternalXml(this);
            
            XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(InternalXml), kAINode.NodeSerialTypes);

            // Settings for writing the XML file 
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;

            // Create the writer and write the file. 
            XmlWriter lWriter = XmlWriter.Create(XmlLocation.FullName, lSettings);
            lProjectSerialiser.WriteObject(lWriter, lSaveableBehaviour);
            lWriter.Close();   
        }

        private void ReleasePorts()
        {
            InReleasePhase = true;
            foreach (InternalPort lInternalPort in mInternalPorts.Values)
            {
                kAIPort lPort = lInternalPort.Port;
                lPort.Release();
            }

            foreach (kAINode lNode in mInternalNodes.Values)
            {
                foreach (kAIPort lPort in lNode.GetExternalPorts())
                {
                    lPort.Release();
                }
            }
            InReleasePhase = false;
        }

        /// <summary>
        /// Update this behaviour, updating an active nodes and processing any events. 
        /// </summary>
        /// <param name="lDeltaTime">The time passed since last update. </param>
        protected override void InternalUpdate(float lDeltaTime)
        {
            ReleasePorts();

            foreach (kAINode lNode in mInternalNodes.Values)
            {
                // This calls the update on the node contents (if this is a behaviour, this will only happen if the behaviour 
                // is active). 
                lNode.NodeContents.Update(lDeltaTime);
            }
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
            kAIPort lPortToAdd = lInternalPort.Port;
            lPortToAdd.OwningBehaviour = this;
            if (mInternalPorts.ContainsKey(lInternalPort.Port.PortID))
            {
                throw new kAIBehaviourPortAlreadyExistsException(this, mInternalPorts[lInternalPort.Port.PortID].Port, lPortToAdd);
            }
            else
            {
                mInternalPorts.Add(lInternalPort.Port.PortID, lInternalPort);

                if (lInternalPort.IsGloballyAccesible)
                {
                    kAIPort lExternalPort = new kAIPort(lPortToAdd.PortID, lPortToAdd.PortDirection.OppositeDirection(), lPortToAdd.DataType);

                    // Trigger the internal port if our direction is in
                    if (lExternalPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
                    {
                        lExternalPort.OnTriggered += (lSender) =>
                            {
                                mInternalPorts[lSender.PortID].Port.Trigger();
                            };
                    }
                    else // The external port in an outward port, so should be trigger when the internal port is
                    {
                        lPortToAdd.OnTriggered += (lSender) =>
                            {
                                GetPort(lSender.PortID).Trigger();
                            };
                    }
                }

            }
        }

        private kAIPort GetInternalPort(kAIPortID lPortID, kAINodeID lNodeID)
        {
            // Is the start node ID a real node.
            if (lNodeID == kAINodeID.InvalidNodeID)
            {
                return mInternalPorts[lPortID].Port;
            }
            else // The start node is invalid, so is a global port. 
            {
                kAINode lStartNode = mInternalNodes[lNodeID];

                Assert(lStartNode);

                // TODO: Need a method to just get a port from the dictionary
                return lStartNode.GetExternalPorts().First((lPort) => { return lPort.PortID == lPortID; });
            }
        }

        private bool ContainsPort(kAIPort lPort)
        {
            if (lPort.OwningNode == null)
            {
                return mInternalPorts.ContainsKey(lPort.PortID);
            }
            else
            {
                return mInternalNodes.ContainsKey(lPort.OwningNodeID);
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
            kAIObject.Assert(null, lSerialObject, "Null entity has been passed in to load ");

            SerialObject lRealSerial = lSerialObject as SerialObject;
            kAIObject.Assert(null, lRealSerial, "Tried to load an XML Behaviour from a inappropriate serial object - " + lSerialObject.GetType());

            XmlObjectSerializer lProjectDeserialiser = new DataContractSerializer(typeof(InternalXml), kAINode.NodeSerialTypes);
            try
            {
                Stream lXmlStream = lRealSerial.XmlBehaviourFile.OpenRead();

                InternalXml lXmlFile = (InternalXml)lProjectDeserialiser.ReadObject(lXmlStream);

                Assert(null, lXmlFile);

                lXmlStream.Close();

                return new kAIXmlBehaviour(lXmlFile, lAssemblyGetter, lRealSerial.XmlBehaviourFile);
            }
            catch (System.UnauthorizedAccessException)
            {
                //TODO: Error - have you forgot to check the file out of source control?
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                //TODO: Error - Directory not found, ensure file still exists
            }
            catch(System.IO.IOException)
            {
                // TODOO: Error - File is already open elsewhere. 
            }

            return null;
        }        

        /// <summary>
        /// Called once this behaviour gets activated. 
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            // Trigger the activate port. 
            mInternalPorts[kOnActivatePortID].Port.Trigger();
        }

        /// <summary>
        /// Called once the behaviour gets deactivated
        /// </summary>
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        // The deactivate port was triggered => this behaviour wants to be deactivated. 
        void lDeactivatePort_OnTriggered(kAIPort lSender)
        {
            Deactivate();
        }

        
    }
}
