using System;
using UnityEngine;
using kAI.Core;

public class OrderSwitcherBehaviour : kAICodeBehaviour
{
    IndividualOrder lastOrder;

    kAITriggerPort moveOrderTrigger;
    kAITriggerPort attackOrderTrigger;
    kAITriggerPort noOrderTrigger;
    kAITriggerPort clearOrderTrigger;

    kAIDataPort<IndividualOrder> orderPort;

    bool lPerformingOrder = false;

    public OrderSwitcherBehaviour()
        :base(null)
    {
        lastOrder = null;

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

            if (orderPort.Data != null && !orderPort.Data.Equals(lastOrder))
            {
                bool result = orderPort.Data.Equals(lastOrder);
                //LogMessageIfShip("Result:" + result, lUserData);
                //LogMessageIfShip("Different order: " + orderPort.Data + "/" + lastOrder, lUserData);

                // we've just received an order, override
                ActivateOder(orderPort.Data.TypeOfOrder);
                lastOrder = orderPort.Data;

                bool lResult = lastOrder.Equals(orderPort.Data);
                if (!lResult)
                {
                    throw new Exception("This should be equal");
                }
            }
            // else no order (or the same order) so we continue with current order
        }
        else
        {
            // no order being performed
            if (orderPort.Data != null)
            {
                // we've just received an order, override
                if (!orderPort.Data.Equals(lastOrder))
                {
                    LogMessageIfShip("New order from the data", lUserData);
                    ActivateOder(orderPort.Data.TypeOfOrder);
                    lastOrder = orderPort.Data;
                }
                else
                {

                }
                {
                    ActivateOder(IndividualOrder.IndividualOrderType.eIdle);
                }
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

    void LogMessageIfShip(string lMessage, object lShip)
    {
        if(((GameObject)lShip).name == "SpaceShip")
        {
           LogMessage(lMessage);
        }
    }

    public override kAI.Core.Debug.kAINodeObjectDebugInfo GenerateDebugInfo()
    {
        return new OrderSwitcherDebugInfo(this, lPerformingOrder, lastOrder);
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

    public Vector3S LastOrderPosition
    {
        get;
        private set;
    }

    public string LastOrderName
    {
        get;
        private set;
    }

    public OrderSwitcherDebugInfo(OrderSwitcherBehaviour lBehaviour, bool lPerformingOrder, IndividualOrder lLastOrder)
        :base(lBehaviour)
    {
        PerformingOrder = lPerformingOrder;
        if (lLastOrder is IndividualMoveOrder)
        {
            LastOrderPosition = new Vector3S(((IndividualMoveOrder)lLastOrder).Destination);
        }
        else if (lLastOrder is IndividualAttackOrder)
        {
            LastOrderName = ((IndividualAttackOrder)lLastOrder).Target.GetFullName();
        }
    }
}

