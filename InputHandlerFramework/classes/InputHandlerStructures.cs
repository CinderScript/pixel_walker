public class AgentBehaviorProperties
{
	public string Behavior { get; }
	public string Object { get; set; }
	public string Location { get; }
	
	public AgentBehaviorProperties(string behavior, string sceneObject, string location)
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
	Unknown, Navigate, PickUp, Drop, SetDown, Open
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