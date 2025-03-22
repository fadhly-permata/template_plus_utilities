using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for JObject to enhance JSON manipulation capabilities.
/// </summary>
/// <remarks>
/// Extends JObject functionality with type-safe property access, deep merging, and property manipulation.
///
/// Key features:
/// - Type-safe property access using dot notation
/// - Deep merging of JObjects with array handling
/// - Selective property removal with recursive support
/// - Null-safe operations with configurable fallbacks
///
/// > [!IMPORTANT]
/// > All operations create new instances and do not modify the original objects.
///
/// > [!NOTE]
/// > Uses Newtonsoft.Json.Linq for JSON manipulation.
/// </remarks>
public static class JObjectExtensions
{
    /// <summary>
    /// Retrieves a strongly-typed value from a JObject using dot notation path with default fallback.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="source">The source JObject.</param>
    /// <param name="path">The dot notation path (e.g., "user.address.city").</param>
    /// <param name="defaultValue">The fallback value if path doesn't exist or conversion fails.</param>
    /// <returns>The value at the specified path converted to type T, or defaultValue if not found.</returns>
    /// <remarks>
    /// Provides safe navigation through nested JSON structures with graceful fallback handling.
    ///
    /// > [!NOTE]
    /// > Returns defaultValue in these cases:
    /// > - Source is null
    /// > - Path is empty/null
    /// > - Path doesn't exist
    /// > - Type conversion fails
    ///
    /// > [!TIP]
    /// > Use null as defaultValue to get nullable behavior.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""user"": {
    ///         ""profile"": {
    ///             ""age"": 30,
    ///             ""isActive"": true
    ///         }
    ///     }
    /// }");
    ///
    /// // Basic usage
    /// int age = json.PropGet(path: "user.profile.age", defaultValue: 0);
    ///
    /// // Nullable with deep path
    /// bool? isActive = json.PropGet&lt;bool&gt;(
    ///     path: "user.profile.isActive",
    ///     defaultValue: null
    /// );
    ///
    /// // Non-existent path with default
    /// string name = json.PropGet(
    ///     path: "user.profile.name",
    ///     defaultValue: "Unknown"
    /// );
    /// </code>
    /// </example>
    public static T PropGet<T>(this JObject source, string path, T? defaultValue)
    {
        if (source == null || string.IsNullOrEmpty(value: path))
            return defaultValue ?? default!;

        try
        {
            var token = source.SelectToken(path: path);
            return token != null
                ? token.Value<T>() ?? defaultValue ?? default!
                : defaultValue ?? default!;
        }
        catch
        {
            return defaultValue ?? default!;
        }
    }

