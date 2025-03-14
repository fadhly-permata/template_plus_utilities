using Newtonsoft.Json.Linq;

namespace IDC.Utilities;

/// <summary>
/// Generates API controllers and endpoints from JSON configuration.
/// </summary>
/// <remarks>
/// Provides functionality to generate API controllers and endpoints at runtime using a JSON configuration file.
/// The configuration file should define:
/// - API settings (method, route, tags, description)
/// - Route parameters, query parameters, and request body
/// - Database settings (engine, query type, return type)
/// - Message templates for success/failure responses
/// - Optional callback methods
///
/// Example usage:
/// <code>
/// var generator = new EndPointGenerator(configPath)
///     .SetOutputDirectory("/Controllers/Generated")
///     .GenerateControllers();
/// </code>
/// </remarks>
public sealed class EndPointGenerator
{
    private readonly JObject _config;
    private string _outputDirectory = string.Empty;

    public EndPointGenerator(string configPath)
    {
        ArgumentNullException.ThrowIfNull(argument: configPath);
        if (!File.Exists(path: configPath))
            throw new FileNotFoundException(message: $"Configuration file not found: {configPath}");

        var jsonContent = File.ReadAllText(path: configPath);
        _config = JObject.Parse(json: jsonContent);
    }

    public EndPointGenerator SetOutputDirectory(string path)
    {
        _outputDirectory = path;
        return this;
    }

    public EndPointGenerator GenerateControllers()
    {
        foreach (var endpoint in _config.Properties())
        {
            if (endpoint.Name == "generated_dir")
                continue;
            GenerateController(endpoint.Name, endpoint.Value as JObject ?? []);
        }
        return this;
    }

    private void GenerateController(string name, JObject config)
    {
        var apiSettings = config["apiSettings"] as JObject ?? [];
        var template = BuildControllerTemplate(name: name, config: config);
        var filePath = Path.Combine(path1: _outputDirectory, path2: $"{name}Controller.cs");

        Directory.CreateDirectory(path: _outputDirectory);
        File.WriteAllText(path: filePath, contents: template);
    }

    private string BuildControllerTemplate(string name, JObject config)
    {
        var apiSettings = config["apiSettings"] as JObject ?? [];
        var dbSettings = config["dbSettings"] as JObject ?? [];
        var routeParams = config["route_params"] as JArray;
        var queryParams = config["query_params"] as JArray;
        var bodyRequest = config["body_request"] as JArray;

        return $$"""
            using Microsoft.AspNetCore.Mvc;
            using IDC.Utilities.Models.API;
            using IDC.Utilities.Data;
            using Newtonsoft.Json.Linq;

            namespace IDC.Template.Controllers.Generated;

            [ApiController]
            [Route("api/epg/[controller]")]
            public class {{name}}Controller : ControllerBase
            {
                private readonly SQLiteHelper _sqliteHelper;
                private readonly PostgreSQLHelper _postgreHelper;
                private readonly SystemLogging _systemLogging;
                private readonly Language _language;

                public {{name}}Controller(
                    SQLiteHelper sqliteHelper,
                    PostgreSQLHelper postgreHelper,
                    SystemLogging systemLogging,
                    Language language)
                {
                    _sqliteHelper = sqliteHelper;
                    _postgreHelper = postgreHelper;
                    _systemLogging = systemLogging;
                    _language = language;
                }

                {{GenerateEndpointMethod(config: config)}}
            }
            """;
    }

