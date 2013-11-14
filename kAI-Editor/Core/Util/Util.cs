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

        public static int GetSquareSize(this Rectangle lSize)
        {
            return (lSize.Width * lSize.Width) + (lSize.Height * lSize.Height);
        }

        /// <summary>
        /// Partition a source into two lists based on some splitter function. 
        /// </summary>
        /// <typeparam name="T">The type of object. </typeparam>
        /// <param name="lSource">The enumerable data source. </param>
        /// <param name="lSplitter">
        /// A function which takes an element in the source and determines if it should 
        /// be in the true set or the false set. 
        /// </param>
        /// <returns>
        /// Two lists, one corresponding to elements that the partitioning function returned 
        /// true for (Item1) and one which the partitioning function return false (Item2).
        /// </returns>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Split<T>(this IEnumerable<T> lSource, Func<T, bool> lSplitter)
        {
            List<T> lTrueSet = new List<T>();
            List<T> lFalseSet = new List<T>();

            foreach (T lEntry in lSource)
            {
                if (lSplitter(lEntry))
                {
                    lTrueSet.Add(lEntry);
                }
                else
                {
                    lFalseSet.Add(lEntry);
                }
            }

            return new Tuple<IEnumerable<T>, IEnumerable<T>>(lTrueSet, lFalseSet);
        }
    }
}
