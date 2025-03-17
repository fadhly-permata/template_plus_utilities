namespace IDC.Template.Utilities.Models;

/// <summary>
/// Represents a request for generating a user-specific API key.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="AppId">The application identifier associated with the API key.</param>
/// <param name="ExpiryDate">The date and time when the API key will expire.</param>
public record UserApiKeyRequest(string UserId, string AppId, DateTime ExpiryDate);

/// <summary>
/// Represents a request for generating a temporary API key.
/// </summary>
/// <param name="Validity">The duration for which the temporary API key will be valid.</param>
/// <param name="Purpose">The intended purpose or use case for the temporary API key.</param>
public record TemporaryApiKeyRequest(TimeSpan Validity, string Purpose);

/// <summary>
/// Represents a request for generating a client-specific API key.
/// </summary>
/// <param name="ClientId">The unique identifier of the client.</param>
/// <param name="ClientSecret">The secret key associated with the client.</param>
/// <param name="Permissions">Array of permission strings defining the access scope.</param>
public record ClientApiKeyRequest(string ClientId, string ClientSecret, string[] Permissions);

/// <summary>
/// Represents a request for generating an environment-specific API key.
/// </summary>
/// <param name="Environment">The target environment (e.g., development, staging, production).</param>
/// <param name="ServiceName">The name of the service requiring the API key.</param>
/// <param name="Version">The version identifier of the service.</param>
public record EnvironmentApiKeyRequest(string Environment, string ServiceName, string Version);
