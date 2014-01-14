using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;

namespace kAI.Core.Debugger
{
    [ServiceContract]
    public interface IDebugServer
    {
        [OperationContract]
        void SelectBehaviour(kAIBehaviourID lBehaviourID);
    }

    public class DebugServer : IDebugServer
    {
        static DebugServer activeServer;
        public static DebugServer Server
        {
            get 
            {
                if (activeServer == null)
                {
                    activeServer = new DebugServer(null);
                    activeServer.StartServer();
                }

                return activeServer;
            }

        }

        public static DebugServer CreateServer(kAIILogger lLogger)
        {
            activeServer = new DebugServer(lLogger);
            activeServer.StartServer();

            return activeServer;
        }

        Dictionary<kAIBehaviourID, kAIXmlBehaviour> mDebuggableBehaviours;
        kAIXmlBehaviour mCurrentBehaviour;

        public const string DebugServerPipeID = "kAIDebugServer";

        Stream mBehaviourStream;

        public bool DebugServerRunning
        {
            get;
            private set;
        }

        static kAIILogger sLogger;

        static Thread debugThread;

        public DebugServer(kAIILogger lLogger)
        {
            mDebuggableBehaviours = new Dictionary<kAIBehaviourID, kAIXmlBehaviour>();
            DebugServerRunning = false;
            sLogger = lLogger;
            mCurrentBehaviour = null;
        }

        public void RegisterBehaviour(kAIXmlBehaviour lBehaviour)
        {
            mDebuggableBehaviours.Add(lBehaviour.BehaviourID, lBehaviour);
        }

        public void SelectBehaviour(kAIBehaviourID lBehaviourID)
        {
            mCurrentBehaviour = mDebuggableBehaviours[lBehaviourID];
            if (mBehaviourStream != null)
            {
                //todo
            }
            mBehaviourStream = new NamedPipeServerStream("kAIDebuggerBehaviour_" + lBehaviourID);

            RefreshBehaviour();
        }

        public void StartServer()
        {
           /* sLogger.LogMessage("Starting Debug Server");

            ServiceHost host = new ServiceHost(this, new Uri("net.pipe://localhost")/*, new Uri("http://localhost:8000"));
            {
                sLogger.LogMessage("Started ");
                host.AddServiceEndpoint(typeof(IDebugServer), new NetNamedPipeBinding(), "PipeServer");

                host.AddServiceEndpoint(typeof(IDebugServer), new BasicHttpBinding(), "DebugServer");

                host.Open();

                sLogger.LogMessage("Opened");


                debugThread = new Thread(() => Run(host));
                debugThread.Start();   
            }*/

                   
        }

        public void StopServer()
        {
            DebugServerRunning = false;
            debugThread.Join();
        }

        public void RefreshBehaviour()
        {
            if (mCurrentBehaviour != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();               

                formatter.Serialize(mBehaviourStream, mCurrentBehaviour);

                mBehaviourStream.Flush();
            }
        }

        void Run(ServiceHost host)
        {
            while (true)
            {
                //todo: 
                sLogger.LogMessage("Serve running " + host.State.ToString());
            }

            sLogger.LogMessage("Closed :(");

            host.Close();

        }
    }
}
