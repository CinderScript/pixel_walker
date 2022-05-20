using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PromptLoader
{
	public static GptPrompts GetPromptsFromFile(string filePath)
	{
		var inputClassifier = "Prompt: Input Classifier";
		var questionClassifier = "Prompt: Question Classifier";
		var responseClassifier = "Prompt: Response Classifier";
		var BestMatchSelector = "Prompt: Response BestMatchSelector";

		GptPrompts prompts;
		// TODO: load json from file, deserialize json into Prompts object

		prompts = new GptPrompts(inputClassifier, questionClassifier, responseClassifier, BestMatchSelector);
		return prompts;
	}
}

public class GptPrompts
{
	public string InputClassifier { get; }
	public string QuestionResponder { get; }
	public string CommandParser { get; }
	public string BestMatchSelector { get; }

	public GptPrompts(string inputClassifier, string questionResponder, string commandParser, string bestMatchSelector)
	{
		InputClassifier = inputClassifier;
		QuestionResponder = questionResponder;
		CommandParser = commandParser;
		BestMatchSelector = bestMatchSelector;
	}
}