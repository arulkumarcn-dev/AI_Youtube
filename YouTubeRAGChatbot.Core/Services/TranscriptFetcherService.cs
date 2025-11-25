using YoutubeExplode;
using YouTubeRAGChatbot.Core.Models;
using System.Text;

namespace YouTubeRAGChatbot.Core.Services;

public interface ITranscriptFetcherService
{
    Task<TranscriptData> FetchTranscriptAsync(string videoIdOrUrl);
    Task<List<TranscriptData>> FetchMultipleTranscriptsAsync(IEnumerable<string> videoIdsOrUrls);
    Task SaveTranscriptAsync(TranscriptData transcript, string directory);
    Task<TranscriptData> LoadTranscriptAsync(string videoId, string directory);
}

public class TranscriptFetcherService : ITranscriptFetcherService
{
    private readonly YoutubeClient _youtubeClient;

    public TranscriptFetcherService()
    {
        _youtubeClient = new YoutubeClient();
    }

    public async Task<TranscriptData> FetchTranscriptAsync(string videoIdOrUrl)
    {
        try
        {
            // Parse video ID from URL or use directly
            var videoId = ExtractVideoId(videoIdOrUrl);
            Console.WriteLine($"  Fetching video info for: {videoId}");
            
            // Get video metadata
            var video = await _youtubeClient.Videos.GetAsync(videoId);
            Console.WriteLine($"  Video title: {video.Title}");
            
            // Get closed captions - try English first, then any language
            Console.WriteLine($"  Fetching caption tracks...");
            var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoId);
            Console.WriteLine($"  Found {trackManifest.Tracks.Count} caption tracks");
            
            // Try to get English track
            var trackInfo = trackManifest.Tracks.FirstOrDefault(t => t.Language.Code.StartsWith("en"));
            
            // If no English, take the first available
            trackInfo ??= trackManifest.Tracks.FirstOrDefault();
            
            if (trackInfo == null)
            {
                throw new Exception($"No captions available for video: {videoId}");
            }
            
            Console.WriteLine($"  Using captions: {trackInfo.Language.Name}");

            // Get the actual closed caption track
            var track = await _youtubeClient.Videos.ClosedCaptions.GetAsync(trackInfo);
            
            // Build transcript data
            var transcriptData = new TranscriptData
            {
                VideoId = videoId,
                Title = video.Title,
                Duration = video.Duration ?? TimeSpan.Zero,
                FetchedAt = DateTime.UtcNow
            };

            var fullTextBuilder = new StringBuilder();
            
            foreach (var caption in track.Captions)
            {
                transcriptData.Segments.Add(new TranscriptSegment
                {
                    Text = caption.Text,
                    StartTime = caption.Offset,
                    Duration = caption.Duration
                });
                
                fullTextBuilder.Append(caption.Text).Append(" ");
            }

            transcriptData.TranscriptText = fullTextBuilder.ToString().Trim();
            
            Console.WriteLine($"✓ Fetched transcript for: {video.Title} ({videoId})");
            
            return transcriptData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error fetching transcript: {ex.Message}");
            throw;
        }
    }

    public async Task<List<TranscriptData>> FetchMultipleTranscriptsAsync(IEnumerable<string> videoIdsOrUrls)
    {
        var transcripts = new List<TranscriptData>();
        var videoList = videoIdsOrUrls.ToList();
        
        Console.WriteLine($"\nFetching transcripts for {videoList.Count} videos...");
        
        for (int i = 0; i < videoList.Count; i++)
        {
            try
            {
                Console.WriteLine($"[{i + 1}/{videoList.Count}] Processing: {videoList[i]}");
                var transcript = await FetchTranscriptAsync(videoList[i]);
                transcripts.Add(transcript);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipping video due to error: {ex.Message}");
            }
        }
        
        Console.WriteLine($"\n✓ Successfully fetched {transcripts.Count} transcripts");
        return transcripts;
    }

    public async Task SaveTranscriptAsync(TranscriptData transcript, string directory)
    {
        Directory.CreateDirectory(directory);
        
        var filePath = Path.Combine(directory, $"{transcript.VideoId}.txt");
        await File.WriteAllTextAsync(filePath, transcript.TranscriptText);
        
        var metadataPath = Path.Combine(directory, $"{transcript.VideoId}_metadata.json");
        var metadata = new
        {
            transcript.VideoId,
            transcript.Title,
            Duration = transcript.Duration.ToString(),
            transcript.FetchedAt,
            SegmentCount = transcript.Segments.Count
        };
        
        await File.WriteAllTextAsync(metadataPath, 
            System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
        
        Console.WriteLine($"✓ Saved transcript: {filePath}");
    }

    public async Task<TranscriptData> LoadTranscriptAsync(string videoId, string directory)
    {
        var filePath = Path.Combine(directory, $"{videoId}.txt");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Transcript file not found: {filePath}");
        }

        var text = await File.ReadAllTextAsync(filePath);
        
        return new TranscriptData
        {
            VideoId = videoId,
            TranscriptText = text,
            FetchedAt = File.GetCreationTimeUtc(filePath)
        };
    }

    private string ExtractVideoId(string videoIdOrUrl)
    {
        if (videoIdOrUrl.Contains("youtube.com") || videoIdOrUrl.Contains("youtu.be"))
        {
            var uri = new Uri(videoIdOrUrl);
            
            if (videoIdOrUrl.Contains("youtube.com/watch"))
            {
                // Parse query string manually
                var query = uri.Query.TrimStart('?');
                var parameters = query.Split('&');
                foreach (var param in parameters)
                {
                    var keyValue = param.Split('=');
                    if (keyValue.Length == 2 && keyValue[0] == "v")
                    {
                        return keyValue[1];
                    }
                }
                throw new Exception("Invalid YouTube URL - no video ID found");
            }
            else if (videoIdOrUrl.Contains("youtu.be/"))
            {
                return uri.AbsolutePath.TrimStart('/').Split('?')[0];
            }
        }
        
        return videoIdOrUrl; // Assume it's already a video ID
    }
}
