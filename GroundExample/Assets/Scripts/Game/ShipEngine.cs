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
        RaycastHit lInfo;
        if (Physics.Raycast(new Ray(transform.position, -50 * Vector3.up), out lInfo))
        {
            transform.position = new Vector3(transform.position.x, lInfo.point.y + 30.0f, transform.position.z);
        }
        currentDirection = Vector3.zero;
	}

    

    public void SetDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            SetLookAt(direction);
        }
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
            engine.SetDirection(velocityPort.Data);
            if(velocityPort.Data == Vector3.zero && facePort.Data != Vector3.zero)
            {
                engine.SetLookAt(facePort.Data);
            }
            velocityPort.Data = Vector3.zero;
            facePort.Data = Vector3.zero;
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