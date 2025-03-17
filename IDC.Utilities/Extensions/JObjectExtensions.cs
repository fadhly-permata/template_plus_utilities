using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for JObject to enhance its functionality.
/// </summary>
/// <remarks>
/// This class contains various utility methods for working with JObjects,
/// including property retrieval, merging, updating, and cloning operations.
/// </remarks>
public static class JObjectExtensions
{
    /// <summary>
    /// Gets a value from a JObject using a dot notation path.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="source">The source JObject to get the value from.</param>
    /// <param name="path">The dot notation path to the value (e.g., "user.address.city").</param>
    /// <param name="defaultValue">The default value to return if the path doesn't exist or conversion fails.</param>
    /// <returns>The value at the specified path converted to type T, or defaultValue if not found or conversion fails.</returns>
    /// <remarks>
    /// This method safely handles null source objects and empty paths by returning the default value.
    /// Uses SelectToken for deep path traversal and handles conversion exceptions gracefully.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject
    /// {
    ///     ["user"] = new JObject
    ///     {
    ///         ["age"] = 30
    ///     }
    /// };
    ///
    /// int? age = json.PropGet&lt;int&gt;(path: "user.age", defaultValue: null);
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
    /// Gets a value from a JObject using a dot notation path.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="source">The source JObject to get the value from.</param>
    /// <param name="path">The dot notation path to the value (e.g., "user.address.city").</param>
    /// <param name="onNullMessage">The message to throw if the path doesn't exist.</param>
    /// <param name="throwOnNull">Whether to throw a KeyNotFoundException if the path doesn't exist.</param>
    /// <returns>The value at the specified path converted to type T, or default if not found or conversion fails.</returns>
    /// <remarks>
    /// This method safely handles null source objects and empty paths by returning the default value.
    /// Uses SelectToken for deep path traversal and handles conversion exceptions gracefully.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject
    /// {
    ///     ["user"] = new JObject
    ///     {
    ///         ["age"] = 30
    ///     }
    /// };
    ///
    /// int? age = json.PropGet&lt;int&gt;(path: "user.age");
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
    /// Merges two JObjects recursively, with optional array merging support.
    /// </summary>
    /// <param name="source">The source JObject to merge into.</param>
    /// <param name="other">The other JObject to merge from.</param>
    /// <param name="mergeArrays">When true, arrays are merged using union operation. When false, arrays are replaced.</param>
    /// <returns>A new JObject containing the merged result.</returns>
    /// <remarks>
    /// - Properties from 'other' override properties from 'source' if they exist
    /// - Nested objects are merged recursively
    /// - Arrays can be either merged (union) or replaced based on mergeArrays parameter
    /// - Null values are handled safely
    /// </remarks>
    /// <example>
    /// <code>
    /// var source = JObject.Parse(@"{ ""name"": ""John"", ""scores"": [1, 2] }");
    /// var other = JObject.Parse(@"{ ""age"": 30, ""scores"": [3, 4] }");
    ///
    /// var result = source.PropMerge(
    ///     other: other,
    ///     mergeArrays: true
    /// );
    /// // Result: { "name": "John", "age": 30, "scores": [1, 2, 3, 4] }
    /// </code>
    /// </example>
    public static JObject PropMerge(this JObject source, JObject other, bool mergeArrays = false)
    {
        var result = new JObject();
        source
            ?.Properties()
            .ToList()
            .ForEach(action: prop =>
                result.Add(propertyName: prop.Name, value: prop.Value?.DeepClone())
            );

        if (other == null)
            return result;

        foreach (var kvp in other)
        {
            if (!result.ContainsKey(propertyName: kvp.Key))
            {
                result.Add(propertyName: kvp.Key, value: kvp.Value?.DeepClone());
                continue;
            }

            if (result[kvp.Key] is JObject sourceObj && kvp.Value is JObject otherObj)
                result[kvp.Key] = sourceObj.PropMerge(other: otherObj, mergeArrays: mergeArrays);
            else if (
                mergeArrays
                && result[kvp.Key] is JArray sourceArray
                && kvp.Value is JArray otherArray
            )
                result[kvp.Key] = new JArray(content: sourceArray.Union(second: otherArray));
            else
                result[kvp.Key] = kvp.Value?.DeepClone();
        }

        return result;
    }

