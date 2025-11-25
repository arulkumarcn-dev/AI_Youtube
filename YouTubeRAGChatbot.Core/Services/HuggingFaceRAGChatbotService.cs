using YouTubeRAGChatbot.Core.Models;
using System.Text;

namespace YouTubeRAGChatbot.Core.Services;

public class HuggingFaceRAGChatbotService : IRAGChatbotService
{
    private readonly IVectorDatabaseService _vectorDatabase;
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly double _temperature;
    private readonly int _maxTokens;

    public HuggingFaceRAGChatbotService(
        IVectorDatabaseService vectorDatabase,
        IHuggingFaceService huggingFaceService,
        double temperature = 0.7,
        int maxTokens = 1000)
    {
        _vectorDatabase = vectorDatabase;
        _huggingFaceService = huggingFaceService;
        _temperature = temperature;
        _maxTokens = maxTokens;
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

        // Create prompt for Hugging Face models (using instruction format)
        var prompt = $@"<s>[INST] You are a helpful AI assistant that answers questions based ONLY on the provided context from YouTube video transcripts.

Context from transcripts:
{contextBuilder}

Question: {question}

Instructions:
- Answer the question using ONLY the information from the provided context
- If the answer is not in the context, say ""I cannot find this information in the available transcripts""
- Be specific and cite relevant parts of the transcript when possible
- Include video IDs when mentioning information from specific videos
- Do not make up information or use external knowledge

Answer: [/INST]";

        string answer;
        try
        {
            answer = await _huggingFaceService.GenerateChatCompletionAsync(prompt, _temperature, _maxTokens);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error calling Hugging Face API: {ex.Message}", ex);
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
            Answer = answer,
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
