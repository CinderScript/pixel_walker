using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

public class UserInputHandler
{
    private Gpt3Connection connection;
    private GptPrompts prompts;
    private int Test;

    public UserInputHandler(string apiKey, string gptPromptsFilePath)
    {
        connection = new Gpt3Connection(apiKey);
        prompts = PromptLoader.GetPromptsFromFile(gptPromptsFilePath);
    }

    public GptResponse GetGptResponce(string userInput, out Exception exception)
    {
        InputType inputType = classifyText(userInput);
        string generatedText = "Error: No Responce Saved";

        AgentBehaviorProperties agentBehaviorProperties = null;

        switch (inputType)
        {
            case InputType.Unknown:
                // TODO: send back responce or check again
                exception = new Exception("The user's input could not be classified.");
                return null;

            case InputType.Question:
                generatedText = answerQuestion(userInput);
                break;

            case InputType.Command:
                agentBehaviorProperties = parseCommand(userInput);

                // error checking - if this is null (couldn't parse) then send exception
                if (agentBehaviorProperties == null)
                {
                    exception = new Exception("Couldn't parse command");
                    return null;
                }
                else
                {   // COMMAND WAS PARSED
                    string nameOfUserRequestedObject = agentBehaviorProperties.Object;

                    // TODO: GET BEST MATCH TO ITEM IN SCENE IF COMMAND
                    string objectBestMatch = getObjectBestMatch(nameOfUserRequestedObject);
                }
                break;
        }

        GptResponse gptResponce = new GptResponse(
            inputType,
            generatedText,
            agentBehaviorProperties);

        exception = null;
        return gptResponce;
    }

    private InputType classifyText(string userInput)
    {
        string promptStart = prompts.InputClassifier;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        string answer = responce.Item1;

        // TODO: CODE TO SELECT CORRECT INPUT CLASSIFICATION
        InputType type; // todo

        if (answer == "Question")
        {
            type = InputType.Question;
        }
        else if (answer == "Command")
        {
            type = InputType.Command;
        }
        else
        {
            type = InputType.Unknown;
        }
        return type;
    }

    private string answerQuestion(string userInput)
    {
        string promptStart = prompts.QuestionResponder;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        string answer = responce.Item1;

        return answer;
    }

    private AgentBehaviorProperties parseCommand(string userInput)
    {
        string promptStart = prompts.CommandParser;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        string answer = responce.Item1;

        string behavior = null;
        string sceneObject = null;
        string location = null;

        if (answer.Contains("behavior"))
        {
            int startIndex = answer.IndexOf("behavior: ") + "behavior: ".Length;
            int endIndex = answer.IndexOf(",");
            behavior = answer.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (answer.Contains("object"))
        {
            int startIndex = answer.IndexOf("object: ") + "object: ".Length;
            int endIndex = 0;
            if (answer.Contains("location"))
            {
                endIndex = answer.IndexOf(',', answer.IndexOf(',') + 1);
            }
            else 
            { 
                endIndex = answer.IndexOf('}'); 
            }   

            sceneObject = answer.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (answer.Contains("location"))
        {
            int startIndex = answer.IndexOf("location: ") + "location: ".Length;
            int endIndex = answer.IndexOf('}');
            location = answer.Substring(startIndex, endIndex - startIndex).Trim();
        }

        // TODO: use jsonFormattedText to create object.  need to deserialize responce
        bool success = true; // TODO: check if success


        // IF THERE IS AN ERROR AND WE CAN'T SERIALIZE, RETURN NULL - FAILED, Try again?
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
        string fullPrompt = promptStart + "Input: " + sceneObject;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        string answer = responce.Item1.Trim();

        //we dont have list yet so List for now
        string[] tools = { "Hammer", "Saw",
                        "Gun", "Knife" };
        // TODO: if the responce is not a valid object, try sending to GPT-3 3 more times before returning ""
        int trial = 1;
        //Loop if responce not in tool and trial under 4, (so we can do 3 times)

        while (!((tools.Contains(answer)) || (trial < 4)))
        {
            responce = connection.GenerateText(fullPrompt);
            answer = responce.Item1.Trim();
            trial++;
        }

        return answer;
    }
}