# 🤖 YouTube Video Q&A RAG Chatbot - .NET Edition

A Retrieval-Augmented Generation (RAG) based chatbot built with .NET 8.0 that answers questions based on YouTube video transcripts. Uses Microsoft Semantic Kernel, OpenAI, and Blazor.

## ✨ Features

- **YouTube Transcript Fetching**: Download transcripts using YoutubeExplode
- **Intelligent Text Chunking**: Split transcripts for optimal processing
- **Vector Database**: Efficient embedding storage with cosine similarity search
- **Microsoft Semantic Kernel**: Modern AI orchestration
- **OpenAI Integration**: GPT models for chat and embeddings
- **Console Application**: Interactive chat loop (runs until 'exit')
- **Blazor Web UI**: Beautiful, responsive web interface
- **Source Citations**: Shows which video chunks were used
- **Persistent Storage**: JSON-based vector database

## 📋 Requirements

- .NET 8.0 SDK or later
- OpenAI API Key
- YouTube videos with available captions/transcripts

## 🚀 Installation

### 1. Install .NET 8.0 SDK

Download from: https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```powershell
dotnet --version
```

### 2. Clone or Download the Project

```powershell
cd d:\AI
```

### 3. Restore NuGet Packages

```powershell
dotnet restore YouTubeRAGChatbot.sln
```

### 4. Configure API Key

Edit `appsettings.json` in both Console and Web projects:

**YouTubeRAGChatbot.Console/appsettings.json**
**YouTubeRAGChatbot.Web/appsettings.json**

```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key-here",
    "Model": "gpt-3.5-turbo",
    "EmbeddingModel": "text-embedding-ada-002",
    "Temperature": 0.3
  }
}
```

## 📖 Usage

### Option 1: Console Application (Recommended for First Time)

#### Setup Mode - Add Videos

```powershell
# Navigate to solution directory
cd d:\AI

# Run setup with video IDs
dotnet run --project YouTubeRAGChatbot.Console setup VIDEO_ID1,VIDEO_ID2,VIDEO_ID3
```

Example:
```powershell
dotnet run --project YouTubeRAGChatbot.Console setup dQw4w9WgXcQ,9bZkp7q19f0
```

Or run without arguments for interactive mode:
```powershell
dotnet run --project YouTubeRAGChatbot.Console setup
```

#### Chat Mode

```powershell
dotnet run --project YouTubeRAGChatbot.Console
```

Type your questions and press Enter. Type `exit`, `quit`, or `bye` to quit.

### Option 2: Blazor Web Application

#### Run the Web App

```powershell
dotnet run --project YouTubeRAGChatbot.Web
```

Then open your browser to: `https://localhost:5001` or `http://localhost:5000`

#### Using the Web Interface

1. **Add Videos Tab**: Enter YouTube video URLs or IDs, click "Add Videos to Database"
2. **Chat Tab**: Ask questions about the videos
3. **Database Info Tab**: View statistics and configured videos

## 📁 Solution Structure

```
YouTubeRAGChatbot/
├── YouTubeRAGChatbot.sln              # Solution file
│
├── YouTubeRAGChatbot.Core/            # Core library (shared)
│   ├── Models/
│   │   └── TranscriptData.cs         # Data models
│   ├── Configuration/
│   │   └── AppSettings.cs            # Configuration classes
│   └── Services/
│       ├── TranscriptFetcherService.cs   # YouTube transcript fetching
│       ├── TextChunkerService.cs         # Text chunking
│       ├── VectorDatabaseService.cs      # Vector storage & search
│       └── RAGChatbotService.cs          # RAG chatbot logic
│
├── YouTubeRAGChatbot.Console/         # Console application
│   ├── Program.cs                     # Console app with chat loop
│   └── appsettings.json              # Configuration
│
└── YouTubeRAGChatbot.Web/             # Blazor web application
    ├── Program.cs                     # Web app startup
    ├── appsettings.json              # Configuration
    ├── Components/
    │   ├── App.razor                 # Root component
    │   ├── Routes.razor              # Routing
    │   ├── Layout/
    │   │   └── MainLayout.razor      # Main layout
    │   └── Pages/
    │       ├── Chat.razor            # Chat interface
    │       ├── AddVideos.razor       # Add videos page
    │       └── Database.razor        # Database info page
    └── wwwroot/
        └── app.css                   # Styles
```

## 🎯 How It Works

1. **Transcript Fetching**: Downloads YouTube transcripts using YoutubeExplode library
2. **Text Chunking**: Splits transcripts into 1000-character chunks with 200-character overlap
3. **Embeddings**: Generates embeddings using OpenAI's `text-embedding-ada-002` model
4. **Vector Storage**: Stores embeddings in JSON format with cosine similarity search
5. **RAG Pipeline**: 
   - Retrieves top 4 most relevant chunks for user questions
   - Sends chunks as context to GPT model
   - LLM generates answer based only on provided context
