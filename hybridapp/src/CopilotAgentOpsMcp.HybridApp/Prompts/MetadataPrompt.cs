using System.ComponentModel;

using ModelContextProtocol.Server;

namespace CopilotAgentOpsMcp.HybridApp.Prompts;

/// <summary>
/// This provides interfaces for metadata prompts.
/// </summary>
public interface IMetadataPrompt
{
    /// <summary>
    /// Gets a prompt for searching the Copilot AgentOps content.
    /// </summary>
    /// <param name="keyword">The keyword to search for.</param>
    /// <returns>A formatted search prompt.</returns>
    string GetSearchPrompt(string keyword);

    /// <summary>
    /// Gets a prompt for the install workflow.
    /// </summary>
    /// <param name="artifactType">The type of artifact (instruction, prompt, agent, chatmode, or 'any').</param>
    /// <param name="keyword">The keyword to search for.</param>
    /// <returns>A formatted install workflow prompt.</returns>
    string GetInstallWorkflowPrompt(string artifactType, string keyword);
}

/// <summary>
/// This represents the prompts entity for the Copilot AgentOps repository.
/// </summary>
[McpServerPromptType]
public class MetadataPrompt : IMetadataPrompt
{
    /// <inheritdoc />
    [McpServerPrompt(Name = "get_search_prompt", Title = "Prompt for searching the Copilot AgentOps content")]
    [Description("Get a prompt for searching the Copilot AgentOps content.")]
    public string GetSearchPrompt(
        [Description("The keyword to search for")] string keyword)
    {
        return $"""
        Please search all the chatmodes, instructions, prompts, agents, and collections that are related to the search keyword, `{keyword}`.

        Here's the process to follow:

        1. Use the `copilot-agentops` MCP server.
        1. Search all chatmodes, instructions, prompts, and agents for the keyword provided.
        1. DO NOT load any chatmodes, instructions, prompts, or agents from the MCP server until the user asks to do so.
        1. Scan local chatmodes, instructions, prompts, and agents markdown files in `.github/chatmodes`, `.github/instructions`, `.github/prompts`, and `.github/agents` directories respectively.
        1. Compare existing chatmodes, instructions, prompts, and agents with the search results.
        1. Provide a structured response in a table format that includes the already exists, mode (chatmodes, instructions, prompts or agents), filename, title and description of each item found. 
           Here's an example of the table format:

           | Exists | Mode         | Filename                      | Title         | Description   |
           |--------|--------------|-------------------------------|---------------|---------------|
           | ✅    | chatmodes    | chatmode1.chatmode.md         | ChatMode 1    | Description 1 |
           | ❌    | instructions | instruction1.instructions.md  | Instruction 1 | Description 1 |
           | ✅    | prompts      | prompt1.prompt.md             | Prompt 1      | Description 1 |
           | ❌    | agents       | agent1.agent.md               | Agent 1       | Description 1 |

           ✅ indicates that the item already exists in this repository, while ❌ indicates that it does not.

        1. If any item doesn't exist in the repository, ask which item the user wants to install.
        1. If the user wants to install it, use the install_artifact tool with the appropriate mode and filename to save to .github/[mode]/ directory.
        1. Include a search for Collections, which are made up of multiple chatmodes, instructions, prompts, and agents, but contain a name, description and tags.
        1. If there are any that match, provide a summary of the collection, including its name, description, tags, and the items it contains.
        1. Do NOT automatically install or save any items. Wait for explicit user confirmation.
        1. Use the table from above to show the items in the collection.
        """;
    }

    /// <summary>
    /// Gets a prompt to display details about a specific collection.
    /// </summary>
    [McpServerPrompt(Name = "search_collections", Title = "Prompt to search Collections in the Copilot AgentOps repo by keyword")]
    [Description("Prompt to search Collections in the Copilot AgentOps repo by keyword.")]
    public string SearchCollectionsPrompt([
        Description("The keyword to search the Collections for")]
        string keyword)
    {
        return $"""
        Please fetch the collection that matches `{keyword}` in the ID, name, description, or tags from the copilot-agentops MCP server collections endpoint.

        Provide a human-friendly summary that includes:
        - Collection name and description
        - Tags
        - A breakdown of items grouped by kind (chat-mode, instruction, prompt, agent) with filenames

        Do NOT automatically install or save any items. Wait for explicit user confirmation.
        """;
    }

    /// <inheritdoc />
    [McpServerPrompt(Name = "install_workflow", Title = "Workflow for installing artifacts by search")]
    [Description("Guides the AI to search and install artifacts when user expresses install intent like 'Install instruction for dotnet'.")]
    public string GetInstallWorkflowPrompt(
        [Description("Type of artifact: instruction, prompt, agent, chatmode, or 'any' for all types")] string artifactType,
        [Description("Keyword to search for relevant artifacts")] string keyword)
    {
        return $"""
        User wants to install {artifactType} related to "{keyword}". Follow this workflow:

        **Step 1: Search for artifacts**
        - Use the `search_instructions` tool with keyword: `{keyword}`
        - This will return matching chatmodes, instructions, prompts, and agents

        **Step 2: Filter by type (if specified)**
        - If artifactType is "{artifactType}" (not "any"), focus on that type
        - Otherwise, show all relevant matches

        **Step 3: Check for existing files**
        - Scan the local `.github/` directory for existing artifacts:
          - `.github/chatmodes/` for chatmodes
          - `.github/instructions/` for instructions
          - `.github/prompts/` for prompts
          - `.github/agents/` for agents

        **Step 4: Present results in a table**
        Format results as a table with match relevance:

        | Match | Type         | Filename                        | Title              | Description          | Status          |
        |-------|--------------|----------------------------------|-------------------|----------------------|-----------------|
        | ⭐⭐⭐ | instructions | dotnet-best-practices.instruction.md | .NET Best Practices | Coding standards... | Not installed |
        | ⭐⭐   | prompts      | code-review.prompt.md            | Code Review       | Review checklist... | Already exists  |

        Match indicators:
        - ⭐⭐⭐ = Highly relevant (exact keyword match in title)
        - ⭐⭐ = Relevant (keyword in description)
        - ⭐ = Somewhat relevant (related terms)

        **Step 5: Make recommendation**
        - If only 1 result: "Found a perfect match! Would you like to install it?"
        - If clear best match (⭐⭐⭐): "The best match is [filename]. Would you like to install it?"
        - If multiple good matches: "Found [N] matches. Which one would you like to install?" (show table)
        - If no matches: "No {artifactType} found for '{keyword}'. Try a different search term."

        **Step 6: Install upon confirmation**
        Once user confirms which artifact to install, use the `install_artifact` tool:
        - **mode**: Convert type to enum (Instructions, Prompts, Agents, or ChatModes)
        - **filename**: The exact filename from search results
        - **targetRepoRoot**: ".github" (default)

        Example call:
        ```
        install_artifact(
          mode: Instructions,
          filename: "dotnet-best-practices.instruction.md",
          targetRepoRoot: ".github"
        )
        ```

        **Step 7: Confirm installation**
        After successful installation, report:
        "✅ Installed to: [relative path, e.g., .github/instructions/dotnet-best-practices.instruction.md]"

        **Important notes:**
        - Do NOT install without user confirmation
        - Show the table even if there's only one match
        - If artifact already exists, ask if user wants to overwrite
        - Handle errors gracefully with helpful messages
        """;
    }
}
