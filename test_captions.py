from youtube_transcript_api import YouTubeTranscriptApi

# Test various popular videos to find ones with captions
test_videos = [
    ("Bitcoin Explained", "Gc2en3nHxA4"),
    ("What is AI", "mJeNghZXtMo"),
    ("Python Tutorial", "_uQrJ0TkZlc"),
    ("JavaScript Tutorial", "W6NZfCO5SIk"),
    ("Docker Tutorial", "pg19Z8LL06w"),
    ("Kubernetes", "X48VuDVv0do"),
    ("React Tutorial", "SqcY0GlETPk"),
    ("How Internet Works", "x3c1ih2NJEg"),
]

print("Testing videos for captions...\n")

working_videos = []

for title, video_id in test_videos:
    try:
        transcript = YouTubeTranscriptApi.get_transcript(video_id)
        if transcript:
            working_videos.append((title, video_id))
            print(f"✅ {title}: {video_id}")
            print(f"   URL: https://www.youtube.com/watch?v={video_id}")
    except Exception as e:
        print(f"❌ {title}: {video_id} - {str(e)[:50]}")

print(f"\n\nFound {len(working_videos)} working videos!")
print("\nTry these in your web app:")
for title, video_id in working_videos[:3]:
    print(f"  {video_id}")
