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
	private CharacterPose characterPose;

	private float remainStandingreward = 0.01f;

	void Awake()
	{
		cc = CharacterController.GetComponent<CharacterController>();
	}

	public override void OnEpisodeBegin()
	{
		cc.ResetPose();
		remainStandingreward = 0.01f;
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

		if (cc.PositionReferencePoint.transform.position.y > 0.7f)
		{
			AddReward(remainStandingreward);
			remainStandingreward += 0.005f;
		}
		else if (cc.PositionReferencePoint.transform.position.y < 0.5f)
		{
			AddReward(-0.1f);
		}

		if (cc.PositionReferencePoint.transform.position.y < 0.4f)
		{
			EndEpisode();
		}

		if (remainStandingreward > 0.99999999f)
		{
			SetReward(1);
			EndEpisode();
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
	}
}
