using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Flee : CodeBehaviourTester
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

public class FleeBehaviour : kAIUnityAIBehaviour
{
    public FleeBehaviour()
        :base()
    {
    }
    public override void UnityInternalUpdate(float lDeltaTime, GameObject lShipObject)
    {
        
    }

    public static Vector3 GetFleePosition(GameObject self)
    {
        return Vector3.zero;
    }
}