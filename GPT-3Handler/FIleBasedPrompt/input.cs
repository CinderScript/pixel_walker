using System.IO;
using System.Text.Json;

public class InputResponder
{
	private Gpt3_Handler gpt3_Handler;
	private Prompts prompts;

	public InputResponder(Gpt3_Handler gpt3_Handler, Prompts prompts)
	{
		this.gpt3_Handler = gpt3_Handler;
		this.prompts = prompts;
	}

	public DavesResponse DaveResponse(string userInput)
	{
		StatementType type = gpt3_Handler.getResponse; /// from gpt-3
		DavesResponse davesResponse;
		if (type == StatementType.Question)
		{
			GetAnswer(userInput);
		}
		else if (type == StatementType.Command)
		{
			GetCommandParse(userInput);
		}
		else
		{

		}
	}
		private StatementType InputType(string prompt)
		{
			return StatementType.Question;
		}
		private string GetAnswer(string prompt)
		{
		
		}
		private Json GetCommandParse(string prompt)
		{
			return JsonSerializer.Serialize<Gpt3_Handler>(prompt);
		}
	}
	public enum StatementType
	{
		Question, Command, Unknown
	}
	public struct DavesResponse
	{
		StatementType StatementType;
		string Answer;
		Json CommandParse;
	}
