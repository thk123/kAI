using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class AttackFormation : Formation
{
    kAIDataPort<GameObject> targetPort;
    kAIDataPort<float> weaponRangePort;


    public AttackFormation()
        : base()
    {
        targetPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(targetPort);

        weaponRangePort = new kAIDataPort<float>("Range", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(weaponRangePort);
    }

    public override IndividualOrder CreateOrder(int index, SquadMember memeber, Vector3 destination, Vector3 direction)
    {
        GameObject target = targetPort.Data;
        if (target == null)
        {
            LogWarning("No target to create an attack formation around");
            return IndividualOrder.CreateMoveOrder(destination);
        }
        //TODO: this doesn't work, keeps switching between move and attack
        // changing to a broader error margin helped but didn't fix
        if ((memeber.gameObject.transform.position - targetPort.Data.transform.position).sqrMagnitude < (weaponRangePort.Data * weaponRangePort.Data) + (160.0f * 160.0f))
        {
            return IndividualOrder.CreateAttackOrder(targetPort.Data);
        }
        else
        {
            Vector3 pathToTarget = direction;
            Vector3 straightDistanceOffTarget =(150.0f * pathToTarget.normalized);
            if (index == 0)
            {
                Vector3 targetPosition = target.transform.position + straightDistanceOffTarget;
                return IndividualOrder.CreateMoveOrder(targetPosition);
            }
            else
            {
                index = index - 1;

                // this is the row in the arrow rank
                int depth = (index / 2) + 1;

                int side = index % 2; // 0 means left hand side, 1 means right hand side

                // transform so -1 means left hand size, 1 means right hand side
                side *= 2;
                side -= 1;


                Vector3 lNewVector = Quaternion.Euler(Vector3.up * side * depth * 10.0f) * straightDistanceOffTarget;

                return IndividualOrder.CreateMoveOrder(target.transform.position + lNewVector);
            }
        }
    }
}
