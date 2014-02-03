using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShipEngine : MonoBehaviour {

    public float maxSpeed;

    public Vector3 currentDirection;

	// Use this for initialization
	void Start () 
	{
        currentDirection = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
        transform.position += currentDirection;

	}

    

    public void SetDirection(Vector3 direction)
    {
        currentDirection = direction.normalized * maxSpeed;
        currentDirection.y = 0;
    }
}
