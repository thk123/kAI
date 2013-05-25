using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using kAI.Core;
using kAI.Editor.Core;

namespace kAI.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            kAIObject.GlobalLogger = ConsoleLogger.Get();
            ConsoleLogger.Get().LogMessage("kAI Editor starting");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Editor());
        }
    }
}
