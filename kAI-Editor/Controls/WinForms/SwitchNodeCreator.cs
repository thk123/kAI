using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using kAI.Editor.Core;
using kAI.Core;

namespace kAI.Editor.Controls.WinForms
{
    public partial class SwitchNodeCreator : Form
    {
        public kAINodeObject mObject; 

        public SwitchNodeCreator()
        {
            InitializeComponent();
            mSearchableList.SetDataSource(GlobalServices.LoadedProject.ProjectTypes.Where((lType) =>
                {
                    return lType.IsEnum;
                }));

            mObject = null;
        }

        private void mCreateBtn_Click(object sender, EventArgs e)
        {
            Type lSelectedType = (Type)mSearchableList.SelectedItem;
            mObject = kAISwitchNode.CreateSwitchNode(lSelectedType);
        }
    }
}
