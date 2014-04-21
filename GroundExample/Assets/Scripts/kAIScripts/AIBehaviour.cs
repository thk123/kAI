using UnityEngine;
using kAI.Core;
using System.Collections;
using System.IO;
using System.Reflection;

using kAI.Core.Debug;

public class AIBehaviour : MonoBehaviour, kAIILogger {

    public string XmlPath;

    public kAIXmlBehaviour mXmlBehaviour
    {
        get;
        private set;
    }

    static bool firstTime = true;

    static AIBehaviour()
    {
        // add custom properties dictionaries
        Vector3Propertes x = new Vector3Propertes();
        kAIFunctionNode.kAIFunctionConfiguration.kAIReturnConfiguration.kAIReturnConfigurationDictionary.AddDictionary(x);
    }

    void Awake()
    {
        

        if(XmlPath != null && XmlPath != string.Empty)
        {
            FileInfo lFile = new FileInfo(Application.dataPath + "/" + XmlPath);
            mXmlBehaviour = kAIXmlBehaviour.LoadFromFile(lFile, GetAssemblyByName);
        }

        //DebugServer.Server.RegisterBehaviour(mXmlBehaviour);
    }

	// Use this for initialization
	void Start () {
        kAIObject.GlobalLogger = this;
        if (mXmlBehaviour != null)
        {
            mXmlBehaviour.SetGlobal();
            mXmlBehaviour.ForceActivation();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (firstTime)
        {
            //DebugServer.Server.RefreshBehaviour();
            firstTime = false;
        }
        if (mXmlBehaviour != null)
        {
            mXmlBehaviour.Update(Time.deltaTime, this.gameObject);
        }

        
	}

    public kAIPort GetPort(kAIPortID lPort)
    {
        try
        {
            if(mXmlBehaviour == null)
            {
                return null;
            }
            return mXmlBehaviour.GetPort(lPort);
        }
        catch
        {
            return null;
        }
    }



    void LateUpdate()
    {
        firstTime = true;
    }

    public Assembly GetAssemblyByName(string lAssemblyName)
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

    public void LogCriticalError(string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lError);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }

        /*if (kAIDebugServer.IsInit)
        {
            kAIDebugServer.AddMessage(lError, lDetails);
        }*/
    }

    public void LogError(string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lError);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }

        /*if (kAIDebugServer.IsInit)
        {
            kAIDebugServer.AddMessage(lError, lDetails);
        }*/
    }

    public void LogMessage(string lMessage, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lMessage);
        foreach(var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
        /*if (kAIDebugServer.IsInit)
        {
            kAIDebugServer.AddMessage(lMessage, lDetails);
        }*/
    }

    public void LogWarning(string lWarning, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lWarning);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }

        /*if (kAIDebugServer.IsInit)
        {
            kAIDebugServer.AddMessage(lWarning, lDetails);
        }*/
    }
}