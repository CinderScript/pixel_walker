using System.Collections;
using System.Threading.Tasks;

using Unity.MLAgents;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class AgentBase : Agent
{
	public abstract BehaviorType MyBehaviorType { get; }

	public bool RequestAcademyStep { get; private set; }

	[Header("Agent")]
	public GameObject agentBody;

	[Header("Assigned at RTime")]
	[SerializeField]
	protected Transform playerArea;
	[SerializeField]
	protected Transform target;
	[SerializeField]
	protected UserInputValues userInputValues;
	[SerializeField]
	protected CharacterMovementInput movementValues;

	// signlals BehaviorFinishAwaiter to return
	private bool isFinished = false;
	private bool isCancelled = false;
	private bool isSuccess = false;

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
		agentBody = GetComponentInParent<CharacterController>().gameObject;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<AgentArea>().transform;
		movementValues = playerArea.GetComponentInChildren<CharacterMovementInput>();

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

		return new BehaviorResult(MyBehaviorType, isSuccess, isCancelled);
	}
	protected abstract void initializeBehavior();

	public void CancelBehavior()
	{
		isCancelled = true;
		StopBehavior(false);  // sets isSuccess true/false
	}

	protected void StopBehavior(bool success)
	{
		isFinished = true;
		isSuccess = success;
		movementValues.ClearValues();
		EndEpisode();
	}

	IEnumerator BehaviorFinishAwaiter()
	{
		while (!isFinished)
		{
			yield return new WaitForFixedUpdate();
		}
	}
}

public class BehaviorResult
{
	public BehaviorType Behavior { get; }
	public bool Success { get; }
	public bool Cancelled { get; }
	public BehaviorResult(BehaviorType behavior, bool success, bool cancelled)
	{
		Behavior = behavior;
		Success = success;
		Cancelled = cancelled;
	}
}