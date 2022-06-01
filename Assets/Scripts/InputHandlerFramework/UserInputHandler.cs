/**
* Project: Pixel Walker
*
* Description: UserInputHandler Class handles user input,
* and then sends it to GPT-3 while it awaits a response
* It classifies the string and then parses it accordingly.
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
using System.Threading.Tasks;

public class UserInputHandler
{
    private Gpt3Connection connection;
    private GptPrompts prompts;

    private PromptLoader classifer;
    private const int MAX_RETRIES = 3;
    private int retries = MAX_RETRIES;
    private int bestMatchRetries = MAX_RETRIES;
    private string propsInScene;

    
    /// <summary>
    /// Creates an User Input Handler object for interfacing
    /// with GPT-3
    /// </summary>
    /// <param name="apiKey"> the unencrypted api key</param>
    /// <param name="gptPromptsFilePath">file path of encrypted key</param>
    /// <param name="engine">engine type for GPT-3 (default Davinci)</param>
    public UserInputHandler(string propsInScene, string apiKey, string gptPromptsFilePath, EngineType engine)
    {
        this.propsInScene = propsInScene;
        connection = new Gpt3Connection(apiKey, engine);
        classifer = new PromptLoader();
        prompts = classifer.GetPromptsFromFile(gptPromptsFilePath);
    }

    /// <summary>
    /// Returns A GptResponse object, classifying text as it is recieved
    /// </summary>
    /// <param name="userInput">the users input string</param>
    /// <returns></returns>
    public async Task<GptResponse> GetGptResponce(string userInput)
    {
        InputType inputType;

        try
        {
            inputType = await classifyText(userInput);
        }
        catch (Exception)
        {

            throw;
        }
        string generatedText = "Error: No Responce Saved";

        AgentBehaviorProperties agentBehaviorProperties = null;

        string objectBestMatch = "";

        switch (inputType)
        {
            case InputType.Unknown:
                --retries;
                GptResponse temp = new GptResponse(inputType, generatedText, agentBehaviorProperties);
                if (retries > 0)
                {
                    temp = await GetGptResponce(userInput);
                }
                retries = MAX_RETRIES;
                return temp;

            case InputType.Question:
                try
                {
                    generatedText = await answerQuestion(userInput);
                }
                catch (Exception)
                {
                    throw;
                }
                break;

            case InputType.Conversation:
                try
                {
                    generatedText = await respondConversation(userInput);
                }
                catch (Exception)
                {
                    throw;
                }
                break;
            case InputType.Command:
                try
                {
                    agentBehaviorProperties = await parseCommand(userInput);

                }
                catch (Exception)
                {
                    throw;
                }
                if (agentBehaviorProperties == null)
                {
                    return null;
                }
                else
                {   
                    string nameOfUserRequestedObject = agentBehaviorProperties.Object;

                    try
                    {
                        objectBestMatch = await getObjectBestMatch(nameOfUserRequestedObject);
                        agentBehaviorProperties.Object = objectBestMatch;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    // best match on location
                }
                break;
        }

        GptResponse gptResponce = new GptResponse(
            inputType,
            generatedText,
            agentBehaviorProperties);

        return gptResponce;
    }

    /// <summary>
    /// Classifies whethert he user's input is a question, command, conversation
    /// or unknown.
    /// </summary>
    /// <param name="userInput">the user's input string</param>
    /// <returns></returns>
    private async Task<InputType> classifyText(string userInput)
    {
        string promptStart = prompts.InputClassifier;
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        string responce;
        try
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        catch (Exception)
        {
            throw;
        }

        string answer = responce.ToLower();

        // TODO: CODE TO SELECT CORRECT INPUT CLASSIFICATION
        InputType type; // todo

        if (answer == "question")
        {
            type = InputType.Question;
        }
        else if (answer == "conversation")
        {
            type = InputType.Conversation;
        }
        else if (answer == "command")
        {
            type = InputType.Command;
        }
        else
        {
            type = InputType.Unknown;
        }
        return type;
    }

    /// <summary>
    /// Method for handling user input if conversation.
    /// </summary>
    /// <param name="userInput">the user's input string</param>
    /// <returns></returns>
    private async Task<string> respondConversation(string userInput)
    {
        string promptStart = prompts.ConversationResponder;
        promptStart = promptStart.Replace("{$$props$$}", propsInScene);

        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        string responce;
        try
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        catch (Exception)
        {
            throw;
        }

        return responce;
    }

    /// <summary>
    /// Method for handling user input if a question.
    /// </summary>
    /// <param name="userInput">The user's input string</param>
    /// <returns></returns>
    private async Task<string> answerQuestion(string userInput)
    {
        string promptStart = prompts.QuestionResponder;
        promptStart = promptStart.Replace("{$$props$$}", propsInScene);

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";

        string responce;
        try
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        catch (Exception)
        {
            throw;
        }

        return responce;
    }

    /// <summary>
    /// Method for handling user input if command.
    /// Mehod also parses the response for behavior, object and location
    /// </summary>
    /// <param name="userInput"></param>
    /// <returns></returns>
    private async Task<AgentBehaviorProperties> parseCommand(string userInput)
    {
        string promptStart = prompts.CommandParser;
        string fullPrompt = promptStart + "Input: " + userInput;
        fullPrompt = fullPrompt + "Output:";
        bool success = true;

        string responce;
        try
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        catch (Exception)
        {
            throw;
        }

        BehaviorType behavior = BehaviorType.Unknown;
        string sceneObject = null;
        string location = null;

        if (responce.Contains("behavior"))
        {
            int startIndex = responce.IndexOf("behavior: ") + "behavior: ".Length;
            int endIndex = responce.IndexOf(",");
            var behaviorString = responce.Substring(startIndex, endIndex - startIndex).Trim().ToLower();

            if (behaviorString == "navigate")
            {
                behavior = BehaviorType.Navigate;
            }
            else if (behaviorString == "pickup")
            {
                behavior = BehaviorType.PickUp;
            }
            else if (behaviorString == "drop")
            {
                behavior = BehaviorType.Drop;
            }
            else if (behaviorString == "turnon")
            {
                behavior = BehaviorType.TurnOn;
            }
            else if (behaviorString == "turnoff")
            {
                behavior = BehaviorType.TurnOff;
            }
            else if (behaviorString == "setdown")
            {
                behavior = BehaviorType.SetDown;
            }
            else if (behaviorString == "open")
            {
                behavior = BehaviorType.Open;
            }
        }

        if (responce.Contains("object"))
        {
            int startIndex = responce.IndexOf("object: ") + "object: ".Length;
            int endIndex = 0;

            if (responce.Contains("location"))
            {
                endIndex = responce.IndexOf(',', responce.IndexOf(',') + 1);
            }
            else
            {
                endIndex = responce.IndexOf('}');
            }

            sceneObject = responce.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (responce.Contains("location"))
        {
            int startIndex = responce.IndexOf("location: ") + "location: ".Length;
            int endIndex = responce.IndexOf('}');
            location = responce.Substring(startIndex, endIndex - startIndex).Trim();
        }

        if (!success)
        {
            throw new Exception("Error: Couldn't parse command");
        }

        AgentBehaviorProperties parse = new AgentBehaviorProperties(behavior, sceneObject, location);

        return parse;
    }


    /// <summary>
    /// Method for finding the closest object to the user's input
    /// </summary>
    /// <param name="sceneObject"> name of object in command string</param>
    /// <returns></returns>
    private async Task<string> getObjectBestMatch(string sceneObject)
    {
        string promptStart = prompts.BestMatchSelector;

        promptStart = promptStart.Replace("{$$props$$}", propsInScene);

        string fullPrompt = promptStart + "Input: " + sceneObject.ToLower();
        fullPrompt = fullPrompt + "Output:";

        string responce;
        try
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        catch (Exception)
        {
            throw;
        }

        bool matchObj = propsInScene.Contains(responce.ToLower());
        if (!matchObj)
        {
            responce = await connection.GenerateText(fullPrompt);
        }
        return responce;
    }

}