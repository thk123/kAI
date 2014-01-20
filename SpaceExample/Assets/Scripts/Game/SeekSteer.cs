using UnityEngine;
using System.Collections;

public class SeekSteer : MonoBehaviour
{

    Locomotor locomotor;

    Vector2 target;

    // Use this for initialization
    void Start()
    {
        target = transform.position;
        locomotor = GetComponent<Locomotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = -transform.position.z;
            Camera camera = Camera.main;

            Vector3 mouseLocation = camera.ScreenToWorldPoint(mousePoint);

            target = mouseLocation;

            /*Vector3 mouseLocation = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            print("Mouse location: " + mouseLocation);
            Debug.Break();
            */
        }

        if ((target - (Vector2)transform.position).sqrMagnitude > 0.01f)
        {
            Vector2 desiredVelocity = target - (Vector2)transform.position;
            float magnitude = desiredVelocity.magnitude;
            float angleToGo = Vector2.Angle(transform.right, desiredVelocity) * Mathf.Deg2Rad;
            Vector3 cross = Vector3.Cross(transform.right, desiredVelocity);

            if (cross.z < 0.0f)
            {
                angleToGo = -angleToGo;
            }

            /*desiredVelocity = desiredVelocity - rigidbody2D.velocity;
            desiredVelocity = desiredVelocity - ((Vector2)transform.up * (rigidbody2D.angularVelocity * Mathf.Deg2Rad));/*/

            desiredVelocity = magnitude * new Vector2(Mathf.Cos(angleToGo), Mathf.Sin(angleToGo));

            locomotor.SetDesiredVector(desiredVelocity);
        }
    }
}
