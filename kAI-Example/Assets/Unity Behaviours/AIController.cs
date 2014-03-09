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

	kAIDataPort<float> lHealthPort;
	
	// Use this for initialization
	void Start () {
		aiInterface = GetComponent<kAIUnityBehaviour>();
		controller = GetComponent<CharacterController>();
		swordController = GetComponentInChildren<SwordController>();
		health = GetComponent<HealthBehaviour>();
		
//		aiInterface.GetPort("SwingSword ").OnTriggered += SwingSword_OnTriggerered;
		
		aiInterface.SetData<GameObject>("Target", enemy);

		lHealthPort = (kAIDataPort<float>)aiInterface.GetPort ("CharHealth");
	}
	
	void SwingSword_OnTriggerered(kAIPort lSender)
	{
		swordController.SwingSword(); 
		wasSwingingSword = swordController.swordSwinging;
	}
	
	// Update is called once per frame
	void Update () {
		/*if(wasSwingingSword && !swordController.swordSwinging)
		{
			aiInterface.TriggerPort("SwordSwingFinished");
		}*/
		
		/*if((enemy.transform.position - transform.position).sqrMagnitude < 2 * 2)
		{
			aiInterface.TriggerPort("PlayerNear");	
		}
		
		if((enemy.transform.position - transform.position).sqrMagnitude > 5 * 5)
		{
			aiInterface.TriggerPort("PlayerNotNear");	
		}*/
		/*else
		{
			
			if(enemy.transform.position.x - transform.position.x > 0)
			{
				aiInterface.TriggerPort("PlayerBehind");	
			}
			
		}*/
		
		lHealthPort.Data = health.CurrentHealth;
	}

	public GameObject GetTarget()
	{
		return enemy;
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

	public static bool IsGameObjectNear(GameObject me, GameObject lOtherObject, float lDist)
	{
		return (me.transform.position - lOtherObject.transform.position).sqrMagnitude <= (lDist * lDist);
	}
}

public class MoveTowardsPlayer : kAICodeBehaviour
{
	kAIDataPort<GameObject> lThingToFollowPort;
	public MoveTowardsPlayer(kAIILogger lLogger)
		:base(lLogger)
	{
		lThingToFollowPort = new kAIDataPort<GameObject>("Target", kAIPort.ePortDirection.PortDirection_In, null);
		AddExternalPort(lThingToFollowPort);
	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{
		GameObject lGameObject = lUserData as GameObject;
		if(lGameObject != null)
		{
			CharacterController lCharController = lGameObject.GetComponent<CharacterController>();
			GameObject enemy = lThingToFollowPort.Data;
			if(enemy != null)
			{
				if(enemy.transform.position.x < lGameObject.transform.position.x - 2)
				{
					
					lCharController.MoveLeft();
				}
				else if(enemy.transform.position.x > lGameObject.transform.position.x + 2)
				{
					lCharController.MoveRight();
				}
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
		kAITriggerPort lMoveLeftPort = new kAITriggerPort("MoveLeft", kAIPort.ePortDirection.PortDirection_In, null);
		lMoveLeftPort.OnTriggered += MoveLeft_OnTriggered;
		AddExternalPort(lMoveLeftPort);
		
		kAITriggerPort lMoveRightPort = new kAITriggerPort("MoveRight", kAIPort.ePortDirection.PortDirection_In, null);
		lMoveRightPort.OnTriggered += MoveRight_OnTriggered;
		AddExternalPort(lMoveRightPort);
		
		kAITriggerPort lStopMovingPort = new kAITriggerPort("StopMoving", kAIPort.ePortDirection.PortDirection_In, null);
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


