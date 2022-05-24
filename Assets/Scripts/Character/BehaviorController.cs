using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;

using UnityEngine;

public class BehaviorController : MonoBehaviour
{
	[Header("Behaviors Location")]
	public GameObject behaviorRoot;

	[Header("Behaviors")]
	public List<GameObject> Behaviors;

	public void InitiateBehavior(AgentBehaviorProperties behavior)
	{
		switch (behavior.Type)
		{
			case BehaviorType.None:
				StopBehaviors();
				break;
			case BehaviorType.Navigate:

				break;
			case BehaviorType.PickUp:
				break;
			case BehaviorType.Drop:
				break;
			case BehaviorType.Activate:
				break;
			default:
				Debug.Log($"Behavior type {behavior.Type} not recognized");
				break;
		}
	}

	public void StopBehaviors()
	{
		foreach (var behavior in Behaviors)
		{
			behavior.SetActive(false);
		}
	}

	private void Navigate()
	{
		StopBehaviors();
		var navigateAgent = GetComponentInChildren<NavigateAgent>();
		navigateAgent.gameObject.SetActive(true);
	}
	private void PickUp()
	{
		StopBehaviors();
		var navigateAgent = GetComponentInChildren<NavigateAgent>();
		navigateAgent.gameObject.SetActive(true);
	}
}

public class AgentBehaviorProperties
{
	public BehaviorType Type { get; }
	public string Object { get; set; }
	public string Location { get; }

	public AgentBehaviorProperties(BehaviorType behavior, string sceneObject, string location)
	{
		Type = behavior;
		Object = sceneObject;
		Location = location;
	}
}
public enum BehaviorType
{
	None, Navigate, PickUp, Drop, Activate, SetDown, Open
}