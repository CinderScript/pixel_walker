using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

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
	public SpawnPoints spawnPoints;
	public AreaProps areaProps;
	public Transform target;
	public UserInputValues userInputValues;
	public CharacterMovementInput movementValues;

	// REWARD WEIGHTS
	private const float SUCCESS_REWARD = 3;
	private const float TIME_PENALTY = -0.001f;
	private const float COLLISION_PENALTY = -0.002f;

	private Vector3 startPos;

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

		collisionThrower.OnCharacterCollision += OnCollision;

		startPos = agentReference.transform.position;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<PlayerArea>().transform;

		spawnPoints = playerArea.GetComponentInChildren<SpawnPoints>();
		areaProps = playerArea.GetComponentInChildren<AreaProps>();
		movementValues = playerArea.GetComponentInChildren<CharacterMovementInput>();
	}

	public override void OnEpisodeBegin()
	{
		var spawn = spawnPoints.SelectRandomLocation().position;
		spawn.y = startPos.y; // preserve starting height

		target = areaProps.SelectRandomProp().transform;
		agentBody.transform.position = spawn;
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//Position of target relative to character's position
		sensor.AddObservation(transform.InverseTransformDirection(target.position));
		sensor.AddObservation(transform.InverseTransformPoint(target.position));

		sensor.AddObservation(transform.InverseTransformPoint(playerArea.position));
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		movementValues.bodyForwardMovement = actions.DiscreteActions[0];
		movementValues.bodyRotation = actions.DiscreteActions[1];

		AssignRewards();
	}

	private void AssignRewards()
	{
		// get distance to target - ignore height displacement
		Vector3 charPos = new Vector3(transform.position.x, 0, transform.position.z);
		Vector3 targetPos = new Vector3(target.position.x, 0, target.position.z);

		var distance = Vector3.Distance(charPos, targetPos);
		//Debug.Log(distance);
		if (distance < success_distance)
		{
			AddReward(SUCCESS_REWARD);
			EndEpisode();
		}
		else
		{
			AddReward(TIME_PENALTY);
		}
	}

	private void OnCollision(ControllerColliderHit hitInfo)
	{
		// penalize
		AddReward(COLLISION_PENALTY);
	}

	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position
		Gizmos.color = Color.green;
		if (agentBody)
		{
			Gizmos.DrawSphere(agentBody.transform.position, .3f);
		}
		if (target)
		{
			Vector3 pos = new Vector3(target.position.x, 0, target.position.z);
			Gizmos.DrawSphere(pos, .3f);
		}
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
}