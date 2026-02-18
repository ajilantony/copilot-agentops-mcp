# Copilot AgentOps MCP Server


## Introduction

Copilot AgentOps is a Model Context Protocol (MCP) server that provides seamless integration with  [copilot-agentops](https://github.com/ajilantony/copilot-agentops) repository. This server enables you to search, discover, and load custom GitHub Copilot instructions, prompts, agents, and collections directly into your development environment.

Built with .NET 9.0, Copilot AgentOps demonstrates a hybrid MCP server pattern that supports both STDIO and HTTP communication modes, with flexible deployment options including local development, containerized execution, and Azure cloud hosting.

## Features

- **🔍 Smart Search**: Search through GitHub Copilot customizations using keywords and descriptions
- **📥 Dynamic Loading**: Load custom instructions, prompts, agents, and collections on-demand from the copilot-agentops repository
- **🚀 Hybrid Architecture**: Run as STDIO or HTTP server with the same codebase
- **🐳 Container Ready**: Pre-built Docker images available from GitHub Container Registry
- **☁️ Azure Deployment**: One-command deployment to Azure Container Apps using Azure Developer CLI
- **🔌 IDE Integration**: Native integration with VS Code Agent Mode and Visual Studio

## Quick Start

### One-Click Installation

Install Copilot AgentOps directly into your IDE:

[![Install in VS Code](https://img.shields.io/badge/VS_Code-Install-0098FF?style=for-the-badge&logo=visualstudiocode&logoColor=white)](https://vscode.dev/redirect?url=vscode%3Amcp%2Finstall%3F%7B%22name%22%3A%22copilot-agentops%22%2C%22gallery%22%3Afalse%2C%22command%22%3A%22docker%22%2C%22args%22%3A%5B%22run%22%2C%22-i%22%2C%22--rm%22%2C%22ajilantony%2Fcopilot-agentops%3Alatest%22%5D%7D)

[![Install in Visual Studio](https://img.shields.io/badge/Visual_Studio-Install-C16FDE?style=for-the-badge&logo=visualstudio&logoColor=white)](https://aka.ms/vs/mcp-install?%7B%22name%22%3A%22copilot-agentops%22%2C%22gallery%22%3Afalse%2C%22command%22%3A%22docker%22%2C%22args%22%3A%5B%22run%22%2C%22-i%22%2C%22--rm%22%2C%22ajilantony%2Fcopilot-agentops%3Alatest%22%5D%7D)

### Docker Quick Run

```bash
docker run -i --rm ajilantony/copilot-agentops:latest
```

## Available Tools and Prompts

| Type    | Name                  | Description                                                      | Usage in IDE                             |
|---------|-----------------------|------------------------------------------------------------------|------------------------------------------|
| 🔧 Tool | `search_instructions` | Search custom instructions, prompts, and agents by keywords      | `#search_instructions`                   |
| 🔧 Tool | `load_instruction`    | Load and retrieve full content of a specific customization       | `#load_instruction`                      |
| 💬 Prompt | `get_search_prompt`  | Get a structured prompt template for searching customizations    | `/mcp.copilot-agentops.get_search_prompt` |

## How It Works

1. **Search**: Use `search_instructions` to find relevant GitHub Copilot customizations based on your needs
2. **Review**: Browse through search results showing title, description, type (instruction/prompt/agent), and availability
3. **Load**: Select and load specific customizations using `load_instruction` to get the full markdown content
4. **Apply**: Save the loaded content to your local `.github/` directories for immediate use in your projects

## Use Cases

### For Developers

- **Discover Best Practices**: Find proven Copilot customizations for your technology stack
- **Bootstrap Projects**: Quickly add domain-specific agents and instructions to new projects
- **Stay Updated**: Access the latest Copilot customizations from the community
- **Compare Implementations**: See what customizations exist before creating your own

### For Teams

- **Standardize Workflows**: Share and synchronize team Copilot configurations
- **Knowledge Sharing**: Distribute best practices through curated instructions
- **Onboarding**: Help new team members discover available Copilot resources

### Example Workflow

```text
1. Search for Python testing instructions:
   > #search_instructions keywords: python testing

2. Review results showing available pytest, unittest, and TDD instructions

3. Load a specific instruction:
   > #load_instruction filename: pytest-best-practices.instructions.md

4. Save to your project:
   Save content to .github/instructions/pytest-best-practices.instructions.md

5. Copilot now uses these instructions in your project!
```

## Architecture

Copilot AgentOps implements a hybrid MCP server architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                   MCP Client (VS Code/Visual Studio)         │
└─────────────────────────┬───────────────────────────────────┘
                          │
                ┌─────────┴──────────┐
                │  STDIO or HTTP     │
                └─────────┬──────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│              Copilot AgentOps MCP Server                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Tools: search_instructions, load_instruction        │  │
│  │  Prompts: get_search_prompt                          │  │
│  │  Service: MetadataService (metadata caching)         │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
                ┌─────────┴──────────┐
                │  GitHub Repository │
                │  copilot-agentops  │
                └────────────────────┘
```

### Key Components

- **MetadataService**: Caches and serves metadata from the copilot-agentops repository
- **MetadataTool**: Implements search and load operations
- **MetadataPrompt**: Provides structured prompt templates
- **Hybrid Runtime**: Supports both STDIO (process communication) and HTTP (REST API) modes

## Deployment Options

### 1. Local Development (STDIO)

```bash
cd hybridapp
dotnet run --project ./src/CopilotAgentOpsMcp.HybridApp
```

### 2. Local HTTP Server

```bash
dotnet run --project ./src/CopilotAgentOpsMcp.HybridApp -- --http
# Server runs at http://localhost:5250
```

### 3. Container (STDIO)

```bash
docker build -f Dockerfile.copilot-agentops -t copilot-agentops:latest .
docker run -i --rm copilot-agentops:latest
```

### 4. Container (HTTP)

```bash
docker run -i --rm -p 8080:8080 ajilantony/copilot-agentops:latest --http
# Server runs at http://localhost:8080
```

### 5. Azure Container Apps

```bash
cd hybridapp
azd auth login
azd up
```

After deployment, get your server URL:

```bash
azd env get-value AZURE_RESOURCE_MCP_COPILOT_AGENTOPS_FQDN
```

## Configuration

Configure the MCP server by copying the appropriate configuration to `.vscode/mcp.json`:

```bash
# For local STDIO mode
cp hybridapp/.vscode/mcp.stdio.local.json .vscode/mcp.json

# For local HTTP mode
cp hybridapp/.vscode/mcp.http.local.json .vscode/mcp.json

# For containerized mode
cp hybridapp/.vscode/mcp.stdio.container.json .vscode/mcp.json

# For Azure deployment
cp hybridapp/.vscode/mcp.http.remote.json .vscode/mcp.json
```

Then restart VS Code or reload the MCP server from the Command Palette (`MCP: List Servers`).

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://docs.docker.com/get-started/get-docker/) (for container deployments)
- [Visual Studio Code](https://code.visualstudio.com/) with [C# Dev Kit](https://marketplace.visualstudio.com/items/?itemName=ms-dotnettools.csdevkit)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) and [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) (for Azure deployment)

## Project Structure

```
hybridapp/
├── src/
│   └── CopilotAgentOpsMcp.HybridApp/
│       ├── Program.cs                      # Application entry point
│       ├── Configurations/                 # App settings
│       ├── Models/                         # Data models
│       ├── Services/                       # MetadataService implementation
│       ├── Tools/                          # MCP tools
│       ├── Prompts/                        # MCP prompts
│       └── metadata.json                   # Cached repository metadata
├── infra/                                  # Azure infrastructure (Bicep)
├── .vscode/                                # MCP configuration templates
└── README.md                               # Sample documentation
```

## Resources

- [Model Context Protocol Specification](https://spec.modelcontextprotocol.io/)
- [Copilot AgentOps Repository](https://github.com/ajilantony/copilot-agentops)
- [.NET MCP SDK Documentation](https://learn.microsoft.com/dotnet/ai/mcp-dotnet-sdk)
- [Full Documentation](hybridapp/README.md)

---

**Note**: This MCP server retrieves content from the [copilot-agentops](https://github.com/ajilantony/copilot-agentops) repository.

