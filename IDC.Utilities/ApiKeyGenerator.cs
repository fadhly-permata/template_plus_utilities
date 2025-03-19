using System.Security.Cryptography;
using System.Text;

namespace IDC.Utilities;

/// <summary>
/// Provides functionality for generating and validating API keys.
/// </summary>
/// <remarks>
/// Implements various API key generation strategies including user-based, temporary, client-based, and environment-based keys.
/// All generated keys are hashed using SHA256 and returned as Base64 strings.
///
/// Example usage:
/// ```csharp
/// // Generate user-based key
/// var userKey = ApiKeyGenerator.Generate(userId: "user123", appId: "app456", expiryDate: DateTime.Now.AddDays(30));
///
/// // Generate temporary key
/// var tempKey = ApiKeyGenerator.GenerateTemporary(validity: TimeSpan.FromHours(1), purpose: "password-reset");
///
/// // Validate key
/// bool isValid = ApiKeyGenerator.ValidateApiKey(apiKey: "someKey", registeredKeys: new[] { "validKey1", "validKey2" });
/// ```
/// </remarks>
public sealed class ApiKeyGenerator
{
    /// <summary>
    /// Generates a user-specific API key.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="appId">The application identifier.</param>
    /// <param name="expiryDate">The expiration date for the API key.</param>
    /// <param name="salt">Optional salt value for additional security. Default: "your-salt-here"</param>
    /// <returns>A Base64 encoded SHA256 hash string.</returns>
    /// <remarks>
    /// The generated key combines userId, appId, expiry date, and salt to create a unique hash.
    ///
    /// Example:
    /// ```csharp
    /// var key = ApiKeyGenerator.Generate(
    ///     userId: "user123",
    ///     appId: "myApp",
    ///     expiryDate: DateTime.Now.AddMonths(1),
    ///     salt: "customSalt"
    /// );
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when userId or appId is null.</exception>
    public static string Generate(
        string userId,
        string appId,
        DateTime expiryDate,
        string salt = "your-salt-here"
    ) =>
        Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes($"{userId}-{appId}-{expiryDate:yyyyMMdd}-{salt}")
            )
        );

    /// <summary>
    /// Generates a temporary API key with specified validity period.
    /// </summary>
    /// <param name="validity">The time span for which the key should be valid.</param>
    /// <param name="purpose">The purpose or context for the temporary key.</param>
    /// <param name="salt">Optional salt value for additional security. Default: "your-salt-here"</param>
    /// <returns>A Base64 encoded SHA256 hash string.</returns>
    /// <remarks>
    /// Useful for generating short-lived API keys for specific operations.
    ///
    /// Example:
    /// ```csharp
    /// var tempKey = ApiKeyGenerator.GenerateTemporary(
    ///     validity: TimeSpan.FromHours(2),
    ///     purpose: "file-download",
    ///     salt: "customSalt"
    /// );
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when validity is negative or purpose is null/empty.</exception>
    public static string GenerateTemporary(
        TimeSpan validity,
        string purpose,
        string salt = "your-salt-here"
    ) =>
        Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    $"{purpose}-{DateTime.UtcNow.Add(validity):yyyyMMddHHmmss}-{salt}"
                )
            )
        );

    /// <summary>
    /// Generates a client-specific API key with permission set.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client.</param>
    /// <param name="clientSecret">The client's secret key.</param>
    /// <param name="permissions">Array of permission strings.</param>
    /// <param name="salt">Optional salt value for additional security. Default: "your-salt-here"</param>
    /// <returns>A Base64 encoded SHA256 hash string.</returns>
    /// <remarks>
    /// Combines client credentials with permissions to create a unique access key.
    ///
    /// Example:
    /// ```csharp
    /// var clientKey = ApiKeyGenerator.GenerateForClient(
    ///     clientId: "client123",
    ///     clientSecret: "secret456",
    ///     permissions: new[] { "read", "write" },
    ///     salt: "customSalt"
    /// );
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when clientId, clientSecret, or permissions is null.</exception>
    public static string GenerateForClient(
        string clientId,
        string clientSecret,
        string[] permissions,
        string salt = "your-salt-here"
    ) =>
        Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    $"{clientId}-{clientSecret}-{string.Join(",", permissions)}-{salt}"
                )
            )
        );

    /// <summary>
    /// Generates an environment-specific API key.
    /// </summary>
    /// <param name="environment">The target environment (e.g., "production", "staging").</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="version">The version identifier.</param>
    /// <param name="salt">Optional salt value for additional security. Default: "your-salt-here"</param>
    /// <returns>A Base64 encoded SHA256 hash string.</returns>
    /// <remarks>
    /// Creates environment-specific keys for service authentication.
    ///
    /// Example:
    /// ```csharp
    /// var envKey = ApiKeyGenerator.GenerateForEnvironment(
    ///     environment: "production",
    ///     serviceName: "payment-service",
    ///     version: "v1.0",
    ///     salt: "customSalt"
    /// );
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when environment, serviceName, or version is null.</exception>
    public static string GenerateForEnvironment(
        string environment,
        string serviceName,
        string version,
        string salt = "your-salt-here"
    ) =>
        Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes($"{environment}-{serviceName}-{version}-{salt}"))
        );

    /// <summary>
    /// Validates an API key against a collection of registered keys.
    /// </summary>
    /// <param name="apiKey">The API key to validate.</param>
    /// <param name="registeredKeys">Collection of valid API keys.</param>
    /// <returns>True if the API key is valid; otherwise, false.</returns>
    /// <remarks>
    /// Simple validation method that checks if the provided key exists in the registered keys collection.
    ///
    /// Example:
    /// ```csharp
    /// bool isValid = ApiKeyGenerator.ValidateApiKey(
    ///     apiKey: "someKey123",
    ///     registeredKeys: new[] { "validKey1", "validKey2" }
    /// );
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when apiKey is null.</exception>
    public static bool ValidateApiKey(string apiKey, IEnumerable<string> registeredKeys) =>
        registeredKeys?.Contains(apiKey) ?? false;

    /// <summary>
    /// Validates an API key and provides error information if validation fails.
    /// </summary>
    /// <param name="apiKey">The API key to validate.</param>
    /// <param name="registeredKeys">Collection of valid API keys.</param>
    /// <param name="errMessage">Output parameter containing error message if validation fails.</param>
    /// <returns>True if the API key is valid; otherwise, false.</returns>
    /// <remarks>
    /// Enhanced validation method that provides detailed error information.
    ///
    /// Example:
    /// ```csharp
    /// if (!ApiKeyGenerator.ValidateApiKey(
    ///     apiKey: "someKey123",
    ///     registeredKeys: new[] { "validKey1", "validKey2" },
    ///     out string? errorMessage
    /// ))
    /// {
    ///     Console.WriteLine($"Validation failed: {errorMessage}");
    /// }
    /// ```
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when apiKey is null.</exception>
    public static bool ValidateApiKey(
        string apiKey,
        IEnumerable<string> registeredKeys,
        out string? errMessage
    )
    {
        errMessage = null;
        try
        {
            if (registeredKeys == null || !registeredKeys.Any())
            {
                errMessage = "No registered API keys found";
                return false;
            }

            if (!registeredKeys.Contains(apiKey))
            {
                errMessage = "Invalid API key";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            errMessage = ex.Message;
            return false;
        }
    }
}
