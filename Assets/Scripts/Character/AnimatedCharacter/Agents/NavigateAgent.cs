/**
 *	Project:		Pixel Walker
 *	
 *	Description:	NavigateAgent is an ML-Agents agent. This agent uses
 *					a trained neural network to navigate around the 
 *					environment and locate a given target.  This agent uses
 *					a several observations to navigate successfully:
 *						VectorSensors that racast out from his feet.
 *						GridSensor that gives corse grained data about surroundings.
 *						One-hot encoding of the agent's current room.
 *						One-hot encoding of the target's room.
 *						Rotational and positional vector data.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

[DisallowMultipleComponent]
public class NavigateAgent : AgentBase
{
	[Header("For Training")]
	public float defaultSuccessDistance = 0.7f;
	public float standTargetSuccessDistanceOverride = 0.3f;
	private float successDistance;

	[Header("Assigned at RTime")]
	[SerializeField]
	private RoomName currentRoom;
	[SerializeField]
	private Room targetRoom;
	[SerializeField]
	private bool isCurrentlyColliding = false;
	[SerializeField]
	private float distanceToTarget;

	public override BehaviorType MyBehaviorType => BehaviorType.Navigate;

	// REWARD WEIGHTS
	private const float SUCCESS_REWARD = 5;
	private const float TIME_PENALTY = -0.001f;
	
	private const float COLLISION_PENALTY_DEFAULT = -0.002f;
	private float collisionPenalty;

	int numberOfRooms;

	bool areActionsOverriden = false;

	// used to detect if the agent is currently colliding
	// with an obstical so it can be used as an observation
	private ControllerColliderHit lastHit;

	protected override void Awake()
	{
		base.Awake();
		
		var collisionThrower = agentBody.GetComponent<CollisionThrower>();
		collisionThrower.OnCharacterCollision += CharacterCollisionHandler;

		numberOfRooms = Enum.GetValues(typeof(RoomName)).Length;
	}

	protected override void initializeBehavior()
	{
		// is the target a location or an object?
		Room room = target.GetComponent<Room>();
		// if it is a location then...
		if (room)
		{
			successDistance = 2.0f;
			targetRoom = room;
		}
		else
		{
			targetRoom = target.GetComponent<PropInfo>().room;

			// get penalty for this lesson in curriculum - only used during training
			collisionPenalty = Academy.Instance.EnvironmentParameters
				.GetWithDefault("collision_penalty", COLLISION_PENALTY_DEFAULT);

			var standTarget = target.GetComponentInChildren<StandTarget>();

			if (standTarget)
			{
				target = standTarget.transform;
				successDistance = standTargetSuccessDistanceOverride;
			}
			else
			{
				successDistance = defaultSuccessDistance;
			}
		}
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//Position of target relative to character's position
		sensor.AddObservation(transform.InverseTransformDirection(target.position));
		sensor.AddObservation(transform.InverseTransformPoint(target.position));
		sensor.AddObservation(transform.eulerAngles); // north is always north for any PlayerArea

		// Position of character relative to the Player Area
		sensor.AddObservation( playerArea.transform.position - transform.position);

		// position of the target relative to the player area
		sensor.AddObservation(playerArea.transform.position - target.position);

		// position of the target room
		var roomPos = playerArea.transform.position - targetRoom.transform.position;

		// add one hot encoding for the current room of player
		// add one hot encoding for the room of the target
		sensor.AddOneHotObservation((int)currentRoom, numberOfRooms);
		sensor.AddOneHotObservation((int)targetRoom.Name, numberOfRooms); // make goal-signal?

		// report and consume any collisions
		Vector2 collisionVector;
		if (lastHit != null)
		{
			// isCurrentlyColliding is true
			var point = agentBody.InverseTransformPoint(lastHit.point);
			collisionVector = new Vector2(point.x, point.z);
			lastHit = null; // consume
		}
		else
		{
			collisionVector = Vector2.zero;
			isCurrentlyColliding = false;
		}

		sensor.AddObservation(collisionVector);
		sensor.AddObservation(isCurrentlyColliding);

	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		if (!areActionsOverriden)
		{
			movementValues.bodyForwardMovement = actions.DiscreteActions[0];
			movementValues.bodyRotation = actions.DiscreteActions[1];
			AssignRewards();
		}
	}
	
	public void SetCurrentRoom(Room room)
	{
		currentRoom = room.Name;
	}

	private void AssignRewards()
	{
		// done with episode?
		bool giveTimePenalty = true;
		
		// don't let the agent trigger the reward based on distance if the agent
		// is not in the target's room.
		if (currentRoom == targetRoom.Name)
		{
			// get distance to target - ignore height displacement
			Vector3 charPos = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

			distanceToTarget = Vector3.Distance(charPos, targetPos);
			if (distanceToTarget < successDistance)
			{
				giveTimePenalty = false;
				Finished_Success();
			}
		}

		// make agent work quickly
		if (!giveTimePenalty)
		{
			AddReward(TIME_PENALTY);
		}
	}
	private async void Finished_Success() {
		AddReward(SUCCESS_REWARD);

		areActionsOverriden = true;
		movementValues.ClearValues(); // otherwise will use last input action
		await RotateTowardsPos(target.position, 5f);
		areActionsOverriden = false;
		StopBehavior(true);         // base class signal stop
	}
	private void CharacterCollisionHandler(GameObject thrower, ControllerColliderHit hitInfo)
	{
		if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Structure"))
		{
			lastHit = hitInfo;
			isCurrentlyColliding = true;
			AddReward(collisionPenalty);
		}
		// penalize
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
		actions[0] = userInputValues.forward;
		actions[1] = userInputValues.rotate;
	}

	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position
		Gizmos.color = Color.magenta;
		if (target)
		{
			Vector3 pos = new Vector3(target.position.x, 0, target.position.z);
			Gizmos.DrawSphere(pos, .45f);
		}
	}
}