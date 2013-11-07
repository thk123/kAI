using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
	
	
	CharacterController controller;
	SwordController swordController;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		swordController = GetComponentInChildren<SwordController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A))
		{
			controller.MoveLeft();
		}
		else if(Input.GetKey(KeyCode.D))
		{
			controller.MoveRight();
		}
		
		if(Input.GetKey(KeyCode.W))
		{
			swordController.SwingSword();
		}
	}
}
