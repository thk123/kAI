using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectionBox : MonoBehaviour {

    public bool Selected
    {
        get;
        set;
    }
    
	// Use this for initialization
	void Start () 
	{
        GenerateBoundsBox(true);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    void OnGUI()
    {
        if (Selected)
        {
            GUIUtility.RotateAroundPivot(35.0f, Vector2.zero);
            GUI.Box(GenerateBoundsBox(), new GUIContent());
        }
    }

    Rect GenerateBoundsBox(bool drawSpheres = false)
    {
        Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity); 
        Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        
        foreach (Transform childTransform in transform)
        {
            Vector3 position = childTransform.position;
            max = Vector3.Max(position, max);
            min = Vector3.Min(position, min);

//             if (position.x > max.x)
//             {
//                 max.x = position.x;
//             }
//             else if (position.x < min.x)
//             {
//                 min.x = position.x;
//             }
// 
//             if (position.y > max.y)
//             {
//                 max.y = position.y;
//             }
//             else if (position.y < min.y)
//             {
//                 min.y = position.y;
//             }
//             if (position.z > max.z)
//             {
//                 max.z = position.z;
//             }
//             else if (position.z < min.z)
//             {
//                 min.z = position.z;
//             }
        }

        Vector3 topLeft = Camera.main.WorldToScreenPoint(min);
        Vector3 bottomRight = Camera.main.WorldToScreenPoint(max);


        if(drawSpheres)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = min;
            s.name = "min";

            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = max;
            s.name = "max";

            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = topLeft;
            s.name = "TL";

            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = bottomRight;
            s.name = "BR";
        }

        return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
    }
}
