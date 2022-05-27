using System.Collections;

using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;

public class PickUpAgent : AgentBase
{
	public override BehaviorType MyBehaviorType => BehaviorType.PickUp;

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