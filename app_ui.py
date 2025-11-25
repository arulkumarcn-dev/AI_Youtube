import gradio as gr
from config import Config
from vector_database import VectorDatabase
from rag_chatbot import RAGChatbot
from transcript_fetcher import YouTubeTranscriptFetcher
from text_chunker import TranscriptChunker
import os

# Global variables
chatbot_instance = None
vectorstore_instance = None


def initialize_chatbot():
    """Initialize the chatbot and vector database"""
    global chatbot_instance, vectorstore_instance
    
    try:
        Config.validate()
        
        # Load vector database
        vdb = VectorDatabase(
            persist_directory=Config.VECTOR_DB_PATH,
            embedding_model=Config.EMBEDDING_MODEL,
            openai_api_key=Config.OPENAI_API_KEY
        )
        
        if os.path.exists(Config.VECTOR_DB_PATH):
            vectorstore_instance = vdb.load_vectorstore()
        else:
            return None, "⚠️ Vector database not found. Please add videos first."
        
        # Initialize chatbot
        if Config.LLM_PROVIDER == "openai":
            chatbot_instance = RAGChatbot(
                vectorstore=vectorstore_instance,
                llm_provider="openai",
                openai_api_key=Config.OPENAI_API_KEY,
                model_name=Config.OPENAI_MODEL
            )
        else:
            chatbot_instance = RAGChatbot(
                vectorstore=vectorstore_instance,
                llm_provider="gemini",
                google_api_key=Config.GOOGLE_API_KEY,
                model_name=Config.GEMINI_MODEL
            )
        
        info = vdb.get_collection_info()
        return chatbot_instance, f"✅ Chatbot ready! Database contains {info['count']} document chunks."
        
    except Exception as e:
        return None, f"❌ Error: {str(e)}"


def add_videos(video_urls):
    """Add new videos to the database"""
    global vectorstore_instance
    
    if not video_urls or not video_urls.strip():
        return "⚠️ Please enter at least one video URL or ID"
    
    try:
        # Parse video IDs
        video_ids = [v.strip() for v in video_urls.replace('\n', ',').split(',') if v.strip()]
        
        if not video_ids:
            return "⚠️ No valid video IDs found"
        
        # Fetch transcripts
        fetcher = YouTubeTranscriptFetcher(transcript_dir=Config.TRANSCRIPT_DIR)
        saved_files = fetcher.fetch_and_save(video_ids)
        
        if not saved_files:
            return "❌ Failed to fetch any transcripts"
        
        # Chunk transcripts
        chunker = TranscriptChunker(
            chunk_size=Config.CHUNK_SIZE,
            chunk_overlap=Config.CHUNK_OVERLAP
        )
        documents = chunker.chunk_from_files(Config.TRANSCRIPT_DIR)
        
        # Create or update vector database
        vdb = VectorDatabase(
            persist_directory=Config.VECTOR_DB_PATH,
            embedding_model=Config.EMBEDDING_MODEL,
            openai_api_key=Config.OPENAI_API_KEY
        )
        
        if os.path.exists(Config.VECTOR_DB_PATH):
            vdb.load_vectorstore()
            vdb.add_documents(documents)
            message = f"✅ Added {len(saved_files)} videos with {len(documents)} chunks to existing database"
        else:
            vdb.create_vectorstore(documents)
            message = f"✅ Created new database with {len(saved_files)} videos and {len(documents)} chunks"
        
        # Reinitialize chatbot
        vectorstore_instance = vdb.vectorstore
        return message
        
    except Exception as e:
        return f"❌ Error: {str(e)}"


def chat_interface(message, history):
    """Chat interface for Gradio"""
    global chatbot_instance
    
    if chatbot_instance is None:
        chatbot_instance, status = initialize_chatbot()
        if chatbot_instance is None:
            return status
    
    try:
        response = chatbot_instance.chat(message, verbose=True)
        return response
    except Exception as e:
        return f"❌ Error: {str(e)}"


def get_database_info():
    """Get information about the current database"""
    try:
        if not os.path.exists(Config.VECTOR_DB_PATH):
            return "No database found. Please add videos first."
        
        vdb = VectorDatabase(
            persist_directory=Config.VECTOR_DB_PATH,
            embedding_model=Config.EMBEDDING_MODEL,
            openai_api_key=Config.OPENAI_API_KEY
        )
        vdb.load_vectorstore()
        info = vdb.get_collection_info()
        
        # Get transcript files
        transcript_files = []
        if os.path.exists(Config.TRANSCRIPT_DIR):
            transcript_files = [f for f in os.listdir(Config.TRANSCRIPT_DIR) if f.endswith('.txt')]
        
        return f"""📊 Database Information:
        
• Document chunks: {info['count']}
• Transcript files: {len(transcript_files)}
• Database path: {Config.VECTOR_DB_PATH}
• LLM Provider: {Config.LLM_PROVIDER}
• Embedding Model: {Config.EMBEDDING_MODEL}

📹 Videos in database:
{chr(10).join([f"  - {f.replace('.txt', '')}" for f in transcript_files])}
"""
    except Exception as e:
        return f"❌ Error: {str(e)}"


