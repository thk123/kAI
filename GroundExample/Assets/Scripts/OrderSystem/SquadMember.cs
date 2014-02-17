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
        orderPort = (kAIDataPort<IndividualOrder>)GetComponent<AIBehaviour>().GetPort("Order");
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
}
