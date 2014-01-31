using UnityEngine;
using System;
using System.Collections;

using kAI.Core;

public class CheckAhead : kAICodeBehaviour {

    kAIDataPort<bool> OnObstacle;

    kAIDataPort<Vector2> newDestination;


    public CheckAhead()
        :base(null)
    {
        OnObstacle = new kAIDataPort<bool>("IsObstacle", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(OnObstacle);

        newDestination = new kAIDataPort<Vector2>("AvoidDestination", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(newDestination);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        LogMessage("Updating");
        if(shipObject != null)
        {
            ShipEngine engine = shipObject.GetComponent<ShipEngine>();

            Vector2 velocity = shipObject.rigidbody2D.velocity;

            float distToSlowDown = velocity.magnitude / (2 * engine.accelerationForce);
            bool shouldAvoid = false;

            if(distToSlowDown > 0.0f)
            {
                RaycastHit2D[] resultList = Physics2D.RaycastAll(shipObject.transform.GetPosition2D(), velocity, 2.0f * distToSlowDown);
                
                RaycastHit2D? result = null;

                foreach(RaycastHit2D hit in resultList)
                {
                    if(hit.transform != shipObject.transform)
                    {
                        result = hit;
                        break;
                    }
                }

                

                if (result.HasValue)
                {
                    
                    if (Mathf.Abs(Vector2.Dot(result.Value.rigidbody.velocity, shipObject.transform.up)) < 0.1f)
                    {
                        shouldAvoid = true;
                        LogMessage("Radius: " + result.Value.collider.GetColliderRadius2D());
                        Vector2 avoidPos = result.Value.transform.GetPosition2D() - (Mathf.Max(result.Value.rigidbody.velocity.sqrMagnitude, 0.5f) * (Vector2)shipObject.transform.up);

                        newDestination.Data = avoidPos;

                        Deactivate();
                    }
                }
            }

            OnObstacle.Data = shouldAvoid;
        }
    }
}


public class AvoidObject : kAICodeBehaviour
{
    kAIDataPort<GameObject> objectToAvoid;

    public AvoidObject()
        :base(null)
    {
        objectToAvoid = new kAIDataPort<GameObject>("AvoidObject", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(objectToAvoid);


    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        LogMessage("Updating avoid");
        GameObject shipObject = lUserData as GameObject;
        if(shipObject != null)
        {
            if(objectToAvoid.Data != null)
            {
                ShipEngine engine = shipObject.GetComponent<ShipEngine>();

                GameObject shipToAvoid = objectToAvoid.Data;

                Vector2 targetVelocity;
                if(shipToAvoid.rigidbody2D != null)
                {
                    targetVelocity = shipToAvoid.rigidbody2D.velocity;
                }
                else
                {
                    targetVelocity = Vector2.zero;
                }

                float angleToApply = engine.torqueForce;

                if(targetVelocity != Vector2.zero)
                {
                    
                    float angle = Vector2.Angle(shipObject.rigidbody2D.velocity, targetVelocity);
                    Vector3 crossVec = Vector3.Cross(shipObject.rigidbody2D.velocity, targetVelocity);

                    if (crossVec.z < 0.0f)
                    {
                        angle = -angle;
                    }

                    LogMessage("Angle: " + angle);

                    
                    angleToApply = engine.torqueForce * Mathf.Sign(-angle);
                    
                }

                //if (Mathf.Abs(shipObject.rigidbody2D.angularVelocity) <= 25.0f)
                {
                    engine.ApplyTorque(angleToApply);
                }	
                engine.ApplyAccelerateForce(1.0f);
            }
        }
    }
}

public static class CollisionAvoidanceFunctions
{
    public static GameObject ObjectOfCollision(GameObject self)
    {
        //ConeChecker checker = self.transform.GetComponentInChildren<ConeChecker>();
        /*NeedConeChecker checkerGetter = self.transform.GetComponent<NeedConeChecker>();
        ConeChecker checker = checkerGetter.checker;
        if(checker != null)
        {
            return checker.currentCollision;
        }
        else
        {
            throw new Exception("Could not get ConeChecker from child");
        }*/

        ConeChecker3D checkerGetter3D = self.GetComponentInChildren<ConeChecker3D>();

        if (checkerGetter3D != null)
        {
            return checkerGetter3D.currentCollision;
        }
        else
        {
            throw new Exception("Could not get ConeChecker from child");
        }
    }
}