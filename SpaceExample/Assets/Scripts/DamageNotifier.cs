using UnityEngine;
using System.Collections;

using kAI.Core;

[RequireComponent(typeof(AIBehaviour))]
public abstract class BehaviourInterface : MonoBehaviour
{
    protected kAIBehaviour behaviour;

    public virtual void Start()
    {
        behaviour = GetComponent<AIBehaviour>().mXmlBehaviour; 
    }
}

public class DamageNotifier : BehaviourInterface
{

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnDamage(Vector2 damageDirection)
    {
        kAIPort lDamagePort;
        if(behaviour.TryGetPort("OnDamage", out lDamagePort))
        {
            ((kAITriggerPort)lDamagePort).Trigger();
        }

        kAIPort lDamageDirPort;
        if(behaviour.TryGetPort("OnDamageDirection", out lDamageDirPort))
        {
            ((kAIDataPort<Vector2>)lDamageDirPort).Data = damageDirection;
        }
    }
}
