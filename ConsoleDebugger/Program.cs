using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using kAI.Core;
using kAI.Core.Debugger;

namespace ConsoleDebugger
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IDebugServer> pipeFactor = new ChannelFactory<IDebugServer>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/PipeServer"));

            ChannelFactory<IDebugServer> httpFactory = new ChannelFactory<IDebugServer>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/DebugServer"));

            //IDebugServer server = pipeFactor.CreateChannel();
            IDebugServer serverhttp = httpFactory.CreateChannel();

            //server.SelectBehaviour("SimpleAIBase");

            serverhttp.SelectBehaviour("SimpleAIBase");

            Console.ReadLine();
        }
    }
}
