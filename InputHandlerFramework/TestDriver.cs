using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputHandlerFramework
{
	internal class TestDriver
	{
		static void Main(string[] args)
		{
			// GUI CODE - SHUBs side

			var apiKey = "1234567890";
			var promptsPath = "filePath.jason";
			var userInput = "user entered something here";

			UserInputHandler handler = new UserInputHandler(apiKey, promptsPath);
			Exception ex;
			GptResponse responce = handler.GetGptResponce(userInput, out ex);

			// SHUB's section: does something with responce
			if (responce != null)
			{
				var type = responce.Type;
				Console.WriteLine($"Type: {type}");

				if (type == InputType.Question)
				{
					var text = responce.GeneratedText;
					Console.WriteLine($"Question's Answer: {text}");
				}
				else if (type == InputType.Command)
				{
					var properties = responce.BehaviorProperties;
					Console.WriteLine($"Behavior: {properties.Behavior}");
					Console.WriteLine($"Object: {properties.Object}");
					Console.WriteLine($"Location: {properties.Location}");
				}
				else
				{
					Console.WriteLine("Should Not Enter This Code Block. (null result)");
				}
			}
			else
			{
				Console.WriteLine($"An error occured: {ex}");
			}

			Console.ReadKey();
		}
	}
}