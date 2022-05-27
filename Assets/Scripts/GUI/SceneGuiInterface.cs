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

		//StartNavigationTraining();
		Test();

		//StartCoroutine(TriggerAfterSeconds(4));
	}

	IEnumerator TriggerAfterSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		CancelBehavior();
	}

	public async void Test()
	{
		var properties = new AgentBehaviorProperties(BehaviorType.PickUp, "Workshop Light Switch", "");
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
			Debug.Log($"{result.Behavior} finished, but without success.");
	}

	public async Task<BehaviorResult> StartBehavior(AgentBehaviorProperties properties)
	{
		var target = propReferences.GetProp(properties.Object).transform;
		var propName = target.GetComponent<PropInfo>().Name;

		if (target)
		{
			return await behaviorController.StartBehavior(properties.Behavior, target);
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

	public void CancelBehavior()
	{
		behaviorController.CancelCurrentBehavior();
	}

	public string GetPropsList()
	{
		return propReferences.GetAllPropNames();
	}
}