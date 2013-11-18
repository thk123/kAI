using System;
using UnityEngine;
using kAI.Core;

namespace AssemblyCSharp
{
	public class TestBehaviour : kAICodeBehaviour
	{
		public TestBehaviour (kAIILogger lLogger)
			:base(lLogger)
		{
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
			GameObject lBehaviour = lUserData as GameObject;
			if(lBehaviour != null)
			{
				lBehaviour.transform.Translate(-1, 0, 0);	
				
			}
		}
	}
}

