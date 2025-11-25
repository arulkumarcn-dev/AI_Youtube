using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using YouTubeRAGChatbot.Core.Models;

// Create sample vector database with proper structure
var random = new Random(42); // Fixed seed for consistent embeddings

float[] GenerateRandomEmbedding(int size = 1536)
{
    var embedding = new float[size];
    for (int i = 0; i < size; i++)
    {
        embedding[i] = (float)(random.NextDouble() * 2 - 1); // Range: -1 to 1
    }
    // Normalize the vector
    var magnitude = 0.0;
    foreach (var val in embedding)
        magnitude += val * val;
    magnitude = Math.Sqrt(magnitude);
    for (int i = 0; i < size; i++)
        embedding[i] = (float)(embedding[i] / magnitude);
    return embedding;
}

var vectorStoreItems = new List<object>
{
    new
    {
        Chunk = new TextChunk
        {
            Content = "Artificial Intelligence, or AI, is a branch of computer science that aims to create machines capable of intelligent behavior. AI systems can learn from experience, adjust to new inputs, and perform human-like tasks. This revolutionary field combines various disciplines including mathematics, logic, psychology, and linguistics.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE001",
                ["VideoTitle"] = "Introduction to Artificial Intelligence",
                ["StartTime"] = "00:00:00"
            },
            ChunkIndex = 0,
            VideoId = "SAMPLE001"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Machine Learning is a subset of AI that focuses on the development of algorithms that can learn from and make predictions based on data. It uses statistical techniques to enable computer systems to learn without being explicitly programmed. ML models improve their performance as they are exposed to more data over time.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE001",
                ["VideoTitle"] = "Introduction to Artificial Intelligence",
                ["StartTime"] = "00:00:25"
            },
            ChunkIndex = 1,
            VideoId = "SAMPLE001"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Deep Learning is a specialized form of Machine Learning that uses neural networks with multiple layers. These deep neural networks can automatically discover intricate patterns in large datasets, making them particularly effective for tasks like image recognition, natural language processing, and speech recognition.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE001",
                ["VideoTitle"] = "Introduction to Artificial Intelligence",
                ["StartTime"] = "00:00:50"
            },
            ChunkIndex = 2,
            VideoId = "SAMPLE001"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Neural networks are computing systems inspired by biological neural networks in animal brains. They consist of interconnected nodes or neurons organized in layers. The input layer receives data, hidden layers process it through weighted connections, and the output layer produces results. Each connection has a weight adjusted during training.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE002",
                ["VideoTitle"] = "Understanding Neural Networks",
                ["StartTime"] = "00:00:00"
            },
            ChunkIndex = 0,
            VideoId = "SAMPLE002"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Training a neural network involves adjusting weights and biases to minimize the difference between predicted and actual outputs. This is done using backpropagation and gradient descent algorithms. The network learns by iteratively processing training examples and updating parameters based on errors.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE002",
                ["VideoTitle"] = "Understanding Neural Networks",
                ["StartTime"] = "00:00:30"
            },
            ChunkIndex = 1,
            VideoId = "SAMPLE002"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Natural Language Processing, or NLP, is a field of AI focusing on interaction between computers and human language. It enables machines to read, understand, and derive meaning from human languages. NLP combines computational linguistics with machine learning and deep learning to process natural language data.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE003",
                ["VideoTitle"] = "Natural Language Processing Fundamentals",
                ["StartTime"] = "00:00:00"
            },
            ChunkIndex = 0,
            VideoId = "SAMPLE003"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Modern NLP systems use tokenization, named entity recognition, sentiment analysis, and language translation. Large Language Models like GPT have revolutionized NLP by understanding context and generating human-like text. These models are trained on vast amounts of text data and can perform various language tasks.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE003",
                ["VideoTitle"] = "Natural Language Processing Fundamentals",
                ["StartTime"] = "00:00:28"
            },
            ChunkIndex = 1,
            VideoId = "SAMPLE003"
        },
        Embedding = GenerateRandomEmbedding()
    },
    new
    {
        Chunk = new TextChunk
        {
            Content = "Computer Vision is an AI field that trains computers to interpret visual information. It involves acquiring, processing, analyzing, and understanding digital images and videos. Computer vision tasks include image classification, object detection, facial recognition, and image segmentation using deep learning techniques.",
            Metadata = new Dictionary<string, object>
            {
                ["VideoId"] = "SAMPLE004",
                ["VideoTitle"] = "Computer Vision and Image Recognition",
                ["StartTime"] = "00:00:00"
            },
            ChunkIndex = 0,
            VideoId = "SAMPLE004"
        },
        Embedding = GenerateRandomEmbedding()
    }
};

Directory.CreateDirectory("YouTubeRAGChatbot.Web/vectordb");
var json = JsonSerializer.Serialize(vectorStoreItems, new JsonSerializerOptions 
{ 
    WriteIndented = true 
});
File.WriteAllText("YouTubeRAGChatbot.Web/vectordb/vectordb.json", json);

Console.WriteLine("✅ Created sample vector database with 8 chunks");
Console.WriteLine($"   Each embedding has 1536 dimensions");
Console.WriteLine($"   File: YouTubeRAGChatbot.Web/vectordb/vectordb.json");
