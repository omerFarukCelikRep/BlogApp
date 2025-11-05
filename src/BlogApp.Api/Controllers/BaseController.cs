using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class BaseController : ControllerBase
{
    private const string DefaultIpAddress = "::1";
    protected IActionResult GetResponse(Core.Results.Result result)
    {
        return new ObjectResult(result)
        {
            StatusCode = result.StatusCode
        };
    }

    protected string GetIpAddress()
    {
        var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
        return remoteIpAddress is null ? DefaultIpAddress : remoteIpAddress.ToString();
    }
}