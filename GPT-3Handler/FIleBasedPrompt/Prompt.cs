using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PromptLoader
{

    //NOTE: FilePrompts not implemented yet,
    //thus using hard coded classifier variables for testing purposes - Shub
    public GptPrompts GetPromptsFromFile(string filePath)
    {

        StreamReader r = new StreamReader(filePath);
        string jsonString = r.ReadToEnd();

        GptPrompts prompts;
        // TODO: load json from file, deserialize json into Prompts object
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
public class Program
    {
        static void Main()
        {
            PromptLoader example = new PromptLoader();
            GptPrompts prompts = new GptPrompts("","","","");
            prompts = example.GetPromptsFromFile("C:/Users/Tn/Documents/ReadPrompts/Prompt.json");
            Console.WriteLine($"{prompts.InputClassifier}");
        }
    }
