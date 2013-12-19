﻿using UnityEngine;
using System.Collections;

using kAI.Core;

public class AIRotateToPoint : kAICodeBehaviour
{
	kAIDataPort<float> targetAngle;
	
	float force;
	float time;

	public float applyingForce
	{
		get;
		private set;
	}

	bool firstRun = true;

	public AIRotateToPoint() 
		:base(null)
	{
		targetAngle = new kAIDataPort<float>("Data", kAIPort.ePortDirection.PortDirection_In, null);
		AddExternalPort(targetAngle);
	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		if(firstRun)
		{
			OnActivate();
			firstRun = false;
		}

		time += lDeltaTime;
		if(time <= Mathf.PI)
		{
			GameObject ship = lUserData as GameObject;
			if(ship != null)
			{
				applyingForce = ((Mathf.Cos(time) * force));

				ShipEngine engine = ship.GetComponent<ShipEngine>();



				float angularVelocity = ship.rigidbody2D.angularVelocity;
				LogMessage("Actual speed:" + angularVelocity);

				float predictedAngularVelocity = angularVelocity + (applyingForce * lDeltaTime);
				LogMessage("Predicted: " + predictedAngularVelocity);

				if((predictedAngularVelocity < 0.0f && angularVelocity > 0.0f) ||
				   (predictedAngularVelocity > 0.0f && angularVelocity < 0.0f))
				{
					float angularRatio = Mathf.Abs(angularVelocity / (applyingForce * lDeltaTime));
					applyingForce = applyingForce * angularRatio;
				}

                // TODO: This 0.01 actually needs to be a) smaller and b) related to the size of the collider
				engine.ApplyTorque(applyingForce * ship.rigidbody2D.mass * 0.01f);
			}
		}
	}
	
	protected override void OnActivate ()
	{
		float delta = targetAngle.Data;
		force  = delta/2.0f;
		time = 0.0f;
		LogMessage("Force:" + force);
		base.OnActivate ();
	}
	
}