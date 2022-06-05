/**
 *	Project:		Pixel Walker
 *	
 *	Description:	ActivateAgent is an ML-Agents agent. The activate agent activates
 *					in game objects that have an ActivatableTrigger attached.
 *					
 *					Currently, this agent does not take a ml-agents model to drive
 *					actions, but is coded with procedural movements controlled by
 *					inverse kinimatics.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using System.Collections;
using System.Threading.Tasks;

using Unity.MLAgents.Actuators;

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActivateAgent : AgentBase
{
	[Header("Rig On Off Interpolation")]
	public TwoBoneIKConstraint armIKRotationRig;
	public ChainIKConstraint armIKPositionRig;
	public Rig headAimRig;

	[Header("Assigned at RTime")]
	[SerializeField]
	private ActivationTrigger activationTrigger;
	private Transform targetSwitchTransform;

	private Rigidbody handTargetRb;
	private Transform handTargetTransform;
	private Collider handTargetCollider;
	private Vector3 handTargetStartPosLocal;

	public override BehaviorType MyBehaviorType => BehaviorType.Activate;

	// don't use model decision during rotation to target
	bool areActionsOverriden;

	bool didHandTriggerActivatable;
	BehaviorType behaviorRequested;

	protected override void Awake()
	{
		base.Awake();

		playerArea = GetComponentInParent<AgentArea>().transform;
		handTargetTransform = agentBody.GetComponentInChildren<HandTarget>().transform;
		handTargetRb = handTargetTransform.GetComponent<Rigidbody>();
		handTargetCollider = handTargetTransform.GetComponent<Collider>();
		handTargetStartPosLocal = handTargetRb.transform.localPosition;
	}

	public Task<BehaviorResult> PerformeBehavior(Transform target, BehaviorType behaviorType)
	{
		behaviorRequested = behaviorType;
		return base.PerformeBehavior(target);
	}
	protected override void initializeBehavior()
	{
		// before starting the behavior, rotate so arm faces the target
		// and then activate the hand target and inverse kinimatic rig
		activationTrigger = target.GetComponentInChildren<ActivationTrigger>();
		
		// if there was an activation triger on this target
		if (activationTrigger)
		{
			// check if this is a toggle, then check to see if it can be turned off or on
			if (activationTrigger is ToggleSwitch)
			{
				var toggleSwitch = activationTrigger as ToggleSwitch;
				var state = toggleSwitch.CurrentState;
				
				if (state == ActivationState.On)
				{
					if (behaviorRequested == BehaviorType.TurnOn)
					{
						var msg = "This item is already turned on!";
						StopBehavior(false, msg);
						return;         // EARLY OUT
					}
				}
				else if (state == ActivationState.Off)
				{
					if (behaviorRequested == BehaviorType.TurnOff)
					{
						var msg = "This item is already turned off!";
						StopBehavior(false, msg);
						return;         // EARLY OUT
					}
				}
			}

		}
		else
		{
			var msg = "I can't see anything to activate on this object.\n" +
				"There are not buttons or switches.";
			StopBehavior(false, msg);
			return;			// EARLY OUT
		}

		// PERFORM THE ACTIVATION!
		targetSwitchTransform = activationTrigger.transform;
		areActionsOverriden = false;
		didHandTriggerActivatable = false;
		PerformActivate();

	}
	private async void PerformActivate()
	{
		// face the target and
		// turn on arm inverse kinimatics
		handTargetCollider.enabled = true;
		areActionsOverriden = true;
		movementValues.ClearValues(); // otherwise will use last input action
		await rotateTowardsTargetWithOffset(-35f, 3.0f);
		await Task.Delay(800);
		
		// enable hand IK and reach for trigger
		StartCoroutine(SetHandRigEnabled(true, .3f));
		await TouchActivatableTrigger(0.65f);

		// if the switch wasn't triggered, trigger it now
		if (!didHandTriggerActivatable)
		{
			activationTrigger.TriggerActivatables();
			didHandTriggerActivatable = true;
		}

		await Task.Delay(200);

		// disable hand IK and reset hand target position
		await SetHandRigEnabled(false, 0.4f);
		await Task.Delay(100);
		
		var handStartingPos = agentBody.TransformPoint(handTargetStartPosLocal);
		SetHandIKPosition(handStartingPos);
		handTargetCollider.enabled = false;
		areActionsOverriden = false;
		StopBehavior(true);
	}
	private async Task rotateTowardsTargetWithOffset(float offsetDegrees, float epsilon)
	{
		// direction the agent should be facing
		var targetPos = new Vector3(target.position.x, 0, target.position.z);
		var agentPos = new Vector3(agentBody.position.x, 0, agentBody.position.z);
		Vector3 dirFromAgentToTarget = targetPos - agentPos;

		Vector3 dirWithOffset = Quaternion.AngleAxis(offsetDegrees, Vector3.up) * dirFromAgentToTarget;
		await RotateToDir(dirWithOffset, epsilon);
	}
	
	IEnumerator SetHandRigEnabled(bool enabled, float duration)
	{
		float rigTargetWeight = enabled ? 1f : 0f;
		float rigRotationIKStartWeight = armIKRotationRig.weight;
		float rigPositionIKStartWeight = armIKPositionRig.weight;
		var elapsedTime = 0f;

		// lerp rig weight to none or full
		while (duration > elapsedTime)
		{
			var rotWeight = Mathf.Lerp(rigRotationIKStartWeight, rigTargetWeight, elapsedTime / duration);
			var posWeight = Mathf.Lerp(rigPositionIKStartWeight, rigTargetWeight, elapsedTime / duration);
			armIKRotationRig.weight = rotWeight;
			armIKPositionRig.weight = posWeight;
			elapsedTime += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		// done lerping.  Make sure weight is set to target in case it didn't lerp fully
		armIKRotationRig.weight = rigTargetWeight;
		armIKPositionRig.weight = rigTargetWeight;
	}
	IEnumerator TouchActivatableTrigger(float duration)
	{
		var elapsedTime = 0f;
		var startingPos = agentBody.TransformPoint(handTargetStartPosLocal);

		// lerp hand IK target position to the switch
		while (duration > elapsedTime && !didHandTriggerActivatable)
		{
			var nextPosition = Vector3.Slerp(startingPos, targetSwitchTransform.position, elapsedTime / duration);
			SetHandIKPosition(nextPosition);
			elapsedTime += Time.fixedDeltaTime;

			Debug.DrawLine(targetSwitchTransform.position, startingPos, Color.magenta);
			yield return new WaitForFixedUpdate();
		}

		if (!didHandTriggerActivatable)
		{
			Debug.LogError("didn't reach activation switch");
		}
	}
	private void SetHandIKPosition(Vector3 pos)
	{
		handTargetRb.position = pos;
		handTargetRb.velocity = Vector3.zero;
		handTargetRb.angularVelocity = Vector3.zero;
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		if (!areActionsOverriden)
		{
			movementValues.handForwardMovement = actions.DiscreteActions[0];
			movementValues.handSideMovement = actions.DiscreteActions[1];
			movementValues.handVerticalMovement = actions.DiscreteActions[2];
		}
	}

	/// <summary>
	/// Called by the trigger when it detects a collision with the hand IK target
	/// </summary>
	public void SetTriggerActivated()
	{
		// the object colliding with the hand IK target should be the switch
		didHandTriggerActivatable = true;
	}

	/// <summary>
	/// Heuristic is called where there is not Model assigned and
	/// ML-Agents is not training. Heuristic checks for user input
	/// and assignes that input to the agents action buffer, which
	/// the OnActionsRecieved base method will use to move the agent.
	/// </summary>
	/// <param name="actionsOut">action buffer to populate</param>
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var actions = actionsOut.DiscreteActions;
		actions[0] = userInputValues.handForward;
		actions[1] = userInputValues.handSide;
		actions[2] = userInputValues.handVertical;
	}
}