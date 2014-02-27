using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using kAI.Editor.Core;
using kAI.Core.Debug;
using Winterdom.IO.FileMap;
using kAI.Core;

namespace kAI.Editor.Controls.WinForms
{
    public partial class DebugControl : UserControl
    {
        kAIDebugger mDebugger;
        public DebugControl()
        {
            InitializeComponent();
            SetTextboxSize();
            mCurrentEntry = null;
            mNodeNest = new List<kAINodeID>();
            
        }

        kAIBehaviourEntry? mCurrentEntry;
        List<kAINodeID> mNodeNest;

        public void SetSelectedDebugInfo(kAIBehaviourEntry lSelectedEntry)
        {
            mNodeNest.Clear();

            mCurrentEntry = lSelectedEntry;
        }

        public void PushChildNode(kAINodeID lNodeId)
        {
            mNodeNest.Add(lNodeId);
        }

        private void connectDebugBtn_Click(object sender, EventArgs e)
        {
            try
            {
                mDebugger = new kAIDebugger("TODO");
            }
            catch (Winterdom.IO.FileMap.FileMapIOException ex)
            {
                GlobalServices.Logger.LogError("Could not open main file, ensure the game is running");
                return;
            }

            if (GlobalServices.LoadedProject != null)
            {
                try
                {
                    IEnumerable<kAIBehaviourEntry> entries = mDebugger.GetAvaliableBehaviours();
                    foreach (kAIBehaviourEntry entry in entries)
                    {
                        BehaviourEntryNode entityNode = new BehaviourEntryNode(entry);
                        entityNode.AddChildren(mDebugger);
                        treeView1.Nodes.Add(entityNode);

                    }

                    treeView1.Enabled = true;
                    treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);

                    disconnectDebugBtn.Enabled = true;

                    GlobalServices.BehaviourComposor.OnUpdate += new Action(BehaviourComposor_OnUpdate);
                }
                catch (ThreadMessaging.SemaphoreFailedException)
                {
                    GlobalServices.Logger.LogError("Error locking semaphore");
                }
                catch (ObjectDisposedException)
                {
                    GlobalServices.Logger.LogError("Behaviour debug file missing, ensure the game is running");
                }
                catch (FileMapIOException)
                {
                    GlobalServices.Logger.LogError("Error reading debug file");
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    GlobalServices.Logger.LogError("Error deserialisng debug info, ensure versions match");
                }

            }
            else
            {
                GlobalServices.Logger.LogError("Could not connect debugger - no loaded projects");
            }
        }

        void BehaviourComposor_OnUpdate()
        {
            if (mCurrentEntry != null)
            {
                

                kAIXmlBehaviourDebugInfo lDebugInfo = mDebugger.LoadEntry(mCurrentEntry.Value);
                kAIXmlBehaviourDebugInfo lActualDebugInfo = lDebugInfo;
                foreach (kAINodeID lNodeID in mNodeNest)
                {
                    lActualDebugInfo = lDebugInfo.InternalNodes.Find((lNode) => { return lNode.NodeID == lNodeID; }).Contents as kAIXmlBehaviourDebugInfo;
                }

                if (GlobalServices.BehaviourComposor.Behaviour.BehaviourID != lActualDebugInfo.BehaviourID)
                {
                    kAIINodeSerialObject lXmlBehaviour = GlobalServices.LoadedProject.NodeObjects[lActualDebugInfo.BehaviourID];
                    GlobalServices.BehaviourComposor.LoadBehaviour(kAIXmlBehaviour.Load(lXmlBehaviour, GlobalServices.LoadedProject.GetAssemblyByName));
                }

                GlobalServices.BehaviourComposor.ApplyDebugInfo(lActualDebugInfo);
            }
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DebugTreeNode selectedNode = (DebugTreeNode)e.Node;
            selectedNode.OnClick(this, GlobalServices.BehaviourComposor);
        }

