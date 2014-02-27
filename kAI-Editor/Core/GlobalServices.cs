using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kAI.Core;
using kAI.Editor.Controls;
using kAI.Editor.Controls.WinForms;

namespace kAI.Editor.Core
{
    static class GlobalServices
    {
        public static kAIILogger Logger;

        public static kAIProject LoadedProject;

        public static Editor Editor;

        public static kAIBehaviourEditorWindow BehaviourComposor;

        public static DebugControl Debugger;
    }
}
