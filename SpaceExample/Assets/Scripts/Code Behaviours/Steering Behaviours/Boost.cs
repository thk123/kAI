using UnityEngine;
using System.Collections;
using kAI.Core;

public class Boost : kAICodeBehaviour {

    float time;

    public Boost()
    { }

    protected override void OnActivate()
    {
        base.OnActivate();
        time = 0.0f;
    }



    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        if (shipObject != null)
        {
            ShipEngine engine = shipObject.GetComponent<ShipEngine>();

            time += lDeltaTime;

            Vector2 force = new Vector2(time, Mathf.Sin(time));

            float accelerateForce = Vector2.Dot(force, shipObject.transform.right);

            engine.ApplyAccelerateForce(accelerateForce * 0.1f);

            
        }
    }
}
