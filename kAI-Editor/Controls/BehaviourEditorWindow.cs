using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;
using System.Drawing;
using kAI.Editor.Controls.DX.Coordinates;

namespace kAI.Editor.Controls
{
    interface kAIIBehaviourEditorGraphicalImplementator
    {
        void Init(Control lParentControl, kAIBehaviourEditorWindow lWindow);
        void UnloadBehaviour();

        void EditorUpdate();
        void Destroy();

        void AddNode(kAINode lNode, Point lPoint);
        void AddNode(kAINode lNode, kAIAbsolutePosition lPoint);
        void RemoveNode(kAINode lNode);

        void AddConnexion(kAIPort.kAIConnexion lConnexion);
        void RemoveConnexion(kAIPort.kAIConnexion lConnexion);

        void AddInternalPort(kAIPort lPort);
        void RemoveInternalPort(kAIPort lPort);

        void AddExternalPort(kAINode lParentNode, kAIPort lPort);
        void RemoveExternalPort(kAINode lParentNode, kAIPort lPort);

        bool CanConnect();
    }

    class kAIBehaviourEditorWindow
    {
        kAIIBehaviourEditorGraphicalImplementator mEditorImpl; 
        

        FileInfo mBehaviourLocation;

        kAIProject mProject;

        Point mMousePositionOnContext;

        Editor mEditor;

        static Random sRandom = new Random();

        public ContextMenu GlobalContextMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// The behaviour currently being shown in this editor. 
        /// </summary>
        public kAIXmlBehaviour Behaviour
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a behaviour editor window using the specified implementation
        /// </summary>
        /// <param name="lProject">The project we are loading.</param>
        /// <param name="lEditorImpl">The implementation to use as the renderer. </param>
        /// <param name="lEditor">The editor that owns this behaviour editor. </param>
        public kAIBehaviourEditorWindow(kAIProject lProject, kAIIBehaviourEditorGraphicalImplementator lEditorImpl, Editor lEditor)
        {
            mProject = lProject;

            mEditor = lEditor;
            mEditorImpl = lEditorImpl;

            MenuItem lAddNodeMenuItem = new MenuItem("Add Node...");
            lAddNodeMenuItem.Click += new EventHandler(lAddNodeMenuItem_Click);            

            GlobalContextMenu = new ContextMenu(new MenuItem[] {
                lAddNodeMenuItem
            });

            GlobalContextMenu.Popup += new EventHandler(GlobalContextMenu_Popup);
        }

        /// <summary>
        /// Initalise the behaviour composer inside the given control. 
        /// </summary>
        /// <param name="lContainer"></param>
        public void Init(Control lContainer)
        {
            mEditorImpl.Init(lContainer, this);

            UnloadBehaviour();
        }

        /// <summary>
        /// Add a connexion to the loaded behaviour. 
        /// </summary>
        /// <param name="lStartPort">The start port of the connexion. </param>
        /// <param name="lEndPort">The end port of the connexion. </param>
        public void AddConnexion(kAIPort lStartPort, kAIPort lEndPort)
        {
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            Behaviour.AddConnexion(lStartPort, lEndPort);

            mEditorImpl.AddConnexion(new kAIPort.kAIConnexion(lStartPort, lEndPort));
        }

        public void RemoveConnexion(kAIPort lStartPort, kAIPort lEndPort)
        {
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            lStartPort.BreakConnexion(lEndPort);

            mEditorImpl.RemoveConnexion(new kAIPort.kAIConnexion(lStartPort, lEndPort));
        }

        public void RemoveConnexion(kAIPort.kAIConnexion lConnexion)
        {
            RemoveConnexion(lConnexion.StartPort, lConnexion.EndPort);
        }

        /// <summary>
        /// Load an existing XML behaviour in to the editor. 
        /// </summary>
        /// <param name="lBehaviour">The XML behaviour to load. </param>
        public void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            if (Behaviour != null)
            {
                UnloadBehaviour();
            }

            foreach (kAIPort lGlobalPort in lBehaviour.InternalPorts)
            {
                mEditorImpl.AddInternalPort(lGlobalPort);
            }            

            foreach (kAINode lInternalNode in lBehaviour.InternalNodes)
            {
                mEditorImpl.AddNode(lInternalNode, GetPositionForNode());
            }

            Behaviour = lBehaviour;

            Behaviour.ForceActivation();
        }

