using UnityEngine;
using System.Collections;

public interface IOrderReciever
{
    void GiveOrder(Vector3 temp);
}

[RequireComponent(typeof(ShipSelector))]
public class MouseOrderController : MonoBehaviour {

    public GameObject receiver;


    ShipSelector selector;

	// Use this for initialization
	void Start () {
        selector = GetComponent<ShipSelector>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonUp(1))
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = transform.position.y - 33.0f;
            Camera camera = GetComponent<Camera>();
            //print("Camera: " + camera.name);
            Vector3 mouseLocation = camera.ScreenToWorldPoint(mousePoint);
            if(selector.selectedShip != null)
            {
                SquadMember receiever = selector.selectedShip.GetComponent<SquadMember>();
                if (receiever != null)
                {
                    mouseLocation.y = receiever.transform.position.y;
                    receiever.ReceiveIndividualOrder(IndividualOrder.CreateMoveOrder(mouseLocation));
                }
            }

            /*Vector3 mouseLocation = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            print("Mouse location: " + mouseLocation);
            Debug.Break();
            */
        }
	}
}
