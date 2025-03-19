namespace IDC.Utilities.Models;

/// <summary>
/// Defines logging severity levels for application-wide logging.
/// </summary>
/// <remarks>
/// Provides standardized logging levels to categorize log messages based on their severity and importance.
/// Used throughout the application to maintain consistent logging practices.
///
/// Example usage:
/// ```csharp
/// public void LogMessage(string message, LogLevel level)
/// {
///     switch (level)
///     {
///         case LogLevel.Information:
///             Console.WriteLine($"[INFO] {message}");
///             break;
///         case LogLevel.Warning:
///             Console.WriteLine($"[WARN] {message}");
///             break;
///         case LogLevel.Error:
///             Console.WriteLine($"[ERROR] {message}");
///             break;
///     }
/// }
/// ```
///
/// > [!IMPORTANT]
/// > Choose appropriate log levels based on the message significance:
/// > - Use Information for routine operations
/// > - Use Warning for potential issues
/// > - Use Error for actual failures
///
/// > [!TIP]
/// > Consider implementing log filtering based on these levels in production environments
/// > to control log verbosity and storage requirements.
/// </remarks>
public enum LogLevel
{
    /// <summary>
    /// Represents informational messages that track the normal flow of the application.
    /// </summary>
    /// <remarks>
    /// Use for general operational messages that highlight the progress of the application.
    /// These messages should have long-term value for debugging and monitoring.
    ///
    /// Example scenarios:
    /// - Application startup/shutdown
    /// - User authentication success
    /// - Successful API calls
    /// - Cache hits/misses
    /// - Configuration loading
    ///
    /// > [!NOTE]
    /// > Information logs should not include sensitive data.
    /// </remarks>
    Information,

    /// <summary>
    /// Represents potentially harmful situations or unexpected states that might lead to errors.
    /// </summary>
    /// <remarks>
    /// Use for situations that are unexpected but not necessarily errors.
    /// The application continues to function but requires attention.
    ///
    /// Example scenarios:
    /// - Deprecated API usage
    /// - Resource usage nearing limits
    /// - Missing optional configuration
    /// - Slow database queries
    /// - Retry attempts
    ///
    /// > [!WARNING]
    /// > Warning logs should include enough context to diagnose potential issues.
    /// </remarks>
    Warning,

    /// <summary>
    /// Represents error conditions and exceptions that affect functionality.
    /// </summary>
    /// <remarks>
    /// Use for error conditions that require immediate attention.
    /// These indicate failures in the application that need investigation.
    ///
    /// Example scenarios:
    /// - Unhandled exceptions
    /// - API call failures
    /// - Database connection errors
    /// - Authentication failures
    /// - Data validation errors
    ///
    /// > [!CAUTION]
    /// > Error logs should include:
    /// > - Stack traces
    /// > - Error codes
    /// > - Correlation IDs
    /// > - Relevant context data
    ///
    /// > [!IMPORTANT]
    /// > Ensure error logs don't expose sensitive information in production.
    /// </remarks>
    Error
}
