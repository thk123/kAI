using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;
namespace kAI.Core
{

    public partial class kAIFunctionNode : kAINodeObject
    {
        /// <summary>
        /// Represents details about the function we are going to invoke
        /// e.g. the type of generic parametters. 
        /// </summary>
        public partial class kAIFunctionConfiguration
        {
            /// <summary>
            /// Represents the settings for how the FunctionNode should handle returns. 
            /// </summary>
            public partial class kAIReturnConfiguration
            {
                /// <summary>
                /// Serialisable version of the ReturnConfiguration.
                /// </summary>
                [DataContract()]
                public class SerialObject
                {
                    /// <summary>
                    /// For each of the properties in the ReturnConfiguration, we store if it is enabled or not. 
                    /// </summary>
                    [DataMember()]
                    public List<bool> PropertyEnabled;

                    /// <summary>
                    /// Create a serialised version of the return configuration. 
                    /// </summary>
                    /// <param name="lReturnConfig">The return configuration to serialise. </param>
                    public SerialObject(kAIReturnConfiguration lReturnConfig)
                    {
                        PropertyEnabled = lReturnConfig.mIsEnabled;
                    }

                    /// <summary>
                    /// Creates a ReturnConfiguration based on a serialised version of it. 
                    /// </summary>
                    /// <param name="lReturnType">The return type of the method, used for recreating the config. </param>
                    /// <param name="lAssemblyGetter">Method to resolve missing types. </param>
                    /// <returns></returns>
                    public kAIReturnConfiguration Instantiate(Type lReturnType, kAIXmlBehaviour.GetAssemblyByName lAssemblyGetter)
                    {
                        Assert(null, PropertyEnabled, "PropertyEnabled missing from save");
                        return new kAIReturnConfiguration(lReturnType, PropertyEnabled);
                    }
                }

                /// <summary>
                /// For each of the properties specified, are we enabled. 
                /// </summary>
                List<bool> mIsEnabled;

                /// <summary>
                /// The return type.
                /// </summary>
                public Type ReturnType
                {
                    get;
                    private set;
                }

                /// <summary>
                /// The set of properties this return type has.
                /// </summary>
                kAIIReturnConfigurationDictionary lPropertyDictionary;

                /// <summary>
                /// Get the number of properties asociated with this return type.
                /// </summary>
                public int PropertyCount
                {
                    get
                    {
                        return lPropertyDictionary.PropertyCount;
                    }
                }

                /// <summary>
                /// Get the ordered property names for this return. 
                /// </summary>
                public IEnumerable<string> PropertyNames
                {
                    get
                    {
                        return lPropertyDictionary.PropertyNames;
                    }
                }

                /// <summary>
                /// Is the return type configured. 
                /// </summary>
                public bool IsConfigured
                {
                    get
                    {
                        return !ReturnType.IsGenericParameter;
                    }
                }

                /// <summary>
                /// Triggered if the return is a generic type and has now been set to a concrete type.
                /// This means that potentially the return properties have changed. 
                /// </summary>
                public event EventHandler ReturnConfigurationChanged;

                /// <summary>
                /// Create an initial return configuration for a specified type. 
                /// </summary>
                /// <param name="lReturnType"></param>
                public kAIReturnConfiguration(Type lReturnType)
                {
                    SetReturnType(lReturnType);                   
                }

                /// <summary>
                /// Create a return configuration of a type and the status of each of the properties. 
                /// </summary>
                /// <param name="lReturnType">The type we are configuring. </param>
                /// <param name="lIsPropertyEnabled">The list of whether each property is enabled. </param>
                private kAIReturnConfiguration(Type lReturnType, List<bool> lIsPropertyEnabled)
                    : this(lReturnType)
                {
                    for (int lPropertyIndex = 0; lPropertyIndex < lIsPropertyEnabled.Count; ++lPropertyIndex)
                    {
                        SetPropertyState(lPropertyIndex, lIsPropertyEnabled[lPropertyIndex]);
                    }
                }

