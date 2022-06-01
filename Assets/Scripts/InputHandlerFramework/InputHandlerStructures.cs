/**
* Project: Pixel Walker
*
* Description: This class contains all
* structures used to store information for 
* Dave and the UI.
* 
* Author: Pixel Walker -
* Maynard, Gregory
* Shubhajeet, Baral
* Do, Khuong
* Nguyen, Thuong
*
* Date: 05-26-2022
*/


/// <summary>
/// Provides a description of a requested behavior and the information 
/// that the beahavior controller needs to select the correct agent 
/// and give the correct target.
/// </summary>
public class AgentBehaviorProperties
{
	public BehaviorType Behavior { get; }
	public string Object { get; set; }
	public string Location { get; }
	public AgentBehaviorProperties(BehaviorType behavior, string sceneObject, string location = null)
	{
		Behavior = behavior;
		Object = sceneObject;
		Location = location;
	}
}

/// <summary>
/// 
/// </summary>
public enum InputType
{
	Unknown, Question, Command, Conversation
}

public enum BehaviorType
{
	Unknown, None, Navigate, PickUp, Drop, TurnOn, TurnOff, SetDown, Open, Activate
}

public enum EngineType
{
	Davinci, Curie, Babbage, Ada
}

public class GptResponse
{
	public InputType Type { get; }
	public string GeneratedText { get; }
	public AgentBehaviorProperties BehaviorProperties { get; }
	public GptResponse(InputType type, string generatedText, AgentBehaviorProperties behaviorProperties)
	{
		Type = type;
		GeneratedText = generatedText;
		BehaviorProperties = behaviorProperties;
	}
}
