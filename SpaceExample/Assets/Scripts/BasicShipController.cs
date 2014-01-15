using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShipEngine))]
public class BasicShipController : MonoBehaviour {

	ShipEngine engine;

    IWeaponSystem weaponSystem;

	// Use this for initialization
	void Start () {
		engine = GetComponent<ShipEngine>();

        weaponSystem = (IWeaponSystem)GetComponent(typeof(IWeaponSystem));

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow))
		{
			engine.ApplyAccelerateForce(engine.accelerationForce);
		}

		if(Input.GetKey(KeyCode.DownArrow))
		{
			engine.ApplyAccelerateForce(-engine.accelerationForce);
		}

		if(Input.GetKey(KeyCode.LeftArrow))
		{
            engine.ApplyTorque(1.0f * Time.deltaTime);
		}
		else if(Input.GetKey(KeyCode.RightArrow))
		{
			engine.ApplyTorque(-1.0f * Time.deltaTime);
		}

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(weaponSystem != null)
            {
                weaponSystem.Fire();
            }
        }
	}
}
