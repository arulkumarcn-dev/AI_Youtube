# 🎯 How to Use the Google Colab Notebooks

The `.ipynb` files are **Google Colab notebooks** - they run in Google's cloud, not on your local machine.

## 🚀 Quick Start (3 Steps)

### Step 1: Open in Google Colab

**Option A: Direct Upload**
1. Go to https://colab.research.google.com/
2. Click **File → Upload notebook**
3. Upload `YouTube_RAG_Chatbot_Colab_Complete.ipynb`

**Option B: From GitHub**
1. Go to https://colab.research.google.com/
2. Click **File → Open notebook → GitHub**
3. Enter: `arulkumarcn-dev/AI_Youtube`
4. Select the notebook

**Option C: Direct Link (if on GitHub)**
- Click the notebook in GitHub
- Click "Open in Colab" badge (if added)
- Or replace `github.com` with `colab.research.google.com/github` in the URL

### Step 2: Run the Cells

1. Click the **▶️ play button** on each cell (in order, 1-11)
2. Or use **Runtime → Run all** (runs all cells at once)

### Step 3: Add Your API Key

When you reach Cell 2:
- **For HuggingFace (FREE):** Get token from https://huggingface.co/settings/tokens
- **For OpenAI (Paid):** Get key from https://platform.openai.com/api-keys

## 📋 Which Notebook to Use?

### YouTube_RAG_Chatbot_Colab_Complete.ipynb ✅ RECOMMENDED
- **Supports:** OpenAI OR HuggingFace
- **Best for:** Flexibility, free option available
- **Change provider:** Set `AI_PROVIDER = "HuggingFace"` or `"OpenAI"` in Cell 2

### YouTube_RAG_Chatbot_Colab.ipynb
- **Supports:** OpenAI only
- **Best for:** If you already have OpenAI API key
- **Simpler:** Fewer configuration options

## ⚠️ Common Issues & Solutions

### Issue 1: "Module not found" errors
**Solution:** Make sure Cell 1 (package installation) completed successfully
- Look for ✅ at the end
- If failed, click the cell again to re-run

### Issue 2: "No transcripts fetched successfully"
**Solution:** The video doesn't have captions enabled
- Try different video IDs
- Use educational content (usually has captions)
- Test with: `dQw4w9WgXcQ` or `jNQXAC9IVRw`

### Issue 3: API key errors
**Solution:** 
- OpenAI: Check key starts with `sk-`
- HuggingFace: Check token starts with `hf_`
- Make sure no extra spaces

### Issue 4: "Runtime disconnected"
**Solution:** Free Colab has time limits
- Click **Reconnect** button
- Re-run cells from where it stopped

### Issue 5: Slow responses with HuggingFace
**Solution:** This is normal - HuggingFace free API is slower
- First request: 30-60 seconds (model loads)
- Subsequent requests: 10-20 seconds
- OpenAI is faster but costs money

## 🎓 Complete Walkthrough

### Cell 1: Install Packages (⏱️ 1-2 minutes)
```python
%%capture
!pip install youtube-transcript-api langchain ...
```
- Installs all required libraries
- Only need to run once per session

### Cell 2: Choose AI Provider & Add Key
```python
AI_PROVIDER = "HuggingFace"  # Change to "OpenAI" if preferred
```
- **HuggingFace:** Free, slower (~30s per query)
- **OpenAI:** Paid (~$0.02 per video), faster (~3s per query)

### Cell 3: Import Libraries
- Loads Python modules
- Should complete instantly
- If errors here, re-run Cell 1

### Cell 4: Transcript Fetcher
- Creates the YouTube caption fetcher
- Just defines functions, doesn't run anything

### Cell 5: Add Videos ⭐ IMPORTANT
```python
VIDEO_IDS = [
    "dQw4w9WgXcQ",  # Your video ID here
]
```
- Add video IDs or URLs (comma-separated)
- Videos MUST have captions enabled
- Can run multiple times to add more videos

### Cell 6: Create Text Chunks
- Splits transcripts into 1000-character pieces
- Automatically runs after Cell 5

### Cell 7: Create Embeddings (⏱️ 1-3 minutes)
- Converts text to vectors for search
- **Slowest step** - be patient!
- HuggingFace: Downloads model first time (~500MB)
- OpenAI: Sends to API

