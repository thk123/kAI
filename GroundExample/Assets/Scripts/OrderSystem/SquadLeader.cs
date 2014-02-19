﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiscUtil;
using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
public class SquadLeader : MonoBehaviour {

    public List<SquadMember> squadMembers
    {
        get;
        private set;
    }

    AIBehaviour mAiBehaviour;
    int count = 0;
    kAIDataPort<IndividualOrder> orderPort;

    public Vector3 SquadPosition
    {
        get
        {
            return squadMembers.Select<SquadMember, Vector3>((element) => { return element.gameObject.transform.position; }).Average();
        }
    }

	// Use this for initialization
	void Start () 
	{
        squadMembers = new List<SquadMember>();
        squadMembers.AddRange(transform.GetComponentsInChildren<SquadMember>());
        print(squadMembers.Count);
        mAiBehaviour = GetComponent<AIBehaviour>();
        orderPort = (kAIDataPort<IndividualOrder>)mAiBehaviour.GetPort("Order");
	}
	
	// Update is called once per frame
	void Update () 
	{
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

    public void IssueOrder(IndividualOrder order)
    {
        orderPort.Data = order;
        count = 1;
    }
}

public class IssueOrder : kAICodeBehaviour
{
    kAIDataPort<List<SquadMember>> memebersPorts;
    kAIDataPort<IndividualOrder> orderPort;

    public IssueOrder()
        : base()
    {
        memebersPorts = new kAIDataPort<List<SquadMember>>("Member", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(memebersPorts);

        orderPort = new kAIDataPort<IndividualOrder>("Order", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(orderPort);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        foreach (SquadMember member in memebersPorts.Data)
        {
            member.ReceiveIndividualOrder(orderPort.Data);
        }
    }
}

public static class SquadLeaderFunctions
{
    public static List<SquadMember> GetSquadMemebers(GameObject self)
    {
        return self.GetComponent<SquadLeader>().squadMembers;
    }

    public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> list, Func<T, TResult> converter)
    {
        foreach (T element in list)
        {
            yield return converter(element);
        }
    }

    public static Vector3 Average(this IEnumerable<Vector3> lResult)
    {
        Vector3 lValue = Vector3.zero;
        int length = 0;
        foreach (Vector3 lEntry in lResult)
        {
            lValue += lEntry;
            ++length;
        }

        if (length > 0)
        {
            return (1 / length) * lValue;
        }
        else
        {
            return Vector3.zero;
        }
    }
}


