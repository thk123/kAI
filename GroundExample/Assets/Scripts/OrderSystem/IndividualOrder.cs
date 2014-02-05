using System;
using System.Collections.Generic;
using kAI.Core;
using UnityEngine;

public abstract class IndividualOrder
{
    public enum IndividualOrderType
    {
        eInvalid = 0,

        //eMoveInFormation,
        eMoveDirectToPoint,
        eAttackTarget, 
    }

    public abstract IndividualOrderType TypeOfOrder
    {
        get;
    }

    public static IndividualOrder CreateMoveOrder(Vector3 destination)
    {
        return new IndividualMoveOrder { Destination = destination };
    }
}

public class IndividualMoveOrder : IndividualOrder
{
    public Vector3 Destination
    {
        get;
        set;
    }

    public override IndividualOrderType TypeOfOrder
    {
        get { return IndividualOrderType.eMoveDirectToPoint; }
    }    
}

public class IndividualAttackOrder : IndividualOrder
{
    public GameObject Target
    {
        get;
        set;
    }

    public override IndividualOrderType TypeOfOrder
    {
        get { return IndividualOrderType.eAttackTarget; }
    }
}

public class OrderSwitcherBehaviour : kAICodeBehaviour
{
    IndividualOrder.IndividualOrderType lastOrderType;

    kAITriggerPort moveOrderTrigger;
    kAITriggerPort attackOrderTrigger;
    kAIDataPort<IndividualOrder> orderPort;

    public OrderSwitcherBehaviour()
        :base(null)
    {
        lastOrderType = IndividualOrder.IndividualOrderType.eInvalid;

        moveOrderTrigger = new kAITriggerPort("MoveOrder", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(moveOrderTrigger);

        attackOrderTrigger = new kAITriggerPort("AttackOrder", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(attackOrderTrigger);

        orderPort = new kAIDataPort<IndividualOrder>("Order", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(orderPort);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(orderPort.Data != null)
        {
            if (lastOrderType != orderPort.Data.TypeOfOrder)
            {
                lastOrderType = orderPort.Data.TypeOfOrder;

                switch (lastOrderType)
                {
                    case IndividualOrder.IndividualOrderType.eInvalid:
                        break;
                    case IndividualOrder.IndividualOrderType.eMoveDirectToPoint:
                        moveOrderTrigger.Trigger();
                        break;
                    case IndividualOrder.IndividualOrderType.eAttackTarget:
                        attackOrderTrigger.Trigger();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

public class OrderDataExtractor : kAICodeBehaviour
{
    kAIDataPort<IndividualOrder> orderPort;

    kAIDataPort<Vector3> destinationPort;
    kAIDataPort<GameObject> targetPort;

    public OrderDataExtractor()
        :base(null)
    {
        orderPort = new kAIDataPort<IndividualOrder>("Order", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(orderPort);

        destinationPort = new kAIDataPort<Vector3>("Destination", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(destinationPort);

        targetPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(targetPort);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(orderPort.Data != null)
        {
            switch (orderPort.Data.TypeOfOrder)
            {
                case IndividualOrder.IndividualOrderType.eInvalid:
                    break;
                case IndividualOrder.IndividualOrderType.eMoveDirectToPoint:
                    IndividualMoveOrder moveOrder = (IndividualMoveOrder)orderPort.Data;
                    destinationPort.Data = moveOrder.Destination;
                    break;
                case IndividualOrder.IndividualOrderType.eAttackTarget:
                    IndividualAttackOrder attackOrder = (IndividualAttackOrder)orderPort.Data;
                    targetPort.Data = attackOrder.Target;
                    break;
                default:
                    break;
            }
        }
    }
}


