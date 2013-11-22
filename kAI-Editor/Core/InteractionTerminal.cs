using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

using kAI.Core;

namespace kAI.Editor.Core
{
    static class kAIInteractionTerminal
    {
        static kAIXmlBehaviour mBehaviour;

        static List<Tuple<Regex, MethodInfo>> mCommands;

        [AttributeUsage(AttributeTargets.Method)]
        public class TerminalCommand : Attribute
        {
            public TerminalCommand()
            {              
            }
        }

        public static void Init(kAIXmlBehaviour lBehaviour)
        {
            mBehaviour = lBehaviour;

            mCommands = new List<Tuple<Regex, MethodInfo>>();

            foreach (MethodInfo lMethod in typeof(kAIInteractionTerminal).GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (lMethod.GetCustomAttributes(typeof(TerminalCommand), false).Length > 0)
                {
                    StringBuilder lRegex = new StringBuilder(lMethod.Name + @"\s*\(\s*");
                    foreach (ParameterInfo lParam in lMethod.GetParameters())
                    {
                        if (lParam.ParameterType == typeof(string))
                        {
                            lRegex.Append("\"(.+)\"");
                        }
                        else if(lParam.ParameterType == typeof(int))
                        {
                            lRegex.Append(@"(\d+)");
                        }

                        if(lParam != lMethod.GetParameters().Last())
                        {
                            lRegex.Append(@"\s*,\s*");
                        }
                    }

                    lRegex.Append(@"\s*\)");

                    mCommands.Add(new Tuple<Regex, MethodInfo>(new Regex(lRegex.ToString()), lMethod));
                }
            }
        }

        public static bool RunCommand(string lCommand)
        {
            foreach (Tuple<Regex, MethodInfo> lMethodMatcher in mCommands)
            {
                Regex lMatcher = lMethodMatcher.Item1;
                Match lMatch = lMatcher.Match(lCommand);
                if (lMatch.Success)
                {
                    ParameterInfo[] lParamInfos = lMethodMatcher.Item2.GetParameters();
                    if(lMatch.Groups.Count - 1 == lParamInfos.Length)
                    {
                        int lCount = 0;
                        object[] lParamValues = new object[lParamInfos.Length];
                        foreach (ParameterInfo lParamInfo in lParamInfos)
                        {
                            if (lParamInfo.ParameterType == typeof(string))
                            {
                                lParamValues[lCount] = lMatch.Groups[lCount + 1].Value;
                            }
                            else if (lParamInfo.ParameterType == typeof(int))
                            {
                                try
                                {
                                    int lValue = Int32.Parse(lMatch.Groups[lCount + 1].Value);
                                    lParamValues[lCount] = lValue;
                                }
                                catch (FormatException ex)
                                {
                                    kAIObject.LogWarning(null, "Error parsing integer value for parameter", 
                                        new KeyValuePair<string, object>("Exception", ex),
                                        new KeyValuePair<string, object>("Input", lMatch.Groups[lCount + 1].Value));
                                    return false;
                                }
                            }

                            ++lCount;
                        }

                        try
                        {
                            return (bool)lMethodMatcher.Item2.Invoke(null, lParamValues);
                        }
                        catch (TargetException ex)
                        {
                            kAIObject.LogWarning(null, "Error calling function", new KeyValuePair<string, object>("Exception", ex));
                            return false;
                        }
                        catch (ArgumentException ex)
                        {
                            kAIObject.LogWarning(null, "Error calling function", new KeyValuePair<string, object>("Exception", ex));
                            return false;
                        }
                        catch (TargetInvocationException ex)
                        {
                            kAIObject.LogWarning(null, "Error calling function", new KeyValuePair<string, object>("Exception", ex));
                            return false;
                        }
                        catch (TargetParameterCountException ex)
                        {
                            kAIObject.LogWarning(null, "Error calling function", new KeyValuePair<string, object>("Exception", ex));
                            return false;
                        }
                        catch (MethodAccessException ex)
                        {
                            kAIObject.LogWarning(null, "Error calling function", new KeyValuePair<string, object>("Exception", ex));
                            return false;
                        }                        
                    }
                }
            }

            kAIObject.LogWarning(null, "Error could not match function", new KeyValuePair<string, object>("Command", lCommand));
            kAIObject.LogWarning(null, "Run Help() for a list of commands ");
            return false;
        }

