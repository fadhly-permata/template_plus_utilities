namespace IDC.Utilities.Models.API;

/// <summary>
/// Represents a standardized API response format with generic data payload.
/// </summary>
/// <typeparam name="T">The type of data payload.</typeparam>
/// <remarks>
/// Provides a consistent structure for API responses with status, message, and data handling capabilities.
/// Supports method chaining for fluent configuration.
/// </remarks>
/// <example>
/// <code>
/// var response = new APIResponseData&lt;UserModel&gt;()
///     .ChangeStatus(status: "Success")
///     .ChangeMessage(message: "User retrieved successfully")
///     .ChangeData(data: userModel);
/// </code>
/// </example>
public class APIResponseData<T> : APIResponse
{
    /// <summary>Gets or sets the data payload.</summary>
    /// <value>The data of type T.</value>
    /// <remarks>The default value is default(T).</remarks>
    public T? Data { get; internal set; }

    /// <summary>
    /// Changes the data payload of the API response.
    /// </summary>
    /// <param name="data">The new data to set.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    /// <remarks>
    /// This method allows changing the data payload while maintaining fluent interface pattern.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;();
    /// response.ChangeData(data: userModel);
    /// </code>
    /// </example>
    public virtual APIResponseData<T> ChangeData(T? data)
    {
        Data = data;
        return this;
    }

    /// <summary>
    /// Changes the status of the API response.
    /// </summary>
    /// <param name="status">The new status to set.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeStatus(string? status)
    {
        base.ChangeStatus(status: status);
        return this;
    }

    /// <summary>
    /// Changes the status using a language key.
    /// </summary>
    /// <param name="language">The language instance to use for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeStatus(Language language, string key)
    {
        base.ChangeStatus(language: language, key: key);
        return this;
    }

    /// <summary>
    /// Changes the message of the API response.
    /// </summary>
    /// <param name="message">The new message to set.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeMessage(string? message)
    {
        base.ChangeMessage(message: message);
        return this;
    }

    /// <summary>
    /// Changes the message using a language key.
    /// </summary>
    /// <param name="language">The language instance to use for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeMessage(Language language, string key)
    {
        base.ChangeMessage(language: language, key: key);
        return this;
    }

    /// <summary>
    /// Changes the message using an exception.
    /// </summary>
    /// <param name="exception">The exception to extract message from.</param>
    /// <param name="includeStackTrace">Whether to include stack trace in message.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeMessage(
        Exception exception,
        bool includeStackTrace = false
    )
    {
        base.ChangeMessage(exception: exception, includeStackTrace: includeStackTrace);
        return this;
    }

    /// <summary>
    /// Changes the message using an exception and logs it.
    /// </summary>
    /// <param name="exception">The exception to extract message from.</param>
    /// <param name="logging">The logging instance to use.</param>
    /// <param name="includeStackTrace">Whether to include stack trace in message.</param>
    /// <returns>The current APIResponseData instance for method chaining.</returns>
    public override APIResponseData<T> ChangeMessage(
        Exception exception,
        SystemLogging logging,
        bool includeStackTrace = false
    )
    {
        base.ChangeMessage(
            exception: exception,
            logging: logging,
            includeStackTrace: includeStackTrace
        );
        return this;
    }
}
