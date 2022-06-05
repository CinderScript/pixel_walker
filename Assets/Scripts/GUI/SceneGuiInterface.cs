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
		//StartCoroutine(StartLater(BehaviorTest));
		//Debug.Log(propReferences.GetAllPropNames());
	}

	IEnumerator StartLater(System.Action action)
	{
		yield return new WaitForSeconds(1);
		action();
	}

	async void BehaviorTest()
	{
		AgentBehaviorProperties properties =
			new AgentBehaviorProperties(BehaviorType.TurnOff, "Yellow Claw Hammer", "Music Room");
		var start = StartBehavior(properties);

		Debug.Log("Starting Message: " + start.message);

		var result = await start.result;
		Debug.Log("result msg: " + result.Message);
		Debug.Log("success: " + result.Success);
	}

	public (string message, Task<BehaviorResult> result) StartBehavior(AgentBehaviorProperties properties)
	{
		return behaviorController.StartBehavior(properties);
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

	public void CancelBehavior()
	{
		behaviorController.CancelCurrentBehavior();
	}

	public string GetPropsList()
	{
		return propReferences.GetAllPropNames();
	}
}