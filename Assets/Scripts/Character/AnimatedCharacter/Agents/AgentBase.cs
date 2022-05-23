using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

[DisallowMultipleComponent]
public class AgentBase : Agent
{
	[Header("Agent")]
	public GameObject agentBody;

	[Header("Assigned at RTime")]
	public Transform playerArea;
	public Transform target;
	public UserInputValues userInputValues;
	public CharacterMovementInput movementValues;

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
			Debug.LogError("No UserInputValues scripts was found. Please add one.");
		}
		else
		{
			userInputValues = userInputScripts[0];
		}


		// GET AGENT'S CHARACTER CONTROLLER	(for observations)	
		var controller = GetComponentInParent<CharacterController>();
		agentBody = controller.gameObject;

		// GET SCENE REFERENCES - spawn points, props, controller movement value input location
		playerArea = GetComponentInParent<AgentArea>().transform;
		movementValues = playerArea.GetComponentInChildren<CharacterMovementInput>();
	}
}