using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class CodeBehaviourTester : MonoBehaviour
{
    protected kAICodeBehaviour codeBehaviour;

    protected void InitBehaviour()
    {
        codeBehaviour.SetGlobal();
        codeBehaviour.ForceActivation();
    }

    
    protected void UpdateBehaviour()
    {
        codeBehaviour.Update(Time.deltaTime, gameObject);
    }
}

public class MoveTo : CodeBehaviourTester, IOrderReciever {


	// Use this for initialization
	void Start () 
	{
        codeBehaviour = new MoveToBehaviour();
        InitBehaviour();

        kAIPort lPort = codeBehaviour.GetPort("Point");
        ((kAIDataPort<Vector3>)lPort).Data = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        UpdateBehaviour();
	}

    public void GiveOrder(Vector3 temp)
    {
        //Vector3 actualMove = new Vector3(temp.x, transform.position.y, temp.y);
        kAIPort lPort = codeBehaviour.GetPort("Point");
        temp.y = transform.position.y;
        ((kAIDataPort<Vector3>)lPort).Data = temp;

    }
}

public class MoveToBehaviour : kAICodeBehaviour
{
    public kAIDataPort<Vector3> pointToMoveTo;

    public kAIDataPort<Vector3> outComeVelocity;

    public MoveToBehaviour()
        :base(null)
    {
        pointToMoveTo = new kAIDataPort<Vector3>("Point", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(pointToMoveTo);

        outComeVelocity = new kAIDataPort<Vector3>("Velocity", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(outComeVelocity);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        if(shipObject != null)
        {
            
            Vector3 targetPositon = pointToMoveTo.Data;
            
            if((targetPositon - shipObject.transform.position).sqrMagnitude > 5.0f)
            {
                outComeVelocity.Data = targetPositon - shipObject.transform.position;
            }
            else 
            {
                outComeVelocity.Data = Vector3.zero;
                Deactivate();
            }
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        outComeVelocity.Data = Vector3.zero;
    }
}
