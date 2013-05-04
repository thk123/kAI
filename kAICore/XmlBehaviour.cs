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

            public string GetFriendlyName()
            {
                return BehaviourID;
            }

            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.BehaviourXml;
            }

            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolve)
            {
                return kAIXmlBehaviour.Load(this, lAssemblyResolve);
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
            struct InternalNode
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

                public InternalNode(kAINode lNode)
                {
                    NodeID = lNode.NodeID;
                    NodeType = lNode.GetNodeContentsType().FullName;
                    NodeTypeAssembly = lNode.GetNodeContentsType().Assembly.FullName;

                    NodeSerialType = lNode.GetNodeSerialableContentType().FullName;
                    NodeSerialAssembly = lNode.GetNodeSerialableContentType().Assembly.FullName;
                    NodeContents = lNode.GetNodeSerialisableContent();
                }
            }

            [DataMember()]
            public string BehaviourID
            {
                get;
                set;
            }

            [DataMember()]
            private List<InternalNode> InternalNodes
            {
                get;
                set;
            }

            public kAIXmlBehaviour_InternalXml()
            {
                InternalNodes = new List<InternalNode>();
            }

            public kAIXmlBehaviour_InternalXml(kAIXmlBehaviour lBehaviour)
            {
                // the behaviour to save
                BehaviourID = lBehaviour.BehaviourID;

                InternalNodes = new List<InternalNode>();

                foreach (kAINode lNodeBase in lBehaviour.InternalNodes)
                {
                    Type lContentType = lNodeBase.GetNodeSerialableContentType();
                    object lContent = lNodeBase.GetNodeSerialisableContent();

                    lBehaviour.Assert(lContentType == lContent.GetType(), "The content returned from the node does not match the reported type...");

                    InternalNodes.Add(new InternalNode(lNodeBase));
                }
            }

            public IEnumerable<kAINode> GetInternalNodes(GetAssemblyByName lAssemblyGetter)
            {
                foreach (InternalNode lInternalNode in InternalNodes)
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
                return mNodes.Values;
            }
        }

        Dictionary<kAINodeID, kAINode> mNodes;

        /// <summary>
        /// Create a new XML behaviour 
        /// </summary>
        /// <param name="lBehaviourID">The name of the new behaviour. </param>
        /// <param name="lLogger">Optionally, the logger this behaviour should use. </param>
        public kAIXmlBehaviour(kAIBehaviourID lBehaviourID, FileInfo lFile, kAIILogger lLogger = null)
            : base(lBehaviourID, lLogger)
        {
            mNodes = new Dictionary<kAINodeID, kAINode>();
            XmlLocation = lFile;
        }

        private kAIXmlBehaviour(kAIXmlBehaviour_InternalXml lSource, GetAssemblyByName lAssemblyGetter, FileInfo lSourceFile, kAIILogger lLogger = null)
            : base(lSource.BehaviourID, lLogger)
        {
            mNodes = new Dictionary<kAINodeID, kAINode>();
            foreach (kAINode lNode in lSource.GetInternalNodes(lAssemblyGetter))
            {
                mNodes.Add(lNode.NodeID, lNode);
            }

            XmlLocation = lSourceFile;
        }

        /// <summary>
        /// Add a node inside this behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        public void AddNode(kAINode lNode)
        {
            lNode.Active = false;
            mNodes.Add(lNode.NodeID, lNode);
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

        public override kAIINodeSerialObject GetDataContractClass()
        {
            return new SerialObject(this);
        }

        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        /// <summary>
        /// Load an XML Behaviour from a file. 
        /// </summary>
        /// <param name="lSerialObject">the serialised version of this XML behaviour.</param>
        /// <param name="lAssemblyGetter">The method to use resolve unknown types. </param>
        /// <returns>An instantiated behaviour based on the provided XML. </returns>
        public static kAIXmlBehaviour Load(SerialObject lSerialObject, GetAssemblyByName lAssemblyGetter)
        {
            XmlObjectSerializer lProjectDeserialiser = new DataContractSerializer(typeof(kAIXmlBehaviour_InternalXml), kAINode.NodeSerialTypes);

            kAIXmlBehaviour_InternalXml lXmlFile = (kAIXmlBehaviour_InternalXml)lProjectDeserialiser.ReadObject(lSerialObject.XmlBehaviourFile.OpenRead());

            return new kAIXmlBehaviour(lXmlFile, lAssemblyGetter, lSerialObject.XmlBehaviourFile);
        }

        
    }
}
