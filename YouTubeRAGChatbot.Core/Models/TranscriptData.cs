namespace YouTubeRAGChatbot.Core.Models;

public class TranscriptData
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TranscriptText { get; set; } = string.Empty;
    public List<TranscriptSegment> Segments { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public DateTime FetchedAt { get; set; }
}

public class TranscriptSegment
{
    public string Text { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan Duration { get; set; }
}

public class TextChunk
{
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public int ChunkIndex { get; set; }
    public string VideoId { get; set; } = string.Empty;
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<SourceReference> Sources { get; set; } = new();
}

public class SourceReference
{
    public string VideoId { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
}
