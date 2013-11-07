using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	
	public float maxSpeed = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void MoveLeft()
	{
		transform.Translate(-maxSpeed * Time.deltaTime, 0.0f, 0.0f);
	}	
	
	public void MoveRight()
	{
		transform.Translate(maxSpeed * Time.deltaTime, 0.0f, 0.0f);
	}
}
