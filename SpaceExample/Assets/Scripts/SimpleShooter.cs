using UnityEngine;
using System.Collections;

using kAI.Core;

public class SimpleShooter : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
        AIBehaviour controller = GetComponent<AIBehaviour>();

        kAIPort lTargetPort = controller.mXmlBehaviour.GetPort("Target");

        ((kAIDataPort<GameObject>)lTargetPort).Data = target;

	}
	
	// Update is called once per frame
	void Update () {
	
	}   
}
