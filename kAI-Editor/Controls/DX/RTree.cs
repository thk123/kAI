using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using kAI.Core;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// An R-Tree - data structure for storing potentially intersecting rectangles 
    /// and searching them efficiently. 
    /// </summary>
    /// <typeparam name="T">The type of object stored for each rectangle. </typeparam>
    class kAIRTree<T> where T : class
    {
        /// <summary>
        /// A heuristic for evaluating which sub rectangle to use. 
        /// </summary>
        /// <param name="lNewRectangle">The rectangle we are trying to insert</param>
        /// <param name="lExistingRectangles">The set of children. </param>
        /// <returns>An index of the lExistingRectangles array. </returns>
        delegate int Heuristic(Rectangle lNewRectangle, kAIRTree<T>[] lExistingRectangles);

        // The number of pages each node has as children
        const int kPageSize = 4;

        // The largest size where we will still expand an existing node despite their being an empty slot
        const int kMaxNewPageOversizeSquared = 5 * 5 * 5 * 5;

        // The set of child nodes of this node
        kAIRTree<T>[] mRootNodes;

        // Number of filed pages at this node
        int mNumberOfNodes;

        // Number of actual data elements contained in this (sub)tree.
        int lDataEntries;

        // The heuristic the tree is using. 
        Heuristic mSelectedHeuristic;

        /// <summary>
        /// The contents of this node, null if just a node. 
        /// </summary>
        public T Contents
        {
            get;
            set;
        }

        /// <summary>
        /// The rectangle of this node, all sub nodes are contained within this node. 
        /// </summary>
        public Rectangle Rectangle
        {
            get;
            private set;
        }        

        /// <summary>
        /// For creating a new R-Tree. 
        /// </summary>
        public kAIRTree()
            :this(Rectangle.Empty, null)
        {  }

        /// <summary>
        /// For creating new nodes within an existing tree. 
        /// </summary>
        /// <param name="lRect">The rectangle. </param>
        /// <param name="lContents">The contents of this node. </param>
        kAIRTree(Rectangle lRect, T lContents)
        {
            mRootNodes = new kAIRTree<T>[kPageSize];
            mNumberOfNodes = 0;
            mSelectedHeuristic = SmallestVolumeIncreaseHeuristic;
            Rectangle = lRect;
            Contents = lContents;

            if (lContents != null)
            {
                lDataEntries = 1;
            }
            else
            {
                lDataEntries = 0;
            }
        }


        /// <summary>
        /// Add a new entry to the tree. 
        /// </summary>
        /// <param name="lRectangle">The rectangle to use. </param>
        /// <param name="lContents">The contents of the node. </param>
        public void AddRectangle(Rectangle lRectangle, T lContents)
        {
            lDataEntries += 1;

            if (Rectangle == lRectangle)
            {
                // TODO: this exception should be enabled, at the moment though, you can pick up 2 nodes and drop them down in the same place
                // once this is fixed, this exception should be reenabled
                //throw new Exception("Rectangle already added");
                kAIObject.Assert(null, false, "Probably got 2 nodes amirite?");
                return;
            }

            // If we don't already contain the rectangle, we must bend to fit this requirement as our masters have chosen this to be the case
            if (!Rectangle.Contains(lRectangle))
            {
                // We are resizing this rectangle so it does not represent the object, so we add that
                Rectangle lOldRectangle = Rectangle;

                // Create the new rectangle
                Rectangle = Rectangle.Union(Rectangle, lRectangle);

                // If we were a precise match, we now are not and must push ourselves down in to the tree
                if (Contents != null)
                {
                    AddRectangle(lOldRectangle, Contents);
                    Contents = null;
                }
            }

            // Select a rectangle using the heuristic. 
            int lChosenRectangleID = mSelectedHeuristic(lRectangle, mRootNodes);

            // if the heuristic choose make a new node, make a new node. 
            if (mRootNodes[lChosenRectangleID] == null)
            {
                mRootNodes[lChosenRectangleID] = new kAIRTree<T>(lRectangle, lContents);
                ++mNumberOfNodes;
            }
            else // Using an existing node, so we add the new contents to it. 
            {
                mRootNodes[lChosenRectangleID].AddRectangle(lRectangle, lContents);
            }

            kAIObject.Assert(null, mRootNodes[lChosenRectangleID].Rectangle.Contains(lRectangle));

        }

        /// <summary>
        /// Search the tree for rectangles that contain the specified point. 
        /// </summary>
        /// <param name="lPoint">The point to search for. </param>
        /// <returns>A list of the data whose rectangle contains the point. </returns>
        public IEnumerable<T> GetRectsContainingPoint(Point lPoint)
        {
            List<T> lRectsContaining = new List<T>();

            if (Contents != null)
            {
                lRectsContaining.Add(Contents);
            }

            for (int lPageIndex = 0; lPageIndex < mNumberOfNodes; ++lPageIndex)
            {
                kAIObject.Assert(null, mRootNodes[lPageIndex], "R-Tree is in an invalid state - has an empty page where it didn't think it did.");
                if (mRootNodes[lPageIndex].Rectangle.Contains(lPoint))
                {
                    lRectsContaining.AddRange(mRootNodes[lPageIndex].GetRectsContainingPoint(lPoint));
                }
            }

            return lRectsContaining;
        }

        /// <summary>
        /// Remove a rectangle from the tree. 
        /// </summary>
        /// <param name="lRectangle">The rectangle to search for. </param>
        /// <returns>The data associated with the rectangle (or null if the rectangle can't be found). </returns>
        public T RemoveRectangle(Rectangle lRectangle)
        {
            if(Contents != null && lRectangle.Equals(Rectangle))
            {
                T lContents = Contents;
                Contents = null;
                --lDataEntries;
                return lContents;
            }

            for (int lPageIndex = 0; lPageIndex < mNumberOfNodes; ++lPageIndex)
            {
                kAIRTree<T> lSubTree = mRootNodes[lPageIndex];
                if (lSubTree.Rectangle.Contains(lRectangle))
                {
                    T lResult = lSubTree.RemoveRectangle(lRectangle);
                    if (lResult != null)
                    {
                        // if the tree is now empty, we delete it
                        if (lSubTree.lDataEntries == 0)
                        {
                            // remove the sub tree
                            while (lPageIndex < mNumberOfNodes - 1)
                            {
                                mRootNodes[lPageIndex] = mRootNodes[lPageIndex + 1];
                                ++lPageIndex;
                            }
                            mRootNodes[mNumberOfNodes - 1] = null;
                            --mNumberOfNodes;
                            
                        }

                        // We are removing an entry, so we drop the data count and pass it on up
                        --lDataEntries;
                        return lResult;

                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all the actual elements within the tree. 
        /// </summary>
        /// <returns>A list of all the elements. </returns>
        public IEnumerable<T> GetAllContents()
        {
            List<T> lRects = new List<T>();

            if (Contents != null)
            {
                lRects.Add(Contents);
            }

            foreach (kAIRTree<T> lSubTree in mRootNodes)
            {
                if (lSubTree != null)
                {
                    lRects.AddRange(lSubTree.GetAllContents());
                }
            }

            return lRects;
        }

        /// <summary>
        /// A heuristic for choosing a rectangle from a list.
        /// We choose the rectangle that will create the smallest possible
        /// increase in area of the rectangle. 
        /// </summary>
        /// <param name="lNewRectangle">The rectangle we are trying to insert</param>
        /// <param name="lExistingRectangles">The set of children. </param>
        /// <returns>An index of the lExistingRectangles array. </returns>
        int SmallestVolumeIncreaseHeuristic(Rectangle lNewRectangle, kAIRTree<T>[] lExistingRectangles)
        {
            int lBest;
            int lBestIndex;

            if (mNumberOfNodes < kPageSize)
            {
                lBest = kMaxNewPageOversizeSquared;
                lBestIndex = mNumberOfNodes;
            }
            else
            {
                lBest = Int32.MaxValue;
                lBestIndex = -1;
            }

            for (int lChildIndex = 0; lChildIndex < kPageSize; ++lChildIndex)
            {
                kAIRTree<T> lSubRect = mRootNodes[lChildIndex];
                if (lSubRect != null)
                {
                    if (lSubRect.Rectangle.Contains(lNewRectangle))
                    {
                        // We have found an ideal rectangle, we put it in that
                        return lChildIndex;
                    }
                    else
                    {
                        // We try creating a new rectangle that would contain both
                        Rectangle lSuperRectangle = Rectangle.Union(lNewRectangle, lSubRect.Rectangle);

                        // new rectangle should contain both
                        kAIObject.Assert(null, lSuperRectangle.Contains(lNewRectangle));
                        kAIObject.Assert(null, lSuperRectangle.Contains(lSubRect.Rectangle));

                        // This is the new size of the rectangle, we want to minimize this value
                        int lDeltaSizeSqrd = lSuperRectangle.GetSquareSize();

                        // If this is the best so far, we mark this as the rectangle we plan to make
                        if (lDeltaSizeSqrd < lBest)
                        {
                            lBest = lDeltaSizeSqrd;
                            lBestIndex = lChildIndex;
                        }
                    }
                }
            }

            kAIObject.Assert(null, lBestIndex >= 0);
            kAIObject.Assert(null, lBestIndex < kPageSize);

            return lBestIndex;
        }

        /// <summary>
        /// Returns a string representing this (sub)tree. 
        /// </summary>
        /// <returns>A string with the rectangle of this node and its contents (if it has any). </returns>
        public override string ToString()
        {
            if (Contents != null)
            {
                return Rectangle.ToString() + "[ " + Contents.ToString() + " ]";
            }
            else
            {
                return Rectangle.ToString() + "[ - ]";
            }
        }
    }


}
