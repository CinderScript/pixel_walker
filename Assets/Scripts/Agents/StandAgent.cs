using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using UnityEngine;

public class StandAgent : Agent
{
	public GameObject CharacterController;

	private CharacterController cc;

	void Awake()
	{
		cc = CharacterController.GetComponent<CharacterController>();
	}

	public override void Initialize()
	{
		
	}

	public override void OnEpisodeBegin()
	{
		Debug.Log("New Episode started");
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		var observations = cc.GetLimbObservations();
		for (int i = 0; i < cc.NumberOfObservations; i++)
		{
			sensor.AddObservation(observations[i]);
		}
	}

	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		cc.ProcessActionBuffers(actionBuffers);

		if (cc.PositionReferencePoint.transform.position.y > 0.4f)
		{
			AddReward(0.01f);
		}

		if (cc.PositionReferencePoint.transform.position.y < 0.3f)
		{
			EndEpisode();
		}

		if (GetCumulativeReward() > 0.5)
		{
			Debug.Log("rewarded");
		}

		Debug.Log($"torque used: {cc.TotalTorqueUsed}");
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
	}
}
