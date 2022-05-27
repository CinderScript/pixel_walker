//**Instructions on getting 'Newtonsoft.Json' from NuGet on Visual Studio  
//https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio

using Newtonsoft.Json.Linq; // <-- Get 'Newtonsoft.Json' if unidentified namespace**
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;

public class Gpt3Connection
{
    public string ApiKey { get; }

    public EngineType GptEngine{ get;}

    // any other relevent properties

    public Gpt3Connection(string apiKey, EngineType gptEngine)
    {
        ApiKey = apiKey;
        GptEngine = gptEngine;
    }

    
    public Tuple<string, Exception> GenerateText(string prompt)
    {
        string engineUsed = "";
        if(GptEngine == EngineType.Curie){
            engineUsed = "text-curie-001";
        }
        else if(GptEngine == EngineType.Babbage){
            engineUsed = "text-babbage-001";
        }
        else if(GptEngine == EngineType.Ada){
            engineUsed = "text-ada-001";
        }
        else{
            engineUsed = "text-davinci-002";
        }

        Tuple<string, Exception> res = CallOpenAI(250, prompt, engineUsed, 0.7, 1, 0, 0);
        return res;
    }

    private Tuple<string, Exception> CallOpenAI(int tokens, string input, string engine,
        double temperature, int topP, int frequencyPenalty, int presencePenalty)
    {
        Exception e = null;
        Tuple<string, Exception> replywithException = null;

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

                    var response = httpClient.SendAsync(request).Result;
                    string json = response.Content.ReadAsStringAsync().Result;

                    var parsedJSON = JObject.Parse(json);
                    var modelType = parsedJSON["model"];
                    var replyText = parsedJSON["choices"][0]["text"];

                    if (json != null)
                    {
                        //string fullResoponse =  + " \nmodel used: " + modelType.ToString().Trim();
                        replywithException = Tuple.Create(replyText.ToString().Trim(), e);
                        Debug.Log(modelType.ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            replywithException = Tuple.Create("ERROR", ex);
        }
        return replywithException;
    }
}
