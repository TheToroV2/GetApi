using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class ChatGPTService : IChatGPTService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

     public ChatGPTService(HttpClient httpClient)
         {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey"); 

             if (string.IsNullOrEmpty(_apiKey))
                {
                throw new Exception("OpenAI API key is missing. Set it in environment variables.");
            }
        }



        public async Task<string> GetChatResponse(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new
            {
                role = "system",
                content = "You are an SQL expert. Translate the following natural language query into SQL code. " +
                "Provide only the SQL code in your response."
            },
            new { role = "user", content = prompt }
        },
                max_tokens = 200
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonDocument>(responseContent);
                return jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
        }

    }
}

