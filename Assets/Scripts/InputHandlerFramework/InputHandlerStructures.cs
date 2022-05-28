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
	Unknown, Question, Command, Conversation
}

public enum BehaviorType
{
	Unknown, None, Navigate, PickUp, Drop, Activate, SetDown, Open
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
