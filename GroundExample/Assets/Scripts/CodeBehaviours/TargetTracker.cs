using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using kAI.Core;

public class TargetTracker : kAICodeBehaviour 
{
    kAIDataPort<GameObject> mTargetPort;

    kAITriggerPort mOnTargetFleePort;
    kAIDataPort<float> mTargetHealthPort;
    kAIDataPort<SquadLeader> mSquadPort;

    bool mIsListeningToFleePort;

    public TargetTracker()
        : base()
    {
        mIsListeningToFleePort = false;
        mTargetPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(mTargetPort);

        mOnTargetFleePort = new kAITriggerPort("TargetFleeing", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mOnTargetFleePort);

        mTargetHealthPort = new kAIDataPort<float>("TargetHealth", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mTargetHealthPort);

        mSquadPort = new kAIDataPort<SquadLeader>("OwningSquad", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mSquadPort);
    }

    void lFleeingPort_OnTriggered(kAIPort lSender)
    {
        mOnTargetFleePort.Trigger();
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject lTarget = mTargetPort.Data;
        if (lTarget != null)
        {
            HealthBehaviour lTargetHealth = lTarget.GetComponent<HealthBehaviour>();
            if (lTargetHealth != null)
            {
                mTargetHealthPort.Data = lTargetHealth.CurrentHealth;
            }
            else
            {
                LogWarning("Target does not have a health behaviour");
            }

            SquadLeader squadLeader = lTarget.transform.parent.GetComponent<SquadLeader>();
            if (squadLeader != null)
            {
                mSquadPort.Data = squadLeader;
            }
            else
            {
                LogWarning("No squad leader");
            }

            if(!mIsListeningToFleePort)
            {
                AIBehaviour lAiBehaviour = lTarget.GetComponent<AIBehaviour>();
                if(lAiBehaviour != null)
                {
                    kAITriggerPort lFleeingPort = lAiBehaviour.GetPort("Fleeing") as kAITriggerPort;

                    if (lFleeingPort != null)
                    {
                        lFleeingPort.OnTriggered += new kAITriggerPort.TriggerEvent(lFleeingPort_OnTriggered);
                        mIsListeningToFleePort = true;
                    }
                    else
                    {
                        LogWarning("No fleeing port found on target");
                    }
                }
                else
                {
                    LogWarning("No AI Behaviour found on target");
                }
            }
        }

    }

    protected override void OnDeactivate()
    {
        if(mIsListeningToFleePort)
        {
            GameObject lTarget = mTargetPort.Data;
            if (lTarget != null)
            {
                AIBehaviour lAiBehaviour = lTarget.GetComponent<AIBehaviour>();
                kAITriggerPort lFleeingPort = lAiBehaviour.GetPort("Fleeing") as kAITriggerPort;

                if (lFleeingPort != null)
                {
                    lFleeingPort.OnTriggered -= new kAITriggerPort.TriggerEvent(lFleeingPort_OnTriggered);
                    mIsListeningToFleePort = false;
                }
            }
            base.OnDeactivate();
        }

    }
}

public class PickTargetFromSquadBehaviour : kAICodeBehaviour
{
    kAIDataPort<SquadLeader> mSquadLeaderPort;
    kAITriggerPort mPickTargetPort;
    kAIDataPort<GameObject> mTargetPort;

    public PickTargetFromSquadBehaviour()
        : base()
    {
        mSquadLeaderPort = new kAIDataPort<SquadLeader>("SquadLeader", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(mSquadLeaderPort);

        mPickTargetPort = new kAITriggerPort("PickTarget", kAIPort.ePortDirection.PortDirection_In);
        mPickTargetPort.OnTriggered += new kAITriggerPort.TriggerEvent(mPickTargetPort_OnTriggered);
        AddExternalPort(mPickTargetPort);

        mTargetPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(mTargetPort);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        SelectTarget();
    }

    void mPickTargetPort_OnTriggered(kAIPort lSender)
    {
        LogMessage("Picking new target!");
        SelectTarget();
    }

    private void SelectTarget()
    {
        mTargetPort.Data = WeaponFunctions.GetRandomSquadMember(mSquadLeaderPort.Data);
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if (mTargetPort.Data == null)
        {
            SelectTarget();
        }
    }
}