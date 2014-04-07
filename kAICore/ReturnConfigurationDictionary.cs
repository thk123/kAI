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
                public interface kAIIReturnConfigurationDictionary
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
                    /// The type this is a property dictionary for 
                    /// </summary>
                    Type PropertyType
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
                    /// This is the properties for all types so not directly applicable. 
                    /// </summary>
                    public Type PropertyType
                    {
                        get
                        {
                            return mType;
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
                public class kAIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// Configure the default dictionary. 
                    /// </summary>
                    public static void ConfigureDictionary()
                    {
                        sReturnConfigs = new Dictionary<string, kAIIReturnConfigurationDictionary>();

                        
                        kAIReturnConfigurationDictionary.AddBooleanConfiguration();
                    }

                    /// <summary>
                    /// Add the default return properties to a specific property. 
                    /// </summary>
                    /// <typeparam name="U">The type we are creating. </typeparam>
                    /// <param name="lCustom">The configuration dictionary we are creating. </param>
                    public static void AddDefaultConfigToCustom<U>(kAIReturnConfigurationDictionary<U> lCustom)
                    {
                        kAIDefaultReturnConfiguration lDefaultReturn = new kAIDefaultReturnConfiguration(typeof(U));

                        int lDefaultPropertyIndex = 0;
                        foreach (string lPropertyName in lDefaultReturn.PropertyNames)
                        {
                            Assert(null, lDefaultPropertyIndex < lDefaultReturn.PropertyCount);
                            int lCapturedPropertyIndex = lDefaultPropertyIndex;

                            lCustom.AddProperty(
                                lPropertyName,  // the name of the property
                                () =>  // the function to create a port
                                {
                                    return lDefaultReturn.GetExternalPort(lCapturedPropertyIndex);
                                },

                                (lNodeObject, lResult, lPrevResult) => // the action to take when the function is evaluated
                                {
                                    lDefaultReturn.RunCode(lCapturedPropertyIndex, lNodeObject, lResult, lPrevResult);
                                }
                            );

                            ++lDefaultPropertyIndex;
                        }
                    }

                    /// <summary>
                    /// Create the ReturnConfigurationDictionary for type boolean.
                    /// </summary>
                    public static void AddBooleanConfiguration()
                    {
                        AddDictionary(new kAIBooleanConfiguration());
                    }

                    /// <summary>
                    /// Add a dictionary to the set of return configurations. 
                    /// </summary>
                    /// <param name="lDictionary">The set of properties. </param>
                    public static void AddDictionary(kAIIReturnConfigurationDictionary lDictionary)
                    {
                        sReturnConfigs.Add(lDictionary.PropertyType.ToString(), lDictionary);
                    }

                }


                 
                /// <summary>
                /// Represents a specialised set of return properties for a specific type. 
                /// </summary>
                /// <typeparam name="T">The type this is a specific set for. </typeparam>
                public class kAIReturnConfigurationDictionary<T> : kAIIReturnConfigurationDictionary
                {
                    /// <summary>
                    /// The actions to perform when the function is evaluated. 
                    /// </summary>
                    List<Action<kAINodeObject, T, T>> mEvaluateActions;

                    /// <summary>
                    /// Functions for generating ports that are required. 
                    /// </summary>
                    List<Func<kAIPort>> mAdditionalPorts;

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
                            return mEvaluateActions.Count;
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
                    /// The type this a dictionary of properties for.
                    /// </summary>
                    public Type PropertyType
                    {
                        get
                        {
                            return typeof(T);
                        }
                    }

                    /// <summary>
                    /// Create a new configuration dictionary. 
                    /// </summary>
                    public kAIReturnConfigurationDictionary()
                    {
                        mEvaluateActions = new List<Action<kAINodeObject, T, T>>();
                        mAdditionalPorts = new List<Func<kAIPort>>();
                        mProperties = new List<string>();
                    }

                    /// <summary>
                    /// Get one of the external ports for a specific property. 
                    /// </summary>
                    /// <param name="lPropertyIndex">The index of the property. </param>
                    /// <returns>A new instance of the port. </returns>
                    public kAIPort GetExternalPort(int lPropertyIndex)
                    {
                        return mAdditionalPorts[lPropertyIndex]();
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
                        mEvaluateActions[lPropertyIndex](lFunctionNode, (T)lResult, (T)lPreviousResult);
                    }

                    /// <summary>
                    /// Add a property to this dictionary of properties. 
                    /// </summary>
                    /// <param name="lPropertyName">The (unqiue) name of the property. </param>
                    /// <param name="lPortCreator">A function which creates any ports this property requires. </param>
                    /// <param name="lAction">The action that should be performed when the function is evaluated. </param>
                    public void AddProperty(string lPropertyName, Func<kAIPort> lPortCreator, Action<kAINodeObject, T, T> lAction)
                    {
                        mAdditionalPorts.Add(lPortCreator);
                        mEvaluateActions.Add(lAction);
                        mProperties.Add(lPropertyName);
                    }
                }

                /// <summary>
                /// The dictionary of return configurations for each type. 
                /// </summary>
                static Dictionary<string, kAIIReturnConfigurationDictionary> sReturnConfigs;

                class kAIBooleanConfiguration : kAIReturnConfigurationDictionary<bool>
                {
                    public kAIBooleanConfiguration()
                    {
                        kAIReturnConfigurationDictionary.AddDefaultConfigToCustom<bool>(this);


                        // OnTrue Trigger
                        {
                            const string kPortID = "OnTrue";
                            Func<kAIPort> lOnTruePortCreate = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            

                            Action<kAINodeObject, bool, bool> lOnTrueAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (lResult)
                                {
                                    kAITriggerPort lActualOnTrue = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnTrue.Trigger();
                                }
                            };


                            AddProperty("TriggerOnTrue", lOnTruePortCreate, lOnTrueAction);
                        }

                        // OnFalse Trigger
                        {
                            const string kPortID = "OnFalse";
                            Func<kAIPort> lOnFalse = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };
                            

                            Action<kAINodeObject, bool, bool> lOnFalseAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (!lResult)
                                {
                                    kAITriggerPort lActualOnFalse = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnFalse.Trigger();
                                }
                            };


                            AddProperty("TriggerOnFalse", lOnFalse, lOnFalseAction);
                        }

                        // OnBecomeTrue Trigger
                        {
                            const string kPortID = "OnBecomeTrue";
                            Func<kAIPort> lOnTruePortCreate = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };

                            Action<kAINodeObject, bool, bool> lOnTrueAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (lResult && !lOldResult)
                                {
                                    kAITriggerPort lActualOnTrue = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnTrue.Trigger();
                                }
                            };

                            
                            AddProperty("TriggerOnBecomeTrue", lOnTruePortCreate, lOnTrueAction);
                        }

                        // OnBecomeFalse Trigger
                        {
                            const string kPortID = "OnBecomeFalse";
                            Func<kAIPort> lOnFalse = () => { return new kAITriggerPort(kPortID, kAIPort.ePortDirection.PortDirection_Out); };

                            Action<kAINodeObject, bool, bool> lOnFalseAction = (lNodeObject, lResult, lOldResult) =>
                            {
                                if (!lResult && lOldResult)
                                {
                                    kAITriggerPort lActualOnFalse = lNodeObject.GetPort(kPortID) as kAITriggerPort;
                                    lActualOnFalse.Trigger();
                                }
                            };

                            AddProperty("TriggerOnBecomeFalse", lOnFalse, lOnFalseAction);
                        }

                        kAIReturnConfigurationDictionary.AddDefaultConfigToCustom<bool>(this);
                    }
                }
            }
        }
    }
}