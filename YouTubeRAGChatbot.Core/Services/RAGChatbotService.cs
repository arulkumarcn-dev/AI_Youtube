using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using YouTubeRAGChatbot.Core.Models;
using System.Text;

namespace YouTubeRAGChatbot.Core.Services;

public interface IRAGChatbotService
{
    Task<ChatResponse> AskAsync(string question);
    Task<string> ChatAsync(string question, bool includeSource = true);
}

public class RAGChatbotService : IRAGChatbotService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly IVectorDatabaseService _vectorDatabase;
    private readonly OpenAIPromptExecutionSettings _executionSettings;

    public RAGChatbotService(
        IVectorDatabaseService vectorDatabase,
        string apiKey,
        string modelName = "gpt-3.5-turbo",
        double temperature = 0.3)
    {
        _vectorDatabase = vectorDatabase;

        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(modelName, apiKey);
        _kernel = builder.Build();

        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
        
        _executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = temperature,
            MaxTokens = 1000
        };
    }

    public async Task<ChatResponse> AskAsync(string question)
    {
        // Retrieve relevant chunks
        var relevantChunks = await _vectorDatabase.SearchAsync(question, topK: 4);

        if (relevantChunks.Count == 0)
        {
            return new ChatResponse
            {
                Answer = "I don't have any relevant information to answer this question.",
                Sources = new List<SourceReference>()
            };
        }

        // Build context from retrieved chunks
        var contextBuilder = new StringBuilder();
        foreach (var (chunk, _) in relevantChunks)
        {
            contextBuilder.AppendLine($"[Video {chunk.VideoId}]: {chunk.Content}");
            contextBuilder.AppendLine();
        }

        // Create prompt
        var prompt = $@"You are a helpful AI assistant that answers questions based ONLY on the provided context from YouTube video transcripts.

Context from transcripts:
{contextBuilder}

Question: {question}

Instructions:
- Answer the question using ONLY the information from the provided context
- If the answer is not in the context, say ""I cannot find this information in the available transcripts""
- Be specific and cite relevant parts of the transcript when possible
- Include video IDs when mentioning information from specific videos
- Do not make up information or use external knowledge

Answer:";

        // Get completion from LLM
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        ChatMessageContent result;
        try
        {
            result = await _chatService.GetChatMessageContentAsync(chatHistory, _executionSettings, _kernel);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Network error connecting to OpenAI API. This could be due to:\n" +
                              $"1. Invalid API key\n" +
                              $"2. Firewall/proxy blocking the connection\n" +
                              $"3. Network connectivity issues\n" +
                              $"Original error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error calling OpenAI API: {ex.Message}\n" +
                              $"Please check your API key and network connection.", ex);
        }
        
        // Build response with sources
        var sources = relevantChunks.Select(item => new SourceReference
        {
            VideoId = item.chunk.VideoId,
            ChunkIndex = item.chunk.ChunkIndex,
            Content = item.chunk.Content.Length > 200 
                ? item.chunk.Content.Substring(0, 200) + "..." 
                : item.chunk.Content,
            SimilarityScore = item.score
        }).ToList();

        return new ChatResponse
        {
            Answer = result.Content ?? string.Empty,
            Sources = sources
        };
    }

    public async Task<string> ChatAsync(string question, bool includeSources = true)
    {
        var response = await AskAsync(question);
        
        if (!includeSources || response.Sources.Count == 0)
        {
            return response.Answer;
        }

        var resultBuilder = new StringBuilder();
        resultBuilder.AppendLine(response.Answer);
        resultBuilder.AppendLine();
        resultBuilder.AppendLine("📚 Sources:");
        
        for (int i = 0; i < response.Sources.Count; i++)
        {
            var source = response.Sources[i];
            resultBuilder.AppendLine($"{i + 1}. Video ID: {source.VideoId}, Chunk: {source.ChunkIndex}");
            resultBuilder.AppendLine($"   Similarity: {source.SimilarityScore:F3}");
            resultBuilder.AppendLine($"   URL: https://www.youtube.com/watch?v={source.VideoId}");
        }

        return resultBuilder.ToString();
    }
}
