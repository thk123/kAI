using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{

    /// <summary>
    /// The type of event fired when a trigger is hit. 
    /// </summary>
    /// <param name="lTrigger">The trigger that was fired.</param>
    /// <param name="lTriggerReciever">The trigger reciver that recieved it. </param>
    public delegate void TriggeredEvent(kAITrigger lTrigger, kAITriggerReceiver lTriggerReciever);

    /// <summary>
    /// A trigger that will get fired. An out port. 
    /// </summary>
    public class kAITrigger : kAIPort
    {
        /// <summary>
        /// The ID of the trigger, used externally to fire the trigger. 
        /// </summary>
        public kAITriggerID TriggerID
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor for creating a trigger. 
        /// </summary>
        /// <param name="lTriggerID">The ID of the trigger.</param>
        /// <param name="lLogger">Optionaly, the logger this instance should use. </param>
        public kAITrigger(kAITriggerID lTriggerID, kAIILogger lLogger = null)
            : base(lTriggerID.TriggerID, kAIPort.ePortDirection.PortDirection_Out, kAIPortType.TriggerType, lLogger)
        {
            TriggerID = lTriggerID;
        }

        /// <summary>
        /// Cause this trigger to be executed, notifying any trigger receivers. 
        /// </summary>
        public void ExecuteTrigger()
        {
            // If we are connected to something, we tell it the event was triggered.
            if (IsConnected)
            {
                foreach (kAIPort lPort in mConnectingPorts )
                {
                    kAITriggerReceiver lReceiver = lPort as kAITriggerReceiver;
                    if (lReceiver != null)
                    {
                        lReceiver.ExecuteTrigger(this);
                    }
                    else
                    {
                        LogError("Trigger connected to a non-trigger receiver", lReceiver);
                    }
                }
            }
        }
    }

    /// <summary>
    /// A trigger receiver represents something that is connected to a trigger. 
    /// </summary>
    public class kAITriggerReceiver : kAIPort
    {
        /// <summary>
        /// The event triggered when a connected trigger goes off. 
        /// </summary>
        public event TriggeredEvent OnTriggered;

        /// <summary>
        /// Default constructor for a trigger receiver.
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lLogger">Optionaly, the logger this instance should use. </param>
        public kAITriggerReceiver(kAIPortID lPortID, kAIILogger lLogger = null)
            : base(lPortID, kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType, lLogger)
        {
        }

        /// <summary>
        /// Cause the trigger to be executed, firing the event. 
        /// </summary>
        /// <param name="lTrigger">The trigger that has caused this. </param>
        public void ExecuteTrigger(kAITrigger lTrigger)
        {
            OnTriggered(lTrigger, this);
        }
    }

    /// <summary>
    /// A simple wrapper class for trigger IDs 
    /// </summary>
    public class kAITriggerID
    {
        /// <summary>
        /// The string of the trigger id
        /// </summary>
        public string TriggerID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a TriggerID from a string.
        /// </summary>
        /// <param name="lTriggerID"></param>
        public kAITriggerID(string lTriggerID)
        {
            TriggerID = lTriggerID;
        }

        /// <summary>
        /// Implicitly convert between kAITriggerIDs and strings.
        /// </summary>
        /// <param name="lTriggerID">The existing trigger ID.</param>
        /// <returns>The string representing the trigger ID.</returns>
        public static implicit operator string(kAITriggerID lTriggerID)
        {
            return lTriggerID.TriggerID;
        }

        /// <summary>
        /// Implicitly convert between kAITriggerIDs and strings.
        /// </summary>
        /// <param name="lTriggerID">The string of a trigger id.</param>
        /// <returns>A kAITriggerID from the string. </returns>
        public static implicit operator kAITriggerID(string lTriggerID)
        {
            return new kAITriggerID(lTriggerID);
        }

        /// <summary>
        /// Checks two TriggerID's match.
        /// </summary>
        /// <param name="lTriggerIDA">The first trigger ID.</param>
        /// <param name="lTriggerIDB">The second trigger ID.</param>
        /// <returns>Whether the two triggers match.</returns>
        public static bool operator ==(kAITriggerID lTriggerIDA, kAITriggerID lTriggerIDB)
        {
            return lTriggerIDA.TriggerID == lTriggerIDB.TriggerID;
        }

        /// <summary>
        /// Checks two TriggerID's don't match. 
        /// </summary>
        /// <param name="lTriggerIDA">The first trigger ID.</param>
        /// <param name="lTriggerIDB">The second trigger ID.</param>
        /// <returns>Whether the two triggers match.</returns>
        public static bool operator !=(kAITriggerID lTriggerIDA, kAITriggerID lTriggerIDB)
        {
            return !(lTriggerIDA == lTriggerIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAITriggerID lPortID = obj as kAITriggerID;
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
        /// Returns the TriggerID.
        /// </summary>
        /// <returns>The PortID. </returns>
        public override string ToString()
        {
            return TriggerID;
        }
    }
}
