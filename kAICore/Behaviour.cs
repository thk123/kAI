using System;//
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

    public abstract class kAIBehaviourBase : kAIObject
    {
        public kAIBehaviourBase(kAIILogger lLogger = null)
            : base(lLogger)
        {}

        public abstract Type GetSerialType();
    }

    /// <summary>
    /// Represents a kAIBehaviour (can be code or XML). 
    /// </summary>

    public abstract class kAIBehaviour<SerialType> : kAIBehaviourBase, kAIINodeObject<SerialType>
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

        /*public eBehaviourFlavour BehaviourFlavour
        {
            get;
            private set;
        }*/

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
            : base(lLogger)
        {
            BehaviourID = lNodeID;

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
                ThrowException(new kAIBehaviourPortAlreadyExistsException<SerialType>(this, lNewPort, mExternalPorts[lNewPort.PortID]));
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
        public abstract SerialType GetDatatContractClass();

        public override Type GetSerialType()
        {
            return typeof(SerialType);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class kAICodeBehaviour : kAIBehaviour<kAICodeBehaviour.kAICodeBehaviour_SerialiableObject>
    {
        [DataContract()]
        public class kAICodeBehaviour_SerialiableObject
        {
            //NOTE: This may not be required. 
            [DataMember()]
            public string BehaviourID;

            [DataMember()]
            public string BehaviourType;

            [DataMember()]
            public string BehaviourAssembly;

            public kAICodeBehaviour_SerialiableObject(kAICodeBehaviour lBehaviour)
            {
                BehaviourID = lBehaviour.BehaviourID;
                Type lBehaviourType = lBehaviour.GetType();

                BehaviourType = lBehaviourType.FullName;
                BehaviourAssembly = lBehaviourType.Assembly.FullName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lBehaviourID"></param>
        /// <param name="lLogger"></param>
        protected kAICodeBehaviour(kAIBehaviourID lBehaviourID, kAIILogger lLogger = null)
            : base(lBehaviourID, lLogger)
        {}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="lData"></param>
        /// <param name="lAssemblyGetter"></param>
        /// <returns></returns>
        public static kAICodeBehaviour Load(kAICodeBehaviour_SerialiableObject lSerialObject, kAIXmlBehaviour.GetAssemblyByName lAssemblyGetter)
        {
            Type lBehaviourType = lAssemblyGetter((string)lSerialObject.BehaviourAssembly).GetType(lSerialObject.BehaviourType);
            //kAICodeBehaviour lNewBehaviour = new kAICodeBehaviour(lData[0]);
            ConstructorInfo lConstructor = lBehaviourType.GetConstructor(new Type[] { typeof(kAIILogger) });
            object lBehaviour = lConstructor.Invoke(new Object[]{ null });
            return (kAICodeBehaviour)lBehaviour;
        }

        public override kAICodeBehaviour_SerialiableObject GetDatatContractClass()
        {
            return new kAICodeBehaviour_SerialiableObject(this);
        }
    }
}

