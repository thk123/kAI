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
    public partial class kAIFunctionNode : kAINodeObject
    {
        

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
            /// The configuration of the function. 
            /// </summary>
            [DataMember()]
            public kAIFunctionConfiguration.SerialObject FunctionConfiguraion;

            /// <summary>
            /// Create a serial representation of the specified function node. 
            /// </summary>
            /// <param name="lNode">The function node to serialise. </param>
            public SerialObject(kAIFunctionNode lNode)
            {
                MethodName = lNode.mMethod.Name;
                TypeName = lNode.mMethod.DeclaringType.FullName;
                AssemblyName = lNode.mMethod.DeclaringType.Assembly.GetName().Name;
                FunctionConfiguraion = new kAIFunctionConfiguration.SerialObject(lNode.mConfig);
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
                Assert(null, AssemblyName, "ASsemblyName not present...");
                Assembly lFunctionAssembly = lAssemblyResolver(AssemblyName);
                Assert(null, lFunctionAssembly, "Could not find assembly: " + AssemblyName);
                
                Type lDeclType = lFunctionAssembly.GetType(TypeName);
                Assert(null, lDeclType, "Could not find type: " + TypeName);

                MethodInfo lMethod = lDeclType.GetMethod(MethodName);
                Assert(null, lMethod, "Could not find method: " + MethodName);

                Assert(null, FunctionConfiguraion, "Function configuration missing from file");
                kAIFunctionConfiguration lConfig = FunctionConfiguraion.Instantiate(lMethod, lAssemblyResolver);

                return new kAIFunctionNode(lMethod, lConfig);
            }
        }

        List<kAIDataPort> lInParameters;
        kAIDataPort lOperandPort;
        List<kAIDataPort> lOutParameters;

        object lLastResult;

        MethodInfo mMethod;
        kAIFunctionConfiguration mConfig;
        /// <summary>
        /// Create a function node representing a specific function. 
        /// </summary>
        /// <param name="lBaseMethod">The method to base this function node on. </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        /// <param name="lConfig"></param>
        public kAIFunctionNode(MethodInfo lBaseMethod, kAIFunctionConfiguration lConfig, kAIILogger lLogger = null)
            :base(lLogger)
        {
            Assert(lConfig.IsConfigured, "Need a fully configured configuration to create a function node. ");
            

            lInParameters = new List<kAIDataPort>();
            lOutParameters = new List<kAIDataPort>();
            int lParamIndex = 0;
            foreach (ParameterInfo lParam in lBaseMethod.GetParameters())
            {
                if (lParam.IsOut || lParam.ParameterType.IsByRef)
                {
                    throw new NotImplementedException("Methods cannot currently have out or ref parameters");
                }
                else
                {
                    kAIDataPort lInParam = kAIDataPort.CreateDataPort(lConfig.GetConcreteType(lParamIndex), lParam.Name, kAIPort.ePortDirection.PortDirection_In);
                    lInParameters.Add(lInParam);
                    AddExternalPort(lInParam);

                    ++lParamIndex;
                }
            }

            if (lBaseMethod.ReturnType != null)
            {
                foreach (kAIPort lAdditionalReturnPort in lConfig.ReturnConfiguration.GetExternalPorts())
                {
                    AddExternalPort(lAdditionalReturnPort);
                }
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
            if (lLastResult == null)
            {
                lLastResult = mMethod.ReturnType.GetDefault();
            }

            object lReturnObject = mMethod.Invoke(lOperand, lParams);

            foreach (kAIDataPort lOutParamPort in lOutParameters)
            {
                lOutParamPort.SetData(lParams[i]);
                ++i;
            }

            mConfig.ReturnConfiguration.RunCode(this, lReturnObject, lLastResult);

            lLastResult = lReturnObject;
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

    /// <summary>
    /// Built-in functions for Function nodes. 
    /// </summary>
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
