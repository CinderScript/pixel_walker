using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

public class SceneGuiInterface : MonoBehaviour
{
    [Header("Area of BehaviorController")]
    [Tooltip("This camera controller will use this PlayerArea to find the player to follow.")]
    public GameObject sceneArea;

	private BehaviorController behaviorController;
	private AreaPropReferences propReferences;

	void Awake()
    {
        behaviorController = sceneArea.GetComponentInChildren<BehaviorController>();
    }

	private void Start()
	{
		propReferences = sceneArea.GetComponent<AreaPropReferences>();

		StartNavigationTraining();
		//TestNavigation()
	}

	public async void TestNavigation()
	{
		var properties = new AgentBehaviorProperties(BehaviorType.Navigate, "Workshop Light Switch", "");
		await SelectBehavior(properties);
		Debug.Log("Done Navigating!");
	}

	public async Task SelectBehavior(AgentBehaviorProperties properties)
	{
		var target = propReferences.GetProp(properties.Object).transform;
		var propName = target.GetComponent<PropInfo>().Name;

		if (target)
		{
			await behaviorController.StartBehavior(properties.Behavior, target);
		}
		else
		{
			throw new Exception( $"{propName} not found!" );
		}

	}

	public void StartNavigationTraining()
	{
		// get each behavior controller and start them all on training.
		var controllers = FindObjectsOfType<BehaviorController>();
		foreach (var controller in controllers)
		{
			controller.TrainNavigation();
		}
	}

	public void StopBehavior()
	{
		behaviorController.StopCurrentBehavior();
	}

	public string GetPropsList()
	{
		return propReferences.GetAllPropNames();
	}
}