### Cell 8: Create Chatbot
- Sets up the RAG (Retrieval-Augmented Generation) system
- Should complete in seconds

### Cell 9: Chat Function
- Defines the `chat("question")` function
- Doesn't run anything yet

### Cell 10-11: Test Chat
- Try example questions
- Or use Gradio UI for web interface

## 💡 Pro Tips

### Tip 1: Use Colab Secrets (Secure API Keys)
1. Click the **🔑 Key icon** in left sidebar
2. Add secret: `OPENAI_API_KEY` or `HF_TOKEN`
3. The notebook will auto-load it
4. No need to type key manually each time

### Tip 2: Save Your Work
- **File → Save a copy in Drive** (saves to Google Drive)
- Your videos and settings persist
- No need to re-add videos next time

### Tip 3: Free GPU (Optional)
- **Runtime → Change runtime type → GPU**
- Makes HuggingFace embeddings MUCH faster
- Free tier includes GPU access

### Tip 4: Keep Session Alive
- Free Colab disconnects after 90 minutes idle
- Keep browser tab open
- Use "Reconnect" if disconnected

### Tip 5: Test with Short Videos First
- Start with 2-5 minute videos
- Test that everything works
- Then add longer videos

## 📊 What to Expect

### With OpenAI (Paid):
- Setup time: 2-3 minutes
- Per video processing: 30-60 seconds
- Query response time: 2-5 seconds
- Cost: ~$0.02 per video, ~$0.001 per query
- Quality: Excellent

### With HuggingFace (Free):
- Setup time: 5-10 minutes (model download)
- Per video processing: 1-2 minutes
- Query response time: 10-30 seconds
- Cost: FREE! 🎉
- Quality: Good

## 🐛 Troubleshooting Commands

If things break, run these in a new cell:

```python
# Check Python version (should be 3.10+)
!python --version

# Check installed packages
!pip list | grep langchain

# Clear and restart
# Runtime → Restart runtime

# Test imports individually
from langchain_openai import ChatOpenAI
print("✅ OpenAI works")

from langchain_huggingface import HuggingFaceEmbeddings
print("✅ HuggingFace works")
```

## 🔗 Useful Links

- **Google Colab:** https://colab.research.google.com/
- **HuggingFace Tokens:** https://huggingface.co/settings/tokens
- **OpenAI Keys:** https://platform.openai.com/api-keys
- **GitHub Repo:** https://github.com/arulkumarcn-dev/AI_Youtube
- **LangChain Docs:** https://python.langchain.com/docs/

## ❓ FAQ

**Q: Can I run this on my local machine?**
A: Yes, but you need Python installed. Use the Python files (`main.py`, `app_ui.py`) instead of Colab notebooks.

**Q: Why HuggingFace if it's slower?**
A: It's completely free! Good for learning/testing without paying.

**Q: Can I use both OpenAI and HuggingFace?**
A: Yes, just change `AI_PROVIDER` in Cell 2 and re-run cells 2-11.

**Q: How many videos can I add?**
A: No hard limit, but:
- More videos = longer processing time
- Free Colab may timeout with 20+ videos
- Start with 2-5 videos to test

**Q: Do I need to re-run everything each time?**
A: If you save to Google Drive, you can load previous databases. Otherwise yes, re-run cells 1-11.

**Q: Can I share my Gradio link?**
A: Yes! The link in Cell 11 is public for 72 hours. Anyone can chat with your videos.

**Q: What if my video doesn't have captions?**
A: The notebook will show an error. Try a different video. Most educational content has captions.

## 🎯 Success Checklist

Before asking for help, verify:
- ✅ Cell 1 completed without errors
- ✅ API key entered correctly (no spaces)
- ✅ Video has captions (test on YouTube website first)
- ✅ All cells run in order (1→2→3...→11)
- ✅ Internet connection stable
- ✅ Colab runtime still connected

## 🆘 Still Having Issues?

1. **Read error messages carefully** - they usually tell you what's wrong
2. **Check which cell failed** - errors show cell number
3. **Try Runtime → Restart runtime** and run all cells again
4. **Test with a known-good video:** `dQw4w9WgXcQ`
5. **Try HuggingFace instead of OpenAI** (or vice versa)

---

**🎬 Ready to start? Upload the notebook to Google Colab and run Cell 1!**
