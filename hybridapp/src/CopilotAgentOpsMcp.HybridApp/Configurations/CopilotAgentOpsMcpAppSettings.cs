using CopilotAgentOpsMcp.Shared.Configurations;

using Microsoft.OpenApi.Models;

namespace CopilotAgentOpsMcp.HybridApp.Configurations;

/// <summary>
/// This represents the application settings for Copilot AgentOps app.
/// </summary>
public class CopilotAgentOpsMcpAppSettings : AppSettings
{
    /// <inheritdoc />
    public override OpenApiInfo OpenApi { get; set; } = new()
    {
        Title = "Copilot AgentOps MCP",
        Version = "1.0.0",
        Description = "A simple MCP server for searching and loading custom instructions from GitHub's copilot-agentops repository."
    };
}