6. **Response**: Returns answer with source citations (video IDs and chunk indices)

## 💡 Example Usage

### Console Application

```
You: What is the main topic discussed in the video?

🤔 Thinking...

Bot: [Answer based on transcript content]

📚 Sources:
1. Video ID: dQw4w9WgXcQ, Chunk: 0
   Similarity: 0.892
   URL: https://www.youtube.com/watch?v=dQw4w9WgXcQ

You: exit

👋 Goodbye! Thanks for using the RAG chatbot.
```

### Web Application

1. Navigate to **Add Videos** tab
2. Enter: `https://www.youtube.com/watch?v=dQw4w9WgXcQ`
3. Click "Add Videos to Database"
4. Go to **Chat** tab
5. Ask: "What is this video about?"
6. Get AI-powered response with sources!

## ⚙️ Configuration

Edit `appsettings.json`:

| Setting | Description | Default |
|---------|-------------|---------|
| `OpenAI.ApiKey` | Your OpenAI API key | (required) |
| `OpenAI.Model` | GPT model for chat | gpt-3.5-turbo |
| `OpenAI.EmbeddingModel` | Embedding model | text-embedding-ada-002 |
| `OpenAI.Temperature` | Response randomness (0-1) | 0.3 |
| `RAG.ChunkSize` | Text chunk size | 1000 |
| `RAG.ChunkOverlap` | Chunk overlap | 200 |
| `RAG.TopK` | Number of chunks to retrieve | 4 |
| `Storage.TranscriptDirectory` | Transcript storage path | ./transcripts |
| `Storage.VectorDbDirectory` | Vector DB storage path | ./vectordb |

## 🔧 Building the Project

### Build All Projects

```powershell
dotnet build YouTubeRAGChatbot.sln
```

### Build Specific Project

```powershell
dotnet build YouTubeRAGChatbot.Console
dotnet build YouTubeRAGChatbot.Web
```

### Publish for Deployment

```powershell
# Console app
dotnet publish YouTubeRAGChatbot.Console -c Release -o ./publish/console

# Web app
dotnet publish YouTubeRAGChatbot.Web -c Release -o ./publish/web
```

## 🐛 Troubleshooting

### "No captions available"
- Video doesn't have transcripts enabled
- Try a different video or enable captions

### "The type or namespace name 'SemanticKernel' does not exist"
- Run: `dotnet restore YouTubeRAGChatbot.sln`
- Ensure NuGet packages are installed

### "Vector database not found"
- Run setup first: `dotnet run --project YouTubeRAGChatbot.Console setup`
- Or add videos through the web interface

### Build errors
- Verify .NET 8.0 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean; dotnet build`

## 📦 NuGet Packages Used

### Core Library
- `Microsoft.SemanticKernel` (1.0.1) - AI orchestration
- `Microsoft.SemanticKernel.Connectors.OpenAI` (1.0.1) - OpenAI integration
- `YoutubeExplode` (6.3.16) - YouTube transcript fetching
- `Newtonsoft.Json` (13.0.3) - JSON serialization

### Console Application
- `Microsoft.Extensions.Configuration` - Configuration management
- `Microsoft.Extensions.Configuration.Json` - JSON config support

### Web Application
- ASP.NET Core 8.0 - Web framework
- Blazor Server - Interactive UI components

## 🎓 Architecture Highlights

- **Clean Architecture**: Core business logic separated from UI
- **Dependency Injection**: Services registered and injected
- **Async/Await**: Fully asynchronous operations
- **Semantic Kernel**: Modern AI SDK from Microsoft
- **Blazor Server**: Real-time interactive UI without JavaScript
- **RAG Pattern**: Retrieval-Augmented Generation for accuracy

## 🤝 Extending the Project

Ideas for enhancement:
- Add Azure OpenAI support
- Implement conversation history
- Add Pinecone or Qdrant vector database
- Support multiple languages
- Add video timestamp citations
- Implement user authentication
- Add Redis caching for performance

## 📄 License

This project is for educational purposes. Ensure compliance with:
- YouTube Terms of Service
- OpenAI API Terms
- Video transcript usage rights

## 🆘 Support

For issues:
1. Check the Troubleshooting section
2. Verify API keys are correct
3. Ensure .NET 8.0 SDK is installed
4. Check videos have transcripts available

## 🎉 Quick Start Commands

```powershell
# 1. Restore packages
dotnet restore

# 2. Edit appsettings.json with your API key

# 3. Setup with videos
dotnet run --project YouTubeRAGChatbot.Console setup VIDEO_ID1,VIDEO_ID2

# 4. Run console chat
dotnet run --project YouTubeRAGChatbot.Console

# OR run web app
dotnet run --project YouTubeRAGChatbot.Web
```

---

**Built with ❤️ using .NET 8.0, Microsoft Semantic Kernel, OpenAI, and Blazor**
