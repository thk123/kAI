using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ShipEngine : MonoBehaviour {

	public float accelerationForce;
	public float decelerationForce;
	public float torqueForce;


	public float forceToApply;
	public float torqueToApply;

	// Use this for initialization
	void Start () {
		forceToApply = 0.0f;
		torqueToApply = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		rigidbody.AddForce(Mathf.Min (forceToApply, accelerationForce) * transform.right);
		forceToApply = 0.0f;

		rigidbody.AddRelativeTorque(transform.up * Mathf.Min(torqueToApply, torqueForce));
		torqueToApply = 0.0f;
	}

	public void ApplyAccelerateForce(float requestedForce)
	{
		forceToApply += requestedForce;
	}

	public void ApplyDeccelerateForce(float requestedForce)
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
		torqueToApply += requestedTorque;
	}
	
}
