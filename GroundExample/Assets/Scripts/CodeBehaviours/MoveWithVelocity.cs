using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class MoveWithVelocity : CodeBehaviourTester {

    public Vector3 moveVelocity;

	// Use this for initialization
	void Start () 
	{
        codeBehaviour = new MoveWithVelocityBehaviour();
        InitBehaviour();
	}
	
	// Update is called once per frame
	void Update () 
	{
        ((kAIDataPort<Vector3>)codeBehaviour.GetPort("TargetVelocity")).Data = moveVelocity;
        UpdateBehaviour();
	}

    void OnDisable()
    {
        codeBehaviour.ForceDeactivate();
    }
}

[Serializable]
public abstract class kAIUnityAIBehaviour : kAICodeBehaviour
{

    public kAIUnityAIBehaviour(kAIILogger lLogger = null)
        :base(lLogger)
    { }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        if (shipObject != null)
        {
            UnityInternalUpdate(lDeltaTime, shipObject);
        }
    }

    public abstract void UnityInternalUpdate(float lDeltaTime, GameObject lShipObject);

}


public class MoveWithVelocityBehaviour : kAIUnityAIBehaviour
{
    kAIDataPort<Vector3> targetVelocityPort;

    ShipEngine engine;

    public MoveWithVelocityBehaviour()
        :base(null)
    {
        targetVelocityPort = new kAIDataPort<Vector3>("TargetVelocity", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(targetVelocityPort);
        engine = null;
    }


    public override void UnityInternalUpdate(float lDeltaTime, GameObject lShipObject)
    {
        engine = lShipObject.GetComponent<ShipEngine>();
        engine.SetDirection(targetVelocityPort.Data);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if(engine != null)
        {
            engine.SetDirection(Vector3.zero);
        }
    }
}