                /// <summary>
                /// Run all the post-function evaluation code enabled on this configuration. 
                /// </summary>
                /// <param name="lFunctionNode">The function node being evaluated. </param>
                /// <param name="lResult">The result of the function. </param>
                /// <param name="lPreviousResult">The previous value of the function.</param>
                public void RunCode(kAINodeObject lFunctionNode, object lResult, object lPreviousResult)
                {
                    for (int lPropertyIndex = 0; lPropertyIndex < mIsEnabled.Count; ++lPropertyIndex)
                    {
                        if (mIsEnabled[lPropertyIndex])
                        {
                            lPropertyDictionary.RunCode(lPropertyIndex, lFunctionNode, lResult, lPreviousResult);
                        }
                    }
                }

                /// <summary>
                /// Gets all the ports needed to be added for this return configuration. 
                /// </summary>
                /// <returns>All the ports that need adding. </returns>
                public IEnumerable<kAIPort> GetExternalPorts()
                {
                    for (int lPropertyIndex = 0; lPropertyIndex < mIsEnabled.Count; ++lPropertyIndex)
                    {
                        if (mIsEnabled[lPropertyIndex])
                        {
                            kAIPort lNewPort = lPropertyDictionary.GetExternalPort(lPropertyIndex);
                            if (lNewPort != null)
                            {
                                yield return lNewPort;
                            }
                        }
                    }
                }

                /// <summary>
                /// Set the specified property to be either enabled or disabled. 
                /// </summary>
                /// <param name="lPropertyIndex">The property index. </param>
                /// <param name="lValue">The new value of the property. </param>
                public void SetPropertyState(int lPropertyIndex, bool lValue)
                {
                    mIsEnabled[lPropertyIndex] = lValue;
                }

                /// <summary>
                /// Gets the current state of the property. 
                /// </summary>
                /// <param name="lPropertyIndex">The index of the property. </param>
                /// <returns></returns>
                public bool GetPropertyState(int lPropertyIndex)
                {
                    return mIsEnabled[lPropertyIndex];
                }

                /// <summary>
                /// Sets the return type to a concrete type. 
                /// </summary>
                /// <param name="lType">The concrete type. </param>
                public void SetReturnType(Type lType)
                {
                    ReturnType = lType;

                    // Need to reconfigure the return type
                    if (sReturnConfigs.ContainsKey(ReturnType.ToString()))
                    {
                        lPropertyDictionary = sReturnConfigs[ReturnType.ToString()];
                    }
                    else
                    {
                        lPropertyDictionary = new kAIDefaultReturnConfiguration(ReturnType);
                    }

                    mIsEnabled = new List<bool>(lPropertyDictionary.PropertyDefaults);

                    if (ReturnConfigurationChanged != null)
                    {
                        ReturnConfigurationChanged(this, new EventArgs());
                    }
                }
            }


            /// <summary>
            /// Represents a serialisable version of the a FunctionConfiguration. 
            /// </summary>
            [DataContract()]
            public class SerialObject
            {
                /// <summary>
                /// The ordered list of concrete types for each of the generic parameters. 
                /// </summary>
                [DataMember()]
                public List<SerialType> GenericConfiguration;

                /// <summary>
                /// The configuration for the return type. 
                /// </summary>
                [DataMember()]
                public kAIReturnConfiguration.SerialObject ReturnConfiguration;

                /// <summary>
                /// Is the first parameter of the function the pointer passed in the update. 
                /// </summary>
                [DataMember()]
                public bool IsFirstParameterSelf;

                /// <summary>
                /// Does the function have a port to execute it, or should it be executed every frame.
                /// </summary>
                [DataMember(IsRequired = false)]
                public bool IsFunctionExecutable;

