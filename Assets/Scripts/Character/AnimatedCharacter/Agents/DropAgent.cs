﻿
using System.Threading.Tasks;

public class DropAgent : AgentBase
{
	public override BehaviorType MyBehaviorType => BehaviorType.Drop;
	
	protected override void initializeBehavior()
	{
		throw new System.NotImplementedException();
	}
}