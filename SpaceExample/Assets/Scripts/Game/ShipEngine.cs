using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipEngine : MonoBehaviour {

	public float accelerationForce;
	public float decelerationForce;
	public float torqueForce;


	public float forceToApply;
    float lateralForceToApply;
	public float torqueToApply;

    float time;
    float startTime;

	// Use this for initialization
	void Start () {
		forceToApply = 0.0f;
		torqueToApply = 0.0f;

        time = Time.fixedTime;
        startTime = time;
        
	}
	// Update is called once per frame
	void Update () {
       
	}

	void FixedUpdate()
	{
        float oldTime = time;
        time = Time.fixedTime - startTime;

        float variableFixedDeltaTime = time - oldTime;
        float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;


		forceToApply = Mathf.Sign(forceToApply) * Mathf.Min(Mathf.Abs(forceToApply), accelerationForce);
        Vector2 forceVector = forceToApply * transform.right;

		rigidbody2D.AddForce(forceToApply * transform.right * fixedRatio);
		forceToApply = 0.0f;

        rigidbody2D.AddForce(lateralForceToApply * transform.up * fixedRatio);
        lateralForceToApply = 0.0f;

		torqueToApply = Mathf.Min(torqueToApply, torqueForce);
        rigidbody2D.AddTorque(torqueToApply * fixedRatio * rigidbody2D.mass * collider2D.GetColliderRadius2D());

        
		torqueToApply = 0.0f;
	}

	public void ApplyAccelerateForce(float requestedForce)
	{
		forceToApply += requestedForce;
	}

    public void LateralStabilise()
    {
        float lateralSpeed = Vector2.Dot(rigidbody2D.velocity, transform.up);
        lateralForceToApply = -lateralSpeed / Time.deltaTime;

    }

	public void ApplyTorque(float requestedTorque)
	{
		torqueToApply += requestedTorque;
	}	
}

static class Extensions
{
    public static Vector2 GetPosition2D(this Transform transform)
    {
        return transform.position;
    }

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

    public static bool IsApproximaitely(this float aValue, float bValue, float error = float.Epsilon)
    {
        return Mathf.Abs(aValue - bValue) < error;
    }

}
