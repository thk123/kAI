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
        //rigidbody2D.hingeJoint = Quaternion.identity;
        
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

		//rigidbody2D.AddTorque(Mathf.Min(torqueToApply, torqueForce));
        rigidbody2D.AddTorque(torqueToApply);
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
		torqueToApply += requestedTorque;
	}
	
}

static class Extensions
{
    public static float GetColliderRadius2D(this Collider2D collider)
    {
        if(collider is BoxCollider2D)
        {
            // BoxColliders do not work well with trying to produce good behaviour
            
            BoxCollider2D boxCollider = (BoxCollider2D)collider;
            Vector2 scale2d = new Vector2(collider.transform.localScale.x, collider.transform.localScale.y);
            Vector2 colliderScale = 0.5f * boxCollider.size;

            Vector2 scaleVector = new Vector2(colliderScale.x * scale2d.x, colliderScale.y * scale2d.y);

            return scaleVector.sqrMagnitude; /** (0.5f * collider.transform.localScale).sqrMagnitude*/; //0.01f; /*boxCollider.size.magnitude * 0.5f;*/
        }
        else if(collider is CircleCollider2D)
        {
            float radius = ((CircleCollider2D)collider).radius;
            Vector2 scale2d = new Vector2(collider.transform.localScale.x, collider.transform.localScale.y);


            Vector2 scaleVector = new Vector2((0.5f * radius) * scale2d.x, (0.5f * radius) * scale2d.y);

            return scaleVector.sqrMagnitude;
        }
        else if(collider is PolygonCollider2D)
        {
            throw new System.NotImplementedException();
        }
        else
        {
            throw new System.InvalidOperationException();
        }
    }
}
