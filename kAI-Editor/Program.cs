using System;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

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
        static void Main(string[] args)
        {
            CommandLineHandler.ProcessCommands(args);
            //kAIObject.GlobalLogger = ConsoleLogger.Get();
            ConsoleLogger.Get().LogMessage("kAI Editor starting");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var lEditor = new Editor();

            // We run the editor on the SlimDX message pump to allow for the DirectX editor. 
            MessagePump.Run(lEditor, () =>
            {
                lEditor.RenderUpdate();
            });

        }
    }
}
