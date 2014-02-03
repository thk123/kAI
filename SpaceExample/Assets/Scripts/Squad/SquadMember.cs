using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
public class SquadMember : MonoBehaviour {

    kAIDataPort<IndividualOrder> orderPort;



	// Use this for initialization
	void Start () {
        orderPort = (kAIDataPort<IndividualOrder>)GetComponent<AIBehaviour>().mXmlBehaviour.GetPort("Order");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReceiveIndividualOrder(IndividualOrder order)
    {
        orderPort.Data = order;
    }

    //public void AddToSquad()
}
