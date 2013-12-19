using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
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
        // TODO: have removed cap on force to apply since if we miss a fixed update, we need to catch up
        // should move this catch up handling in to here. 
		//forceToApply = Mathf.Sign(forceToApply) * Mathf.Min(Mathf.Abs(forceToApply), accelerationForce);
		rigidbody2D.AddForce(forceToApply * new Vector2(transform.right.x, transform.right.y));
		forceToApply = 0.0f;

		rigidbody2D.AddTorque(Mathf.Min(torqueToApply, torqueForce));
        //rigidbody2D.AddTorque(0.001f);
		torqueToApply = 0.0f;
	}

	public void ApplyAccelerateForce(float requestedForce)
	{
		forceToApply += requestedForce;
	}

	public void ApplyDeccelerateForce(float requestedForce)
	{
		Debug.LogError("Something");
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
		torqueToApply = requestedTorque;
	}
	
}
