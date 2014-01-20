using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using kAI.Core;


public class MoveToPoint : kAICodeBehaviour
{
    AdvanceTo lateralMover;
    AIRotateToPoint rotationalMover;

    public kAIDataPort<Vector2> targetPoint;

    float currentRotation;

    Vector2 currentTarget = Vector2.zero;

    public MoveToPoint()
        : base(null)
    {
        lateralMover = new AdvanceTo();
        rotationalMover = new AIRotateToPoint();

        rotationalMover.SetGlobal();
        rotationalMover.ForceActivation();

        lateralMover.SetGlobal();
        lateralMover.ForceActivation();

        targetPoint = new kAIDataPort<Vector2>("Target", kAIPort.ePortDirection.PortDirection_In, null);
        AddExternalPort(targetPoint);

    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject ship = lUserData as GameObject;
        if (ship != null)
        {
            ShipEngine engine = ship.GetComponent<ShipEngine>();
            ComputeValues(engine);
            if(currentTarget != targetPoint.Data)
            {
                

                currentTarget = targetPoint.Data;
            }

            lateralMover.Update(lDeltaTime, lUserData);
            //rotationalMover.Update(lDeltaTime, lUserData);
        }
    }

    private void ComputeValues(ShipEngine engine)
    {
        Vector2 targetPosition = targetPoint.Data;
        ///LogMessage("Mouse point: " + targetPosition);
        Vector2 moveVector = targetPoint.Data - (Vector2)engine.transform.position;

       // LogMessage(moveVector.ToString());

        float angle = Vector2.Angle(engine.transform.right, moveVector);
        ((kAIDataPort<float>)rotationalMover.GetPort("Data")).Data = angle * Mathf.Deg2Rad;

        

        //LogMessage("Angle: " + angle);

        float distance = Vector2.Dot(moveVector, engine.transform.right);
        ((kAIDataPort<float>)lateralMover.GetPort("Data")).Data = distance;
        
       // LogMessage("Distance: " + distance);
    }

}