                /// <summary>
                /// Create a Serialised version of the specified function configuration. 
                /// </summary>
                /// <param name="lConfig">The configuration to serailise. </param>
                public SerialObject(kAIFunctionConfiguration lConfig)
                {
                    Assert(null, lConfig.IsConfigured, "Attempting to save a non-configured function configuration");

                    GenericConfiguration = new List<SerialType>();

                    foreach (Type lType in lConfig.GenericTypes)
                    {
                        GenericConfiguration.Add(new SerialType(lType));
                    }

                    ReturnConfiguration = new kAIReturnConfiguration.SerialObject(lConfig.ReturnConfiguration);

                    IsFirstParameterSelf = lConfig.FirstParameterSelf;

                    IsFunctionExecutable = lConfig.ExectueTriggerPort;
                }

                /// <summary>
                /// Create a function configuration based on this serialsed configuration. 
                /// </summary>
                /// <param name="lFunction">The function we are creating a confiruration for. </param>
                /// <param name="lAssemblyResolver">For resolving assemblies. </param>
                /// <returns>The return configuration stored in this serial object. </returns>
                public kAIFunctionConfiguration Instantiate(MethodInfo lFunction, kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
                {
                    Assert(null, GenericConfiguration, "Generic Parameter configuration missing");
                    Assert(null, ReturnConfiguration, "Return Configuration missing");

                    return new kAIFunctionConfiguration(lFunction, GenericConfiguration.Select<SerialType, Type>((lType) =>
                        {
                            return lType.Instantiate(lAssemblyResolver);
                        }), IsFirstParameterSelf, IsFunctionExecutable, ReturnConfiguration, lAssemblyResolver);
                }
            }

            /// <summary>
            /// Triggered when the configuration is configured and ready to be used to
            /// make a function node. E.g. when all generic parameters have been specified. 
            /// </summary>
            public event EventHandler OnConfigured;

            /// <summary>
            /// This list of generic parmaters and they're assigned concrete type. 
            /// </summary>
            Type[] mGenericTypes;

            /// <summary>
            /// Stores mapping for each generic parameter to the set of parameters the type corresponds to
            /// eg mGenericMappings[1] = {0, 2} on SomeFunctionT,U means the first and third parameters are 
            /// of type U. 
            /// </summary>
            List<int>[] mGenericMappings;

            /// <summary>
            /// Is the first parameter of the function the pointer passed in the update. 
            /// </summary>
            public bool FirstParameterSelf
            {
                get;
                set;
            }

            /// <summary>
            /// Should there be a trigger port that dictates when the function should be executed.
            /// If true, the function will only be run on triggered
            /// Otherwise, the function will be recomputed each frame.
            /// Note: if your function requires access to the object passed in in the update,
            /// this will only be avaliable after one update. 
            /// </summary>
            public bool ExectueTriggerPort
            {
                get;
                set;
            }
            
            /// <summary>
            /// The return configuration of this function configuration. 
            /// </summary>
            public kAIReturnConfiguration ReturnConfiguration
            {
                get;
                private set;
            }

            /// <summary>
            /// The generic parameters (if configured, what concrete types they are set to). 
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
            /// The type for each of the parameters of the method. If this configuration is
            /// configured, all of these will be actual types. 
            /// </summary>
            Type[] ParameterTypes
            {
                get;
                set;
            }

            /// <summary>
            /// Returns true if all the parameter types have concrete types. 
            /// </summary>
            public bool IsConfigured
            {
                get
                {
                    return ReturnConfiguration.IsConfigured &&
                        ParameterTypes.All((lType) => { return !lType.IsGenericParameter; }); ;
                }
            }

