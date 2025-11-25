# Create sample vector database with demo content
# This allows you to test the chatbot immediately

Write-Host "Creating sample vector database..." -ForegroundColor Cyan

# Create vectordb directory
$vectorDbPath = ".\vectordb"
if (-not (Test-Path $vectorDbPath)) {
    New-Item -ItemType Directory -Path $vectorDbPath | Out-Null
}

# Sample transcript about AI and Machine Learning
$sampleData = @{
    Chunks = @(
        @{
            VideoId = "SAMPLE001"
            VideoTitle = "Introduction to Artificial Intelligence"
            Text = "Artificial Intelligence, or AI, is a branch of computer science that aims to create machines capable of intelligent behavior. AI systems can learn from experience, adjust to new inputs, and perform human-like tasks."
            ChunkIndex = 0
            StartTime = "00:00:00"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE001"
            VideoTitle = "Introduction to Artificial Intelligence"
            Text = "Machine Learning is a subset of AI that focuses on the development of algorithms that can learn from and make predictions or decisions based on data. It uses statistical techniques to give computer systems the ability to learn without being explicitly programmed."
            ChunkIndex = 1
            StartTime = "00:00:15"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE001"
            VideoTitle = "Introduction to Artificial Intelligence"
            Text = "Deep Learning is a specialized form of Machine Learning that uses neural networks with multiple layers. These deep neural networks can automatically discover intricate patterns in large datasets, making them particularly effective for tasks like image recognition and natural language processing."
            ChunkIndex = 2
            StartTime = "00:00:30"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE002"
            VideoTitle = "Understanding Neural Networks"
            Text = "Neural networks are computing systems inspired by the biological neural networks in animal brains. They consist of interconnected nodes or neurons organized in layers. The input layer receives data, hidden layers process it, and the output layer produces the final result."
            ChunkIndex = 0
            StartTime = "00:00:00"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE002"
            VideoTitle = "Understanding Neural Networks"
            Text = "Training a neural network involves adjusting the weights and biases of connections between neurons to minimize the difference between predicted and actual outputs. This is typically done using backpropagation and gradient descent algorithms."
            ChunkIndex = 1
            StartTime = "00:00:20"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE003"
            VideoTitle = "Natural Language Processing Basics"
            Text = "Natural Language Processing, or NLP, is a field of AI that focuses on the interaction between computers and human language. It enables machines to read, understand, and derive meaning from human languages in a valuable way."
            ChunkIndex = 0
            StartTime = "00:00:00"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE003"
            VideoTitle = "Natural Language Processing Basics"
            Text = "Modern NLP systems use techniques like tokenization, named entity recognition, sentiment analysis, and language translation. Large Language Models like GPT have revolutionized NLP by understanding context and generating human-like text."
            ChunkIndex = 1
            StartTime = "00:00:18"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        },
        @{
            VideoId = "SAMPLE004"
            VideoTitle = "Computer Vision and Image Recognition"
            Text = "Computer Vision is an AI field that trains computers to interpret and understand visual information from the world. It involves acquiring, processing, analyzing, and understanding digital images and videos to produce numerical or symbolic information."
            ChunkIndex = 0
            StartTime = "00:00:00"
            Embedding = @(1..1536 | ForEach-Object { (Get-Random -Minimum -1.0 -Maximum 1.0) })
        }
    )
    VideoMetadata = @(
        @{
            VideoId = "SAMPLE001"
            Title = "Introduction to Artificial Intelligence"
            Duration = "00:01:30"
            TranscriptLength = 450
            ChunkCount = 3
        },
        @{
            VideoId = "SAMPLE002"
            Title = "Understanding Neural Networks"
            Duration = "00:01:15"
            TranscriptLength = 380
            ChunkCount = 2
        },
        @{
            VideoId = "SAMPLE003"
            Title = "Natural Language Processing Basics"
            Duration = "00:00:55"
            TranscriptLength = 320
            ChunkCount = 2
        },
        @{
            VideoId = "SAMPLE004"
            Title = "Computer Vision and Image Recognition"
            Duration = "00:00:40"
            TranscriptLength = 250
            ChunkCount = 1
        }
    )
}

# Convert to JSON and save
$jsonContent = $sampleData | ConvertTo-Json -Depth 10
$jsonContent | Out-File -FilePath "$vectorDbPath\vectordb.json" -Encoding UTF8

Write-Host "✓ Sample database created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Sample videos added:" -ForegroundColor Yellow
Write-Host "  1. Introduction to Artificial Intelligence" -ForegroundColor White
Write-Host "  2. Understanding Neural Networks" -ForegroundColor White
Write-Host "  3. Natural Language Processing Basics" -ForegroundColor White
Write-Host "  4. Computer Vision and Image Recognition" -ForegroundColor White
Write-Host ""
Write-Host "You can now:" -ForegroundColor Cyan
Write-Host "  1. Run the web app: dotnet run --project YouTubeRAGChatbot.Web" -ForegroundColor White
Write-Host "  2. Ask questions about AI, ML, Neural Networks, NLP, or Computer Vision" -ForegroundColor White
Write-Host ""
Write-Host "Try asking:" -ForegroundColor Green
Write-Host "  - What is machine learning" -ForegroundColor White
Write-Host "  - Explain how neural networks work" -ForegroundColor White
Write-Host "  - What is NLP" -ForegroundColor White
