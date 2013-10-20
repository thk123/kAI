using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;
using UnityEngine;

namespace TestCodeBehaviours
{
    public class TestBehaviour : kAICodeBehaviour
    {

        public TestBehaviour(kAIILogger lLogger = null)
            : base(lLogger)
        {
            kAIPort lTestA = new kAIPort("AnotherPort", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType);
            lTestA.OnTriggered+=new kAIPort.TriggerEvent(lTestA_OnTriggered);
            AddExternalPort(lTestA);
        }

        void lTestA_OnTriggered(kAIPort lSender)
        {
            // This is an example of illegal code as we cannot guarentee if the port triggered will have been released
            // yet or not. Calling code like this within a trigger response will throw an exception.
            //GetPort("Deactivate").Trigger();
        }

        int i = 0;

        protected override void InternalUpdate(float lDeltaTime, object lUserData)
        {
            MonoBehaviour lBehaviour = lUserData as MonoBehaviour;
            if (lBehaviour != null)
            {
                lBehaviour.transform.Translate(10.0f, 10.0f, 0.0f);
            }
            LogMessage("Hello World!");
            ++i;

            if (i > 50)
            {
            //    Deactivate();
            }
        }
    }
}
