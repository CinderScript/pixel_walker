using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class UserInputHandler
{
	private Gpt3Connection connection;
	private GptPrompts prompts;
	private int Test;

	public UserInputHandler(string apiKey, string gptPromptsFilePath)
	{
		connection = new Gpt3Connection(apiKey);
	}

	public GptResponse? GetGptResponce(string userInput, out Exception exception)
	{
		InputType inputType = classifyText(userInput);
		string generatedText = "Error: No Responce Saved";
		
		AgentBehaviorProperties? agentBehaviorProperties = null;
		Exception responseException = null;

		switch (inputType)
		{
			case InputType.Unknown:
				// TODO: send back responce or check again
				break;
				
			case InputType.Question:
				generatedText = answerQuestion(userInput);
				break;
				
			case InputType.Command:
				AgentBehaviorProperties? behavior = parseCommand(userInput);
				// error checking - if this is null (couldn't parse) then send exception
				if (!behavior.HasValue)
				{
					exception = new Exception("Couldn't parse command");
					return null;
				}
				break;
		}

		// TODO: GET BEST MATCH TO ITEM IN SCENE IF COMMAND
		string objectBestMatch = 

		GptResponse gptResponce = new GptResponse(
			inputType, 
			generatedText, 
			agentBehaviorProperties, 
			responseException);
		
		return gptResponce;
	}

	private InputType classifyText(string userInput)
	{
		string promptStart = prompts.InputClassifier;

		// TODO: COMPLETE THE PROMPT WITH USER INPUT
		string fullPrompt = "TODO: COMPLETE THE PROMPT WITH USER INPUT";

		string responce = connection.GenerateText(fullPrompt);

		// TODO: CODE TO SELECT CORRECT INPUT CLASSIFICATION
		InputType type = InputType.Unknown; // todo

		return type;
	}

	private string answerQuestion(string userInput)
	{
		string promptStart = prompts.QuestionResponder;

		// TODO: COMPLETE THE PROMPT WITH USER INPUT
		string fullPrompt = "TODO: COMPLETE THE PROMPT WITH USER INPUT";

		string responce = connection.GenerateText(fullPrompt);

		string answer = responce; // TODO? IS THAT ALL?

		return answer;
	}

	private AgentBehaviorProperties? parseCommand(string userInput)
	{
		string promptStart = prompts.CommandParser;

		// TODO: COMPLETE THE PROMPT WITH USER INPUT
		string fullPrompt = "TODO: COMPLETE THE PROMPT WITH USER INPUT";

		string responce = connection.GenerateText(fullPrompt);

		// TODO: use jsonFormattedText to create object.  need to deserialize responce
		bool success = true; // TODO: check if success
		string behavior = "ToDo: get behavior";
		string sceneObject = "ToDo: get object";
		string location = "ToDo: get location";

		// IF THERE IS AN ERROR AND WE CAN'T SERIALIZE, TRY AGAIN?, RETURN NULL IF FAILED
		if (!success)
		{
			return null;
		}

		AgentBehaviorProperties parse = new AgentBehaviorProperties(behavior, sceneObject, location);

		return parse;
	}

	private string getObjectBestMatch(string sceneObject)
	{
		string promptStart = prompts.BestMatchSelector;

		// TODO: COMPLETE THE PROMPT WITH USER INPUT
		string fullPrompt = "TODO: COMPLETE THE PROMPT WITH USER INPUT";

		string responce = connection.GenerateText(fullPrompt);

		// TODO: if the responce is not a valid object, try 3 more times before returning ""
		return "NOT IMPLEMENTED";
	}
}