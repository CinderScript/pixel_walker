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
                // TODO: send back responce or check again
                throw new Exception("The user's input could not be classified.");

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

                // error checking - if this is null (couldn't parse) then send exception
                if (agentBehaviorProperties == null)
                {
                    //exception = new Exception("Couldn't parse command");
                    return null;
                }
                else
                {   // COMMAND WAS PARSED
                    string nameOfUserRequestedObject = agentBehaviorProperties.Object;

                    // TODO: GET BEST MATCH TO ITEM IN SCENE IF COMMAND
                    try
                    {
                        objectBestMatch = await getObjectBestMatch(nameOfUserRequestedObject);
                        generatedText = objectBestMatch;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                break;
        }

        GptResponse gptResponce = new GptResponse(
            inputType,
            generatedText,
            agentBehaviorProperties);

        return gptResponce;
    }

    private async Task<InputType> classifyText(string userInput)
    {
        string promptStart = prompts.InputClassifier;

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

        string answer = responce;

        // TODO: CODE TO SELECT CORRECT INPUT CLASSIFICATION
        InputType type; // todo

        if (answer == "Question")
        {
            type = InputType.Question;
        }
        else if (answer == "Conversation")
        {
            type = InputType.Conversation;
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
    private async Task<string> respondConversation(string userInput)
    {
        string promptStart = prompts.ConversationResponder;
        promptStart = promptStart.Replace("{$$props$$}", "Light Switch Workshop, Bench Grinder, 5 Gallon Bucket, Lid of Paint Can, Can of Blue Paint, Wood Box, Garden Rake, Unnamed, Red Plastic Bin, Orange Handled Pliers, Unnamed, Red Pipe Wrench, Unnamed, Yellow Level, Wooden Workbench, Small Portable Workbench, Head of Welding Torch, Blue Spray Paint, Brown Spray Paint, Yellow Spray Paint, Red Spray Paint, Green Spray Paint, White Paint Can, C Clamp, Sound System, Map of New Mexico, Drill Press, Arizona License Plate, Light Switch Tool Room, Acetylene Tank, Oxygen Tank, Plastic Safety Goggles, Blue Level, Blue Plastic Crate, Unnamed, Blue Paint Can, Bench Vice, Paint Thinner, Large Phillips Screwdriver, Small Phillips Screwdriver, Small Stool, Yellow Claw Hammer, Handsaw, Tree on Rock");

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
    private async Task<string> answerQuestion(string userInput)
    {
        string promptStart = prompts.QuestionResponder;
        promptStart = promptStart.Replace("{$$props$$}", "Light Switch Workshop, Bench Grinder, 5 Gallon Bucket, Lid of Paint Can, Can of Blue Paint, Wood Box, Garden Rake, Unnamed, Red Plastic Bin, Orange Handled Pliers, Unnamed, Red Pipe Wrench, Unnamed, Yellow Level, Wooden Workbench, Small Portable Workbench, Head of Welding Torch, Blue Spray Paint, Brown Spray Paint, Yellow Spray Paint, Red Spray Paint, Green Spray Paint, White Paint Can, C Clamp, Sound System, Map of New Mexico, Drill Press, Arizona License Plate, Light Switch Tool Room, Acetylene Tank, Oxygen Tank, Plastic Safety Goggles, Blue Level, Blue Plastic Crate, Unnamed, Blue Paint Can, Bench Vice, Paint Thinner, Large Phillips Screwdriver, Small Phillips Screwdriver, Small Stool, Yellow Claw Hammer, Handsaw, Tree on Rock");

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

    private async Task<AgentBehaviorProperties> parseCommand(string userInput)
    {
        string promptStart = prompts.CommandParser;

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

        // TODO: HANDLE EXCEPTION

        BehaviorType behavior = BehaviorType.Unknown;
        string sceneObject = null;
        string location = null;

        if (responce.Contains("behavior"))
        {
            int startIndex = responce.IndexOf("behavior: ") + "behavior: ".Length;
            int endIndex = responce.IndexOf(",");
            var behaviorString = responce.Substring(startIndex, endIndex - startIndex).Trim();

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

        // TODO: use jsonFormattedText to create object.  need to deserialize responce
        bool success = true; // TODO: check if success


        // IF THERE IS AN ERROR AND WE CAN'T SERIALIZE, RETURN NULL - FAILED, Try again?
        if (!success)
        {
            throw new Exception("Error: Couldn't parse command");
        }

        AgentBehaviorProperties parse = new AgentBehaviorProperties(behavior, sceneObject, location);

        return parse;
    }

    private async Task<string> getObjectBestMatch(string sceneObject)
    {
        string promptStart = prompts.BestMatchSelector;
        
        promptStart = promptStart.Replace("{$$props$$}", "Light Switch Workshop, Bench Grinder, 5 Gallon Bucket, Lid of Paint Can, Can of Blue Paint, Wood Box, Garden Rake, Unnamed, Red Plastic Bin, Orange Handled Pliers, Unnamed, Red Pipe Wrench, Unnamed, Yellow Level, Wooden Workbench, Small Portable Workbench, Head of Welding Torch, Blue Spray Paint, Brown Spray Paint, Yellow Spray Paint, Red Spray Paint, Green Spray Paint, White Paint Can, C Clamp, Sound System, Map of New Mexico, Drill Press, Arizona License Plate, Light Switch Tool Room, Acetylene Tank, Oxygen Tank, Plastic Safety Goggles, Blue Level, Blue Plastic Crate, Unnamed, Blue Paint Can, Bench Vice, Paint Thinner, Large Phillips Screwdriver, Small Phillips Screwdriver, Small Stool, Yellow Claw Hammer, Handsaw, Tree on Rock");

        // TODO: COMPLETE THE PROMPT WITH USER INPUT
        string fullPrompt = promptStart + "Input: " + sceneObject;
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

        // TODO: HANDLE EXCEPTION

        //we dont have list yet so List for now
        string[] props = { "Light Switch Workshop, Bench Grinder, 5 Gallon Bucket, Lid of Paint Can, Can of Blue Paint, Wood Box, Garden Rake, Unnamed, Red Plastic Bin, Orange Handled Pliers, Unnamed, Red Pipe Wrench, Unnamed, Yellow Level, Wooden Workbench, Small Portable Workbench, Head of Welding Torch, Blue Spray Paint, Brown Spray Paint, Yellow Spray Paint, Red Spray Paint, Green Spray Paint, White Paint Can, C Clamp, Sound System, Map of New Mexico, Drill Press, Arizona License Plate, Light Switch Tool Room, Acetylene Tank, Oxygen Tank, Plastic Safety Goggles, Blue Level, Blue Plastic Crate, Unnamed, Blue Paint Can, Bench Vice, Paint Thinner, Large Phillips Screwdriver, Small Phillips Screwdriver, Small Stool, Yellow Claw Hammer, Handsaw, Tree on Rock" };
        // TODO: if the responce is not a valid object, try sending to GPT-3 3 more times before returning ""
        int trial = 1;
        //Loop if responce not in tool and trial under 4, (so we can do 3 times)

        while (!((props.Contains(responce)) || (trial < 4)))
        {
            responce = await connection.GenerateText(fullPrompt);
            trial++;
        }

        return responce;
    }

}