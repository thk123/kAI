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
using kAI.Editor.Controls.WinForms;




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

        PropertiesWindow mPropertiesWindow;

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
            mProjectLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(showPropertiesGridToolStripMenuItem));

            mBehaviourLoadedControls = new List<PropertyControllerBase<bool>>();
            mBehaviourLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(addBehaviourToolStripMenuItem));
            mBehaviourLoadedControls.Add(PropertyController.CreateForEnabledToolStrip(saveToolStripMenuItem));
            mBehaviourLoadedControls.Add(PropertyController.CreateForEnabledControl(uiLogger1.mCmdTextbox));

            // Disable all the project specific controls);
            SetEnabledSetControls(mProjectLoadedControls, false);
            SetEnabledSetControls(mBehaviourLoadedControls, false);

            mBehaviourEditor = null;
            mPropertiesWindow = null;

            kAIObject.GlobalLogger = uiLogger1;
            kAIObject.GlobalLogger.LogMessage("kAI Editor loaded");
            GlobalServices.Logger = uiLogger1;
            GlobalServices.Editor = this;

            CommandLineHandler.TakeActions();
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
        void LoadProject(kAIProject lProject)
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

            GlobalServices.LoadedProject = mLoadedProject;
        }

        private void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            CreateBehaviourEditorWindow();
            mBehaviourEditor.LoadBehaviour(lBehaviour);
            SetEnabledSetControls(mBehaviourLoadedControls, true);

            kAIInteractionTerminal.Init(lBehaviour);
        }

        private void UnloadBehaviour()
        {
            mBehaviourEditor.UnloadBehaviour();
            DestroyBehaviourEditorWindow();
            SetEnabledSetControls(mBehaviourLoadedControls, false);

            kAIInteractionTerminal.Deinit();
        }

        /// <summary>
        /// Close the currently open project. 
        /// </summary>
        private void CloseProject()
        {
            System.Diagnostics.Debug.Assert(mLoadedProject != null);

            UnloadBehaviour();

            MainEditor.Panel1.Controls.Clear();

            SetEnabledSetControls(mProjectLoadedControls, false);

            mLoadedProject = null;
            GlobalServices.LoadedProject = null;
        }

        void mBehaviourTree_OnBehaviourDoubleClick(kAIINodeSerialObject lObject)
        {
            // If it was a kAI-Behaviour, we load it.
            if (lObject.GetNodeFlavour() == eNodeFlavour.BehaviourXml)
            {
                LoadBehaviour(kAIXmlBehaviour.Load(lObject, mLoadedProject.GetAssemblyByName));
            }
            
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

                mBehaviourEditor = new kAIBehaviourEditorWindow(mLoadedProject, lImpl, this);

                mBehaviourEditor.ObjectSelected += new Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry>(mBehaviourEditor_ObjectSelected);

                Control lEditorControl = new Control();
                lEditorControl.Dock = DockStyle.Fill;
                splitContainer1.Panel1.Controls.Add(lEditorControl);
                mBehaviourEditor.Init(lEditorControl);
                
                CreatePropertiesWindow();
            }
        }

        private void CreatePropertiesWindow()
        {
            if (mPropertiesWindow == null)
            {
                mPropertiesWindow = new PropertiesWindow(mLoadedProject);
                mPropertiesWindow.Show(splitContainer1.Panel1);
                mPropertiesWindow.Disposed += new EventHandler(mPropertiesWindow_Disposed);
            }
        }

        void mBehaviourEditor_ObjectSelected(kAI.Editor.ObjectProperties.kAIIPropertyEntry lSelectedObject)
        {
            if (mPropertiesWindow != null)
            {
                mPropertiesWindow.SelectObject(lSelectedObject);
            }
        }

        private void DestroyBehaviourEditorWindow()
        {
            if (mBehaviourEditor != null)
            {
                mBehaviourEditor.Destroy();
                mBehaviourEditor = null;
                splitContainer1.Panel1.Controls.Clear();
            }

            SetEnabledSetControls(mBehaviourLoadedControls, false);
        }

        void addBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(mIsProjectLoaded)
            {

                kAIINodeObject lSelectedNode = SelectNode();
                mBehaviourEditor.AddNode(lSelectedNode, mBehaviourEditor.GetPositionForNode());
            }
        }

        /// <summary>
        /// Presents a dialog for the user to choose what kind of node to create. 
        /// </summary>
        /// <returns>A node object that the user wants to instantiate into a node. </returns>
        public kAIINodeObject SelectNode()
        {
            kAIObject.Assert(null, mIsProjectLoaded, "No loaded project to choose from");
            kAINodeChooser lChooser = new kAINodeChooser(mLoadedProject);
            if (lChooser.ShowDialog() == DialogResult.OK)
            {
                return lChooser.GetSelectedBehaviour();
            }
            else
            {
                return null;
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

        public void LoadProject(FileInfo lProjectFile)
        {
            if (!lProjectFile.Exists)
            {
                GlobalServices.Logger.LogError("Could not load project - file not found", new KeyValuePair<string, object>("Project Location", lProjectFile.FullName));
                return;
            }

            kAIBehaviourID lBehaviourToLoad;
            kAIProject lProject = kAIProject.Load(lProjectFile, out lBehaviourToLoad);

            LoadProject(lProject);

            if (lBehaviourToLoad != null)
            {
                try
                {
                    kAIINodeSerialObject lNodeObject = lProject.NodeObjects[lBehaviourToLoad];

                    if (lNodeObject.GetNodeFlavour() == eNodeFlavour.BehaviourXml)
                    {
                        LoadBehaviour(kAIXmlBehaviour.Load(lNodeObject, lProject.GetAssemblyByName));
                    }
                    else
                    {
                        kAIObject.GlobalLogger.LogWarning("The last loaded behaviour is now somehow not an XML behaviour. ");
                    }
                }
                catch (KeyNotFoundException)
                {
                    kAIObject.GlobalLogger.LogWarning("Could not find behaviour that was loaded. ");
                }
            }
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog lOpenDialog = new OpenFileDialog();
            lOpenDialog.Filter = "kAI Project Files |*." + kAIProject.kProjectFileExtension;
            if (lOpenDialog.ShowDialog() == DialogResult.OK)
            {
                LoadProject(new FileInfo(lOpenDialog.FileName));
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
            XmlBehaviourPropertiesEditor lCreator = new XmlBehaviourPropertiesEditor(mLoadedProject, null);

            if(lCreator.ShowDialog() == DialogResult.OK)
            {
                CreateBehaviourEditorWindow();

                kAIXmlBehaviour lBehaviour = lCreator.Behaviour;
                mLoadedProject.AddXmlBehaviour(lBehaviour.GetDataContractClass(null));

                mBehaviourTree.UpdateTree(mLoadedProject);

                LoadBehaviour(lBehaviour);
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

            mLoadedProject.Save(mBehaviourEditor.Behaviour.BehaviourID);
        }

        private void Editor_FormClosed(object sender, FormClosedEventArgs e)
        {
            DestroyBehaviourEditorWindow();
        }

        private void runCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool lResult;
            do 
            {
                lResult = kAIInteractionTerminal.RunCommand(Console.ReadLine());

            } while (!lResult);
            
        }

        private void behaviourPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlBehaviourPropertiesEditor lPropertiesEditor = new XmlBehaviourPropertiesEditor(mLoadedProject, mBehaviourEditor.Behaviour);
            DialogResult lResult = lPropertiesEditor.ShowDialog();

        }

        private void showPropertiesGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreatePropertiesWindow();
        }

        void mPropertiesWindow_Disposed(object sender, EventArgs e)
        {
            mPropertiesWindow.Disposed -= mPropertiesWindow_Disposed;
            mPropertiesWindow = null;
        }
    }
}
