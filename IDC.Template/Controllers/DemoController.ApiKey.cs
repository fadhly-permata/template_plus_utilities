using IDC.Template.Utilities;
using IDC.Template.Utilities.Models;
using IDC.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

public partial class DemoController
{
    private static string GetSalt() => System.IO.File.ReadAllText("wwwroot/security/enc_salt.txt");

    /// <summary>
    /// Generates a user-specific API key
    /// </summary>
    /// <param name="request">User API key request containing userId, appId, and expiryDate</param>
    /// <returns>Generated API key</returns>
    [Tags(tags: "API Keys"), HttpPost(template: "ApiKey/user")]
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
    [Tags(tags: "API Keys"), HttpPost(template: "ApiKey/temporary")]
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
    [Tags(tags: "API Keys"), HttpPost(template: "ApiKey/client")]
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
    [Tags(tags: "API Keys"), HttpPost(template: "ApiKey/environment")]
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