def create_ui():
    """Create Gradio UI"""
    
    with gr.Blocks(title="YouTube RAG Chatbot", theme=gr.themes.Soft()) as app:
        gr.Markdown("""
        # 🤖 YouTube Video Q&A Chatbot
        ### RAG-Based Chatbot for YouTube Transcripts
        
        Ask questions about YouTube videos and get answers based on their transcripts!
        """)
        
        with gr.Tabs():
            # Chat Tab
            with gr.Tab("💬 Chat"):
                chatbot = gr.Chatbot(
                    height=500,
                    label="Chat History",
                    bubble_full_width=False
                )
                
                with gr.Row():
                    msg = gr.Textbox(
                        placeholder="Ask a question about the videos...",
                        label="Your Question",
                        scale=4
                    )
                    submit_btn = gr.Button("Send", variant="primary", scale=1)
                
                with gr.Row():
                    clear_btn = gr.Button("Clear Chat")
                
                gr.Markdown("""
                **Tips:**
                - Ask specific questions about the content in the videos
                - The bot will only answer based on the transcript content
                - Sources will be provided with each answer
                """)
                
                # Chat functionality
                def respond(message, chat_history):
                    bot_message = chat_interface(message, chat_history)
                    chat_history.append((message, bot_message))
                    return "", chat_history
                
                msg.submit(respond, [msg, chatbot], [msg, chatbot])
                submit_btn.click(respond, [msg, chatbot], [msg, chatbot])
                clear_btn.click(lambda: None, None, chatbot, queue=False)
            
            # Add Videos Tab
            with gr.Tab("➕ Add Videos"):
                gr.Markdown("""
                ### Add YouTube Videos to Database
                
                Enter YouTube video URLs or IDs (one per line or comma-separated):
                """)
                
                video_input = gr.Textbox(
                    placeholder="Example:\nhttps://www.youtube.com/watch?v=VIDEO_ID\nor just: VIDEO_ID",
                    label="Video URLs/IDs",
                    lines=5
                )
                
                add_btn = gr.Button("Add Videos to Database", variant="primary")
                status_output = gr.Textbox(label="Status", lines=3)
                
                add_btn.click(add_videos, inputs=[video_input], outputs=[status_output])
                
                gr.Markdown("""
                **Note:**
                - After adding videos, you can immediately start chatting
                - The bot will automatically reinitialize with new content
                - Make sure videos have captions/transcripts available
                """)
            
            # Database Info Tab
            with gr.Tab("📊 Database Info"):
                gr.Markdown("### Database Information")
                
                info_output = gr.Textbox(label="Database Status", lines=15)
                refresh_btn = gr.Button("Refresh Info", variant="secondary")
                
                refresh_btn.click(get_database_info, outputs=[info_output])
                
                # Load info on tab open
                app.load(get_database_info, outputs=[info_output])
            
            # Help Tab
            with gr.Tab("❓ Help"):
                gr.Markdown("""
                ## How to Use This Chatbot
                
                ### 1. Setup (First Time)
                1. Go to the **Add Videos** tab
                2. Enter YouTube video URLs or IDs
                3. Click "Add Videos to Database"
                4. Wait for processing to complete
                
                ### 2. Chat
                1. Go to the **Chat** tab
                2. Ask questions about the video content
                3. Get answers based on the transcripts
                
                ### 3. Features
                - **Context-based answers**: Only answers from transcript content
                - **Source citations**: Shows which video chunks were used
                - **Multiple videos**: Can add and query multiple videos
                - **Persistent database**: Videos are saved for future sessions
                
                ### 4. Example Questions
                - "What is the main topic discussed in the video?"
                - "What are the key points mentioned?"
                - "Can you summarize the content?"
                - "What was said about [specific topic]?"
                
                ### 5. Configuration
                - Edit the `.env` file to change API keys and models
                - Supports both OpenAI (GPT) and Google (Gemini) models
                
                ### 6. Requirements
                - Videos must have captions/transcripts available
                - API key for OpenAI or Google Gemini
                - Internet connection for API calls
                """)
        
        gr.Markdown("""
        ---
        **Note**: This chatbot only answers based on the transcript content. 
        Make sure to add videos before starting to chat!
        """)
    
    return app


def launch_ui():
    """Launch the Gradio interface"""
    try:
        # Check configuration
        Config.validate()
        
        # Create and launch UI
        app = create_ui()
        app.launch(
            server_name="0.0.0.0",
            server_port=7860,
            share=False,
            show_error=True
        )
        
    except ValueError as e:
        print(f"❌ Configuration Error: {e}")
        print("\nPlease set up your .env file with the required API keys.")
    except Exception as e:
        print(f"❌ Error: {str(e)}")


if __name__ == "__main__":
    launch_ui()
