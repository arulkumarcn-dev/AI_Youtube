using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using YouTubeRAGChatbot.Core.Models;

namespace YouTubeRAGChatbot.Core.Services;

public interface IHuggingFaceService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<string> GenerateChatCompletionAsync(string prompt, double temperature = 0.7, int maxTokens = 1000);
}

public class HuggingFaceService : IHuggingFaceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _embeddingModel;
    private readonly string _chatModel;

    public HuggingFaceService(string apiKey, string embeddingModel, string chatModel)
    {
        _apiKey = apiKey;
        _embeddingModel = embeddingModel;
        _chatModel = chatModel;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var url = $"https://api-inference.huggingface.co/models/{_embeddingModel}";
            
            var request = new
            {
                inputs = text
            };

            var response = await _httpClient.PostAsJsonAsync(url, request);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Hugging Face API error ({response.StatusCode}):\n{responseContent}\n\n" +
                                  $"Common issues:\n" +
                                  $"1. Invalid API key (check format: hf_...)\n" +
                                  $"2. Model '{_embeddingModel}' not accessible\n" +
                                  $"3. Rate limit exceeded\n" +
                                  $"4. Model is loading (wait 20-60 seconds and retry)");
            }

            try
            {
                var embedding = JsonSerializer.Deserialize<float[]>(responseContent);
                
                if (embedding == null || embedding.Length == 0)
                {
                    throw new Exception($"Received empty embedding. Response: {responseContent}");
                }

                Console.WriteLine($"  Generated embedding with {embedding.Length} dimensions");
                return embedding;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse embedding response. Got: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...\n\nError: {ex.Message}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Network error connecting to Hugging Face API: {ex.Message}\n" +
                              $"Please check:\n" +
                              $"1. Your Hugging Face API key is valid\n" +
                              $"2. You have internet connectivity\n" +
                              $"3. The model '{_embeddingModel}' is accessible", ex);
        }
    }

    public async Task<string> GenerateChatCompletionAsync(string prompt, double temperature = 0.7, int maxTokens = 1000)
    {
        try
        {
            var url = $"https://api-inference.huggingface.co/models/{_chatModel}";
            
            var request = new
            {
                inputs = prompt,
                parameters = new
                {
                    temperature = temperature,
                    max_new_tokens = maxTokens,
                    return_full_text = false
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Hugging Face API error: {response.StatusCode} - {error}");
            }

            var responseText = await response.Content.ReadAsStringAsync();
            
            // Parse the response - HF returns array of objects with 'generated_text'
            using var doc = JsonDocument.Parse(responseText);
            
            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var firstElement = doc.RootElement[0];
                if (firstElement.TryGetProperty("generated_text", out var generatedText))
                {
                    return generatedText.GetString() ?? string.Empty;
                }
            }

            return responseText; // Fallback to raw response
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Network error connecting to Hugging Face API: {ex.Message}\n" +
                              $"Please check:\n" +
                              $"1. Your Hugging Face API key is valid\n" +
                              $"2. You have internet connectivity\n" +
                              $"3. The model '{_chatModel}' is accessible", ex);
        }
    }
}
