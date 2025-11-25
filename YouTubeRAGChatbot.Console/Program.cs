using Microsoft.Extensions.Configuration;
using YouTubeRAGChatbot.Core.Configuration;
using YouTubeRAGChatbot.Core.Services;

namespace YouTubeRAGChatbot.Console;

class Program
{
    static async Task Main(string[] args)
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var settings = new AppSettings();
        configuration.Bind(settings);

        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║     YouTube RAG Chatbot - .NET Edition                   ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        System.Console.WriteLine($"🤖 AI Provider: {settings.AIProvider}");
        System.Console.WriteLine();

        // Check if setup mode
        if (args.Length > 0 && args[0].ToLower() == "setup")
        {
            await RunSetupAsync(settings, args.Skip(1).ToArray());
            return;
        }

        // Run chat mode
        await RunChatAsync(settings);
    }

    static async Task RunSetupAsync(AppSettings settings, string[] videoIds)
    {
        System.Console.WriteLine("═══════════════════════════════════════════════════════════");
        System.Console.WriteLine("SETUP MODE - Adding YouTube Videos");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════");
        System.Console.WriteLine();

        // Get video IDs if not provided
        if (videoIds.Length == 0)
        {
            System.Console.WriteLine("Enter YouTube video IDs or URLs (comma-separated):");
            System.Console.Write("> ");
            var input = System.Console.ReadLine();
            videoIds = input?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToArray() ?? Array.Empty<string>();
        }

        if (videoIds.Length == 0)
        {
            System.Console.WriteLine("❌ No video IDs provided.");
            return;
        }

        try
        {
            // Step 1: Fetch transcripts
            System.Console.WriteLine($"\n📥 Step 1: Fetching {videoIds.Length} transcripts...");
            var fetcher = new TranscriptFetcherService();
            var transcripts = await fetcher.FetchMultipleTranscriptsAsync(videoIds);

            if (transcripts.Count == 0)
            {
                System.Console.WriteLine("❌ No transcripts were fetched successfully.");
                return;
            }

            // Save transcripts
            foreach (var transcript in transcripts)
            {
                await fetcher.SaveTranscriptAsync(transcript, settings.Storage.TranscriptDirectory);
            }

            // Step 2: Chunk transcripts
            System.Console.WriteLine($"\n✂️  Step 2: Chunking transcripts...");
            var chunker = new TextChunkerService();
            var chunks = chunker.ChunkMultipleTranscripts(
                transcripts,
                settings.RAG.ChunkSize,
                settings.RAG.ChunkOverlap
            );

            // Step 3: Create vector database
            System.Console.WriteLine($"\n🗄️  Step 3: Creating vector database...");
            IVectorDatabaseService vectorDb;
            
            if (settings.AIProvider.Equals("HuggingFace", StringComparison.OrdinalIgnoreCase))
            {
                var hfService = new HuggingFaceService(
                    settings.HuggingFace.ApiKey,
                    settings.HuggingFace.EmbeddingModel,
                    settings.HuggingFace.Model
                );
                vectorDb = new HuggingFaceVectorDatabaseService(hfService);
            }
            else
            {
                vectorDb = new VectorDatabaseService(
                    settings.OpenAI.ApiKey,
                    settings.OpenAI.EmbeddingModel
                );
            }
            
            await vectorDb.InitializeAsync();
            await vectorDb.AddChunksAsync(chunks);
            await vectorDb.SaveToFileAsync(settings.Storage.VectorDbDirectory);

            System.Console.WriteLine("\n✅ Setup Complete!");
            System.Console.WriteLine($"   - {transcripts.Count} videos processed");
            System.Console.WriteLine($"   - {chunks.Count} chunks created");
            System.Console.WriteLine($"\nYou can now run the chatbot without 'setup' argument.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"\n❌ Error during setup: {ex.Message}");
        }
    }

    static async Task RunChatAsync(AppSettings settings)
    {
        System.Console.WriteLine("═══════════════════════════════════════════════════════════");
        System.Console.WriteLine("CHAT MODE");
        System.Console.WriteLine("═══════════════════════════════════════════════════════════");

        try
        {
            // Load vector database
            System.Console.WriteLine("\n📂 Loading vector database...");
            IVectorDatabaseService vectorDb;
            
            if (settings.AIProvider.Equals("HuggingFace", StringComparison.OrdinalIgnoreCase))
            {
                var hfService = new HuggingFaceService(
                    settings.HuggingFace.ApiKey,
                    settings.HuggingFace.EmbeddingModel,
                    settings.HuggingFace.Model
                );
                vectorDb = new HuggingFaceVectorDatabaseService(hfService);
            }
            else
            {
                vectorDb = new VectorDatabaseService(
                    settings.OpenAI.ApiKey,
                    settings.OpenAI.EmbeddingModel
                );
            }

            var dbPath = Path.GetFullPath(settings.Storage.VectorDbDirectory);
            System.Console.WriteLine($"   Path: {dbPath}");
            await vectorDb.LoadFromFileAsync(dbPath);
            System.Console.WriteLine($"✅ Database loaded: {vectorDb.GetChunkCount()} chunks available");

            // Initialize chatbot
            System.Console.WriteLine("\n🤖 Initializing chatbot...");
            IRAGChatbotService chatbot;
            
            if (settings.AIProvider.Equals("HuggingFace", StringComparison.OrdinalIgnoreCase))
            {
                var hfService = new HuggingFaceService(
                    settings.HuggingFace.ApiKey,
                    settings.HuggingFace.EmbeddingModel,
                    settings.HuggingFace.Model
                );
                chatbot = new HuggingFaceRAGChatbotService(
                    vectorDb,
                    hfService,
                    settings.HuggingFace.Temperature,
                    settings.HuggingFace.MaxTokens
                );
            }
            else
            {
                chatbot = new RAGChatbotService(
                    vectorDb,
                    settings.OpenAI.ApiKey,
                    settings.OpenAI.Model,
                    settings.OpenAI.Temperature
                );
            }
            System.Console.WriteLine("✅ Chatbot ready!");

            System.Console.WriteLine("\n═══════════════════════════════════════════════════════════");
            System.Console.WriteLine("Ask questions about the YouTube videos!");
            System.Console.WriteLine("Type 'exit' to quit");
            System.Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            // Chat loop
            while (true)
            {
                System.Console.Write("You: ");
                var question = System.Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(question))
                {
                    continue;
                }

                if (question.ToLower() is "exit" or "quit" or "bye")
                {
                    System.Console.WriteLine("\n👋 Goodbye! Thanks for using the RAG chatbot.");
                    break;
                }

                System.Console.WriteLine("\n🤔 Thinking...\n");

                try
                {
                    var response = await chatbot.ChatAsync(question, true);
                    System.Console.WriteLine($"Bot: {response}\n");
                    System.Console.WriteLine("─────────────────────────────────────────────────────────────\n");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"❌ Error: {ex.Message}\n");
                }
            }
        }
        catch (FileNotFoundException)
        {
            System.Console.WriteLine("❌ Vector database not found.");
            System.Console.WriteLine("\nPlease run setup first:");
            System.Console.WriteLine("   dotnet run --project YouTubeRAGChatbot.Console setup VIDEO_ID1,VIDEO_ID2");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
