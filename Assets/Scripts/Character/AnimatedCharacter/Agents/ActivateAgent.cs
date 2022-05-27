using System;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

public class ActivateAgent : AgentBase
{
	public override BehaviorType MyBehaviorType => BehaviorType.Activate;

	protected override void initializeBehavior()
	{
	}
	public override void CollectObservations(VectorSensor sensor)
	{
	}
	public override void OnActionReceived(ActionBuffers actions)
	{
		AssignRewards();
	}

	public void AssignRewards()
	{

	}
}