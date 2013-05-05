﻿using System;//
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Reflection;

namespace kAI.Core
{
    /// <summary>
    /// The flavour of behaviour. 
    /// </summary>
    public enum eBehaviourFlavour
    {
        /// <summary>
        /// A code behaviour - one extracted from a dll.
        /// </summary>
        BehaviourFlavour_Code,

        /// <summary>
        /// A behaviour created within kAI-Editor, mainly the compilation of other behaviors/actions. 
        /// </summary>
        BehaviourFlavour_Xml,
    }

    /// <summary>
    /// Represents a kAIBehaviour (can be code or XML). 
    /// </summary>
    public abstract class kAIBehaviour : kAIObject, kAIINodeObject
    {
        readonly kAIPortID kOnActivatePort = "OnActivate";
        readonly kAIPortID kDeactivatePort = "Deactivate";

        Dictionary<kAIPortID, kAIPort> mExternalPorts;

        /// <summary>
        /// The unique (in this behaviour) name of this behaviour instance.
        /// </summary>
        public kAIBehaviourID BehaviourID
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of externally connectible ports. 
        /// </summary>
        public IEnumerable<kAIPort> GlobalPorts
        {
            get
            {
                return mExternalPorts.Values;
            }
        }

        /// <summary>
        /// Construct a new instance of this behaviour. 
        /// </summary>
        /// <param name="lNodeID">The unique (to this behaviours enviorment) node ID for this instance. </param>
        /// <param name="lLogger">Optionally, the logger this instance should use when logging anything. </param>
        public kAIBehaviour(kAIBehaviourID lNodeID, kAIILogger lLogger = null)
            : this(lLogger)
        {
            BehaviourID = lNodeID;
        }

        /// <summary>
        /// Used for constructing a behaviour with a name equal to the type. 
        /// </summary>
        /// <param name="lLogger">Optionally, the logger this instance should use when logging anything. </param>   
        protected kAIBehaviour(kAIILogger lLogger = null)
            : base(lLogger)
        {
            BehaviourID = GetType().Name;


            mExternalPorts = new Dictionary<kAIPortID, kAIPort>();

            // Create standard set of activate and deactivate ports.
            kAIPort lOnActivatePort = new kAIPort("OnActivate", kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType);
            /*kAIPort lOnPausePort = new kAIPort("OnPause", kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType);
            kAIPort lOnResumePort = new kAIPort("OnResume", kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType);*/
            //kAIPort lOnDeactiavePort = new kAIPort("OnDeactivate", kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType);
            kAIPort lDeactivatePort = new kAIPort("Deactivate", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType);

            AddPort(lOnActivatePort);
            /*AddPort(lOnPausePort);
            AddPort(lOnResumePort);
            AddPort(lOnDeactiavePort);*/
            AddPort(lDeactivatePort);
        }

        /// <summary>
        /// Add a globally accessible port to this behaviour.
        /// </summary>
        /// <param name="lNewPort">The new port to add. </param>
        /// <exception cref="kAIBehaviourPortAlreadyExistsException">
        /// If a port with the same PortID already exists in this behaviour.
        /// </exception>
        public void AddPort(kAIPort lNewPort)
        {
            if (!mExternalPorts.ContainsKey(lNewPort.PortID))
            {
                mExternalPorts.Add(lNewPort.PortID, lNewPort);
            }
            else
            {
                ThrowException(new kAIBehaviourPortAlreadyExistsException(this, lNewPort, mExternalPorts[lNewPort.PortID]));
            }
        }

        /// <summary>
        /// Gets a externally accesible port by name. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <returns>Returns the port if it exists. </returns>
        public kAIPort GetPort(kAIPortID lPortID)
        {
            return mExternalPorts[lPortID];
        }

        /// <summary>
        /// The list of externally connectible ports. 
        /// </summary>
        /// <returns>A list of ports that can be connected to externally. </returns>
        public IEnumerable<kAIPort> GetExternalPorts()
        {
            return GlobalPorts;
        }

        /// <summary>
        /// Update this behaviour. Depends on the behaviors implementation as to what happens here. 
        /// </summary>
        /// <param name="lDeltaTime">The time in seconds between the last update and this. </param>
        public abstract void Update(float lDeltaTime);

        /// <summary>
        /// Deactivate this behaviour. 
        /// </summary>
        protected void Deactivate()
        {
            GetPort(kDeactivatePort).Trigger();
        }

        /// <summary>
        /// Get the data required to turn this object into an XML structure. 
        /// </summary>
        /// <returns></returns>
        public abstract kAIINodeSerialObject GetDataContractClass();

        /// <summary>
        /// Gets the type of object returned in <see cref="GetDataContractClass"/>.
        /// </summary>
        /// <returns>The type of the serialiable used in this behaviour.</returns>
        public abstract Type GetDataContractType();
    }


    /// <summary>
    /// Represents a code behvaiour (one with an external implementation).
    /// </summary>
    public abstract class kAICodeBehaviour : kAIBehaviour
    {

