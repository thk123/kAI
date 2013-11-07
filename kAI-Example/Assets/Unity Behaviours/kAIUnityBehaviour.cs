using UnityEngine;
using kAI.Core;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

using AssemblyCSharp;

public class kAIUnityBehaviour : MonoBehaviour, kAIILogger
{
	
	kAIXmlBehaviour mXmlBehaviour;
	
	public kAIXmlBehaviour XmlBehaviour
	{
		get
		{
			return mXmlBehaviour;
		}
	}
	
	public string XmlBehaviourPath;
	
	object lObject;
	
	void Awake()
	{
		kAIObject.GlobalLogger = this;
		//kAIObject.ExceptionHandler = HandleException;
		kAIObject.ExceptionOnAssert = true;
		
		FileInfo lFile = new FileInfo(XmlBehaviourPath);
		mXmlBehaviour = kAIXmlBehaviour.LoadFromFile(lFile, GetAssemblyByName);
	}
	
	TestBehaviour ls;
	// Use this for initialization
	void Start () {
		lObject = new object();
		//kAIRelativeObject.AddPathID(mXmlBehaviour.XmlLocationID, lFile.Directory);
		
		
		mXmlBehaviour.SetGlobal();
		mXmlBehaviour.ForceActivation();
		
		/*mXmlBehaviour.GetPort("TestPort").OnTriggered += delegate(kAIPort lSender) {
			print ("Inner behaviour deactivated");
		};*/
		
		ls = new TestBehaviour(null);
	}
	
	// Update is called once per frame
	void Update () {
		//mXmlBehaviour.Update(Time.deltaTime);
		lock(lObject)
		{
			mXmlBehaviour.Update(Time.deltaTime, this.gameObject);
		}
		
		/*if(!stopped && Time.timeSinceLevelLoad > 5.0f)
		{
			kAIPort lPort = mXmlBehaviour.GetPort("AnotherTestPort");
			lPort.Trigger();
			
			stopped = true;
		}*/
	}
	
	public kAIPort GetPort(string lPortID)
	{
		return mXmlBehaviour.GetPort(lPortID);	
	}
	
	public void TriggerPort(string lPortID)
	{
			
		lock(lObject)
		{
			mXmlBehaviour.GetPort(lPortID).Trigger();	
		}
	}
	
	public void DoSomeMagic()
	{
		LogMessage("Hello from Unity");
	}
	
	public void HandleException(Exception e, kAIObject sender)
	{
		Debug.LogException(e);	
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

	#region kAIILogger implementation
	public void LogMessage (string lMessage, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.Log(lMessage);
		foreach(KeyValuePair<string, object> keys in lDetails)
		{
			Debug.Log("\t" + keys.Key + ": " + keys.Value);	
		}
	}

	public void LogWarning (string lWarning, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogWarning(lWarning);
		foreach(KeyValuePair<string, object> keys in lDetails)
		{
			Debug.Log("\t" + keys.Key + ": " + keys.Value);	
		}
	}

	public void LogError (string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogError(lError);
		foreach(KeyValuePair<string, object> keys in lDetails)
		{
			Debug.Log("\t" + keys.Key + ": " + keys.Value);	
		}
	}

	public void LogCriticalError (string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogError(lError);
		foreach(KeyValuePair<string, object> keys in lDetails)
		{
			Debug.Log("\t" + keys.Key + ": " + keys.Value);	
		}
	}
	#endregion
}




