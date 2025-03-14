using IDC.Utilities.Extensions;

namespace IDC.Utilities.Models.API;

/// <summary>
/// Represents a standardized API response format.
/// </summary>
/// <remarks>
/// Provides a consistent structure for API responses with status and message handling capabilities.
/// Supports method chaining for fluent configuration.
/// </remarks>
/// <example>
/// <code>
/// var response = new APIResponse()
///     .ChangeStatus("Failed")
///     .ChangeMessage("Operation could not be completed");
/// </code>
/// </example>
public class APIResponse
{
    /// <summary>Gets or sets the status.</summary>
    /// <value>The status.</value>
    /// <remarks>The default value is "Success".</remarks>
    public string? Status { get; internal set; } = "Success";

    /// <summary>Gets or sets the message.</summary>
    /// <value>The message.</value>
    /// <remarks>The default value is "API processing is done.".</remarks>
    public string? Message { get; internal set; } = "API processing has been completed.";

    /// <summary>
    /// Changes the status of the API response using a string value.
    /// </summary>
    /// <param name="status">The new status to set.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    /// <remarks>
    /// This method allows changing the status using a string value.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeStatus(status: "Failed");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeStatus(string? status)
    {
        Status = status ?? throw new ArgumentNullException(nameof(status));
        return this;
    }

    /// <summary>
    /// Changes the status of the API response using a Language instance.
    /// </summary>
    /// <param name="language">The Language instance to get the status from.</param>
    /// <param name="key">The key to lookup the status message.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when language or key is null.</exception>
    /// <remarks>
    /// This method allows changing the status using a Language instance and key lookup.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeStatus(language: languageInstance, key: "api.status.status_failed");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeStatus(Language language, string key)
    {
        Status = language.GetMessage(path: key);
        return this;
    }

    /// <summary>
    /// Changes the message of the API response using a string value.
    /// </summary>
    /// <param name="message">The new message to set.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    /// <remarks>
    /// This method allows changing the message using a string value.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeMessage("Processing completed successfully");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(string? message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        return this;
    }

    /// <summary>
    /// Changes the message of the API response using a Language instance.
    /// </summary>
    /// <param name="language">The Language instance to get the message from.</param>
    /// <param name="key">The key to lookup the message.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <remarks>
    /// This method allows changing the message using a Language instance and key lookup.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeMessage(language: languageInstance, key: "api.messages.success");
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(Language language, string key)
    {
        Message = language.GetMessage(path: key);
        return this;
    }

    /// <summary>
    /// Changes the message of the API response using an exception.
    /// </summary>
    /// <param name="exception">The exception to extract the message from.</param>
    /// <param name="includeStackTrace">Whether to include the stack trace in the message.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <remarks>
    /// This method sets the message to the exception details and outputs it to the console.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeMessage(exception: ex, includeStackTrace: true);
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(Exception exception, bool includeStackTrace = false)
    {
        Message = exception.GetExceptionDetails(includeStackTrace: includeStackTrace);
        Console.WriteLine(Message);

        return this;
    }

    /// <summary>
    /// Changes the message of the API response using an exception and logs it.
    /// </summary>
    /// <param name="exception">The exception to extract the message from.</param>
    /// <param name="logging">The logging instance to use.</param>
    /// <param name="includeStackTrace">Whether to include the stack trace in the message.</param>
    /// <returns>The current APIResponse instance for method chaining.</returns>
    /// <remarks>
    /// This method sets the message to the exception details and logs it using the provided logging instance.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponse();
    /// response.ChangeMessage(exception: ex, logging: logger, includeStackTrace: true);
    /// </code>
    /// </example>
    public virtual APIResponse ChangeMessage(
        Exception exception,
        SystemLogging logging,
        bool includeStackTrace = false
    )
    {
        Message = exception.GetExceptionDetails(includeStackTrace: includeStackTrace);
        logging.LogError(Message);

        return this;
    }
}
