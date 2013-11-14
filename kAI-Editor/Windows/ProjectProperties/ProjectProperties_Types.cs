using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;
using System.Reflection;
using kAI.Editor.Controls;
using kAI.Editor.Core.Util;

namespace kAI.Editor.Forms.ProjectProperties
{
    partial class ProjectPropertiesForm : Form
    {
        Queue<Action> mApplyActions;

        string mSearchTerm;

        /// <summary>
        /// Sets the controls on the types tab from the project properties. 
        /// </summary>
        private void SetTypesFormFromProject()
        {
            mSearchTerm = String.Empty;

            List<Type> lAllTypes = new List<Type>();
            foreach (Assembly lRefdAssembly in Project.ProjectDLLs)
            {
                lAllTypes.AddRange(lRefdAssembly.GetExportedTypes().Where(CheckType));

                foreach (Assembly lExecRefdAssembly in lRefdAssembly.GetReferencedAssemblies().Select<AssemblyName, Assembly>((lName) =>
                {
                    return Project.GetAssemblyByName(lName.Name);
                }))
                {
                    if (lExecRefdAssembly != null)
                    {
                        lAllTypes.AddRange(lExecRefdAssembly.GetExportedTypes().Where(CheckType));
                    }
                }
            }

            //lAllTypes.AddRange(Assembly.GetExecutingAssembly().GetExportedTypes().Where(CheckType));

            /*foreach (Assembly lExecRefdAssembly in Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select<AssemblyName, Assembly>((lName) =>
                {
                    return Project.GetAssemblyByName(lName.Name);
                }))
            {
                lAllTypes.AddRange(lExecRefdAssembly.GetExportedTypes().Where(CheckType));
            }*/
            
            // Splits all the types into item1 if the project contains the type, item2 if not
            Tuple<IEnumerable<Type>, IEnumerable<Type>> lTypeList = lAllTypes.Split((lType) => { return Project.ProjectTypes.Contains(lType); });

            mIncludedTypesList.Items.AddRange(lTypeList.Item1.ToArray());
            mAllTypesList.SetSource(lTypeList.Item2);
            

            
            mApplyActions = new Queue<Action>();
        }

        private bool CheckType(Type lType)
        {
            bool lAllowed = !lType.IsGenericType; // disallow generic types

            lAllowed &= !lType.IsAbstract;
            lAllowed &= !lType.IsInterface;
            lAllowed &= !lType.IsNested;

            return lAllowed;
        }

        /// <summary>
        /// Sets the project properties from the settings on the types screen. 
        /// </summary>
        private void SetProjectFromTypesForm()
        {
            //TODO: probably not required but for consistency should. 

            foreach(Action lAction in mApplyActions)
            {
                lAction();
            }
        }

        private void mAddBtn_Click(object sender, EventArgs e)
        {
            mIncludedTypesList.Items.AddRange(mAllTypesList.SelectedItems.Cast<Type>().ToArray());
            Type[] lSelectedTypes = mAllTypesList.SelectedItems.Cast<Type>().ToArray();
            mAllTypesList.SelectedItems.Clear();
            foreach (Type lSelectedType in lSelectedTypes)
            {
                mAllTypesList.Items.Remove(lSelectedType);
                mApplyActions.Enqueue(() => { Project.ProjectTypes.Add(lSelectedType); });
            }
            
        }

        private void mRemoveBtn_Click(object sender, EventArgs e)
        {
            mAllTypesList.Items.AddRange(mIncludedTypesList.SelectedItems.Cast<Type>().ToArray());
            Type[] lSelectedTypes = mIncludedTypesList.SelectedItems.Cast<Type>().ToArray();
            mIncludedTypesList.SelectedItems.Clear();
            foreach (Type lSelectedType in lSelectedTypes)
            {
                mIncludedTypesList.Items.Remove(lSelectedType);
                mApplyActions.Enqueue(() => { Project.ProjectTypes.Remove(lSelectedType); });
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            mAllTypesList.UpdateSearchTerm(mTypeSearchBoxBtn.Text);
        }
    }
}
