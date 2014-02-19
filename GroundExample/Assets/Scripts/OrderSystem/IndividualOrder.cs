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
        eIdle,
    }

    public abstract IndividualOrderType TypeOfOrder
    {
        get;
    }

    public static IndividualOrder CreateMoveOrder(Vector3 destination)
    {
        return new IndividualMoveOrder { Destination = destination };
    }

    internal static IndividualOrder CreateAttackOrder(GameObject target)
    {
        return new IndividualAttackOrder { Target = target };
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
    kAITriggerPort noOrderTrigger;
    kAITriggerPort clearOrderTrigger;

    kAIDataPort<IndividualOrder> orderPort;

    bool lPerformingOrder = false;

    public OrderSwitcherBehaviour()
        :base(null)
    {
        lastOrderType = IndividualOrder.IndividualOrderType.eInvalid;

        moveOrderTrigger = new kAITriggerPort("MoveOrder", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(moveOrderTrigger);

        attackOrderTrigger = new kAITriggerPort("AttackOrder", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(attackOrderTrigger);

        noOrderTrigger = new kAITriggerPort("Idle", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(noOrderTrigger);

        clearOrderTrigger = new kAITriggerPort("OrderCompleted", kAIPort.ePortDirection.PortDirection_In);
        clearOrderTrigger.OnTriggered += clearOrderTrigger_OnTriggered;
        AddExternalPort(clearOrderTrigger);


        orderPort = new kAIDataPort<IndividualOrder>("Order", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(orderPort);
    }

    void clearOrderTrigger_OnTriggered(kAIPort lSender)
    {
        lPerformingOrder = false;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if (lPerformingOrder)
        {
            // doing an order 

            if (orderPort.Data != null)
            {
                // we've just received an order, override
                ActivateOder(orderPort.Data.TypeOfOrder);
            }
            // else no order so we continue with current order
        }
        else
        {
            // no order being performed
            if (orderPort.Data != null)
            {
                // we've just received an order, override
                ActivateOder(orderPort.Data.TypeOfOrder);
            }
            else
            {
                // no order so we activate idle
                ActivateOder(IndividualOrder.IndividualOrderType.eIdle);
            }
        }

       
            
        
    }

    void ActivateOder(IndividualOrder.IndividualOrderType order)
    {
        if(order != lastOrderType)
        {
            switch (order)
            {
                case IndividualOrder.IndividualOrderType.eInvalid:
                    break;
                case IndividualOrder.IndividualOrderType.eMoveDirectToPoint:
                    moveOrderTrigger.Trigger();
                    lPerformingOrder = true;
                    break;
                case IndividualOrder.IndividualOrderType.eAttackTarget:
                    attackOrderTrigger.Trigger();
                    lPerformingOrder = true;
                    break;
                case IndividualOrder.IndividualOrderType.eIdle:
                    noOrderTrigger.Trigger();
                    break;
                default:
                    break;
            }
        }

        lastOrderType = order;
    }



    public override kAI.Core.Debug.kAINodeObjectDebugInfo GenerateDebugInfo()
    {
        return new OrderSwitcherDebugInfo(this, lPerformingOrder);
    }
}

[Serializable]
public class OrderSwitcherDebugInfo : kAI.Core.Debug.kAIBehaviourDebugInfo
{
    public bool PerformingOrder
    {
        get;
        private set;
    }

    public OrderSwitcherDebugInfo(OrderSwitcherBehaviour lBehaviour, bool lPerformingOrder)
        :base(lBehaviour)
    {
        PerformingOrder = lPerformingOrder;
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
                    Vector3 destination = moveOrder.Destination;
                    destination.y = (lUserData as GameObject).transform.position.y;
                    destinationPort.Data = destination;
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

public static class AIFunctions
{
    // TODO: Here we don't currently support complex generic return types, should specialise for now
    public static KeyValuePair<Vector3, float> CombineVectorFloat(Vector3 lKeyValue, float lValueValue)
    {
        return new KeyValuePair<Vector3, float>(lKeyValue, lValueValue);
    }
}


