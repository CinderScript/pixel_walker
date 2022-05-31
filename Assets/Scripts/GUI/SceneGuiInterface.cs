/**
 *	Project:		Pixel Walker
 *	
 *	Description:	SceneGuiInterface provides the connection between the
 *					GUI and the BehaviorController. This class exposes 
 *					Behaviorcontroller methods that begin the appropriate 
 *					behavior when given a AgentBehaviorProperties object.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using System.Collections;
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
		propReferences = sceneArea.GetComponent<AreaPropReferences>();
    }

	private void Start()
	{
		//StartNavigationTraining(false);
		//GuiUsageExample_DebugTest();
		//Debug.Log(propReferences.GetAllPropNames());
	}

	IEnumerator TriggerAfterSeconds_DebugTest(float sec)
	{
		yield return new WaitForSeconds(sec);
		var properties = new AgentBehaviorProperties(BehaviorType.TurnOn, "Drill Press", "");

		StartBehavior(properties);
	}
	
	private async void GuiUsageExample_DebugTest()
	{
		while (true)
		{
			var properties = new AgentBehaviorProperties(BehaviorType.Navigate, "yellow_claw_hammer");

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

		if (target)
		{
			return await behaviorController.StartBehavior(properties.Behavior, target.transform);
		}
		else
		{
			string msg = "I can't find an object with the name " + properties.Object;
			return new BehaviorResult(BehaviorType.None, false, false, msg);
		}

	}

	public void StartNavigationTraining(bool randomLocation)
	{
		// get each behavior controller and start them all on training.
		var controllers = FindObjectsOfType<BehaviorController>();
		foreach (var controller in controllers)
		{
			controller.TrainNavigation(randomLocation);
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