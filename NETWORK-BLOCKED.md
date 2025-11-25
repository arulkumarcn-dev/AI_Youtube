# ⚠️ Network Blocking Issue Detected

## Problem
Your network/firewall (Cisco Security) is blocking access to HuggingFace API:
```
Error: (403) Forbidden
Cisco block: https://block.sse.cisco.com/
```

## Root Cause
Corporate firewalls often block AI API services. The HuggingFace API (`api-inference.huggingface.co`) is being blocked by Cisco Umbrella/OpenDNS security.

---

## ✅ Solutions (Choose One)

### Solution 1: Use OpenAI Instead (Recommended for Corporate Networks)
OpenAI is less likely to be blocked by corporate firewalls.

**Steps:**
1. Get OpenAI API key: https://platform.openai.com/api-keys
2. Add billing: https://platform.openai.com/account/billing
3. Update `appsettings.json`:
```json
{
  "AIProvider": "OpenAI",
  "OpenAI": {
    "ApiKey": "sk-YOUR-OPENAI-KEY-HERE",
    "Model": "gpt-3.5-turbo",
    "EmbeddingModel": "text-embedding-ada-002"
  }
}
```

**Cost:** ~$0.002 per 1000 tokens (very cheap for testing)

---

### Solution 2: Use VPN/Proxy
Connect through a VPN that allows HuggingFace access.

---

### Solution 3: Run Locally (No API, No Internet)
Use local LLM models - completely offline!

**Option A: Ollama (Easiest)**
```powershell
# Install Ollama from https://ollama.ai
ollama pull mistral
ollama serve
```

Then create a local API wrapper service (I can help implement this).

**Option B: LM Studio**
- Download: https://lmstudio.ai/
- Load models like Mistral, Llama 2
- Provides OpenAI-compatible API

---

### Solution 4: Contact IT Department
Request whitelisting for:
- `api-inference.huggingface.co`
- Port 443 (HTTPS)

---

## 🔍 Test Your Network

### Test HuggingFace Access:
```powershell
Test-NetConnection -ComputerName api-inference.huggingface.co -Port 443
```

### Test OpenAI Access:
```powershell
Test-NetConnection -ComputerName api.openai.com -Port 443
```

---

## 🚀 Quick Switch to OpenAI

**Console App:**
Edit `YouTubeRAGChatbot.Console\appsettings.json`:
```json
{
  "AIProvider": "OpenAI",
  "OpenAI": {
    "ApiKey": "sk-YOUR-KEY-HERE"
  }
}
```

**Web App:**
Edit `YouTubeRAGChatbot.Web\appsettings.json` (same change)

**Then run:**
```powershell
dotnet run --project YouTubeRAGChatbot.Console
```

---

## 💡 Recommendation

**For corporate/restricted networks:** Use OpenAI
- More reliable
- Less likely blocked
- Better performance
- Minimal cost (~$0.50 for 100 questions)

**For home/unrestricted networks:** Use HuggingFace
- Free
- Privacy-friendly
- Good for learning

---

## Next Steps

**Which solution do you prefer?**

1. **OpenAI** - I'll guide you through getting an API key
2. **Local LLM** - I'll implement Ollama integration
3. **Contact IT** - I'll provide the whitelist request template
4. **VPN** - Test with your VPN and let me know

Let me know and I'll help you get it running! 🚀
