using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using kAI.Core;

[RequireComponent(typeof(ShipEngine))]
public class SimpleShipControl : MonoBehaviour, kAIILogger {

	[System.Serializable]
	public enum SimpleShipController
	{
		Stabilize,
		AdvanceToPoint,
		RotateToAngle,
		None, 
	}

	kAICodeBehaviour selectedMode;

	public SimpleShipController Type = SimpleShipController.None;

	public float parameter;

	Dictionary<SimpleShipController, kAICodeBehaviour> controllers;
	// Use this for initialization
	void Start () {

        kAIObject.GlobalLogger = this;
        //kAIObject.ExceptionHandler = HandleException;
        kAIObject.ExceptionOnAssert = true;

		controllers = new Dictionary<SimpleShipController, kAICodeBehaviour>();
        LogMessage("Calling constructors");
		controllers.Add(SimpleShipController.Stabilize, new AIRotationalStabilize());
		controllers.Add(SimpleShipController.RotateToAngle, new AIRotateToPoint());
		controllers.Add(SimpleShipController.AdvanceToPoint, new AdvanceTo());


		
		if(controllers.TryGetValue(Type, out selectedMode))
		{

			selectedMode.SetGlobal();
			selectedMode.ForceActivation();

            kAIDataPort dataPort = (kAIDataPort)selectedMode.GetPort("Data");
			dataPort.SetData(parameter);
		}
		else
		{
			selectedMode = null;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(selectedMode != null)
		{
			selectedMode.Update(Time.fixedDeltaTime, gameObject);
		}
		else
		{
			if(controllers.TryGetValue(Type, out selectedMode))
			{
				selectedMode.SetGlobal();
				selectedMode.ForceActivation();
				((kAIDataPort)selectedMode.GetPort("Data")).SetData(parameter);
			}
		}
	}

	
	#region kAIILogger implementation
	
	public void LogMessage (string lMessage, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		print (lMessage);
		foreach(var kvp in lDetails)
		{
			print (kvp.Key + ": " + kvp.Value);
		}
	}
	
	public void LogWarning (string lWarning, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogWarning(lWarning);
	}
	
	public void LogError (string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogError (lError);
	}
	
	public void LogCriticalError (string lError, params System.Collections.Generic.KeyValuePair<string, object>[] lDetails)
	{
		Debug.LogError (lError);
	}
	
	#endregion
}
