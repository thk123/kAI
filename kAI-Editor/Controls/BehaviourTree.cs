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
        /// A node in a behaviour tree that represents a specific behaviour template. 
        /// </summary>
        class BehaviourNode : TreeNode
        {
            /// <summary>
            /// Handler for when a behaviour node gets instantiated. 
            /// </summary>
            /// <param name="lSender">The tree node that sent the request. </param>
            /// <param name="lTemplate">The template that node represented. </param>
            public delegate void InstantiatedEvent(BehaviourNode lSender, kAIBehaviourTemplate lTemplate);

            /// <summary>
            /// Occurs when the behaviour is instantiated. 
            /// </summary>
            public event InstantiatedEvent OnInstantiation;

            /// <summary>
            /// The template this node represents. 
            /// </summary>
            public kAIBehaviourTemplate TemplateBehaviour
            {
                get;
                private set;
            }

            /// <summary>
            /// Creates node representing the specific template. 
            /// </summary>
            /// <param name="lTemplate">The behaviour template this node represents. </param>
            public BehaviourNode(kAIBehaviourTemplate lTemplate)
                :base(lTemplate.BehaviourName)
            {
                TemplateBehaviour = lTemplate;

                MenuItem lInstantiateMenu = new MenuItem("Instantiate");
                lInstantiateMenu.Click += new EventHandler(lInstantiateMenu_Click);
                ContextMenu = new System.Windows.Forms.ContextMenu();
                ContextMenu.MenuItems.Add(lInstantiateMenu);
            }

            void lInstantiateMenu_Click(object sender, EventArgs e)
            {
                if(OnInstantiation != null)
                    OnInstantiation(this, TemplateBehaviour);
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

            foreach (kAIBehaviourTemplate lTemplate in lProject.Behaviours)
            {
                //TODO: Sort by folders and DLLs
                if (lTemplate.BehaviourFlavour == kAIBehaviourTemplate.eBehaviourFlavour.BehaviourFlavour_Code)
                {
                    lCodeNode.Nodes.Add(new BehaviourNode(lTemplate));
                }
                else
                {
                    lXmlNode.Nodes.Add(new BehaviourNode(lTemplate));
                }
            }
        }
    }
}
