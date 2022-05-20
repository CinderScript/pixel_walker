using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

public class NavigateAgent : Agent
{
	[Header("Agent and Target")]
	public GameObject agentReference;
	public GameObject agentBody;
	public Transform target;

	[Header("Collision Detection")]
	public CollisionThrower CollisionThrower;

	[Header("Heuristic Input")]
	public UserInputValues userInputValues;
	public CharacterMovementInput movementValues;

	[Header("Spawn")]
	public SpawnPoints spawnPoints;

	[Header("For Training")]
	public AreaProps AreaProps;
	public float success_distance = 0.35f;

	// REWARD WEIGHTS
	private const float SUCCESS_REWARD = 3;
	private const float TIME_PENALTY = -0.001f;
	private const float COLLISION_PENALTY = -0.002f;

	private Vector3 startPos;

	private void Awake()
	{
		var root = GetComponentInParent<CharacterRoot>();
		agentReference = root.gameObject;

		var controller = GetComponentInParent<CharacterController>();
		agentBody = controller.gameObject;

		CollisionThrower.OnCharacterCollision += OnCollision;

		startPos = agentReference.transform.position;
	}
	
	public override void OnEpisodeBegin()
	{
		var spawn = spawnPoints.SelectRandomLocation().position;
		spawn.y = startPos.y; // preserve starting height
		
		target = AreaProps.SelectRandomProp().transform;
		agentBody.transform.position = spawn;
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//Position of target relative to character's position
		sensor.AddObservation(transform.InverseTransformDirection(target.position));
		sensor.AddObservation(transform.InverseTransformPoint(target.position));
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
}