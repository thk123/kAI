using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// A ListBox that can be filtered. 
    /// </summary>
    /// <typeparam name="T">The type of object to put in the list box. </typeparam>
    public class FilterableList<T> : ListBox
    {
        List<T> mDataSource;

        string mSearchQuery;

        Stack<List<T>> lRemovedEntries;

        /// <summary>
        /// Create a list which can be searched in real time. 
        /// </summary>
        public FilterableList()
            :base()
        {
            mSearchQuery = String.Empty;
        }

        /// <summary>
        /// Set the data source for this list. 
        /// </summary>
        /// <param name="lSource">The source. </param>
        public void SetSource(IEnumerable<T> lSource)
        {
            mDataSource = new List<T>();
            lRemovedEntries = new Stack<List<T>>();
            foreach (T lData in lSource)
            {
                mDataSource.Add(lData);
                Items.Add(lData);
            }
        }

        /// <summary>
        /// Update the list with a new search query. 
        /// </summary>
        /// <param name="lSearchTerm">The new search value. </param>
        public void UpdateSearchTerm(string lSearchTerm)
        {
            if(lSearchTerm != mSearchQuery)
            {
                // new search term longer than the original query
                if (lSearchTerm.Length > mSearchQuery.Length)
                {
                    // is an extension of the original query so take each character in turn and filter
                    if (lSearchTerm.StartsWith(mSearchQuery))
                    {
                        for (int lStartIndex = mSearchQuery.Length; lStartIndex < lSearchTerm.Length; ++lStartIndex)
                        {
                            AppendToSearchTerm(lSearchTerm[lStartIndex]);
                        }
                    }
                    else // not an extension, so we find the last common character
                    {
                        int lLastCommonChar = CommonStem(mSearchQuery, lSearchTerm);

                        // first we pop back to the last common character
                        PopSearchChar(mSearchQuery.Length - lLastCommonChar);

                        // then we push the new characters on

                        for (int i = lLastCommonChar; i < lSearchTerm.Length; ++i)
                        {
                            AppendToSearchTerm(lSearchTerm[i]);
                        }
                    }
                }
                else if(lSearchTerm.Length == mSearchQuery.Length)
                {
                    int lLastCommonChar = CommonStem(mSearchQuery, lSearchTerm);

                    // first we pop back to the last common character
                    PopSearchChar(mSearchQuery.Length - lLastCommonChar);

                    // then we push the new characters on

                    for (int i = lLastCommonChar; i < lSearchTerm.Length; ++i)
                    {
                        AppendToSearchTerm(lSearchTerm[i]);
                    }
                }
                else // new search term is shorter than the current term
                {
                    if (mSearchQuery.StartsWith(lSearchTerm))
                    {
                        PopSearchChar(mSearchQuery.Length - lSearchTerm.Length);
                    }
                    else
                    {
                        int lLastCommonChar = CommonStem(mSearchQuery, lSearchTerm);

                        PopSearchChar(mSearchQuery.Length - lLastCommonChar);

                        for (int i = lLastCommonChar; i < lSearchTerm.Length; ++i)
                        {
                            AppendToSearchTerm(lSearchTerm[i]);
                        }
                    }
                }
            }
        }

        void AppendToSearchTerm(char lNewChar)
        {
            List<T> lNewlyRemoved = new List<T>();
            foreach (T lObject in Items.Cast<T>())
            {
                if (!IsMatch(lObject, mSearchQuery + lNewChar))
                {
                    lNewlyRemoved.Add(lObject);
                }
            }

            foreach (T lRemoved in lNewlyRemoved)
            {
                Items.Remove(lRemoved);
            }

            lRemovedEntries.Push(lNewlyRemoved);

            mSearchQuery += lNewChar;
        }

        void PopSearchChar(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                List<T> lEntriesToAdd = lRemovedEntries.Pop();
                foreach (T lEntry in lEntriesToAdd)
                {
                    Items.Add(lEntry);
                }
            }

            mSearchQuery = mSearchQuery.Substring(0, mSearchQuery.Length - count);
        }


        private int CommonStem(string lStringA, string lStringB)
        {
            for (int i = 0; i < Math.Min(lStringA.Length, lStringB.Length); ++i)
            {
                if (lStringA[i] != lStringB[i])
                {
                    return i;
                }
            }
            return Math.Min(lStringA.Length, lStringB.Length);

        }

        bool IsMatch(T lEntry, string lSearchEntry)
        {
            return lEntry.ToString().ToUpper().Contains(lSearchEntry.ToUpper());
        }
    }
}
