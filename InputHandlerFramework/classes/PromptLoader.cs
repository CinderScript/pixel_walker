using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PromptLoader
{

    //NOTE: FilePrompts not implemented yet,
    //thus using hard coded classifier variables for testing purposes - Shub
    public static GptPrompts GetPromptsFromFile(string filePath)
    {
        var inputClassifier = "Input: Is there a hammer in this room?" +
                       "Output: Question {stop}" +
                       "Input: Is there a hammer here" +
                       "Output: Question {stop}" +
                       "Input: Can you move the table over there" +
                       "Output: Question {stop}" +
                       "Input: Is the heater on?" +
                       "Output: Question {stop}" +
                       "Input: Can you pick up the package, the one near the door?" +
                       "Output: Question {stop}" +
                       "Input: Turn on the headlight" +
                       "Output: Command {stop}" +
                       "Input: Push that box away, the one on your left" +
                       "Output: Command {stop}" +
                       "Input: Please fix the car" +
                       "Output: Command {stop}" +
                       "Input: Move the fire extinguisher to the back" +
                       "Output: Command {stop}" +
                       "Input: Is the right-front tire fixed?" +
                       "Output: Question {stop}" +
                       "Input: Did you have lunch?" +
                       "Output: Question {stop}";

        var questionClassifier = "Dave is a two-month-old robot currently working in a garage. " +
            "There are many tools in the garage. He can find objects, pick things up, and set things back down. " +
            "Dave can also open close doors, chests, and other things. Dave currently cannot use tools or other objects. " +
            "This is a list of all the objects in the garage: {$$props$$}." +
            "Dave is a mechanic, but in his spare time, he enjoys reading sci-fi novels. Revelation Space is his favorite series. He also loves listening to Beetles. Dave likes his glasses so he doesn’t want to take them off or change them. He also doesn’t know how to ride a skateboard. Dave knows how to turn tools and appliances on, but doesn’t know how to use them." +
            "Input: Hey Dave, can you find a hammer?" +
            "Output: Yes. {stop}" +
            "Input: Who are you?" +
            "Output: I'm Dave, a friendly robot. Who are you? {stop}";
        ;
        var responseClassifier =
                            "Input: Find the hammer" +
                           "Output: {behavior: WalkTo, object: hammer}" +
                           "Input: Pick up the screwdriver." +
                           "Output: {behavior: PickUp, object: screwdriver}" +
                           "Input: Set down the book on the bench" +
                           "Output: {behavior: SetDown, object: book, location: bench}" +
                           "Input: Set down the lamp" +
                           "Output: {behavior: SetDown, object: book, location: null}" +
                           "Input: Locate the water bottle" +
                           "Output: {behavior: WalkTo, object: water_bottle}" +
                           "Input: Put down the saw" +
                           "Output: {behavior: SetDown, object: saw, location: null}" +
                           "Input: Open the backdoor" +
                           "Output: {behavior: Open, object: backdoor}" +
                           "Input: Close the cabinet" +
                           "Output: {behavior: Close, object: cabinet}" +
                           "Input: Throw the ball into the bucket" +
                           "Output: {behavior: Throw, object: ball, location: bucket}" +
                           "Input: Stack the cardboard boxes" +
                           "Output: {behavior: Stack, object: cardboard_boxes, location: null}" +
                           "Input: Navigate the toolbox" +
                           "Output: {behavior: WalkTo, object: toolbox}" +
                           "Input: Obtain the wrench" +
                           "Output: {behavior: PickUp, object: wrench}" +
                           "Input: Get yourself a saw" +
                           "Output: {behavior: PickUp, object: saw}" +
                           "Input: Stroll over to the door" +
                           "Output: {behavior: WalkTo, object: door}" +
                           "Input: Drop the nail in the toolbox" +
                           "Output: {behavior: SetDown, object: nail, location: toolbox}";

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