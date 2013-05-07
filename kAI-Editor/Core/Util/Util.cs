using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

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

        public static Point GetControlPosition(this Control lControl, Control lDesiredRoot = null)
        {
            Point lPosition = lControl.Location;
            while (lControl.Parent != lDesiredRoot)
            {
                lControl = lControl.Parent;
                lPosition = AddPoints(lPosition, lControl.Location);
            }

            return lPosition;
        }
    }
}
