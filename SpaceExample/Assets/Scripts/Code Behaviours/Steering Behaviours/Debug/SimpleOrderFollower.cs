using UnityEngine;
using System.Collections;

using kAI.Core;

public class SimpleOrderFollower : MonoBehaviour, IOrderReciever {

    MoveTo mover;

	// Use this for initialization
	void Start () {
        mover = new MoveTo();
        mover.SetGlobal();
        mover.ForceActivation();
        
	}
	
	// Update is called once per frame
	void Update () {
        mover.Update(Time.deltaTime, gameObject);
	}

    public void GiveOrder(Vector2 temp)
    {
        ((kAIDataPort<Vector2>)mover.GetPort("Target")).Data = temp;
    }
}
