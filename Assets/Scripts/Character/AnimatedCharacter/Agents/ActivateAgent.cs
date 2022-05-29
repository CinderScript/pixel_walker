using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

public class ActivateAgent : AgentBase
{
	[Header("Assigned at RTime")]
	[SerializeField]
	private float distanceToTarget;

	public override BehaviorType MyBehaviorType => BehaviorType.Activate;

	protected override void Awake()
	{
		base.Awake();

		// setup hand collision detection
		var hand = agentBody.GetComponentInChildren<AgentHand>();
		var handCollisionThrower = hand.GetComponent<CollisionThrower>();
		handCollisionThrower.OnCollisionEnterEvent += HandCollision;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<AgentArea>().transform;
	}

	protected override void initializeBehavior()
	{
		
	}
	
	public override void CollectObservations(VectorSensor sensor)
	{
	}
	public override void OnActionReceived(ActionBuffers actions)
	{
		movementValues.handForwardMovement = actions.DiscreteActions[0];
		movementValues.handSideMovement = actions.DiscreteActions[1];
		movementValues.handVerticalMovement = actions.DiscreteActions[2];

		AssignRewards();
	}

	public void AssignRewards()
	{

	}

	public void HandCollision(GameObject thrower, Collision collision)
	{
		Debug.Log("Hand Collision");
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