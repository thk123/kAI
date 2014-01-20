using UnityEngine;
using System;
using System.Collections;

public class AlternateMoveToPoint : MonoBehaviour, IOrderReciever {

    Vector2 target;
    ShipEngine engine;
    float time;
    float startTime;

    
    private float angleToGo;

	// Use this for initialization
	void Start () {
        engine = GetComponent<ShipEngine>();
        target = transform.position;
        time = 0.0f;
        startTime = 0.0f;
	}    

    static float Move(float currentVelocity, float distanceRemaining, float maxForce)
    {
        float decelerationDistance = (currentVelocity * currentVelocity) / (2 * maxForce);

        bool movingInRightDirection = Mathf.Sign(currentVelocity * distanceRemaining) > 0;

        if (movingInRightDirection)
        {
            // then the velocity is moving in the direction
            if (decelerationDistance >= Mathf.Abs(distanceRemaining))
            {
                return Decelerate(currentVelocity, maxForce);
            }
            else
            {
                return maxForce * Mathf.Sign(currentVelocity);
            }
        }
        else
        {
            return -maxForce * Mathf.Sign(currentVelocity);
        }
    }

    // Calculates a force to halt the current velocity (not overshoot)
    static float Stop(float currentVelocity, float maxForce)
    {
        float forceToApply = ((-currentVelocity)) / Time.deltaTime;
        return forceToApply;
    }

    // Calculates a torque to halt the current spin (not overshoot)
    static float StopSpin(float currentAngularVelocity, float maxForce)
    {
        float forceToApply = ((-currentAngularVelocity)) / Time.deltaTime;
        return forceToApply;
    }

    // Calculates a force to spin the remaining anlge, slowing down when we need to 
    static float Spin(float currentAngularVelocity, float angleRemaining, float maxForce)
    {
        float decelerationDistance = (currentAngularVelocity * currentAngularVelocity) / (2 * maxForce);

        bool movingInRightDirection = Mathf.Sign(currentAngularVelocity * angleRemaining) > 0;

        float directionOfForce;
        if (movingInRightDirection)
        {
            if (decelerationDistance <= Mathf.Abs(angleRemaining))
            {
                directionOfForce = 1;
            }
            else
            {
                return DecelerateSpin(currentAngularVelocity, maxForce);
            }
        }
        else
        {
            directionOfForce = -1;
        }
        return directionOfForce * maxForce * Mathf.Sign(currentAngularVelocity);
    }

    // Calculates a force to slow down the ship
    static float Decelerate(float currentVelocity, float maxForce)
    {
        float signOfMovement = Mathf.Sign(currentVelocity);
        if (Mathf.Sign(currentVelocity - (signOfMovement * maxForce * Time.deltaTime)) != signOfMovement)
        {
            return Stop(currentVelocity, maxForce);
        }
        else
        {
            return -maxForce * signOfMovement; 
        }
            
    }

    // Calculates a torque to slow down spin. 
    static float DecelerateSpin(float currentVelocity, float maxForce)
    {
        float signOfMovement = Mathf.Sign(currentVelocity);
        if (Mathf.Sign(currentVelocity - (signOfMovement * maxForce * Time.deltaTime)) != signOfMovement)
        {
            return StopSpin(currentVelocity, maxForce);
        }
        else
        {
            return -maxForce * signOfMovement;
        }

    }

	// Update is called once per frame
    void Update()
    {
        Vector2 moveVector = target - (Vector2)transform.position;

        float distanceToGo = (moveVector).magnitude;
        float direction = Mathf.Sign(Vector2.Dot(moveVector, transform.right));

        float currentVelocity = Vector2.Dot(rigidbody2D.velocity, transform.right);

        float maxForce = GetComponent<ShipEngine>().accelerationForce;


        // identify the angle (using cross to determine the orientation
        angleToGo = Vector2.Angle(transform.right, moveVector) * Mathf.Deg2Rad;
        Vector3 cross = Vector3.Cross(transform.right, moveVector);

        if (cross.z < 0.0f)
        {
            angleToGo = -angleToGo;
        }

        float angularVelocity = rigidbody2D.angularVelocity * Mathf.Deg2Rad;
        float maxTorque = engine.torqueForce;

        float forceToApply;
        float torqueToApply;

        // are we a reasonable distance awway from the target
        if (distanceToGo > 0.01f)
        {
            // is the angle within a small threshhold
            if (Mathf.Abs(angleToGo) <= 1.0f * Mathf.Deg2Rad)
            {
                // we are facing in the right direction so stop spinning and start moving. 
                torqueToApply = DecelerateSpin(angularVelocity, maxTorque);
                forceToApply = Move(currentVelocity, distanceToGo * direction, maxForce);
            }
            else // angle > 1 degree, i.e. not pointing in the right direction
            {
                // CHEAT: here we ask the engine to stop us moving perpendicular to our facing
                // if we don't do this then we will always have some lateral movement with no way of 
                // stopping it or else we'd have to break before turning
                engine.LateralStabilise();

                // are we moving reasonably fast (along the direction of motion) so we slow down to correct our rotation
                if (currentVelocity > 0.1f)
                {
                    // Slow down and fix angle
                    forceToApply = Decelerate(currentVelocity, maxForce);
                    torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                }
                else //currentVelocity < 0.1 so we just fix our angle
                {
                    // fix angle (if we slow down we get a jerky motion where we break at various points along the path)
                    torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                    forceToApply = 0.0f;
                }
            }
        }
        else // we are near the target
        {
            forceToApply = Decelerate(currentVelocity, maxForce);
            torqueToApply = DecelerateSpin(angularVelocity, maxTorque);
        }

        float oldTime = time;
        time = Time.fixedTime - startTime;

        float variableFixedDeltaTime = time - oldTime;
        float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;

        engine.ApplyAccelerateForce(forceToApply * fixedRatio);
        engine.ApplyTorque(torqueToApply * fixedRatio * rigidbody2D.mass * collider2D.GetColliderRadius2D());
    }

    

    public void GiveOrder(Vector2 temp)
    {
        target = temp;
    }
}
