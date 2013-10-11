using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;

namespace kAI.Editor.Controls
{
    interface kAIIBehaviourEditorGraphicalImplementator
    {
        void Init(Control lParentControl, kAIBehaviourEditorWindow lWindow);
        void UnloadBehaviour();

        void EditorUpdate();
        void Destroy();

        void AddNode(kAINode lNode);
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
        /// <summary>
        /// The behaviour currently being shown in this editor. 
        /// </summary>
        kAIXmlBehaviour mBehaviour;

        FileInfo mBehaviourLocation;

        kAIProject mProject;

        

        public kAIBehaviourEditorWindow(kAIProject lProject, kAIIBehaviourEditorGraphicalImplementator lEditorImpl)
        {
            mProject = lProject;
                       

            mEditorImpl = lEditorImpl;

            UnloadBehaviour();
        }

        public void Init(Control lContainer)
        {
            mEditorImpl.Init(lContainer, this);
        }


        public void AddConnexion(kAIPort lStartPort, kAIPort lEndPort)
        {
            kAIObject.Assert(null, mBehaviour, "No loaded behaviour");

            mBehaviour.AddConnexion(lStartPort, lEndPort);

            mEditorImpl.AddConnexion(new kAIPort.kAIConnexion(lStartPort, lEndPort));
        }

        /// <summary>
        /// Load an existing XML behaviour in to the editor. 
        /// </summary>
        /// <param name="lBehaviour">The XML behaviour to load. </param>
        public void LoadBehaviour(kAIXmlBehaviour lBehaviour)
        {
            if (mBehaviour != null)
            {
                UnloadBehaviour();
            }

            foreach (kAIPort lGlobalPort in lBehaviour.InternalPorts)
            {
                AddInternalPort(lGlobalPort);
            }

            

            foreach (kAINode lInternalNode in lBehaviour.InternalNodes)
            {
                AddNode(lInternalNode);
            }

            // TODO: load connexions

            mBehaviour = lBehaviour;

            
        }

        public void AddInternalPort(kAIPort lInternalPort)
        {
            mEditorImpl.AddInternalPort(lInternalPort);
        }

        public void AddNode(kAINode lNode)
        {
            // TODO: work out if needed?
            // TODO: come back when done generic classes for other controls
            // Maybe should be in the impl any way
            //mNodes.Add(lNode);

            mEditorImpl.AddNode(lNode);
        }

        public void SaveBehaviour()
        {
            // Not sure if this needs to be here
            if (mBehaviour != null)
            {
                mBehaviour.Save();
            }
        }

        public void UnloadBehaviour()
        {
            if (mBehaviour != null)
            {
                mBehaviour.Save();
            }

            mEditorImpl.UnloadBehaviour();

            mBehaviour = null;
            mBehaviourLocation = null;

        }

        public void Update()
        {
            mEditorImpl.EditorUpdate();
        }

        public void Destroy()
        {
            mEditorImpl.Destroy();
        }

        public bool CanConnect()
        {
            return mEditorImpl.CanConnect();
        }

    }

    
}
