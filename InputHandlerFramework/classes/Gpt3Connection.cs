using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Gpt3Connection
{
	public string ApiKey { get; }

	// any other relevent properties
	
	public Gpt3Connection(string apiKey)
	{
		ApiKey = apiKey;
	}

	public string GenerateText(string propt)
	{
		// call the API
		// return the result
		return "not implemented";
	}
}
