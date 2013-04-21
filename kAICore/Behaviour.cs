using System;//
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
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
        public kAIBehaviourID NodeID
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
            : base(lLogger)
        {
            NodeID = lNodeID;

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
                throw new kAIBehaviourPortAlreadyExistsException(this, lNewPort, mExternalPorts[lNewPort.PortID]);
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
    }

    /// <summary>
    /// Exception when try to add a global port to a behaviour that already has a port with the same node ID. 
    /// </summary>
    public class kAIBehaviourPortAlreadyExistsException : Exception
    {
        /// <summary>
        /// The port that was already present with the same PortID. 
        /// </summary>
        public kAIPort mExistingPort
        {
            get;
            private set;
        }

        /// <summary>
        /// The port that was being added but whose name was already taken. 
        /// </summary>
        public kAIPort mNewPort
        {
            get;
            private set;
        }

        /// <summary>
        /// The behaviour who the port was being added to. 
        /// </summary>
        public kAIBehaviour mBehaviour
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct the exception. 
        /// </summary>
        /// <param name="lBehaviour">The behaviour which this port was added to. </param>
        /// <param name="lExistingPort">The existing port that has the same name as the new port. </param>
        /// <param name="lNewPort">The new port to be added. </param>
        public kAIBehaviourPortAlreadyExistsException(kAIBehaviour lBehaviour, kAIPort lExistingPort, kAIPort lNewPort)
        {
            mExistingPort = lExistingPort;
            mNewPort = lNewPort;
            mBehaviour = lBehaviour;
        }

        /// <summary>
        /// Gets the message of the exception. 
        /// </summary>
        /// <returns>A string explaining what has happened. </returns>
        public override string ToString()
        {
            return "Attempted to create a port in behaviour \"" + mBehaviour.NodeID + "\" whose name already existed: \"" + mExistingPort.PortID + "\"";
        } 
    }
}

