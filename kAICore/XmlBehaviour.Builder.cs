using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace kAI.Core
{
    /// <summary>
    /// Defines an interface for modifying a behaviour that has either not been created or has.
    /// </summary>
    public interface kAIIBehaviourModifier
    {
        /// <summary>
        /// Sets the ID of the behaviour. 
        /// </summary>
        /// <param name="lBehaviourID"></param>
        void SetBehaviourID(kAIBehaviourID lBehaviourID);

        /// <summary>
        /// Set the path of the behaviour. 
        /// </summary>
        /// <param name="lPath">The path of the behaviour. </param>
        void SetPath(kAIRelativePath lPath);

        /// <summary>
        /// Gets the ports defined in this behaviour before modification. 
        /// </summary>
        IEnumerable<kAIPort> Ports
        {
            get;
        }

        /// <summary>
        /// Gets the ID of the behaviour before modification. 
        /// </summary>
        kAIBehaviourID BehaviourID
        {
            get;
        }

        /// <summary>
        /// Gets the behaviour path before modification.
        /// </summary>
        kAIRelativePath BehaviourPath
        {
            get;
        }

        /// <summary>
        /// Add a port to the modified behaviour. 
        /// </summary>
        /// <param name="lNewPort"></param>
        void AddPort(kAIPort lNewPort);

        /// <summary>
        /// Remove a port from the modified behaviour.
        /// </summary>
        /// <param name="lPortToRemove"></param>
        void RemovePort(kAIPort lPortToRemove);

        /// <summary>
        /// Return the modified behaviour. 
        /// </summary>
        /// <returns></returns>
        kAIXmlBehaviour Construct();
    }

    public partial class kAIXmlBehaviour
    {
        /// <summary>
        /// Constructs a XMLBehaviour as we go. 
        /// </summary>
        public class Builder : kAIIBehaviourModifier
        {
            List<kAIPort> lPortsToAdd;

            /// <summary>
            /// The behaviour ID as it currently stands. 
            /// </summary>
            public kAIBehaviourID BehaviourID
            {
                get;
                private set;
            }

            /// <summary>
            /// The path as it currently stands. 
            /// </summary>
            public kAIRelativePath BehaviourPath
            {
                get;
                private set;
            }        

            /// <summary>
            /// The set of ports at the start of construction - the default internal ports. 
            /// </summary>
            public IEnumerable<kAIPort> Ports
            {
                get
                {
                    // We construct some dummy ports to fill in the box. 
                    return kAIXmlBehaviour.sDefaultInternalPorts.Select((lFunc) => { return lFunc(null); });
                }
            }

            /// <summary>
            /// Create a new builder. 
            /// </summary>
            public Builder()
            {
                lPortsToAdd = new List<kAIPort>();
            }

            /// <summary>
            /// Set the behaviour ID of the behaviour to be constructed. 
            /// </summary>
            /// <param name="lBehaviourID">The new behaviour ID. </param>
            public void SetBehaviourID(kAIBehaviourID lBehaviourID)
            {
                BehaviourID = lBehaviourID;
            }

            /// <summary>
            /// Sets the location of the behaviour to be created. 
            /// </summary>
            /// <param name="lBehaviourPath">The relative path of where the behaviour should be saved.</param>
            public void SetPath(kAIRelativePath lBehaviourPath)
            {
                BehaviourPath = lBehaviourPath;
            }

            /// <summary>
            /// Request that a externally accessible internal port should be added to the behaviour. 
            /// </summary>
            /// <param name="lPort">The port to add. </param>
            public void AddPort(kAIPort lPort)
            {
                lPortsToAdd.Add(lPort);
            }

            /// <summary>
            /// Request that a specific port should in fact not be added to the behaviour. 
            /// </summary>
            /// <param name="lPort">The port we no longer wish to add. </param>
            public void RemovePort(kAIPort lPort)
            {
                lPortsToAdd.Remove(lPort);
            }

            /// <summary>
            /// Finalise the behaviour and construct it. 
            /// </summary>
            /// <returns>A behaviour as described by this builder. </returns>
            public kAIXmlBehaviour Construct()
            {
                // convert file info in relative path
                kAIXmlBehaviour lNewBehaviour = new kAIXmlBehaviour(BehaviourID, BehaviourPath);

                foreach (kAIPort lPort in lPortsToAdd)
                {
                    lNewBehaviour.AddInternalPort(lPort, true);
                }

                return lNewBehaviour;
            }

        }
    }
}
