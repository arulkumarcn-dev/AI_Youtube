using YouTubeRAGChatbot.Core.Models;

namespace YouTubeRAGChatbot.Core.Services;

public interface ITextChunkerService
{
    List<TextChunk> ChunkText(string text, string videoId, int chunkSize = 1000, int overlap = 200);
    List<TextChunk> ChunkTranscript(TranscriptData transcript, int chunkSize = 1000, int overlap = 200);
    List<TextChunk> ChunkMultipleTranscripts(List<TranscriptData> transcripts, int chunkSize = 1000, int overlap = 200);
}

public class TextChunkerService : ITextChunkerService
{
    public List<TextChunk> ChunkText(string text, string videoId, int chunkSize = 1000, int overlap = 200)
    {
        var chunks = new List<TextChunk>();
        
        if (string.IsNullOrWhiteSpace(text))
        {
            return chunks;
        }

        int startIndex = 0;
        int chunkIndex = 0;

        while (startIndex < text.Length)
        {
            // Determine chunk end
            int endIndex = Math.Min(startIndex + chunkSize, text.Length);
            
            // Try to break at sentence or word boundary
            if (endIndex < text.Length)
            {
                // Look for sentence ending
                int lastPeriod = text.LastIndexOf(". ", endIndex, Math.Min(endIndex - startIndex, 100), StringComparison.Ordinal);
                if (lastPeriod > startIndex)
                {
                    endIndex = lastPeriod + 1;
                }
                else
                {
                    // Look for space
                    int lastSpace = text.LastIndexOf(' ', endIndex - 1, Math.Min(endIndex - startIndex, 50));
                    if (lastSpace > startIndex)
                    {
                        endIndex = lastSpace;
                    }
                }
            }

            // Extract chunk
            string chunkContent = text.Substring(startIndex, endIndex - startIndex).Trim();
            
            if (!string.IsNullOrWhiteSpace(chunkContent))
            {
                chunks.Add(new TextChunk
                {
                    Content = chunkContent,
                    ChunkIndex = chunkIndex,
                    VideoId = videoId,
                    Metadata = new Dictionary<string, object>
                    {
                        ["video_id"] = videoId,
                        ["chunk_id"] = chunkIndex,
                        ["start_position"] = startIndex,
                        ["end_position"] = endIndex
                    }
                });
                
                chunkIndex++;
            }

            // Move to next chunk with overlap
            startIndex = endIndex - overlap;
            
            // Ensure we make progress
            if (startIndex <= 0 || startIndex >= text.Length)
            {
                startIndex = endIndex;
            }
        }

        return chunks;
    }

    public List<TextChunk> ChunkTranscript(TranscriptData transcript, int chunkSize = 1000, int overlap = 200)
    {
        var chunks = ChunkText(transcript.TranscriptText, transcript.VideoId, chunkSize, overlap);
        
        // Add additional metadata
        foreach (var chunk in chunks)
        {
            chunk.Metadata["title"] = transcript.Title;
            chunk.Metadata["duration"] = transcript.Duration.ToString();
            chunk.Metadata["url"] = $"https://www.youtube.com/watch?v={transcript.VideoId}";
        }
        
        Console.WriteLine($"✓ Created {chunks.Count} chunks from video {transcript.VideoId}");
        
        return chunks;
    }

    public List<TextChunk> ChunkMultipleTranscripts(List<TranscriptData> transcripts, int chunkSize = 1000, int overlap = 200)
    {
        var allChunks = new List<TextChunk>();
        
        foreach (var transcript in transcripts)
        {
            var chunks = ChunkTranscript(transcript, chunkSize, overlap);
            allChunks.AddRange(chunks);
        }
        
        Console.WriteLine($"\n✓ Total chunks created: {allChunks.Count}");
        
        return allChunks;
    }
}