    private string GenerateEndpointMethod(JObject config)
    {
        var apiSettings = config["apiSettings"] as JObject ?? [];
        var route = apiSettings["route"]?.ToString() ?? string.Empty;
        var method = apiSettings["method"]?.ToString()?.ToUpper() ?? "GET";
        var isAsync = apiSettings["use_async"]?.ToObject<bool>() ?? false;

        return $$"""
            [Http{{method}}("{{route}}")]
            public async Task<IActionResult> Execute(
                {{GenerateParameters(config: config)}}
            )
            {
                try
                {
                    var result = await {{GenerateDatabaseOperation(config: config)}};

                    var response = new APIResponseData<object>()
                        .ChangeStatus(_language.GetMessage(key: "api.status.success"))
                        .ChangeMessage(_language.GetMessage(key: "{{config["messages"]?[
                "success"
            ]}}"))
                        .ChangeData(data: result);

                    {{GenerateCallbacks(config: config, type: "success")}}

                    return Ok(value: response);
                }
                catch (Exception ex)
                {
                    var response = new APIResponseData<object>()
                        .ChangeStatus(_language.GetMessage(key: "api.status.failed"))
                        .ChangeMessage(ex: ex, systemLogging: _systemLogging);

                    {{GenerateCallbacks(config: config, type: "failed")}}

                    return BadRequest(error: response);
                }
            }
            """;
    }

    private string GenerateParameters(JObject config)
    {
        var parameters = new List<string>();

        if (config["route_params"] is JArray routeParams)
        {
            parameters.AddRange(
                routeParams.Select(p =>
                    $"[FromRoute] {GetCSharpType(p["type"]?.ToString())} {p["name"]}"
                )
            );
        }

        if (config["query_params"] is JArray queryParams)
        {
            parameters.AddRange(
                queryParams.Select(p =>
                    $"[FromQuery] {GetCSharpType(p["type"]?.ToString())} {p["name"]}"
                )
            );
        }

        if (config["body_request"] is JArray bodyParams)
        {
            parameters.AddRange(
                bodyParams.Select(p =>
                    $"[FromBody] {GetCSharpType(p["type"]?.ToString())} {p["name"]}"
                )
            );
        }

        return string.Join(", ", parameters);
    }

    private string GenerateDatabaseOperation(JObject config)
    {
        var dbSettings = config["dbSettings"] as JObject ?? [];
        var engine = dbSettings["engine"]?.ToString()?.ToLower();
        var queryType = dbSettings["query_type"]?.ToString()?.ToLower();
        var query = dbSettings["query"]?.ToString();

        return engine switch
        {
            "sqlitehelper" => GenerateSQLiteOperation(queryType, query),
            "postgrehelper" => GeneratePostgreOperation(queryType, query),
            _ => "Task.FromResult<object>(null)"
        };
    }

    private string GenerateSQLiteOperation(string? queryType, string? query) =>
        queryType switch
        {
            "scalar"
                => $"Task.FromResult(_sqliteHelper.Connect(useTransaction: true).ExecuteScalar(query: \"{query}\", out object? result).TransactionCommit().GetResult())",
            "nonquery"
                => $"Task.FromResult(_sqliteHelper.Connect(useTransaction: true).ExecuteNonQuery(query: \"{query}\", out int result).TransactionCommit().GetResult())",
            "query"
                => $"Task.FromResult(_sqliteHelper.Connect().ExecuteQuery(query: \"{query}\").GetResult())",
            _ => "Task.FromResult<object>(null)"
        };

    private string GeneratePostgreOperation(string? queryType, string? query) =>
        queryType switch
        {
            "scalar" => $"_postgreHelper.ExecuteScalarAsync(query: \"{query}\")",
            "nonquery" => $"_postgreHelper.ExecuteNonQueryAsync(query: \"{query}\")",
            "query" => $"_postgreHelper.ExecuteQueryAsync(query: \"{query}\")",
            _ => "Task.FromResult<object>(null)"
        };

    private string GenerateCallbacks(JObject config, string type)
    {
        if (config["callback"] is not JObject callbacks)
            return string.Empty;
        return callbacks[$"on_{type}"]?.ToString() ?? string.Empty;
    }

    private static string GetCSharpType(string? jsonType) =>
        jsonType?.ToLower() switch
        {
            "int" => "int",
            "varchar" => "string",
            "decimal" => "decimal",
            "datetime" => "DateTime",
            "bool" => "bool",
            "float" => "float",
            "double" => "double",
            "jobject" => "JObject",
            _ => "object"
        };
}
