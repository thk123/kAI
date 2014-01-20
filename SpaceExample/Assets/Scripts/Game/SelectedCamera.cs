using UnityEngine;
using System.Collections;

public class SelectedCamera : MonoBehaviour {

	public GameObject NothingSelected;
	public Vector3 offset;

	GameObject selected;

	// Use this for initialization
	void Start () {
		selected = NothingSelected;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = selected.transform.position + offset;
		transform.LookAt(selected.transform);
	}

	public void SelectObject(GameObject newSelected)
	{
		selected = newSelected;
	}

	public void DeselectObject()
	{
		selected = NothingSelected;
	}
}
