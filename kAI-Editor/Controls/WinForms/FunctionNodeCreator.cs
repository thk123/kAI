using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.ObjectProperties;
using System.Drawing.Design;


namespace kAI.Editor.Controls.WinForms
{
    partial class FunctionNodeCreator : Form
    {
        MethodInfo mSelectedMethod;

        kAIFunctionNode.kAIFunctionConfiguration mConfig;

        public kAIFunctionNode FunctionNode
        {
            get;
            private set;
        }

        //List<kAIFunctionParameterConfiguration> lParamConfigs;

        public FunctionNodeCreator(kAIProject lProject)
        {
            InitializeComponent();
            //lParamConfigs = new List<kAIFunctionParameterConfiguration>();

            mFunctionList.SetDataSource(lProject.ProjectFunctions);
            mFunctionList.SelectionMode = SelectionMode.One;

            mFunctionList.SelectedValueChange += new EventHandler(mFunctionList_SelectedValueChange);

            //listBox1.SelectedValueChanged += new EventHandler(listBox1_SelectedValueChanged);
        }

        /*void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = listBox1.SelectedItem;
        }*/

        void mFunctionList_SelectedValueChange(object sender, EventArgs e)
        {
            if(mFunctionList.SelectedItem != null)
            {
                MethodInfo lSelectedMethod = mFunctionList.SelectedItem as MethodInfo;

                if (lSelectedMethod != null)
                {
                    SetMethod(lSelectedMethod);
                }
                else
                {
                    throw new Exception("Invalid entry in the function list");
                }
            }
        }

        void SetMethod(MethodInfo lMethod)
        {
            mSelectedMethod = lMethod;
            mConfig = new kAIFunctionNode.kAIFunctionConfiguration(mSelectedMethod);

            mConfig.OnConfigured += new EventHandler(mConfig_OnConfigured);

            int lGenericParamIndex = 0;
            foreach (Type lType in mConfig.GenericTypes)
            {
                // Each combo box corresponds to precisely one generic parameter
                ComboBox lComboBox = new ComboBox();
                lComboBox.Tag = lGenericParamIndex;
                lComboBox.Items.AddRange(GetSuitableTypes(lType));
                flowLayoutPanel1.Controls.Add(lComboBox);
                lComboBox.SelectedValueChanged += new EventHandler(lComboBox_SelectedValueChanged);
                ++lGenericParamIndex;
            }

            int lPropertyIndex = 0;
            foreach(string lPropertyName in mConfig.ReturnConfiguration.PropertyNames)
            {
                CheckBox lCheckbox = new CheckBox();
                lCheckbox.Text = lPropertyName;
                lCheckbox.Checked = mConfig.ReturnConfiguration.GetPropertyState(lPropertyIndex);
                int lStoredProperyIndex = lPropertyIndex;
                lCheckbox.CheckedChanged += (sender, e) =>
                    {
                        CheckBox lClickedCheckbox = sender as CheckBox;
                        mConfig.ReturnConfiguration.SetPropertyState(lStoredProperyIndex, lClickedCheckbox.CheckState == CheckState.Checked);
                    };

                mOutParametersFlow.Controls.Add(lCheckbox);
                ++lPropertyIndex;
            }

            if (mConfig.IsConfigured)
            {
                mConfig_OnConfigured(null, null);
            }

            /*listBox1.Items.Clear();
            lParamConfigs.Clear();

            listBox1.Items.Add(new MethodConfigurationProperties(new kAIFunctionNode.FunctionConfiguration(mSelectedMethod)));

            foreach (ParameterInfo lParam in lMethod.GetParameters())
            {
                if (!lParam.IsOut)
                {
                    listBox1.Items.Add(lParam);
                    lParamConfigs.Add(new kAIFunctionParameterConfiguration(lParam));
                    
                }
            }*/
        }

        void mConfig_OnConfigured(object sender, EventArgs e)
        {
            mCreateBtn.Enabled = true;
        }

        void lComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox lSelected = sender as ComboBox;
            mConfig.SetGenericParameter((int)lSelected.Tag, (Type)lSelected.SelectedItem);
        }

        Type[] GetSuitableTypes(Type lParamType)
        {
            List<Type> lSuitableTypes = new List<Type>();
            foreach (Type lType in PropertiesWindow.Project.ProjectTypes)
            {
                lSuitableTypes.Add(lType);
            }

            return lSuitableTypes.ToArray();
        }

        private void mCreateBtn_Click(object sender, EventArgs e)
        {
            FunctionNode = new kAIFunctionNode(mSelectedMethod, mConfig);
        }
    }

    class TypeChooser : UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            Type lTypeProperty = value as Type;
            if (lTypeProperty != null)
            {
                IWindowsFormsEditorService _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                ListBox lTypeComboBox = new ListBox();
                lTypeComboBox.SelectionMode = SelectionMode.One;
                lTypeComboBox.Click += (lSender, e) => { _editorService.CloseDropDown(); };
                foreach(Type lType in PropertiesWindow.Project.ProjectTypes)
                {
                    lTypeComboBox.Items.Add(lType);
                }
                _editorService.DropDownControl(lTypeComboBox);

                if (lTypeComboBox == null) // no selection, return the passed-in value as is
                {
                    return value;
                }

                return lTypeComboBox.SelectedItem;
            }
            else
            {
                return value;
            }
        }
    }

    class ParameterProperties
    {

        [EditorAttribute(typeof(TypeChooser), typeof(UITypeEditor))]
        public Type Type;
        

        public ParameterProperties(Type lParamConfig)
        {
            Type = lParamConfig;
        }
    }

    class MethodConfigurationProperties
    {
        [TypeConverterAttribute(typeof(kAISimpleExpandableConverter<ParameterProperties>))]
        public ParameterProperties[] Types
        {
            get;
            set;
        }


        public MethodConfigurationProperties(kAIFunctionNode.kAIFunctionConfiguration lFunctionConfig)
        {
            Type[] lGenTypes = lFunctionConfig.GenericTypes;
            Types = new ParameterProperties[lGenTypes.Length];
            for (int i = 0; i < lGenTypes.Length; ++i)
            {
                Types[i] = new ParameterProperties(lGenTypes[i]);
            }
        }
    }
}
