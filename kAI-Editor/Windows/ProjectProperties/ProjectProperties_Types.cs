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
            mApplyActions = new Queue<Action>();

            List<Type> lAllTypes = new List<Type>();
            foreach (Assembly lRefdAssembly in Project.ProjectDLLs)
            {
                lAllTypes.AddRange(lRefdAssembly.GetExportedTypes().Where(CheckType));
            }

            // We also include mscorlib for basic types
            Assembly lCoreAssembly = Project.GetAssemblyByName("mscorlib");
            lAllTypes.AddRange(lCoreAssembly.GetExportedTypes().Where(CheckType));
            
            

            // new project so we add the default stuff 
            if (mIsNewProject)
            {
                // Splits all the types in to 2 partitions, adding them to the project if they are a default type
                Tuple<IEnumerable<Type>, IEnumerable<Type>> lTypeList = lAllTypes.Split(IsDefaultIncludeType);

                mIncludedTypesList.Items.AddRange(lTypeList.Item1.ToArray());
                //mAllTypesList.SetSource(lTypeList.Item2);

                foreach (Type lDefaultType in lTypeList.Item1)
                {
                    Type lTypeToAdd = lDefaultType;
                    mApplyActions.Enqueue(() => { Project.ProjectTypes.Add(lTypeToAdd); });
                }
            }
            else
            {
                // Splits all the types into item1 if the project contains the type, item2 if not
                Tuple<IEnumerable<Type>, IEnumerable<Type>> lTypeList = lAllTypes.Split(
                    (lType) => 
                    {
                        foreach (Type lExistingType in Project.ProjectTypes)
                        {
                            if (lExistingType.FullName == lType.FullName)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                

                mIncludedTypesList.Items.AddRange(lTypeList.Item1.ToArray());
                mAllTypesList.SetDataSource(lTypeList.Item2);
            }
        }

        private bool IsDefaultIncludeType(Type lType)
        {
            bool lIsDefaultType = lType == typeof(int);
            lIsDefaultType |= lType == typeof(uint);
            lIsDefaultType |= lType == typeof(float);
            lIsDefaultType |= lType == typeof(double);
            lIsDefaultType |= lType == typeof(string);
            lIsDefaultType |= lType == typeof(char);

            return lIsDefaultType;
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

                // Must capture the correct type
                Type lNewlyAddedType = lSelectedType;
                mApplyActions.Enqueue(() => { Project.ProjectTypes.Add(lNewlyAddedType); });
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

                // Must capture the correct type
                Type lRemovedType = lSelectedType;
                mApplyActions.Enqueue(() => { Project.ProjectTypes.Remove(lRemovedType); });
            }
        }


    }
}
