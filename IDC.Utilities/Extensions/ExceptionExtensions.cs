using System.Text;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for Exception handling and manipulation.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Gets the complete exception message including all inner exceptions.
    /// </summary>
    /// <param name="ex">The exception to get messages from.</param>
    /// <param name="separator">The separator between exception messages. Defaults to " -> ".</param>
    /// <returns>A string containing all exception messages concatenated.</returns>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Some code that throws
    /// }
    /// catch (Exception ex)
    /// {
    ///     string fullMessage = ex.GetFullMessage(separator: " | ");
    /// }
    /// </code>
    /// </example>
    public static string GetFullMessage(this Exception ex, string separator = " -> ")
    {
        var messages = new List<string> { ex.Message };
        var inner = ex.InnerException;

        while (inner != null)
        {
            messages.Add(item: inner.Message);
            inner = inner.InnerException;
        }

        return string.Join(separator: separator, values: messages);
    }

    /// <summary>
    /// Gets detailed exception information including type, message, stack trace, and data.
    /// </summary>
    /// <param name="ex">The exception to get details from.</param>
    /// <param name="includeStackTrace">Whether to include stack trace in the output.</param>
    /// <returns>A string containing detailed exception information.</returns>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Some code that throws
    /// }
    /// catch (Exception ex)
    /// {
    ///     string details = ex.GetExceptionDetails(includeStackTrace: true);
    /// }
    /// </code>
    /// </example>
    public static string GetExceptionDetails(this Exception ex, bool includeStackTrace = true)
    {
        var sb = new StringBuilder();

        void AddExceptionInfo(Exception exception, string prefix = "")
        {
            sb.AppendLine(handler: $"{prefix}Type: {exception.GetType().FullName}");
            sb.AppendLine(handler: $"{prefix}Message: {exception.Message}");

            if (includeStackTrace && !string.IsNullOrEmpty(value: exception.StackTrace))
            {
                sb.AppendLine(handler: $"{prefix}StackTrace:");
                sb.AppendLine(handler: $"{prefix}{exception.StackTrace.Replace("at ", "--> ")}");
            }

            if (exception.Data.Count > 0)
            {
                sb.AppendLine(handler: $"{prefix}Additional Data:");
                foreach (var key in exception.Data.Keys)
                    sb.AppendLine(handler: $"{prefix}  {key}: {exception.Data[key]}");
            }

            if (exception.InnerException != null)
            {
                sb.AppendLine(handler: $"{prefix}Inner Exception:");
                AddExceptionInfo(exception: exception.InnerException, prefix: $"{prefix}  ");
            }
        }

        AddExceptionInfo(exception: ex);
        return sb.ToString();
    }

    /// <summary>
    /// Adds additional data to the exception's Data dictionary.
    /// </summary>
    /// <param name="ex">The exception to add data to.</param>
    /// <param name="key">The key for the data entry.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The original exception with added data.</returns>
    /// <example>
    /// <code>
    /// throw new Exception("Process failed")
    ///     .AddData(key: "ProcessId", value: 123)
    ///     .AddData(key: "Timestamp", value: DateTime.UtcNow);
    /// </code>
    /// </example>
    public static Exception AddData(this Exception ex, string key, object? value)
    {
        ex.Data[key] = value;
        return ex;
    }

    /// <summary>
    /// Adds multiple data entries to the exception's Data dictionary.
    /// </summary>
    /// <param name="ex">The exception to add data to.</param>
    /// <param name="data">Dictionary containing the data to add.</param>
    /// <returns>The original exception with added data.</returns>
    /// <example>
    /// <code>
    /// var additionalData = new Dictionary&lt;string, object?&gt;
    /// {
    ///     ["UserId"] = 456,
    ///     ["Operation"] = "DataSync"
    /// };
    /// throw new Exception("Process failed").AddData(data: additionalData);
    /// </code>
    /// </example>
    public static Exception AddData(this Exception ex, Dictionary<string, object?> data)
    {
        foreach (var kvp in data)
            ex.Data[kvp.Key] = kvp.Value;

        return ex;
    }

    /// <summary>
    /// Gets a value from the exception's Data dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="ex">The exception to get data from.</param>
    /// <param name="key">The key of the data to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key doesn't exist or the value can't be converted.</param>
    /// <returns>The value if found and convertible to T, otherwise the default value.</returns>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     throw new Exception("Process failed")
    ///         .AddData(key: "RetryCount", value: 3);
    /// }
    /// catch (Exception ex)
    /// {
    ///     int retryCount = ex.GetData&lt;int&gt;(key: "RetryCount", defaultValue: 0);
    /// }
    /// </code>
    /// </example>
    public static T? GetData<T>(this Exception ex, string key, T? defaultValue = default)
    {
        if (!ex.Data.Contains(key: key))
            return defaultValue;

        try
        {
            return ex.Data[key] is T typedValue ? typedValue : defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}
