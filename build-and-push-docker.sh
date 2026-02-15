#!/bin/bash
# Build and Push Copilot AgentOps MCP Server to DockerHub
# Usage: ./build-and-push-docker.sh <dockerhub-username> [tag]

set -e

USERNAME=$1
TAG=${2:-latest}
IMAGE_NAME="copilot-agentops"
FULL_IMAGE_NAME="${USERNAME}/${IMAGE_NAME}:${TAG}"

if [ -z "$USERNAME" ]; then
    echo "Usage: $0 <dockerhub-username> [tag]"
    echo "Example: $0 ajilantony latest"
    exit 1
fi

echo "Building Docker image: $FULL_IMAGE_NAME"

# Build the Docker image
docker build -f Dockerfile.copilot-agentops -t "$FULL_IMAGE_NAME" .

echo "✅ Successfully built: $FULL_IMAGE_NAME"

# Also tag as latest if a specific tag was provided
if [ "$TAG" != "latest" ]; then
    LATEST_IMAGE_NAME="${USERNAME}/${IMAGE_NAME}:latest"
    docker tag "$FULL_IMAGE_NAME" "$LATEST_IMAGE_NAME"
    echo "✅ Also tagged as: $LATEST_IMAGE_NAME"
fi

echo ""
echo "Pushing to DockerHub..."
echo "Please ensure you are logged in to DockerHub (run 'docker login' if needed)"

# Push the image
docker push "$FULL_IMAGE_NAME"

echo "✅ Successfully pushed: $FULL_IMAGE_NAME"

# Push latest tag if applicable
if [ "$TAG" != "latest" ]; then
    docker push "${USERNAME}/${IMAGE_NAME}:latest"
    echo "✅ Successfully pushed: ${USERNAME}/${IMAGE_NAME}:latest"
fi

echo ""
echo "✅ Image is now available at: https://hub.docker.com/r/${USERNAME}/${IMAGE_NAME}"
echo ""
echo "To use in VS Code MCP, update .vscode/mcp.json with:"
echo "  \"${FULL_IMAGE_NAME}\""
