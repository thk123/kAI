using System;
using kAI.Core;
using UnityEngine;

public class AdvanceTo : kAICodeBehaviour
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
	
	public AdvanceTo() 
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
				
				Vector3 currentVelocity = ship.rigidbody.velocity;
				Vector3 direction = ship.transform.right;

				float velocityAlongDir = Vector3.Dot(currentVelocity, direction);
				LogMessage("Actual Velocity: " + velocityAlongDir);
				float velocityPrediction = velocityAlongDir + (applyingForce * lDeltaTime);
				LogMessage("Predicted Velocity: " + velocityPrediction);

				if((velocityPrediction < 0.0f  && velocityAlongDir > 0.0f ) ||
				   (velocityPrediction > 0.0f  && velocityAlongDir < 0.0f ) )
				{
					LogMessage("Low Power");
					float ratio = Mathf.Abs((velocityAlongDir / (applyingForce * lDeltaTime)));
					applyingForce = applyingForce * ratio;
					time = Mathf.PI  * 2.0f;
				}
				
				engine.ApplyAccelerateForce(applyingForce * ship.rigidbody.mass);
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

