//**Instructions on getting 'Newtonsoft.Json' from NuGet on Visual Studio  
//https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Security.AccessControl;

public class Gpt3Connection
{
    public string ApiKey { get; }
    public EngineType GptEngine { get; }

    // any other relevent properties

    public Gpt3Connection(string apiKey, EngineType gptEngine)
    {
        ApiKey = apiKey;
        GptEngine = gptEngine;
    }


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
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return reply;
    }


}
