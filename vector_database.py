from langchain.vectorstores import Chroma
from langchain.embeddings import OpenAIEmbeddings
from langchain.docstore.document import Document
from typing import List, Optional
import os
import shutil

class VectorDatabase:
    """Manages vector database operations for embeddings"""
    
    def __init__(self, persist_directory: str = "./chroma_db", 
                 embedding_model: str = "text-embedding-ada-002",
                 openai_api_key: Optional[str] = None):
        """
        Initialize the vector database
        
        Args:
            persist_directory: Directory to persist the database
            embedding_model: OpenAI embedding model name
            openai_api_key: OpenAI API key
        """
        self.persist_directory = persist_directory
        self.embedding_model = embedding_model
        
        # Initialize embeddings
        self.embeddings = OpenAIEmbeddings(
            model=embedding_model,
            openai_api_key=openai_api_key
        )
        
        self.vectorstore = None
    
    def create_vectorstore(self, documents: List[Document]) -> Chroma:
        """
        Create a new vector store from documents
        
        Args:
            documents: List of Document objects to embed
            
        Returns:
            Chroma vectorstore instance
        """
        print(f"Creating vector database with {len(documents)} documents...")
        
        # Create vectorstore
        self.vectorstore = Chroma.from_documents(
            documents=documents,
            embedding=self.embeddings,
            persist_directory=self.persist_directory
        )
        
        # Persist the database
        self.vectorstore.persist()
        print(f"✓ Vector database created and persisted to {self.persist_directory}")
        
        return self.vectorstore
    
    def load_vectorstore(self) -> Chroma:
        """
        Load existing vector store from disk
        
        Returns:
            Chroma vectorstore instance
        """
        if not os.path.exists(self.persist_directory):
            raise FileNotFoundError(f"Vector database not found at {self.persist_directory}")
        
        print(f"Loading vector database from {self.persist_directory}...")
        
        self.vectorstore = Chroma(
            persist_directory=self.persist_directory,
            embedding_function=self.embeddings
        )
        
        print("✓ Vector database loaded successfully")
        return self.vectorstore
    
    def add_documents(self, documents: List[Document]):
        """
        Add new documents to existing vectorstore
        
        Args:
            documents: List of Document objects to add
        """
        if self.vectorstore is None:
            raise ValueError("Vectorstore not initialized. Call create_vectorstore or load_vectorstore first.")
        
        print(f"Adding {len(documents)} documents to vector database...")
        self.vectorstore.add_documents(documents)
        self.vectorstore.persist()
        print("✓ Documents added and persisted")
    
    def search(self, query: str, k: int = 4) -> List[Document]:
        """
        Search for similar documents
        
        Args:
            query: Search query
            k: Number of results to return
            
        Returns:
            List of most similar documents
        """
        if self.vectorstore is None:
            raise ValueError("Vectorstore not initialized. Call create_vectorstore or load_vectorstore first.")
        
        results = self.vectorstore.similarity_search(query, k=k)
        return results
    
    def search_with_score(self, query: str, k: int = 4) -> List[tuple]:
        """
        Search for similar documents with similarity scores
        
        Args:
            query: Search query
            k: Number of results to return
            
        Returns:
            List of (Document, score) tuples
        """
        if self.vectorstore is None:
            raise ValueError("Vectorstore not initialized. Call create_vectorstore or load_vectorstore first.")
        
        results = self.vectorstore.similarity_search_with_score(query, k=k)
        return results
    
    def get_retriever(self, k: int = 4):
        """
        Get a retriever interface for the vectorstore
        
        Args:
            k: Number of documents to retrieve
            
        Returns:
            Retriever object
        """
        if self.vectorstore is None:
            raise ValueError("Vectorstore not initialized. Call create_vectorstore or load_vectorstore first.")
        
        return self.vectorstore.as_retriever(
            search_kwargs={"k": k}
        )
    
    def delete_vectorstore(self):
        """Delete the vector database from disk"""
        if os.path.exists(self.persist_directory):
            shutil.rmtree(self.persist_directory)
            print(f"✓ Vector database deleted from {self.persist_directory}")
            self.vectorstore = None
        else:
            print("No vector database found to delete")
    
    def get_collection_info(self) -> dict:
        """
        Get information about the vector database collection
        
        Returns:
            Dictionary with collection information
        """
        if self.vectorstore is None:
            raise ValueError("Vectorstore not initialized")
        
        collection = self.vectorstore._collection
        return {
            'name': collection.name,
            'count': collection.count(),
            'persist_directory': self.persist_directory
        }


if __name__ == "__main__":
    # Example usage
    from langchain.docstore.document import Document
    
    # Create sample documents
    sample_docs = [
        Document(
            page_content="This is a sample document about AI.",
            metadata={'source': 'example1', 'video_id': 'vid1'}
        ),
        Document(
            page_content="Another document about machine learning.",
            metadata={'source': 'example2', 'video_id': 'vid2'}
        )
    ]
    
    # Initialize vector database
    # vdb = VectorDatabase(openai_api_key="your-api-key")
    # vdb.create_vectorstore(sample_docs)
    
    print("Vector database module ready")
