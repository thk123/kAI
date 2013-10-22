using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Glee;
using Microsoft.Glee.Drawing;
using Microsoft.Glee.GraphViewerGdi;
using Microsoft.Glee.Splines;

using GleeNode = Microsoft.Glee.Drawing.Node;

using kAI.Core;
using kAI.Editor.Controls;
using kAI.Editor.Controls.DX.Coordinates;

namespace kAI.Editor.Controls.GLEE
{
    class BehaviourEditorWindowGLEE : kAIIBehaviourEditorGraphicalImplementator
    {
        GViewer mGraphViewer;
        Graph mBehaviourGraph;

        //Dictionary<kAINodeID, Tuple<kAINode, GleeNode>> mNodes;

        public void Init(System.Windows.Forms.Control lParentControl, kAIBehaviourEditorWindow lEditor)
        {
            mGraphViewer = new GViewer();
            lParentControl.Controls.Add(mGraphViewer);
            mGraphViewer.Dock = DockStyle.Fill;
            mBehaviourGraph = new Graph("Behaviour Graph");
            //mNodes = new Dictionary<kAINodeID, Tuple<kAINode, GleeNode>>();
        }

        public void UnloadBehaviour()
        {
            throw new NotImplementedException();
        }

        public void EditorUpdate()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void AddNode(kAI.Core.kAINode lNode, kAIAbsolutePosition lPoint)
        {
            GleeNode lGleeNode = mBehaviourGraph.AddNode(lNode.NodeID);
            lGleeNode.UserData = lNode;

            //mNodes.Add(lNode.NodeID, new Tuple<kAINode, GleeNode>(lNode, lGleeNode));

        }

        public void AddNode(kAI.Core.kAINode lNode, System.Drawing.Point lPoint)
        {
            AddNode(lNode, lPoint);
        }

        public bool CanConnect()
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(kAI.Core.kAINode lNode)
        {
            mBehaviourGraph.NodeMap.Remove(lNode.NodeID);
        }

        public void AddConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
            //mBehaviourGraph.AddEdge(lConnexion.StartPort.)
        }

        public void RemoveConnexion(kAI.Core.kAIPort.kAIConnexion lConnexion)
        {
            throw new NotImplementedException();
        }

        public void AddInternalPort(kAI.Core.kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public void RemoveInternalPort(kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public void AddExternalPort(kAI.Core.kAINode lParentNode, kAI.Core.kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public void RemoveExternalPort(kAINode lParentNode, kAIPort lPort)
        {
            throw new NotImplementedException();
        }

        public event Action<kAI.Core.kAIPort, kAI.Core.kAIPort> OnConnexion;
    }
}
