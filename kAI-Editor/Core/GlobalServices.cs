using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kAI.Core;
using kAI.Editor.Controls;

namespace kAI.Editor.Core
{
    static class GlobalServices
    {
        public static kAIILogger Logger;

        public static kAIProject LoadedProject;

        public static Editor Editor;

        public static kAIBehaviourEditorWindow BehaviourComposor;
    }
}
