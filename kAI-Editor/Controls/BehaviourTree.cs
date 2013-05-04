using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor;
using kAI.Editor.Core;


namespace kAI.Editor.Controls
{
    /// <summary>
    /// A tree showing a set of behaviours.
    /// </summary>
    partial class BehaviourTree : UserControl
    {
        /// <summary>
        /// Handler for when a behaviour node gets instantiated. 
        /// </summary>
        /// <param name="lTemplate">The template that node represented. </param>
        public delegate void NodeEvent(kAIINodeSerialObject lTemplate);

        public event NodeEvent OnBehaviourDoubleClick;
        public event NodeEvent OnBehaviourInstantiated;


        /// <summary>
        /// A node in a behaviour tree that represents a specific behaviour template. 
        /// </summary>
        class BehaviourTreeNode : TreeNode
        {
            /// <summary>
            /// Occurs when the behaviour is instantiated. 
            /// </summary>
            public event NodeEvent OnInstantiation;

            /// <summary>
            /// The template this node represents. 
            /// </summary>
            public kAIINodeSerialObject TemplateBehaviour
            {
                get;
                private set;
            }

            /// <summary>
            /// Creates node representing the specific template. 
            /// </summary>
            /// <param name="lTemplate">The behaviour template this node represents. </param>
            public BehaviourTreeNode(kAIINodeSerialObject lTemplate)
                :base(lTemplate.GetFriendlyName())
            {
                TemplateBehaviour = lTemplate;

                MenuItem lInstantiateMenu = new MenuItem("Instantiate");
                lInstantiateMenu.Click += new EventHandler(lInstantiateMenu_Click);
                ContextMenu = new System.Windows.Forms.ContextMenu();
                ContextMenu.MenuItems.Add(lInstantiateMenu);
            }

            // The instantiate button was pressed. 
            void lInstantiateMenu_Click(object sender, EventArgs e)
            {
                if (OnInstantiation != null)
                {
                    OnInstantiation(TemplateBehaviour);
                }
            }
        }

        /// <summary>
        /// Create a behaviour tree for a given project. 
        /// </summary>
        /// <param name="lProject">The project whose behaviours we want to show. </param>
        public BehaviourTree(kAIProject lProject)
        {
            InitializeComponent();

            FillTree(lProject);


            BehaviourTree_Tree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(BehaviourTree_Tree_NodeMouseDoubleClick);
        }


        /// <summary>
        /// When the behaviors have changed, update the tree. 
        /// </summary>
        /// <param name="lProject"></param>
        public void UpdateTree(kAIProject lProject)
        {
            BehaviourTree_Tree.Nodes.Clear();
            FillTree(lProject);
        }

        /// <summary>
        /// Fills an empty tree with the behaviors from a given project. 
        /// </summary>
        /// <param name="lProject">The project to get the behaviours from. </param>
        private void FillTree(kAIProject lProject)
        {
            TreeNode lRootNode = BehaviourTree_Tree.Nodes.Add("Behaviours");

            TreeNode lXmlNode = lRootNode.Nodes.Add("Xml Behaviours");
            TreeNode lCodeNode = lRootNode.Nodes.Add("Code Behaviours");

            foreach (kAIINodeSerialObject lSerialNode in lProject.NodeObjects.Values)
            {
                //TODO: Sort by folders and DLLs

                BehaviourTreeNode lNewNode = new BehaviourTreeNode(lSerialNode);
                switch (lSerialNode.GetNodeFlavour())
                {
                    case eNodeFlavour.BehaviourXml:
                        lXmlNode.Nodes.Add(lNewNode);
                        break;
                    case eNodeFlavour.BehaviourCode:
                        lCodeNode.Nodes.Add(lNewNode);
                        break;
                    case eNodeFlavour.UnknownType:
                        break;
                    default:
                        break;
                }

                lNewNode.OnInstantiation += new NodeEvent(lNewNode_OnInstantiation);
            }
        }

        // Propagate the behaviour instantiated event out. 
        void lNewNode_OnInstantiation(kAIINodeSerialObject lTemplate)
        {
            if (OnBehaviourInstantiated != null)
            {
                OnBehaviourInstantiated(lTemplate);
            }
        }


        // A node was double clicked, so open it. 
        void BehaviourTree_Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            BehaviourTreeNode lSelectedNode = e.Node as BehaviourTreeNode;
            if (lSelectedNode != null)
            {
                if (OnBehaviourDoubleClick != null)
                {
                    OnBehaviourDoubleClick(lSelectedNode.TemplateBehaviour);
                }
            }
        }
    }
}
