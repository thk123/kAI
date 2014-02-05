using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace kAI.Core
{
    /// <summary>
    /// A non-generic DataPort. 
    /// TODO: probably not needed accept to have the static constructor. 
    /// </summary>
    public abstract class kAIDataPort : kAIPort
    {
        /// <summary>
        /// Create a data port.
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lPortDirection">The direction of the port. </param>
        /// <param name="lType">The type of the port. </param>
        /// <param name="lLogger">Optionally, the logger this port should use. </param>
        protected kAIDataPort(kAIPortID lPortID, ePortDirection lPortDirection, kAIPortType lType, kAIILogger lLogger = null)
            : base(lPortID, lPortDirection, lType, lLogger)
        {}

        // TODO: Move me up the heirachy 
        /// <summary>
        /// Bind two ports together so as the data is set in one, it is mirrored in the other. 
        /// </summary>
        /// <param name="lOtherEnd">The other end of the binding. </param>
        internal abstract void BindPorts(kAIDataPort lOtherEnd);

        /// <summary>
        /// Get the data stored in this data port. 
        /// </summary>
        /// <returns>The current data in the port. </returns>
        public abstract object GetData();

        /// <summary>
        /// Set the data in this data port. 
        /// </summary>
        /// <param name="lObject">The new value the data should have. </param>
        public abstract void SetData(object lObject);
        /// <summary>
        /// Create a data port of the given type. 
        /// </summary>
        /// <param name="lDataType">The type of the data port. </param>
        /// <param name="lPortID">The ID of the data port. </param>
        /// <param name="lPortDirection">The direction of the data port. </param>
        /// <returns>A kAIDataPort<lDataType>. </lDataType></returns>
        public static kAIDataPort CreateDataPort(Type lDataType, kAIPortID lPortID, ePortDirection lPortDirection)
        {
            Type lDataPortType = typeof(kAIDataPort<>);
            Type lGenericDataPortType = lDataPortType.MakeGenericType(lDataType);
            ConstructorInfo lDataPortConstructor = lGenericDataPortType.GetConstructor(new Type[] { 
                            typeof(kAIPortID), typeof(kAIPort.ePortDirection), typeof(kAIILogger) });

            return (kAIDataPort)lDataPortConstructor.Invoke(new object[] { lPortID, lPortDirection, null });
        }
    }

    /// <summary>
    /// Represents a data port. 
    /// </summary>
    /// <typeparam name="T">The type of the data port. </typeparam>
    public class kAIDataPort<T> : kAIDataPort
    {
        /*// <summary>
        /// Some event has happened with the data. 
        /// </summary>
        /// <param name="lSender">The port whose data has been affected. </param>
        /// <param name="lData">The new value of the data. </param>
        //public delegate void DataEvent(kAIPort lSender, T lData);

        /// <summary>
        /// Occurs when the data goes from a default value to a non-default value. 
        /// </summary>
        //public event DataEvent OnDataSet;

        /// <summary>
        /// Occurs when the data changes from a non-default value to a non-default value. 
        /// </summary>
        //public event DataEvent OnDataChanged;

        /// <summary>
        /// Occurs when the data changes from a non-default value to a default value. 
        /// </summary>
        //public event DataEvent OnDataUnset;*/

        T mData;

        /// <summary>
        /// Get the data currently in this port. 
        /// </summary>
        public T Data
        {
            get
            {
                return mData;
            }
            set
            {
                if(PortDirection == kAIPort.ePortDirection.PortDirection_In)
                {
                    // TODO: this is causing a crash from Unity...
                    /*if (IsTNull(mData)) // if we are currently unset
                    {
                        if (!IsTNull(value)) // providing we are setting to something
                        {
                            OnDataSet(this, value);
                        }
                    }
                    else // currently set to something
                    {
                        if (!mData.Equals(value)) // if we are different
                        {
                            if (value.Equals(default(T))) // if we are setting to null or equivalent
                            {
                                OnDataUnset(this, value);
                            }
                            else // new value not null or equivalent. 
                            {
                                OnDataChanged(this, value);
                            }
                        }
                    }*/
                }
                else
                {
                    foreach (kAIDataPort<T> lConnectedPorts in mConnectingPorts.Values.Cast<kAIDataPort<T>>())
                    {
                        lConnectedPorts.Data = value;
                    }
                }

                mData = value;
                if (mBoundEnd != null)
                {
                    mBoundEnd.Data = value;
                }
            }

        }

        kAIDataPort<T> mBoundEnd;

        /// <summary>
        /// Create a DataPort
        /// </summary>
        /// <param name="lPortID">The unique (to owner) ID of this port. </param>
        /// <param name="lPortDirection">The direction of the port. </param>
        /// <param name="lLogger">Optionally, the logger this port should use.</param>
        public kAIDataPort(kAIPortID lPortID, ePortDirection lPortDirection, kAIILogger lLogger = null)
            : base(lPortID, lPortDirection, typeof(T), lLogger)
        {

        }

        /// <summary>
        /// Get the data stored in this data port. 
        /// </summary>
        /// <returns>The current data in the port. </returns>
        public override object GetData()
        {
            return Data;
        }

        /// <summary>
        /// Set the data in this data port. 
        /// </summary>
        /// <param name="lData">The new value the data should have. </param>
        public override void SetData(object lData)
        {
            Data = (T)lData;
        }

        internal override void BindPorts(kAIDataPort lOtherEnd)
        {
            if (PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                throw new Exception("Attempt to bind an outbound port, should bind in port to out port");
            }

            kAIDataPort<T> lCastOtherEnd = lOtherEnd as kAIDataPort<T>;
            if (lCastOtherEnd != null)
            {
                mBoundEnd = lCastOtherEnd;
            }
            else
            {
                throw new Exception("Attempted to bind 2 inequivalent data ports?");
            }
        }

        /// <summary>
        /// Create the port matching this but in the other direction.
        /// </summary>
        /// <returns>A port of the same type as this, but in the opposite direction. </returns>
        internal override kAIPort CreateOppositePort()
        {
            return new kAIDataPort<T>(PortID, PortDirection.OppositeDirection());
        }


        /// <summary>
        /// Releases this port. On data ports, this doesn't do anything. 
        /// </summary>
        public override void Release()
        {}

        /// <summary>
        /// Occurs when the port it gets connected to another port. 
        /// </summary>
        /// <param name="lOtherEnd">The port it is being connected to. </param>
        protected override void OnConnect(kAIPort lOtherEnd)
        {
            base.OnConnect(lOtherEnd);

            if (PortDirection == kAIPort.ePortDirection.PortDirection_Out)
            {
                kAIDataPort<T> lOtherEndCast = (kAIDataPort<T>)lOtherEnd;
                Assert(lOtherEndCast, "Invalid port being connected to");
                lOtherEndCast.Data = Data;
            }
        }

        private bool IsTNull(T lT)
        {
            return ((object)lT) == null || lT == null || lT.Equals(default(T));
        }
    }
}
