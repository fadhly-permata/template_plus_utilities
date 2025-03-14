using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IDC.Template.Utilities.Middlewares.Swagger;

/// <summary>
/// Filter to set default group for Swagger/OpenAPI endpoints that don't have any group specified.
/// </summary>
/// <remarks>
/// This filter implements IDocumentFilter to modify the OpenAPI document and assigns 'v1' tag
/// to any operation that doesn't have a version tag.
/// </remarks>
/// <example>
/// <code>
/// builder.Services.AddSwaggerGen(options =>
/// {
///     options.DocumentFilter&lt;DefaultGroupDocFilter&gt;();
/// });
/// </code>
/// </example>
public class DefaultGroupDocFilter : IDocumentFilter
{
    /// <summary>
    /// Applies default grouping to the Swagger/OpenAPI document operations.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    /// <remarks>
    /// Iterates through all API operations and adds a 'v1' tag to any operation
    /// that doesn't have a version tag specified.
    /// </remarks>
    /// <example>
    /// <code>
    /// var filter = new DefaultGroupDocFilter();
    /// filter.Apply(swaggerDoc: openApiDoc, context: filterContext);
    /// </code>
    /// </example>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) =>
        swaggerDoc
            .Paths.SelectMany(static path => path.Value.Operations)
            .Where(static operation => !operation.Value.Tags.Any())
            .ToList()
            .ForEach(static operation => operation.Value.Tags = [new OpenApiTag { Name = "Main" }]);
}
