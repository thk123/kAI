using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;

namespace kAI.Editor.Controls.WinForms
{
    public partial class ConstantNodeCreator : Form
    {
        public kAIConstantNode Node
        {
            get;
            private set;
        }

        public ConstantNodeCreator()
        {
            InitializeComponent();

            mIntConstantRadio.Checked = true;

        }

        private void mCreateBtn_Click(object sender, EventArgs e)
        {
            if (mIntConstantRadio.Checked)
            {
                int lValue;
                if (Int32.TryParse(mValueTextBox.Text, out lValue))
                {
                    Node = new kAIConstantIntNode(lValue);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Invalid entry in to the value text box");
                }
            }
            else if (mFloatConstantRadio.Checked)
            {
                float lValue;
                if (float.TryParse(mValueTextBox.Text, out lValue))
                {
                    Node = new kAIConstantFloatNode(lValue);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Invalid entry in to the value text box");
                }
            }
            else if (mStringConstantRadio.Checked)
            {
                Node = new kAIConstantStringNode(mValueTextBox.Text);
                DialogResult = DialogResult.OK;
            }
        }
    }
}
