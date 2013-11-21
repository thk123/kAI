using UnityEngine;
using kAI.Core;
using System.Collections;

[RequireComponent(typeof(kAIUnityBehaviour))]
public class FunctionTest : MonoBehaviour {

	kAIUnityBehaviour lAIController;

	public int lValue1;
	public int lValue2;

	kAIDataPort<int> lNumber1Port;
	kAIDataPort<int> lNumber2Port;

	// Use this for initialization
	void Start () {
		lAIController = GetComponent<kAIUnityBehaviour>();
		lNumber1Port = (kAIDataPort<int>)lAIController.GetPort("number1");
		lNumber2Port = (kAIDataPort<int>)lAIController.GetPort("number2");
	}
	
	// Update is called once per frame
	void Update () {
		lNumber1Port.Data = lValue1;
		lNumber2Port.Data = lValue2;
	}
}


public class PrintCodeBehaviour : kAICodeBehaviour
{
	kAIDataPort<string> lMessagePort;
	public PrintCodeBehaviour (kAIILogger lLogger)
		:base(lLogger)
	{
		lMessagePort = new kAIDataPort<string>("Message", kAIPort.ePortDirection.PortDirection_In, null);
		AddExternalPort(lMessagePort);
		/*kAIPort lTestA = new kAIPort("AnotherPort", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType);
            lTestA.OnTriggered+=new kAIPort.TriggerEvent(lTestA_OnTriggered);
            AddExternalPort(lTestA);*/
	}
	
	void lTestA_OnTriggered(kAIPort lSender)
	{
		// This is an example of illegal code as we cannot guarentee if the port triggered will have been released
		// yet or not. Calling code like this within a trigger response will throw an exception.
		//GetPort("Deactivate").Trigger();
	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		LogMessage(lMessagePort.Data);
		Deactivate();
	}

}