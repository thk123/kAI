using UnityEngine;
using System.Collections;

public class Locomotor : MonoBehaviour
{

    Vector2 currentDesire;

    public float maxForce = 1.0f;

    // Use this for initialization
    void Start()
    {
        currentDesire = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float desireLength = currentDesire.magnitude;
        if (desireLength > 0.0f)
        {
            float resultLength = Mathf.Max(desireLength, maxForce);
            currentDesire = (resultLength * currentDesire) / desireLength;

            //rigidbody2D.AddForce(Vector2.Dot(currentDesire, transform.right) * transform.right);
            //rigidbody2D.AddForce(currentDesire);
            //rigidbody2D.AddTorque(Vector2.Dot(currentDesire, transform.up) * 1.0f * rigidbody2D.mass * collider2D.GetColliderRadius2D());
        }
        currentDesire = Vector2.zero;
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(0, 0, 200, 50), currentDesire.ToString());
    }

    public void SetDesiredVector(Vector2 desire)
    {
        currentDesire = desire;
    }
}
