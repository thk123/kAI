﻿using System;
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
    using DefaultPortEntry = KeyValuePair<kAIPortID, Func<kAIXmlBehaviour, kAIPort>>;

    /// <summary>
    /// An XML behaviour (ie one created by the editor).
    /// </summary>
    [Serializable]
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
            public kAIRelativePath XmlBehaviourFile;

            /// <summary>
            /// Create the serialisable object that is used to embed this behaviour into a node. 
            /// </summary>
            /// <param name="lXmlBehaviour">The XML Behaviour to base this off. </param>
            /// <param name="lOwningBehaviour"></param>
            public SerialObject(kAIXmlBehaviour lXmlBehaviour, kAIXmlBehaviour lOwningBehaviour)
            {
                BehaviourID = lXmlBehaviour.BehaviourID;
                if (lOwningBehaviour == null)
                {
                    // in this instance, we are just serialising the behaviour for the project, so we don't need to make relative
                    XmlBehaviourFile = lXmlBehaviour.XmlLocation;
                }
                else
                {
                    XmlBehaviourFile = new kAIRelativePath(lXmlBehaviour.XmlLocation.GetFile(), lOwningBehaviour.XmlLocation.GetFile().Directory, lOwningBehaviour.XmlLocationID);
                }
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
        [Serializable]
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

            /// <summary>
            /// Display the port string
            /// </summary>
            /// <returns>The string representaton of the Port.</returns>
            public override string ToString()
            {
                return Port.ToString();
            }

        }

        //TODO: move me to the project with a specific method exposed. 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lFullName"></param>
        /// <returns></returns>
        public delegate Assembly GetAssemblyByName(string lFullName);

        /// <summary>
        /// Event handler for when a new internal port is added to this XML behaviour. 
        /// </summary>
        /// <param name="lSender">The XML Behaviour that has had the internal port added to. </param>
        /// <param name="lNewPort">The port that has been added. </param>
        public delegate void InternalPortAdded(kAIXmlBehaviour lSender, kAIPort lNewPort);

        /// <summary>
        /// Extension for XML behaviours. 
        /// </summary>
        public const string kAIXmlBehaviourExtension = "xml";

        //Internal port IDs
        private static readonly kAIPortID kOnActivatePortID = "OnActivate";
        private static readonly kAIPortID kDeactivatePortID = "Deactivate";

        /// <summary>
        /// Nodes within this XML behaviour. 
        /// </summary>
        //[NonSerialized]
        Dictionary<kAINodeID, kAINode> mInternalNodes;

        /// <summary>
        /// Ports within this XML behaviour.
        /// </summary>
        //[NonSerialized]
        Dictionary<kAIPortID, InternalPort> mInternalPorts;

        //kAIDebugInfo mDebugInfo;


        /// <summary>
        /// The location of the XML file.
        /// </summary>
        public kAIRelativePath XmlLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// The ID of the directory for this behaviour (for <see cref="kAIRelativeObject"/>) so we know what to store sub behaviours with. 
        /// </summary>
        public string XmlLocationID
        {
            get 
            {
                return "Behaviour:" + BehaviourID;
            }
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
        /// The dictionary of default internal port IDs and the functions to create them.
        /// </summary>
        static IEnumerable<DefaultPortEntry> sDefaultPortsInteralDictionary
        {
            get
            {
                yield return new DefaultPortEntry(kOnActivatePortID, (lBehaviour) =>
                {
                    return new kAITriggerPort(kOnActivatePortID, kAIPort.ePortDirection.PortDirection_Out);
                });

                yield return new DefaultPortEntry(kDeactivatePortID, (lBehaviour) =>
                {
                    kAITriggerPort lDeactivatePort = new kAITriggerPort(kDeactivatePortID, kAIPort.ePortDirection.PortDirection_In);
                    if (lBehaviour != null)
                    {
                        lDeactivatePort.OnTriggered += new kAITriggerPort.TriggerEvent(lBehaviour.lDeactivatePort_OnTriggered);
                    }
                    return lDeactivatePort;
                });
            }
        }

        /// <summary>
        /// The functions to create the default internal ports for an XML behaviour.
        /// Call the function with null to get a template port. 
        /// </summary>
        public static IEnumerable<Func<kAIXmlBehaviour, kAIPort>> sDefaultInternalPorts
        {
            get
            {
                return sDefaultPortsInteralDictionary.Select((lKVP) => { return lKVP.Value; });
            }
        }

        /// <summary>
        /// The names that are reserved for internal ports. 
        /// </summary>
        public static IEnumerable<kAIPortID> sDefaultInternalPortNames
        {
            get
            {
                return sDefaultPortsInteralDictionary.Select((lKVP) => { return lKVP.Key; });
            }
        }

        /// <summary>
        /// Triggered when a new internal port is added to this behaviour.
        /// </summary>
        [field: NonSerialized]
        public event InternalPortAdded OnInternalPortAdded;

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
            //mDebugInfo = null;
        }


        /// <summary>
        /// Create a new XML behaviour 
        /// </summary>
        /// <param name="lBehaviourID">The name of the new behaviour. </param>
        /// <param name="lFile">Where this behaviour should be saved. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        private kAIXmlBehaviour(kAIBehaviourID lBehaviourID, kAIRelativePath lFile, kAIILogger lLogger = null)
            : this(lBehaviourID, lLogger)
        {
            //kAIRelativePath lRelativePath = new kAIRelativePath(lFile, )
            XmlLocation = lFile;
            kAIRelativeObject.AddPathID(XmlLocationID, XmlLocation.GetFile().Directory);

            foreach (Func<kAIXmlBehaviour, kAIPort> lPortCreator in sDefaultInternalPorts)
            {
                AddInternalPort(lPortCreator(this), false);
            }
        }

        /// <summary>
        /// Create a kAIXMLBehaviour based off a loaded save file for it. 
        /// </summary>
        /// <param name="lSource">The loaded XML file. </param>
        /// <param name="lAssemblyGetter">The method to use to resolve assembly names to get types. </param>
        /// <param name="lFile">The source file this behaviour is loaded from. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        private kAIXmlBehaviour(InternalXml lSource, GetAssemblyByName lAssemblyGetter, kAIRelativePath lFile, kAIILogger lLogger = null)
            : this(lSource.BehaviourID, lLogger)
        {
            XmlLocation = lFile;
            if (lFile != null) // if we are null, then we are loading the behaviour without the project so we don't know where we are relative to anything. 
            {
                kAIRelativeObject.AddPathID(XmlLocationID, XmlLocation.GetFile().Directory);
            }

            foreach (Func<kAIXmlBehaviour, kAIPort> lPortCreator in sDefaultInternalPorts)
            {
                AddInternalPort(lPortCreator(this), false);
            }

            ConstructFromInternalXml(lSource, lAssemblyGetter);
        }

        /// <summary>
        /// Create a kAIXMLBehaviour based off a loaded save file for it. 
        /// </summary>
        /// <param name="lSource">The loaded XML file. </param>
        /// <param name="lAssemblyGetter">The method to use to resolve assembly names to get types. </param>
        /// <param name="lFile">The source file this behaviour is loaded from. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        private kAIXmlBehaviour(InternalXml lSource, GetAssemblyByName lAssemblyGetter, FileInfo lFile, kAIILogger lLogger = null)
            : this(lSource.BehaviourID, lLogger)
        {
            // We are loading directly from a file, so we don't know our path relative to the project
            XmlLocation = null;
            kAIRelativeObject.AddPathID(XmlLocationID, lFile.Directory);

            foreach (Func<kAIXmlBehaviour, kAIPort> lPortCreator in sDefaultInternalPorts)
            {
                AddInternalPort(lPortCreator(this), false);
            }

            ConstructFromInternalXml(lSource, lAssemblyGetter);
        }
        
        /// <summary>
        /// Add a node inside this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        public void AddNode(kAINode lNode)
        {
            lNode.OnNodeIDChanged += (lOldNodeID, lNewNodeID, lNewNode) =>
                {
                    mInternalNodes.Remove(lOldNodeID);
                    mInternalNodes.Add(lNewNodeID, lNewNode);
                };
            mInternalNodes.Add(lNode.NodeID, lNode);
        }

        /// <summary>
        /// Remove a node from within this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to remove. </param>
        public void RemoveNode(kAINode lNode)
        {
            foreach (kAIPort lPort in lNode.GetExternalPorts())
            {
                lPort.BreakAllConnexions();
            }

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
            XmlWriter lWriter = XmlWriter.Create(XmlLocation.GetFile().FullName, lSettings);
            lProjectSerialiser.WriteObject(lWriter, lSaveableBehaviour);
            lWriter.Close();   
        }

        /// <summary>
        /// Go through the internal XML and create the XML behaviour from it.
        /// </summary>
        /// <param name="lSource">The loaded XML file. </param>
        /// <param name="lAssemblyGetter">The method to use to resolve assembly names to get types. </param>
        private void ConstructFromInternalXml(InternalXml lSource, GetAssemblyByName lAssemblyGetter)
        {
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
        /// <param name="lUserData">The user data. </param>
        protected override void InternalUpdate(float lDeltaTime, object lUserData)
        {
            ReleasePorts();

            foreach (kAINode lNode in mInternalNodes.Values)
            {
                // This calls the update on the node contents (if this is a behaviour, this will only happen if the behaviour 
                // is active). 
                lNode.NodeContents.Update(lDeltaTime, lUserData);
            }
        }

        

        /// <summary>
        /// Gets the <see cref="kAIINodeSerialObject"/>of this XML Behaviour. 
        /// </summary>
        /// <returns>The serial object representing this XML behaviour. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this, lOwningBehaviour);
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
        public void AddInternalPort(kAIPort lNewPort, bool lExpose = false)
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
                    kAIPort lExternalPort = lPortToAdd.CreateOppositePort();

                    if (lExternalPort is kAITriggerPort)
                    {
                        kAITriggerPort lExternalTrigger = lExternalPort as kAITriggerPort;
                        // Trigger the internal port if our direction is in
                        if (lExternalPort.PortDirection == kAIPort.ePortDirection.PortDirection_In)
                        {
                            lExternalTrigger.OnTriggered += (lSender) =>
                                {
                                    ((kAITriggerPort)mInternalPorts[lSender.PortID].Port).Trigger();
                                };
                        }
                        else // The external port in an outward port, so should be trigger when the internal port is
                        {
                            ((kAITriggerPort)lPortToAdd).OnTriggered += (lSender) =>
                                {
                                    lExternalTrigger.Trigger();
                                };
                        }
                    }
                    else
                    {
                        kAIDataPort lExternalDataPort = lExternalPort as kAIDataPort;
                        // we select the port which as an in direction as this is the one that will receive the data
                        kAIDataPort lOutPort;
                        kAIDataPort lInPort;
                        kAIPort.OrderPorts<kAIDataPort>(lExternalDataPort, (kAIDataPort)lPortToAdd, out lOutPort, out lInPort);
                        lInPort.BindPorts(lOutPort);
                    }

                    AddExternalPort(lExternalPort);
                }

                if (OnInternalPortAdded != null)
                {
                    OnInternalPortAdded(this, lPortToAdd);
                }
            }
        }

        /// <summary>
        /// Remove an internal port from the behaviour/ 
        /// </summary>
        /// <param name="lPort">The port to remove. </param>
        public void RemoveInternalPort(kAIPort lPort)
        {
            RemoveInternalPort(lPort.PortID);
        }

        /// <summary>
        /// Remove an internal port from this behaviour.
        /// </summary>
        /// <param name="lPortID">The ID of the port to remove. </param>
        public void RemoveInternalPort(kAIPortID lPortID)
        {
            if (!mInternalPorts.ContainsKey(lPortID))
            {
                throw new kAIBehaviourPortNotFoundException(this, lPortID);
            }
            else
            {
                InternalPort lInternalPort = mInternalPorts[lPortID];
                lInternalPort.Port.BreakAllConnexions();

                if (lInternalPort.IsGloballyAccesible)
                {
                    LogWarning("Removing a globally accessible port can result in errors if the port is used in other behaviours");
                }
            }
        }


        /// <summary>
        /// Gets a port belonging to this behaviour. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lNodeID">The node the port belongs to, invalid id if an internal port. </param>
        /// <returns>The port matching lNodeID:lPortID</returns>
        public kAIPort GetInternalPort(kAIPortID lPortID, kAINodeID lNodeID)
        {
            return GetInternalPort(new kAIFQPortID(lNodeID, lPortID));
        }

        /// <summary>
        /// Gets a port belonging to this behaviour. 
        /// </summary>
        /// <param name="lPortID">The fully qualified port id. </param>
        /// <returns></returns>
        public kAIPort GetInternalPort(kAIFQPortID lPortID)
        {
            // Is the start node ID a real node.
            if (lPortID.NodeID == kAINodeID.InvalidNodeID)
            {
                if (mInternalPorts.ContainsKey(lPortID.PortID))
                {
                    return mInternalPorts[lPortID.PortID].Port;
                }
                else
                {
                    throw new Exception("Could not find internal port " + lPortID);
                }
            }
            else // The start node is invalid, so is a global port. 
            {
                if (mInternalNodes.ContainsKey(lPortID.NodeID))
                {
                    kAINode lStartNode = mInternalNodes[lPortID.NodeID];

                    if (lStartNode == null)
                    {
                        throw new Exception("Could not find node to have port on:" + lPortID.NodeID);
                    }

                    // TODO: Need a method to just get a port from the dictionary
                    try
                    {
                        return lStartNode.GetExternalPorts().First((lPort) => { return lPort.PortID == lPortID.PortID; });
                    }
                    catch (InvalidOperationException)
                    {
                        throw new Exception("Could not find port: " + lPortID);
                    }
                    
                }
                else
                {
                    throw new Exception("Could not find node " + lPortID.NodeID);
                }
            }
        }

        /// <summary>
        /// Checks whether a given behaviour contains a specific port either 
        /// as an internal port or an external port on an internal node.
        /// </summary>
        /// <param name="lPort">The port to check. </param>
        /// <returns>Returns true if either an internal port or belonging to internal node. </returns>
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
        /// Load an XML behaviour from its file. 
        /// </summary>
        /// <param name="lPath">Path to the xml behaviour</param>
        /// <param name="lAssemblyGetter">The method to get assemblies.</param>
        /// <returns>The behaviour, read for use. </returns>
        public static kAIXmlBehaviour LoadFromFile(FileInfo lPath, GetAssemblyByName lAssemblyGetter)
        {
            using (Stream lXmlStream = lPath.OpenRead())
            {
                return LoadFromStream(lXmlStream, lAssemblyGetter, lPath);
            }
        }

        static kAIXmlBehaviour LoadFromStream(Stream lSourceStream, GetAssemblyByName lAssemblyGetter, FileInfo lPath)
        {
            XmlObjectSerializer lProjectDeserialiser = new DataContractSerializer(typeof(InternalXml), kAINode.NodeSerialTypes);

            InternalXml lXmlFile = (InternalXml)lProjectDeserialiser.ReadObject(lSourceStream);

            Assert(null, lXmlFile);

            return new kAIXmlBehaviour(lXmlFile, lAssemblyGetter, lPath);
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
                Stream lXmlStream = lRealSerial.XmlBehaviourFile.GetFile().OpenRead();

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
                kAIObject.GlobalLogger.LogError("Could not find file", new KeyValuePair<string, object>("Path:", lRealSerial.XmlBehaviourFile.GetFile().FullName));
            }
            catch(System.IO.IOException)
            {
                kAIObject.GlobalLogger.LogError("Could not find file", new KeyValuePair<string, object>("Path:", lRealSerial.XmlBehaviourFile.GetFile().FullName));
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
            ((kAITriggerPort)mInternalPorts[kOnActivatePortID].Port).Trigger();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BehaviourID;
        }

        /// <summary>
        /// Generate the debug info for this XML behaviour. 
        /// </summary>
        /// <returns>The debug info for this XML behaviour. </returns>
        public override Debug.kAINodeObjectDebugInfo GenerateDebugInfo()
        {
            return new Debug.kAIXmlBehaviourDebugInfo(this);
        }
        
    }
}
