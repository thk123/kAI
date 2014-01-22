using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using kAI.Core;

public class AIRotateToPoint : kAICodeBehaviour
{
	kAIDataPort<float> targetAngle;

    // TODO: Once we have DataPort events we can loose this and just subscribe to on changed
    float currentTargetAngle;
	
    
    MathFunction forceFunction;
    float time;
    float totalTime;
    float startTime;

    ShipEngine engine;

    float debugStartingAngle;

	public AIRotateToPoint() 
		:base(null)
	{
		targetAngle = new kAIDataPort<float>("Data", kAIPort.ePortDirection.PortDirection_In, null);

        LogMessage("adding port");
		AddExternalPort(targetAngle);
	}

    protected override void OnActivate()
    {
        base.OnActivate();
        currentTargetAngle = -1.0f;
        LogMessage("RotateTo " + targetAngle.Data);
        
    }

    // Calculates a torque to halt the current spin (not overshoot)
    static float StopSpin(float currentAngularVelocity, float maxForce)
    {
        float forceToApply = ((-currentAngularVelocity)) / Time.deltaTime;
        return forceToApply;
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

	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
        //LogMessage("Running the update");
        GameObject ship = lUserData as GameObject;
        if (ship != null)
        {
            engine = ship.GetComponent<ShipEngine>();
            float angularVelocity = ship.rigidbody2D.angularVelocity * Mathf.Deg2Rad;
            float angleToGo = targetAngle.Data * Mathf.Deg2Rad;

            float torqueToApply;
            
            if(Mathf.Abs(targetAngle.Data) < 1.0f)
            {
                torqueToApply = DecelerateSpin(angularVelocity, engine.torqueForce);
            }
            else
            {
                torqueToApply = Spin(angularVelocity, angleToGo, engine.torqueForce);
            }
            LogMessage("Angle remaining: " + targetAngle.Data + " Applying force: " + torqueToApply);
            engine.ApplyTorque(torqueToApply);

            /*if (currentTargetAngle != targetAngle.Data)
            {
                
                debugStartingAngle = ship.transform.eulerAngles.z;

                ComputeForces(targetAngle.Data * Mathf.Deg2Rad, ship.rigidbody2D.angularVelocity * Mathf.Deg2Rad, engine);
                startTime = Time.fixedTime;
                currentTargetAngle = targetAngle.Data;

                LogMessage("Computing forces, time: " + startTime+  "Angle: " + currentTargetAngle);
            }

            float oldTime = time;
            time = Time.fixedTime - startTime;

            if (time <= totalTime)
            {
                float applyingForce = forceFunction.GetValue(time);

                // The force is given in radians but for whatever reason the angular velocity is in degrees per second
                // we convert the acceleration in to degrees per second squared to predict the next frames angularVelocity
                float currentAngularVelocity = ship.rigidbody2D.angularVelocity;
                float angularVelocityPrediction = currentAngularVelocity + ((applyingForce * Mathf.Rad2Deg) * lDeltaTime);                

                // TODO: we really should have the second param >= 0.0f but have an issue with only starting moving on the second frame. 
                if (time > 0.0f && // we are not interested if we are just starting
                   ((currentAngularVelocity < 0.0f && angularVelocityPrediction > 0.0f) ||
                   (currentAngularVelocity > 0.0f && angularVelocityPrediction < 0.0f)))
                {
                    float ratio = Mathf.Abs((currentAngularVelocity / ((applyingForce * Mathf.Rad2Deg) * lDeltaTime)));
                    applyingForce = applyingForce * ratio;*/

                    // This version is accurate to within 4 dp
                    /*float finishingAngle = ship.transform.eulerAngles.z;
                    float difference = finishingAngle - debugStartingAngle;
                    Debug.Log("Difference: " + difference + ", " + difference * Mathf.Deg2Rad);*/

                    // We have reversed the direction so we must have arrived
                    // TODO: The time is short, we reach the target before we mean to
                    // This is possibly due to going to slow when we switch from cos to hard deceleration
                    /*LogMessage("deactivating from the method");
                    Deactivate();
                }

                engine.ApplyTorque(applyingForce);
            }
            else
            {
                LogMessage("Deactivating due to no time: " + time + ", " + totalTime);
                Deactivate();
            }*/
        }      
	}

