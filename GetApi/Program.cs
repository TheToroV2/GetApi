using Service; // Import your service namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register HttpClient and your service
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();