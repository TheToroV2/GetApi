using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("Gemini_ApiKey");
            Console.WriteLine($"API Key: {_apiKey}");


            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("Gemini API key is missing. Set it in environment variables.");
            }
        }

        public async Task<string> TranslateToSQL(string naturalLanguageQuery)
        {
            string prompt = $"Translate the following natural language query to SQL:\n\n{naturalLanguageQuery}";
            return await GetGeminiResponse(prompt); // Reuse the core logic
        }


        public async Task<string> GetChatResponse(string prompt)
        {
            return await GetGeminiResponse(prompt);
        }

        private async Task<string> GetGeminiResponse(string prompt)  // Refactored core logic
        {
            var requestBody = new
            {
                model = "gemini-1.5-flash", 
                contents = new[]
                {
                new { role = "user", parts = new[] { new { text = prompt } } }
            }
            };

            var requestContent = new StringContent( JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,"application/json"
            );

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key=" + _apiKey

,
                requestContent 
            );

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                try //Try-catch block for JSON parsing
                {
                    //JSON parsing with JsonNode:
                    using JsonDocument document = JsonDocument.Parse(responseContent);
                    JsonElement root = document.RootElement;

                    if (root.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                    {
                        JsonElement firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out JsonElement content) && content.TryGetProperty("parts", out JsonElement parts) && parts.GetArrayLength() > 0)
                        {
                            JsonElement firstPart = parts[0];
                            if (firstPart.TryGetProperty("text", out JsonElement text))
                            {
                                return text.GetString();
                            }
                        }
                    }

                    // Handle cases where the JSON structure is unexpected
                    return "Unexpected JSON response structure."; // Or throw an exception
                }
                catch (JsonException ex)
                {
                    // Handle JSON parsing errors
                    return $"Error parsing JSON response: {ex.Message}"; // Or throw the exception
                }


            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}"); // Include error details
            }
        }

    }
}

