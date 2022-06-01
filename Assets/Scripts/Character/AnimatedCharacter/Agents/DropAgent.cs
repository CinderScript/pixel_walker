/**
 *	Project:		Pixel Walker
 *	
 *	Description:	DropAgent is an ML-Agents agent that is currently not 
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

public class DropAgent : AgentBase
{
	public override BehaviorType MyBehaviorType => BehaviorType.Drop;
	
	protected override void initializeBehavior()
	{
		throw new System.NotImplementedException();
	}
}