        [TerminalCommand()]
        public static bool TriggerPort(string lPortName)
        {
            try
            {
                kAIPort lPort = mBehaviour.InternalPorts.First((lPortToCheck) => { return lPortToCheck.PortID == lPortName; });
                kAITriggerPort lTriggerPort = lPort as kAITriggerPort;
                if (lTriggerPort != null)
                {
                    lTriggerPort.Trigger();
                    return true;
                }
                else
                {
                    kAIObject.LogWarning(null, "This is not a trigger port");
                    return false;
                }
            }
            catch(InvalidOperationException)
            {
                kAIObject.LogWarning(null, "Could not find port of specified name", new KeyValuePair<string, object>("Port name given", lPortName));
                return false;
            }
        }

        [TerminalCommand()]
        public static bool RunUpdate(int count)
        {
            for(int i = 0; i < count; ++i)
            {
                mBehaviour.Update(1.0f / 60.0f, null);
            }

            return true;
        }

        [TerminalCommand()]
        public static bool AddTriggerPort(string lPortID, string lPortDirection)
        {
            mBehaviour.AddInternalPort(new kAITriggerPort(lPortID, (kAIPort.ePortDirection)Enum.Parse(typeof(kAIPort.ePortDirection), lPortDirection)), true);
            return true;
        }

        [TerminalCommand()]
        public static bool ConnectPorts(string lStart, string lEnd)
        {
            kAIPort lStartPort;
            {
                string[] lStartSplit = lStart.Split(':');
                if (lStartSplit.Length != 2)
                {
                    kAIObject.LogWarning(null, "Invalid start port id, use format: \"NodeID:PortID\"");
                    return false;
                }

                bool lIsNodePort = lStartSplit[0].Length > 0;
                kAINodeID lNodeID = lIsNodePort ? new kAINodeID(lStartSplit[0]) : kAINodeID.InvalidNodeID;
                kAIPortID lPortID = new kAIPortID(lStartSplit[1]);

                lStartPort = mBehaviour.GetInternalPort(lPortID, lNodeID);
            }

            kAIPort lEndPort;
            {
                string[] lEndSplit = lEnd.Split(':');
                if (lEndSplit.Length != 2)
                {
                    kAIObject.LogWarning(null, "Invalid end port id, use format: \"NodeID:PortID\"");
                    return false;
                }

                bool lIsNodePort = lEndSplit[0].Length > 0;
                kAINodeID lNodeID = lIsNodePort ? new kAINodeID(lEndSplit[0]) : kAINodeID.InvalidNodeID;
                kAIPortID lPortID = new kAIPortID(lEndSplit[1]);

                lEndPort = mBehaviour.GetInternalPort(lPortID, lNodeID);
            }

            kAIPort.ePortConnexionResult lResult = kAIPort.ConnectPorts(lStartPort, lEndPort);

            return lResult == kAIPort.ePortConnexionResult.PortConnexionResult_OK;
            
        }

        [TerminalCommand()]
        public static bool Help()
        {
            foreach (Tuple<Regex, MethodInfo> lMethod in mCommands)
            {
                StringBuilder lHelpMessage = new StringBuilder(lMethod.Item2.Name + "(" );
                foreach(ParameterInfo lParam in lMethod.Item2.GetParameters())
                {
                    lHelpMessage.Append(lParam.Name + " : " + lParam.ParameterType.ToString());
                    if(lParam != lMethod.Item2.GetParameters().Last())
                    {
                        lHelpMessage.Append(", ");
                    }
                }

                lHelpMessage.Append(")");
                kAIObject.LogMessage(null, lHelpMessage.ToString());
            }

            return true;
        }

    }
}
