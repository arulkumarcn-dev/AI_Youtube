# 🤗 Hugging Face API Setup Guide

## ✨ Why Hugging Face?

Hugging Face offers **FREE** API access to thousands of open-source AI models! Unlike OpenAI which requires billing, you can:
- ✅ Use completely **FREE** (with rate limits)
- ✅ Access open-source models like Mistral, Llama, etc.
- ✅ No credit card required for basic usage
- ✅ Great for testing and development

---

## 📝 Getting Your Free Hugging Face API Key

### Step 1: Create Account (FREE)

1. **Visit:** https://huggingface.co/join
2. **Sign up** using:
   - Email address
   - Google account
   - GitHub account
3. **Verify your email** (check spam folder)

### Step 2: Generate API Token

1. **Go to:** https://huggingface.co/settings/tokens
   - Or: Click your profile → Settings → Access Tokens

2. **Click:** "New token"

3. **Fill in details:**
   - **Name:** `RAG-Chatbot` (or any name)
   - **Type:** Select **"Read"** (sufficient for API inference)
   - **Repositories:** Leave empty (not needed)

4. **Click:** "Generate token"

5. **IMPORTANT:** Copy the token immediately!
   - Format: `hf_...` (starts with `hf_`)
   - Save it securely - you won't see it again

### Step 3: Configure Your Chatbot

Edit **both** appsettings.json files:

**YouTubeRAGChatbot.Console\appsettings.json**
**YouTubeRAGChatbot.Web\appsettings.json**

```json
{
  "AIProvider": "HuggingFace",
  "HuggingFace": {
    "ApiKey": "hf_YOUR_TOKEN_HERE",
    "Model": "mistralai/Mistral-7B-Instruct-v0.2",
    "EmbeddingModel": "sentence-transformers/all-MiniLM-L6-v2",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

---

## 🚀 Quick Start

### 1. Update Configuration

```powershell
# Edit appsettings.json
notepad YouTubeRAGChatbot.Console\appsettings.json
```

Change:
- `"AIProvider": "HuggingFace"`
- `"ApiKey": "hf_YOUR_TOKEN_HERE"`

### 2. Run the Chatbot

```powershell
# Console app
dotnet run --project YouTubeRAGChatbot.Console

