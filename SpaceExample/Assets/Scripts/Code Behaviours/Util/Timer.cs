using UnityEngine;
using System.Collections;

using kAI.Core;

public class Timer : kAICodeBehaviour{

    kAIDataPort<float> totalTime;


    kAITriggerPort onTimeElapsed;

    float timeElapsed;

	public Timer()
    {
        totalTime = new kAIDataPort<float>("TotalTime", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(totalTime);

        onTimeElapsed = new kAITriggerPort("TimeElapsed", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(onTimeElapsed);

        kAITriggerPort reset = new kAITriggerPort("Reset", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(reset);
        reset.OnTriggered += reset_OnTriggered;
    }

    void reset_OnTriggered(kAIPort lSender)
    {
        timeElapsed = 0.0f;
    }

    protected override void OnActivate()
    {
        timeElapsed = 0.0f;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        timeElapsed += lDeltaTime;

        if(timeElapsed > totalTime.Data)
        {
            onTimeElapsed.Trigger();
            timeElapsed = 0.0f;
        }
    }
}
