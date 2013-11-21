using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Reflection;

namespace kAI.Core
{
    /// <summary>
    /// A constant node. 
    /// </summary>
    public abstract class kAIConstantNode : kAINodeObject
    {
        Type mType;

        /// <summary>
        /// Create a constant node of specific type. 
        /// </summary>
        /// <param name="lConstantType">The type of the constant. </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        public kAIConstantNode(Type lConstantType, kAIILogger lLogger= null)
            : base(lLogger)
        {
            mType = lConstantType;
        }

        /// <summary>
        /// Update this node, does nothing for a constant. 
        /// </summary>
        /// <param name="lDeltaTime">The time passed since the last update. </param>
        /// <param name="lUserData">The user data. </param>
        public override void Update(float lDeltaTime, object lUserData)
        { }

        /// <summary>
        /// Gets the default name for creating a node of this type.
        /// </summary>
        /// <returns>A example name for a node of this type. </returns>
        public override string GetNameTemplate()
        {
            return "Constant" + mType.Name;
        }

        /// <summary>
        /// Load this node object using the relevant serial object. 
        /// </summary>
        /// <param name="lSerialObject">The serialsed version of this object. </param>
        /// <param name="lAssemblyResolver">The method to resolve assembly names. </param>
        /// <returns>An instantiated version of this object. </returns>
        public static kAIINodeObject Load(kAIINodeSerialObject lSerialObject, kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
        {
            return lSerialObject.Instantiate(lAssemblyResolver);
        }
    }

    /// <summary>
    /// A constant integer value. 
    /// </summary>
    public class kAIConstantIntNode : kAIConstantNode
    {
        /// <summary>
        /// The serial object of a constant interger node. 
        /// </summary>
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            /// <summary>
            /// The value of the node. 
            /// </summary>
            [DataMember()]
            int Value;

            /// <summary>
            /// Creates a serialisation of a constant integer node. 
            /// </summary>
            /// <param name="lNode">The node object to serialise. </param>
            public SerialObject(kAIConstantIntNode lNode)
            {
                Value = lNode.Value;
            }

            /// <summary>
            /// A string representation of the node contents. 
            /// </summary>
            /// <returns>The value of the node.</returns>
            public string GetFriendlyName()
            {
                return Value.ToString();
            }

            /// <summary>
            /// Get the flavour of the node, in this case, a constant. 
            /// </summary>
            /// <returns>eNodeFlavour.Constant</returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.Constant;
            }

