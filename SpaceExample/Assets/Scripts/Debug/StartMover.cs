using UnityEngine;
using System.Collections;

public class StartMover : MonoBehaviour {

    public float initalForce = 1.0f;

    public Vector2 direction;

	// Use this for initialization
	void Start () {
        rigidbody2D.AddForce(direction * initalForce / Time.fixedDeltaTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
