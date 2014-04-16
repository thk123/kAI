using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using kAI.Core.Debug;

namespace kAI.Core
{
    /// <summary>
    /// Represents a port that can receive multiple objects of the same type.
    /// At the moment, this can only be used as an in bound port and only accessible
    /// via a code behaviour. 
    /// </summary>
    public abstract class kAIEnumerableDataPort : kAIPort
    {
        /// <summary>
        /// Create a EnumerableDataPort
        /// </summary>
        /// <param name="lPortID">The ID of the port.  </param>
        /// <param name="lPortType">The type of the value (ie, we accept a list of this type, not just this type). </param>
        /// <param name="lLogger">Optionally, the logger this port should use. </param>
        protected kAIEnumerableDataPort(kAIPortID lPortID, kAIPortType lPortType, kAIILogger lLogger=  null)
            : base(lPortID, ePortDirection.PortDirection_In, lPortType, lLogger)
        {
        }

        /// <summary>
        /// Create an enumerable port for a specific type. 
        /// </summary>
        /// <param name="lPortType">The type which you want </param>
        /// <param name="lPortID"></param>
        /// <returns></returns>
        public static kAIEnumerableDataPort CreateEnumerablePort(Type lPortType, kAIPortID lPortID)
        {
            if (lPortType.GetInterfaces().Contains(typeof(IEnumerable<>)))
            {
                kAIObject.LogWarning(null, "Creating an enumerable port with an enumerable type. This means the input will be a collection of collections. Are you sure this is what you want?",
                    new KeyValuePair<String, object>("Port", lPortID), new KeyValuePair<String, object>("Type", lPortType));
            }

            Type lDataPortType = typeof(kAIEnumerableDataPort<>);
            Type lGenericDataPortType = lDataPortType.MakeGenericType(lPortType);
            ConstructorInfo lDataPortConstructor = lGenericDataPortType.GetConstructor(new Type[] { 
                            typeof(kAIPortID), typeof(kAIILogger) });

            return (kAIEnumerableDataPort)lDataPortConstructor.Invoke(new object[] { lPortID, null });
        }

    }

    /// <summary>
    /// Represents a port that can be connected to from multiple data ports of type T.
    /// It combines these ports in to one list. 
    /// At the moment, this can only be used as an in bound port and only accessible
    /// via a code behaviour. 
    /// </summary>
    /// <typeparam name="T">The type we are creating an enumerable collection for. </typeparam>
    public class kAIEnumerableDataPort<T> : kAIEnumerableDataPort
    {
        // A mapping between the port that send the value and the value, used if connected to multiple Ts
        Dictionary<kAIFQPortID, T> mValues;

        /// <summary>
        /// Gets all the objects stored at this port in an undefined order. 
        /// </summary>
        public IEnumerable<T> Values
        {
            get
            {
                return mValues.Values;
            }
        }
                
        /// <summary>
        /// Create an enumerable data port that accepts connexions from data ports of type T.
        /// </summary>
        /// <param name="lPortID">The ID of the port. </param>
        /// <param name="lLogger">Optionally, the logger the port should use. </param>
        public kAIEnumerableDataPort(kAIPortID lPortID, kAIILogger lLogger = null)
            :base(lPortID, typeof(T), lLogger)
        {
            mValues = new Dictionary<kAIFQPortID, T>();
        }


        /// <summary>
        /// Releases this port. On enumerable ports, this doesn't do anything. 
        /// </summary>
        public override void Release()
        {}


        /// <summary>
        /// Would create the opposite direction port, ie if this was an internal XML port would create the external one.
        /// However, this is not implemented yet as we don't have out going enumerable ports. 
        /// </summary>
        /// <returns>The port created. </returns>
        internal override kAIPort CreateOppositePort()
        {
            throw new Exception("Cannot create opposite port - this would create a data port of IEnumerbale<T> which you can't use yet");
        }

        /*protected override ePortConnexionResult CanConnect(kAIPort lOtherPort)
        {
            ePortConnexionResult baseResult = base.CanConnect(lOtherPort);

            if (baseResult == ePortConnexionResult.PortConnexionResult_InvalidDataType)
            {
                kAIPortType lOtherType = lOtherPort.DataType;

                // if we are not a trigger
                if (!lOtherType.Equals(kAIPortType.TriggerType))
                {
                    // then we are a data type, lets see if we work as an entry in to our value
                    Type concreteType = lOtherType.DataType;
                    if (concreteType is T || concreteType is IEnumerable<T>)
                    {
                        return kAIPort.ePortConnexionResult.PortConnexionResult_OK;
                    }
                }
            }

            return baseResult;
        }*/

        /// <summary>
        /// Gets all the objects stored at this port in an undefined order. 
        /// </summary>
        /// <returns>A IEnumerable&lt;T&gt; of all the data ports connected to this. </returns>
        public override object GetData()
        {
            return mValues.Values;
        }

        /// <summary>
        /// Set the data of the port. Object should be of type T. 
        /// </summary>
        /// <param name="lObject">An object of type T that will be added to the list</param>
        /// <param name="lSender"></param>
        public override void SetData(object lObject, kAIPort lSender)
        {
            // The object should be assignable from T. 
            if (typeof(T).IsAssignableFrom(lObject.GetType()))
            {
                if (lSender == null)
                {
                    throw new Exception("No port sender, this port has to be inbound, this makes no sense");
                }

                mValues[lSender.FQPortID] = (T)lObject;
            }
            else 
            {
                throw new Exception("Invalid type to set the Enumerable port to.");

                // here we can test whether the thing is actually an enuerable object and just work like a data port
                // but we are not supporting that for now. 
                /*IEnumerable<T> enumerableSource = lObject as IEnumerable<T>;

                if (enumerableSource != null)
                {
                    mValuesAll.Clear();
                    foreach (T lNewT in enumerableSource)
                    {
                        mValuesAll.Add(lNewT);
                    }
                }
                else
                {*/
                    
                //}
            }
        }

        /// <summary>
        /// Generate the debug info for this enumerable data port. 
        /// </summary>
        /// <returns>The debug info for this enumerable data port. </returns>
        public override Debug.kAIPortDebugInfo GenerateDebugInfo()
        {
            return new kAIEnumerablePortDebugInfo(this, Values.Select<T, string>((lItem) => { return lItem.ToString(); } ));
        }
        
    }

    
}


