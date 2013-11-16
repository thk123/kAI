using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace kAI.Core
{
    /// <summary>
    /// A node that operates on a function. 
    /// </summary>
    public class kAIFunctionNode : kAINodeObject
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
            /// Create a serial representation of the specified function node. 
            /// </summary>
            /// <param name="lNode">The function node to serialise. </param>
            public SerialObject(kAIFunctionNode lNode)
            {
                MethodName = lNode.mMethod.Name;
                TypeName = lNode.mMethod.DeclaringType.FullName;
                AssemblyName = lNode.mMethod.DeclaringType.Assembly.GetName().Name;
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

                return new kAIFunctionNode(lMethod);
            }
        }

        List<kAIDataPort> lInParameters;
        kAIDataPort lReturnPort;
        kAITriggerPort lOnTruePort;
        kAIDataPort lOperandPort;
        List<kAIDataPort> lOutParameters;

        MethodInfo mMethod;

        /// <summary>
        /// Create a function node representing a specific function. 
        /// </summary>
        /// <param name="lBaseMethod">The method to base this function node on. </param>
        /// <param name="lLogger">Optionally, the logger this node should use. </param>
        public kAIFunctionNode(MethodInfo lBaseMethod, kAIILogger lLogger = null)
            :base(lLogger)
        {
            lInParameters = new List<kAIDataPort>();
            lOutParameters = new List<kAIDataPort>();
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
                    kAIDataPort lInParam = kAIDataPort.CreateDataPort(lParam.ParameterType, lParam.Name, kAIPort.ePortDirection.PortDirection_In);
                    lInParameters.Add(lInParam);
                    AddExternalPort(lInParam);
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

            mMethod = lBaseMethod;
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
        public static bool IfEquals(int lA, int lB)
        {
            return lA == lB;
        }
    }
}
