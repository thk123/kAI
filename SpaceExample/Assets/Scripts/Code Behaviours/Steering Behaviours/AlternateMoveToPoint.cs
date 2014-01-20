using UnityEngine;
using System;
using System.Collections;

public class AlternateMoveToPoint : MonoBehaviour, IOrderReciever {

    Vector2 target;
    ShipEngine engine;
    float time;
    float startTime;

    
    private float angleToGo;

	// Use this for initialization
	void Start () {
        engine = GetComponent<ShipEngine>();
        target = transform.position;
        time = 0.0f;
        startTime = 0.0f;

        RunTests();
	}

    void RunTests()
    {
        RunSpinTests();
    }

    abstract class Test
    {
        protected float predictedAnswer;
        public Test(float predicted)
        {
            predictedAnswer = predicted;
        }

        public void RunTest(int index)
        {
            float result = RunTest();
            if (Mathf.Sign(result) != Mathf.Sign(predictedAnswer))
            {
                throw new Exception("Test " + GetType() + ":" + index + "failed, result: " + result);
            }
            else
            {
                print("Test " + GetType() + ":" + index + "passed with result " + result);
            }
        }

        protected abstract float RunTest();
    }

    class SpinTest : Test
    {
        float angularVelocity;
        float angleRemaining;

        public SpinTest(float velocity, float angle, float predicted)
            :base(predicted)
        {
            angularVelocity = velocity;
            angleRemaining = angle;
        }

        protected override float RunTest()
        {
            return Spin(angularVelocity, angleRemaining, 1.0f);
        }
    }

    class MoveTest : Test
    {
        private float currentVelocity;
        private float distanceRemaining;

        public MoveTest(float velocity, float distance, float predicted)
            :base(predicted)
        {
            currentVelocity = velocity;
            distanceRemaining = distance;
        }

        protected override float RunTest()
        {
            return Move(currentVelocity, distanceRemaining, 1.0f);
        }
    }

    void RunSpinTests()
    {
        Test[] tests = new Test[] {
            new SpinTest(10.0f, 90.0f, 1.0f),       // 0 
            new SpinTest(10.0f, 10.0f, -1.0f),      // 1
            new SpinTest(10.0f, -10.0f, -1.0f),     // 2
            new SpinTest(10.0f, -100.0f, -1.0f),    // 3

            new SpinTest(-10.0f, 90.0f, 1.0f),      // 4
            new SpinTest(-10.0f, 10.0f, 1.0f),      // 5
            new SpinTest(-10.0f, -10.0f, 1.0f),     // 6
            new SpinTest(-10.0f, -100.0f, -1.0f),   // 7

            new MoveTest(10.0f, 90.0f, 1.0f),       // 8 
            new MoveTest(10.0f, 10.0f, -1.0f),      // 9
            new MoveTest(10.0f, -10.0f, -1.0f),     // 10
            new MoveTest(10.0f, -100.0f, -1.0f),    // 11

            new MoveTest(-10.0f, 90.0f, 1.0f),      // 12
            new MoveTest(-10.0f, 10.0f, 1.0f),      // 13
            new MoveTest(-10.0f, -10.0f, 1.0f),     // 14
            new MoveTest(-10.0f, -100.0f, -1.0f),   // 15   
        };

        for(int i = 0; i < tests.Length; ++i)
        {
            tests[i].RunTest(i);
        }
    }
	
    static float Stop(float currentVelocity, float maxForce)
    {
        float forceToApply = ((-currentVelocity)) / Time.deltaTime;
        return forceToApply;
    }

    static float StopSpin(float currentAngularVelocity, float maxForce)
    {
        float forceToApply = ((-currentAngularVelocity)) / Time.deltaTime;
        return forceToApply;
    }

    static float Spin(float currentAngularVelocity, float angleRemaining, float maxForce)
    {
        /*float decelerationDistance = (currentAngularVelocity * currentAngularVelocity) / (2 * maxForce);

        float angleRemainingDirection = Mathf.Sign(angleRemaining);

        bool shouldDecelerate = decelerationDistance >= Mathf.Abs(angleRemaining);
        float directionOfForce = shouldDecelerate ? -1 : 1;

        return directionOfForce * maxForce * angleRemainingDirection;*/

        float decelerationDistance = (currentAngularVelocity * currentAngularVelocity) / (2 * maxForce);

        print("Deceleration DIstnacce: " + decelerationDistance);

        bool movingInRightDirection = Mathf.Sign(currentAngularVelocity * angleRemaining) > 0;

        float directionOfForce;
        if(movingInRightDirection)
        {
            if(decelerationDistance <= Mathf.Abs(angleRemaining))
            {
                directionOfForce = 1;
            }
            else
            {
                return DecelerateSpin(currentAngularVelocity, maxForce);
            }
        }
        else
        {
            directionOfForce = -1;
        }
        //print("movingInRightDir: " + movingInRightDirection + ", Deceleration Distnacce: " + decelerationDistance + ", current vel:" + currentAngularVelocity);
        return directionOfForce * maxForce * Mathf.Sign(currentAngularVelocity); 
    }

