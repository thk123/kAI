using UnityEngine;
using kAI.Core;
using System.Collections;
using System.IO;
using System.Reflection;

public class AIBehaviour : MonoBehaviour {

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

}