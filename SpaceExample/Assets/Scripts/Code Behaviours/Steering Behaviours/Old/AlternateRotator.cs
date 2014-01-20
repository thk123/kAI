using UnityEngine;
using System.Collections;

public class AlternateRotator : MonoBehaviour {

    public float addAngle = 2.0f;

    float targetAngle;
    float direction;

    float time;
    float startTime;
    ShipEngine engine;

    /*float startingAngle;
    
    */
	// Use this for initialization
	void Start () {
        /*startingAngle = transform.rotation.eulerAngles.z;
        
        targetAngle = targetAngle + startingAngle;
        print("Setting start angle to " + startingAngle);*/
        SetAngle(addAngle);
        engine = GetComponent<ShipEngine>();
        time = Time.fixedTime;
        startTime = time;
	}

    void SetAngle(float angle)
    {
        // Normalise the angle to be between [-180, 180] 

        while(angle > 180.0f)
        {
            angle -= 360.0f;
        }

        while(angle < -180.0f)
        {
            angle += 360.0f;
        }

        float startingAngle = transform.rotation.eulerAngles.z;
        targetAngle = startingAngle + angle;

        targetAngle %= 360.0f;

        print("Target angle: " + targetAngle);

        direction = Mathf.Sign(angle);
    }
	
	// Update is called once per frame
    void Update()
    {

        float oldTime = time;
        time = Time.fixedTime - startTime;

        float variableFixedDeltaTime = time - oldTime;
        float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;

        float currentAngle = transform.rotation.eulerAngles.z;
        float currentVelocity = rigidbody2D.angularVelocity;
        //print("Current Velocity: " + currentVelocity);

        float angleRemaining;

        if (direction > 0)
        {
            // going CCW
            if (targetAngle < currentAngle)
            {
                angleRemaining = (360.0f - currentAngle) + targetAngle;

            }
            else
            {
                angleRemaining = targetAngle - currentAngle;
            }
        }
        else
        {
            // going cw
            if (targetAngle > currentAngle)
            {
                angleRemaining = currentAngle + (360.0f - targetAngle);
            }
            else
            {
                angleRemaining = currentAngle - targetAngle;
            }
        }
        print(angleRemaining);
        if (angleRemaining > 180.0f)
        {
            angleRemaining = 0.0f;
            targetAngle = currentAngle;
            rigidbody2D.angularVelocity = 0.0f;
            Destroy(this);
            return;
        }




        float applyingForce;
        float decelerationDistance = (currentVelocity * currentVelocity) / (2 * engine.torqueForce * Mathf.Rad2Deg);
        if (Mathf.Abs(angleRemaining) > 0.0f)
        {
            if (angleRemaining >= decelerationDistance)
            {
                applyingForce = engine.torqueForce * fixedRatio;
            }
            else
            {
                applyingForce = -engine.torqueForce * fixedRatio;
                if (Mathf.Abs(angleRemaining) <= 1.0f)
                {
                    applyingForce = ((-currentVelocity * Mathf.Deg2Rad)) / Time.deltaTime;
                    if (Mathf.Abs(currentVelocity) > 0.0f)
                    {
                        if (engine.HaltRotation())
                        {
                            applyingForce = 0.0f;

                            Destroy(this);
                        }
                    }
                }
                /*
                float angularVelocityPrediction = currentVelocity + ((applyingForce * Mathf.Rad2Deg) * Time.deltaTime);
                print("Predication: " + angularVelocityPrediction);
                if ((currentVelocity <= 0.0f && angularVelocityPrediction > 0.0f) ||
                   (currentVelocity >= 0.0f && angularVelocityPrediction < 0.0f))
                {
                    float ratio = Mathf.Abs((currentVelocity / (((applyingForce * Mathf.Rad2Deg)) * Time.deltaTime)));
        
                    print("Scaling: " + ratio);
        
                    applyingForce = applyingForce * ratio;
                }*/

            }


        }
        else
        {
            applyingForce = ((-currentVelocity * Mathf.Deg2Rad)) / Time.deltaTime;
            if (Mathf.Abs(currentVelocity) > 0.0f)
            {
                if (engine.HaltRotation())
                {
                    applyingForce = 0.0f;

                    Destroy(this);
                }
            }
        }
        engine.ApplyTorque(applyingForce * rigidbody2D.mass * collider2D.GetColliderRadius2D() * direction);


    }
}
