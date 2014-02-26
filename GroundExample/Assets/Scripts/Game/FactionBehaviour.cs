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

    public static Color HumanColour = Color.blue;
    public static Color ComputerColour = Color.red;

    public eFactions Faction;

	// Use this for initialization
	void Start () 
	{
	    
	}
	
	// Update is called once per frame
	void Update () 
	{
        GetComponentInChildren<MeshRenderer>().material.color = Faction == eFactions.eFactionHuman ? HumanColour : ComputerColour;
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
