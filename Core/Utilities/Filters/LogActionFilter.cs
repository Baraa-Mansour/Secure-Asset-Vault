using Core.Requests;
using Core.Utilities.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.Utilities.Filters;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;
    private Stopwatch _stopwatch = null!;

    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();

        var method = context.ActionDescriptor.DisplayName;
        var parameters = context.ActionArguments.ToDictionary(
                kvp => kvp.Key,
                kvp => Sanitize(kvp.Value)
            );

        

        var hasLogAttr = context.ActionDescriptor.EndpointMetadata.OfType<LogActionAttribute>().Any();

        if (hasLogAttr)
        {
            _logger.LogInformation("Start: {Method} with params {@Params}", method, parameters);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();

        var method = context.ActionDescriptor.DisplayName;
        var hasLogAttr = context.ActionDescriptor.EndpointMetadata.OfType<LogActionAttribute>().Any();

        if (hasLogAttr)
        {
            if (context.Exception is null)
            {
                _logger.LogInformation("End: {Method} took {Elapsed}ms", method, _stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogError(context.Exception, "Error in {Method} after {Elapsed}ms", method, _stopwatch.ElapsedMilliseconds);
            }
        }
    }

    /// <summary>
    /// check if the request have sensetive data
    /// </summary>
    /// <param name="value"></param>
    /// <returns>object with safe data</returns>
    private object? Sanitize(object? value)
    {
        if(value is RegisterRequest register)
        {
            return new
            {
                register.Username,
                register.Email
            };
        }
        if (value is LoginRequest logIn)
        {
            return new
            {
                logIn.UsernameOrEmail,
            };
        }

        return value;
    }
}
