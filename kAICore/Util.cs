using System;
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

        /// <summary>
        /// Compare two enumerable sources and check they match at every element and in size. 
        /// </summary>
        /// <typeparam name="T">The type of element. </typeparam>
        /// <param name="lListA">The first source. </param>
        /// <param name="lListB">The second source. </param>
        /// <param name="lComparer">Optionally, the comparision between 2 objects to use. If none specified, will use T.Equals</param>
        /// <returns>True if the sources contain the same number of elements and each element matches</returns>
        public static bool DoMatch<T>(this IEnumerable<T> lListA, IEnumerable<T> lListB, Func<T, T, bool> lComparer = null)
        {
            if (lComparer == null)
            {
                lComparer = (lA, lB) => { return lA.Equals(lB); };
            }


            IEnumerator<T> lEnumeratorA = lListA.GetEnumerator();
            IEnumerator<T> lEnumeratorB = lListB.GetEnumerator();

            while (lEnumeratorA.MoveNext() & lEnumeratorB.MoveNext())
            {
                if (!lComparer(lEnumeratorA.Current, lEnumeratorB.Current))
                {
                    return false;
                }
            }

            bool lAFinished = !lEnumeratorA.MoveNext();
            bool lBFinished = !lEnumeratorB.MoveNext();

            // If both lists are finished, we matched both of them correctly.
            return lAFinished && lBFinished;

        }

        /// <summary>
        /// Check whether a given enum has a given flag. 
        /// </summary>
        /// <param name="lType">The enum to check. </param>
        /// <param name="lFlagToCheck">The flag to check. </param>
        /// <returns>True if the flag is enabled. </returns>
        public static bool HasFlag<T>(this T lType, T lFlagToCheck) where T : IConvertible
        {
            int aValue = Convert.ToInt32(lType);
            int bValue = Convert.ToInt32(lFlagToCheck);

            return (aValue & bValue) > 0;
        }

        /// <summary>
        /// Get a list of the flags enabled in this enum. 
        /// </summary>
        /// <param name="lType">The enum to check. </param>
        /// <returns>An array of each of the flags that are enabled. </returns>
        public static T[] GetAsFlags<T>(this T lType) where T : IConvertible
        {
            List<T> lConstraints = new List<T>();
            foreach (T lConstraint in Enum.GetValues(lType.GetType()))
            {
                if (lType.HasFlag(lConstraint))
                {
                    lConstraints.Add(lConstraint);
                }
            }

            return lConstraints.ToArray();
        }

    }

    /// <summary>
    /// A serialisation of System.Type. 
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
        /// Create a serial representation of a given type. 
        /// </summary>
        /// <param name="lType">The type to serailse. </param>
        public SerialType(Type lType)
        {
            TypeName = lType.FullName;
            AssemblyName = lType.Assembly.GetName().Name;
        }

        /// <summary>
        /// Get the type that is serialised in this. 
        /// </summary>
        /// <param name="lAssemblyResolver">The method to use to resolve assembly names. </param>
        /// <returns>The type represented by this serialisation. </returns>
        public Type Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
        {
            Assembly lFunctionAssembly = lAssemblyResolver(AssemblyName);
            return lFunctionAssembly.GetType(TypeName);
        }
    }
}
