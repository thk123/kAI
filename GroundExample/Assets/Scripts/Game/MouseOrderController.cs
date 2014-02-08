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
            if (selector.selectedShip != null)
            {
                SquadMember receiever = selector.selectedShip.GetComponent<SquadMember>();
                Vector3 mousePoint = Input.mousePosition;
                mousePoint.z = transform.position.y - 33.0f;
                Camera camera = GetComponent<Camera>();
                //print("Camera: " + camera.name);

                // find out if we are attacking an enemy
                Ray selectionRay = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;

                IndividualOrder order = null;
                if (Physics.Raycast(selectionRay, out info))
                {
                    HealthBehaviour selectableObject = info.collider.gameObject.GetComponent<HealthBehaviour>();
                    if (selectableObject != null)
                    {
                        order = IndividualOrder.CreateAttackOrder(selectableObject.gameObject);
                    }
                }

                if (order == null)
                {
                    Vector3 mouseLocation = camera.ScreenToWorldPoint(mousePoint);
                    mouseLocation.y = receiever.transform.position.y;
                    order = IndividualOrder.CreateMoveOrder(mouseLocation);
                }



                if (receiever != null)
                {
                    receiever.ReceiveIndividualOrder(order);
                }
            }
            

            /*Vector3 mouseLocation = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            print("Mouse location: " + mouseLocation);
            Debug.Break();
            */
        }
	}
}
