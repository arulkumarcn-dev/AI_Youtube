# 🤖 YouTube Video Q&A RAG Chatbot

A Retrieval-Augmented Generation (RAG) based chatbot that answers questions based on YouTube video transcripts. The bot provides accurate, context-based answers using AI and only responds from the available transcript content.

## ✨ Features

- **YouTube Transcript Fetching**: Automatically download transcripts from YouTube videos
- **Text Chunking**: Intelligent splitting of transcripts for optimal processing
- **Vector Database**: Efficient storage and retrieval using ChromaDB
- **Multiple LLM Support**: Works with OpenAI GPT and Google Gemini
- **Interactive Chat**: Loop-based console chat until 'exit' command
- **Gradio UI**: Beautiful web interface for easy interaction
- **Source Citations**: Shows which video chunks were used for answers
- **Dynamic Updates**: Add new videos without restarting

## 📋 Requirements

- Python 3.8+
- OpenAI API Key (for GPT models) OR Google API Key (for Gemini)
- YouTube videos with available captions/transcripts

## 🚀 Installation

### 1. Clone or Download the Project

```powershell
cd d:\AI
```

### 2. Create Virtual Environment (Recommended)

```powershell
python -m venv venv
.\venv\Scripts\Activate.ps1
```

### 3. Install Dependencies

```powershell
pip install -r requirements.txt
```

### 4. Configure Environment Variables

Copy the example environment file and add your API keys:

```powershell
cp .env.example .env
```

Edit `.env` file and add your API key:

```env
# For OpenAI (GPT models)
OPENAI_API_KEY=your_openai_api_key_here
LLM_PROVIDER=openai

# OR for Google Gemini
GOOGLE_API_KEY=your_google_api_key_here
LLM_PROVIDER=gemini
```

## 📖 Usage

### Option 1: Gradio Web UI (Recommended)

Launch the web interface:

```powershell
python app_ui.py
```

Then open your browser to: `http://localhost:7860`

**Steps in UI:**
1. Go to "Add Videos" tab
2. Enter YouTube video URLs or IDs
3. Click "Add Videos to Database"
4. Go to "Chat" tab and start asking questions!

### Option 2: Console Mode

#### First Time Setup

```powershell
# Setup with video IDs
python main.py setup VIDEO_ID1,VIDEO_ID2,VIDEO_ID3

# Or enter video IDs interactively
python main.py setup
```

Example:
```powershell
python main.py setup dQw4w9WgXcQ,9bZkp7q19f0
```

#### Chat in Console

```powershell
python main.py
```

Type your questions and press Enter. Type `exit` to quit.

## 📁 Project Structure

```
d:\AI\
├── config.py                  # Configuration management
├── transcript_fetcher.py      # YouTube transcript fetching
├── text_chunker.py           # Text splitting and chunking
├── vector_database.py        # Vector DB operations
├── rag_chatbot.py            # RAG chatbot logic
├── main.py                   # Console interface
├── app_ui.py                 # Gradio web UI
├── requirements.txt          # Python dependencies
├── .env.example              # Environment template
├── .env                      # Your configuration (create this)
├── transcripts/              # Saved transcripts (auto-created)
└── chroma_db/                # Vector database (auto-created)
```

## 🎯 How It Works

1. **Transcript Fetching**: Downloads YouTube video transcripts using the YouTube Transcript API
2. **Text Chunking**: Splits large transcripts into manageable chunks (1000 chars with 200 overlap)
3. **Embeddings**: Generates embeddings for each chunk using OpenAI's embedding model
4. **Vector Storage**: Stores embeddings in ChromaDB for fast retrieval
5. **RAG Pipeline**: When you ask a question:
   - Retrieves most relevant chunks from vector DB
   - Sends chunks as context to LLM (GPT/Gemini)
   - LLM generates answer based only on provided context
6. **Response**: Returns answer with source citations

## 💡 Example Usage

### Adding Videos

**YouTube URLs:**
```
https://www.youtube.com/watch?v=dQw4w9WgXcQ
https://www.youtube.com/watch?v=9bZkp7q19f0
```

**Or just Video IDs:**
```
dQw4w9WgXcQ
9bZkp7q19f0
```

### Example Questions

```
You: What is the main topic discussed in the video?
Bot: [Answer based on transcript content with sources]

You: What are the key points mentioned about AI?
Bot: [Detailed answer with video ID citations]

You: Can you summarize the content?
Bot: [Summary based on available transcripts]

You: exit
👋 Goodbye!
```

## ⚙️ Configuration Options

Edit `config.py` or `.env` to customize:

| Setting | Description | Default |
|---------|-------------|---------|
| `LLM_PROVIDER` | Choose 'openai' or 'gemini' | openai |
| `OPENAI_MODEL` | GPT model name | gpt-3.5-turbo |
| `GEMINI_MODEL` | Gemini model name | gemini-pro |
| `EMBEDDING_MODEL` | Embedding model | text-embedding-ada-002 |
| `CHUNK_SIZE` | Text chunk size | 1000 |
| `CHUNK_OVERLAP` | Chunk overlap | 200 |
| `VECTOR_DB_PATH` | Database location | ./chroma_db |

## 🔧 Troubleshooting

### "No transcript found"
- Video doesn't have captions enabled
- Try a different video or enable captions on YouTube

### "API Key Error"
- Check your `.env` file has the correct API key
- Verify the key has not expired
- Ensure `LLM_PROVIDER` matches your key type

### "Vector database not found"
- Run setup first: `python main.py setup`
- Or add videos through the Gradio UI

### Import Errors
- Reinstall dependencies: `pip install -r requirements.txt`
- Check Python version: `python --version` (needs 3.8+)

## 📚 Module Details

### `transcript_fetcher.py`
Fetches YouTube transcripts and saves them as text files.

### `text_chunker.py`
Uses LangChain's RecursiveCharacterTextSplitter to divide transcripts.

### `vector_database.py`
Manages ChromaDB operations including embedding generation and similarity search.

### `rag_chatbot.py`
Implements the RAG pipeline with LangChain and supports both OpenAI and Gemini.

### `main.py`
Console-based chat interface with loop until 'exit'.

### `app_ui.py`
Gradio-based web UI with tabs for chat, adding videos, and database info.

## 🎓 Educational Use Cases

This project demonstrates:
- **RAG Architecture**: Retrieval-Augmented Generation pattern
- **Vector Databases**: Semantic search with embeddings
- **LLM Integration**: Working with OpenAI and Google APIs
- **Text Processing**: Chunking and embedding strategies
- **Web UI Development**: Building interfaces with Gradio

## 🤝 Contributing

Feel free to enhance this project:
- Add support for more LLM providers
- Implement caching for faster responses
- Add conversation history
- Support other video platforms

## 📄 License

This project is for educational purposes. Ensure you comply with:
- YouTube Terms of Service
- OpenAI/Google API Terms
- Transcript usage rights

## 🆘 Support

For issues or questions:
1. Check the Troubleshooting section
2. Review the error messages carefully
3. Verify API keys and configuration
4. Ensure videos have transcripts available

## 🎉 Quick Start Summary

```powershell
# 1. Install dependencies
pip install -r requirements.txt

# 2. Set up environment
cp .env.example .env
# Edit .env with your API key

# 3. Launch UI
python app_ui.py

# 4. Add videos and start chatting!
```

---

**Built with ❤️ using LangChain, OpenAI, ChromaDB, and Gradio**
