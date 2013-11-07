using UnityEngine;
using kAI.Core;
using System.Collections;

[RequireComponent(typeof(kAIUnityBehaviour))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HealthBehaviour))]
public class AIController : MonoBehaviour {
	
	kAIUnityBehaviour aiInterface;
	
	CharacterController controller;
	SwordController swordController;
	HealthBehaviour health;
	public GameObject enemy;
	
	bool wasSwingingSword = false;
	
	public float lowHealthThreshhold = 25.0f;
	
	// Use this for initialization
	void Start () {
		aiInterface = GetComponent<kAIUnityBehaviour>();
		controller = GetComponent<CharacterController>();
		swordController = GetComponentInChildren<SwordController>();
		health = GetComponent<HealthBehaviour>();
		
		aiInterface.GetPort("SwingSword").OnTriggered += SwingSword_OnTriggerered;
	}
	
	void SwingSword_OnTriggerered(kAIPort lSender)
	{
		swordController.SwingSword();
		wasSwingingSword = swordController.swordSwinging;
	}
	
	// Update is called once per frame
	void Update () {
		if(wasSwingingSword && !swordController.swordSwinging)
		{
			aiInterface.TriggerPort("SwordSwingFinished");
		}
		
		if((enemy.transform.position - transform.position).sqrMagnitude < 2 * 2)
		{
			aiInterface.TriggerPort("PlayerNear");	
		}
		
		if((enemy.transform.position - transform.position).sqrMagnitude > 5 * 5)
		{
			aiInterface.TriggerPort("PlayerNotNear");	
		}
		/*else
		{
			
			if(enemy.transform.position.x - transform.position.x > 0)
			{
				aiInterface.TriggerPort("PlayerBehind");	
			}
			
		}*/
		
		if(health.CurrentHealth < lowHealthThreshhold)
		{
			aiInterface.TriggerPort("LowHealth");	
		}
		else
		{
			aiInterface.TriggerPort("NotLowHealth");	
		}
	}
}

public class SwordSwingAction : kAICodeBehaviour
{
	bool mStartSwing = false;
	
	SwordController mController;
	
	public SwordSwingAction(kAIILogger lLogger)
		:base(lLogger)
	{}
	
	protected override void OnActivate ()
	{
		base.OnActivate ();
		mStartSwing = true;
	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		if(mStartSwing)
		{
			GameObject lGameObject = lUserData as GameObject;
			if(lGameObject != null)
			{
				mController = lGameObject.GetComponentInChildren<SwordController>();
				mController.SwingSword();
				mStartSwing = false;
			}
		}
		else
		{
			if(mController != null)
			{
				if(!mController.swordSwinging)
				{
					Deactivate();
				}
			}
		}
	}
}

public class MoveTowardsPlayer : kAICodeBehaviour
{
	public MoveTowardsPlayer(kAIILogger lLogger)
		:base(lLogger)
	{}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		GameObject lGameObject = lUserData as GameObject;
		if(lGameObject != null)
		{
			AIController lController = lGameObject.GetComponent<AIController>();
			CharacterController lCharController = lGameObject.GetComponent<CharacterController>();
			if(lController.enemy.transform.position.x < lGameObject.transform.position.x - 2)
			{
				
				lCharController.MoveLeft();
			}
			else if(lController.enemy.transform.position.x > lGameObject.transform.position.x + 2)
			{
				lCharController.MoveRight();
			}
		}
		
	}
}

public class MoveAction : kAICodeBehaviour
{
	enum MoveDirection
	{
		eNoMove,
		eMoveLeft, 
		eMoveRight
	}
	
	MoveDirection moveDirection;
	
	CharacterController mController;
	
	public MoveAction(kAIILogger lLogger)
		:base(lLogger)
	{
		kAIPort lMoveLeftPort = new kAIPort("MoveLeft", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType, null);
		lMoveLeftPort.OnTriggered += MoveLeft_OnTriggered;
		AddExternalPort(lMoveLeftPort);
		
		kAIPort lMoveRightPort = new kAIPort("MoveRight", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType, null);
		lMoveRightPort.OnTriggered += MoveRight_OnTriggered;
		AddExternalPort(lMoveRightPort);
		
		kAIPort lStopMovingPort = new kAIPort("StopMoving", kAIPort.ePortDirection.PortDirection_In, kAIPortType.TriggerType, null);
		lStopMovingPort.OnTriggered += StopMoving_OnTriggered;
		AddExternalPort(lStopMovingPort);
		
		moveDirection = MoveDirection.eNoMove;
	}

	void StopMoving_OnTriggered (kAIPort lSender)
	{
		moveDirection = MoveDirection.eNoMove;
	}

	void MoveRight_OnTriggered (kAIPort lSender)
	{
		moveDirection = MoveDirection.eMoveRight;
	}

	void MoveLeft_OnTriggered (kAIPort lSender)
	{
		moveDirection = MoveDirection.eMoveLeft;
	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		GameObject lGameObject = lUserData as GameObject;
		if(lGameObject != null)
		{
			if(mController == null)
			{
				mController = lGameObject.GetComponent<CharacterController>();
			}
			
			switch (moveDirection) {
			case MoveDirection.eMoveLeft:
				mController.MoveLeft();
				break;
				
			case MoveDirection.eMoveRight:
				mController.MoveRight();
				break;
			default:
			break;
			}
		}
	}
}
