using UnityEngine;
using System.Collections;

using kAI.Core;

public interface IWeaponSystem
{
    void Fire();

    void SetSpawnPoint(Transform spawn);
}

public class WeaponSystemManager : MonoBehaviour {

    public GameObject MainWeaponObject;

    public GameObject MissileWeaponObject;

    public Transform SpawnPoint;

    public IWeaponSystem MainWeapon
    {
        get
        {
            if (MainWeaponObject != null)
            {
                return (IWeaponSystem)MainWeaponObject.GetComponent(typeof(IWeaponSystem));
            }
            else
            {
                return null;
            }
        }
    }

    public IWeaponSystem MissileWeapon
    {
        get
        {
            if (MissileWeaponObject != null)
            {
                return (IWeaponSystem)MissileWeaponObject.GetComponent(typeof(IWeaponSystem));
            }
            else
            {
                return null;
            }
        }
    }


	// Use this for initialization
	void Start () {
	    if(MainWeapon != null)
        {
            MainWeapon.SetSpawnPoint(SpawnPoint);
        }

        if(MissileWeapon != null)
        {
            MissileWeapon.SetSpawnPoint(SpawnPoint);
        }
	}
	
	// Update is called once per frame
	void Update () {
	

	}

    
}

public static class WeaponFunctions
{
    public static float DistanceToTarget(GameObject self, GameObject target)
    {
        Debug.Log("DistanceToTarget");
        return (self.transform.position - target.transform.position).magnitude;
    }

    public static float AngleToTarget(GameObject self, GameObject target)
    {
        if(target != null)
        {
            Vector2 vectorToTarget = (target.transform.position - self.transform.position);
            float answer = Vector2.Angle(self.transform.right, vectorToTarget);

            Vector3 cross = Vector3.Cross(self.transform.right, vectorToTarget);

            if (cross.z < 0.0f)
            {
                answer = -answer;
            }


            Debug.Log("AngleToTarget: " + answer);
            return answer;
        }
        else
        {
            return 0.0f;
        }
    }
}

public class FireWeaponBehaviour : kAICodeBehaviour
{
    bool shouldFire;
    kAIDataPort<float> gunArc;
    public FireWeaponBehaviour()
    {
        kAITriggerPort fireTrigger = new kAITriggerPort("Fire", kAIPort.ePortDirection.PortDirection_In);
        fireTrigger.OnTriggered += fireTrigger_OnTriggered;
         
        AddExternalPort(fireTrigger);

        gunArc = new kAIDataPort<float>("GunArc", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(gunArc);
    }

    void fireTrigger_OnTriggered(kAIPort lSender)
    {
        LogMessage("Fire trigger");
        shouldFire = true;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(shouldFire)
        {
            GameObject shipObject = lUserData as GameObject;

            if(shipObject != null)
            {
                WeaponSystemManager weaponSystem = shipObject.GetComponent<WeaponSystemManager>();
                gunArc.Data = 45.0f; //weaponSystem.MainWeapon.

                weaponSystem.MainWeapon.Fire();

                shouldFire = false;
            }

        }
    }
}