    static float Move(float currentVelocity, float distanceRemaining, float maxForce)
    {
        float decelerationDistance = (currentVelocity * currentVelocity) / (2 * maxForce);

        bool movingInRightDirection = Mathf.Sign(currentVelocity * distanceRemaining) > 0;

        if (movingInRightDirection)
        {
            // then the velocity is moving in the direction
            if (decelerationDistance >= Mathf.Abs(distanceRemaining))
            {
                return Decelerate(currentVelocity, maxForce);
            }
            else
            {
                return maxForce * Mathf.Sign(currentVelocity);
            }
        }
        else
        {
            return -maxForce * Mathf.Sign(currentVelocity);
        }

        /* if(decelerationDistance >= Mathf.Abs(distanceRemaining))
         {
             //breaking
             return -maxForce * Mathf.Sign(currentVelocity * distanceRemaining);
         }
         else
         {
             //accel
             return maxForce * Mathf.Sign(currentVelocity * distanceRemaining);
         }*/
    }

    static float Decelerate(float currentVelocity, float maxForce)
    {
        float signOfMovement = Mathf.Sign(currentVelocity);
        if (Mathf.Sign(currentVelocity - (signOfMovement * maxForce * Time.deltaTime)) != signOfMovement)
        {
            return Stop(currentVelocity, maxForce);
        }
        else
        {
            return -maxForce * signOfMovement; 
        }
            
    }

    static float DecelerateSpin(float currentVelocity, float maxForce)
    {
        float signOfMovement = Mathf.Sign(currentVelocity);
        if (Mathf.Sign(currentVelocity - (signOfMovement * maxForce * Time.deltaTime)) != signOfMovement)
        {
            return StopSpin(currentVelocity, maxForce);
        }
        else
        {
            return -maxForce * signOfMovement;
        }

    }

