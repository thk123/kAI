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

                

                if (result.HasValue && result.Value.transform != shipObject.transform)
                {
                    LogMessage(result.Value.transform.gameObject.name);
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
