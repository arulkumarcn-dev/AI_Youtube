import os
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

class Config:
    """Configuration class for the RAG chatbot"""
    
    # API Keys
    OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
    GOOGLE_API_KEY = os.getenv("GOOGLE_API_KEY")
    
    # LLM Provider Selection
    LLM_PROVIDER = os.getenv("LLM_PROVIDER", "openai")
    
    # Model Configuration
    OPENAI_MODEL = os.getenv("OPENAI_MODEL", "gpt-3.5-turbo")
    GEMINI_MODEL = os.getenv("GEMINI_MODEL", "gemini-pro")
    EMBEDDING_MODEL = os.getenv("EMBEDDING_MODEL", "text-embedding-ada-002")
    
    # Vector Database
    VECTOR_DB_PATH = os.getenv("VECTOR_DB_PATH", "./chroma_db")
    
    # Text Chunking Configuration
    CHUNK_SIZE = 1000
    CHUNK_OVERLAP = 200
    
    # Transcript Storage
    TRANSCRIPT_DIR = "./transcripts"
    
    @classmethod
    def validate(cls):
        """Validate that required API keys are set"""
        if cls.LLM_PROVIDER == "openai" and not cls.OPENAI_API_KEY:
            raise ValueError("OPENAI_API_KEY is required when using OpenAI")
        if cls.LLM_PROVIDER == "gemini" and not cls.GOOGLE_API_KEY:
            raise ValueError("GOOGLE_API_KEY is required when using Gemini")
