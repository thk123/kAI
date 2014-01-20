using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a string and its previous value on screen. 
/// </summary>
public class DebugString : MonoBehaviour {

    string debugString;
    string oldDebugString;

    public Rect boxPosition;

    public string DebugStringValue
    {
        get
        {
            return debugString;
        }
        set
        {
            if (value != debugString)
            {
                oldDebugString = debugString;
                debugString = value;
            }
        }
    }
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.TextArea(boxPosition, DebugStringValue + "\n" + oldDebugString);
    }
}
