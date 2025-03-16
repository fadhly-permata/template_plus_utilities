using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IDC.Template.Utilities.Middlewares.Swagger;

/// <summary>
/// Filter to set default group for Swagger/OpenAPI endpoints that don't have any group specified.
/// </summary>
/// <remarks>
/// This filter implements IDocumentFilter to modify the OpenAPI document and assigns endpoints to appropriate groups.
/// If an endpoint doesn't have a group specified, it will be assigned to the 'Main' group by default.
/// Demo endpoints are those under the '/api/epg/' path.
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
    /// Groups endpoints based on their path and tags:
    /// - Uses existing tags if available
    /// - For untagged endpoints under '/api/epg/', groups under 'Demo'
    /// - For other untagged endpoints, groups under 'Main'
    /// </remarks>
    /// <example>
    /// <code>
    /// var filter = new DefaultGroupDocFilter();
    /// filter.Apply(swaggerDoc: openApiDoc, context: filterContext);
    /// </code>
    /// </example>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.ToDictionary(x => x.Key, x => x.Value);
        swaggerDoc.Paths.Clear();

        var isDemoDoc = swaggerDoc.Info.Title.Contains("Demo", StringComparison.OrdinalIgnoreCase);

        foreach (var path in paths)
        {
            var isDemoEndpoint = path.Key.StartsWith(
                "/api/demo/",
                StringComparison.OrdinalIgnoreCase
            );

            if (isDemoDoc == isDemoEndpoint)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);

                foreach (var operation in path.Value.Operations)
                {
                    if (!operation.Value.Tags.Any())
                    {
                        operation.Value.Tags =
                        [
                            new OpenApiTag { Name = isDemoDoc ? "Demo" : "Main" }
                        ];
                    }
                }
            }
        }

        swaggerDoc.Tags = swaggerDoc
            .Paths.SelectMany(p => p.Value.Operations.SelectMany(o => o.Value.Tags))
            .DistinctBy(t => t.Name)
            .OrderBy(t => t.Name)
            .ToList();
    }
}
