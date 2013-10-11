using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Reflection;
using System.Xml;
using System.IO;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using kAI.Editor.Forms;
using kAI.Editor.Forms.ProjectProperties;
using kAI.Editor.Controls;
using kAI.Editor.Controls.DX;




namespace kAI.Editor
{
    /// <summary>
    /// The general form for creating kAI Behaviours
    /// </summary>
    public partial class Editor : Form
    {
        bool mIsProjectLoaded;
        kAIProject mLoadedProject;

        BehaviourTree mBehaviourTree;

        List<PropertyControllerBase<bool>> mProjectLoadedControls; // Controls that should only be enabled when a project is loaded.
        List<PropertyControllerBase<bool>> mBehaviourLoadedControls;

        kAIBehaviourEditorWindow mBehaviourEditor;

        /// <summary>
        /// Create a new editor. 
        /// </summary>
        public Editor()
        {
            InitializeComponent();

            mIsProjectLoaded = false;
            mLoadedProject = null;

            mProjectLoadedControls = new List<PropertyControllerBase<bool>>();
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(projectPropertiesToolStripMenuItem));
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(closeProjectToolStripMenuItem));
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(createNewXmlBehaviourToolStripMenuItem));
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(saveProjectToolStripMenuItem));

            mBehaviourLoadedControls = new List<PropertyControllerBase<bool>>();
            mBehaviourLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(addBehaviourToolStripMenuItem));
            mBehaviourLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(saveToolStripMenuItem));

            // Disable all the project specific controls);
            SetEnabledSetControls(mProjectLoadedControls, false);
            SetEnabledSetControls(mBehaviourLoadedControls, false);

            mBehaviourEditor = null;
        }

        /// <summary>
        /// If we have an XML behaviour loaded, we update the DX Editor window
        /// </summary>
        public void RenderUpdate()
        {
            if(mBehaviourEditor != null)
            {
                mBehaviourEditor.Update();
            }
        }

        /// <summary>
        /// Load a given project in to the editor. 
        /// </summary>
        /// <param name="lProject"></param>
        private void LoadProject(kAIProject lProject)
        {
            mLoadedProject = lProject;
            mIsProjectLoaded = true;

            // Remove all old controls from the right hand window
            MainEditor.Panel1.Controls.Clear();

            SetEnabledSetControls(mProjectLoadedControls, true);

            mBehaviourTree = new BehaviourTree(lProject);
            mBehaviourTree.Dock = DockStyle.Fill;
            mBehaviourTree.OnBehaviourDoubleClick += new BehaviourTree.NodeEvent(mBehaviourTree_OnBehaviourDoubleClick);
            MainEditor.Panel1.Controls.Add(mBehaviourTree);
        }

        void mBehaviourTree_OnBehaviourDoubleClick(kAIINodeSerialObject lObject)
        {
            // If it was a kAI-Behaviour, we load it.
            if (lObject.GetNodeFlavour() == eNodeFlavour.BehaviourXml)
            {
                LoadBehaviour(kAIXmlBehaviour.Load(lObject, mLoadedProject.GetAssemblyByName));
            }
            
        }

        private void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            CreateBehaviourEditorWindow();
            mBehaviourEditor.LoadBehaviour(lBehaviour);
            SetEnabledSetControls(mBehaviourLoadedControls, true);
        }

        /// <summary>
        /// Close the currently open project. 
        /// </summary>
        private void CloseProject()
        {
            System.Diagnostics.Debug.Assert(mLoadedProject != null);

            DestroyBehaviourEditorWindow();

            mLoadedProject = null;

            SetEnabledSetControls(mBehaviourLoadedControls, false);
            SetEnabledSetControls(mProjectLoadedControls, false);
        }

        /// <summary>
        /// Either enable or disable all the project controls (eg controls that only make sense when a project is open). 
        /// </summary>
        /// <param name="lSetOfControls">The set of controls to enable or disable.</param>
        /// <param name="lEnabled">Should the project controls be enabled. </param>
        private void SetEnabledSetControls(List<PropertyControllerBase<bool>> lSetOfControls, bool lEnabled)
        {
            foreach (var lControls in lSetOfControls)
            {
                lControls.SetProperty(lEnabled);
            }
        }

        private void CreateBehaviourEditorWindow()
        {
            if (mBehaviourEditor == null)
            {
                // Here we chose our implemenation for the editor. 
                //kAIIBehaviourEditorGraphicalImplementator lImpl = new BehaviourEditorWindowWinForms();
                kAIIBehaviourEditorGraphicalImplementator lImpl = new kAIBehaviourEditorWindowDX();

                mBehaviourEditor = new kAIBehaviourEditorWindow(mLoadedProject, lImpl);
                mBehaviourEditor.Init(MainEditor.Panel2);
            }
        }

        private void DestroyBehaviourEditorWindow()
        {
            if (mBehaviourEditor != null)
            {
                mBehaviourEditor.Destroy();
            }

            SetEnabledSetControls(mBehaviourLoadedControls, false);
        }

        void addBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mIsProjectLoaded)
            {
                BehaviourChooser lChooser = new BehaviourChooser(mLoadedProject);
                if (lChooser.ShowDialog() == DialogResult.OK)
                {
                    // TODO: Add a behaviour to the behaviour editor DX
                    //mBehaviourEditor.AddBehaviour(lChooser.GetSelectedBehaviour());
                }
            }
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectPropertiesForm lNewProjectPropertiesViewer = new ProjectPropertiesForm(mLoadedProject);

            // The user has decided to create a new project
            if (lNewProjectPropertiesViewer.ShowDialog() == DialogResult.OK)
            {
                LoadProject(lNewProjectPropertiesViewer.Project);
            }
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog lOpenDialog = new OpenFileDialog();
            lOpenDialog.Filter = "kAI Project Files |*." + kAIProject.kProjectFileExtension;
            if (lOpenDialog.ShowDialog() == DialogResult.OK)
            {
                kAIProject lProject = kAIProject.Load(new FileInfo(lOpenDialog.FileName));

                LoadProject(lProject);
            }
        }

        private void projectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectPropertiesForm lEditProject = new ProjectPropertiesForm(mLoadedProject);
            lEditProject.ShowDialog();

            mBehaviourTree.UpdateTree(mLoadedProject);
        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseProject();
        }

        private void createNewXmlBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: should get a default name from the project. 
            XmlBehaviourCreator lCreator = new XmlBehaviourCreator(mLoadedProject, "New Behaviour");

            if(lCreator.ShowDialog() == DialogResult.OK)
            {
                CreateBehaviourEditorWindow();

                //kAIXmlBehaviour lBehaviour = 
                
                kAIXmlBehaviour lBehaviour = new kAIXmlBehaviour(lCreator.BehaviourID, lCreator.BehaviourPath);
                LoadBehaviour(lBehaviour);


                /*kAIXmlBehaviour lBehaviour = mBehaviourEditor.NewBehaviour();

                mLoadedProject.AddXmlBehaviour(lBehaviour.GetDataContractClass());

                mBehaviourTree.UpdateTree(mLoadedProject);*/
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mBehaviourEditor.SaveBehaviour();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog lOpenDialog = new OpenFileDialog();
            lOpenDialog.Filter = "XML Behaviours|*" + kAIXmlBehaviour.kAIXmlBehaviourExtension;

            if (lOpenDialog.ShowDialog() == DialogResult.OK)
            {


                //TODO: Add existing behaviour and hence create a behaviour from a FileInfo. 
                /*kAIXmlBehaviour lBehaviour = kAIXmlBehaviour.Load(new FileInfo(lOpenDialog.FileName), (s) =>
                {
                    if (s == Assembly.GetExecutingAssembly().FullName)
                    {
                        return Assembly.GetExecutingAssembly();
                    }
                    else
                    {
                        foreach(AssemblyName lAssemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                        {
                            if (lAssemblyName.FullName == s)
                            {
                                Assembly lAssembly = Assembly.Load(lAssemblyName);
                                if (lAssembly != null)
                                {
                                    return lAssembly;
                                }
                            }
                        }


                        return mLoadedProject.ProjectDLLs.Find((lAssembly) =>
                            {
                                return lAssembly.FullName == s;
                            });
                    }
                });

                LoadBehaviour(null);

                mBehaviourEditor.LoadBehaviour(lBehaviour);*/
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mLoadedProject.Save();
        }

        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyBehaviourEditorWindow();
        }
    }
}
