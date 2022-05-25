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

        // This will create a new instance of prompt to be deserialized
        GptPrompts prompts = new GptPrompts();
        prompts = JsonConvert.DeserializeObject<GptPrompts>(jsonString);

        // This will join the string array's elements into one single output string 
        prompts.InputClassifier = String.Join("", prompts.InputList);
        prompts.QuestionResponder = String.Join("", prompts.QuestionList);
        prompts.CommandParser = String.Join("", prompts.CommandList);
        prompts.BestMatchSelector = String.Join("", prompts.BestMatchList);
        return prompts;
    }
}

public class GptPrompts
{
    // initialize the string[] to hold the string object in arrays
    public string[] InputList { get; set; }
    public string[] QuestionList { get; set; }
    public string[] CommandList { get; set; }
    public string[] BestMatchList { get; set; }

    // string object to output with the input handler framework
    public string InputClassifier { get; set; }
    public string QuestionResponder { get; set; }
    public string CommandParser { get; set; }
    public string BestMatchSelector { get; set; }
}

// A testing class with a pre-defined file path to validate the prompt loading
public class PromptLoaderTestDriver
{
    static void Main()
    {
        PromptLoader example = new PromptLoader();
        GptPrompts prompts = example.GetPromptsFromFile("C:/development/pixel_walker/GPT-3Handler/FIleBasedPrompt/Prompt.json");

        System.Diagnostics.Debug.WriteLine(prompts.InputClassifier);
    }
}
