using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Core.Debug;

namespace kAI.Editor.Controls.WinForms.Debug
{
    public partial class DebugWindow : Form
    {
        public event Action<kAIBehaviourEntry> OnEntrySelected;

        public DebugWindow()
        {
            
            InitializeComponent();
            HasSelectedEntry = false;
            
        }

        public void SetEntries(IEnumerable<kAIBehaviourEntry> lEntries)
        {
            //mDebuggableBehaviours.Items.Clear();
            foreach (kAIBehaviourEntry lEntry in lEntries)
            {
                if (!mDebuggableBehaviours.Items.Contains(lEntry))
                {
                    mDebuggableBehaviours.Items.Add(lEntry);
                }
            }

        }

        private void mDebuggableBehaviours_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(mDebuggableBehaviours.SelectedItem != null)
            {
                kAIBehaviourEntry lSelected = (kAIBehaviourEntry)mDebuggableBehaviours.SelectedItem;

                if (OnEntrySelected != null)
                {
                    OnEntrySelected(lSelected);
                }

                SelectedEntry = lSelected;
                HasSelectedEntry = true;
            }
            else
            {
                HasSelectedEntry = false;
            }
        }

        public bool HasSelectedEntry { get; private set; }

        public kAIBehaviourEntry SelectedEntry { get; private set; }
    }
}
