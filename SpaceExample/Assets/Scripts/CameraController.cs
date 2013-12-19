using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float cameraPanningSpeed = 0.5f;

	Vector3 cameraPanLeft;
	Vector3 cameraPanForward;

	public bool locked;

	// Use this for initialization
	void Start () {
		cameraPanLeft = -1 * transform.right;
		cameraPanForward = transform.up;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			locked = !locked;
		}

		if(!locked)
		{
			if(Input.mousePosition.x <= 10.0f)
			{
				transform.Translate(cameraPanningSpeed * cameraPanLeft, Space.World);
			}


			if(Input.mousePosition.x >= Screen.width - 10.0f )
			{
				transform.Translate(cameraPanningSpeed * -1 * cameraPanLeft, Space.World);
			}

			if(Input.mousePosition.y <= 10.0f)
			{
				transform.Translate(-1 * cameraPanningSpeed * cameraPanForward, Space.World);
			}

			if(Input.mousePosition.y >= Screen.height - 10.0f)
			{
				transform.Translate(cameraPanningSpeed * cameraPanForward, Space.World);
			}

			float scrollValue = Input.GetAxis("Mouse ScrollWheel");

			if(scrollValue > 0.0f)
			{
				transform.Translate(Vector3.forward, Space.Self);
			}
			else if(scrollValue < 0.0f)
			{
				transform.Translate(-1 * Vector3.forward, Space.Self);
			}
		}

	}

	void OnGUI()
	{
		if(locked)
		{
			GUI.TextArea(new Rect(10, 10, 100, 20), "Locked");
		}
	}

}
