using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConeChecker3D : MonoBehaviour {

	// Use this for initialization
	void Start () {
        trackingObjects = new HashSet<GameObject>();

        print("1(T): " + WillBeCollision(Vector2.up, Vector2.zero, Vector2.zero, Vector2.up));

		print("2(T): "+ WillBeCollision(new Vector2(1.0f, 0.0f), new Vector2(1.4f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(3.2f, 0.0f)));

        print("3(T): " + WillBeCollision(Vector2.right, Vector2.zero, Vector2.zero, Vector2.right));
        print("4(F): " + WillBeCollision(-Vector2.right, Vector2.zero, Vector2.zero, Vector2.right));
        print("5(F): " + WillBeCollision(Vector2.right, Vector2.zero, Vector2.zero, -Vector2.right));
        print("6(T): " + WillBeCollision(-Vector2.right, Vector2.zero, Vector2.zero, -Vector2.right));


        print("7(T): " + WillBeCollision(Vector2.up, Vector2.zero, -Vector2.up, Vector2.up));
        print("8(T): " + WillBeCollision(Vector2.up, Vector2.zero, -Vector2.up, Vector2.up));
	}

    HashSet<GameObject> trackingObjects = new HashSet<GameObject>();


    public GameObject currentCollision
    {
        get;
        private set;
    }

	// Update is called once per frame
	void Update () {



        Vector2 ourVelocity = transform.parent.rigidbody2D.velocity;

        currentCollision = null;

        foreach(GameObject trackingShip in trackingObjects)
        {
            Vector2 theirVelocity;
            if (trackingShip.collider2D.rigidbody2D != null)
            {
                theirVelocity = trackingShip.collider2D.rigidbody2D.velocity;
            }
            else
            {
                theirVelocity = Vector2.zero;
            }

            //print("Checking collision between: " + transform.parent.GetPosition2D() + ", " + ourVelocity + ", " + trackingShip.transform.GetPosition2D() + ", " + theirVelocity);

			if (WillBeCollision(transform.parent.GetPosition2D(), ourVelocity, theirVelocity, trackingShip.transform.GetPosition2D()))
            {
                print("There will be a collision");
                currentCollision = trackingShip.collider2D.gameObject;
            }
            else
            {
                //Debug.Break();
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject)
        {
            /*if (!trackingObjects.Contains(other.gameObject))
            {*/
                print("Adding object: " + other.gameObject.name + "Count: " + trackingObjects.Count);
                trackingObjects.Add(other.gameObject.transform.parent.gameObject);
           /* }*/
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject)
        {
            print("Removing object: " + other.gameObject.name);
            trackingObjects.Remove(other.gameObject.transform.parent.gameObject);
        }
    }


    static bool WillBeCollision(Vector2 firstVel, Vector2 firstPos, Vector2 secondVel, Vector2 secondPos)
    {

        // a + bt = c + dt where 
        //		a is first object starting position
        //		b is first object velocity
        //		c is 2nd object starting position
        //		d is 2nd object velocity

        // We rearrange to:
        // a - c = (d-b)t
        Vector2 posDifference = firstPos - secondPos;
        Vector2 velDifference = secondVel - firstVel;

        // is there a difference in velocites between the xs, use it estimate t
        if (velDifference.x != 0.0f)
        {

            // The time for the x values to coincide
            float xT = posDifference.x / velDifference.x;
            if(xT >= 0.0f)
            {
                // does the y values coincide at this point?
                if ((velDifference.y * xT).IsApproximaitely(posDifference.y, 0.3f))
                {
                    // they do
                    return true;
                }
                else
                {
                    // they do not, therefore we won't collide as x and y will match at different times
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else // the difference in x velocities is 0
        {
            if (velDifference.y != 0.0f)
            {
                if (posDifference.x.IsApproximaitely(0.0f, 0.3f))
                {
                    if(Mathf.Sign(firstVel.y) != Mathf.Sign(secondVel.y) 
					   || (firstVel.y != 0 && secondVel.y == 0)
					   || (secondVel.y != 0 && firstVel.y == 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // both have same x and y velocites so moving in paralell => not going to collide
                return false;
            }
        }
    }
}
