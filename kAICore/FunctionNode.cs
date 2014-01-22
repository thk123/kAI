using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;
using MiscUtil;
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
            : base(lLogger)
        {
            Assert(lConfig.IsConfigured, "Need a fully configured configuration to create a function node. ");


            lInParameters = new List<kAIDataPort>();
            lOutParameters = new List<kAIDataPort>();
            int lParamIndex = 0;
            foreach (ParameterInfo lParam in lBaseMethod.GetParameters())
            {
                if (lParamIndex == 0 && lConfig.FirstParameterSelf)
                {
                    ++lParamIndex;
                    continue;
                }

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
            if (lBaseMethod.IsGenericMethod)
            {
                mMethod = lBaseMethod.MakeGenericMethod(lConfig.GenericTypes);
            }
            else
            {
                mMethod = lBaseMethod;
            }
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
            int lParamCount = lInParameters.Count + lOutParameters.Count;
            if (mConfig.FirstParameterSelf)
            {
                ++lParamCount;
            }

            object[] lParams = new object[lParamCount];

            int i = 0;

            if (mConfig.FirstParameterSelf)
            {
                lParams[i] = lUserData;
                ++i;
            }

            foreach (kAIDataPort lInParamPort in lInParameters)
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
        public static bool IfEquals<T>(T entry1, T entry2)
        {
            return (entry1 == null && entry2 == null) || entry1.Equals(entry2);
        }

        [StaticConstraint(StaticConstraint.StaticConstraintType.eConstraint_LessThan | StaticConstraint.StaticConstraintType.eConstraint_GreaterThan)]
        public static bool IfLessThan<T>(T entry1, T entry2)
        {
            return Operator.LessThan<T>(entry1, entry2);
        } 

        [StaticConstraint(StaticConstraint.StaticConstraintType.eConstraint_LessThan | StaticConstraint.StaticConstraintType.eConstraint_GreaterThan)]
        public static bool IfGreaterThan<T>(T entry1, T entry2)
        {
            return Operator.GreaterThan<T>(entry1, entry2);
        }
         
        [StaticConstraint(StaticConstraint.StaticConstraintType.eConstraint_LessThan | StaticConstraint.StaticConstraintType.eConstraint_GreaterThan)]
        public static bool IfLessThanOrEqual<T>(T entry1, T entry2)
        {  
            return Operator.LessThanOrEqual<T>(entry1, entry2);
        }

        [StaticConstraint(StaticConstraint.StaticConstraintType.eConstraint_LessThan | StaticConstraint.StaticConstraintType.eConstraint_GreaterThan)]
        public static bool IfGreaterThanOrEqual<T>(T entry1, T entry2)
        {
            return Operator.GreaterThanOrEqual<T>(entry1, entry2);
        }

        [StaticConstraint(StaticConstraint.StaticConstraintType.eConstraint_Plus)]
        public static T Sum<T>(T entry1, T entry2)
        {
            return Operator.Add<T>(entry1, entry2);
        }

        public static float Abs(float lValue)
        {
            return Math.Abs(lValue);
        }
    }

    /// <summary>
    /// Use to apply static constraints on to a type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StaticConstraint : Attribute
    {
        /// <summary>
        /// For each generic parameter, the constraint being applied to it. 
        /// </summary>
        public StaticConstraintType[] Constraints
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MethodDescription
        {
            /// <summary>
            /// The name of the method
            /// </summary>
            string mMethodName;

            /// <summary>
            /// The types of each of the parameters. 
            /// </summary>
            List<Type> mParameters;

            /// <summary>
            /// The type of the return
            /// </summary>
            Type mReturnType;

            /// <summary>
            /// The set of types which match this method description but not in the standard way
            /// E.g. primitves don't have a method op_Addition but can still be added. 
            /// </summary>
            List<Type> mSpecialCases;


            /// <summary>
            /// Does a given type have the static corresponding method. 
            /// </summary>
            /// <param name="lType">The type to check. </param>
            /// <returns>True if the type implements the relevant method, false otherwise. </returns>
            public bool DoesTypeHaveMethodMatch(Type lType)
            {
                if (mSpecialCases.Contains(lType))
                {
                    return true;
                }

                MethodInfo lMethod = lType.GetMethod(mMethodName);

                if (lMethod == null)
                {
                    return false;
                }

                if (!lMethod.ReturnType.Equals(mReturnType))
                {
                    return false;
                }

                ParameterInfo[] lParams = lMethod.GetParameters();

                IEnumerable<Type> lParamTypes = lParams.Select<ParameterInfo, Type>((lParam) => { return lParam.ParameterType; });

                if (!lParamTypes.DoMatch(mParameters))
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Create a method description for operator+
            /// </summary>
            /// <param name="lType">The type of things to add. </param>
            /// <returns>A method description for the lType+lType. </returns>
            public static MethodDescription[] PlusOperator(Type lType)
            {
                MethodDescription lPlusOp = new MethodDescription();
                lPlusOp.mMethodName = "op_Addition";
                lPlusOp.mParameters = new List<Type>(new Type[] { lType, lType });
                lPlusOp.mReturnType = lType;

                lPlusOp.mSpecialCases = new List<Type>();
                lPlusOp.mSpecialCases.AddRange(GetPrimitiveTypes());
                return new[] { lPlusOp };
            }

            /// <summary>
            /// Create a method description for operator-
            /// </summary>
            /// <param name="lType">The type of things to add. </param>
            /// <returns>A method description for the lType+lType. </returns>
            public static MethodDescription[] MinusOperator(Type lType)
            {
                MethodDescription lSubOp = new MethodDescription();
                lSubOp.mMethodName = "op_Subtraction";
                lSubOp.mParameters = new List<Type>(new Type[] { lType, lType });
                lSubOp.mReturnType = lType;

                lSubOp.mSpecialCases = new List<Type>();
                lSubOp.mSpecialCases.AddRange(GetNumericalPrimitives());
                return new[] { lSubOp };
            }

            /// <summary>
            /// Create a method description for operator less than 
            /// </summary>
            /// <param name="lType">The type of things to compare. </param>
            /// <returns>A method description describing a less than operator</returns>
            public static MethodDescription[] LessThanOperator(Type lType)
            {
                MethodDescription lLessThanOp = new MethodDescription();
                lLessThanOp.mMethodName = "op_LessThan";
                lLessThanOp.mParameters = new List<Type>(new Type[] { lType, lType });
                lLessThanOp.mReturnType = lType;

                lLessThanOp.mSpecialCases = new List<Type>();
                lLessThanOp.mSpecialCases.AddRange(GetNumericalPrimitives());


                return new[] { lLessThanOp };
            }

            /// <summary>
            /// Create a method description for operator less than 
            /// </summary>
            /// <param name="lType">The type of things to compare. </param>
            /// <returns>A method description describing a less than operator</returns>
            public static MethodDescription[] GreaterThanOperator(Type lType)
            {
                MethodDescription lGreaterThanOp = new MethodDescription();
                lGreaterThanOp.mMethodName = "op_GreaterThan";
                lGreaterThanOp.mParameters = new List<Type>(new Type[] { lType, lType });
                lGreaterThanOp.mReturnType = lType;

                lGreaterThanOp.mSpecialCases = new List<Type>();
                lGreaterThanOp.mSpecialCases.AddRange(GetNumericalPrimitives());

                return new[] { lGreaterThanOp };
            }

            /// <summary>
            /// All the primitives. 
            /// </summary>
            /// <returns>All the primitve data types that can be added etc. </returns>
            static IEnumerable<Type> GetPrimitiveTypes()
            {
                return GetNumericalPrimitives().Concat(GetNonNumericalPrimitves());
            }

            static IEnumerable<Type> GetNonNumericalPrimitves()
            {
                yield return typeof(char);
                yield return typeof(string);
            }

            static IEnumerable<Type> GetNumericalPrimitives()
            {
                yield return typeof(int);
                yield return typeof(float);
                yield return typeof(long);
                yield return typeof(uint);
                yield return typeof(double);
                yield return typeof(byte);
                yield return typeof(short);
                yield return typeof(ushort);
            }
        }

        /// <summary>
        /// The types of static constraints
        /// </summary>
        public enum StaticConstraintType
        {
            /// <summary>
            /// Should have operator+
            /// </summary>
            eConstraint_Plus = 1,
            /// <summary> 
            /// Should have operator-
            /// </summary>
            eConstraint_Minus = 1 << 1,
            /// <summary>
            /// Should have less than 
            /// </summary>
            eConstraint_LessThan = 1 << 2,

            /// <summary>
            /// Should have greater than
            /// </summary>
            eConstraint_GreaterThan = 1 << 3,
        }

        /// <summary>
        /// Attribute for specifying that the parameter must implement a certain static method. 
        /// </summary>
        /// <param name="lConstraint">The constraint. </param>
        public StaticConstraint(params StaticConstraintType[] lConstraint)
        {
            Constraints = lConstraint;
        }

        /// <summary>
        /// Checks whether a given type matches the static constraints on the corresponding generic parameter. 
        /// </summary>
        /// <param name="lGenericParamIndex">The index of the generic parameter. </param>
        /// <param name="lParameterType">The type we are checking to see if it matches. </param>
        /// <returns>True if the type is a suitable type for the specified generic parameter. </returns>
        public bool MatchesConstraint(int lGenericParamIndex, Type lParameterType)
        {
            StaticConstraintType lTypeFlags = Constraints[lGenericParamIndex];

            StaticConstraintType[] lActualConstraints = lTypeFlags.GetAsFlags();

            List<MethodDescription> lMethodDescriptions = new List<MethodDescription>(lActualConstraints.Length);

            foreach (StaticConstraintType lActualConstraint in lActualConstraints)
            {
                MethodDescription[] lDescription = sStaticMethods[lActualConstraint](lParameterType);
                lMethodDescriptions.AddRange(lDescription);
            }

            return lMethodDescriptions.All((lMethodDesc) => { return lMethodDesc.DoesTypeHaveMethodMatch(lParameterType); });
        }


        // Create the dictionary of MethodDescriptions for each constraint type. 
        static Dictionary<StaticConstraintType, Func<Type, MethodDescription[]>> sStaticMethods;

        static StaticConstraint()
        {
            sStaticMethods = new Dictionary<StaticConstraintType, Func<Type, MethodDescription[]>>();
            sStaticMethods.Add(StaticConstraintType.eConstraint_Plus, MethodDescription.PlusOperator);
            sStaticMethods.Add(StaticConstraintType.eConstraint_Minus, MethodDescription.MinusOperator);
            sStaticMethods.Add(StaticConstraintType.eConstraint_LessThan, MethodDescription.LessThanOperator);
            sStaticMethods.Add(StaticConstraintType.eConstraint_GreaterThan, MethodDescription.GreaterThanOperator);
        }


    }
}
