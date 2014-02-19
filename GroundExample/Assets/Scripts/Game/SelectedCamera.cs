using UnityEngine;
using System.Collections;

public class SelectedCamera : MonoBehaviour {

	public GameObject NothingSelected;
	public Vector3 offset;

	SquadLeader selected;

	// Use this for initialization
	void Start () {
        selected = null;
	}
	
	// Update is called once per frame
	void Update () {
        if (selected != null)
        {
            transform.position = selected.SquadPosition + offset;
            transform.LookAt(selected.SquadPosition);
        }
        else
        {
            transform.position = NothingSelected.transform.position;
        }
	}

	public void SelectObject(SquadLeader newSelected)
	{
		selected = newSelected;
	}

	public void DeselectObject()
	{
        selected = null;
	}
}
