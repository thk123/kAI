using UnityEngine;
using System.Collections;

public class StartRotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        rigidbody2D.AddTorque(Mathf.PI / 2.0f / Time.fixedDeltaTime * collider2D.GetColliderRadius2D());
	}
	
	// Update is called once per frame
	void Update () {
	}
}
