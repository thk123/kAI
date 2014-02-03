using UnityEngine;
using System.Collections;

public abstract class SquadOrder
{
    public enum OrderType
    {
        eMoveToPoint,
        eAttackTarget,
        // Formation?
    }

    public abstract OrderType TypeOfOrder
    {
        get;
    }

    public static SquadOrder CreateMoveOrder(Vector2 destination)
    {
        return new MoveOrder(destination);
    }

    public static SquadOrder CreateAttackOrder(GameObject target)
    {
        return new AttackOrder(target);
    }

    public static OrderType GetOrderType(SquadOrder order)
    {
        return order.TypeOfOrder;
    }
}

public class MoveOrder : SquadOrder
{
    public override OrderType TypeOfOrder
    {
	    get 
        { 
            return OrderType.eMoveToPoint;
        }
    }

    public Vector2 targetPosition
    {
        get;
        private set;
    }

    public MoveOrder(Vector2 dest)
    {
        targetPosition = dest;
    }
}

public class AttackOrder : SquadOrder
{
    public override OrderType TypeOfOrder
    {
        get 
        { 
            return OrderType.eAttackTarget; 
        }
    }

    public GameObject target
    {
        get;
        private set;
    }

    public AttackOrder(GameObject target)
    {
        this.target = target;
    }
}

public abstract class IndividualOrder
{
    public enum IndividualOrderType
    {
        eMoveInFormation,
        eMoveDirectToPoint,
        eAttackTarget,
    }

    public abstract IndividualOrderType TypeOfOrder
    {
        get;
    }
}


