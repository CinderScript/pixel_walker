using Newtonsoft.Json.Linq;
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

    public string Engine;

    // any other relevent properties

    public Gpt3Connection(string apiKey)
    {
        ApiKey = apiKey;
    }

    public Tuple<string, Exception> GenerateText(string prompt, string engine)
    {
        Tuple<string, Exception> result = CallOpenAI(250, prompt, engine, 0.7, 1, 0, 0);
        return result;
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

                    var response = await httpClient.SendAsync(request).Result;
                    var json =  response.Content.ReadAsStringAsync().Result;

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