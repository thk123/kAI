﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;


namespace kAI.Core
{
    /// <summary>
    /// kAICore utility and extension functions.
    /// </summary>
    public static class CoreUtil
    {
        /// <summary>
        /// Find out if a type is inherited (either directly or indirectly) from a given type.
        /// </summary>
        /// <param name="lBaseType">The type to check. </param>
        /// <param name="lTargetParent">The parent type to see if lBaseType inherits from. </param>
        /// <returns>True if the provided type, or any of its parents are lTargetParent.</returns>
        public static bool DoesInherit(this Type lBaseType, Type lTargetParent)
        {
            Type lCurrentType = lBaseType;
            do
            {
                if (lCurrentType == lTargetParent)
                    return true;

                lCurrentType = lCurrentType.BaseType;
            } while (lCurrentType != typeof(Object) && lCurrentType != null);

            return false;
        }

        /// <summary>
        /// Get the opposite direction of the port. 
        /// </summary>
        /// <param name="lDirection">The direction to revsere. </param>
        /// <returns>The opposite direction (eg in returns out, out returns in). </returns>
        public static kAIPort.ePortDirection OppositeDirection(this kAIPort.ePortDirection lDirection)
        {
            if (lDirection == kAIPort.ePortDirection.PortDirection_In)
                return kAIPort.ePortDirection.PortDirection_Out;
            else
                return kAIPort.ePortDirection.PortDirection_In;
        }

        /// <summary>
        /// Gets the value or the provided default value if the value is null. 
        /// </summary>
        /// <typeparam name="T">The nullable type to use.</typeparam>
        /// <param name="lObject">The object to check.</param>
        /// <param name="lDefault">The default value to return if lObject is null. </param>
        /// <returns>Either lObject or lDefault iff lObject is null. </returns>
        public static T GetValueOrDefault<T>(this T lObject, T lDefault) where T : class
        {
            return lObject != null ? lObject : lDefault;
        }

        /// <summary>
        /// Gets a default(T) for a specific T
        /// </summary>
        /// <param name="lObjectType">The type of this object. </param>
        /// <returns>The default value for this object. </returns>
        public static object GetDefault(this Type lObjectType)
        {
            if (lObjectType.IsValueType)
            {
                return Activator.CreateInstance(lObjectType);
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    public class SerialType
    {
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
        /// 
        /// </summary>
        /// <param name="lType"></param>
        public SerialType(Type lType)
        {
            TypeName = lType.FullName;
            AssemblyName = lType.Assembly.GetName().Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lAssemblyResolver"></param>
        /// <returns></returns>
        public Type Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
        {
            Assembly lFunctionAssembly = lAssemblyResolver(AssemblyName);
            return lFunctionAssembly.GetType(TypeName);
        }
    }
}
