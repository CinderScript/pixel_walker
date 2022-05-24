public class AgentBehaviorProperties
{
	public BehaviorType Behavior { get; }
	public string Object { get; set; }
	public string Location { get; }
	
	public AgentBehaviorProperties(BehaviorType behavior, string sceneObject, string location)
	{
		Behavior = behavior;
		Object = sceneObject;
		Location = location;
	}
}

public enum InputType
{
	Unknown, Question, Command
}

public enum BehaviorType
{
	None, Navigate, PickUp, Drop, Activate, SetDown, Open
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