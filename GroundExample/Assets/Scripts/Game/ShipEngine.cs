using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class ShipEngine : MonoBehaviour {

    public float maxSpeed;

    public Vector3 currentDirection;

	// Use this for initialization
	void Start () 
	{
        currentDirection = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
        transform.position += currentDirection;

	}

    

    public void SetDirection(Vector3 direction)
    {
        SetLookAt(direction);
        currentDirection = direction.normalized * maxSpeed;
        currentDirection.y = 0;
    }

    internal void SetLookAt(Vector3 newLook)
    {
        if (newLook != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(newLook);
        }
    }
}


public class ShipEngineController : kAIUnityAIBehaviour
{
    kAIDataPort<Vector3> velocityPort;
    kAIDataPort<Vector3> facePort;

    public ShipEngineController()
        :base(null)
    {
        velocityPort = new kAIDataPort<Vector3>("Velocity", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(velocityPort);

        facePort = new kAIDataPort<Vector3>("FaceTo", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(facePort);
    }

    

    public override void UnityInternalUpdate(float lDeltaTime, GameObject lShipObject)
    {
        //LogMessage("Running ship update");
        ShipEngine engine = lShipObject.GetComponent<ShipEngine>();
        if(engine != null)
        {
            if (velocityPort.Data != Vector3.zero)
            {
                //LogMessage("Setting velocity: " + (velocityPort.Data).ToString());
                engine.SetDirection(velocityPort.Data);
            }
            else if (facePort.Data != Vector3.zero)
            {
                engine.SetLookAt(facePort.Data);
            }
            else
            {
                //LogMessage("Nothing receivedf");
            }
        }
        else
        {
            throw new Exception("Attached a ShipEngineController to an object without an engine!");
        }
    }
}

public class WeightedVectorAverage : kAICodeBehaviour
{
    kAIEnumerableDataPort<KeyValuePair<Vector3, float>> weightedVectorsPort;

    kAIDataPort<Vector3> resultPort;

    public WeightedVectorAverage()
        :base(null)
    {
        weightedVectorsPort = new kAIEnumerableDataPort<KeyValuePair<Vector3, float>>("Vectors", null);
        AddExternalPort(weightedVectorsPort);

        resultPort = new kAIDataPort<Vector3>("Result", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(resultPort);
    }


    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        Vector3 resultVector = Vector3.zero;

        foreach(KeyValuePair<Vector3, float> entry in weightedVectorsPort.Values)
        {
            resultVector += (entry.Key * entry.Value);
        }

        resultPort.Data = resultVector;
    }
}