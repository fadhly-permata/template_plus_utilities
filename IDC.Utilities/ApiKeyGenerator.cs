using System.Security.Cryptography;
using System.Text;

namespace IDC.Utilities;

public sealed class ApiKeyGenerator
{
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

    public static string GenerateForEnvironment(
        string environment,
        string serviceName,
        string version,
        string salt = "your-salt-here"
    ) =>
        Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes($"{environment}-{serviceName}-{version}-{salt}"))
        );

    public static bool ValidateApiKey(string apiKey, IEnumerable<string> registeredKeys) =>
        registeredKeys?.Contains(apiKey) ?? false;

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
