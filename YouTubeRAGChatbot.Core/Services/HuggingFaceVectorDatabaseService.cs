using YouTubeRAGChatbot.Core.Models;
using System.Text.Json;

namespace YouTubeRAGChatbot.Core.Services;

public class HuggingFaceVectorDatabaseService : IVectorDatabaseService
{
    private readonly IHuggingFaceService _huggingFaceService;
    private List<(TextChunk chunk, float[] embedding)> _vectorStore = new();

    public HuggingFaceVectorDatabaseService(IHuggingFaceService huggingFaceService)
    {
        _huggingFaceService = huggingFaceService;
    }

    public Task InitializeAsync()
    {
        _vectorStore.Clear();
        Console.WriteLine("✓ Vector database initialized");
        return Task.CompletedTask;
    }

    public async Task AddChunksAsync(List<TextChunk> chunks)
    {
        Console.WriteLine($"Adding {chunks.Count} chunks to vector database...");
        
        foreach (var chunk in chunks)
        {
            var embedding = await _huggingFaceService.GenerateEmbeddingAsync(chunk.Content);
            _vectorStore.Add((chunk, embedding));
        }
        
        Console.WriteLine($"✓ Added {chunks.Count} chunks to vector database");
    }

    public async Task<List<(TextChunk chunk, double score)>> SearchAsync(string query, int topK = 4)
    {
        if (_vectorStore.Count == 0)
        {
            Console.WriteLine("⚠️ Warning: Vector store is empty!");
            return new List<(TextChunk, double)>();
        }

        Console.WriteLine($"🔍 Searching {_vectorStore.Count} chunks for: {query}");

        // Generate embedding for query
        float[] queryEmbedding;
        try
        {
            queryEmbedding = await _huggingFaceService.GenerateEmbeddingAsync(query);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating query embedding: {ex.Message}", ex);
        }
        
        // Calculate cosine similarity with all chunks
        var similarities = _vectorStore
            .Select(item => (
                chunk: item.chunk,
                score: CosineSimilarity(queryEmbedding, item.embedding)
            ))
            .OrderByDescending(x => x.score)
            .Take(topK)
            .ToList();

        return similarities;
    }

    public async Task SaveToFileAsync(string directory)
    {
        Directory.CreateDirectory(directory);
        
        var data = _vectorStore.Select(item => new
        {
            Chunk = item.chunk,
            Embedding = item.embedding
        }).ToList();

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        var filePath = Path.Combine(directory, "vectordb.json");
        
        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"✓ Vector database saved to {filePath}");
    }

    public async Task LoadFromFileAsync(string directory)
    {
        var filePath = Path.Combine(directory, "vectordb.json");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Vector database file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<List<VectorStoreItem>>(json);

        if (data != null)
        {
            _vectorStore = data.Select(item => 
                (item.Chunk, item.Embedding)
            ).ToList();
            
            Console.WriteLine($"✓ Vector database loaded from {filePath} ({_vectorStore.Count} chunks)");
        }
    }

    public int GetChunkCount()
    {
        return _vectorStore.Count;
    }

    private double CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must have the same length");
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        magnitudeA = Math.Sqrt(magnitudeA);
        magnitudeB = Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }

    private class VectorStoreItem
    {
        public TextChunk Chunk { get; set; } = new();
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
