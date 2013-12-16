using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ShipEngine : MonoBehaviour {

	public float accelerationForce;
	public float decelerationForce;
	public float torqueForce;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ApplyAccelerateForce(float requestedForce = -1.0f)
	{
		if(requestedForce < 0.0f)
		{
			requestedForce = accelerationForce;
		}

		if(requestedForce > 0.0f)
		{
			rigidbody.AddForce(Mathf.Min (requestedForce, accelerationForce) * transform.right);
		}
	}

	public void ApplyDeccelerateForce(float requestedForce = -1.0f)
	{
		if(requestedForce < 0.0f)
		{
			requestedForce = decelerationForce;
		}
		
		if(requestedForce > 0.0f)
		{
			rigidbody.AddForce(Mathf.Min (requestedForce, decelerationForce) * -1 * transform.right);
		}
	}

	public void ApplyTorque(float requestedTorque)
	{
		rigidbody.AddTorque(transform.up * Mathf.Min(requestedTorque, torqueForce));
	}
	
}
