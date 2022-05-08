using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;


public class GPTHandler : MonoBehaviour
{
/*

    public class GPTJson{
        public string id; 
        public string obj; 
        public string created; 
        
        public string model; 
        
        public List<Metadata> choices;

    }

    public class Metadata{
        public string text{ get; set; }
    }
*/

    public static string callOpenAI(int tokens, string input, string engine,
        double temperature, int topP, int frequencyPenalty, int presencePenalty)
        {
            var openAiKey = "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";
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
                                                            ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");

                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = httpClient.SendAsync(request).Result;
                        string json = response.Content.ReadAsStringAsync().Result;

                        //var dynObj = JsonConvert.DeserializeObject(json);

                        if (json != null)
                        {
                            return json;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            return null;

        }
}
