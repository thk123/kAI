using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class CircleFormation : Formation 
{
    public override IndividualOrder CreateOrder(int index, SquadMember memeber, Vector3 destination, Vector3 direction)
    {
        Vector3 lDestination = memeber.transform.position;
        Vector3 lForwardDirection = Vector3.Cross(direction, Vector3.up);
        lDestination += lForwardDirection.normalized * 1.0f;

        return IndividualOrder.CreateMoveOrder(lDestination);
    }
}

public class IndividualFigureofEight : kAIUnityAIBehaviour
{
    kAIDataPort<Vector3> mMovePort;
    kAIDataPort<Vector3> mFacePort;

    float startAngle = 0.0f;
    float currentDirection = 1.0f;
    int lastSwap = 0;
    public IndividualFigureofEight()
        : base()
    {
        mMovePort = new kAIDataPort<Vector3>("Move", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mMovePort);

        mFacePort = new kAIDataPort<Vector3>("Face", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mFacePort);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        startAngle = Mathf.NegativeInfinity;
    }

    public override void UnityInternalUpdate(float lDeltaTime, GameObject lShipObject)
    {
       // LogMessage("Angle:" + lShipObject.transform.rotation.eulerAngles.y + ", fc: " + lastSwap );

        if (startAngle == Mathf.NegativeInfinity)
        {
            startAngle = lShipObject.transform.rotation.eulerAngles.y;
            
        }
        else
        {
            if (Mathf.Abs(lShipObject.transform.rotation.eulerAngles.y - startAngle) < 5.0f && lastSwap > 60)
            {
                currentDirection *= -1.0f;
                startAngle = lShipObject.transform.rotation.eulerAngles.y;
                lastSwap = 0;
                
            }
        }

        ++lastSwap;

        Vector3 lMoveDir = Quaternion.Euler(Vector3.up * 5.0f * currentDirection) * lShipObject.transform.forward;
        mMovePort.Data = lMoveDir;
        mFacePort.Data = lMoveDir;
    }
}

