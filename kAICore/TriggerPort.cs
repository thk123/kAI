using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// A port that can be triggered. 
    /// </summary>
    public class kAITriggerPort : kAIPort
    {

        /// <summary>
        /// For when this node gets triggered by something connecting to it (or an external event).
        /// </summary>
        /// <param name="lSender">The port that just got triggered. </param>
        public delegate void TriggerEvent(kAIPort lSender);
        //TODO: Maybe provide the origin of the trigger?

        /// <summary>
        /// Occurs when this node gets triggered. 
        /// </summary>
        public event TriggerEvent OnTriggered;

        bool mHasBeenTriggered;

        DateTime mLastTrigger;

        /// <summary>
        /// Create a new trigger port. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lPortDirection">The direction of the port.</param>
        /// <param name="lLogger">Optionally, the logger the port shall use. </param>
        public kAITriggerPort(kAIPortID lPortID, ePortDirection lPortDirection, kAIILogger lLogger = null)
            : base(lPortID, lPortDirection, kAIPortType.TriggerType, lLogger)
        {
            OnTriggered += new TriggerEvent(kAITriggerPort_OnTriggered);

            mLastTrigger = new DateTime(0);
        }

        

        void kAITriggerPort_OnTriggered(kAIPort lSender)
        {
            mLastTrigger = DateTime.Now;
        }

        /// <summary>
        /// Tell this port it just got triggered. 
        /// </summary>
        public void Trigger()
        {
            Assert(mOwningBehaviourSet, "A port has not been told what XML behaviour it belongs to: " + PortID.ToString());
            if (!CheckState())
            {
                throw new TriggeredPortInReleasePhaseException(this);
            }



            if (PortDirection == ePortDirection.PortDirection_Out)
            {
                foreach (kAITriggerPort lConnectedPorts in mConnectingPorts.Values.Cast<kAITriggerPort>())
                {
                    lConnectedPorts.Trigger();
                }
            }

            mHasBeenTriggered = true;

            // If we are a global port (i.e. not inside any XML behaviour) we will never be released so we just release instantly. 
            // TODO: Prove this is ok
            if (mOwningBehaviour == null)
            {
                Release();
            }
        }

        /// <summary>
        /// Release this port (first stage of the drill down update). 
        /// </summary>
        public override void Release()
        {
            Assert(mOwningBehaviourSet, "A port has not been told what XML behaviour it belongs to: " + PortID.ToString());
            if (mHasBeenTriggered)
            {
                // TODO: it is vital this does not trigger more ports within the same behaviour
                if (OnTriggered != null)
                {
                    OnTriggered(this);
                }
                mHasBeenTriggered = false;
            }
        }

        internal override kAIPort CreateOppositePort()
        {
            return new kAITriggerPort(PortID, PortDirection.OppositeDirection());
        }

        /// <summary>
        /// Check the the state is not in a release of another port (if we are, we cannot trigger additional ports). 
        /// </summary>
        /// <returns>True if the state is valid for triggering a port. </returns>
        private bool CheckState()
        {
            if (mOwningBehaviour == null)
            {
                // This is only true when we are an external port of a behaviour that is at the root#
                // i.e. not embedded in some other behaviour. This means there is no release phase 
                return true;
            }
            else
            {
                return !mOwningBehaviour.InReleasePhase;
            }
        }

        /// <summary>
        /// Generate the debug info for this trigger port. 
        /// </summary>
        /// <returns>The debug info for this trigger port. </returns>
        public override Debug.kAIPortDebugInfo GenerateDebugInfo()
        {
            return new Debug.kAITriggerPortDebugInfo(mLastTrigger, this);
        }
    }

    class TriggeredPortInReleasePhaseException : Exception
    {
        public kAIPort PortTriggered
        {
            get;
            private set;
        }

        public TriggeredPortInReleasePhaseException(kAIPort lPortTriggered)
        {
            PortTriggered = lPortTriggered;
        }

        public override string Message
        {
            get
            {
                return "Cannot trigger port " + PortTriggered.FQPortID + " in the release phase as this causes non determinism.";
            }
        }
    }
}
