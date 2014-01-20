using UnityEngine;
using System.Collections;
using kAI.Core;

public class AIRotationalStabilize : kAICodeBehaviour
{
	public AIRotationalStabilize()
		:base(null)
	{}

	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		GameObject ship = lUserData as GameObject;
		if(ship != null)
		{
			ShipEngine engine = ship.GetComponent<ShipEngine>();

			//Vector3 currentVelocity = ship.rigidbody.velocity;
			Vector3 currentRotationalVelocity = ship.rigidbody.angularVelocity;

			// we are always rotating around z, so we just try and stop tgat

			engine.ApplyTorque(-1 * Mathf.Sign(currentRotationalVelocity.y) * engine.torqueForce);


		}
	}
}

public class AIBreak : kAICodeBehaviour
{
	public AIBreak()
		:base(null)
	{}

	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		GameObject ship = lUserData as GameObject;
		if(ship != null)
		{
			ShipEngine engine = ship.GetComponent<ShipEngine>();
			Vector3 currentVelocity = ship.rigidbody.velocity;
		}
	}
}



