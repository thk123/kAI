using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Reflection;



namespace kAI.Core
{
    using kAIPortCreatorFunction = Func<kAIBehaviour, kAIPort>;
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
    public abstract class kAIBehaviour : kAINodeObject
    {
        //External port IDs
        private static readonly kAIPortID kActivatePortID = "Activate";
        private static readonly kAIPortID kDeactivatePortID = "Deactivate";
        private static readonly kAIPortID kOnDeactivatePortID = "OnDeactivated";

        bool mActive;

        bool mWasActive;

        /// <summary>
        /// The unique (in this behaviour) name of this behaviour instance.
        /// </summary>
        public kAIBehaviourID BehaviourID
        {
            get;
            private set;
        }

        /// <summary>
        /// Is this behaviour currently active. 
        /// </summary>
        public bool Active
        {
            get
            {
                return mActive;
            }
            private set
            {
                if (mActive != value)
                {
                    mWasActive = mActive;
                    mActive = value;

                    //TODO: Notifications to derived classes and methods for them to deactivate the whole system.
                }
            }
        }

        /// <summary>
        /// Gets the functions to create the default external ports for a behaviour.
        /// Call the function with null to generate some dummy ports. 
        /// </summary>
        public static IEnumerable<kAIPortCreatorFunction> sDefaultExternalPorts
        {
            get
            {
                return sDefaultExternalPortsDictionary.Select((lKVP) => { return lKVP.Value; });
            }
        }

        /// <summary>
        /// Gets the reserved names for the default external ports for a behaviour. 
        /// </summary>
        public static IEnumerable<kAIPortID> sDefaultExternalPortNames
        {
            get
            {
                return sDefaultExternalPortsDictionary.Select((lKVP) => { return lKVP.Key; });
            }
        }

        /// <summary>
        /// Contains the dictionary of reserved names and the functions to create the corresponding ports.
        /// </summary>
        static IEnumerable<KeyValuePair<kAIPortID, kAIPortCreatorFunction>> sDefaultExternalPortsDictionary
        {
            get
            {
                yield return new KeyValuePair<kAIPortID, kAIPortCreatorFunction>(kActivatePortID, (lBehaviour) =>
                {
                    kAITriggerPort lActivatePort = new kAITriggerPort(kActivatePortID, kAIPort.ePortDirection.PortDirection_In);
                    
                    if (lBehaviour != null)
                    {
                        lActivatePort.OnTriggered += new kAITriggerPort.TriggerEvent(lBehaviour.lActivatePort_OnTriggered);
                    }

                    return lActivatePort;
                });

                yield return new KeyValuePair<kAIPortID, kAIPortCreatorFunction>(kDeactivatePortID, (lBehaviour) =>
                {

                    kAITriggerPort lDeactivatePort = new kAITriggerPort(kDeactivatePortID, kAIPort.ePortDirection.PortDirection_In);

                    if (lBehaviour != null)
                    {
                        lDeactivatePort.OnTriggered += new kAITriggerPort.TriggerEvent(lBehaviour.lDeactivatePort_OnTriggered);
                    }

                    return lDeactivatePort;
                });

                yield return new KeyValuePair<kAIPortID, kAIPortCreatorFunction>(kOnDeactivatePortID, (lBehaviour) =>
                {
                    kAITriggerPort lOnDeactivatedPort = new kAITriggerPort(kOnDeactivatePortID, kAIPort.ePortDirection.PortDirection_Out);
                    return lOnDeactivatedPort;
                });
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

            foreach (Func<kAIBehaviour, kAIPort> lDefaultPortCreator in sDefaultExternalPorts)
            {
                AddExternalPort(lDefaultPortCreator(this));
            }

            mActive = false;
            mWasActive = false;
        }

        void lActivatePort_OnTriggered(kAIPort lSender)
        {
            Activate(); 
        }

        void lDeactivatePort_OnTriggered(kAIPort lSender)
        {
            Deactivate();
        }

        /// <summary>
        /// Should be used if this behaviour is not contained within another behaviour as 
        /// these can behave a little differently. 
        /// </summary>
        public void SetGlobal()
        {
            foreach (kAIPort lExternalPort in GlobalPorts)
            {
                lExternalPort.OwningBehaviour = null;
            }
        }

        /// <summary>
        /// Gets the external port of this behaviour to activate it. 
        /// </summary>
        /// <returns>The external port to activate this behaviour. </returns>
        public void ForceActivation()
        {
            ((kAITriggerPort)GetPort(kActivatePortID)).Trigger();
            //mExternalPorts[kActivatePortID].Release();
        }

        /// <summary>
        /// Gets the external port of this behaviour to deactivate it. 
        /// </summary>
        /// <returns>The external port to deactivate this behaviour. </returns>
        public void ForceDeactivate()
        {

            ((kAITriggerPort)GetPort(kDeactivatePortID)).Trigger();
            //mExternalPorts[kDeactivatePortID].Release();
        }

        /// <summary>
        /// Update this behaviour (if active). 
        /// </summary>
        /// <param name="lDeltaTime">The time in seconds that has passed since the last frame. </param>
        /// <param name="lUserData">The user data. </param>
        public override void Update(float lDeltaTime, object lUserData)
        {
            if (Active)
            {
                //InternalUpdate(lDeltaTime, lUserData);

                try
                {
                    InternalUpdate(lDeltaTime, lUserData);
                    
                }
                catch (System.Security.SecurityException ex)
                {
                    LogMessage("Failed to update code behaviour " + BehaviourID + ": " + ex.Message);
                }
            }
            else if (mWasActive) // just been deactivated so we trigger the OnDeactivated Port
            {
                ((kAITriggerPort)GetPort(kOnDeactivatePortID)).Trigger();
                mWasActive = false;
            }
        }

        /// <summary>
        /// Update this behaviour. Depends on the behaviors implementation as to what happens here. 
        /// </summary>
        /// <param name="lDeltaTime">The time in seconds between the last update and this. </param>
        /// <param name="lUserData">The user data. </param>
        protected abstract void InternalUpdate(float lDeltaTime, object lUserData);

        /// <summary>
        /// Deactivate this behaviour. 
        /// </summary>
        protected void Deactivate()
        {
            if (Active)
            {
                Active = false;
                OnDeactivate();
                // TODO: this is called from XML behaviour if its inner Deactivate port is triggered, this could trigger the OnDeactivated without risk
            }
        }

        /// <summary>
        /// Internal call for thwn the beavhiour gets activated. 
        /// </summary>
        protected virtual void OnDeactivate()
        {}

        /// <summary>
        /// Internal call for when the behaviour gets activated. 
        /// </summary>
        protected virtual void OnActivate()
        {}

        /// <summary>
        /// Activate the behaviour.
        /// </summary>
        protected void Activate()
        {
            if (!Active)
            {                
                Active = true;
                OnActivate();
            }
        }

        /// <summary>
        /// Gets the name nodes should be based off. 
        /// </summary>
        /// <returns>The behaviour id. </returns>
        public override string GetNameTemplate()
        {
            return BehaviourID;
        }

    }


