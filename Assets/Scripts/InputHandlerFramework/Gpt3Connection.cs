/**
* Project: Pixel Walker
*
* Description: Gpt3Connection class creates a Restful
* API call to GPT-3 to send and recieve responses
* 
* Author: Pixel Walker -
* Maynard, Gregory
* Shubhajeet, Baral
* Do, Khuong
* Nguyen, Thuong
*
* Date: 05-26-2022
*/


using Newtonsoft.Json.Linq;

using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using UnityEngine;

public class Gpt3Connection
{
    public string ApiKey { get; }
    public EngineType GptEngine { get; }

    public Gpt3Connection(string apiKey, EngineType gptEngine)
    {
        ApiKey = apiKey;
        GptEngine = gptEngine;
    }

    /// <summary>
    /// Uses the CallOpenAI method to generate a response.
    /// Response varies on engine selected.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public async Task<string> GenerateText(string prompt)
    {
        string engineUsed;
        switch (GptEngine)
        {
            case EngineType.Curie:
                engineUsed = "text-curie-001";
                break;
            case EngineType.Babbage:
                engineUsed = "text-babbage-001";
                break;
            case EngineType.Ada:
                engineUsed = "text-ada-001";
                break;
            case EngineType.Davinci:
                engineUsed = "text-davinci-002";
                break;
            default:
                engineUsed = "text-davinci-002";
                break;

        }

        var res = await CallOpenAI(400, prompt, engineUsed, 0.7, 1, 0, 0);
        return res;
    }

    /// <summary>
    /// A RESTful API call that sends a string input with relevant parameters
    /// to OpenAI and returns a JSON containing response 
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="input"></param>
    /// <param name="engine"></param>
    /// <param name="temperature"></param>
    /// <param name="topP"></param>
    /// <param name="frequencyPenalty"></param>
    /// <param name="presencePenalty"></param>
    /// <returns></returns>
    private async Task<string> CallOpenAI(int tokens, string input, string engine,
        double temperature, int topP, int frequencyPenalty, int presencePenalty)
    {
        string reply = null;

        var openAiKey = ApiKey;
        var apiCall = "https://api.openai.com/v1/engines/" + engine + "/completions";
        try
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                    request.Content = new StringContent("{\n  \"prompt\": \"" + input + "\",\n  \"temperature\": " +
                                                        temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + tokens + ",\n  \"top_p\": " + topP +
                                                        ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty +
                                                            ",\n  \"stop\": " + "[\"{stop}\"]" + "\n}");

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
                    var json = await response.Content.ReadAsStringAsync();

                    var parsedJSON = JObject.Parse(json);
                    var modelType = parsedJSON["model"];
                    var replyText = parsedJSON["choices"][0]["text"];

                    if (json != null)
                    {
                        //string fullResoponse =  + " \nmodel used: " + modelType.ToString().Trim();
                        reply = replyText.ToString().Trim();
                        Debug.Log(modelType.ToString() + ": " + reply);
                        Debug.Log(parsedJSON.ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
			var msg = "Was not able to connect to GPT-3.\n\n" +
				      "Please make sure that you have enough credit on your\n" +
					  "GPT-3 API key and that you have a working internet connection.";
			throw new Exception(msg);
		}
        return reply;
    }


}
