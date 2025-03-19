using IDC.Utilities.Extensions;

namespace IDC.Utilities.Models.API;

/// <summary>
/// Represents a standardized base API response format with status and message handling capabilities.
/// </summary>
/// <remarks>
/// Provides a consistent structure for API responses with built-in support for:
/// - Status management
/// - Message handling
/// - Exception integration
/// - Localization support
/// - Logging capabilities
///
/// > [!IMPORTANT]
/// > All methods follow the fluent interface pattern for method chaining.
///
/// > [!NOTE]
/// > This class serves as the base for all API responses and can be extended for specific use cases.
///
/// > [!TIP]
/// > For responses with data payload, use <see cref="APIResponseData{T}"/> instead.
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// var response = new APIResponse()
///     .ChangeStatus(status: "Success")
///     .ChangeMessage(message: "Operation completed successfully");
/// </code>
///
/// With error handling:
/// <code>
/// try
/// {
///     // Some operation
///     return new APIResponse()
///         .ChangeStatus(status: "Success");
/// }
/// catch (Exception ex)
/// {
///     return new APIResponse()
///         .ChangeStatus(status: "Error")
///         .ChangeMessage(exception: ex, logging: _logger);
/// }
/// </code>
/// </example>
public class APIResponse
{
    /// <summary>
    /// Gets or sets the status of the API response.
    /// </summary>
    /// <value>
    /// A string representing the current status. Defaults to "Success".
    /// </value>
    /// <remarks>
    /// > [!NOTE]
    /// > The setter is internal to maintain encapsulation.
    ///
    /// > [!TIP]
    /// > Common status values include: "Success", "Error", "Warning", "Info"
    /// </remarks>
    public string? Status { get; internal set; } = "Success";

    /// <summary>
    /// Gets or sets the message associated with the API response.
    /// </summary>
    /// <value>
    /// A string containing the response message. Defaults to "API processing has been completed."
    /// </value>
    /// <remarks>
    /// > [!NOTE]
    /// > The setter is internal to maintain encapsulation.
    ///
    /// > [!TIP]
    /// > Use <see cref="ChangeMessage"/> methods for fluent modifications.
    /// </remarks>
    public string? Message { get; internal set; } = "API processing has been completed.";

    /// <summary>
    /// Changes the status of the API response.
    /// </summary>
    /// <param name="status">The new status to set.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    /// <remarks>
    /// > [!IMPORTANT]
    /// > The status cannot be null.
    ///
    /// > [!TIP]
    /// > Use consistent status values across your application.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse()
    ///     .ChangeStatus(status: "Processing")
    ///     .ChangeStatus(status: "Completed");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeStatus(string? status)
    {
        Status = status ?? throw new ArgumentNullException(paramName: nameof(status));
        return this;
    }

    /// <summary>
    /// Changes the status using a localized message.
    /// </summary>
    /// <param name="language">The language instance for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Enables internationalization support for status messages.
    ///
    /// > [!NOTE]
    /// > The language key must exist in the language configuration.
    ///
    /// > [!TIP]
    /// > Use consistent key patterns like "api.status.success".
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse()
    ///     .ChangeStatus(
    ///         language: languageInstance,
    ///         key: "api.status.processing"
    ///     );
    /// </code>
    /// </example>
    public virtual APIResponse ChangeStatus(Language language, string key)
    {
        Status = language.GetMessage(path: key);
        return this;
    }

    /// <summary>
    /// Changes the message of the API response.
    /// </summary>
    /// <param name="message">The new message to set.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    /// <remarks>
    /// > [!IMPORTANT]
    /// > The message cannot be null.
    ///
    /// > [!TIP]
    /// > Use clear and concise messages that describe the operation result.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse()
    ///     .ChangeMessage(message: "User authentication successful")
    ///     .ChangeMessage(message: "Session initialized");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(string? message)
    {
        Message = message ?? throw new ArgumentNullException(paramName: nameof(message));
        return this;
    }

    /// <summary>
    /// Changes the message using a localized message.
    /// </summary>
    /// <param name="language">The language instance for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Enables internationalization support for response messages.
    ///
    /// > [!NOTE]
    /// > Falls back to the key if translation is not found.
    ///
    /// > [!TIP]
    /// > Use hierarchical keys like "api.messages.auth.success".
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse()
    ///     .ChangeMessage(
    ///         language: languageInstance,
    ///         key: "api.messages.user.created"
    ///     );
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(Language language, string key)
    {
        Message = language.GetMessage(path: key);
        return this;
    }

    /// <summary>
    /// Changes the message using exception details.
    /// </summary>
    /// <param name="exception">The exception to extract message from.</param>
    /// <param name="includeStackTrace">Whether to include stack trace details.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Automatically formats exception details into a readable message.
    ///
    /// > [!WARNING]
    /// > Including stack trace in production may expose sensitive information.
    ///
    /// > [!NOTE]
    /// > The message is also output to the console for debugging.
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     throw new InvalidOperationException("Database connection failed");
    /// }
    /// catch (Exception ex)
    /// {
    ///     var response = new APIResponse()
    ///         .ChangeStatus(status: "Error")
    ///         .ChangeMessage(
    ///             exception: ex,
    ///             includeStackTrace: false
    ///         );
    /// }
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(Exception exception, bool includeStackTrace = false)
    {
        Message = exception.GetExceptionDetails(includeStackTrace: includeStackTrace);
        Console.WriteLine(value: Message);
        return this;
    }

    /// <summary>
    /// Changes the message using exception details and logs it.
    /// </summary>
    /// <param name="exception">The exception to extract message from.</param>
    /// <param name="logging">The logging instance to use.</param>
    /// <param name="includeStackTrace">Whether to include stack trace details.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Combines exception handling with logging capabilities.
    ///
    /// > [!IMPORTANT]
    /// > This method will log the exception before setting the message.
    ///
    /// > [!TIP]
    /// > Use this method when you need both error reporting and logging.
    ///
    /// > [!CAUTION]
    /// > Ensure proper log level configuration in production.
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     throw new SecurityException("Invalid authentication token");
    /// }
    /// catch (Exception ex)
    /// {
    ///     var response = new APIResponse()
    ///         .ChangeStatus(status: "Error")
    ///         .ChangeMessage(
    ///             exception: ex,
    ///             logging: _logger,
    ///             includeStackTrace: false
    ///         );
    /// }
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(
        Exception exception,
        SystemLogging logging,
        bool includeStackTrace = false
    )
    {
        Message = exception.GetExceptionDetails(includeStackTrace: includeStackTrace);
        logging.LogError(message: Message);
        return this;
    }
}
