using YouTubeRAGChatbot.Web.Components;
using YouTubeRAGChatbot.Core.Services;
using YouTubeRAGChatbot.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Load settings
var settings = new AppSettings();
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);

Console.WriteLine($"AI Provider: {settings.AIProvider}");

// Register services
builder.Services.AddSingleton<ITranscriptFetcherService, TranscriptFetcherService>();
builder.Services.AddSingleton<ITextChunkerService, TextChunkerService>();

// Register AI provider-specific services
if (settings.AIProvider == "HuggingFace")
{
    // HuggingFace services
    builder.Services.AddSingleton<IHuggingFaceService>(_ => 
        new HuggingFaceService(
            settings.HuggingFace.ApiKey, 
            settings.HuggingFace.EmbeddingModel,
            settings.HuggingFace.Model));
    
    builder.Services.AddSingleton<IVectorDatabaseService>(sp =>
    {
        var hfService = sp.GetRequiredService<IHuggingFaceService>();
        return new HuggingFaceVectorDatabaseService(hfService);
    });
    
    builder.Services.AddSingleton<IRAGChatbotService>(sp =>
    {
        var vectorDb = sp.GetRequiredService<IVectorDatabaseService>();
        var hfService = sp.GetRequiredService<IHuggingFaceService>();
        return new HuggingFaceRAGChatbotService(
            vectorDb,
            hfService,
            settings.HuggingFace.Temperature,
            settings.HuggingFace.MaxTokens
        );
    });
}
else
{
    // OpenAI services
    builder.Services.AddSingleton<IVectorDatabaseService>(_ => 
        new VectorDatabaseService(settings.OpenAI.ApiKey, settings.OpenAI.EmbeddingModel));
    
    builder.Services.AddSingleton<IRAGChatbotService>(sp =>
    {
        var vectorDb = sp.GetRequiredService<IVectorDatabaseService>();
        return new RAGChatbotService(vectorDb, settings.OpenAI.ApiKey, settings.OpenAI.Model, settings.OpenAI.Temperature);
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
