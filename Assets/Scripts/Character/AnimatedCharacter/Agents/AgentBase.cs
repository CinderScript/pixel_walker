using System;
using System.Collections;
using System.Threading.Tasks;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

[DisallowMultipleComponent]
public abstract class AgentBase : Agent
{
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
	private bool isBehaviorFinished = true;

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
	}

	public async Task PerformeBehavior(Transform target = null)
	{
		isBehaviorFinished = false;
		this.target = target;
		initializeBehavior();
		await BehaviorFinishAwaiter();
		StopBehavior();
	}
	protected abstract void initializeBehavior();

	public void StopBehavior()
	{
		isBehaviorFinished = true;
		movementValues.ClearValues();
	}

	IEnumerator BehaviorFinishAwaiter()
	{
		while (!isBehaviorFinished)
		{
			yield return new WaitForFixedUpdate();
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