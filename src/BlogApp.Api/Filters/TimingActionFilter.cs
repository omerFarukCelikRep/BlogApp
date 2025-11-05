using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApp.Api.Filters;

public class TimingActionFilter :IActionFilter
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
}