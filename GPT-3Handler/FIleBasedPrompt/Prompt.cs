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
    }

    public class Program
    {
        static void Main()
        {
            StreamReader r = new StreamReader("C:/Users/Tn/Documents/ReadPrompts/Prompt.json");
            string jsonString = r.ReadToEnd();
            GPTPrompt Prompt = JsonSerializer.Deserialize<GPTPrompt>(jsonString);
            Console.WriteLine(Prompt.ClassifyResponsePrompt);
            Console.ReadKey();
        }
    }
}