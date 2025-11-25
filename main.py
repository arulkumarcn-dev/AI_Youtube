import os
from config import Config
from transcript_fetcher import YouTubeTranscriptFetcher
from text_chunker import TranscriptChunker
from vector_database import VectorDatabase
from rag_chatbot import RAGChatbot

def setup_database(video_ids=None):
    """
    Setup: Fetch transcripts, chunk them, and create vector database
    
    Args:
        video_ids: List of YouTube video IDs or URLs to process
    """
    print("\n" + "="*60)
    print("SETTING UP RAG CHATBOT DATABASE")
    print("="*60)
    
    # Step 1: Fetch transcripts
    print("\n📥 Step 1: Fetching YouTube Transcripts...")
    fetcher = YouTubeTranscriptFetcher(transcript_dir=Config.TRANSCRIPT_DIR)
    
    if video_ids:
        fetcher.fetch_and_save(video_ids)
    else:
        print("No video IDs provided. Please add transcripts manually to the 'transcripts' folder.")
        return False
    
    # Step 2: Chunk transcripts
    print("\n✂️  Step 2: Chunking Transcripts...")
    chunker = TranscriptChunker(
        chunk_size=Config.CHUNK_SIZE,
        chunk_overlap=Config.CHUNK_OVERLAP
    )
    documents = chunker.chunk_from_files(Config.TRANSCRIPT_DIR)
    
    if not documents:
        print("❌ No documents created. Please check your transcripts.")
        return False
    
    # Step 3: Create vector database
    print("\n🗄️  Step 3: Creating Vector Database...")
    vdb = VectorDatabase(
        persist_directory=Config.VECTOR_DB_PATH,
        embedding_model=Config.EMBEDDING_MODEL,
        openai_api_key=Config.OPENAI_API_KEY
    )
    vdb.create_vectorstore(documents)
    
    print("\n✅ Setup Complete!")
    return True


def run_console_chat():
    """Run the chatbot in console mode with a loop until 'exit'"""
    print("\n" + "="*60)
    print("RAG CHATBOT - YOUTUBE VIDEO Q&A")
    print("="*60)
    
    # Validate configuration
    try:
        Config.validate()
    except ValueError as e:
        print(f"❌ Configuration Error: {e}")
        print("\nPlease set up your .env file with the required API keys.")
        return
    
    # Load vector database
    print("\n📂 Loading vector database...")
    try:
        vdb = VectorDatabase(
            persist_directory=Config.VECTOR_DB_PATH,
            embedding_model=Config.EMBEDDING_MODEL,
            openai_api_key=Config.OPENAI_API_KEY
        )
        vectorstore = vdb.load_vectorstore()
        
        # Get collection info
        info = vdb.get_collection_info()
        print(f"✅ Database loaded: {info['count']} documents")
        
    except FileNotFoundError:
        print("❌ Vector database not found. Please run setup first.")
        print("\nRun setup by calling: setup_database(['video_id1', 'video_id2'])")
        return
    
    # Initialize chatbot
    print("\n🤖 Initializing chatbot...")
    
    if Config.LLM_PROVIDER == "openai":
        chatbot = RAGChatbot(
            vectorstore=vectorstore,
            llm_provider="openai",
            openai_api_key=Config.OPENAI_API_KEY,
            model_name=Config.OPENAI_MODEL
        )
    else:
        chatbot = RAGChatbot(
            vectorstore=vectorstore,
            llm_provider="gemini",
            google_api_key=Config.GOOGLE_API_KEY,
            model_name=Config.GEMINI_MODEL
        )
    
    print("✅ Chatbot ready!")
    print("\n" + "="*60)
    print("Ask questions about the YouTube videos!")
    print("Type 'exit' to quit")
    print("="*60 + "\n")
    
    # Chat loop
    while True:
        try:
            # Get user input
            question = input("You: ").strip()
            
            # Check for exit command
            if question.lower() in ['exit', 'quit', 'bye']:
                print("\n👋 Goodbye! Thanks for using the RAG chatbot.")
                break
            
            # Skip empty questions
            if not question:
                continue
            
            # Get answer
            print("\n🤔 Thinking...\n")
            response = chatbot.chat(question, verbose=True)
            print(f"Bot: {response}\n")
            print("-" * 60 + "\n")
            
        except KeyboardInterrupt:
            print("\n\n👋 Interrupted. Goodbye!")
            break
        except Exception as e:
            print(f"\n❌ Error: {str(e)}\n")
            continue


def main():
    """Main entry point"""
    import sys
    
    if len(sys.argv) > 1 and sys.argv[1] == "setup":
        # Setup mode - provide video IDs as arguments
        video_ids = sys.argv[2:] if len(sys.argv) > 2 else []
        
        if not video_ids:
            print("\n📝 Setup Mode")
            print("\nProvide YouTube video IDs or URLs (comma-separated):")
            input_str = input("> ")
            video_ids = [v.strip() for v in input_str.split(",") if v.strip()]
        
        if video_ids:
            setup_database(video_ids)
        else:
            print("❌ No video IDs provided.")
    else:
        # Chat mode
        run_console_chat()


if __name__ == "__main__":
    main()
