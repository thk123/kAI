using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace kAI.Editor.Core.Util
{
    /// <summary>
    /// The editor used to create kAI Behaviours.
    /// </summary>

    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Utility functions used by the editor
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Add two System.Drawing.Points together. 
        /// </summary>
        /// <param name="lLHS">The left hand point.</param>
        /// <param name="lRHS">The right hand point.</param>
        /// <returns>The sum of the two points.</returns>
        public static Point SubtractPoints(Point lLHS, Point lRHS)
        {
            return new Point(lLHS.X - lRHS.X, lLHS.Y - lRHS.Y);
        }

        /// <summary>
        /// Subtracts two System.Drawing.Points from one and another. 
        /// </summary>
        /// <param name="lLHS">The left hand point.</param>
        /// <param name="lRHS">The right hand point.</param>
        /// <returns>The difference of the two points.</returns>
        public static Point AddPoints(Point lLHS, Point lRHS)
        {
            return new Point(lLHS.X + lRHS.X, lLHS.Y + lRHS.Y);
        }

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
    }
}
