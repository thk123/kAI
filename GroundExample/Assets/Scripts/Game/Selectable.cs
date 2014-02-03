using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

	bool selected;

	Rect boxRectangle;

	MeshRenderer mesh;

	// Use this for initialization
	void Start () {
		selected = false;
		mesh = GetComponentInChildren<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(selected)
		{
			Bounds bounds = mesh.bounds;
			Vector3 maxPoint = Camera.main.WorldToScreenPoint(bounds.max);
			Vector3 minPoint = Camera.main.WorldToScreenPoint(bounds.min);

			boxRectangle = new Rect(maxPoint.x, maxPoint.z, maxPoint.x - minPoint.x, maxPoint.z - minPoint.z);
		}
	}

	void OnGUI()
	{
		if(selected)
		{
			
			GUI.Box(boxRectangle, new GUIContent());
		}
	}

	public void Select()
	{
		selected = true;
	}

	public void Deselect()
	{
		selected = false;
	}
}
