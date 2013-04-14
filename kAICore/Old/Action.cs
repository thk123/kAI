using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// An actor capable of performing actions (as set up in kAI) from a provided action id. 
    /// </summary>
    public interface kAIIActionActor
    {
        /// <summary>
        /// The actor should do the action provided. 
        /// </summary>
        /// <param name="lActionID">The string of the action to execute. </param>
        void DoAction(kAIActionID lActionID);
    }

    /// <summary>
    /// An action node is a node that executes a specific action using a supplied <see cref="kAIIActionActor"/>.
    /// </summary>
    public class kAIAction : kAINode
    {
        /// <summary>
        /// The ID of the port to use to trigger the action.
        /// </summary>
        public static readonly kAIPortID ActivatePortID = "Activate";

        /// <summary>
        /// The ID of the port that will trigger when the action is complete. 
        /// </summary>
        public static readonly kAITriggerID ActionFinishedPortID = "Done";

        /// <summary>
        /// The list of actors that will execute the action.
        /// </summary>
        List<kAIIActionActor> mActors;

        /// <summary>
        /// The ID of the action, will be sent to the actor for performing an action. 
        /// </summary>
        public kAIActionID ActionID
        {
            get;
            private set;
        }

        /// <summary>
        /// Standard constructor for creating an action. 
        /// </summary>
        /// <param name="lActionID">The ID of the action, will be send to the ActionActor. </param>
        /// <param name="mActor">The actor who will execute this action. </param>
        /// <param name="lLogger">The logger this instance will use. </param>
        public kAIAction(kAIActionID lActionID, kAIIActionActor mActor = null, kAIILogger lLogger = null)
            :base(GenerateNodeID(lActionID), GenerateInPortList(), GenerateOutPortList(), lLogger)
        {
            mActors = new List<kAIIActionActor>();
            ActionID = lActionID;
            mActors.Add(mActor);

            // Register to event (must be done after constructor

            kAITriggerReceiver lTriggerReceiver = GetPort(ActivatePortID, kAIPort.ePortDirection.PortDirection_In) as kAITriggerReceiver;
            if (lTriggerReceiver != null)
            {
                lTriggerReceiver.OnTriggered += new TriggeredEvent(TriggerReceiver_OnTriggered);
            }
            else
            {
                LogCriticalError("Action can't find the Activate port");
            }
        }

        /// <summary>
        /// Event handler for when the trigger receiver gets triggered, perform the action and tell it's trigger that we're done. 
        /// </summary>
        /// <param name="lTrigger">The trigger that caused this</param>
        /// <param name="lTriggerReciever">Our trigger that was got triggered.</param>
        void TriggerReceiver_OnTriggered(kAITrigger lTrigger, kAITriggerReceiver lTriggerReciever)
        {
            foreach (kAIIActionActor lActor in mActors)
            {
                lActor.DoAction(ActionID);
            }

            // TODO: here we are using the fact that the trigger id and the node id are the same, this doesn't seem sound
            kAITrigger lDoneTrigger = GetPort(ActionFinishedPortID.TriggerID, kAIPort.ePortDirection.PortDirection_Out) as kAITrigger;

            if (lDoneTrigger != null)
            {
                lDoneTrigger.ExecuteTrigger();
            }
            else
            {
                LogCriticalError("Action can't find the Done port");
            }
        }

        /// <summary>
        /// Generates the node ID this action will have based on the ID. 
        /// </summary>
        /// <param name="lActionID">The ID of the action</param>
        /// <returns>The node id. </returns>
        private static kAINodeID GenerateNodeID(kAIActionID lActionID)
        {
            return "Node_Action_" + lActionID;
        }

        /// <summary>
        /// Generates the list of in ports (ie the activate port) the action has. 
        /// </summary>
        /// <returns>A list of one element (the activate port).</returns>
        private static List<kAIPort> GenerateInPortList()
        {
            List<kAIPort> lInPorts = new List<kAIPort>();

            kAITriggerReceiver lInTrigger = new kAITriggerReceiver(ActivatePortID);
            lInPorts.Add(lInTrigger);

            return lInPorts;
        }

        /// <summary>
        /// Generates the list of out ports (ie the action finished port) the action has. 
        /// </summary>
        /// <returns>The list of one element (the action done port).</returns>
        private static List<kAIPort> GenerateOutPortList()
        {
            List<kAIPort> lOutPorts = new List<kAIPort>();

            kAITrigger lOutTrigger = new kAITrigger(ActionFinishedPortID);

            lOutPorts.Add(lOutTrigger);

            return lOutPorts;
        }
    }

    /// <summary>
    /// A simple wrapper class for action IDs 
    /// </summary>
    public class kAIActionID
    {
        /// <summary>
        /// The string of the action id
        /// </summary>
        public string ActionID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a ActionID from a string.
        /// </summary>
        /// <param name="lActionID"></param>
        public kAIActionID(string lActionID)
        {
            ActionID = lActionID;
        }

        /// <summary>
        /// Implicitly convert between kAIActionIDs and strings.
        /// </summary>
        /// <param name="lActionID">The existing action ID.</param>
        /// <returns>The string representing the action ID.</returns>
        public static implicit operator string(kAIActionID lActionID)
        {
            return lActionID.ActionID;
        }

        /// <summary>
        /// Implicitly convert between kAIActionIDs and strings.
        /// </summary>
        /// <param name="lActionID">The string of a action id.</param>
        /// <returns>A kAIActionID from the string. </returns>
        public static implicit operator kAIActionID(string lActionID)
        {
            return new kAIActionID(lActionID);
        }

        /// <summary>
        /// Checks two ActionID's match.
        /// </summary>
        /// <param name="lActionIDA">The first action ID.</param>
        /// <param name="lActionIDB">The second action ID.</param>
        /// <returns>Whether the two actions match.</returns>
        public static bool operator ==(kAIActionID lActionIDA, kAIActionID lActionIDB)
        {
            return lActionIDA.ActionID == lActionIDB.ActionID;
        }

        /// <summary>
        /// Checks two ActionID's don't match. 
        /// </summary>
        /// <param name="lActionIDA">The first action ID.</param>
        /// <param name="lActionIDB">The second action ID.</param>
        /// <returns>Whether the two actions match.</returns>
        public static bool operator !=(kAIActionID lActionIDA, kAIActionID lActionIDB)
        {
            return !(lActionIDA == lActionIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAIActionID lPortID = obj as kAIActionID;
            if (lPortID != null)
            {
                return lPortID == this;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Standard hash code.
        /// </summary>
        /// <returns>The hash of the object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns the ActionID.
        /// </summary>
        /// <returns>The PortID. </returns>
        public override string ToString()
        {
            return ActionID;
        }
    }
}
