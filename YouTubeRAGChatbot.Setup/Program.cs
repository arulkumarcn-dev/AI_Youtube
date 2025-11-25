using YouTubeRAGChatbot.Core.Services;
using YouTubeRAGChatbot.Core.Configuration;
using Microsoft.Extensions.Configuration;

Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  Quick Setup - Initialize Vector Database                 ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var settings = new AppSettings();
configuration.Bind(settings);

Console.WriteLine($"AI Provider: {settings.AIProvider}");
Console.WriteLine();

// Sample video ID (Rick Astley - Never Gonna Give You Up - has captions)
var sampleVideoId = "dQw4w9WgXcQ";

Console.WriteLine($"This will add a sample video to initialize your database.");
Console.WriteLine($"Video: https://www.youtube.com/watch?v={sampleVideoId}");
Console.WriteLine();
Console.Write("Continue? (Y/n): ");
var response = Console.ReadLine()?.Trim().ToLower();

if (response == "n" || response == "no")
{
    Console.WriteLine("Setup cancelled.");
    return;
}

try
{
    // Step 1: Fetch transcript
    Console.WriteLine("\n📥 Step 1: Fetching YouTube transcript...");
    var fetcher = new TranscriptFetcherService();
    var transcript = await fetcher.FetchTranscriptAsync(sampleVideoId);
    await fetcher.SaveTranscriptAsync(transcript, settings.Storage.TranscriptDirectory);

    // Step 2: Chunk transcript
    Console.WriteLine("\n✂️  Step 2: Chunking transcript...");
    var chunker = new TextChunkerService();
    var chunks = chunker.ChunkTranscript(transcript, settings.RAG.ChunkSize, settings.RAG.ChunkOverlap);

    // Step 3: Create vector database
    Console.WriteLine("\n🗄️  Step 3: Creating vector database...");
    IVectorDatabaseService vectorDb;
    
    if (settings.AIProvider == "HuggingFace")
    {
        var hfService = new HuggingFaceService(
            settings.HuggingFace.ApiKey,
            settings.HuggingFace.EmbeddingModel
        );
        vectorDb = new HuggingFaceVectorDatabaseService(hfService, settings.Storage.VectorDbDirectory);
    }
    else
    {
        vectorDb = new VectorDatabaseService(
            settings.OpenAI.ApiKey,
            settings.OpenAI.EmbeddingModel
        );
        await vectorDb.InitializeAsync();
    }
    
    await vectorDb.AddChunksAsync(chunks);
    await vectorDb.SaveToFileAsync(settings.Storage.VectorDbDirectory);

    Console.WriteLine("\n✅ Setup Complete!");
    Console.WriteLine($"   - Video added: {transcript.Title}");
    Console.WriteLine($"   - Chunks created: {chunks.Count}");
    Console.WriteLine($"\n🎉 You can now run the web app or console app!");
    Console.WriteLine("\nNext steps:");
    Console.WriteLine("  • Run: dotnet run --project YouTubeRAGChatbot.Web");
    Console.WriteLine("  • Or: dotnet run --project YouTubeRAGChatbot.Console");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    Console.WriteLine("\nTroubleshooting:");
    Console.WriteLine("  • Check your API key in appsettings.json (OpenAI or HuggingFace)");
    Console.WriteLine("  • Ensure you have internet connection");
    Console.WriteLine("  • Try a different video ID if this one fails");
    Console.WriteLine("  • For HuggingFace: First request may take 20-60s to load model");
}