	// Update is called once per frame
	void Update () {
        Vector2 moveVector = target - (Vector2)transform.position;

        float distanceToGo = (moveVector).magnitude;
        float forceScale = Vector2.Dot(moveVector, transform.right);


        float direction = Mathf.Sign(Vector2.Dot(moveVector, transform.right));
        float currentVelocity = Vector2.Dot(rigidbody2D.velocity, transform.right);
        float maxForce = GetComponent<ShipEngine>().accelerationForce;

        angleToGo = Vector2.Angle(transform.right, moveVector) * Mathf.Deg2Rad;
        Vector3 cross = Vector3.Cross(transform.right, moveVector);

        float angleDirection = 1.0f;
        if(cross.z < 0.0f)
        {
            angleToGo = -angleToGo;
            angleDirection = -1.0f;
        }

        float angularVelocity = rigidbody2D.angularVelocity * Mathf.Deg2Rad;
        float maxTorque = engine.torqueForce;

        float forceToApply;
        float torqueToApply;
        //print("ANGLE: " + (angleToGo * Mathf.Rad2Deg) + "DISTANCE: " + distanceToGo);
        if(distanceToGo > 0.01f)
        {
            //if (Mathf.Abs(angleToGo) <= (3.0f * (Mathf.Log((distanceToGo + 1) * 10)) * Mathf.Deg2Rad))
            if (Mathf.Abs(angleToGo) <= 1.0f * Mathf.Deg2Rad)
            {
                torqueToApply = DecelerateSpin(angularVelocity, maxTorque);
                forceToApply = Move(currentVelocity, distanceToGo * Mathf.Sign(forceScale), maxForce);
            }
            else
            {
                engine.LateralStabilise();
                //forceToApply = Decelerate(currentVelocity, transform.right, maxForce);
                /*if (Mathf.Abs(angleToGo) >= 5.0f * Mathf.Deg2Rad)
                {*/
                    if (currentVelocity > 0.1f)
                    {
                        forceToApply = Decelerate(currentVelocity, maxForce);
                        torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                    }
                    else
                    {
                        torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                        forceToApply = 0.0f;
                    }
                }
               /*else
                {
                    debugString = ("Angle off by a little bit so don't break");
                    torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                    forceToApply = 0.0f;
                }
            }*/
        }
        else
        {
            forceToApply = Decelerate(currentVelocity, maxForce);
            torqueToApply = DecelerateSpin(angularVelocity, maxTorque);
        }

        /*if(direction == -1)
        {
            // the thing is behind us, are we near?

            if(distanceToGo > 0.01)
            {
                // we are still a way away so we spin
                if(angleToGo < 30.0f * Mathf.Deg2Rad)
                {
                    throw new InvalidOperationException("The thing is behind us but the angle is small??");
                }
                else
                {
                    print("Spining around");
                    torqueToApply = Spin(currentVelocity, distanceToGo, maxForce);

                }
                forceToApply = 0.0f;
            }
            else
            {
                forceToApply = Stop(currentVelocity, maxForce);
                torqueToApply = StopSpin(angularVelocity, maxTorque);
            }
        }
        else if (direction == 1)
        {
            if(distanceToGo > 0.01)
            {
                // the target is ahead of us, so we move forward
                if (angleToGo < 5.0f * Mathf.Deg2Rad)
                {
                    torqueToApply = StopSpin(angularVelocity, maxTorque);
                    forceToApply = Move(currentVelocity, distanceToGo, maxForce);
                }
                else
                {
                    print("Keep on spinni");
                    torqueToApply = Spin(angularVelocity, angleToGo, maxTorque);
                    forceToApply = 0.0f; // Stop(currentVelocity, maxForce);
                }
                
               
            }
            else
            {
                forceToApply = Stop(currentVelocity, maxForce);
                torqueToApply = StopSpin(angularVelocity, maxTorque);
            }
        }
        else
        {
            throw new Exception("Unknow direction " + direction);
        }
        */
        float oldTime = time;
        time = Time.fixedTime - startTime;

        float variableFixedDeltaTime = time - oldTime;
        float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;

        engine.ApplyAccelerateForce(forceToApply * fixedRatio);
        engine.ApplyTorque(torqueToApply * fixedRatio * rigidbody2D.mass * collider2D.GetColliderRadius2D());
         // old code
        
        /*
        if(distanceToGo > 0.01f)
        {

            {
                
                float maxForce = GetComponent<ShipEngine>().accelerationForce;
                

                float oldTime = time;
                time = Time.fixedTime - startTime;

                float variableFixedDeltaTime = time - oldTime;
                float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;

                // 1 if we are moving in the same direction as the goal,
                // -1 if we are moving AWAY from the target
                float magicsign = Mathf.Sign(currentVelocity * direction);

                if(magicsign == -1)
                {
                    print("Attempting to stop the fucker mk2 ");
                    // apply the right force to stop dead
                    float  forceToApply2 = ((-currentVelocity)) / Time.deltaTime;
                    engine.ApplyAccelerateForce(forceToApply2);
                    return;
                }

                float decelerationDistance = (currentVelocity * currentVelocity) / (2 * maxForce);


                float forceToApply;


                if(distanceToGo > decelerationDistance)
                {
                    forceToApply = maxForce;
                }
                else
                {
                    forceToApply = -maxForce;
                }
                print("Dist to go:" + distanceToGo + ", deceleration distnace: " + decelerationDistance +", sign:" + direction);
                engine.ApplyAccelerateForce(direction * forceToApply);
            }*/

            /*// we attempt to get closer
            float maxForce = GetComponent<ShipEngine>().torqueForce;

            float oldTime = time;
            time = Time.fixedTime - startTime;

            float variableFixedDeltaTime = time - oldTime;
            float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;

            float angle = Vector2.Angle(transform.right, moveVector);
            print("Angle: " + angle + ", " + Mathf.Abs(angle) + ", " + (Mathf.Abs(angle) > 5.0f));

            float currentAngularVelocity = rigidbody2D.angularVelocity;
            if(Mathf.Abs(angle) != 0.0f)
            {
                float decelerationAngle = ((currentAngularVelocity * currentAngularVelocity * Mathf.Deg2Rad * Mathf.Deg2Rad) / (2 * maxForce));
                print("declerationAngle: " + decelerationAngle);

                float applyingForce;

                if (angle <= decelerationAngle * Mathf.Rad2Deg)
                {
                    applyingForce = -maxForce;
                    print("Applying deceleration");
                }
                else
                {
                    applyingForce = maxForce;
                    print("Apply acceleration");
                }

                float angularVelocityPrediction = currentAngularVelocity + ((applyingForce * Mathf.Rad2Deg) * Time.deltaTime);

                // TODO: we really should have the second param >= 0.0f but have an issue with only starting moving on the second frame. 
                if (time > 0.0f && // we are not interested if we are just starting
                   ((currentAngularVelocity < 0.0f && angularVelocityPrediction > 0.0f) ||
                   (currentAngularVelocity > 0.0f && angularVelocityPrediction < 0.0f)))
                {
                    float ratio = Mathf.Abs((currentAngularVelocity / ((applyingForce * Mathf.Rad2Deg) * Time.deltaTime)));
                    applyingForce = applyingForce * ratio;
            */
                    // This version is accurate to within 4 dp
                    /*float finishingAngle = ship.transform.eulerAngles.z;
                    float difference = finishingAngle - debugStartingAngle;
                    Debug.Log("Difference: " + difference + ", " + difference * Mathf.Deg2Rad);*/

                    // We have reversed the direction so we must have arrived
                    // TODO: The time is short, we reach the target before we mean to
                    // This is possibly due to going to slow when we switch from cos to hard deceleration
             /*
                }
       
                engine.ApplyTorque(applyingForce * rigidbody2D.mass * collider2D.GetColliderRadius2D() * fixedRatio);
                
            }*/

        /*
        }
        else
        {
            print("Attempting to stop the fucker");
            // apply the right force to stop dead
            float forceToApply = ((-currentVelocity)) / Time.deltaTime;
            engine.ApplyAccelerateForce(forceToApply);
        }*/



	}

    

    public void GiveOrder(Vector2 temp)
    {
        target = temp;
        //target.y = transform.position.y;
    }
}