        private void disconnectDebugBtn_Click(object sender, EventArgs e)
        {
            mDebugger.Dispose();
            GlobalServices.BehaviourComposor.ClearDebugInfo();
            mDebugger = null;
            mCurrentEntry = null;
            treeView1.Nodes.Clear();
        }

        private void toolStrip1_Resize(object sender, EventArgs e)
        {
            SetTextboxSize();    
        }

        void SetTextboxSize()
        {
            int nonTextboxWidth = 0;
            foreach (ToolStripItem lItem in toolStrip1.Items)
            {
                if (lItem.IsOnOverflow)
                    continue;

                if (lItem is ToolStripTextBox)
                {
                    nonTextboxWidth += lItem.Margin.Horizontal;
                    continue;
                }

                nonTextboxWidth += lItem.Width + lItem.Margin.Horizontal;
            }

            toolStripTextBox1.Size =new System.Drawing.Size(toolStrip1.Width - nonTextboxWidth, toolStripTextBox1.Height);
        }
    }

    abstract class DebugTreeNode : TreeNode
    {
        public DebugTreeNode(string lDisplayString)
            : base(lDisplayString)
        {

        }

        public abstract void OnClick(DebugControl control, kAIBehaviourEditorWindow editor);
    }

    class BehaviourEntryNode : DebugTreeNode
    {
        public kAIBehaviourEntry Entry
        {
            get;
            private set;
        }

        public BehaviourEntryNode(kAIBehaviourEntry lEntry)
            :base(lEntry.EntryID)
        {
            Entry = lEntry;
        }

        public override string ToString()
        {
            return Entry.EntryID;
        }

        public void AddChildren(kAIDebugger lDebugger)
        {
            kAIXmlBehaviourDebugInfo lDebugInfo = lDebugger.LoadEntry(Entry);
            foreach (kAINodeDebugInfo lEntry in lDebugInfo.InternalNodes)
            {
                NodeEntryNode lNewNode = new NodeEntryNode(lEntry);
                lNewNode.AddChildren(lDebugger);
                Nodes.Add(lNewNode);
            }
        }

        public override void OnClick(DebugControl control, kAIBehaviourEditorWindow editor)
        {
            control.SetSelectedDebugInfo(Entry);
        }
    }

    class NodeEntryNode : DebugTreeNode 
    {
        kAINodeDebugInfo mInfo;
        bool mIsXmlNode;


        public NodeEntryNode(kAINodeDebugInfo lInfo)
            :base(lInfo.NodeID)
        {
            mInfo = lInfo;
            mIsXmlNode = mInfo.Contents is kAIXmlBehaviourDebugInfo;
        }

        public override string ToString()
        {
            return mInfo.NodeID;
        }

        public override void OnClick(DebugControl control, kAIBehaviourEditorWindow editor)
        {
            DebugTreeNode parentNode = this;
            BehaviourEntryNode lRootNode = null;
            Stack<kAINodeID> lNodes = new Stack<kAINodeID>();
            do 
            {
                NodeEntryNode lCurrentNode = (NodeEntryNode)parentNode;
                lNodes.Push(lCurrentNode.mInfo.NodeID);
                lRootNode = parentNode.Parent as BehaviourEntryNode;

            } while (lRootNode == null);

            control.SetSelectedDebugInfo(lRootNode.Entry);

            foreach (kAINodeID lNodeId in lNodes)
            {
                control.PushChildNode(lNodeId);
            }

            
        }

        internal void AddChildren(kAIDebugger lDebugger)
        {
            
            if(mIsXmlNode)
            {
                kAIXmlBehaviourDebugInfo lContentsDebug = (kAIXmlBehaviourDebugInfo)mInfo.Contents;

                foreach (kAINodeDebugInfo lChildDebugInfo in lContentsDebug.InternalNodes)
                {
                    NodeEntryNode lNewNode = new NodeEntryNode(lChildDebugInfo);
                    lNewNode.AddChildren(lDebugger);
                    Nodes.Add(lNewNode);
                }
            }
        }
    }
}
