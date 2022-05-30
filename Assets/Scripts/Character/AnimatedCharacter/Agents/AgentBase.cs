using System.Collections;
using System.Threading.Tasks;

using Unity.MLAgents;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class AgentBase : Agent
{
	public abstract BehaviorType MyBehaviorType { get; }

	public bool RequestAcademyStep { get; private set; }

	[Header("Assigned at RTime")]
	public Transform agentBody;
	[SerializeField]
	protected Transform playerArea;
	[SerializeField]
	protected Transform target;
	[SerializeField]
	protected UserInputValues userInputValues;
	[SerializeField]
	protected CharacterMovementValues movementValues;

	// signlals BehaviorFinishAwaiter to return
	private bool isFinished = false;
	private bool isCancelled = false;
	private bool isSuccess = false;

	private string stopBehaviorMessage;

	protected virtual void Awake()
	{
		// GET USER INPUT SCRIPT
		UserInputValues[] userInputScripts = FindObjectsOfType(typeof(UserInputValues)) as UserInputValues[];
		if (userInputScripts.Length > 1)
		{
			Debug.LogError($"{userInputScripts.Length} were found. Only one is allowed.");
		}
		else if (userInputScripts.Length == 0)
		{
			Debug.LogError("No UserInputValues script was found. Please add one.");
		}
		else
		{
			userInputValues = userInputScripts[0];
		}

		// GET AGENT'S CHARACTER CONTROLLER	(for observations)	
		agentBody = GetComponentInParent<CharacterController>().transform;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<AgentArea>().transform;
		movementValues = playerArea.GetComponentInChildren<CharacterMovementValues>();

		RequestAcademyStep = false;
	}

	public async Task<BehaviorResult> PerformeBehavior(Transform target = null)
	{
		isFinished = false;
		isCancelled = false;
		isSuccess = false;

		this.target = target;
		initializeBehavior();

		RequestAcademyStep = true;
		await BehaviorFinishAwaiter();
		RequestAcademyStep = false;

		return new BehaviorResult(
			MyBehaviorType, isSuccess, 
			isCancelled, stopBehaviorMessage);
	}
	protected abstract void initializeBehavior();

	public void CancelBehavior()
	{
		isCancelled = true;
		StopBehavior(false);  // sets isSuccess true/false
	}

	protected void StopBehavior(bool success, string msg = null)
	{
		isFinished = true;
		isSuccess = success;
		movementValues.ClearValues();
		stopBehaviorMessage = msg;
		EndEpisode();
	}
	
	IEnumerator BehaviorFinishAwaiter()
	{
		while (!isFinished)
		{
			yield return new WaitForFixedUpdate();
		}
	}

	protected IEnumerator RotateToDir(Vector3 direction, float epsilon)
	{
		bool finished = false;

		while (!finished)
		{
			var signedAngleToTarget 
				= Vector3.SignedAngle(
					agentBody.forward,
					direction,
					agentBody.up);

			if (signedAngleToTarget < epsilon && signedAngleToTarget > -epsilon)
			{
				movementValues.ClearValues(); // otherwise will use last input action
				finished = true;
			}
			else
			{
				// is target to the right or left?
				var isRight = signedAngleToTarget > 0;
				// is target in front or behind?
				var isFront = signedAngleToTarget < 90 && signedAngleToTarget > -90;

				if (isRight && isFront)
				{
					// turn right
					movementValues.bodyRotation = 1;
				}
				else if (!isRight && isFront)
				{
					// turn left
					movementValues.bodyRotation = 2;
				}
				else if (isRight && !isFront)
				{
					// turn left
					movementValues.bodyRotation = 2;
				}
				else if (!isRight && !isFront)
				{
					// turn right
					movementValues.bodyRotation = 1;
				}

				yield return new WaitForFixedUpdate();
			}
		}
	}
	protected async Task RotateTowardsPos(Vector3 position, float epsilon)
	{
		// direction the agent should be facing
		var targetPos = new Vector3(position.x, 0, position.z);
		var agentPos = new Vector3(agentBody.position.x, 0, agentBody.position.z);
		Vector3 dirFromAgentToTarget = targetPos - agentPos;

		await RotateToDir(dirFromAgentToTarget, epsilon);
	}
}

public class BehaviorResult
{
	public BehaviorType Behavior { get; }
	public bool Success { get; }
	public bool Cancelled { get; }
	public string Message { get; set; }
	public BehaviorResult(BehaviorType behavior, bool success, bool cancelled, string msg = null)
	{
		Behavior = behavior;
		Success = success;
		Cancelled = cancelled;
		Message = msg;
	}
}