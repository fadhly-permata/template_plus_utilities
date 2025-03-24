using IDC.Utilities.Models.Data;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data.MongoDB;

public sealed partial class Repository<T>
{
    private static (
        List<(string ArrayName, int Index, string Field)> arrayPaths,
        List<string> regularPaths
    ) SeparatePaths(string[] paths)
    {
        var arrayPaths = new List<(string ArrayName, int Index, string Field)>();
        var regularPaths = new List<string>();

        foreach (var path in paths)
        {
            var parts = path.Split(separator: '.');
            if (parts.Length >= 2 && int.TryParse(s: parts[1], result: out int index))
            {
                var field =
                    parts.Length > 2 ? string.Join(separator: '.', values: parts.Skip(2)) : "";

                arrayPaths.Add(item: (ArrayName: parts[0], Index: index, Field: field));
                regularPaths.Add(item: parts[0]); // Add the array itself to regular paths
            }
            else
            {
                regularPaths.Add(item: path);
            }
        }

        return (arrayPaths, regularPaths);
    }

    private static JObject ProcessRegularPaths(
        JObject source,
        JObject target,
        IEnumerable<string> paths,
        List<(string ArrayName, int Index, string Field)> arrayPaths
    )
    {
        foreach (
            var path in paths.Where(predicate: p =>
                !arrayPaths.Any(predicate: ap => ap.ArrayName == p)
            )
        )
        {
            var pathParts = path.Split(separator: '.');
            var current = source;
            var resultTarget = target;

            for (int i = 0; i < pathParts.Length; i++)
            {
                var part = pathParts[i];

                if (i == pathParts.Length - 1)
                {
                    if (current[part] != null)
                    {
                        resultTarget[part] = current[part];
                    }
                }
                else
                {
                    if (resultTarget[part] == null && current[part] != null)
                    {
                        resultTarget[part] = current[part];
                    }
                    else if (resultTarget[part] == null)
                    {
                        resultTarget[part] = new JObject();
                    }

                    resultTarget = resultTarget[part]?.ToObject<JObject>() ?? [];
                    current = current[part]?.ToObject<JObject>() ?? [];
                }
            }
        }

        return target;
    }

    private static JObject ProcessArrayPaths(
        JObject source,
        JObject target,
        List<(string ArrayName, int Index, string Field)> arrayPaths
    )
    {
        foreach (var (arrayName, index, field) in arrayPaths)
        {
            if (source[arrayName] is JArray array && index < array.Count)
            {
                var arrayElement = array[index];

                if (!string.IsNullOrEmpty(field))
                {
                    var fieldValue = arrayElement[field];
                    if (fieldValue != null)
                    {
                        var arrayObj = new JObject { [field] = fieldValue };
                        target[arrayName] = new JArray { arrayObj };
                    }
                }
                else
                {
                    target[arrayName] = new JArray { arrayElement };
                }
            }
        }

        return target;
    }

    /// <summary>
    /// Asynchronously retrieves all documents from the collection with optional field selection.
    /// </summary>
    /// <param name="paths">Optional array of field paths to include in the result. Supports nested paths and array indexing.</param>
    /// <param name="callback">Optional callback action that receives the list of retrieved documents.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation that returns a list of documents.</returns>
    /// <remarks>
    /// Retrieves documents from the MongoDB collection with selective field inclusion.
    /// Supports both regular nested paths and array element access.
    ///
    /// > [!CAUTION]
    /// > Use with caution on large collections as it may impact performance.
    ///
    /// > [!TIP]
    /// > Consider using pagination for large datasets.
    ///
    /// Path formats:
    /// - Regular path: "field.nestedField"
    /// - Array element: "arrayField.0.field"
    ///
    /// Example:
    /// <code>
    /// // C# Implementation
    /// var documents = await repository.GetAllAsync(
    ///     paths: new[] {
    ///         "master_data.cf_los_app_no",
    ///         "data.dataResponseDukcapil.namaLengkap",
    ///         "installment_detail.0.period"
    ///     },
    ///     callback: results => Console.WriteLine($"Retrieved {results.Count} documents")
    /// );
    /// </code>
    ///
    /// Sample Response:
    /// <code>
    /// {
    ///   "data": [
    ///     {
    ///       "master_data": {
    ///         "cf_los_app_no": "APP123"
    ///       },
    ///       "data": {
    ///         "dataResponseDukcapil": {
    ///           "namaLengkap": "John Doe"
    ///         }
    ///       },
    ///       "installment_detail": [
    ///         {
    ///           "period": 1
    ///         }
    ///       ]
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.find/">MongoDB find Operation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/project-fields-from-query-results/">MongoDB Projection Documentation</seealso>
    public async Task<List<T>> GetAllAsync(
        string[]? paths = null,
        Action<List<T>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        if (paths is null or { Length: 0 })
        {
            var (_, results) = await _mongoHelper.FindAsync(
                collection: GetCollection(collectionName: _collectionName),
                filter: [],
                cancellationToken: cancellationToken
            );

            var typedResults = results
                .Select(selector: x => x.ToObject<T>()!)
                .Where(predicate: x => x is not null)
                .ToList();

            callback?.Invoke(obj: typedResults);
            return typedResults;
        }

        var (arrayPaths, regularPaths) = SeparatePaths(paths);

        var (_, pathResults) = await _mongoHelper.FindPathsAsync(
            collection: GetCollection(collectionName: _collectionName),
            filter: [],
            paths: [.. regularPaths.Distinct()],
            cancellationToken: cancellationToken
        );

        var typedPathResults =
            pathResults
                .Select(selector: x =>
                {
                    var resultObj = new JObject();
                    resultObj = Repository<T>.ProcessRegularPaths(
                        source: x,
                        target: resultObj,
                        paths: regularPaths,
                        arrayPaths: arrayPaths
                    );
                    resultObj = Repository<T>.ProcessArrayPaths(
                        source: x,
                        target: resultObj,
                        arrayPaths: arrayPaths
                    );
                    return resultObj.ToObject<T>();
                })
                .Where(predicate: x => x is not null)
                .ToList() as List<T>;

        callback?.Invoke(obj: typedPathResults);
        return typedPathResults;
    }
}
