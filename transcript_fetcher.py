import os
from youtube_transcript_api import YouTubeTranscriptApi
from youtube_transcript_api._errors import TranscriptsDisabled, NoTranscriptFound
from typing import List, Dict
import json

class YouTubeTranscriptFetcher:
    """Fetches and manages YouTube video transcripts"""
    
    def __init__(self, transcript_dir: str = "./transcripts"):
        """
        Initialize the transcript fetcher
        
        Args:
            transcript_dir: Directory to store transcript files
        """
        self.transcript_dir = transcript_dir
        os.makedirs(transcript_dir, exist_ok=True)
    
    @staticmethod
    def extract_video_id(url: str) -> str:
        """
        Extract video ID from YouTube URL
        
        Args:
            url: YouTube video URL or video ID
            
        Returns:
            Video ID string
        """
        if "youtube.com" in url or "youtu.be" in url:
            if "v=" in url:
                return url.split("v=")[1].split("&")[0]
            elif "youtu.be/" in url:
                return url.split("youtu.be/")[1].split("?")[0]
        return url  # Assume it's already a video ID
    
    def fetch_transcript(self, video_id: str) -> Dict:
        """
        Fetch transcript for a single video
        
        Args:
            video_id: YouTube video ID or URL
            
        Returns:
            Dictionary with video_id, transcript text, and metadata
        """
        video_id = self.extract_video_id(video_id)
        
        try:
            # Fetch transcript
            transcript_list = YouTubeTranscriptApi.get_transcript(video_id)
            
            # Combine transcript segments
            full_transcript = " ".join([entry['text'] for entry in transcript_list])
            
            # Create metadata
            metadata = {
                'video_id': video_id,
                'url': f'https://www.youtube.com/watch?v={video_id}',
                'segments': len(transcript_list),
                'duration': transcript_list[-1]['start'] + transcript_list[-1]['duration'] if transcript_list else 0
            }
            
            return {
                'video_id': video_id,
                'transcript': full_transcript,
                'transcript_segments': transcript_list,
                'metadata': metadata
            }
            
        except TranscriptsDisabled:
            raise Exception(f"Transcripts are disabled for video: {video_id}")
        except NoTranscriptFound:
            raise Exception(f"No transcript found for video: {video_id}")
        except Exception as e:
            raise Exception(f"Error fetching transcript: {str(e)}")
    
    def save_transcript(self, video_id: str, transcript_data: Dict) -> str:
        """
        Save transcript to a text file
        
        Args:
            video_id: YouTube video ID
            transcript_data: Transcript data dictionary
            
        Returns:
            Path to saved file
        """
        video_id = self.extract_video_id(video_id)
        
        # Save as text file
        text_filepath = os.path.join(self.transcript_dir, f"{video_id}.txt")
        with open(text_filepath, 'w', encoding='utf-8') as f:
            f.write(transcript_data['transcript'])
        
        # Save metadata as JSON
        json_filepath = os.path.join(self.transcript_dir, f"{video_id}_metadata.json")
        with open(json_filepath, 'w', encoding='utf-8') as f:
            json.dump(transcript_data['metadata'], f, indent=2)
        
        print(f"✓ Transcript saved: {text_filepath}")
        return text_filepath
    
    def fetch_and_save(self, video_ids: List[str]) -> List[str]:
        """
        Fetch and save transcripts for multiple videos
        
        Args:
            video_ids: List of YouTube video IDs or URLs
            
        Returns:
            List of saved file paths
        """
        saved_files = []
        
        for idx, video_id in enumerate(video_ids, 1):
            try:
                print(f"\n[{idx}/{len(video_ids)}] Fetching transcript for: {video_id}")
                transcript_data = self.fetch_transcript(video_id)
                filepath = self.save_transcript(video_id, transcript_data)
                saved_files.append(filepath)
                
            except Exception as e:
                print(f"✗ Error with video {video_id}: {str(e)}")
                continue
        
        print(f"\n✓ Successfully saved {len(saved_files)} transcripts")
        return saved_files
    
    def load_transcript(self, video_id: str) -> str:
        """
        Load transcript from file
        
        Args:
            video_id: YouTube video ID
            
        Returns:
            Transcript text
        """
        video_id = self.extract_video_id(video_id)
        filepath = os.path.join(self.transcript_dir, f"{video_id}.txt")
        
        if not os.path.exists(filepath):
            raise FileNotFoundError(f"Transcript file not found: {filepath}")
        
        with open(filepath, 'r', encoding='utf-8') as f:
            return f.read()
    
    def load_all_transcripts(self) -> List[Dict]:
        """
        Load all saved transcripts
        
        Returns:
            List of dictionaries with video_id and transcript
        """
        transcripts = []
        
        for filename in os.listdir(self.transcript_dir):
            if filename.endswith('.txt'):
                video_id = filename.replace('.txt', '')
                filepath = os.path.join(self.transcript_dir, filename)
                
                with open(filepath, 'r', encoding='utf-8') as f:
                    transcript = f.read()
                
                transcripts.append({
                    'video_id': video_id,
                    'transcript': transcript,
                    'source': filename
                })
        
        return transcripts


if __name__ == "__main__":
    # Example usage
    fetcher = YouTubeTranscriptFetcher()
    
    # Example video IDs (replace with your own)
    video_ids = [
        "dQw4w9WgXcQ",  # Example video ID
        # Add more video IDs or URLs here
    ]
    
    print("YouTube Transcript Fetcher")
    print("=" * 50)
    
    # Fetch and save transcripts
    fetcher.fetch_and_save(video_ids)
