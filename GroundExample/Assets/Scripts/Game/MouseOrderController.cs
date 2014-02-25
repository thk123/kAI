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
                selector.selectedShip.IssueOrder(CreateOrder());
            }
        }
	}

    IndividualOrder CreateOrder()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = transform.position.y - 33.0f;
        Camera camera = GetComponent<Camera>();

        // find out if we are attacking an enemy
        Ray selectionRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;

        IndividualOrder order = null;
        if (Physics.Raycast(selectionRay, out info))
        {
            FactionBehaviour selectableObject = info.collider.gameObject.GetComponent<FactionBehaviour>();
            if (selectableObject != null && selectableObject.IsEnemy(selector.selectedShip.GetFaction()))
            {
                order = IndividualOrder.CreateAttackOrder(selectableObject.gameObject);
            }
            else
            {
                // TODO: make a defend order?
            }
        }

        if (order == null)
        {
            Vector3 mouseLocation = camera.ScreenToWorldPoint(mousePoint);
            order = IndividualOrder.CreateMoveOrder(mouseLocation);
        }


        return order;
       
    }
}
