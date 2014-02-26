using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiscUtil;
using kAI.Core;
using System.Text;

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
            return squadMembers.Select<SquadMember, Vector3>((element) => { if (element != null) return element.gameObject.transform.position; else return Vector3.zero; }).Average();
        }
    }

	// Use this for initialization
	void Start () 
	{
        squadMembers = new List<SquadMember>();
        squadMembers.AddRange(transform.GetComponentsInChildren<SquadMember>());
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

    internal GameObject GetFaction()
    {
        return squadMembers[0].gameObject;
    }
}

public class IssueOrder : kAICodeBehaviour
{
    kAIDataPort<List<SquadMember>> memebersPorts;
    kAIDataPort<List<IndividualOrder>> orderPort;

    public IssueOrder()
        : base()
    {
        memebersPorts = new kAIDataPort<List<SquadMember>>("Member", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(memebersPorts);

        orderPort = new kAIDataPort<List<IndividualOrder>>("Order", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(orderPort);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if(memebersPorts.Data != null && orderPort.Data != null)
        {
            foreach (var lOrderPairs in CoreUtil.MoveThroughPairwise(memebersPorts.Data, orderPort.Data))
            {
                lOrderPairs.Key.ReceiveIndividualOrder(lOrderPairs.Value);
            }
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
            Vector3 lOutVector = (1.0f / (float)length) * lValue;
            return lOutVector;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static string GetFullName(this GameObject lObject)
    {
        StringBuilder sb = new StringBuilder();
        do 
        {
            sb.Append(lObject.name);
            if(lObject.transform.parent != null)
            {
                lObject = lObject.transform.parent.gameObject;
            
                sb.Append(".");
            }
            else
            {
                lObject = null;
            }
        } while (lObject != null);

        return sb.ToString();
    }

}


