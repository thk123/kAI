using UnityEngine;
using System.Collections;

public class StartMover : MonoBehaviour {

    public float initalForce = 1.0f;

	// Use this for initialization
	void Start () {
        rigidbody2D.AddForce(Vector3.right * initalForce / Time.fixedDeltaTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
