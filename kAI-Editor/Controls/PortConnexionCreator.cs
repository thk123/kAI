using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;
using kAI.Editor.Controls.DX;

namespace kAI.Editor.Controls
{
    class kAIPortConnexionCreator
    {
        bool mIsConnecting;
        kAIPort mStartPort;

        kAIBehaviourEditorWindow mWindow;

        /// <summary>
        /// Are we currently creating a link. 
        /// </summary>
        public bool CreatingLink
        {
            get
            {
                return mIsConnecting;
            }
        }

        /// <summary>
        /// Create a new PortConnexionCreator for a given editor window. 
        /// </summary>
        /// <param name="lWindow">The window to add new connexions to. </param>
        public kAIPortConnexionCreator(kAIBehaviourEditorWindow lWindow)
        {
            mIsConnecting = false;
            mStartPort = null;

            mWindow = lWindow;
        }

        /// <summary>
        /// When the mouse is pushed down inside a port. 
        /// </summary>
        /// <param name="lPort">The port the mouse was down inside. </param>
        public void PortDown(kAIPort lPort)
        {
            if (!mIsConnecting)
            {
                StartConnexion(lPort);
            }
        }

        /// <summary>
        /// When the mouse is released inside a port. 
        /// </summary>
        /// <param name="lPort">The port the mouse was released in. </param>
        public void PortUp(kAIPort lPort)
        {
            if (mIsConnecting)
            {
                EndConnexion(lPort);
            }
        }

        /// <summary>
        /// Start creating a connexion. 
        /// </summary>
        /// <param name="lPort">The port we are starting a connexion at. </param>
        private void StartConnexion(kAIPort lPort)
        {
            if(mWindow.CanConnect())
            {
                kAIObject.Assert(lPort, !mIsConnecting, "Attempted to start a connexion after it had begun!");
                kAIObject.Assert(lPort, mStartPort == null, "Attempted to start a connexion after it had begun!");
                mStartPort = lPort;
                mIsConnecting = true;
            }
        }

        /// <summary>
        /// Finish creating a connexion. 
        /// </summary>
        /// <param name="lEndPort">The port that we finished on.</param>
        private void EndConnexion(kAIPort lEndPort)
        {
            
            kAIObject.Assert(lEndPort, mIsConnecting, "Attempted to end a connexion before it had begun!");
            kAIObject.Assert(lEndPort, mStartPort, "Attempted to end a connexion before it had begun!");

            // Providing it is a different port, we make the connexion, otherwise cancel, the person has just clicked on the port. 
            if (lEndPort.PortID != mStartPort.PortID)
            {
                mWindow.AddConnexion(mStartPort, lEndPort);

                mStartPort = null;
                mIsConnecting = false;
            }
        }


    }
}
