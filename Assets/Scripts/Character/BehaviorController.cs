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
	private Agent currentActiveAgent;

	[SerializeField]
	[Header("Loaded Agents")]
	private List<Agent> Agents;

	private void Awake()
	{
		Agents = GetComponentsInChildren<Agent>().ToList();
		Academy.Instance.AgentPreStep += AcademyStepHandler;
	}

	public void StartBehavior(AgentBehaviorProperties behavior)
	{
		switch (behavior.Behavior)
		{
			case BehaviorType.None:
				StopBehavior();
				break;

			case BehaviorType.Navigate:
				currentActiveAgent = Agents.Find( (agent) => agent is NavigateAgent );
				break;

			case BehaviorType.PickUp:
				currentActiveAgent = Agents.Find((agent) => agent is PickUpAgent);
				break;

			case BehaviorType.Drop:
				currentActiveAgent = Agents.Find((agent) => agent is DropAgent);
				break;

			case BehaviorType.Activate:
				currentActiveAgent = Agents.Find((agent) => agent is ActivateAgent);
				break;

			default:
				Debug.Log($"Behavior type {behavior.Behavior} not recognized");
				break;
		}
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