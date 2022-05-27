using Unity.MLAgents;

using UnityEngine;

public class ActivateAgent : AgentBase
{
	public override BehaviorType MyBehaviorType => BehaviorType.Activate;
	
	protected override void initializeBehavior()
	{
		throw new System.NotImplementedException();
	}
}