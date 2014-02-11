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
    public partial class kAIFunctionNode : kAINodeObject
    {
        public partial class kAIFunctionConfiguration
        {
            public partial class kAIReturnConfiguration
            {
                /// <summary>
                /// Create the dictionary for use with the return configuration. 
                /// </summary>
                static kAIReturnConfiguration()
                {
                    kAIReturnConfigurationDictionary.ConfigureDictionary();
                }

                /// <summary>
                /// Represents a list of properties associated with a specific return type. 
                /// </summary>
                interface kAIIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// The number of properties.
                    /// </summary>
                    int PropertyCount
                    {
                        get;
                    }

                    /// <summary>
                    /// The names of all the properties.
                    /// </summary>
                    IEnumerable<string> PropertyNames
                    {
                        get;
                    }

                    /// <summary>
                    /// Should the property be enabled by default. 
                    /// </summary>
                    IEnumerable<bool> PropertyDefaults
                    {
                        get;
                    }

                    /// <summary>
                    /// Get the port for a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <returns>An instance of the port we need to add for this property. </returns>
                    kAIPort GetExternalPort(int lPropertyIndex);

                    /// <summary>
                    /// Runs the code associated with a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <param name="lFunctionNode">The FunctionNode being evaluated. </param>
                    /// <param name="lResult">The result of the function. </param>
                    /// <param name="lPreviousResult">The previous result of the function. </param>
                    void RunCode(int lPropertyIndex, kAINodeObject lFunctionNode, object lResult, object lPreviousResult);
                }

                /// <summary>
                /// The default property set for all types. 
                /// </summary>
                [Serializable]
                class kAIDefaultReturnConfiguration : kAIIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// The number of properties.
                    /// </summary>
                    public int PropertyCount
                    {
                        get
                        {
                            return 1;
                        }
                    }


                    /// <summary>
                    /// The names of all the properties.
                    /// </summary>
                    public IEnumerable<string> PropertyNames
                    {
                        get
                        {
                            yield return "Return Port";
                        }
                    }

                    /// <summary>
                    /// Should the property be enabled by default. 
                    /// </summary>
                    public IEnumerable<bool> PropertyDefaults
                    {
                        get
                        {
                            yield return true;
                        }
                    }

                    /// <summary>
                    /// The type this instance corresponds to. 
                    /// </summary>
                    Type mType;

                    /// <summary>
                    /// Create the default return configuration for a specic type -
                    /// just a data port returning the value;
                    /// </summary>
                    /// <param name="lType">The type of the return. </param>
                    public kAIDefaultReturnConfiguration(Type lType)
                    {
                        mType = lType;
                    }

                    /// <summary>
                    /// Get the port for a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <returns>An instance of the port we need to add for this property. </returns>
                    public kAIPort GetExternalPort(int lPropertyIndex)
                    {
                        if (lPropertyIndex == 0) // Property: Data Return
                        {
                            return kAIDataPort.CreateDataPort(mType, "DataReturn", kAIPort.ePortDirection.PortDirection_Out);
                        }
                        else
                        {
                            throw new Exception("No property at index " + lPropertyIndex);
                        }
                    }

                    /// <summary>
                    /// Runs the code associated with a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <param name="lFunctionNode">The FunctionNode being evaluated. </param>
                    /// <param name="lResult">The result of the function. </param>
                    /// <param name="lPreviousResult">The previous result of the function. </param>
                    public void RunCode(int lPropertyIndex, kAINodeObject lFunctionNode, object lResult, object lPreviousResult)
                    {
                        if (lPropertyIndex == 0)
                        {
                            kAIDataPort lDataPort = (kAIDataPort)lFunctionNode.GetPort("DataReturn");
                            lDataPort.SetData(lResult, null);
                        }
                        else
                        {
                            throw new Exception("No property at index " + lPropertyIndex);
                        }
                    }

                    /// <summary>
                    /// To be used for getting property count and default values.
                    /// </summary>
                    public static kAIDefaultReturnConfiguration DefaultConfig = new kAIDefaultReturnConfiguration(null);
                }

                /// <summary>
                /// Wrapper for static method to create the dictionary. 
                /// </summary>
                [Serializable]
                class kAIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// Configure the default dictionary. 
                    /// </summary>
                    public static void ConfigureDictionary()
                    {
                        sReturnConfigs = new Dictionary<string, kAIIReturnConfigurationDictionary>();

                        // Return configuration for boolean, the type here is just arbitrary. 
                        kAIReturnConfigurationDictionary<object>.AddBooleanConfiguration();
                    }
                }

                /// <summary>
                /// Represents a specialised set of return properties for a specific type. 
                /// </summary>
                /// <typeparam name="T">The type this is a specific set for. </typeparam>
                [Serializable]
                class kAIReturnConfigurationDictionary<T> : kAIIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// The actions to perform when the function is evaluated. 
                    /// </summary>
                    [NonSerialized]
                    List<Action<kAINodeObject, T, T>> lEvaluateActions;

                    /// <summary>
                    /// Functions for generating ports that are required. 
                    /// </summary>
                    [NonSerialized]
                    List<Func<kAIPort>> lAdditionalPorts;

                    /// <summary>
                    /// Names of the properties.
                    /// </summary>
                    List<string> mProperties;

                    /// <summary>
                    /// The number of properties
                    /// </summary>
                    public int PropertyCount
                    {
                        get
                        {
                            return lEvaluateActions.Count;
                        }
                    }

                    /// <summary>
                    /// The names of the properties.
                    /// </summary>
                    public IEnumerable<string> PropertyNames
                    {
                        get
                        {
                            return mProperties;
                        }
                    }

                    /// <summary>
                    /// Whether each property is enabled on default. 
                    /// </summary>
                    public IEnumerable<bool> PropertyDefaults
                    {
                        get
                        {
                            foreach(bool lDefaultValue in kAIDefaultReturnConfiguration.DefaultConfig.PropertyDefaults)
                            {
                            yield return lDefaultValue;

                            }

                            for (int i =  kAIDefaultReturnConfiguration.DefaultConfig.PropertyCount; i < PropertyCount; ++i)
                            {
                                yield return false;
                            }
                        }
                    }

                    /// <summary>
                    /// Create a new configuration dictionary. 
                    /// </summary>
                    public kAIReturnConfigurationDictionary()
                    {
                        lEvaluateActions = new List<Action<kAINodeObject, T, T>>();
                        lAdditionalPorts = new List<Func<kAIPort>>();
                        mProperties = new List<string>();
                    }

                    /// <summary>
                    /// Get one of the external ports for a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <returns>A new instance of the port. </returns>
                    public kAIPort GetExternalPort(int lPropertyIndex)
                    {
                        return lAdditionalPorts[lPropertyIndex]();
                    }

                    /// <summary>
                    /// Run the code for the corresponding proeprty. . 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <param name="lFunctionNode">The node executing the function. </param>
                    /// <param name="lResult">The result of the invoked function. </param>
                    /// <param name="lPreviousResult">The result last time the function was invoked. </param>
                    public void RunCode(int lPropertyIndex, kAINodeObject lFunctionNode, object lResult, object lPreviousResult)
                    {
                        lEvaluateActions[lPropertyIndex](lFunctionNode, (T)lResult, (T)lPreviousResult);
                    }

                    /// <summary>
                    /// Add the default return properties to a specific property. 
                    /// </summary>
                    /// <typeparam name="U">The type we are creating. </typeparam>
                    /// <param name="lCustom">The configuration dictionary we are creating. </param>
                    static void AddDefaultConfigToCustom<U>(kAIReturnConfigurationDictionary<U> lCustom)
                    {
                        kAIDefaultReturnConfiguration lDefaultReturn = new kAIDefaultReturnConfiguration(typeof(U));

                        int lDefaultPropertyIndex = 0;
                        foreach (string lPropertyName in lDefaultReturn.PropertyNames)
                        {
                            Assert(null, lDefaultPropertyIndex < lDefaultReturn.PropertyCount);
                            int lCapturedPropertyIndex = lDefaultPropertyIndex;
                            lCustom.lAdditionalPorts.Add(() => { return lDefaultReturn.GetExternalPort(lCapturedPropertyIndex); });

                            lCustom.lEvaluateActions.Add((lNodeObject, lResult, lPrevResult) =>
                            {
                                lDefaultReturn.RunCode(lCapturedPropertyIndex, lNodeObject, lResult, lPrevResult);
                            });

                            lCustom.mProperties.Add(lPropertyName);

                            ++lDefaultPropertyIndex;
                        }
                    }

                    /// <summary>
                    /// Create the ReturnConfigurationDictionary for type boolean.
                    /// </summary>
                    public static void AddBooleanConfiguration()
                    {
                        kAIReturnConfigurationDictionary<bool> lBoolReturnTypeConfigDictionary = new kAIReturnConfigurationDictionary<bool>();

                        // We also add all the default settings to this.
                        AddDefaultConfigToCustom<bool>(lBoolReturnTypeConfigDictionary);


                        // OnTrue Trigger
                        {
                            const string kPortID = "OnTrue";
                            Func<kAIPort> lOnTruePortCreate = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            lBoolReturnTypeConfigDictionary.lAdditionalPorts.Add(lOnTruePortCreate);

                            Action<kAINodeObject, bool, bool> lOnTrueAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (lResult)
                                {
                                    kAITriggerPort lActualOnTrue = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnTrue.Trigger();
                                }
                            };

                            lBoolReturnTypeConfigDictionary.lEvaluateActions.Add(lOnTrueAction);
                            lBoolReturnTypeConfigDictionary.mProperties.Add("TriggerOnTrue");
                        }

                        // OnFalse Trigger
                        {
                            const string kPortID = "OnFalse";
                            Func<kAIPort> lOnFalse = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            lBoolReturnTypeConfigDictionary.lAdditionalPorts.Add(lOnFalse);

                            Action<kAINodeObject, bool, bool> lOnFalseACtion = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (!lResult)
                                {
                                    kAITriggerPort lActualOnFalse = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnFalse.Trigger();
                                }
                            };

                            lBoolReturnTypeConfigDictionary.lEvaluateActions.Add(lOnFalseACtion);
                            lBoolReturnTypeConfigDictionary.mProperties.Add("TriggerOnFalse");
                        }

                        // OnBecomeTrue Trigger
                        {
                            const string kPortID = "OnBecomeTrue";
                            Func<kAIPort> lOnTruePortCreate = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            lBoolReturnTypeConfigDictionary.lAdditionalPorts.Add(lOnTruePortCreate);

                            Action<kAINodeObject, bool, bool> lOnTrueAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (lResult && !lOldResult)
                                {
                                    kAITriggerPort lActualOnTrue = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnTrue.Trigger();
                                }
                            };

                            lBoolReturnTypeConfigDictionary.lEvaluateActions.Add(lOnTrueAction);
                            lBoolReturnTypeConfigDictionary.mProperties.Add("TriggerOnBecomeTrue");
                        }

                        // OnBecomeFalse Trigger
                        {
                            const string kPortID = "OnBecomeFalse";
                            Func<kAIPort> lOnFalse = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            lBoolReturnTypeConfigDictionary.lAdditionalPorts.Add(lOnFalse);

                            Action<kAINodeObject, bool, bool> lOnFalseACtion = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (!lResult && lOldResult)
                                {
                                    kAITriggerPort lActualOnFalse = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnFalse.Trigger();
                                }
                            };

                            lBoolReturnTypeConfigDictionary.lEvaluateActions.Add(lOnFalseACtion);
                            lBoolReturnTypeConfigDictionary.mProperties.Add("TriggerOnBecomeFalse");
                        }

                        sReturnConfigs.Add(typeof(bool).ToString(), lBoolReturnTypeConfigDictionary);
                    }

                }

                /// <summary>
                /// The dictionary of return configurations for each type. 
                /// </summary>
                static Dictionary<string, kAIIReturnConfigurationDictionary> sReturnConfigs;
            }
        }
    }
}