        /// <summary>
        /// Generate a position for the next node to be added if not specified. 
        /// </summary>
        /// <returns>An absolute position for the node to start at. </returns>
        public kAIAbsolutePosition GetPositionForNode()
        {
            return new kAIAbsolutePosition(-250 + sRandom.Next(500), -250 + sRandom.Next(500), false);
        }

        /// <summary>
        /// Add an internal port to this node. 
        /// </summary>
        /// <param name="lInternalPort"></param>
        public void AddInternalPort(kAIPort lInternalPort)
        {
            // TODO: This is confusing...
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            Behaviour.AddExternalPort(lInternalPort);

            mEditorImpl.AddInternalPort(lInternalPort);
        }

        /// <summary>
        /// Add a node to the loaded behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        /// <param name="lPoint">The absolute position to add the point to. </param>
        public void AddNode(kAINode lNode, kAIAbsolutePosition lPoint)
        {
            // TODO: work out if needed?
            // TODO: come back when done generic classes for other controls
            // Maybe should be in the impl any way
            //mNodes.Add(lNode);

            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            Behaviour.AddNode(lNode);

            mEditorImpl.AddNode(lNode, lPoint);
        }

        /// <summary>
        /// Add a node to the loaded behaviour. 
        /// </summary>
        /// <param name="lNode">The node to add. </param>
        /// <param name="lPoint">The point (relative to the form) to add the node at. </param>
        public void AddNode(kAINode lNode, Point lPoint)
        {
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            Behaviour.AddNode(lNode);



            mEditorImpl.AddNode(lNode, lPoint);
        }

        /// <summary>
        /// Save the behaviour. 
        /// </summary>
        public void SaveBehaviour()
        {
            // Not sure if this needs to be here
            if (Behaviour != null)
            {
                Behaviour.Save();
            }
        }

        /// <summary>
        /// Unload the behaviour. 
        /// </summary>
        public void UnloadBehaviour()
        {
            if (Behaviour != null)
            {
                Behaviour.ForceDeactivate();
                Behaviour.Save();
            }

            mEditorImpl.UnloadBehaviour();

            Behaviour = null;
            mBehaviourLocation = null;

        }

        /// <summary>
        /// Update the editor (eg for DirectX). 
        /// </summary>
        public void Update()
        {
            mEditorImpl.EditorUpdate();
        }

        /// <summary>
        /// Destroy the editor, freeing any resources. 
        /// </summary>
        public void Destroy()
        {
            mEditorImpl.Destroy();
        }

        /// <summary>
        /// Can the editor currently form a connexion. 
        /// </summary>
        /// <returns>True if the editor can make a connect. </returns>
        public bool CanConnect()
        {
            return mEditorImpl.CanConnect();
        }

        /// <summary>
        /// Generate a node name for a given node object. 
        /// </summary>
        /// <param name="lNodeObject">The node object to generate a name for. </param>
        /// <returns>A unique (to this behaviour) NodeID based on the object. </returns>
        public kAINodeID GetNodeName(kAIINodeObject lNodeObject)
        {
            kAIObject.Assert(null, Behaviour, "No behaviour to generate name from");
            string lTemplateName = lNodeObject.GetNameTemplate();
            string lModifiedName = lTemplateName;

            int i = 0;
            while (Behaviour.ContainsNodeID(lModifiedName))
            {
                lModifiedName = lTemplateName + i;
                ++i;
            }

            return lModifiedName;
        }

        /// <summary>
        /// Remvoe a node from the loaded behaviour. 
        /// </summary>
        /// <param name="lNode">The node to remove. </param>
        public void RemoveNode(kAINode lNode)
        {
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");
            Behaviour.RemoveNode(lNode);
            mEditorImpl.RemoveNode(lNode);
        }

        void GlobalContextMenu_Popup(object sender, EventArgs e)
        {
            // We store the position relative to the top left of the control
            mMousePositionOnContext = new Point(Control.MousePosition.X - GlobalContextMenu.SourceControl.Location.X, Control.MousePosition.Y - GlobalContextMenu.SourceControl.Location.Y);
        }

        void lAddNodeMenuItem_Click(object sender, EventArgs e)
        {
            kAIINodeObject lSelectedNode = mEditor.SelectNode();
            AddNode(new kAINode(GetNodeName(lSelectedNode), lSelectedNode), mMousePositionOnContext);
        }
    }    
}
