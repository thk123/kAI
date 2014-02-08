    using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ShipSelector : MonoBehaviour {

	Camera selectionCamera;
	
	public SelectedCamera selectedCamera;

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
			Ray selectionRay =  selectionCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;


            if (Physics.Raycast(selectionRay, out info))
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
