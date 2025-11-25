from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.docstore.document import Document
from typing import List
import os

class TranscriptChunker:
    """Handles text chunking for transcripts"""
    
    def __init__(self, chunk_size: int = 1000, chunk_overlap: int = 200):
        """
        Initialize the text chunker
        
        Args:
            chunk_size: Size of each text chunk
            chunk_overlap: Overlap between consecutive chunks
        """
        self.chunk_size = chunk_size
        self.chunk_overlap = chunk_overlap
        self.text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=chunk_size,
            chunk_overlap=chunk_overlap,
            length_function=len,
            separators=["\n\n", "\n", " ", ""]
        )
    
    def chunk_text(self, text: str, metadata: dict = None) -> List[Document]:
        """
        Split text into chunks
        
        Args:
            text: Text to split
            metadata: Optional metadata to attach to chunks
            
        Returns:
            List of Document objects with chunked text
        """
        if metadata is None:
            metadata = {}
        
        # Split text into chunks
        chunks = self.text_splitter.split_text(text)
        
        # Create Document objects with metadata
        documents = []
        for i, chunk in enumerate(chunks):
            doc_metadata = metadata.copy()
            doc_metadata['chunk_id'] = i
            doc_metadata['chunk_total'] = len(chunks)
            
            documents.append(Document(
                page_content=chunk,
                metadata=doc_metadata
            ))
        
        return documents
    
    def chunk_transcript(self, transcript: str, video_id: str) -> List[Document]:
        """
        Chunk a single transcript with video metadata
        
        Args:
            transcript: Transcript text
            video_id: YouTube video ID
            
        Returns:
            List of Document objects
        """
        metadata = {
            'video_id': video_id,
            'source': f'YouTube: {video_id}',
            'url': f'https://www.youtube.com/watch?v={video_id}'
        }
        
        return self.chunk_text(transcript, metadata)
    
    def chunk_multiple_transcripts(self, transcripts: List[dict]) -> List[Document]:
        """
        Chunk multiple transcripts
        
        Args:
            transcripts: List of transcript dictionaries with 'video_id' and 'transcript'
            
        Returns:
            List of all Document objects from all transcripts
        """
        all_documents = []
        
        for transcript_data in transcripts:
            video_id = transcript_data.get('video_id', 'unknown')
            transcript = transcript_data.get('transcript', '')
            
            if transcript:
                documents = self.chunk_transcript(transcript, video_id)
                all_documents.extend(documents)
                print(f"✓ Chunked {video_id}: {len(documents)} chunks")
        
        print(f"\n✓ Total chunks created: {len(all_documents)}")
        return all_documents
    
    def chunk_from_files(self, transcript_dir: str) -> List[Document]:
        """
        Load and chunk all transcript files from a directory
        
        Args:
            transcript_dir: Directory containing transcript files
            
        Returns:
            List of Document objects
        """
        all_documents = []
        
        if not os.path.exists(transcript_dir):
            raise FileNotFoundError(f"Transcript directory not found: {transcript_dir}")
        
        for filename in os.listdir(transcript_dir):
            if filename.endswith('.txt'):
                video_id = filename.replace('.txt', '')
                filepath = os.path.join(transcript_dir, filename)
                
                with open(filepath, 'r', encoding='utf-8') as f:
                    transcript = f.read()
                
                documents = self.chunk_transcript(transcript, video_id)
                all_documents.extend(documents)
                print(f"✓ Chunked {filename}: {len(documents)} chunks")
        
        print(f"\n✓ Total chunks created: {len(all_documents)}")
        return all_documents


if __name__ == "__main__":
    # Example usage
    chunker = TranscriptChunker(chunk_size=1000, chunk_overlap=200)
    
    # Example text
    sample_text = "This is a sample transcript. " * 100
    
    documents = chunker.chunk_text(sample_text, {'video_id': 'example'})
    print(f"Created {len(documents)} chunks")
    
    if documents:
        print(f"\nFirst chunk preview:")
        print(f"Content length: {len(documents[0].page_content)}")
        print(f"Metadata: {documents[0].metadata}")
