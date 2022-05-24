using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PromptLoader
{

    // This will read Prompts from file and deserialize them into C# objects
    public GptPrompts GetPromptsFromFile(string filePath)
    {
        // This will attempt to read from filepath
        StreamReader r = new StreamReader(filePath);
        string jsonString = r.ReadToEnd();

        // This creates a new instance of prompt to be deserialized, then output out
        GptPrompts prompts;
        prompts = JsonConvert.DeserializeObject<GptPrompts>(jsonString);
        GptPrompts output = new GptPrompts(prompts.InputClassifier, prompts.QuestionResponder, prompts.CommandParser, prompts.BestMatchSelector);
        return output;
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