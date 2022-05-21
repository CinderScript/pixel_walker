using System;
using System.IO;
using Newtonsoft.Json;
namespace System.Text.Json

{
    public class GPTPrompt
    {
        public string ClassifyResponsePrompt { get; set; }
        public string GetQuestionAnswerPrompt { get; set; }
        public string ParseCommandPrompt { get; set; }

        public GPTPrompt GetPrompt()
        {
            GPTPrompt Prompt;
            StreamReader r = new StreamReader("C:/Users/Tn/Documents/ReadPrompts/Prompt.json");
            string jsonString = r.ReadToEnd();
            Prompt = JsonSerializer.Deserialize<GPTPrompt>(jsonString);
            ClassifyResponsePrompt = Prompt.ClassifyResponsePrompt;
            GetQuestionAnswerPrompt = Prompt.GetQuestionAnswerPrompt;
            ParseCommandPrompt = Prompt.ParseCommandPrompt;
            return Prompt;
        }
    }

    public class Program
    {
        static void Main()
        {
            GPTPrompt example = new GPTPrompt();
            example.GetPrompt();
            Console.WriteLine($"{example.ClassifyResponsePrompt}");
        }
    }
}