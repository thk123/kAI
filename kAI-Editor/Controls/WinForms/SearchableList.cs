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
    public partial class SearchableList : UserControl
    {
        List<object> mDataSource;

        string mSearchQuery;

        Stack<List<object>> lRemovedEntries;

        public ListBox.ObjectCollection Items
        {
            get
            {
                return listBox1.Items;
            }
        }

        public ListBox.SelectedObjectCollection SelectedItems
        {
            get
            {
                return listBox1.SelectedItems;
            }
        }

        public object SelectedItem
        {
            get
            {
                return listBox1.SelectedItem;
            }
        }

        public SelectionMode SelectionMode
        {
            get
            {
                return listBox1.SelectionMode;
            }
            set
            {
                listBox1.SelectionMode = value;
                
            }
        }

        public event EventHandler SelectedValueChange;

        public SearchableList()
        {
            mSearchQuery = String.Empty;
            InitializeComponent();
            listBox1.SelectedValueChanged += new EventHandler(listBox1_SelectedValueChanged);
        }

        void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SelectedValueChange != null)
            {
                SelectedValueChange(sender, e);
            }
        }

        public void SetDataSource(IEnumerable<object> lSource)
        {
            mDataSource = new List<object>();
            lRemovedEntries = new Stack<List<object>>();
            foreach (object lData in lSource)
            {
                mDataSource.Add(lData);
                listBox1.Items.Add(lData);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateSearchTerm(textBox1.Text);
        }

        /// <summary>
        /// Update the list with a new search query. 
        /// </summary>
        /// <param name="lSearchTerm">The new search value. </param>
        public void UpdateSearchTerm(string lSearchTerm)
        {
            if (lSearchTerm != mSearchQuery)
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
                else if (lSearchTerm.Length == mSearchQuery.Length)
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
            List<object> lNewlyRemoved = new List<object>();
            foreach (object lObject in listBox1.Items)
            {
                if (!IsMatch(lObject, mSearchQuery + lNewChar))
                {
                    lNewlyRemoved.Add(lObject);
                }
            }

            foreach (object lRemoved in lNewlyRemoved)
            {
                listBox1.Items.Remove(lRemoved);
            }

            lRemovedEntries.Push(lNewlyRemoved);

            mSearchQuery += lNewChar;
        }

        void PopSearchChar(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                List<object> lEntriesToAdd = lRemovedEntries.Pop();
                foreach (object lEntry in lEntriesToAdd)
                {
                    listBox1.Items.Add(lEntry);
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

        bool IsMatch(object lEntry, string lSearchEntry)
        {
            return lEntry.ToString().ToUpper().Contains(lSearchEntry.ToUpper());
        }

        private void SearchableList_Resize(object sender, EventArgs e)
        {
            //listBox1.Height = Height - textBox1.Height - 5;
        }
    }
}
