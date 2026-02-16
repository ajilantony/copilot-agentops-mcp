using System.Text.Json;

using CopilotAgentOpsMcp.HybridApp.Configurations;
using CopilotAgentOpsMcp.HybridApp.Services;
using CopilotAgentOpsMcp.Shared.Configurations;
using CopilotAgentOpsMcp.Shared.Extensions;
using CopilotAgentOpsMcp.Shared.OpenApi;

var useStreamableHttp = AppSettings.UseStreamableHttp(Environment.GetEnvironmentVariables(), args);

IHostApplicationBuilder builder = useStreamableHttp
                                ? WebApplication.CreateBuilder(args)
                                : Host.CreateApplicationBuilder(args);

builder.Services.AddAppSettings<CopilotAgentOpsAppSettings>(builder.Configuration, args);

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true
};
builder.Services.AddSingleton(options);
builder.Services.AddHttpClient<IMetadataService, MetadataService>();

if (useStreamableHttp == true)
{
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddOpenApi("swagger", o =>
    {
        o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
        o.AddDocumentTransformer<McpDocumentTransformer<CopilotAgentOpsAppSettings>>();
    });
    builder.Services.AddOpenApi("openapi", o =>
    {
        o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
        o.AddDocumentTransformer<McpDocumentTransformer<CopilotAgentOpsAppSettings>>();
    });
}

IHost app = builder.BuildApp(useStreamableHttp);

if (useStreamableHttp == true)
{
    (app as WebApplication)!.MapOpenApi("/{documentName}.json");
}

await app.RunAsync();
