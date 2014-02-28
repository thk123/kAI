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
        GameObject result = WeaponFunctions.GetNearestTarget(self, 300.0f);
        if (result != null)
        {
            Vector3 vectorAwayFromTarget = self.transform.position - result.transform.position;
            return self.transform.position + vectorAwayFromTarget;
        }
        else
        {
            // We are fare enough away, we chill
            return self.transform.position;
        }
    }
}