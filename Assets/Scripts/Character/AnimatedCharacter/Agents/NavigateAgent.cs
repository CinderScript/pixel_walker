using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

[DisallowMultipleComponent]
public class NavigateAgent : Agent
{
	[Header("Agent and Area")]
	public GameObject agentReference;
	public GameObject agentBody;

	[Header("Collision Detection")]
	public CollisionThrower collisionThrower;

	[Header("For Training")]
	public float success_distance = 0.35f;

	[Header("Assigned at RTime")]
	public Transform playerArea;
	public SpawnPointReferences spawnPoints;
	public AreaProps areaProps;
	public Transform target;
	public UserInputValues userInputValues;
	public CharacterMovementInput movementValues;
	public Room currentRoom;
	public Room targetRoom;
	public bool isCurrentlyColliding = false;
	public float distanceToTarget;

	// REWARD WEIGHTS
	private const float SUCCESS_REWARD = 5;
	private const float TIME_PENALTY = -0.001f;
	
	private const float COLLISION_PENALTY_DEFAULT = -0.002f;
	private float collisionPenalty;

	private Vector3 startPos;
	private VectorSensorComponent targetRoomSensorComponent;
	int numberOfRooms;

	// used to detect if the agent is currently colliding
	// with an obstical so it can be used as an observation
	private ControllerColliderHit lastHit;

	private void Awake()
	{
		// GET USER INPUT SCRIPT
		UserInputValues[] userInputScripts = FindObjectsOfType(typeof(UserInputValues)) as UserInputValues[];
		if (userInputScripts.Length > 1)
		{
			Debug.LogError($"{userInputScripts.Length} were found. Only one is allowed.");
		}
		else if (userInputScripts.Length == 0)
		{
			Debug.LogError("No UserInputValues scripts was found. Please add one.");
		}
		else
		{
			userInputValues = userInputScripts[0];
		}

		// GET ROOT OF AGENT OBJECT
		var root = GetComponentInParent<CharacterRoot>();
		agentReference = root.gameObject;
		
		// GET AGENT'S CHARACTER CONTROLLER	(for observations)	
		var controller = GetComponentInParent<CharacterController>();
		agentBody = controller.gameObject;

		collisionThrower.OnCharacterCollision += CharacterCollisionHandler;

		startPos = agentReference.transform.position;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<PlayerArea>().transform;

		spawnPoints = playerArea.GetComponentInChildren<SpawnPointReferences>();
		areaProps = playerArea.GetComponentInChildren<AreaProps>();
		movementValues = playerArea.GetComponentInChildren<CharacterMovementInput>();

		numberOfRooms = Enum.GetValues(typeof(RoomName)).Length;

		targetRoomSensorComponent = transform.GetComponent<VectorSensorComponent>();
	}

	public override void OnEpisodeBegin()
	{
		// Randomly place the agent at one of the spawn locations
		var spawn = spawnPoints.SelectRandomLocation();
		var spawnPos = spawn.transform.position;
		spawnPos.y = startPos.y; // preserve starting height
		agentBody.transform.position = spawnPos;

		// select a random prop to find
		target = areaProps.SelectRandomProp().transform;
		targetRoom = target.GetComponent<PropInfo>().room;

		// get the starting room
		currentRoom = spawn.room;

		// get penalty for this lesson in curriculum
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
		sensor.AddOneHotObservation((int)currentRoom.RoomName, numberOfRooms);
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
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var actions = actionsOut.DiscreteActions;
		actions[0] = userInputValues.forward;
		actions[1] = userInputValues.rotate;
	}
	
	public void SetCurrentRoom(Room room)
	{
		currentRoom = room;
	}

	private void AssignRewards()
	{
		// Sparce reward given?
		bool wasRewarded = false;
		
		// don't let the agent trigger the reward based on distance if the agent
		// is not in the target's room.
		if (currentRoom.RoomName == targetRoom.RoomName)
		{
			// get distance to target - ignore height displacement
			Vector3 charPos = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

			distanceToTarget = Vector3.Distance(charPos, targetPos);
			//Debug.Log(distance);
			if (distanceToTarget < success_distance)
			{
				wasRewarded = true;
				AddReward(SUCCESS_REWARD);
				//Debug.Log("Reward: " + GetCumulativeReward());
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
	/// <summary>
	/// Heuristic is called where there is not Model assigned and
	/// ML-Agents is not training. Heuristic checks for user input
	/// and assignes that input to the agents action buffer, which
	/// the OnActionsRecieved base method will use to move the agent.
	/// </summary>
	/// <param name="actionsOut">action buffer to populate</param>
}