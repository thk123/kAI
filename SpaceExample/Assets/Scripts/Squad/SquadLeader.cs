using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
public class SquadLeader : MonoBehaviour, IOrderReciever {

    kAIDataPort<SquadOrder> orderReceiverPort;

    public List<SquadMember> squadMemebers;
    

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GiveOrder(Vector2 temp)
    {
        
    }

    public void AddMemeber(SquadMember memeber)
    {
        squadMemebers.Add(memeber);
    }
}

public static class SquadFunctions
{
    public static void SendOrder(GameObject self, IndividualOrder order)
    {
        SquadLeader leaderBehaviour = self.GetComponent<SquadLeader>();
        if(leaderBehaviour != null)
        {
            foreach (SquadMember memeber in leaderBehaviour.squadMemebers)
            {
                memeber.ReceiveIndividualOrder(order);
            }
        }
        else
        {
            throw new Exception("Attempted to send order when object is not a squad leader");
        }
    }

   
}