            /// <summary>
            /// Create a function configuration for a specific function. 
            /// </summary>
            /// <param name="lMethod">The function to create a corresponding configuration for. </param>
            public kAIFunctionConfiguration(MethodInfo lMethod)
            {
                // If we are a generic method
                if (lMethod.IsGenericMethod)
                {
                    // We store each of the generic arguments
                    mGenericTypes = lMethod.GetGenericArguments();
                }
                else
                {
                    mGenericTypes = Type.EmptyTypes;
                }

                // Create a mapping from each of the generic arguments to all the parameters that correspond to
                mGenericMappings = new List<int>[mGenericTypes.Length];
                for (int i = 0; i < mGenericTypes.Length; ++i)
                {
                    mGenericMappings[i] = new List<int>();
                }

                // Store each of the parameter types for this configuration
                ParameterInfo[] lParams = lMethod.GetParameters();
                ParameterTypes = new Type[lParams.Length];

                for (int lParamIndx = 0; lParamIndx < lParams.Length; ++lParamIndx)
                {
                    ParameterTypes[lParamIndx] = lParams[lParamIndx].ParameterType;

                    // If this parameter is generic
                    if (lParams[lParamIndx].ParameterType.IsGenericParameter)
                    {
                        // We look it up in the generic parameter list 
                        int lGenericMatch = mGenericTypes.ToList().FindIndex((lType) => { return lType.Name == lParams[lParamIndx].ParameterType.Name; });

                        // and add this parameter index to the set of parameters the generic parameter corresponds to. 
                        mGenericMappings[lGenericMatch].Add(lParamIndx);
                    }
                }

                Type lReturnType = lMethod.ReturnType;
                if (lReturnType.ContainsGenericParameters)
                {
                    int lGenericMatch = mGenericTypes.ToList().FindIndex((lType) => { return lType.Name == lReturnType.Name; });
                    mGenericMappings[lGenericMatch].Add(-1);
                }

                ReturnConfiguration = new kAIReturnConfiguration(lMethod.ReturnType);

                FirstParameterSelf = false;
                ExectueTriggerPort = false;

                if (IsConfigured && OnConfigured != null)
                {
                    OnConfigured(this, new EventArgs());
                }
            }

            private kAIFunctionConfiguration(MethodInfo lMethod, IEnumerable<Type> lConfiguredTypes, bool lIsFirstParamSelf, bool lIsExecutionPort, kAIReturnConfiguration.SerialObject lReturnConfig, kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
                : this(lMethod)
            {
                int lGenParamIndex = 0;
                foreach (Type lType in lConfiguredTypes)
                {
                    SetGenericParameter(lGenParamIndex, lType);
                    ++lGenParamIndex;
                }

                ReturnConfiguration = lReturnConfig.Instantiate(ReturnConfiguration.ReturnType, lAssemblyResolver);

                FirstParameterSelf = lIsFirstParamSelf;
                ExectueTriggerPort = lIsExecutionPort;
                Assert(null, IsConfigured, "Loaded an in-complete configuration");
            }

            /// <summary>
            /// Set a concrete type for a specific generic parameter. 
            /// </summary>
            /// <param name="lGenericParametricIndex">The index of the generic parameter. </param>
            /// <param name="lValue">The concrete type to set this parameter to. </param>
            public void SetGenericParameter(int lGenericParametricIndex, Type lValue)
            {
                foreach (int lParamIndex in mGenericMappings[lGenericParametricIndex])
                {
                    // This is the generic parameter corresponding to the return type
                    if (lParamIndex == -1)
                    {
                        ReturnConfiguration.SetReturnType(lValue);
                    }
                    else
                    {
                        ParameterTypes[lParamIndex] = lValue;
                    }
                }

                mGenericTypes[lGenericParametricIndex] = lValue;

                if (IsConfigured && OnConfigured != null)
                {
                    OnConfigured(this, new EventArgs());
                }

            }

            /// <summary>
            /// Either enable or disable a specific property of the return configuration.
            /// </summary>
            /// <param name="lPropertyIndex">The index of the property. </param>
            /// <param name="lValue">Should the property be enabled or disabled. </param>
            public void SetReturnProperty(int lPropertyIndex, bool lValue)
            {
                ReturnConfiguration.SetPropertyState(lPropertyIndex, lValue);
            }


            /// <summary>
            /// Get the concrete type for a specific parameter. 
            /// </summary>
            /// <param name="lParamIndex">The index of the parameter in the method. </param>
            /// <returns>
            /// The type of the parameter. 
            /// This will be an actual type providing the configuration is configured. 
            /// </returns>
            public Type GetConcreteType(int lParamIndex)
            {
                return ParameterTypes[lParamIndex];
            }
        }
    }

}
