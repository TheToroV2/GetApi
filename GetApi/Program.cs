using Service;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Get API Key from configuration
var apiKey = builder.Configuration["OpenAI:ApiKey"];

builder.Services.AddHttpClient<ChatGPTService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
});

// Register ChatGPTService with API Key
builder.Services.AddSingleton(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new ChatGPTService(httpClient, apiKey);
});

var app = builder.Build();
app.Run();