            /// <summary>
            /// Creates the constant node from this serialisation. 
            /// </summary>
            /// <param name="lAssemblyResolver">Used to resolve any types. </param>
            /// <returns>A constant int node. </returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return new kAIConstantIntNode(Value);
            }
        }

        kAIDataPort<int> mValuePort;

        int mValue;

        /// <summary>
        /// The current value. 
        /// </summary>
        public int Value
        {
            get 
            { 
                return mValue; 
            }
            set
            {
                mValue = value;
                mValuePort.Data = value;
            }
        }

        /// <summary>
        /// Create a constant int node with the specified value. 
        /// </summary>
        /// <param name="lValue">The value </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        public kAIConstantIntNode(int lValue, kAIILogger lLogger = null)
            : base(typeof(int), lLogger)
        {
            mValuePort = new kAIDataPort<int>("Value", kAIPort.ePortDirection.PortDirection_Out);
            mValue = lValue;
            AddExternalPort(mValuePort);
            mValuePort.Data = mValue;
        }

        /// <summary>
        /// Gets the serialised version of this node. 
        /// </summary>
        /// <param name="lOwningBehaviour">The XML behaviour this node is being instantiated into. </param>
        /// <returns>A <see cref="kAIConstantIntNode"/> represents by this object. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serial object. 
        /// </summary>
        /// <returns>typeof(SerialObject)</returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        } 
    }

    /// <summary>
    /// A constant float value. 
    /// </summary>
    public class kAIConstantFloatNode : kAIConstantNode
    {
        /// <summary>
        /// The serial object of a constant float node. 
        /// </summary>
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            /// <summary>
            /// The value of the node. 
            /// </summary>
            [DataMember()]
            float Value;

            /// <summary>
            /// Creates a serialisation of a constant float node. 
            /// </summary>
            /// <param name="lNode">The node object to serialise. </param>
            public SerialObject(kAIConstantFloatNode lNode)
            {
                Value = lNode.Value;
            }

            /// <summary>
            /// A string representation of the node contents. 
            /// </summary>
            /// <returns>The value of the node.</returns>
            public string GetFriendlyName()
            {
                return Value.ToString("0.0");
            }

            /// <summary>
            /// Get the flavour of the node, in this case, a constant. 
            /// </summary>
            /// <returns>eNodeFlavour.Constant</returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.Constant;
            }

            /// <summary>
            /// Creates the constant node from this serialisation. 
            /// </summary>
            /// <param name="lAssemblyResolver">Used to resolve any types. </param>
            /// <returns>A constant int node. </returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return new kAIConstantFloatNode(Value);
            }
        }

        kAIDataPort<float> mValuePort;

        float mValue;

        /// <summary>
        /// The current value. 
        /// </summary>
        public float Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                mValuePort.Data = value;
            }
        }

        /// <summary>
        /// Create a constant float node with the specified value. 
        /// </summary>
        /// <param name="lValue">The value </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        public kAIConstantFloatNode(float lValue, kAIILogger lLogger = null)
            : base(typeof(float), lLogger)
        {
            mValuePort = new kAIDataPort<float>("Value", kAIPort.ePortDirection.PortDirection_Out);
            mValue = lValue;
            AddExternalPort(mValuePort);
            mValuePort.Data = mValue;
        }

        /// <summary>
        /// Gets the serialised version of this node. 
        /// </summary>
        /// <param name="lOwningBehaviour">The XML behaviour this node is being instantiated into. </param>
        /// <returns>A <see cref="kAIConstantFloatNode"/> represented by this object. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serial object. 
        /// </summary>
        /// <returns>typeof(SerialObject)</returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }
    }

    /// <summary>
    /// A constant string value. 
    /// </summary>
    public class kAIConstantStringNode : kAIConstantNode
    {
        /// <summary>
        /// The serial object of a constant string node. 
        /// </summary>
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            /// <summary>
            /// The value of the node. 
            /// </summary>
            [DataMember()]
            string Value;

            /// <summary>
            /// Creates a serialisation of a constant float node. 
            /// </summary>
            /// <param name="lNode">The node object to serialise. </param>
            public SerialObject(kAIConstantStringNode lNode)
            {
                Value = lNode.Value;
            }

            /// <summary>
            /// A string representation of the node contents. 
            /// </summary>
            /// <returns>The value of the node.</returns>
            public string GetFriendlyName()
            {
                return Value;
            }

            /// <summary>
            /// Get the flavour of the node, in this case, a constant. 
            /// </summary>
            /// <returns>eNodeFlavour.Constant</returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.Constant;
            }

            /// <summary>
            /// Creates the constant node from this serialisation. 
            /// </summary>
            /// <param name="lAssemblyResolver">Used to resolve any types. </param>
            /// <returns>A constant int node. </returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return new kAIConstantStringNode(Value);
            }
        }

        kAIDataPort<string> mValuePort;

        string mValue;

        /// <summary>
        /// The current value. 
        /// </summary>
        public string Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                mValuePort.Data = value;
            }
        }

        /// <summary>
        /// Create a constant string node with the specified value. 
        /// </summary>
        /// <param name="lValue">The value </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        public kAIConstantStringNode(string lValue, kAIILogger lLogger = null)
            : base(typeof(string), lLogger)
        {
            mValuePort = new kAIDataPort<string>("Value", kAIPort.ePortDirection.PortDirection_Out);
            mValue = lValue;
            AddExternalPort(mValuePort);
            mValuePort.Data = mValue;
        }

        /// <summary>
        /// Gets the serialised version of this node. 
        /// </summary>
        /// <param name="lOwningBehaviour">The XML behaviour this node is being instantiated into. </param>
        /// <returns>A <see cref="kAIConstantStringNode"/> represents by this object. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serial object. 
        /// </summary>
        /// <returns>typeof(SerialObject)</returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }
    }

    /*
    class kAIConstant<T>  : kAIConstant where T : ISerializable
    {
        class InitalisationConfiguration
        {
            List<ConstructorInfo> mValidConstructors;
        }

        [DataContract()]
        class SerialObject : kAIINodeSerialObject
        {
            [DataMember()]
            public SerialType ConstantType;

            [DataMember()]
            public T ConstantValue;



            public SerialObject(kAIConstant<T> lConstantNode)
            {
                Type lT = typeof(T);

                ConstructorInfo[] lConstructors = lT.GetConstructors();
                List<ConstructorInfo> lValidConstructors = new List<ConstructorInfo>();
                foreach (ConstructorInfo lConstructor in lConstructors)
                {
                    bool lConstructorMatches = true;
                    foreach (ParameterInfo lParam in lConstructor.GetParameters())
                    {
                        if (!lParam.ParameterType.GetInterfaces().Contains(typeof(IConvertible)))
                        {
                            lConstructorMatches = false;
                            break;
                        }
                    }

                    if (lConstructorMatches)
                    {
                        lValidConstructors.Add(lConstructor);
                    }
                }
            }

            public string GetFriendlyName()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public eNodeFlavour GetNodeFlavour()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        T mValue;
        public T Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                mDataPort.Data = value;
            }
        }

        kAIDataPort<T> mDataPort;

        public kAIConstant(T lInitalValue, kAIILogger lLogger= null)
            :base(typeof(T), lLogger)
        {
            mDataPort = new kAIDataPort<T>("Value", kAIPort.ePortDirection.PortDirection_Out, lLogger);
            AddExternalPort(mDataPort);
        }

        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        public override string GetNameTemplate()
        {
            return "Constant " + typeof(T).Name;
        }

        public override void Update(float lDeltaTime, object lUserData)
        {
            // nothing to do. 
        }
    }*/
}
