using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[Serializable]
public class Vector3S
{
    public float x;
    public float y;
    public float z;

    public Vector3S(Vector3 lSource)
    {
        x = lSource.x;
        y = lSource.y;
        z = lSource.z;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ")";
    }
}
