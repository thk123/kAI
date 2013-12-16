﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShipEngine))]
public class BasicShipController : MonoBehaviour {

	ShipEngine engine;

	// Use this for initialization
	void Start () {
		engine = GetComponent<ShipEngine>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow))
		{
			engine.ApplyAccelerateForce();
		}

		if(Input.GetKey(KeyCode.DownArrow))
		{
			engine.ApplyDeccelerateForce();
		}

		if(Input.GetKey(KeyCode.LeftArrow))
		{
			engine.ApplyTorque(10.0f);
		}
		else if(Input.GetKey(KeyCode.RightArrow))
		{
			engine.ApplyTorque(-10.0f);
		}
	}
}
