using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlogApp.Domain.Options;

public class JwtOptions : JwtBearerOptions
{
    public string Key { get; set; } = null!;
}