    void ComputeForces(float angle, float currentAngularVelocity, ShipEngine engine)
    {
        // We want to minimize the time given the force of the engine

        // TODO: Lots of assumptions here: 
        // - code for piDistance > 2*maxForce may be invalid, needs some maths work
        // - assume maxforce = 1
        // - If we are going to fast need to support going round more than once or fuck it?
        // - if it is shorter to go CCW will still go CW
        //    - as an added note, need to work out which way to go factoring in current velocity. 

        float direction = Mathf.Sign(angle);
        angle = Mathf.Abs(angle);


        float maxForce = engine.torqueForce ;
        Debug.Log("Angle: " + angle + ", Vel: " + currentAngularVelocity + ", mF: " + maxForce); ;

        // Here we work out this distance left after assuming max deceleration to get rid of the 
        // starting (angular) velocity. 
        float piDistance = angle - ((currentAngularVelocity * currentAngularVelocity) / (2 * maxForce));

        if(piDistance < 0.0f )
        {
            piDistance = 0.0f;
            //throw new NotImplementedException("We need to go round more than once");
        }

        // The time required to zero the current angular velocity.
        float hardDecelTime = currentAngularVelocity / maxForce;
        
        // if we are under this limit, then the optimal acceleration is just the cos curve, compressed if we can do it in less time
        if (piDistance <= 2 * maxForce)
        {
            

            // pi squared, used in below formula
            float pi2 = Mathf.PI * Mathf.PI;

            // solution to the quadratic equation 2d^2 + udpi^2 -dpi^2, where d is the piDistance, and u is initial angular velocity
            float piTime = (0.25f * Mathf.PI) * (Mathf.Sqrt(pi2 * Mathf.Pow(currentAngularVelocity, 2) + (8 * piDistance)) - Mathf.PI * currentAngularVelocity);

            totalTime = piTime + hardDecelTime;
            
            forceFunction = new MathFunction(0.0f, totalTime);

            // The time factor represents how much we have compressed the cos curve
            float timeFactor = Mathf.PI / piTime;

            // the compressed cos curve
            forceFunction.AddSegment((point) =>
            {
                return ((timeFactor * (Mathf.Cos((point * timeFactor)) ))) * direction;
            }, 0.0f, piTime);

            // the hard decleration time
            forceFunction.AddSegment((point) =>
            {
                return -maxForce * direction;
            }, piTime, piTime + hardDecelTime);


        }
        else
        {
            LogMessage("Doing my stuff");

            // We do everything for piDistance then just add the decerlation on to the end. 

            Func<float, float> fullAccel = (point) => { return maxForce * direction; };
            Func<float, float> fullDeccel = (point) => { return -maxForce * direction; };

            // we compute the time to do full accerlation (and deceleration)
            float t0 = ((-Mathf.PI * maxForce) + (Mathf.Sqrt(
                            (((Mathf.PI * Mathf.PI) - 8) * maxForce) + 4.0f * piDistance)) * Mathf.Sqrt(maxForce)) /
                            (2.0f * maxForce);

            forceFunction = new MathFunction(0.0f, t0 + Mathf.PI + t0 + hardDecelTime);

            // We do full acceleration between 0 and t0
            forceFunction.AddSegment(fullAccel, 0.0f, t0);

            // Do a cos acceleration as above between t0 and t0 + pi
            forceFunction.AddSegment((lPoint) =>
            {
                return (Mathf.Cos(lPoint - t0) * maxForce) * direction;
            }, t0, t0 + Mathf.PI);

            // Then a full deceleration at the end 
            forceFunction.AddSegment(fullDeccel, t0 + Mathf.PI, Mathf.PI + (2 * t0) + hardDecelTime);

            // The total time is the time spend doing full acceleration, plus our cos section plus time spent doing full deceleration.
            totalTime = t0 + Mathf.PI + t0 + hardDecelTime;
        }

        forceFunction.EndAddSegment();
        time = 0.0f;
    }
	
}