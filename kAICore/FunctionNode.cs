using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;
namespace kAI.Core
{
    /// <summary>
    /// A node that operates on a function. 
    /// </summary>
    public class kAIFunctionNode : kAINodeObject
    {
        /// <summary>
        /// 
        /// </summary>
        public class FunctionConfiguration
        {
            Type[] mGenericTypes;
            List<int>[] mGenericMappings;

            /// <summary>
            /// 
            /// </summary>
            public Type[] GenericTypes
            {
                get 
                {
                    return mGenericTypes;
                }
                set
                {
                    for (int i = 0; i < value.Length; ++i)
                    {
                        SetGenericParameter(i, value[i]);
                    }

                    mGenericTypes = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <param name="lValue"></param>
            public void SetGenericParameter(int index, Type lValue)
            {
                foreach (int lParamIndex in mGenericMappings[index])
                {
                    ParameterTypes[lParamIndex] = lValue;
                }

                mGenericTypes[index] = lValue;

                if (IsConfigured && OnConfigured != null)
                {
                    OnConfigured(this, new EventArgs());
                }

            }


            Type[] ParameterTypes
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public bool IsConfigured
            {
                get
                {
                    return ParameterTypes.All((lType) => { return !lType.IsGenericParameter; });;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public event EventHandler OnConfigured;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lMethod"></param>
            public FunctionConfiguration(MethodInfo lMethod)
            {
                if (lMethod.IsGenericMethod)
                {
                    mGenericTypes = lMethod.GetGenericArguments();
                    mGenericMappings = new List<int>[mGenericTypes.Length];
                    for (int i = 0; i < mGenericTypes.Length; ++i)
                    {
                        mGenericMappings[i] = new List<int>();
                    }

                }

                ParameterInfo[] lParams = lMethod.GetParameters();

                ParameterTypes = new Type[lParams.Length];

                for (int lParamIndx = 0; lParamIndx < lParams.Length; ++lParamIndx )
                {
                    ParameterTypes[lParamIndx] = lParams[lParamIndx].ParameterType;
                    if (lParams[lParamIndx].ParameterType.IsGenericParameter)
                    {
                        int lGenericMatch = mGenericTypes.ToList().FindIndex((lType) => { return lType.Name == lParams[lParamIndx].ParameterType.Name; });
                        mGenericMappings[lGenericMatch].Add(lParamIndx);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="lParamIndex"></param>
            /// <returns></returns>
            public Type GetConcreteType(int lParamIndex)
            {
                return ParameterTypes[lParamIndex];
            }
        }

        /// <summary>
        /// Represents the serial version of a function node for saving out. 
        /// </summary>
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            /// <summary>
            /// The name of the method this function node corresponds to. 
            /// </summary>
            [DataMember()]
            public string MethodName;

            /// <summary>
            /// The decleraing type of the function. 
            /// </summary>
            [DataMember()]
            public string TypeName;

            /// <summary>
            /// The assembly of the declaring type of the function.
            /// </summary>
            [DataMember()]
            public string AssemblyName;

            /// <summary>
            /// The concrete types for each of the generic parameters;
            /// </summary>
            [DataMember()]
            public List<SerialType> GenericConfiguration;

            /// <summary>
            /// Create a serial representation of the specified function node. 
            /// </summary>
            /// <param name="lNode">The function node to serialise. </param>
            public SerialObject(kAIFunctionNode lNode)
            {
                MethodName = lNode.mMethod.Name;
                TypeName = lNode.mMethod.DeclaringType.FullName;
                AssemblyName = lNode.mMethod.DeclaringType.Assembly.GetName().Name;

                GenericConfiguration = new List<SerialType>();

                foreach (Type lType in lNode.mConfig.GenericTypes)
                {
                    GenericConfiguration.Add(new SerialType(lType));
                }
            }

            /// <summary>
            /// Gets the name of this serial object. 
            /// </summary>
            /// <returns>The method name this function represents. </returns>
            public string GetFriendlyName()
            {
                return MethodName;
            }

            /// <summary>
            /// Gets the type of this node. 
            /// </summary>
            /// <returns>The type -- a function node. </returns>
            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.Function;
            }

            /// <summary>
            /// Create the coresponding node object for this serial object. 
            /// </summary>
            /// <param name="lAssemblyResolver">The method to use to resolve assembly names to get types. </param>
            /// <returns>An instance of a function node object this serial object represents.</returns>
            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                Assembly lFunctionAssembly = lAssemblyResolver(AssemblyName);
                Type lDeclType = lFunctionAssembly.GetType(TypeName);
                MethodInfo lMethod = lDeclType.GetMethod(MethodName);

                FunctionConfiguration lConfig = new FunctionConfiguration(lMethod);
                for(int i =0; i < GenericConfiguration.Count; ++i)
                {
                    lConfig.SetGenericParameter(i, GenericConfiguration[i].Instantiate(lAssemblyResolver));
                }
                

                return new kAIFunctionNode(lMethod, lConfig);
            }
        }

        List<kAIDataPort> lInParameters;
        kAIDataPort lReturnPort;
        kAITriggerPort lOnTruePort;
        kAIDataPort lOperandPort;
        List<kAIDataPort> lOutParameters;

        MethodInfo mMethod;
        FunctionConfiguration mConfig;
        /// <summary>
        /// Create a function node representing a specific function. 
        /// </summary>
        /// <param name="lBaseMethod">The method to base this function node on. </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        /// <param name="lConfig"></param>
        public kAIFunctionNode(MethodInfo lBaseMethod, FunctionConfiguration lConfig, kAIILogger lLogger = null)
            :base(lLogger)
        {
            Assert(lConfig.IsConfigured);

            lInParameters = new List<kAIDataPort>();
            lOutParameters = new List<kAIDataPort>();
            int i = 0;
            foreach (ParameterInfo lParam in lBaseMethod.GetParameters())
            {
                if (lParam.IsOut)
                {
                    kAIDataPort lOutParam = kAIDataPort.CreateDataPort(lParam.ParameterType, lParam.Name, kAIPort.ePortDirection.PortDirection_Out);
                    lOutParameters.Add(lOutParam);
                    AddExternalPort(lOutParam);
                }
                else
                {
                    kAIDataPort lInParam = kAIDataPort.CreateDataPort(lConfig.GetConcreteType(i), lParam.Name, kAIPort.ePortDirection.PortDirection_In);
                    lInParameters.Add(lInParam);
                    AddExternalPort(lInParam);

                    ++i;
                }

                
            }

            if (lBaseMethod.ReturnType != null)
            {
                if (lBaseMethod.ReturnType == typeof(bool))
                {
                    lOnTruePort = new kAITriggerPort("OnTrue", kAIPort.ePortDirection.PortDirection_Out);
                    AddExternalPort(lOnTruePort);
                }
                lReturnPort = kAIDataPort.CreateDataPort(lBaseMethod.ReturnType, lBaseMethod.Name + "_Return", kAIPort.ePortDirection.PortDirection_Out);
                
                AddExternalPort(lReturnPort);
            }

            if (!lBaseMethod.IsStatic)
            {
                lOperandPort = kAIDataPort.CreateDataPort(lBaseMethod.DeclaringType, "Executor", kAIPort.ePortDirection.PortDirection_In);
                AddExternalPort(lOperandPort);
            }

            mMethod = lBaseMethod.MakeGenericMethod(lConfig.GenericTypes);
            mConfig = lConfig;
        }

        /// <summary>
        /// Gets the serial object of this node for serialising. 
        /// </summary>
        /// <param name="lOwningBehaviour">The XML behaviour this node belongs to. </param>
        /// <returns>A <see cref="kAIFunctionNode.SerialObject"/> representing this node. </returns>
        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        /// <summary>
        /// Gets the type of the serial object. 
        /// </summary>
        /// <returns>The type of the serial object.</returns>
        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        /// <summary>
        /// Gets a default name for this node object. 
        /// </summary>
        /// <returns>The method name. </returns>
        public override string GetNameTemplate()
        {
            return mMethod.Name;
        }

        /// <summary>
        /// Causes the function to be reevaluated. 
        /// </summary>
        /// <param name="lDeltaTime">The time since last update. </param>
        /// <param name="lUserData">The user data. </param>
        public override void Update(float lDeltaTime, object lUserData)
        {
            object[] lParams = new object[lInParameters.Count + lOutParameters.Count];

            int i = 0;
            foreach(kAIDataPort lInParamPort in lInParameters)
            {
                lParams[i] = lInParamPort.GetData();
                ++i;
            }

            object lOperand = null;
            if (lOperandPort != null)
            {
                lOperand = lOperandPort.GetData();
            }

            object lReturnObject = mMethod.Invoke(lOperand, lParams);

            foreach (kAIDataPort lOutParamPort in lOutParameters)
            {
                lOutParamPort.SetData(lParams[i]);
                ++i;
            }
            
            lReturnPort.SetData(lReturnObject);

            if (lOnTruePort != null)
            {
                if ((bool)lReturnObject)
                {
                    lOnTruePort.Trigger();
                }
            }
        }

        /// <summary>
        /// Load the node object from a serial object representing it. 
        /// </summary>
        /// <param name="lSerialObject">The serial object representing the node. </param>
        /// <param name="lAssemblyResolver">The method to use to resolve missing assemblies. </param>
        /// <returns>An instance of a kAIFunctionNode based off this serial object. </returns>
        public static kAIINodeObject Load(SerialObject lSerialObject, kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
        {
            return lSerialObject.Instantiate(lAssemblyResolver);
        }
    }

    static class kAIFunctionNodes
    {
        public static bool IfEquals<T>(T lA, T lB)
        {
            return (lA == null && lB == null ) || lA.Equals(lB);
        }

        public static string Print<T, U>(T lOne, U lTwo)
        {
            return lOne.ToString() + lTwo.ToString();
        }
    }
}
