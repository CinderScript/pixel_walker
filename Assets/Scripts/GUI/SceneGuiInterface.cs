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

	[Header("Training Objects")]
	public Transform activateObject;

	void Awake()
    {
        behaviorController = sceneArea.GetComponentInChildren<BehaviorController>();
    }

	private void Start()
	{
		propReferences = sceneArea.GetComponent<AreaPropReferences>();

		//StartNavigationTraining();
		//StartActivateTraining();
		
		Test();

		//StartCoroutine(TriggerAfterSeconds(5));
	}

	IEnumerator TriggerAfterSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		var properties = new AgentBehaviorProperties(BehaviorType.Activate, "Drill Press", "");

		StartBehavior(properties);
	}

	public async void Test()
	{
		while (true)
		{
			var properties = new AgentBehaviorProperties(BehaviorType.Activate, "workshop light switch", "");

			var result = await StartBehavior(properties);
			if (result.Cancelled)
			{
				Debug.Log($"Behavior was cancelled while performing {result.Behavior}.");
			}
			else if (result.Success)
			{
				Debug.Log($"{result.Behavior} successfully finished!");
			}
			else
				Debug.Log($"I couldn't perform the requested action. {result.Message}");

			await Task.Delay(2000);
		}
	}

	public async Task<BehaviorResult> StartBehavior(AgentBehaviorProperties properties)
	{
		var target = propReferences.GetProp(properties.Object);
		var propName = target.GetComponent<PropInfo>().Name;

		if (target)
		{
			return await behaviorController.StartBehavior(properties.Behavior, target.transform);
		}
		else
		{
			string msg = "I can't find an object with the name " + propName;
			return new BehaviorResult(BehaviorType.None, false, false, msg);
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

	public void StartActivateTraining()
	{
		// get each behavior controller and start them all on training.
		var controllers = FindObjectsOfType<BehaviorController>();
		foreach (var controller in controllers)
		{
			controller.TrainActivate(activateObject);
		}
	}

	public void CancelBehavior()
	{
		behaviorController.CancelCurrentBehavior();
	}

	public string GetPropsList()
	{
		return propReferences.GetAllPropNames();
	}
}