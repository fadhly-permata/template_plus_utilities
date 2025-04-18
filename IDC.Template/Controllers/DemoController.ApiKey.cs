using IDC.Template.Utilities;
using IDC.Template.Utilities.Models;
using IDC.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

/// <summary>
/// Controller for managing API key operations and generation
/// </summary>
/// <remarks>
/// Provides endpoints for generating different types of API keys including:
/// - User-specific API keys
/// - Temporary API keys
/// - Client-specific API keys
/// - Environment-specific API keys
/// </remarks>
/// <param name="language">Service for handling localization and messages</param>
/// <param name="systemLogging">Service for system logging operations</param>
/// <example>
/// <code>
/// var controller = new DemoControllerApiKeys(
///     language: new Language(),
///     systemLogging: new SystemLogging()
/// );
/// var response = controller.GenerateUserApiKey(new UserApiKeyRequest());
/// </code>
/// </example>
[Route("api/demo/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Demo")]
public class DemoControllerApiKeys(Language language, SystemLogging systemLogging) : ControllerBase
{
    private static string GetSalt() => System.IO.File.ReadAllText("wwwroot/security/enc_salt.txt");

    /// <summary>
    /// Generates a user-specific API key
    /// </summary>
    /// <param name="request">User API key request containing userId, appId, and expiryDate</param>
    /// <returns>Generated API key</returns>
    [Tags(tags: "API Keys"), HttpPost(template: "user")]
    public APIResponseData<string> GenerateUserApiKey([FromBody] UserApiKeyRequest request)
    {
        try
        {
            return new APIResponseData<string>().ChangeData(
                ApiKeyGenerator.Generate(
                    userId: request.UserId,
                    appId: request.AppId,
                    expiryDate: request.ExpiryDate,
                    salt: GetSalt()
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<string>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Generates a temporary API key with specified validity period
    /// </summary>
    /// <param name="request">Temporary API key request containing validity period and purpose</param>
    /// <returns>Generated temporary API key</returns>
    [Tags(tags: "API Keys"), HttpPost(template: "temporary")]
    public APIResponseData<string> GenerateTemporaryApiKey(
        [FromBody] TemporaryApiKeyRequest request
    )
    {
        try
        {
            return new APIResponseData<string>().ChangeData(
                ApiKeyGenerator.GenerateTemporary(
                    validity: request.Validity,
                    purpose: request.Purpose,
                    salt: GetSalt()
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<string>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Generates a client-specific API key with permissions
    /// </summary>
    /// <param name="request">Client API key request containing clientId, secret, and permissions</param>
    /// <returns>Generated client API key</returns>
    [Tags(tags: "API Keys"), HttpPost(template: "client")]
    public APIResponseData<string> GenerateClientApiKey([FromBody] ClientApiKeyRequest request)
    {
        try
        {
            return new APIResponseData<string>().ChangeData(
                ApiKeyGenerator.GenerateForClient(
                    clientId: request.ClientId,
                    clientSecret: request.ClientSecret,
                    permissions: request.Permissions,
                    salt: GetSalt()
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<string>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Generates an environment-specific API key
    /// </summary>
    /// <param name="request">Environment API key request containing environment, service name, and version</param>
    /// <returns>Generated environment API key</returns>
    [Tags(tags: "API Keys"), HttpPost(template: "environment")]
    public APIResponseData<string> GenerateEnvironmentApiKey(
        [FromBody] EnvironmentApiKeyRequest request
    )
    {
        try
        {
            return new APIResponseData<string>().ChangeData(
                ApiKeyGenerator.GenerateForEnvironment(
                    environment: request.Environment,
                    serviceName: request.ServiceName,
                    version: request.Version,
                    salt: GetSalt()
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<string>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
