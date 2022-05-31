/**
* Project: Pixel Walker
*
* Description: PromptLoader is a class that holds the json object
* read by the GetPromptsFromFile to access the prompts within the object 
* so the input handler can use them.
* 
* Author: Pixel Walker -
* Maynard, Gregory
* Shubhajeet, Baral
* Do, Khuong
* Nguyen, Thuong
*
* Date: 05-26-2022
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PromptLoader
{
    /// <summary>
    /// This will read Prompts from file and deserialize them into C# objects
    /// </summary>
    /// <param name="filePath">name of file  containing prompts</param>
    /// <returns></returns>
    public GptPrompts GetPromptsFromFile(string filePath)
    {
        // This will attempt to read from filepath
        StreamReader r = new StreamReader(filePath);
        string jsonString = r.ReadToEnd();

        // This will create a new instance of prompt to be deserialized
        GptPrompts prompts = new GptPrompts();
        prompts = JsonConvert.DeserializeObject<GptPrompts>(jsonString);

        // This will join the string array's elements into one single output string 
        prompts.InputClassifier = String.Join(" ", prompts.InputList);
        prompts.QuestionResponder = String.Join(" ", prompts.QuestionList);
        prompts.CommandParser = String.Join("", prompts.CommandList);
        prompts.BestMatchSelector = String.Join("", prompts.BestMatchList);
        prompts.ConversationResponder = String.Join("", prompts.ConversationList);
        prompts.PropsListLoader = String.Join(", ", prompts.PropsList);
        return prompts;
    }
}

/// <summary>
/// Class GptPrompts will have string array properties to get the array from
/// json object, string properties so the prompts can be accessed after going through
/// the String.Join process. 
/// </summary>
public class GptPrompts
{
    // initialize the string[] to hold the string object in arrays
    public string[] InputList { get; set; }
    public string[] QuestionList { get; set; }
    public string[] CommandList { get; set; }
    public string[] BestMatchList { get; set; }
    public string[] ConversationList {get; set;}
    public string[] PropsList {get; set;}

    // string object to output with the input handler framework
    public string InputClassifier { get; set; }
    public string QuestionResponder { get; set; }
    public string CommandParser { get; set; }
    public string BestMatchSelector { get; set; }
    public string ConversationResponder {get; set; }
    public string PropsListLoader {get; set; }
}
