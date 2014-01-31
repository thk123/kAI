using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConeChecker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        trackingObjects = new HashSet<GameObject>();

        print("1: " + WillBeCollision(Vector2.up, Vector2.zero, Vector2.zero, Vector2.up));
	}

    HashSet<GameObject> trackingObjects = new HashSet<GameObject>();

    public GameObject objectToCheckFor;

    public GameObject currentCollision
    {
        get;
        private set;
    }

	// Update is called once per frame
	void Update () {

        transform.position = objectToCheckFor.transform.position;
        transform.rotation = objectToCheckFor.transform.rotation;


        Vector2 ourVelocity = objectToCheckFor.rigidbody2D.velocity;

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

            if (WillBeCollision(objectToCheckFor.transform.GetPosition2D(), ourVelocity, trackingShip.transform.GetPosition2D(), theirVelocity))
            {
                print("There will be a collision");
                currentCollision = trackingShip.collider2D.gameObject;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != objectToCheckFor)
        {
            if (!trackingObjects.Contains(other.gameObject))
            {
                print("Adding object: " + other.gameObject.name + "Count: " + trackingObjects.Count);
                trackingObjects.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != objectToCheckFor)
        {
            print("Removing object: " + other.gameObject.name);
            trackingObjects.Remove(other.gameObject);
        }
    }


    static bool WillBeCollision(Vector2 lhsVelocity, Vector2 lhsPosition, Vector2 rhsVelocity, Vector2 rhsPosition)
    {
        Ray2D firstPath = new Ray2D(lhsPosition, lhsVelocity);
        Ray2D secondPath = new Ray2D(rhsPosition, rhsVelocity);

        if (lhsVelocity.y != rhsVelocity.y)
        {
            Vector2 lhs = lhsPosition - rhsPosition;
            Vector2 rhs = rhsVelocity - lhsVelocity;

			if(rhs.x > 0.0f)
			{

            float xT = lhs.x / rhs.x;

            if((rhs.y * xT).IsApproximaitely(lhs.y, 0.1f))
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
				if(lhs.x.IsApproximaitely(0.0f, 0.1f))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

        }
        else
        { 
            return false; 
        }
    }
}
