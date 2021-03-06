using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;

//[DEPRECATED: Now handled by the InputHandlerFramework] - SHUB 5/23/22
public class GPTHandler
{
    public static string keyString = "";
    public static Tuple<string, Exception> CallOpenAI(int tokens, string input, string engine,
        double temperature, int topP, int frequencyPenalty, int presencePenalty)
    {
        Exception e = null;
        Tuple<string, Exception> replywithException = null;

        var openAiKey = keyString;
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
            replywithException = Tuple.Create("ERROR: " + ex.Message, ex);
        }
        return replywithException;
    }
}
