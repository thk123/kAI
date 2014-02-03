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
        SetLookAt(direction);
        currentDirection = direction.normalized * maxSpeed;
        currentDirection.y = 0;
    }

    internal void SetLookAt(Vector3 newLook)
    {
        if (newLook != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(newLook);
        }
    }
}
