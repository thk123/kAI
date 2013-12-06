using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml;


using kAI.Core;
using kAI.Editor.Core;
using System.Drawing;
using kAI.Editor.Controls.DX.Coordinates;
using kAI.Editor.Controls.WinForms;

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

        void SetNodePosition(kAINodeID lNodeID, kAIAbsolutePosition lPoint);

        void AddConnexion(kAIPort.kAIConnexion lConnexion);
        void RemoveConnexion(kAIPort.kAIConnexion lConnexion);

        void AddInternalPort(kAIPort lPort);
        void RemoveInternalPort(kAIPort lPort);

        void AddExternalPort(kAINode lParentNode, kAIPort lPort);
        void RemoveExternalPort(kAINode lParentNode, kAIPort lPort);

        bool CanConnect();

        IEnumerable<Tuple<kAINodeID, kAIAbsolutePosition>> GetNodePositions();

        event Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry> ObjectSelected;
    }

    class kAIBehaviourEditorWindow
    {
        kAIIBehaviourEditorGraphicalImplementator mEditorImpl; 

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
        /// Happens when something is selected within the behaviour editor. 
        /// </summary>
        public event Action<kAI.Editor.ObjectProperties.kAIIPropertyEntry> ObjectSelected;

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

            MenuItem lAddFunctionItem = new MenuItem("Add Function...");
            lAddFunctionItem.Click += new EventHandler(lAddFunctionItem_Click);

            MenuItem lAddConstantItem = new MenuItem("Add Constant...");
            lAddConstantItem.Click += new EventHandler(lAddConstantItem_Click);

            GlobalContextMenu = new ContextMenu(new MenuItem[] {
                lAddNodeMenuItem,
                lAddFunctionItem,
                lAddConstantItem
            });

            GlobalContextMenu.Popup += new EventHandler(GlobalContextMenu_Popup);

            
        }

        /// <summary>
        /// Initialise the behaviour composer inside the given control. 
        /// </summary>
        /// <param name="lContainer"></param>
        public void Init(Control lContainer)
        {
            mEditorImpl.Init(lContainer, this);

            mEditorImpl.ObjectSelected += ObjectSelected;

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
                Behaviour.OnInternalPortAdded -= Behaviour_OnInternalPortAdded;
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


            // Load the meta file
            XmlObjectSerializer lProjectDeserialiser = new DataContractSerializer(typeof(kAIXmlBehaviourMetaSaveFile), kAINode.NodeSerialTypes);

            FileInfo lMetaPath = new FileInfo(Behaviour.XmlLocation.GetFile().FullName + ".meta");
            if(lMetaPath.Exists)
            {
                Stream lXmlStream = lMetaPath.OpenRead();

                kAIXmlBehaviourMetaSaveFile lXmlFile = (kAIXmlBehaviourMetaSaveFile)lProjectDeserialiser.ReadObject(lXmlStream);

                lXmlStream.Close();

                foreach (Tuple<kAINodeID, kAIAbsolutePosition> lNodePosition in lXmlFile.GetPositions())
                {
                    mEditorImpl.SetNodePosition(lNodePosition.Item1, lNodePosition.Item2);
                }
            }
            else
            {
                kAIObject.LogWarning(Behaviour, "No meta file found for behaviour", new KeyValuePair<string, object>("Behaviour", Behaviour.BehaviourID));
            }

            Behaviour.OnInternalPortAdded += new kAIXmlBehaviour.InternalPortAdded(Behaviour_OnInternalPortAdded);

            Behaviour.SetGlobal();
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
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            Behaviour.AddInternalPort(lInternalPort, true);

            mEditorImpl.AddInternalPort(lInternalPort);
        }

        /// <summary>
        /// Add a node to the loaded behaviour. 
        /// </summary>
        /// <param name="lNodeContents">The node to add. </param>
        /// <param name="lPoint">The absolute position to add the point to. </param>
        public void AddNode(kAIINodeObject lNodeContents, kAIAbsolutePosition lPoint)
        {
            // TODO: work out if needed?
            // TODO: come back when done generic classes for other controls
            // Maybe should be in the impl any way
            //mNodes.Add(lNode);

            kAIObject.Assert(null, Behaviour, "No loaded behaviour");

            kAINode lNewNode = new kAINode(GetNodeName(lNodeContents), lNodeContents, Behaviour);

            Behaviour.AddNode(lNewNode);

            mEditorImpl.AddNode(lNewNode, lPoint);
        }

        /// <summary>
        /// Add a node to the loaded behaviour. 
        /// </summary>
        /// <param name="lNodeContents">The node to add. </param>
        /// <param name="lPoint">The point (relative to the form) to add the node at. </param>
        public void AddNode(kAIINodeObject lNodeContents, Point lPoint)
        {
            kAIObject.Assert(null, Behaviour, "No loaded behaviour");
            kAINode lNewNode = new kAINode(GetNodeName(lNodeContents), lNodeContents, Behaviour);
            Behaviour.AddNode(lNewNode);



            mEditorImpl.AddNode(lNewNode, lPoint);
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


            kAIXmlBehaviourMetaSaveFile lMetaFile = new kAIXmlBehaviourMetaSaveFile(mEditorImpl.GetNodePositions());

            XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(kAIXmlBehaviourMetaSaveFile), kAINode.NodeSerialTypes);

            // Settings for writing the XML file 
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;

            // Create the writer and write the file. 
            XmlWriter lWriter = XmlWriter.Create(Behaviour.XmlLocation.GetFile().FullName + ".meta", lSettings);
            lProjectSerialiser.WriteObject(lWriter, lMetaFile);
            lWriter.Close();   

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

        void Behaviour_OnInternalPortAdded(kAIXmlBehaviour lSender, kAIPort lNewPort)
        {
            mEditorImpl.AddInternalPort(lNewPort);
        }

        void GlobalContextMenu_Popup(object sender, EventArgs e)
        {
            // We store the position relative to the top left of the control
            mMousePositionOnContext = new Point(Control.MousePosition.X - GlobalContextMenu.SourceControl.Location.X, Control.MousePosition.Y - GlobalContextMenu.SourceControl.Location.Y);
        }

        void lAddNodeMenuItem_Click(object sender, EventArgs e)
        {
            kAIINodeObject lSelectedNode = mEditor.SelectNode();
            if (lSelectedNode != null)
            {
                AddNode(lSelectedNode, mMousePositionOnContext);
            }
        }

        void lAddConstantItem_Click(object sender, EventArgs e)
        {
            ConstantNodeCreator lNodeCreatore = new ConstantNodeCreator();

            if (lNodeCreatore.ShowDialog() == DialogResult.OK)
            {
                AddNode(lNodeCreatore.Node, mMousePositionOnContext);
            }
        }

        void lAddFunctionItem_Click(object sender, EventArgs e)
        {
            FunctionNodeCreator lFunctionDesigner = new FunctionNodeCreator(mProject);
            DialogResult lResult = lFunctionDesigner.ShowDialog();
            if (lResult == DialogResult.OK)
            {
                AddNode(lFunctionDesigner.FunctionNode, mMousePositionOnContext);
            }
        }
    }    
}