    /// <summary>
    /// Retrieves a strongly-typed value from a JObject using dot notation path with optional error handling.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="source">The source JObject.</param>
    /// <param name="path">The dot notation path (e.g., "user.address.city").</param>
    /// <param name="onNullMessage">Custom error message format when path is not found.</param>
    /// <param name="throwOnNull">Whether to throw KeyNotFoundException when path is not found.</param>
    /// <returns>The value at the specified path converted to type T, or default if not found.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when path is not found and throwOnNull is true.</exception>
    /// <remarks>
    /// Provides controlled error handling for missing JSON properties.
    ///
    /// > [!WARNING]
    /// > Setting throwOnNull to true changes the method's behavior from returning default to throwing exceptions.
    ///
    /// > [!TIP]
    /// > Use {0} in onNullMessage as a placeholder for the path value.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""settings"": {
    ///         ""timeout"": 30,
    ///         ""retries"": null
    ///     }
    /// }");
    ///
    /// // Basic usage
    /// int? timeout = json.PropGet&lt;int&gt;(path: "settings.timeout");
    ///
    /// // With custom error message
    /// int? retries = json.PropGet&lt;int&gt;(
    ///     path: "settings.retries",
    ///     onNullMessage: "Required setting {0} not found"
    /// );
    ///
    /// // With exception throwing
    /// try
    /// {
    ///     int maxConnections = json.PropGet&lt;int&gt;(
    ///         path: "settings.maxConnections",
    ///         throwOnNull: true
    ///     );
    /// }
    /// catch (KeyNotFoundException ex)
    /// {
    ///     // Handle missing property
    /// }
    /// </code>
    /// </example>
    public static T? PropGet<T>(
        this JObject source,
        string path,
        string onNullMessage = "JSON property not found: {0}",
        bool throwOnNull = false
    )
    {
        if (source == null || string.IsNullOrEmpty(value: path))
            return default;

        try
        {
            var token = source.SelectToken(path: path);
            if (token == null && (throwOnNull || !string.IsNullOrEmpty(value: onNullMessage)))
                throw new KeyNotFoundException(
                    message: string.Format(format: onNullMessage, arg0: path)
                );

            return token != null ? token.Value<T>() : default;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Merges two JObjects recursively with optional array handling.
    /// </summary>
    /// <param name="source">The base JObject.</param>
    /// <param name="other">The JObject to merge into the base.</param>
    /// <param name="mergeArrays">When true, arrays are combined. When false, arrays are replaced.</param>
    /// <param name="deepMerge">When true, performs a deep merge of the source object before merging.</param>
    /// <returns>A new JObject containing the merged result.</returns>
    /// <remarks>
    /// Performs a deep merge of two JObjects with the following rules:
    /// - Properties from 'other' override matching properties in 'source'
    /// - Nested objects are merged recursively
    /// - Arrays are either merged or replaced based on mergeArrays parameter
    ///
    /// > [!IMPORTANT]
    /// > Always creates a new JObject instance, preserving the original objects.
    ///
    /// > [!NOTE]
    /// > When mergeArrays is true, arrays are combined using a union operation,
    /// > removing duplicates based on value equality.
    /// </remarks>
    /// <example>
    /// <code>
    /// var source = JObject.Parse(@"{
    ///     ""config"": {
    ///         ""timeout"": 30,
    ///         ""endpoints"": [""api1"", ""api2""],
    ///         ""database"": {
    ///             ""port"": 5432
    ///         }
    ///     }
    /// }");
    ///
    /// var other = JObject.Parse(@"{
    ///     ""config"": {
    ///         ""timeout"": 60,
    ///         ""endpoints"": [""api3""],
    ///         ""database"": {
    ///             ""host"": ""localhost""
    ///         }
    ///     }
    /// }");
    ///
    /// // Merge with array replacement
    /// var replaced = source.PropMerge(
    ///     other: other,
    ///     mergeArrays: false
    /// );
    /// // Result: endpoints = ["api3"]
    ///
    /// // Merge with array combination
    /// var combined = source.PropMerge(
    ///     other: other,
    ///     mergeArrays: true
    /// );
    /// // Result: endpoints = ["api1", "api2", "api3"]
    /// </code>
    /// </example>
    public static JObject PropMerge(
        this JObject source,
        JObject other,
        bool mergeArrays = false,
        bool deepMerge = true
    )
    {
        var result = new JObject();
        source
            ?.Properties()
            .ToList()
            .ForEach(action: prop =>
                result.Add(
                    propertyName: prop.Name,
                    value: deepMerge ? prop.Value?.DeepClone() : prop.Value
                )
            );

        if (other == null)
            return result;

        foreach (var kvp in other)
        {
            if (!result.ContainsKey(propertyName: kvp.Key))
            {
                result.Add(
                    propertyName: kvp.Key,
                    value: deepMerge ? kvp.Value?.DeepClone() : kvp.Value
                );
                continue;
            }

            if (result[kvp.Key] is JObject sourceObj && kvp.Value is JObject otherObj)
                result[kvp.Key] = sourceObj.PropMerge(
                    other: otherObj,
                    mergeArrays: mergeArrays,
                    deepMerge: deepMerge
                );
            else if (
                mergeArrays
                && result[kvp.Key] is JArray sourceArray
                && kvp.Value is JArray otherArray
            )
                result[kvp.Key] = new JArray(content: sourceArray.Union(second: otherArray));
            else
                result[kvp.Key] = deepMerge ? kvp.Value?.DeepClone() : kvp.Value;
        }

        return result;
    }

    /// <summary>
    /// Removes properties from a JObject based on a predicate function.
    /// </summary>
    /// <param name="source">The source JObject.</param>
    /// <param name="predicate">Function determining if a property should be removed.</param>
    /// <param name="recursive">Whether to process nested objects and arrays.</param>
    /// <returns>A new JObject with matching properties removed.</returns>
    /// <remarks>
    /// Creates a new JObject with properties filtered based on the predicate.
    ///
    /// > [!IMPORTANT]
    /// > The original JObject remains unchanged.
    ///
    /// > [!NOTE]
    /// > When recursive is true, the predicate is applied to all nested objects
    /// > and objects within arrays.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""user"": {
    ///         ""id"": 1,
    ///         ""internal"": {
    ///             ""created"": ""2024-01-01"",
    ///             ""modified"": ""2024-01-02""
    ///         },
    ///         ""roles"": [
    ///             { ""id"": 1, ""internal"": true },
    ///             { ""id"": 2, ""internal"": false }
    ///         ]
    ///     }
    /// }");
    ///
    /// // Remove all properties named "internal"
    /// var filtered = json.PropRemove(
    ///     predicate: kvp => kvp.Key == "internal",
    ///     recursive: true
    /// );
    ///
    /// // Remove properties with null values
    /// var nonNull = json.PropRemove(
    ///     predicate: kvp => kvp.Value?.Type == JTokenType.Null,
    ///     recursive: true
    /// );
    /// </code>
    /// </example>
    public static JObject PropRemove(
        this JObject source,
        Func<KeyValuePair<string, JToken?>, bool> predicate,
        bool recursive = true
    )
    {
        if (source == null)
            return [];

        var result = new JObject();
        foreach (var kvp in source)
        {
            if (predicate(arg: kvp))
                continue;

            if (recursive && kvp.Value is JObject childObject)
                result.Add(
                    propertyName: kvp.Key,
                    value: childObject.PropRemove(predicate: predicate, recursive: true)
                );
            else if (recursive && kvp.Value is JArray array)
                result.Add(
                    propertyName: kvp.Key,
                    value: new JArray(
                        content: array.Select(selector: item =>
                            item is JObject obj
                                ? obj.PropRemove(predicate: predicate, recursive: true)
                                : item.DeepClone()
                        )
                    )
                );
            else
                result.Add(propertyName: kvp.Key, value: kvp.Value?.DeepClone());
        }

        return result;
    }

    /// <summary>
    /// Updates multiple properties in a JObject using a dictionary of path-value pairs.
    /// </summary>
    /// <param name="source">The source JObject to update.</param>
    /// <param name="updates">Dictionary containing path-value pairs for updates.</param>
    /// <returns>A new JObject with the updated properties.</returns>
    /// <remarks>
    /// Provides bulk property updates with the following features:
    /// - Creates a deep clone of the source object
    /// - Supports nested paths using dot notation
    /// - Automatically creates missing intermediate objects
    /// - Handles null values gracefully
    ///
    /// > [!IMPORTANT]
    /// > The original JObject remains unchanged as this method creates a new instance.
    ///
    /// > [!NOTE]
    /// > If source is null, returns an empty JObject.
    ///
    /// > [!TIP]
    /// > Use this method for batch updates instead of multiple single-property updates
    /// > for better performance.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""user"": {
    ///         ""name"": ""John"",
    ///         ""settings"": {
    ///             ""theme"": ""dark""
    ///         }
    ///     }
    /// }");
    ///
    /// var updates = new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["user.age"] = 30,
    ///     ["user.settings.language"] = "en-US",
    ///     ["user.settings.theme"] = null,
    ///     ["user.address.city"] = "New York"
    /// };
    ///
    /// var result = json.PropUpdate(updates: updates);
    /// </code>
    /// </example>
    public static JObject PropUpdate(this JObject source, Dictionary<string, object?> updates)
    {
        if (source == null)
            return [];

        var result = source.DeepClone() as JObject ?? [];

        foreach (var update in updates)
        {
            var paths = update.Key.Split(separator: '.');
            var current = result;

            for (int i = 0; i < paths.Length - 1; i++)
            {
                if (current[paths[i]] is not JObject)
                    current[paths[i]] = new JObject();

                current = (JObject)current[paths[i]]!;
            }

            current[paths[^1]] = update.Value != null ? JToken.FromObject(o: update.Value) : null;
        }

        return result;
    }

    /// <summary>
    /// Upserts (updates or inserts) a value in a JObject using a dot notation path.
    /// </summary>
    /// <param name="source">The source JObject to update.</param>
    /// <param name="path">The dot notation path to the target property.</param>
    /// <param name="value">The value to upsert at the specified path.</param>
    /// <returns>A new JObject with the upserted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when path is null or empty.</exception>
    /// <remarks>
    /// Provides single-property upsert functionality with these features:
    /// - Deep cloning of source object
    /// - Automatic creation of intermediate objects
    /// - Support for null values
    /// - Path validation
    ///
    /// > [!IMPORTANT]
    /// > Always validates the path parameter to ensure it's not null or empty.
    ///
    /// > [!NOTE]
    /// > Creates a new instance, preserving the original JObject.
    ///
    /// > [!TIP]
    /// > For multiple updates, consider using the dictionary-based overload
    /// > for better performance.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""user"": {
    ///         ""name"": ""John"",
    ///         ""profile"": {
    ///             ""role"": ""admin""
    ///         }
    ///     }
    /// }");
    ///
    /// // Update existing property
    /// var result1 = json.PropUpsert(
    ///     path: "user.name",
    ///     value: "John Doe"
    /// );
    ///
    /// // Insert new nested property
    /// var result2 = json.PropUpsert(
    ///     path: "user.profile.department",
    ///     value: "Engineering"
    /// );
    ///
    /// // Set property to null
    /// var result3 = json.PropUpsert(
    ///     path: "user.profile.role",
    ///     value: null
    /// );
    /// </code>
    /// </example>
    public static JObject PropUpsert(this JObject source, string path, object? value)
    {
        if (string.IsNullOrEmpty(value: path))
            throw new ArgumentNullException(
                paramName: nameof(path),
                message: "Path cannot be null or empty"
            );

        var result = source?.DeepClone() as JObject ?? [];
        var paths = path.Split(separator: '.');
        var current = result;

        for (int i = 0; i < paths.Length - 1; i++)
        {
            if (current[paths[i]] is not JObject)
                current[paths[i]] = new JObject();

            current = (JObject)current[paths[i]]!;
        }

        var lastPath = paths[^1];
        current[lastPath] = value != null ? JToken.FromObject(o: value) : null;

        return result;
    }

    /// <summary>
    /// Upserts (updates or inserts) multiple properties in a JObject using a dictionary of path-value pairs.
    /// </summary>
    /// <param name="source">The source JObject to update.</param>
    /// <param name="updates">Dictionary of path-value pairs for bulk upserts.</param>
    /// <returns>A new JObject with all upserted properties.</returns>
    /// <exception cref="ArgumentNullException">Thrown when updates dictionary is null.</exception>
    /// <remarks>
    /// Provides bulk upsert operations with these features:
    /// - Atomic updates (all or nothing)
    /// - Consistent behavior with single-property upserts
    /// - Deep cloning of source object
    ///
    /// > [!IMPORTANT]
    /// > Validates the updates dictionary to prevent null reference errors.
    ///
    /// > [!WARNING]
    /// > Order of updates matters when dealing with overlapping paths.
    ///
    /// > [!TIP]
    /// > Use this method for batch operations to improve performance.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""app"": {
    ///         ""name"": ""MyApp"",
    ///         ""config"": {
    ///             ""timeout"": 30
    ///         }
    ///     }
    /// }");
    ///
    /// var updates = new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["app.version"] = "1.0.0",
    ///     ["app.config.timeout"] = 60,
    ///     ["app.config.maxRetries"] = 3,
    ///     ["app.metadata.lastUpdate"] = DateTime.UtcNow
    /// };
    ///
    /// var result = json.PropUpsert(updates: updates);
    /// </code>
    /// </example>
    public static JObject PropUpsert(this JObject source, Dictionary<string, object?> updates)
    {
        if (updates == null)
            throw new ArgumentNullException(
                paramName: nameof(updates),
                message: "Updates dictionary cannot be null"
            );

        var result = source?.DeepClone() as JObject ?? [];

        foreach (var update in updates)
            result = result.PropUpsert(path: update.Key, value: update.Value);

        return result;
    }

    /// <summary>
    /// Clones a property from one path to another within the JObject.
    /// </summary>
    /// <param name="source">The source JObject.</param>
    /// <param name="sourcePath">The dot notation path of the property to clone.</param>
    /// <param name="destinationPath">The dot notation path where to place the clone.</param>
    /// <returns>A new JObject with the cloned property at the specified destination.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either path is null or empty.</exception>
    /// <remarks>
    /// Provides property cloning with these features:
    /// - Deep cloning of the selected property
    /// - Path validation
    /// - Automatic creation of destination path structure
    ///
    /// > [!IMPORTANT]
    /// > Both source and destination paths must be non-null and non-empty.
    ///
    /// > [!NOTE]
    /// > If source path doesn't exist, returns unchanged clone of original.
    ///
    /// > [!WARNING]
    /// > Existing values at the destination path will be overwritten.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{
    ///     ""source"": {
    ///         ""data"": {
    ///             ""id"": 123,
    ///             ""metadata"": {
    ///                 ""created"": ""2024-01-01""
    ///             }
    ///         }
    ///     }
    /// }");
    ///
    /// // Clone entire data object
    /// var result1 = json.PropClone(
    ///     sourcePath: "source.data",
    ///     destinationPath: "backup.data"
    /// );
    ///
    /// // Clone nested property
    /// var result2 = json.PropClone(
    ///     sourcePath: "source.data.metadata",
    ///     destinationPath: "audit.originalMetadata"
    /// );
    ///
    /// // Clone to existing path (overwrites)
    /// var result3 = json.PropClone(
    ///     sourcePath: "source.data.id",
    ///     destinationPath: "source.data.originalId"
    /// );
    /// </code>
    /// </example>
    public static JObject PropClone(this JObject source, string sourcePath, string destinationPath)
    {
        if (string.IsNullOrEmpty(value: sourcePath))
            throw new ArgumentNullException(
                paramName: nameof(sourcePath),
                message: "Source path cannot be null or empty"
            );

        if (string.IsNullOrEmpty(value: destinationPath))
            throw new ArgumentNullException(
                paramName: nameof(destinationPath),
                message: "Destination path cannot be null or empty"
            );

        var result = source?.DeepClone() as JObject ?? [];

        // Get the value from source path
        var sourceValue = result.PropGet<JToken>(path: sourcePath);
        if (sourceValue == null)
            return result;

        // Clone the value to destination path
        result = result.PropUpsert(path: destinationPath, value: sourceValue.DeepClone());

        return result;
    }
}
