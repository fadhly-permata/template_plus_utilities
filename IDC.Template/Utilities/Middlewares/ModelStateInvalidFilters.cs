using IDC.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Filter that validates the model state and returns appropriate error responses when validation fails.
/// </summary>
/// <remarks>
/// This filter implements IActionFilter to check model state validity before action execution.
/// When validation fails, it returns a BadRequestObjectResult with localized error messages.
/// </remarks>
/// <example>
/// <code>
/// builder.Services.AddControllers(options =>
/// {
///     options.Filters.Add(typeof(ModelStateInvalidFilters));
/// });
/// </code>
/// </example>
public class ModelStateInvalidFilters(Language language) : IActionFilter
{
    /// <summary>
    /// Executes the action before it is executed and checks if the model state is valid.
    /// If the model state is not valid, it returns a BadRequestObjectResult with the appropriate error message.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(
                new APIResponseData<List<string>?>()
                    .ChangeStatus(language, "api.status.failed")
                    .ChangeMessage(language, "api.message.request_validation_error")
                    .ChangeData(
                        [
                            .. context
                                .ModelState.Values.SelectMany(e => e.Errors)
                                .Select(e => language.GetMessage(path: e.ErrorMessage))
                        ]
                    )
            );
    }

    /// <summary>
    /// Called after the action method is invoked, but before the action result is executed.
    /// </summary>
    /// <param name="context">The context for the action execution.</param>
    public void OnActionExecuted(ActionExecutedContext context) { }
}