# Web app
dotnet run --project YouTubeRAGChatbot.Web
```

---

## 🎯 Recommended Models

### For Chat (Generation)

| Model | Speed | Quality | Free Tier |
|-------|-------|---------|-----------|
| `mistralai/Mistral-7B-Instruct-v0.2` | ⚡ Fast | ⭐⭐⭐⭐ | ✅ Yes |
| `HuggingFaceH4/zephyr-7b-beta` | ⚡ Fast | ⭐⭐⭐⭐ | ✅ Yes |
| `meta-llama/Llama-2-7b-chat-hf` | ⚡⚡ Medium | ⭐⭐⭐⭐⭐ | ✅ Yes |
| `google/flan-t5-xxl` | ⚡⚡⚡ Slower | ⭐⭐⭐ | ✅ Yes |

### For Embeddings (Search)

| Model | Dimensions | Speed | Quality |
|-------|------------|-------|---------|
| `sentence-transformers/all-MiniLM-L6-v2` | 384 | ⚡⚡⚡ | ⭐⭐⭐⭐ |
| `sentence-transformers/all-mpnet-base-v2` | 768 | ⚡⚡ | ⭐⭐⭐⭐⭐ |
| `sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2` | 384 | ⚡⚡⚡ | ⭐⭐⭐⭐ (Multilingual) |

---

## ⚙️ Configuration Options

### appsettings.json Structure

```json
{
  "AIProvider": "HuggingFace",  // or "OpenAI"
  
  "HuggingFace": {
    "ApiKey": "hf_xxxxxxxxxxxxx",
    "Model": "mistralai/Mistral-7B-Instruct-v0.2",
    "EmbeddingModel": "sentence-transformers/all-MiniLM-L6-v2",
    "Temperature": 0.7,    // 0.0 = deterministic, 1.0 = creative
    "MaxTokens": 1000      // Maximum response length
  },
  
  "OpenAI": {
    "ApiKey": "sk-proj-...",
    "Model": "gpt-3.5-turbo",
    "EmbeddingModel": "text-embedding-ada-002",
    "Temperature": 0.3
  }
}
```

### Switching Between Providers

Change `"AIProvider"` value:
- `"HuggingFace"` - Use Hugging Face (FREE)
- `"OpenAI"` - Use OpenAI (requires billing)

---

## 📊 Feature Comparison

| Feature | Hugging Face | OpenAI |
|---------|--------------|--------|
| **Cost** | 🆓 FREE | 💰 Paid |
| **Signup** | No credit card | Credit card required |
| **Rate Limits** | ~1000 requests/day | Based on usage tier |
| **Response Quality** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Response Speed** | ⚡⚡ Medium | ⚡⚡⚡ Fast |
| **Model Choice** | 1000s of models | Limited selection |
| **Privacy** | Open source models | Proprietary |

---

## 🐛 Troubleshooting

### "Model is currently loading"

**Solution:** Wait 20-30 seconds and try again. Free tier models need to "wake up".

```powershell
# The first request might take longer
# Just retry after a few seconds
```

### "Rate limit exceeded"

**Solution:** You've hit the free tier limit.

**Options:**
1. Wait (limits reset daily)
2. Upgrade to Pro ($9/month for unlimited)
3. Switch to OpenAI temporarily

### "Invalid token"

**Solution:** Check your API key:

```powershell
# Test your token
curl https://huggingface.co/api/whoami-v2 -H "Authorization: Bearer hf_YOUR_TOKEN"
```

Should return your username, not an error.

### "SSL connection error"

**Solution:** Check firewall/proxy settings:

```powershell
Test-NetConnection api-inference.huggingface.co -Port 443
```

---

## 💡 Tips & Best Practices

### 1. Model Selection

- **For accuracy:** Use larger models (7B+)
- **For speed:** Use smaller models (base, MiniLM)
- **For multilingual:** Use `paraphrase-multilingual` models

### 2. Temperature Settings

```json
{
  "Temperature": 0.3  // More focused, factual (good for RAG)
  "Temperature": 0.7  // Balanced (default)
  "Temperature": 1.0  // More creative (not recommended for RAG)
}
```

### 3. Rate Limit Management

- Cache results when possible
- Implement retry logic with delays
- Consider upgrading to Pro for production

### 4. First-Time Delays

```
⏳ Model loading: 20-60 seconds (first request only)
⚡ Subsequent requests: 1-5 seconds
```

---

## 🎓 Example Usage

### Console App

```powershell
# Setup with YouTube videos
dotnet run --project YouTubeRAGChatbot.Console setup VIDEO_ID1,VIDEO_ID2

# Chat
dotnet run --project YouTubeRAGChatbot.Console
```

### Web App

```powershell
# Start server
dotnet run --project YouTubeRAGChatbot.Web

# Open browser
start http://localhost:5000
```

---

## 🔗 Useful Links

- **Hugging Face Hub:** https://huggingface.co/models
- **API Documentation:** https://huggingface.co/docs/api-inference
- **Model Leaderboard:** https://huggingface.co/spaces/HuggingFaceH4/open_llm_leaderboard
- **Pricing:** https://huggingface.co/pricing
- **Status Page:** https://status.huggingface.co/

---

## 🆚 When to Use Which?

### Use Hugging Face When:
- ✅ Testing/development
- ✅ Budget constraints
- ✅ Want open-source models
- ✅ Need model variety
- ✅ Learning/educational projects

### Use OpenAI When:
- ✅ Production applications
- ✅ Need highest quality
- ✅ Want fastest responses
- ✅ Have budget for API costs
- ✅ Need enterprise support

---

## 📋 Checklist

- [ ] Created Hugging Face account
- [ ] Generated API token (starts with `hf_`)
- [ ] Updated `appsettings.json` with API key
- [ ] Set `"AIProvider": "HuggingFace"`
- [ ] Tested with console app
- [ ] Tested with web app

---

## 🎉 You're Ready!

Your chatbot now supports **FREE** Hugging Face models!

No credit card, no billing worries - just pure AI-powered chatting! 🚀

**Need help?** Check the troubleshooting section or create an issue.

---

**Built with ❤️ using .NET 8.0, Hugging Face, and Blazor**
