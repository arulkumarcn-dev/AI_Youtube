from langchain.chat_models import ChatOpenAI
from langchain.chains import RetrievalQA
from langchain.prompts import PromptTemplate
from typing import Optional
import google.generativeai as genai

class RAGChatbot:
    """RAG-based chatbot for Q&A over YouTube transcripts"""
    
    def __init__(self, vectorstore, llm_provider: str = "openai", 
                 openai_api_key: Optional[str] = None,
                 google_api_key: Optional[str] = None,
                 model_name: str = "gpt-3.5-turbo"):
        """
        Initialize the RAG chatbot
        
        Args:
            vectorstore: Vector database instance
            llm_provider: LLM provider ('openai' or 'gemini')
            openai_api_key: OpenAI API key
            google_api_key: Google API key
            model_name: Model name to use
        """
        self.vectorstore = vectorstore
        self.llm_provider = llm_provider
        
        # Initialize LLM based on provider
        if llm_provider == "openai":
            self.llm = ChatOpenAI(
                model_name=model_name,
                temperature=0.3,
                openai_api_key=openai_api_key
            )
        elif llm_provider == "gemini":
            genai.configure(api_key=google_api_key)
            self.gemini_model = genai.GenerativeModel(model_name)
            self.llm = None  # Gemini will be used differently
        else:
            raise ValueError(f"Unsupported LLM provider: {llm_provider}")
        
        # Create custom prompt template
        self.prompt_template = """You are a helpful AI assistant that answers questions based ONLY on the provided context from YouTube video transcripts.

Context from transcripts:
{context}

Question: {question}

Instructions:
- Answer the question using ONLY the information from the provided context
- If the answer is not in the context, say "I cannot find this information in the available transcripts"
- Be specific and cite relevant parts of the transcript when possible
- Include video IDs when mentioning information from specific videos
- Do not make up information or use external knowledge

Answer:"""

        self.prompt = PromptTemplate(
            template=self.prompt_template,
            input_variables=["context", "question"]
        )
        
        # Create retrieval chain for OpenAI
        if llm_provider == "openai":
            self.qa_chain = RetrievalQA.from_chain_type(
                llm=self.llm,
                chain_type="stuff",
                retriever=self.vectorstore.get_retriever(k=4),
                return_source_documents=True,
                chain_type_kwargs={"prompt": self.prompt}
            )
    
    def ask(self, question: str) -> dict:
        """
        Ask a question and get an answer based on the transcripts
        
        Args:
            question: User's question
            
        Returns:
            Dictionary with answer and source documents
        """
        if self.llm_provider == "openai":
            return self._ask_openai(question)
        elif self.llm_provider == "gemini":
            return self._ask_gemini(question)
    
    def _ask_openai(self, question: str) -> dict:
        """Ask question using OpenAI"""
        result = self.qa_chain({"query": question})
        
        return {
            'answer': result['result'],
            'source_documents': result['source_documents'],
            'sources': self._format_sources(result['source_documents'])
        }
    
    def _ask_gemini(self, question: str) -> dict:
        """Ask question using Gemini"""
        # Get relevant documents
        docs = self.vectorstore.search(question, k=4)
        
        # Format context
        context = "\n\n".join([
            f"[Video {doc.metadata.get('video_id', 'unknown')}]: {doc.page_content}"
            for doc in docs
        ])
        
        # Create prompt
        prompt = f"""You are a helpful AI assistant that answers questions based ONLY on the provided context from YouTube video transcripts.

Context from transcripts:
{context}

Question: {question}

Instructions:
- Answer the question using ONLY the information from the provided context
- If the answer is not in the context, say "I cannot find this information in the available transcripts"
- Be specific and cite relevant parts of the transcript when possible
- Include video IDs when mentioning information from specific videos
- Do not make up information or use external knowledge

Answer:"""
        
        # Generate response
        response = self.gemini_model.generate_content(prompt)
        
        return {
            'answer': response.text,
            'source_documents': docs,
            'sources': self._format_sources(docs)
        }
    
    def _format_sources(self, documents) -> str:
        """Format source documents for display"""
        if not documents:
            return "No sources found"
        
        sources = []
        for i, doc in enumerate(documents, 1):
            video_id = doc.metadata.get('video_id', 'unknown')
            chunk_id = doc.metadata.get('chunk_id', 'unknown')
            url = doc.metadata.get('url', '')
            
            sources.append(f"{i}. Video ID: {video_id}, Chunk: {chunk_id}")
            if url:
                sources.append(f"   URL: {url}")
        
        return "\n".join(sources)
    
    def chat(self, question: str, verbose: bool = True) -> str:
        """
        Chat interface that returns formatted response
        
        Args:
            question: User's question
            verbose: Whether to include source information
            
        Returns:
            Formatted answer string
        """
        result = self.ask(question)
        
        response = result['answer']
        
        if verbose and result.get('sources'):
            response += f"\n\n📚 Sources:\n{result['sources']}"
        
        return response


if __name__ == "__main__":
    print("RAG Chatbot module ready")
    print("Initialize with a vectorstore to start chatting")