    /// <summary>
    /// Represents a code behaviour (one with an external implementation).
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
                BehaviourAssembly = lCodeBehaviourType.Assembly.GetName().Name;
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
        /// <param name="lOwningBehaviour">The XML behaviour this behaviour is a node in. </param>
        /// <returns>The object to serialise using a DataContract serialiser. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serialisable object. 
        /// </summary>
        /// <note>
        /// This is probably not required, since is literally 
        /// GetDataContractClass.GetType() and I can't see why you would need 
        /// the type and not the object itself?? (Maybe when loading?)
        /// </note>
        /// <returns>The type of the serial object. </returns>
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
            Object[] lParameters;

            if (lConstructor == null)
            {
                lConstructor = lBehaviourType.GetConstructor(Type.EmptyTypes);
                if (lConstructor == null)
                {
                    throw new Exception("Could not instantiate code behaviour - no valid constructor");
                }
                lParameters = new Object[0];
            }
            else
            {
                lParameters = new Object[] { null };
            }

            object lBehaviour = lConstructor.Invoke(lParameters);
            return (kAICodeBehaviour)lBehaviour;
        }

        /// <summary>
        /// Creates a serial object from the type (to avoid instantiating the whole behaviour when loading a project).
        /// </summary>
        /// <param name="lType">The type to base the serial object off. </param>
        /// <returns>The serial object based on this type. </returns>
        public static kAIINodeSerialObject CreateSerialObjectFromType(Type lType)
        {
            kAIObject.Assert(null, lType.DoesInherit(typeof(kAICodeBehaviour)));

            return new SerialObject(lType);
        }

        /// <summary>
        /// Generate debug info for this code behaviour.
        /// </summary>
        /// <returns>The debug info for this behaviour. </returns>
        public override Debug.kAINodeObjectDebugInfo GenerateDebugInfo()
        {
            return new Debug.kAIBehaviourDebugInfo(this);
        }
    }
}

