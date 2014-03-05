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

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
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

    public override string ToString()
    {
        return Destination.ToString();
    }

    public override bool Equals(object obj)
    {
        IndividualMoveOrder other = obj as IndividualMoveOrder;
      //  kAIObject.LogMessage(null, "Calling my equals");
        if (other == null)
        {
           // kAIObject.LogMessage(null, "Failied on type check");
            return false;
        }
        else
        {
            //kAIObject.LogMessage(null, "Target:" + Destination + "/" + other.Destination);
            return other.Destination.x == Destination.x && other.Destination.z == Destination.z;
        }
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

    public override bool Equals(object obj)
    {
        
        IndividualAttackOrder other = obj as IndividualAttackOrder;
        if (other == null)
        {
            
            return false;
        }
        else
        {
            
            return other.Target == Target;
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


