using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApp.Api.Filters;

public class TimingActionFilter : IEndpointFilter
{
    private readonly Stopwatch _stopwatch = new();

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch.Start();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"ActionExecuted: {elapsedMilliseconds}");
        _stopwatch.Reset();
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _stopwatch.Start();
        var action = await next(context);
        _stopwatch.Stop();
        var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"ActionExecuted: {elapsedMilliseconds}");
        _stopwatch.Reset();
        return action;
    }
}