    /// <summary>
    /// Removes properties from a JObject based on a predicate function.
    /// </summary>
    /// <param name="source">The source JObject to remove properties from.</param>
    /// <param name="predicate">A function that determines whether a property should be removed.</param>
    /// <param name="recursive">Whether to recursively remove properties from nested objects and arrays.</param>
    /// <returns>A new JObject with properties removed according to the predicate.</returns>
    /// <remarks>
    /// This method creates a new JObject and does not modify the original source object.
    /// If the source is null, an empty JObject is returned.
    /// The predicate function should return true for properties that should be removed.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = JObject.Parse(@"{ ""name"": ""John"", ""age"": 30, ""address"": { ""city"": ""New York"" } }");
    /// var result = json.PropRemove(
    ///     predicate: kvp => kvp.Key == "age",
    ///     recursive: true
    /// );
    /// // Result: { "name": "John", "address": { "city": "New York" } }
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
    /// <param name="updates">Dictionary containing path-value pairs where path is dot-notation string and value is the new value.</param>
    /// <returns>A new JObject with the updated properties.</returns>
    /// <remarks>
    /// - Creates a deep clone of the source object before applying updates
    /// - Handles nested paths using dot notation (e.g., "user.address.city")
    /// - Creates intermediate objects if they don't exist
    /// - Supports null values
    /// - Returns empty JObject if source is null
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject
    /// {
    ///     ["user"] = new JObject { ["name"] = "John" }
    /// };
    ///
    /// var updates = new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["user.age"] = 30,
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
    /// <param name="path">The dot notation path to the value (e.g., "user.address.city").</param>
    /// <param name="value">The value to upsert.</param>
    /// <returns>A new JObject with the upserted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when path is null or empty.</exception>
    /// <remarks>
    /// This method creates a deep clone of the source object before applying the upsert.
    /// It creates intermediate objects if they don't exist in the path.
    /// If the source is null, a new JObject is created.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject { ["user"] = new JObject { ["name"] = "John" } };
    /// var result = json.PropUpsert(path: "user.age", value: 30);
    /// // Result: { "user": { "name": "John", "age": 30 } }
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
    /// <param name="updates">Dictionary containing path-value pairs where path is dot-notation string and value is the new value.</param>
    /// <returns>A new JObject with the upserted properties.</returns>
    /// <exception cref="ArgumentNullException">Thrown when updates dictionary is null.</exception>
    /// <remarks>
    /// This method creates a deep clone of the source object before applying the upserts.
    /// It uses the single-property PropUpsert method for each update, ensuring consistent behavior.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject { ["user"] = new JObject { ["name"] = "John" } };
    /// var updates = new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["user.age"] = 30,
    ///     ["user.address.city"] = "New York"
    /// };
    /// var result = json.PropUpsert(updates: updates);
    /// // Result: { "user": { "name": "John", "age": 30, "address": { "city": "New York" } } }
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
    /// Clones a property from a source path to a new destination path within the JObject.
    /// </summary>
    /// <param name="source">The source JObject.</param>
    /// <param name="sourcePath">The dot notation path of the source property to clone.</param>
    /// <param name="destinationPath">The dot notation path where the cloned property will be placed.</param>
    /// <returns>A new JObject with the cloned property at the specified destination path.</returns>
    /// <exception cref="ArgumentNullException">Thrown when sourcePath or destinationPath is null or empty.</exception>
    /// <remarks>
    /// If the source path doesn't exist, the original JObject is returned unchanged.
    /// If the destination path already exists, it will be overwritten with the cloned value.
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = new JObject
    /// {
    ///     ["user"] = new JObject
    ///     {
    ///         ["name"] = "John",
    ///         ["age"] = 30
    ///     }
    /// };
    ///
    /// // Clone "user.name" to "profile.userName"
    /// var result = json.PropClone(
    ///     sourcePath: "user.name",
    ///     destinationPath: "profile.userName"
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
