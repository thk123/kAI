using UnityEngine;
using System.Collections;

public interface IOrderReciever
{
    void GiveOrder(Vector2 temp);
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
            //print("Giving order " + Input.mousePosition);
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = -transform.position.z;
            Camera camera = GetComponent<Camera>();
            //print("Camera: " + camera.name);
            Vector3 mouseLocation = camera.ScreenToWorldPoint(mousePoint);
            //print("Mouse location: " + mouseLocation);

            if(selector.selectedShip != null)
            {
                IOrderReciever receiever = (IOrderReciever)selector.selectedShip.GetComponent(typeof(IOrderReciever));
                if (receiever != null)
                {
                    receiever.GiveOrder(mouseLocation);
                }
            }

            /*Vector3 mouseLocation = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            print("Mouse location: " + mouseLocation);
            Debug.Break();
            */
        }
	}
}
