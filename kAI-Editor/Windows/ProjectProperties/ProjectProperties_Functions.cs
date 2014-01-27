using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;

namespace kAI.Editor.Forms.ProjectProperties
{
    partial class ProjectPropertiesForm : Form
    {
        private void SetFunctionsFormFromProject()
        {
            List<MethodInfo> lAllTheMethods = new List<MethodInfo>();
            foreach (Type lType in Project.ProjectTypes)
            {
                lAllTheMethods.AddRange(lType.GetMethods());
            }

            foreach (Assembly lAssembly in Project.ProjectDLLs)
            {
                lAllTheMethods.AddRange(lAssembly.GetExportedTypes().Where((f) => { return f.IsSealed && f.IsAbstract; }).Select((t) =>
                {
                    return t.GetMethods();
                }).Aggregate((a, b) =>
                {
                    return a.Union(b).ToArray();
                }));
            }

            lAllTheMethods.AddRange(typeof(kAIFunctionNodes).GetMethods());


            if (mIsNewProject)
            {
                // Splits all the types in to 2 partitions, adding them to the project if they are a default type
                Tuple<IEnumerable<MethodInfo>, IEnumerable<MethodInfo>> lTypeList = lAllTheMethods.Split(IsDefaultIncludeFunction);

                mIncludedTypesList.Items.AddRange(lTypeList.Item1.ToArray());
                //mAllTypesList.SetSource(lTypeList.Item2);

                foreach (MethodInfo lDefaultType in lTypeList.Item1)
                {
                    MethodInfo lTypeToAdd = lDefaultType;
                    mApplyActions.Enqueue(() => { Project.ProjectFunctions.Add(lTypeToAdd); });
                }
            }
            else
            {
                // Splits all the types into item1 if the project contains the type, item2 if not
                Tuple<IEnumerable<MethodInfo>, IEnumerable<MethodInfo>> lTypeList = lAllTheMethods.Split(
                    (lType) =>
                    {
                        foreach (MethodInfo lExistingType in Project.ProjectFunctions)
                        {
                            if (lExistingType.Name == lType.Name && 
                                lExistingType.DeclaringType.FullName == lType.DeclaringType.FullName)
                            {
                                return true;
                            }
                        }
                        return false;
                    });


                mProjectFunctionsList.Items.AddRange(lTypeList.Item1.Select<MethodInfo, MyMethodInfoDisplay>((lMethod) => { return new MyMethodInfoDisplay(lMethod); }).ToArray());
                mAllFunctionsList.SetDataSource(lTypeList.Item2.Select<MethodInfo, MyMethodInfoDisplay>((lMethod) => { return new MyMethodInfoDisplay(lMethod); }));
            }
        }   

        private void SetProjectFromFunctionsForm()
        {

        }

        private bool IsDefaultIncludeFunction(MethodInfo lMethod)
        {
            return lMethod.DeclaringType == typeof(kAIFunctionNodes);
        }

        private void mFunctionRemoveBtn_Click(object sender, EventArgs e)
        {
            mAllFunctionsList.Items.AddRange(mProjectFunctionsList.SelectedItems.Cast<MyMethodInfoDisplay>().ToArray());
            MyMethodInfoDisplay[] lSelectedTypes = mProjectFunctionsList.SelectedItems.Cast<MyMethodInfoDisplay>().ToArray();
            mProjectFunctionsList.SelectedItems.Clear();
            foreach (MyMethodInfoDisplay lSelectedType in lSelectedTypes)
            {
                mProjectFunctionsList.Items.Remove(lSelectedType);

                // Must capture the correct type
                MethodInfo lRemovedType = lSelectedType.Method;
                mApplyActions.Enqueue(() => { Project.ProjectFunctions.Remove(lRemovedType); });
            }
        }

        private void mFunctionAddBtn(object sender, EventArgs e)
        {
            mProjectFunctionsList.Items.AddRange(mAllFunctionsList.SelectedItems.Cast<MyMethodInfoDisplay>().ToArray());
            MyMethodInfoDisplay[] lSelectedTypes = mAllFunctionsList.SelectedItems.Cast<MyMethodInfoDisplay>().ToArray();
            mAllFunctionsList.SelectedItems.Clear();
            foreach (MyMethodInfoDisplay lSelectedType in lSelectedTypes)
            {
                mAllFunctionsList.Items.Remove(lSelectedType);

                // Must capture the correct type
                MethodInfo lNewlyAddedType = lSelectedType.Method;
                mApplyActions.Enqueue(() => { Project.ProjectFunctions.Add(lNewlyAddedType); });
            }
        }
    }
}
