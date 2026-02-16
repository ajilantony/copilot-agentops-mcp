using System.Text.Json.Serialization;

namespace CopilotAgentOps.HybridApp.Tools;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InstructionMode
{
    [JsonStringEnumMemberName("undefined")]
    Undefined,

    [JsonStringEnumMemberName("chatmodes")]
    ChatModes,

    [JsonStringEnumMemberName("instructions")]
    Instructions,

    [JsonStringEnumMemberName("prompts")]
    Prompts,

    [JsonStringEnumMemberName("agents")]
    Agents
}