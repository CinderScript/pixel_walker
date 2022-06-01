/**
 *	Project:		Pixel Walker
 *	
 *	Description:	PickUpAgent is an ML-Agents agent that is currently not 
 *					implemented but is attached to the in game character 
 *					and can be controlled by the Behavior Controller.
 *					
 *	Author:			Pixel Walker -
 *						Maynard, Gregory
 *						Shubhajeet, Baral
 *						Do, Khuong
 *						Nguyen, Thuong						
 *					
 *	Date:			05-30-2022
 */

using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

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