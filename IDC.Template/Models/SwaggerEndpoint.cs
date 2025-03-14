namespace IDC.Template.Models;

/// <summary>
/// Represents a Swagger/OpenAPI endpoint configuration
/// </summary>
/// <remarks>
/// Used to define external Swagger documentation endpoints that will be included in the Swagger UI
/// </remarks>
/// <example>
/// <code>
/// var endpoint = new SwaggerEndpoint
/// {
///     Name = "Pet Store API",
///     URL = "https://petstore.swagger.io/v2/swagger.json"
/// };
/// </code>
/// </example>
public class SwaggerEndpoint
{
    /// <summary>Gets or sets the name of the Swagger endpoint</summary>
    /// <value>The display name in Swagger UI</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the URL of the Swagger JSON</summary>
    /// <value>The full URL to the swagger.json file</value>
    public string URL { get; set; } = string.Empty;
}
