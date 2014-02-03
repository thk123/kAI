using UnityEngine;
using kAI.Core;
using System.Collections;
using System.IO;
using System.Reflection;

public class AIBehaviour : MonoBehaviour, kAIILogger {

    public string XmlPath;

    public kAIXmlBehaviour mXmlBehaviour
    {
        get;
        private set;
    }

    static bool firstTime = true;

    void Awake()
    {
        FileInfo lFile = new FileInfo(XmlPath);
        mXmlBehaviour = kAIXmlBehaviour.LoadFromFile(lFile, GetAssemblyByName);

        //DebugServer.Server.RegisterBehaviour(mXmlBehaviour);
    }

	// Use this for initialization
	void Start () {
        kAIObject.GlobalLogger = this;
        mXmlBehaviour.SetGlobal();
        mXmlBehaviour.ForceActivation();
	}
	
	// Update is called once per frame
	void Update () {
        if (firstTime)
        {
            //DebugServer.Server.RefreshBehaviour();
            firstTime = false;
        }
        
        mXmlBehaviour.Update(Time.deltaTime, this.gameObject);

        
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
    }

    public void LogError(string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lError);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
    }

    public void LogMessage(string lMessage, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lMessage);
        foreach(var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
    }

    public void LogWarning(string lWarning, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lWarning);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
    }
}