using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// Represents a port that will receive input from the game/higher behaviours.
    /// </summary>
    /// <typeparam name="T">The type of the input. </typeparam>
    public class kAIInput<T> : kAIPort
    {
        /// <summary>
        /// The current value held by this port. 
        /// </summary>
        T mCurrentValue;
        
        /// <summary>
        /// The default value this port starts out with, and can be reset to.
        /// </summary>
        T mDefaultValue;

        /// <summary>
        /// The ID used to externally access this port. 
        /// </summary>
        public kAIInputID InputID
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor for creating an Input node. 
        /// </summary>
        /// <param name="lInputID">The InputID used to access this input, eg externally.</param>
        /// <param name="lDefault">The default value this port will hold. </param>
        /// <param name="lLogger">Optionally, the logger used for this instance.</param>
        //TODO: again, we assume can simply convert an InputID to a PortID (this might not be ok - could have an input and an output with the same name?)
        public kAIInput(kAIInputID lInputID, T lDefault = default(T), kAIILogger lLogger = null)
            : base(lInputID.InputID, ePortDirection.PortDirection_Out, typeof(T), lLogger)
        {
            InputID = lInputID;
            mDefaultValue = lDefault;
            mCurrentValue = mDefaultValue;
        }

        /// <summary>
        /// Set the value of this input port. 
        /// </summary>
        /// <param name="lNewValue">The new value to be set to. </param>
        public void SetInputValue(T lNewValue)
        {
            // If we are connected, we tell the other port it has received a value.
            if (IsConnected)
            {
                kAIInputReceiver<T> lReceiver = mConnectedPort as kAIInputReceiver<T>;
                if (lReceiver != null)
                {
                    lReceiver.SetValue(lNewValue, this);
                }
                else
                {
                    LogError("Input port connected to a non-matching input port.", mConnectedPort);
                }
            }

            mCurrentValue = lNewValue;
        }

        /// <summary>
        /// Reset the valu held by this port to its default value, notifying anything connected to it. 
        /// </summary>
        public void ResetInputValue()
        {
            SetInputValue(mDefaultValue);
        }

        /// <summary>
        /// When something gets connected to an input node, we tell that node the current value. 
        /// </summary>
        /// <param name="lOtherEnd">The port that has just been conncted from. </param>
        protected override void OnConnect(kAIPort lOtherEnd)
        {
            kAIInputReceiver<T> lReceiver = lOtherEnd as kAIInputReceiver<T>;
            if (lReceiver != null)
            {
                lReceiver.SetValue(mCurrentValue, this);
            }
            else
            {
                LogError("Input port connected to a non-matching input port.", mConnectedPort);
            }
        }

    }

    /// <summary>
    /// Represents a port that can be connected to an Input port. 
    /// </summary>
    /// <typeparam name="T">The type of input to receive (currently must match the kAIInput port).</typeparam>
    class kAIInputReceiver<T> : kAIPort
    {
        /// <summary>
        /// Event delgate for when the value received changes.
        /// </summary>
        /// <param name="lNewValue">The new value this receiver now has. </param>
        /// <param name="lDataOrigin">The kAIInput port this data came from. </param>
        /// <param name="lDataDestiation">The input receiver port that got the data. </param>
        public delegate void ValueChangedEvent(T lNewValue, kAIInput<T> lDataOrigin, kAIInputReceiver<T> lDataDestiation);

        /// <summary>
        /// Event delgate for when the value gets set (might be the same as before).
        /// </summary>
        /// <param name="lNewValue">The new value this receiver now has. </param>
        /// <param name="lDataOrigin">The kAIInput port this data came from. </param>
        /// <param name="lDataDestiation">The input receiver port that got the data. </param>
        public delegate void ValueSetEvent(T lNewValue, kAIInput<T> lDataOrigin, kAIInputReceiver<T> lDataDestiation);

        /// <summary>
        /// The current value this receiver has.
        /// </summary>
        public T CurrentValue
        {
            get;
            private set;
        }        

        /// <summary>
        /// Event for when the value received changes.
        /// </summary>
        public event ValueChangedEvent OnValueChanged;

        /// <summary>
        /// Event for when the value gets set (might be the same as before).
        /// </summary>
        public event ValueSetEvent OnValueSet;

        /// <summary>
        /// Standard constructor for creating a Input Receiver port. 
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lDefaultValue">The default value this port should have if non set or connected. </param>
        /// <param name="lLogger">Optionally, the logger used for this instance.</param>
        public kAIInputReceiver(kAIPortID lPortID, T lDefaultValue = default(T), kAIILogger lLogger = null)
            : base(lPortID, ePortDirection.PortDirection_In, typeof(T), lLogger)
        {
            CurrentValue = lDefaultValue;
        }

        /// <summary>
        /// Sets the value of this port (called by a connected kAIInput port).
        /// </summary>
        /// <param name="lNewValue">The new value to set it. </param>
        /// <param name="lSource">The port that sent the data. </param>
        public void SetValue(T lNewValue, kAIInput<T> lSource)
        {
            if (!lNewValue.Equals(CurrentValue))
            {
                OnValueChanged(lNewValue, lSource, this);
            }

            CurrentValue = lNewValue;

            OnValueSet(lNewValue, lSource, this);
        }
    }

    /// <summary>
    /// A simple wrapper class for input IDs 
    /// </summary>
    public class kAIInputID
    {
        /// <summary>
        /// The string of the input id
        /// </summary>
        public string InputID
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a InputID from a string.
        /// </summary>
        /// <param name="lInputID"></param>
        public kAIInputID(string lInputID)
        {
            InputID = lInputID;
        }

        /// <summary>
        /// Implicitly convert between kAIInputIDs and strings.
        /// </summary>
        /// <param name="lInputID">The existing input ID.</param>
        /// <returns>The string representing the input ID.</returns>
        public static implicit operator string(kAIInputID lInputID)
        {
            return lInputID.InputID;
        }

        /// <summary>
        /// Implicitly convert between kAIInputIDs and strings.
        /// </summary>
        /// <param name="lInputID">The string of a input id.</param>
        /// <returns>A kAIInputID from the string. </returns>
        public static implicit operator kAIInputID(string lInputID)
        {
            return new kAIInputID(lInputID);
        }

        /// <summary>
        /// Checks two InputID's match.
        /// </summary>
        /// <param name="lInputIDA">The first input ID.</param>
        /// <param name="lInputIDB">The second input ID.</param>
        /// <returns>Whether the two inputs match.</returns>
        public static bool operator ==(kAIInputID lInputIDA, kAIInputID lInputIDB)
        {
            return lInputIDA.InputID == lInputIDB.InputID;
        }

        /// <summary>
        /// Checks two InputID's don't match. 
        /// </summary>
        /// <param name="lInputIDA">The first input ID.</param>
        /// <param name="lInputIDB">The second input ID.</param>
        /// <returns>Whether the two inputs match.</returns>
        public static bool operator !=(kAIInputID lInputIDA, kAIInputID lInputIDB)
        {
            return !(lInputIDA == lInputIDB);
        }

        /// <summary>
        /// Standard Equals method, uses proper comparison on correct objects. 
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the two objects have the same ID</returns>
        public override bool Equals(object obj)
        {
            kAIInputID lPortID = obj as kAIInputID;
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
        /// Returns the InputID.
        /// </summary>
        /// <returns>The PortID. </returns>
        public override string ToString()
        {
            return InputID;
        }
    }
}
