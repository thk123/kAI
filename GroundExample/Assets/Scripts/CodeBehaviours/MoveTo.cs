using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class MoveTo : MonoBehaviour, IOrderReciever {

    kAICodeBehaviour moveBeh;

	// Use this for initialization
	void Start () 
	{
        moveBeh = new MoveToBehaviour();
        moveBeh.SetGlobal();
        moveBeh.ForceActivation();

        kAIPort lPort = moveBeh.GetPort("Point");
        ((kAIDataPort<Vector3>)lPort).Data = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        moveBeh.Update(Time.deltaTime, gameObject);
	}

    public void GiveOrder(Vector3 temp)
    {
        //Vector3 actualMove = new Vector3(temp.x, transform.position.y, temp.y);
        kAIPort lPort = moveBeh.GetPort("Point");
        temp.y = transform.position.y;
        ((kAIDataPort<Vector3>)lPort).Data = temp;

    }
}

public class MoveToBehaviour : kAICodeBehaviour
{
    public kAIDataPort<Vector3> pointToMoveTo;

    public MoveToBehaviour()
        :base(null)
    {
        pointToMoveTo = new kAIDataPort<Vector3>("Point", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(pointToMoveTo);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        if(shipObject != null)
        {
            // TODO: maybe rotate the ship then move, but not with physics, just like, over some frames

            
            Vector3 targetPositon = pointToMoveTo.Data;

            if((targetPositon - shipObject.transform.position).sqrMagnitude > 1.0f)
            {
                shipObject.transform.rotation = Quaternion.LookRotation(targetPositon - shipObject.transform.position);

                ShipEngine engine = shipObject.GetComponent<ShipEngine>();
                engine.SetDirection(targetPositon - shipObject.transform.position);
            }
            else 
            {
                ShipEngine engine = shipObject.GetComponent<ShipEngine>();
                engine.SetDirection(Vector3.zero);
            }
        }
    }
}
