﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
