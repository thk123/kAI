﻿using UnityEngine;
using System;
using System.Collections;

using kAI.Core;

public interface IWeaponSystem
{
    float Range
    {
        get;
    }

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
        if (self == null)
        {
            kAIObject.LogWarning(null, "Self null in Distance To Target");
            return float.MaxValue;
        }
        if (target == null)
        {
            kAIObject.LogWarning(null, "Target null in DistnaceToTarget (" + self.GetFullName() + ")");
            return float.MaxValue;
        }
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

    public static float GetWeaponRange(GameObject self)
    {
        WeaponSystemManager weaponSystem = self.GetComponent<WeaponSystemManager>();
        if (weaponSystem != null)
        {
            return weaponSystem.MainWeapon.Range;
        }
        else
        {
            throw new Exception("Cannot compute if in range if no weapon system manager on object");
        }
    }

    public static bool WeaponInRange(GameObject self, GameObject target)
    {
        float distToTarget = DistanceToTarget(self, target);
        return distToTarget <= GetWeaponRange(self);
    }

    public static Vector3 GetObjectPosition(GameObject target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }
        return target.transform.position;
    }

    public static Vector3 PointNearTarget(GameObject self, GameObject target)
    {
        /*Vector3 path = target.transform.position - self.transform.position;
        float pathLength = path.magnitude;
        Vector3 point = path * ((pathLength - GetWeaponRange(self)) / pathLength);
        point.y = self.transform.position.y;
        return point;*/
        if (target == null)
        {
            return self.transform.position;
        }
        return target.transform.position;
    }

    public static GameObject GetNearestTarget(GameObject self, float maxRange)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(self.transform.position, maxRange);

        float nearestDistance =float.MaxValue;
        GameObject nearestObject = null;
        foreach(Collider nearCollider in nearbyObjects)
        {
            
            if (nearCollider != self.collider)
            {
                GameObject nearObject = nearCollider.gameObject;
                FactionBehaviour healthObj = nearObject.GetComponent<FactionBehaviour>();
                if (healthObj != null && healthObj.IsEnemy(self))
                {
                    float dist2 = (nearObject.transform.position - self.transform.position).sqrMagnitude;
                    if (dist2 < nearestDistance)
                    {
                        nearestObject = nearObject;
                        nearestDistance = dist2;
                    }
                }
            }
        }
        if (nearestObject != null)
        {
            //Debug.Log("Found nearest object, distance:" + nearestDistance);
        }
        return nearestObject;
    }

    public static GameObject GetRandomSquadMember(SquadLeader squadLeader)
    {
        if (squadLeader != null)
        {
            GameObject lObject = squadLeader.squadMembers[UnityEngine.Random.Range(0, squadLeader.squadMembers.Count)].gameObject;
            if (lObject == null)
            {
                kAIObject.LogMessage(null, "No member found:" + squadLeader.squadMembers.Count);
            }
            return lObject;
        }
        else
        {
            kAIObject.LogWarning(null, "No squad leader");
            return null;
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

