namespace IDC.Utilities.Models.API;

/// <summary>
/// Represents a standardized API response format with generic data payload.
/// </summary>
/// <typeparam name="T">The type of data payload.</typeparam>
/// <remarks>
/// Provides a consistent structure for API responses with status, message, and data handling capabilities.
///
/// Key features:
/// - Generic data payload support
/// - Fluent interface pattern
/// - Multi-language support
/// - Exception handling integration
/// - Logging capabilities
///
/// > [!IMPORTANT]
/// > All methods in this class maintain immutability by returning a new instance.
///
/// > [!NOTE]
/// > This class inherits from <see cref="APIResponse"/> and extends it with generic data handling capabilities.
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// var response = new APIResponseData&lt;UserModel&gt;()
///     .ChangeStatus(status: "Success")
///     .ChangeMessage(message: "User retrieved successfully")
///     .ChangeData(data: userModel);
/// </code>
///
/// With exception handling:
/// <code>
/// try
/// {
///     // Some operation that might throw
///     return new APIResponseData&lt;UserModel&gt;()
///         .ChangeStatus(status: "Success")
///         .ChangeData(data: result);
/// }
/// catch (Exception ex)
/// {
///     return new APIResponseData&lt;UserModel&gt;()
///         .ChangeStatus(status: "Error")
///         .ChangeMessage(exception: ex, logging: _logger);
/// }
/// </code>
/// </example>
public class APIResponseData<T> : APIResponse
{
    /// <summary>
    /// Gets or sets the data payload of the response.
    /// </summary>
    /// <value>
    /// The strongly-typed data of type <typeparamref name="T"/>, or null if no data is present.
    /// </value>
    /// <remarks>
    /// > [!NOTE]
    /// > The setter is internal to ensure immutability from outside the assembly.
    ///
    /// > [!TIP]
    /// > Use <see cref="ChangeData"/> method for fluent modifications.
    /// </remarks>
    public T? Data { get; internal set; }

    /// <summary>
    /// Changes the data payload of the API response.
    /// </summary>
    /// <param name="data">The new data to set.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// This method follows the fluent interface pattern, allowing method chaining for building responses.
    ///
    /// > [!NOTE]
    /// > Passing null will explicitly set the Data property to null.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;()
    ///     .ChangeData(data: new UserModel
    ///     {
    ///         Id = 1,
    ///         Name = "John Doe"
    ///     });
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
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Overrides the base implementation to maintain proper type for method chaining.
    ///
    /// > [!TIP]
    /// > Common status values: "Success", "Error", "Warning", "Info"
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;()
    ///     .ChangeStatus(status: "Success");
    /// </code>
    /// </example>
    public override APIResponseData<T> ChangeStatus(string? status)
    {
        base.ChangeStatus(status: status);
        return this;
    }

    /// <summary>
    /// Changes the status using a language key.
    /// </summary>
    /// <param name="language">The language instance for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Enables localization support for status messages.
    ///
    /// > [!NOTE]
    /// > The language key must exist in the language configuration.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;()
    ///     .ChangeStatus(language: langInstance, key: "api.status.success");
    /// </code>
    /// </example>
    public override APIResponseData<T> ChangeStatus(Language language, string key)
    {
        base.ChangeStatus(language: language, key: key);
        return this;
    }

    /// <summary>
    /// Changes the message of the API response.
    /// </summary>
    /// <param name="message">The new message to set.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// > [!TIP]
    /// > Use this method for direct message setting without translation.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;()
    ///     .ChangeMessage(message: "Operation completed successfully");
    /// </code>
    /// </example>
    public override APIResponseData<T> ChangeMessage(string? message)
    {
        base.ChangeMessage(message: message);
        return this;
    }

    /// <summary>
    /// Changes the message using a language key.
    /// </summary>
    /// <param name="language">The language instance for translation.</param>
    /// <param name="key">The language key to lookup.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Enables localization support for response messages.
    ///
    /// > [!NOTE]
    /// > Falls back to the key if translation is not found.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = new APIResponseData&lt;UserModel&gt;()
    ///     .ChangeMessage(language: langInstance, key: "api.message.user.created");
    /// </code>
    /// </example>
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
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Useful for error handling scenarios where exception details need to be included in the response.
    ///
    /// > [!WARNING]
    /// > Including stack trace in production environments may expose sensitive information.
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     throw new InvalidOperationException("Invalid state");
    /// }
    /// catch (Exception ex)
    /// {
    ///     var response = new APIResponseData&lt;UserModel&gt;()
    ///         .ChangeMessage(exception: ex, includeStackTrace: false);
    /// }
    /// </code>
    /// </example>
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
    /// <returns>The current instance for method chaining.</returns>
    /// <remarks>
    /// Combines exception handling with logging capabilities.
    ///
    /// > [!IMPORTANT]
    /// > This method will log the exception before setting the message.
    ///
    /// > [!TIP]
    /// > Use this method when you need both error reporting and logging.
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     throw new DatabaseException("Connection failed");
    /// }
    /// catch (Exception ex)
    /// {
    ///     var response = new APIResponseData&lt;UserModel&gt;()
    ///         .ChangeMessage(
    ///             exception: ex,
    ///             logging: _logger,
    ///             includeStackTrace: false
    ///         );
    /// }
    /// </code>
    /// </example>
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
