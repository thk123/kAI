using UnityEngine;
using System.Collections;

using kAI.Core;

public interface IWeaponSystem
{
    void Fire();
}

public class Weapon : MonoBehaviour, IWeaponSystem {

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce;

    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Fire()
    {
        GameObject bullet = (GameObject)Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
        bullet.rigidbody2D.velocity = rigidbody2D.velocity;
        bullet.rigidbody2D.AddForce(projectileForce * transform.right);
    }
}

class FireBehaviour : kAICodeBehaviour
{
    bool shouldFireThisFrame;
    public FireBehaviour()
        :base(null)
    {
        kAITriggerPort fireAction = new kAITriggerPort("Fire", kAIPort.ePortDirection.PortDirection_In);
        fireAction.OnTriggered += fireAction_OnTriggered;

        shouldFireThisFrame = false;
    }

    void fireAction_OnTriggered(kAIPort lSender)
    {
        shouldFireThisFrame = true;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(shouldFireThisFrame)
        {
            GameObject ship = lUserData as GameObject;
            if (ship != null)
            {
                ship.GetComponent<Weapon>().Fire();
                shouldFireThisFrame = false;
            }
        }

    }
}