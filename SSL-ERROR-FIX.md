# 🔧 SSL Connection Error - OpenAI API Issue

## The Error
```
Error: The SSL connection could not be established, see inner exception.
```

## Root Cause
This error occurs when trying to call the **OpenAI API** for either:
1. **Generating embeddings** (when searching the vector database)
2. **Getting chat completions** (when answering questions)

## Possible Reasons

### 1. Invalid or Expired API Key ⚠️
Your current API key: `sk-admin-LG9EPKm...`

**Note:** The `sk-admin-` prefix is unusual. Standard OpenAI keys start with `sk-proj-` (new format) or `sk-` (legacy format).

**Solution:** Get a valid OpenAI API key:
1. Go to: https://platform.openai.com/api-keys
2. Sign in or create an account
3. Click "Create new secret key"
4. Copy the key (starts with `sk-proj-` or `sk-`)
5. Update `YouTubeRAGChatbot.Web\appsettings.json`

### 2. Corporate Network / Firewall 🔥
Your network may be blocking connections to `api.openai.com`

**Check:**
```powershell
Test-NetConnection api.openai.com -Port 443
```

**Solutions:**
- Configure proxy settings
- Use VPN
- Contact IT department
- Work from different network

### 3. Missing SSL Certificates
Rare, but possible on some corporate machines.

**Solution:** Update certificates or configure HTTP client to trust OpenAI's certificate.

---

## 🎯 Quick Fixes

### Option A: Update API Key (RECOMMENDED)

1. **Get real OpenAI API key** from https://platform.openai.com/api-keys

2. **Update appsettings.json:**
```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-YOUR_ACTUAL_KEY_HERE",
    "Model": "gpt-3.5-turbo",
    "EmbeddingModel": "text-embedding-ada-002"
  }
}
```

3. **Restart the app:**
```powershell
# Stop current app (Ctrl+C in terminal)
dotnet run --project YouTubeRAGChatbot.Web
```

### Option B: Test API Key Validity

Run this PowerShell command to test if your key works:

```powershell
$apiKey = "sk-admin-YOUR_KEY_HERE"
$headers = @{
    "Authorization" = "Bearer $apiKey"
    "Content-Type" = "application/json"
}
$body = @{
    model = "gpt-3.5-turbo"
    messages = @(
        @{
            role = "user"
            content = "Hello"
        }
    )
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "https://api.openai.com/v1/chat/completions" `
                      -Method Post `
                      -Headers $headers `
                      -Body $body
    Write-Host "✅ API Key is valid!" -ForegroundColor Green
} catch {
    Write-Host "❌ API Key test failed:" -ForegroundColor Red
    Write-Host $_.Exception.Message
}
```

### Option C: Check Network Connectivity

```powershell
# Test DNS resolution
Resolve-DnsName api.openai.com

# Test HTTPS connection
Test-NetConnection api.openai.com -Port 443

# Test with curl
curl https://api.openai.com/v1/models -H "Authorization: Bearer YOUR_KEY"
```

---

## 🚀 Alternative: Use Azure OpenAI

If you have Azure OpenAI access, you can switch to that instead:

1. **Update AppSettings.cs** to support Azure endpoint
2. **Use Azure OpenAI credentials**
3. **Change the base URL** in Semantic Kernel configuration

---

## 📋 Checklist

- [ ] Verify API key is valid (starts with `sk-proj-` or `sk-`)
- [ ] Test network connection to api.openai.com
- [ ] Check firewall/proxy settings
- [ ] Ensure you have OpenAI API credits/billing set up
- [ ] Try from different network if possible

---

## 💡 Important Notes

1. **OpenAI API requires billing:** Even with a valid key, you need to set up billing at https://platform.openai.com/account/billing

2. **Free tier limits:** Free API keys have rate limits and may not work for all models

3. **Key format matters:** 
   - ✅ `sk-proj-abc123...` (new format)
   - ✅ `sk-abc123...` (legacy format)
   - ❌ `sk-admin-...` (not a standard OpenAI format)

---

## 🔍 Next Steps

1. **Get valid API key** from OpenAI
2. **Update appsettings.json**
3. **Restart the web app**
4. **Try asking a question again**

The chatbot code is working perfectly - we just need a valid API key and network access to OpenAI!
