using UnityEngine;
using System.Collections;
using kAI.Core;


[RequireComponent(typeof(kAIUnityBehaviour))]
public class kAIDebugBehaviour : MonoBehaviour {
	
	kAIXmlBehaviour mBehaviour;
	// Use this for initialization
	void Start () {
		kAIUnityBehaviour lUnityBehaviour = GetComponent<kAIUnityBehaviour>();
		mBehaviour = lUnityBehaviour.XmlBehaviour;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*void OnGUI()
	{
		int i = 0;
		foreach(string s in mBehaviour.DebugInfo.ActiveNodes)
		{
			GUI.Box(new Rect(10, 10 + 60*i, 200, 50), s);	
			++i;
		}
	}*/
}