        /// <summary>
        /// Create a code behaviour. 
        /// </summary>
        /// <note>Since we don't pass up a name, the type of this object is used. </note>
        /// <param name="lLogger">Optionally, the logger with which to log any errors. </param>
        protected kAICodeBehaviour(kAIILogger lLogger = null)
            :base(lLogger)
        {
        }


        /// <summary>
        /// The class used to serialise this behaviour when used as a node within an XML behaviour. 
        /// </summary>
        [DataContract()]
        internal class SerialObject : kAIObject, kAIINodeSerialObject
        {
            [DataMember()]
            public string BehaviourID;

            /// <summary>
            /// The type string of the code behaviour. 
            /// </summary>
            [DataMember()]
            public string BehaviourType;

            /// <summary>
            /// The assembly the code behaviour is implemented in. 
            /// </summary>
            [DataMember()]
            public string BehaviourAssembly;

            /// <summary>
            /// Create the serialisable object from the code behaviour.
            /// </summary>
            /// <param name="lBehaviour">The code behaviour to serialise. </param>
            /// <param name="lLogger">Optionally, the logger this instance should use when logging anything. </param>   
            public SerialObject(kAICodeBehaviour lBehaviour, kAIILogger lLogger = null)
                :this(lBehaviour.GetType(), lLogger)
            {}

            /// <summary>
            /// Create a SerialObject based of the type (so don't have to instantiate an entire behaviour). 
            /// </summary>
            /// <param name="lCodeBehaviourType">The type to base this serial object off. </param>
            /// <param name="lLogger">Optionally, the logger this instance should use when logging anything. </param>   
            public SerialObject(Type lCodeBehaviourType, kAIILogger lLogger = null)
                :base(lLogger)
            {
                // Check the type does actually inherit from CodeBehaviour.
                Assert(lCodeBehaviourType.DoesInherit(typeof(kAICodeBehaviour)));

                BehaviourID = lCodeBehaviourType.Name;
                BehaviourType = lCodeBehaviourType.FullName;
                BehaviourAssembly = lCodeBehaviourType.Assembly.FullName;
            }

            /// <summary>
            /// Gets the string representation of this serial object. 
            /// </summary>
            /// <returns>Returns the type of this code behaviour. </returns>
            public string GetFriendlyName()
            {
                return BehaviourType;
            }

            /// <summary>
            /// Create an instance of this code behaviour for embedding in an XML behaviour. 
            /// </summary>
            /// <param name="lAssemblyResolver">The method to use to resolve assembly names to get types. </param>
            /// <returns>An instantaited version of the code behaviour this serail object represents. </returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return kAICodeBehaviour.Load(this, lAssemblyResolver);
            }

            /// <summary>
            /// Gets the type of this node serial type.
            /// </summary>
            /// <returns>The type -- BehaviourCode. </returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.BehaviourCode;
            }

            /// <summary>
            /// Gets the string representation of this serial object. 
            /// </summary>
            /// <returns>Returns the type of this code behaviour. </returns>
            public override string ToString()
            {
                return GetFriendlyName();
            }
        }

        /// <summary>
        /// Get the serialisable object to save when this behaviour of a child node. 
        /// </summary>
        /// <returns>The object to serialise using a DataContract serialiser. </returns>
        public override kAIINodeSerialObject GetDataContractClass()
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serialisable object. 
        /// </summary>
        /// <note>
        /// This is probably not required, since is literlly 
        /// GetDataContractClass.GetType() and I can't see why you would need 
        /// the type and not the object itself?? (Maybe when loading?)
        /// </note>
        /// <returns></returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        /// <summary>
        /// Create the code behaviour based on the serial object.
        /// </summary>
        /// <param name="lSerialObject">The serialised object that has been loaded. </param>
        /// <param name="lAssemblyGetter">A method to get assemblys by full name to resolve types. </param>
        /// <returns>A fully instantiated kAICodeBehaviour based on the data. </returns>
        public static kAICodeBehaviour Load(kAIINodeSerialObject lSerialObject, kAIXmlBehaviour.GetAssemblyByName lAssemblyGetter)
        {
            SerialObject lCastSerialObject = lSerialObject as SerialObject;

            kAIObject.Assert(null, lCastSerialObject != null);

            Type lBehaviourType = lAssemblyGetter((string)lCastSerialObject.BehaviourAssembly).GetType(lCastSerialObject.BehaviourType);
            //kAICodeBehaviour lNewBehaviour = new kAICodeBehaviour(lData[0]);
            ConstructorInfo lConstructor = lBehaviourType.GetConstructor(new Type[] { typeof(kAIILogger) });
            object lBehaviour = lConstructor.Invoke(new Object[]{ null });
            return (kAICodeBehaviour)lBehaviour;
        }

        /// <summary>
        /// Creates a serial object from the type (to avoid instantiating the whole behaviour when loading a project).
        /// </summary>
        /// <param name="lType">The type to base the serial object off. </param>
        /// <returns>The serial object based on this type. </returns>
        public static kAIINodeSerialObject CreateSerialObjectFromType(Type lType)
        {
            //TODO: Assert(t inherits from kAICodeBehaviour)

            return new SerialObject(lType);
        }
    }
}

