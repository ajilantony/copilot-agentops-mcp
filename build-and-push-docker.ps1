# Build and Push Copilot AgentOps MCP Server to DockerHub
# Usage: .\build-and-push-docker.ps1 -Username <dockerhub-username> [-Tag <tag>]

param(
    [Parameter(Mandatory=$true)]
    [string]$Username,
    
    [Parameter(Mandatory=$false)]
    [string]$Tag = "latest",
    
    [Parameter(Mandatory=$false)]
    [switch]$NoPush
)

$ImageName = "copilot-agentops"
$FullImageName = "${Username}/${ImageName}:${Tag}"

Write-Host "Building Docker image: $FullImageName" -ForegroundColor Cyan

# Build the Docker image
docker build -f Dockerfile.copilot-agentops -t $FullImageName .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Successfully built: $FullImageName" -ForegroundColor Green

# Also tag as latest if a specific tag was provided
if ($Tag -ne "latest") {
    $LatestImageName = "${Username}/${ImageName}:latest"
    docker tag $FullImageName $LatestImageName
    Write-Host "Also tagged as: $LatestImageName" -ForegroundColor Green
}

if (-not $NoPush) {
    Write-Host "`nPushing to DockerHub..." -ForegroundColor Cyan
    
    # Login to DockerHub (if not already logged in)
    Write-Host "Please ensure you are logged in to DockerHub (run 'docker login' if needed)" -ForegroundColor Yellow
    
    # Push the image
    docker push $FullImageName
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Docker push failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Successfully pushed: $FullImageName" -ForegroundColor Green
    
    # Push latest tag if applicable
    if ($Tag -ne "latest") {
        $LatestImageName = "${Username}/${ImageName}:latest"
        docker push $LatestImageName
        Write-Host "Successfully pushed: $LatestImageName" -ForegroundColor Green
    }
    
    Write-Host "`n✅ Image is now available at: https://hub.docker.com/r/${Username}/${ImageName}" -ForegroundColor Green
    Write-Host "`nTo use in VS Code MCP, update .vscode/mcp.json with:" -ForegroundColor Cyan
    Write-Host "  `"$FullImageName`"" -ForegroundColor White
} else {
    Write-Host "`n📦 Image built locally (skipped push)" -ForegroundColor Yellow
    Write-Host "Run without -NoPush to push to DockerHub" -ForegroundColor Yellow
}
