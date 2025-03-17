using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IDC.Template.Utilities.Middlewares.Swagger;

/// <summary>
/// Filter to sort Swagger/OpenAPI endpoints alphabetically.
/// </summary>
/// <remarks>
/// This filter implements IDocumentFilter to modify the OpenAPI document and sort all endpoints by their paths.
/// </remarks>
/// <example>
/// <code>
/// builder.Services.AddSwaggerGen(options =>
/// {
///     options.DocumentFilter&lt;AlphabeticalSortDocumentFilter&gt;();
/// });
/// </code>
/// </example>
public class DocumentSortDocFilter : IDocumentFilter
{
    /// <summary>
    /// Applies alphabetical sorting to the Swagger/OpenAPI document paths.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    /// <remarks>
    /// Orders all API endpoints alphabetically by their path and reconstructs the Paths collection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var filter = new AlphabeticalSortDocumentFilter();
    /// filter.Apply(swaggerDoc: openApiDoc, context: filterContext);
    /// </code>
    /// </example>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc
            .Paths.OrderBy(static x => x.Key)
            .ToDictionary(static x => x.Key, static x => x.Value);

        swaggerDoc.Paths.Clear();
        foreach (var path in paths)
            swaggerDoc.Paths.Add(path.Key, path.Value);
    }
}
