using System.Text;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for enhanced Exception handling and manipulation.
/// </summary>
/// <remarks>
/// This class offers utility methods to:
/// - Extract comprehensive exception messages
/// - Generate detailed exception reports
/// - Manage exception metadata through a key-value store
///
/// Example usage for common scenarios:
/// ```csharp
/// try
/// {
///     throw new Exception("Primary error")
///         .AddData("UserId", 123)
///         .AddData("Operation", "UserSync");
/// }
/// catch (Exception ex)
/// {
///     // Get full message chain
///     string messages = ex.GetFullMessage();
///
///     // Get detailed report
///     string report = ex.GetExceptionDetails();
///
///     // Retrieve metadata
///     int userId = ex.GetData<int>("UserId");
/// }
/// ```
/// </remarks>
public static class ExceptionExtensions
{
    /// <summary>
    /// Retrieves a complete exception message chain including all inner exceptions.
    /// </summary>
    /// <param name="ex">The source exception.</param>
    /// <param name="separator">The delimiter between exception messages.</param>
    /// <returns>A concatenated string of all exception messages.</returns>
    /// <remarks>
    /// Traverses the exception chain from outermost to innermost, joining messages with the specified separator.
    ///
    /// > [!NOTE]
    /// > The separator parameter defaults to " -> " if not specified.
    ///
    /// > [!TIP]
    /// > Use a distinctive separator to easily identify message boundaries in logs.
    ///
    /// Example output:
    /// ```
    /// "Failed to process request -> Database connection error -> Network timeout"
    /// ```
    /// </remarks>
    /// <example>
    /// ```csharp
    /// try
    /// {
    ///     throw new InvalidOperationException("Operation failed",
    ///         new ArgumentException("Invalid parameter"));
    /// }
    /// catch (Exception ex)
    /// {
    ///     string fullMessage = ex.GetFullMessage(" | ");
    ///     // Output: "Operation failed | Invalid parameter"
    /// }
    /// ```
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when ex is null.</exception>
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
    /// Generates a comprehensive exception report including type, message, stack trace, and metadata.
    /// </summary>
    /// <param name="ex">The source exception.</param>
    /// <param name="includeStackTrace">Controls stack trace inclusion in the output.</param>
    /// <returns>A formatted string containing detailed exception information.</returns>
    /// <remarks>
    /// The report includes:
    /// - Exception type (fully qualified name)
    /// - Exception message
    /// - Stack trace (optional)
    /// - Additional data dictionary entries
    /// - Inner exception details (recursive)
    ///
    /// > [!IMPORTANT]
    /// > Stack traces may contain sensitive information. Use includeStackTrace parameter judiciously in production.
    ///
    /// > [!TIP]
    /// > The stack trace lines are prefixed with "--> " for better readability.
    ///
    /// Example output:
    /// ```
    /// Type: System.InvalidOperationException
    /// Message: Operation failed
    /// StackTrace:
    ///   --> at MyNamespace.MyClass.MyMethod() in MyFile.cs:line 123
    /// Additional Data:
    ///   UserId: 456
    ///   Operation: DataSync
    /// Inner Exception:
    ///   Type: System.ArgumentException
    ///   Message: Invalid parameter
    /// ```
    /// </remarks>
    /// <example>
    /// ```csharp
    /// try
    /// {
    ///     throw new InvalidOperationException("Process failed")
    ///         .AddData("RequestId", Guid.NewGuid());
    /// }
    /// catch (Exception ex)
    /// {
    ///     string details = ex.GetExceptionDetails(includeStackTrace: true);
    ///     Logger.Error(details);
    /// }
    /// ```
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when ex is null.</exception>
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
    /// Adds a single key-value pair to the exception's metadata dictionary.
    /// </summary>
    /// <param name="ex">The source exception.</param>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>The original exception for method chaining.</returns>
    /// <remarks>
    /// Provides a fluent interface for adding contextual information to exceptions.
    ///
    /// > [!NOTE]
    /// > If the key already exists, its value will be overwritten.
    ///
    /// > [!WARNING]
    /// > The value should be serializable if the exception will be logged or transmitted.
    /// </remarks>
    /// <example>
    /// ```csharp
    /// throw new Exception("Authentication failed")
    ///     .AddData("Username", "john.doe")
    ///     .AddData("LoginAttempt", 3)
    ///     .AddData("Timestamp", DateTime.UtcNow);
    /// ```
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when ex or key is null.</exception>
    public static Exception AddData(this Exception ex, string key, object? value)
    {
        ex.Data[key] = value;
        return ex;
    }

    /// <summary>
    /// Adds multiple key-value pairs to the exception's metadata dictionary.
    /// </summary>
    /// <param name="ex">The source exception.</param>
    /// <param name="data">A dictionary of metadata entries.</param>
    /// <returns>The original exception for method chaining.</returns>
    /// <remarks>
    /// Bulk operation version of AddData for multiple entries.
    ///
    /// > [!NOTE]
    /// > Existing keys will be overwritten with new values.
    ///
    /// > [!TIP]
    /// > Use object initializer syntax for cleaner code when adding multiple entries.
    /// </remarks>
    /// <example>
    /// ```csharp
    /// var metadata = new Dictionary<string, object?>
    /// {
    ///     ["TransactionId"] = Guid.NewGuid(),
    ///     ["Amount"] = 1250.50m,
    ///     ["Currency"] = "USD",
    ///     ["Status"] = "Failed"
    /// };
    ///
    /// throw new Exception("Transaction failed")
    ///     .AddData(metadata);
    /// ```
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when ex or data is null.</exception>
    public static Exception AddData(this Exception ex, Dictionary<string, object?> data)
    {
        foreach (var kvp in data)
            ex.Data[kvp.Key] = kvp.Value;

        return ex;
    }

    /// <summary>
    /// Retrieves a strongly-typed value from the exception's metadata dictionary.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="ex">The source exception.</param>
    /// <param name="key">The metadata key to retrieve.</param>
    /// <param name="defaultValue">The fallback value if key not found or type mismatch.</param>
    /// <returns>The value cast to type T, or defaultValue if not found/castable.</returns>
    /// <remarks>
    /// Provides type-safe access to exception metadata with fallback support.
    ///
    /// > [!NOTE]
    /// > Returns defaultValue in three cases:
    /// > - Key doesn't exist
    /// > - Value is null
    /// > - Value cannot be cast to type T
    ///
    /// > [!TIP]
    /// > Use nullable types when the value might be null.
    /// </remarks>
    /// <example>
    /// ```csharp
    /// try
    /// {
    ///     throw new Exception("Payment failed")
    ///         .AddData("Amount", 99.99m)
    ///         .AddData("IsRetry", true);
    /// }
    /// catch (Exception ex)
    /// {
    ///     decimal amount = ex.GetData<decimal>("Amount");
    ///     bool isRetry = ex.GetData<bool>("IsRetry");
    ///     string? reference = ex.GetData<string>("Reference", "N/A");
    /// }
    /// ```
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when ex or key is null.</exception>
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
