using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using kAI.Editor.ObjectProperties;

using kAI.Core;
using kAI.Editor.Core;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// Represents the floating properties window. 
    /// </summary>
    partial class PropertiesWindow : Form
    {
        public static kAIProject Project
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new properties window. 
        /// </summary>
        public PropertiesWindow(kAIProject lProject)
        {
            InitializeComponent();

            Project = lProject;

            mPropertiesGrid.Size = flowLayoutPanel1.Size;

            // hack: cannot register double click on the property grid, must recursively find all of the subcontrols and listen to them. 
            RegisterDC(mPropertiesGrid, mPropertiesGrid_DoubleClick);
        }

        /// <summary>
        /// Select a given object. 
        /// </summary>
        /// <param name="lSelectedObject">The object to select</param>
        public void SelectObject(kAIIPropertyEntry lSelectedObject)
        {
            mPropertiesGrid.SelectedObject = lSelectedObject;
        }

        void RegisterDC(Control lControl, EventHandler lDC)
        {
            lControl.DoubleClick += lDC;
            foreach (Control lSubControl in lControl.Controls)
            {
                RegisterDC(lSubControl, lDC);
            }
        }


        private void PropertiesWindow_Activated(object sender, EventArgs e)
        {
            Opacity = 0.9f;
        }

        private void PropertiesWindow_Deactivate(object sender, EventArgs e)
        {
            Opacity = 0.7f;
        }

        private void mPropertiesGrid_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs lArgs = e as MouseEventArgs;
            if (lArgs != null)
            {
                GridItem lSelectedItem = mPropertiesGrid.SelectedGridItem;
                kAIIClickableProperty lPortProps = lSelectedItem.PropertyDescriptor as kAIIClickableProperty;
                if (lPortProps != null)
                {
                    //mPropertiesGrid.SelectedObject = new kAIPortProperties(lPortProps.GetContents());
                    mPropertiesGrid.SelectedObject = lPortProps.GetSelectedProperty();
                }
                else
                {
                    kAIIClickableProperty lOtherPortProps = lSelectedItem.Value as kAIIClickableProperty;
                    if (lOtherPortProps != null)
                    {
                        //mPropertiesGrid.SelectedObject = new kAIPortProperties(lPortProps.GetContents());
                        mPropertiesGrid.SelectedObject = lOtherPortProps.GetSelectedProperty();
                    }
                }
            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            mPropertiesGrid.Size = flowLayoutPanel1.Size;
        }

        
    }
}
