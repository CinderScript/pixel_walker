using System.Collections.Generic;
using System.Linq;

using Unity.MLAgents;

using UnityEngine;

public class BehaviorController : MonoBehaviour
{
	[Header("Decision Requests")]
	[Range(1, 10)]
	[SerializeField]
	private int decisionPeriod = 5;

	[SerializeField]
	private AgentBase currentActiveAgent;

	[SerializeField]
	[Header("Loaded Agents")]
	private List<AgentBase> agents;

	private void Awake()
	{
		Academy.Instance.AgentPreStep += AcademyStepHandler;
		agents = GetComponentsInChildren<AgentBase>().ToList();
		foreach (var agent in agents)
		{
			agent.OnBehaviorFinished += OnBehaviorFinishHandler;
		}
	}

	public AgentBase StartBehavior(AgentBehaviorProperties behavior)
	{
		switch (behavior.Behavior)
		{
			case BehaviorType.None:
				StopBehavior();
				break;

			case BehaviorType.Navigate:
				currentActiveAgent = agents.Find( (agent) => agent is NavigateAgent );
				break;

			case BehaviorType.PickUp:
				currentActiveAgent = agents.Find((agent) => agent is PickUpAgent);
				break;

			case BehaviorType.Drop:
				currentActiveAgent = agents.Find((agent) => agent is DropAgent);
				break;

			case BehaviorType.Activate:
				currentActiveAgent = agents.Find((agent) => agent is ActivateAgent);
				break;

			default:
				Debug.Log($"Behavior type {behavior.Behavior} not recognized");
				break;
		}

		return currentActiveAgent;
	}

	public void OnBehaviorFinishHandler(AgentBase agent)
	{
		currentActiveAgent = null;
	}

	public void StopBehavior()
	{
		currentActiveAgent = null;
	}

	void AcademyStepHandler(int academyStepCount)
	{
		// is it time for Agent to make a descision?
		if (academyStepCount % decisionPeriod == 0)
		{
			currentActiveAgent?.RequestDecision();
		}

		// Act on last decision if there is an active agent
		currentActiveAgent?.RequestAction();
	}

	void OnDestroy()
	{	
		// if the agent is removed AcademyStepHandler cannot be called
		if (Academy.IsInitialized)
		{
			Academy.Instance.AgentPreStep -= AcademyStepHandler;
		}
	}
}