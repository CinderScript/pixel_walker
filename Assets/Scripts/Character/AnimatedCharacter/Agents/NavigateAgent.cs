using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

[DisallowMultipleComponent]
public class NavigateAgent : AgentBase
{
	[Header("For Training")]
	public float success_distance = 0.35f;

	[Header("Assigned at RTime")]
	[SerializeField]
	private RoomName currentRoom;
	[SerializeField]
	private Room targetRoom;
	[SerializeField]
	private bool isCurrentlyColliding = false;
	[SerializeField]
	private float distanceToTarget;
	[SerializeField]
	private CollisionThrower collisionThrower;

	// REWARD WEIGHTS
	private const float SUCCESS_REWARD = 5;
	private const float TIME_PENALTY = -0.001f;
	
	private const float COLLISION_PENALTY_DEFAULT = -0.002f;
	private float collisionPenalty;

	int numberOfRooms;

	// used to detect if the agent is currently colliding
	// with an obstical so it can be used as an observation
	private ControllerColliderHit lastHit;

	private void Awake()
	{
		collisionThrower = agentBody.GetComponent<CollisionThrower>();
		collisionThrower.OnCharacterCollision += CharacterCollisionHandler;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<AgentArea>().transform;

		numberOfRooms = Enum.GetValues(typeof(RoomName)).Length;
	}

	protected override void initializeBehavior()
	{
		targetRoom = target.GetComponent<PropInfo>().room;

		// get penalty for this lesson in curriculum - only used during training
		collisionPenalty = Academy.Instance.EnvironmentParameters
			.GetWithDefault("collision_penalty", COLLISION_PENALTY_DEFAULT);
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
		sensor.AddOneHotObservation((int)targetRoom.RoomName, numberOfRooms); // make goal-signal?

		// report and consume any collisions
		Vector2 collisionVector;
		if (lastHit != null)
		{
			// isCurrentlyColliding is true
			var point = agentBody.transform.InverseTransformPoint(lastHit.point);
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
		movementValues.bodyForwardMovement = actions.DiscreteActions[0];
		movementValues.bodyRotation = actions.DiscreteActions[1];
		AssignRewards();
	}
	
	public void SetCurrentRoom(Room room)
	{
		currentRoom = room.RoomName;
	}

	private void AssignRewards()
	{
		// Sparce reward given?
		bool wasRewarded = false;
		
		// don't let the agent trigger the reward based on distance if the agent
		// is not in the target's room.
		if (currentRoom == targetRoom.RoomName)
		{
			// get distance to target - ignore height displacement
			Vector3 charPos = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

			distanceToTarget = Vector3.Distance(charPos, targetPos);
			if (distanceToTarget < success_distance)
			{
				StopBehavior();		// we are finished
				wasRewarded = true;
				AddReward(SUCCESS_REWARD);
				EndEpisode();
			}
		}

		// make agent work quickly
		if (!wasRewarded)
		{
			AddReward(TIME_PENALTY);
		}
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