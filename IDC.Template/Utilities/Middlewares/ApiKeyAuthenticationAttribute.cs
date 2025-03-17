using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Attribute for applying API key authentication to controllers or actions.
/// </summary>
/// <remarks>
/// This attribute is used to enforce API key authentication on specific controllers or actions.
/// It works in conjunction with the <see cref="ApiKeyAuthenticationMiddleware"/>.
///
/// Example:
/// <code>
/// [ApiKeyAuthentication]
/// public class SecureController : ControllerBase
/// {
///     // Protected endpoints
/// }
///
/// public class MixedController : ControllerBase
/// {
///     [ApiKeyAuthentication]
///     public IActionResult SecureEndpoint()
///     {
///         // Protected endpoint
///     }
/// }
/// </code>
/// </remarks>
/// <seealso cref="TypeFilterAttribute"/>
/// <seealso cref="ApiKeyAuthenticationMiddleware"/>
/// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/security/authentication/">ASP.NET Core Authentication</seealso>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthenticationAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationAttribute"/> class.
    /// </summary>
    /// <remarks>
    /// Creates attribute that injects and uses <see cref="ApiKeyAuthenticationMiddleware"/>
    /// for authentication.
    /// </remarks>
    public ApiKeyAuthenticationAttribute()
        : base(typeof(ApiKeyAuthenticationMiddleware)) { }
}
