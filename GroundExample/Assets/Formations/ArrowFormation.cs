using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public abstract class Formation : kAICodeBehaviour
{
    kAIDataPort<List<SquadMember>> membersPort;

    kAIDataPort<Vector3> destinationPort;

    kAIDataPort<List<IndividualOrder>> orderPort;

    public Formation()
        : base()
    {
        membersPort = new kAIDataPort<List<SquadMember>>("Members", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(membersPort);

        destinationPort = new kAIDataPort<Vector3>("Destination", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(destinationPort);

        orderPort = new kAIDataPort<List<IndividualOrder>>("Orders", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(orderPort);
    }

    public abstract IndividualOrder CreateOrder(int index, SquadMember memeber, Vector3 destination, Vector3 direction);

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(membersPort.Data.Count > 0)
        {
            List<IndividualOrder> orders = new List<IndividualOrder>(membersPort.Data.Count);
            Vector3 dest = destinationPort.Data;
            dest.y = membersPort.Data[0].transform.position.y;
            Vector3 direction = dest - membersPort.Data[0].transform.position;
            if (direction.sqrMagnitude == 0.0f)
            {
                direction = membersPort.Data[0].transform.forward;

            }

            {
                for (int i = 0; i < membersPort.Data.Count; ++i)
                {
                    if (membersPort.Data[i] != null)
                    {
                        orders.Add(CreateOrder(i, membersPort.Data[i], dest, direction));
                    }
                }

                orderPort.Data = orders;
            }
        }

       // Debug.Break();
    }
}

public class LineFormation : Formation
{
    float gapSize = 20.0f;

    public override IndividualOrder CreateOrder(int index, SquadMember memeber, Vector3 destination, Vector3 direction)
    {
        Vector3 perpToDirection = Vector3.Cross(Vector3.up, direction.normalized);
        perpToDirection.Normalize();
        Vector3 newDestination = destination + (index * gapSize * perpToDirection);
        //LogMessage(index +": " + newDestination);
        return new IndividualMoveOrder { Destination = newDestination };
    }
}

public class ArrowFormation : Formation
{
    float rowSize = 20.0f;
    float columnSize = 10.0f;

    public override IndividualOrder CreateOrder(int index, SquadMember memeber, Vector3 destination, Vector3 direction)
    {
        /*            0
         *          1   2           depth: 1
         *        3       4         depth: 2
         *      5           6       depth: 3
         * 
         * */


        if (index == 0)
        {
            return new IndividualMoveOrder { Destination = destination };
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

            Vector3 perpToDirection = Vector3.Cross(direction, Vector3.up);

            Vector3 newDestination = destination - (rowSize * direction.normalized * depth) + (columnSize * perpToDirection.normalized * side * depth);
            //LogMessage(index + ": " + newDestination);
            return new IndividualMoveOrder { Destination = newDestination };
        }
    }
}
