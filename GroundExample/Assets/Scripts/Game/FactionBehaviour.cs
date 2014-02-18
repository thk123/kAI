using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FactionBehaviour : MonoBehaviour {

    [Serializable]
    public enum eFactions
    {
        eFactionHuman,
        eFactionComputer,
    }

    public eFactions Faction;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    public bool IsEnemy(GameObject other)
    {
        FactionBehaviour lOtherBeh = other.GetComponent<FactionBehaviour>();
        return lOtherBeh.Faction != Faction;
    }

    public bool IsAlly(GameObject other)
    {
        return !IsEnemy(other);
    }
}
