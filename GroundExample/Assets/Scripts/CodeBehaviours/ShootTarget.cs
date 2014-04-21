using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class ShootTarget : CodeBehaviourTester {

    public GameObject target;

	// Use this for initialization
	void Start () 
	{
        codeBehaviour = new ShootTargetBehaviour();
        InitBehaviour();

        ((kAIDataPort<GameObject>)codeBehaviour.GetPort("Target")).Data = target;

        ((kAITriggerPort)codeBehaviour.GetPort("OutOfRange")).OnTriggered += ShootTarget_OnTriggered;
        ((kAITriggerPort)codeBehaviour.GetPort("InRange")).OnTriggered += ShootTarget_OutOfRange_OnTriggered;
	}

    void ShootTarget_OutOfRange_OnTriggered(kAIPort lSender)
    {
        GetComponent<MoveTo>().GiveOrder(transform.position);
    }

    void ShootTarget_OnTriggered(kAIPort lSender)
    {
        GetComponent<MoveTo>().GiveOrder(target.transform.position);
    }
	 
	// Update is called once per frame
	void Update () 
	{
        UpdateBehaviour();
	}
}

public class Vector3Propertes : kAIFunctionNode.kAIFunctionConfiguration.kAIReturnConfiguration.kAIReturnConfigurationDictionary<Vector3>
{


    public Vector3Propertes()
    {
        kAIFunctionNode.kAIFunctionConfiguration.kAIReturnConfiguration.kAIReturnConfigurationDictionary.AddDefaultConfigToCustom<Vector3>(this);

        Func<kAIPort> lTestFunction = ()=>{return new kAITriggerPort("IsNormal", kAIPort.ePortDirection.PortDirection_Out);};

        Action<kAINodeObject, Vector3, Vector3> lTestACtion = (lObject, lResult, lOldResult) => { if (lResult.magnitude == 1.0f) { ((kAITriggerPort)lObject.GetPort("IsNormal")).Trigger(); } };


        AddProperty("TestProperty", lTestFunction, lTestACtion);

        
    }
}

public class ShootTargetBehaviour : kAICodeBehaviour
{
    kAIDataPort<GameObject> targetPort;
    kAITriggerPort outOfRangeTrigger;
    kAITriggerPort inRangeTrigger;

    public ShootTargetBehaviour()
        :base(null)
    {
        targetPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(targetPort);

        outOfRangeTrigger = new kAITriggerPort("OutOfRange", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(outOfRangeTrigger);

        inRangeTrigger = new kAITriggerPort("InRange", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(inRangeTrigger);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;

        if(shipObject != null)
        {
            GameObject target = targetPort.Data;
            if(target != null)
            {
                Vector3 pathToTarget =(target.transform.position - shipObject.transform.position);
                float distSqrdToTarget = pathToTarget.sqrMagnitude;

                WeaponSystemManager weaponManager =  shipObject.GetComponent<WeaponSystemManager>();

                if(weaponManager == null)
                {
                    throw new Exception("ShootTargetBehaviour needs a WeaponSystem on the ship");
                }

                IWeaponSystem mainWeapon = weaponManager.MainWeapon;

                if(distSqrdToTarget <=  (mainWeapon.Range * mainWeapon.Range))
                {
                    
                    shipObject.GetComponent<ShipEngine>().SetLookAt(pathToTarget);
                    mainWeapon.Fire();
                    inRangeTrigger.Trigger();

                }
                else
                {
                    outOfRangeTrigger.Trigger();
                }
            }
        }
    }
}