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

public class Gpt3Connection
{
    public string ApiKey { get; }

    // any other relevent properties

    public Gpt3Connection(string apiKey)
    {
        ApiKey = apiKey;
    }

    public Tuple<string, Exception> GenerateText(string prompt)
    {
        Tuple<string, Exception> res = CallOpenAI(250, prompt, "text-davinci-002", 0.7, 1, 0, 0);
        return res;
    }

    public Tuple<string, Exception> CallOpenAI(int tokens, string input, string engine,
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
                    var replyText = parsedJSON["choices"][0]["text"];

                    if (json != null)
                    {
                        replywithException = Tuple.Create(replyText.ToString().Trim(), e);
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
