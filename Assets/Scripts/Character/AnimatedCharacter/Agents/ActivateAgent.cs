using System.Collections;
using System.Threading.Tasks;

using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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
	private ActivatableTrigger targetSwitchActivatable;
	private Transform targetSwitch;

	private Rigidbody handTargetRb;
	private Transform handTargetTransform;
	private Vector3 handTargetStartPosLocal;

	public override BehaviorType MyBehaviorType => BehaviorType.Activate;

	// don't use model decision during rotation to target
	bool areActionsOverriden;

	bool isActivated;

	protected override void Awake()
	{
		base.Awake();

		playerArea = GetComponentInParent<AgentArea>().transform;
		handTargetTransform = agentBody.GetComponentInChildren<HandTarget>().transform;
		handTargetRb = handTargetTransform.GetComponent<Rigidbody>();
		handTargetStartPosLocal = handTargetRb.transform.localPosition;

		var handCollisionThrower = handTargetRb.GetComponent<CollisionThrower>();
		handCollisionThrower.OnCollisionEnterEvent += HandCollision;
	}

	protected override void initializeBehavior()
	{
		// before starting the behavior, rotate so arm faces the target
		// and then activate the hand target and inverse kinimatic rig
		targetSwitchActivatable = target.GetComponentInChildren<ActivatableTrigger>();
		targetSwitch = targetSwitchActivatable.transform;

		areActionsOverriden = false;
		isActivated = false;
		PerformActivate();
	}
	private async void PerformActivate()
	{
		// face the target and
		// turn on arm inverse kinimatics

		areActionsOverriden = true;
		movementValues.ClearValues(); // otherwise will use last input action
		await rotateTowardsTargetWithOffset(-35f, 3.0f);
		await Task.Delay(800);
		
		// enable hand IK and reach for trigger
		StartCoroutine(SetHandRigEnabled(true, .3f));
		await TouchActivatableTrigger(0.65f);

		// if the switch wasn't triggered, trigger it now
		if (!isActivated)
		{
			targetSwitchActivatable.TriggerActivatables();
			isActivated = true;
		}
		await Task.Delay(200);

		// disable hand IK and reset hand target position
		await SetHandRigEnabled(false, 0.4f);
		await Task.Delay(100);
		var handStartingPos = agentBody.TransformPoint(handTargetStartPosLocal);
		SetHandIKPosition(handStartingPos);
		areActionsOverriden = false;
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
		while (duration > elapsedTime && !isActivated)
		{
			var nextPosition = Vector3.Slerp(startingPos, targetSwitch.position, elapsedTime / duration);
			SetHandIKPosition(nextPosition);
			elapsedTime += Time.fixedDeltaTime;

			Debug.DrawLine(targetSwitch.position, startingPos, Color.magenta);
			yield return new WaitForFixedUpdate();
		}

		if (!isActivated)
		{
			Debug.Log("didn't reach activation switch");
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


	public void HandCollision(GameObject thrower, Collision collision)
	{
		isActivated = true;
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