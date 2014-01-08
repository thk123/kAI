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

	public AIRotateToPoint() 
		:base(null)
	{
		targetAngle = new kAIDataPort<float>("Data", kAIPort.ePortDirection.PortDirection_In, null);
		AddExternalPort(targetAngle);
	}

    

	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
        GameObject ship = lUserData as GameObject;
        if (ship != null)
        {
            engine = ship.GetComponent<ShipEngine>();
            if (currentTargetAngle != targetAngle.Data)
            {
                ComputeForces(targetAngle.Data, engine);
                startTime = Time.fixedTime;
                currentTargetAngle = targetAngle.Data;
            }

            float oldTime = time;
            time = Time.fixedTime - startTime;

            float variableFixedDeltaTime = time - oldTime;
            float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;
            if (time <= totalTime)
            {
                float applyingForce = forceFunction.GetValue(time) * fixedRatio;

                // The force is given in radians but for whatever reason the angular velocity is in degrees per second
                // we convert the acceleration in to degrees per second squared to predict the next frames angularVelocity
                float currentAngularVelocity = ship.rigidbody2D.angularVelocity;
                float angularVelocityPrediction = currentAngularVelocity + ((applyingForce * Mathf.Rad2Deg) * lDeltaTime);

                // TODO: we really should have the second param >= 0.0f but have an issue with only starting moving on the second frame. 
                if (time > 0.0f && // we are not interested if we are just starting
                   ((angularVelocityPrediction < 0.0f && angularVelocityPrediction > 0.0f) ||
                   (angularVelocityPrediction > 0.0f && angularVelocityPrediction < 0.0f)))
                {
                    float ratio = Mathf.Abs((angularVelocityPrediction / (applyingForce * lDeltaTime)));
                    applyingForce = applyingForce * ratio;

                    // We have reversed the direction so we must have arrived
                    Deactivate();
                }

                engine.ApplyTorque(applyingForce * ship.rigidbody2D.mass * ship.collider2D.GetColliderRadius2D());
            }
        }

        
	}

    void ComputeForces(float angle, ShipEngine engine)
    {
        // We want to minimize the time given the force of the engine

        float maxForce = engine.torqueForce ;

        // if we are under this limit, then the optimal acceleration is just the cos curve, compressed if we can do it in less time
        if (angle <= 2 * maxForce)
        {
            totalTime = Mathf.Acos((maxForce - angle) / maxForce);
            forceFunction = new MathFunction(0.0f, totalTime);

            // The time factor represents how much we have compressed the cos curve
            float timeFactor = Mathf.PI / totalTime;

            forceFunction.AddSegment((point) =>
            {
                return ((timeFactor * Mathf.Cos(point * timeFactor) * maxForce));
            }, 0.0f, totalTime);


        }
        else
        {
            Func<float, float> fullAccel = (point) => { return maxForce; };
            Func<float, float> fullDeccel = (point) => { return -maxForce; };

            // we compute the time to do full accerlation (and deceleration)
            float t0 = ((-Mathf.PI * maxForce) + (Mathf.Sqrt(
                            (((Mathf.PI * Mathf.PI) - 8) * maxForce) + 4.0f * angle)) * Mathf.Sqrt(maxForce)) /
                            (2.0f * maxForce);

            forceFunction = new MathFunction(0.0f, t0 + Mathf.PI + t0);

            // We do full acceleration between 0 and t0
            forceFunction.AddSegment(fullAccel, 0.0f, t0);

            // Do a cos acceleration as above between t0 and t0 + pi
            forceFunction.AddSegment((lPoint) =>
            {
                return (Mathf.Cos(lPoint - t0) * maxForce);
            }, t0, t0 + Mathf.PI);

            // Then a full deceleration at the end 
            forceFunction.AddSegment(fullDeccel, t0 + Mathf.PI, Mathf.PI + (2 * t0));

            // The total time is the time spend doing full acceleration, plus our cos section plus time spent doing full deceleration.
            totalTime = t0 + Mathf.PI + t0;
        }

        forceFunction.EndAddSegment();
        time = 0.0f;
    }
	
}