using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ShipSelector : MonoBehaviour {

	Camera selectionCamera;
	
	public SelectedCamera selectedCamera;
	public float gameDepth;

	public Selectable selectedShip
	{
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		selectionCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetMouseButtonUp(0))
		{
		
			print ("doing ray cast");
			Ray selectionRay =  selectionCamera.ScreenPointToRay(Input.mousePosition);

			RaycastHit2D info = Physics2D.GetRayIntersection(selectionRay, 1000.0f);


			if(info)
			{
				Selectable selectableObject = info.collider.gameObject.GetComponent<Selectable>();
				if(selectableObject != null)
				{
					if(selectedShip != null)
					{
						selectedShip.Deselect();
					}
					selectedShip = selectableObject;
					selectableObject.Select();

					selectedCamera.SelectObject(selectedShip.gameObject);
					return;
				}
			}

			selectedCamera.DeselectObject();
		}
	}
}
