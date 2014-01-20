using UnityEngine;
using System.Collections;
using kAI.Core;

public class OrderFollower : MonoBehaviour, IOrderReciever, kAIILogger
{
    AIBehaviour slave;
    /*MoveToPoint mover;*/
	// Use this for initialization
	void Start () {
        /*kAIObject.ExceptionOnAssert = true;
        kAIObject.GlobalLogger = this;
        mzver = null;*/
        slave = GetComponent<AIBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
        /*if (mover != null)
        {
            mover.Update(Time.fixedDeltaTime, gameObject);
        }*/
	}

    public void GiveOrder(Vector2 temp)
    {
        ((kAIDataPort<Vector2>)slave.mXmlBehaviour.GetPort("Target")).Data = temp;
        /*if(mover == null)
        {
            mover = new MoveToPoint();
            mover.SetGlobal();
            mover.ForceActivation();
        }

        kAIDataPort<Vector2> port = mover.targetPoint;

        port.Data = temp;*/


    }
    #region kAIILogger implementation

    public void LogMessage(string lMessage, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        print(lMessage);
        foreach (var kvp in lDetails)
        {
            print(kvp.Key + ": " + kvp.Value);
        }
    }

    public void LogWarning(string lWarning, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        Debug.LogWarning(lWarning);
    }

    public void LogError(string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        Debug.LogError(lError);
    }

    public void LogCriticalError(string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
    {
        Debug.LogError(lError);
    }

    #endregion

}
