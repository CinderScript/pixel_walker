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

    public UserInputHandler(string apiKey, string gptPromptsFilePath, EngineType engine)
    {
        connection = new Gpt3Connection(apiKey, engine);
        prompts = PromptLoader.GetPromptsFromFile(gptPromptsFilePath);
    }

    public GptResponse GetGptResponce(string userInput, out Exception exception)
    {
        //Exception ex;
        InputType inputType = classifyText(userInput, out exception);
        string generatedText = "Error: No Responce Saved";

        AgentBehaviorProperties agentBehaviorProperties = null;

        switch (inputType)
        {
            case InputType.Unknown:
                // TODO: send back responce or check again
                exception = new Exception("The user's input could not be classified.");
                //exception = ex;
                return null;

            case InputType.Question:
                generatedText = answerQuestion(userInput, out exception);
                break;

            case InputType.Command:
                agentBehaviorProperties = parseCommand(userInput, out exception);

                // error checking - if this is null (couldn't parse) then send exception
                if (agentBehaviorProperties == null)
                {
                    //exception = new Exception("Couldn't parse command");
                    //exception = ex;
                    return null;
                }
                else
                {   // COMMAND WAS PARSED
                    string nameOfUserRequestedObject = agentBehaviorProperties.Object;

                    // TODO: GET BEST MATCH TO ITEM IN SCENE IF COMMAND
                    string objectBestMatch = getObjectBestMatch(nameOfUserRequestedObject, out exception);
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

    private InputType classifyText(string userInput, out Exception ex)
    {
        string promptStart = prompts.InputClassifier;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        string answer = responce.Item1.Trim();

        ex = responce.Item2;

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

    private string answerQuestion(string userInput, out Exception ex)
    {
        string promptStart = prompts.QuestionResponder;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        // TODO: HANDLE EXCEPTION

        string answer = responce.Item1;
        ex = responce.Item2;

        return answer;
    }

    private AgentBehaviorProperties parseCommand(string userInput, out Exception ex)
    {
        string promptStart = prompts.CommandParser;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        // TODO: HANDLE EXCEPTION

        string responceText = responce.Item1;
        ex = responce.Item2;

        BehaviorType behavior = BehaviorType.Unknown;
        string sceneObject = null;
        string location = null;

        if (responceText.Contains("behavior"))
        {
            int startIndex = responceText.IndexOf("behavior: ") + "behavior: ".Length;
            int endIndex = responceText.IndexOf(",");
            var behaviorString = responceText.Substring(startIndex, endIndex - startIndex).Trim();

            if (behaviorString == "Navigate")
            {
                behavior = BehaviorType.Navigate;
            }
            else if (behaviorString == "PickUp")
            {
                behavior = BehaviorType.PickUp;
            }
            else if (behaviorString == "Drop")
            {
                behavior = BehaviorType.Drop;
            }
            else if (behaviorString == "Activate")
            {
                behavior = BehaviorType.Activate;
            }
            else if (behaviorString == "SetDown")
            {
                behavior = BehaviorType.SetDown;
            }
            else if (behaviorString == "Open")
            {
                behavior = BehaviorType.Open;
            }
        }

        if (responceText.Contains("object"))
        {
            int startIndex = responceText.IndexOf("object: ") + "object: ".Length;
            int endIndex = 0;

            if (responceText.Contains("location"))
            {
                endIndex = responceText.IndexOf(',', responceText.IndexOf(',') + 1);
            }
            else
            {
                endIndex = responceText.IndexOf('}');
            }

            sceneObject = responceText.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (responceText.Contains("location"))
        {
            int startIndex = responceText.IndexOf("location: ") + "location: ".Length;
            int endIndex = responceText.IndexOf('}');
            location = responceText.Substring(startIndex, endIndex - startIndex).Trim();
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

    private string getObjectBestMatch(string sceneObject, out Exception exception)
    {
        string promptStart = prompts.BestMatchSelector;

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + sceneObject;
        fullPrompt = fullPrompt + "Output:";

        Tuple<string, Exception> responce = connection.GenerateText(fullPrompt);

        // TODO: HANDLE EXCEPTION

        string answer = responce.Item1.Trim();
        exception = responce.Item2;

        //we dont have list yet so List for now
        string[] props = { "Hammer", "Saw",
                        "Gun", "Knife" };
        // TODO: if the responce is not a valid object, try sending to GPT-3 3 more times before returning ""
        int trial = 1;
        //Loop if responce not in tool and trial under 4, (so we can do 3 times)

        while (!((props.Contains(answer)) || (trial < 4)))
        {
            responce = connection.GenerateText(fullPrompt);
            answer = responce.Item1.Trim();
            trial++;
        }

        return answer;
    }

}