using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using kAI.Core;

using AssemblyCSharp;

namespace kAIBehaviourLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBehaviour lForced = new TestBehaviour(null);

            string XmlBehaviourPath = @"E:\dev\C#\kAI\kAI-Example\kAIBehaviours\BaseBehaviour.xml";
            kAIXmlBehaviour mXmlBehaviour;

            FileInfo lFile = new FileInfo(XmlBehaviourPath);
            mXmlBehaviour = kAIXmlBehaviour.LoadFromFile(lFile, GetAssemblyByName);

            kAIRelativeObject.AddPathID(mXmlBehaviour.XmlLocationID, lFile.Directory);
        }

        public static Assembly GetAssemblyByName(string lAssemblyName)
        {
            if (lAssemblyName == Assembly.GetExecutingAssembly().GetName().Name)
            {
                return Assembly.GetExecutingAssembly();
            }
            else
            {
                foreach (AssemblyName lRefdAssemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {

                    if (lRefdAssemblyName.Name == lAssemblyName)
                    {
                        Assembly lAssembly = Assembly.Load(lRefdAssemblyName);
                        if (lAssembly != null)
                        {
                            return lAssembly;
                        }
                    }
                }
            }

            /*Resources.Load(		
            UnityEngine.Object[] lResources = Resources.FindObjectsOfTypeAll(typeof(Assembly));*/


            return null;
        }
    }
}
