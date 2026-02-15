using McpSamples.Shared.Configurations;

using Microsoft.OpenApi.Models;

namespace McpSamples.CopilotAgentOps.HybridApp.Configurations;

/// <summary>
/// This represents the application settings for Copilot AgentOps app.
/// </summary>
public class CopilotAgentOpsAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo OpenApi { get; set; } = new()
    {
        Title = "MCP Copilot AgentOps",
        Version = "1.0.0",
        Description = "A simple MCP server for searching and loading custom instructions from GitHub's copilot-agentops repository."
    };
}
