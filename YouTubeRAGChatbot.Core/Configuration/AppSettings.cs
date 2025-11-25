namespace YouTubeRAGChatbot.Core.Configuration;

public class AppSettings
{
    public string AIProvider { get; set; } = "OpenAI"; // "OpenAI" or "HuggingFace"
    public OpenAISettings OpenAI { get; set; } = new();
    public HuggingFaceSettings HuggingFace { get; set; } = new();
    public RAGSettings RAG { get; set; } = new();
    public StorageSettings Storage { get; set; } = new();
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-3.5-turbo";
    public string EmbeddingModel { get; set; } = "text-embedding-ada-002";
    public double Temperature { get; set; } = 0.3;
}

public class HuggingFaceSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "mistralai/Mistral-7B-Instruct-v0.2";
    public string EmbeddingModel { get; set; } = "sentence-transformers/all-MiniLM-L6-v2";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 1000;
}

public class RAGSettings
{
    public int ChunkSize { get; set; } = 1000;
    public int ChunkOverlap { get; set; } = 200;
    public int TopK { get; set; } = 4;
}

public class StorageSettings
{
    public string TranscriptDirectory { get; set; } = "./transcripts";
    public string VectorDbDirectory { get; set; } = "./vectordb";
}
