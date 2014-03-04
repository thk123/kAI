using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
public class SquadMember : MonoBehaviour {

    kAIDataPort<IndividualOrder> orderPort;

    int count = 0;

	// Use this for initialization
	void Start () {
        AIBehaviour lBehaviour = GetComponent<AIBehaviour>();
        orderPort = (kAIDataPort<IndividualOrder>)lBehaviour.GetPort("Order");
        kAITriggerPort lTrigger = (kAITriggerPort)lBehaviour.GetPort("Fleeing");
        lTrigger.OnTriggered += new kAITriggerPort.TriggerEvent(lTrigger_OnTriggered);
        IsFleeing = false;
	}

    void lTrigger_OnTriggered(kAIPort lSender)
    {
        IsFleeing = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (count > 0)
        {
            if (count > 1)
            {
                orderPort.Data = null;
            }
            else
            {
                ++count;
            }
        }
	}

    public void ReceiveIndividualOrder(IndividualOrder order)
    {
        orderPort.Data = order;
        count = 1;
    }

    //public void AddToSquad()

    public bool IsFleeing 
    { 
        get; 
        private set; 
    }
}
