# Docker Build and Publish Guide

This guide will help you build and publish the Copilot AgentOps MCP Server Docker image to DockerHub.

## Prerequisites

1. **Docker Desktop** installed and running
2. **DockerHub Account** - Create one at [hub.docker.com](https://hub.docker.com)
3. **Docker CLI** logged in to DockerHub

## Quick Start

### 1. Login to DockerHub

```bash
docker login
```

Enter your DockerHub username and password when prompted.

### 2. Build and Push (Automated)

#### Windows (PowerShell)

```powershell
# Build and push to DockerHub
.\build-and-push-docker.ps1 -Username your-dockerhub-username

# Build and push with a specific tag
.\build-and-push-docker.ps1 -Username your-dockerhub-username -Tag v1.0.0

# Build only (no push)
.\build-and-push-docker.ps1 -Username your-dockerhub-username -NoPush
```

#### Linux/Mac (Bash)

```bash
# Make the script executable (first time only)
chmod +x build-and-push-docker.sh

# Build and push to DockerHub
./build-and-push-docker.sh your-dockerhub-username

# Build and push with a specific tag
./build-and-push-docker.sh your-dockerhub-username v1.0.0
```

### 3. Update MCP Configuration

After pushing to DockerHub, update your `.vscode/mcp.json` to use your image:

```json
{
  "servers": {
    "copilot-agentops": {
      "type": "stdio",
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "your-dockerhub-username/copilot-agentops:latest"
      ]
    }
  }
}
```

## Manual Build and Push

If you prefer to run commands manually:

### Build the Image

```bash
docker build -f Dockerfile.copilot-agentops -t your-dockerhub-username/copilot-agentops:latest .
```

### Tag with Version (Optional)

```bash
docker tag your-dockerhub-username/copilot-agentops:latest your-dockerhub-username/copilot-agentops:v1.0.0
```

### Push to DockerHub

```bash
# Push latest
docker push your-dockerhub-username/copilot-agentops:latest

# Push specific version
docker push your-dockerhub-username/copilot-agentops:v1.0.0
```

## Test Locally

Before pushing, test the image locally:

```bash
# Run in STDIO mode
docker run -i --rm your-dockerhub-username/copilot-agentops:latest

# Run in HTTP mode
docker run -i --rm -p 8080:8080 your-dockerhub-username/copilot-agentops:latest --http
```

## Multi-Platform Builds

To build for multiple platforms (AMD64 and ARM64):

```bash
# Create a builder instance (first time only)
docker buildx create --name multiplatform --use

# Build and push for multiple platforms
docker buildx build \
  --platform linux/amd64,linux/arm64 \
  -f Dockerfile.copilot-agentops \
  -t your-dockerhub-username/copilot-agentops:latest \
  --push \
  .
```

## Versioning Strategy

Recommended tagging strategy:

- `latest` - Always points to the newest stable version
- `v1.0.0` - Specific semantic version
- `v1.0` - Minor version (updated with patches)
- `v1` - Major version (updated with minor releases)

Example:

```bash
docker build -f Dockerfile.copilot-agentops -t your-dockerhub-username/copilot-agentops:v1.0.0 .
docker tag your-dockerhub-username/copilot-agentops:v1.0.0 your-dockerhub-username/copilot-agentops:v1.0
docker tag your-dockerhub-username/copilot-agentops:v1.0.0 your-dockerhub-username/copilot-agentops:v1
docker tag your-dockerhub-username/copilot-agentops:v1.0.0 your-dockerhub-username/copilot-agentops:latest

docker push your-dockerhub-username/copilot-agentops:v1.0.0
docker push your-dockerhub-username/copilot-agentops:v1.0
docker push your-dockerhub-username/copilot-agentops:v1
docker push your-dockerhub-username/copilot-agentops:latest
```

## CI/CD with GitHub Actions

Create `.github/workflows/docker-publish.yml`:

```yaml
name: Publish Docker Image

on:
  push:
    tags:
      - 'v*'
  workflow_dispatch:

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Login to DockerHub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ secrets.DOCKERHUB_USERNAME }}/copilot-agentops
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=semver,pattern={{major}}
            type=raw,value=latest,enable={{is_default_branch}}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./Dockerfile.copilot-agentops
          platforms: linux/amd64,linux/arm64
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
```

Add these secrets to your GitHub repository:
- `DOCKERHUB_USERNAME` - Your DockerHub username
- `DOCKERHUB_TOKEN` - DockerHub access token (create at [hub.docker.com/settings/security](https://hub.docker.com/settings/security))

## Troubleshooting

### Build Fails

- Ensure Docker Desktop is running
- Check that all source files are present
- Try cleaning Docker cache: `docker builder prune`

### Push Fails

- Verify you're logged in: `docker login`
- Check repository exists on DockerHub (will be auto-created on first push)
- Ensure you have write permissions

### Image Too Large

Current image size is optimized with:
- Alpine Linux base (~5MB)
- Multi-stage build
- Self-contained=false (uses runtime from base image)

To further reduce size:
- Use `dotnet publish` with trimming: `--p:PublishTrimmed=true`
- Remove debug symbols: `--p:DebugSymbols=false`

## Best Practices

1. **Always test locally** before pushing
2. **Use semantic versioning** for tags
3. **Keep `latest` stable** - only update after testing
4. **Document breaking changes** in git tags/releases
5. **Scan for vulnerabilities**: `docker scout cve your-dockerhub-username/copilot-agentops:latest`

## Resources

- [DockerHub](https://hub.docker.com)
- [Docker Documentation](https://docs.docker.com)
- [Docker Build Reference](https://docs.docker.com/engine/reference/commandline/build/)
- [Multi-platform Builds](https://docs.docker.com/build/building/multi-platform/)
