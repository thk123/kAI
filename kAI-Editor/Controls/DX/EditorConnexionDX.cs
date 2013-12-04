using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using kAI.Core;
using kAI.Editor.Controls.DX.Coordinates;

namespace kAI.Editor.Controls.DX
{
    class kAIEditorConnexionDX
    {
        kAIPort.kAIConnexion mConnexion;
        kAIBehaviourEditorWindowDX mEditor;

        List<kAIAbsolutePosition> mPath;


        /// <summary>
        /// The start port (PortDirection_Out) of this connexion. 
        /// </summary>
        public kAIPort Start
        {
            get
            {
                return mConnexion.StartPort;
            }
        }


        /// <summary>
        /// The end port (PortDirection_In) of this connexion. 
        /// </summary>
        public kAIPort End
        {
            get
            {
                return mConnexion.EndPort;
            }
        }

        public kAIEditorConnexionDX(kAIPort.kAIConnexion lConnexion, kAIBehaviourEditorWindowDX lEditor)
        {
            mConnexion = lConnexion;
            mEditor = lEditor;

            mPath = null;
        }

        public void InvalidatePath()
        {
            mPath = null;
        }

        public void Render2D()
        {}

        public void LineRender()
        {
            if (mPath == null)
            {
                mPath = mEditor.GetPointPath(mConnexion);
            }

            mEditor.RenderLine(mPath);
        }
    }
}
