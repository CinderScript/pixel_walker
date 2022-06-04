/**
 *	Project:		Pixel Walker
 *	
 *	Description:	BehaviorController provides syncronizing control
 *					of the different Agent scripts attached to the 
 *					in game character.  This has a direct connection 
 *					with the SceneGuiInterface script to initiate 
 *					behaviors.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Unity.MLAgents;

using UnityEngine;

public class BehaviorController : MonoBehaviour
{
	public bool IsTraining { get; private set; }
	
	[Header("Current Status")]
	[SerializeField]
	private AgentBase currentActiveAgent;

	[Header("Loaded Agents")]
	[SerializeField]
	private List<AgentBase> agents;
	
	[Header("Area References")]
	[SerializeField]
	private AreaPropReferences areaProps;
	[SerializeField]
	private SpawnPointReferences spawnPoints;
	[SerializeField]
	private GameObject agentBody;

	private float startingPositionHeight = 0;
	private int decisionPeriod = 5;

	
	private void Awake()
	{
		Academy.Instance.AutomaticSteppingEnabled = false;
		Academy.Instance.AgentPreStep += AcademyStepHandler;

		//Setup communication with agents
		agents = GetComponentsInChildren<AgentBase>().ToList();
		foreach (var agent in agents)
		{
			//agent.OnBehaviorFinished += OnBehaviorFinishHandler;
		}

		var playerArea = GetComponentInParent<AgentArea>().transform;
		spawnPoints = playerArea.GetComponentInParent<SpawnPointReferences>();
		areaProps = playerArea.GetComponentInParent<AreaPropReferences>();

		// GET AGENT'S CHARACTER CONTROLLER	(for spawning)	
		agentBody = GetComponentInParent<CharacterController>().gameObject;
		startingPositionHeight = agentBody.transform.position.y;
	}

	public async Task<BehaviorResult> StartBehavior(AgentBehaviorProperties properties)
	{
		// is the target a location or an object?
		GameObject targetObject = null;
		if (properties.Object.ToLower() == "null")
		{
			targetObject = areaProps.GetRoom(properties.Location);

			if (!targetObject)
			{
				string msg = "I can't find the location " + properties.Location;
				return new BehaviorResult(BehaviorType.None, false, false, msg);
			}
		}
		else
		{
			targetObject = areaProps.GetProp(properties.Object);

			if (!targetObject)
			{
				string msg = "I can't find an object with the name " + properties.Object;
				return new BehaviorResult(BehaviorType.None, false, false, msg);
			}
		}

		var target = targetObject.transform;
		var behavior = properties.Behavior;

		switch (behavior)
		{
			case BehaviorType.Navigate:
				{


					return await Navigate(target);
				}
				
			case BehaviorType.TurnOn:
				{
					var result = await Navigate(target);
					if (result.Success)
					{
						return await Activate(target, behavior);
					}
					else
					{
						return result;
					}
				}
			case BehaviorType.TurnOff:
				{
					var result = await Navigate(target);
					if (result.Success)
					{
						return await Activate(target, behavior);
					}
					else
					{
						return result;
					}
				}
			case BehaviorType.PickUp:
				{
					var msg = "I don't know how to pick up objects yet...";
					return new BehaviorResult(BehaviorType.PickUp, false, false, msg);
				}
			case BehaviorType.SetDown:
				{
					var msg = "I don't know how to set down objects yet...";
					return new BehaviorResult(BehaviorType.SetDown, false, false, msg);
				}

			case BehaviorType.Drop:
				{
					var msg = "I don't know how to drop objects yet...";
					return new BehaviorResult(BehaviorType.SetDown, false, false, msg);
				}
			case BehaviorType.Open:
				{
					var msg = "I can't open things yet...";
					return new BehaviorResult(BehaviorType.SetDown, false, false, msg);
				}
			//return await Drop();

			default:
				throw new System.Exception("Behavior not implemented");
		}
	}

	private async Task<BehaviorResult> Navigate(Transform target)
	{
		currentActiveAgent = agents.Find((agent) => agent is NavigateAgent);

		return await currentActiveAgent?.PerformeBehavior(target);
	}
	private async Task<BehaviorResult> Activate(Transform target, BehaviorType behavior)
	{
		currentActiveAgent = agents.Find((agent) => agent is ActivateAgent);
		var activateAgent = currentActiveAgent as ActivateAgent;
		return await activateAgent?.PerformeBehavior(target, behavior);
	}
	private async Task<BehaviorResult> PickUp(Transform target)
	{
		currentActiveAgent = agents.Find((agent) => agent is PickUpAgent);
		return await currentActiveAgent?.PerformeBehavior(target);
	}
	private async Task<BehaviorResult> Drop()
	{
		currentActiveAgent = agents.Find((agent) => agent is DropAgent);
		return await currentActiveAgent?.PerformeBehavior();
	}

	public void CancelCurrentBehavior()
	{
		CancelBehavior(currentActiveAgent);
	}
	void CancelBehavior(AgentBase agent)
	{
		agent?.CancelBehavior();
		// stop training in case this is a training session
		IsTraining = false;
	}

	void AcademyStepHandler(int academyStepCount)
	{
		// is it time for Agent to make a descision?
		if (academyStepCount % decisionPeriod == 0)
		{
			if (currentActiveAgent)
			{
				currentActiveAgent.RequestDecision();
			}
		}

		// Act on last decision if there is an active agent
		//currentActiveAgent?.RequestAction(); 

		// Note: input values to controller don't change during inference, so
		// the character conroller performs the same action in between
		// action requests. When using Heuristics, the keystrokes are used
		// to define controller input, so actions are not taken when no keys
		// are pressed.
		// During Inference, the agent actions are stopped in StopBehavior()
		// by clearing all of the CharacterController's input values.
	}

	void OnDestroy()
	{	
		// if the agent is removed AcademyStepHandler cannot be called
		if (Academy.IsInitialized)
		{
			Academy.Instance.AgentPreStep -= AcademyStepHandler;
		}
	}

	/* * * * * * * FOR TRAINING * * * * * * * */
	public async void TrainNavigation(bool randomizeLocation)
	{
		IsTraining = true;
		while (IsTraining)
		{
			if (randomizeLocation)
			{
				// half the time pick a random location, half the time use last location
				var random = Random.Range(0, 2);
				if (random == 1)
				{
					SpawnInRandomLocation();
				}
			}

			var randomProp = areaProps.SelectRandomProp().transform;

			await Navigate(randomProp);
		}
	}

	public void SpawnInRandomLocation()
	{
		// Randomly place the agent at one of the spawn locations
		var spawnPos = spawnPoints.SelectRandomLocation().transform.position;
		spawnPos.y = startingPositionHeight; // preserve starting height

		// disable the CC so it won't override the relocation
		var controller = agentBody.GetComponent<CharacterController>();
		controller.enabled = false;
		agentBody.transform.position = spawnPos;
		controller.enabled = true;
	}
}