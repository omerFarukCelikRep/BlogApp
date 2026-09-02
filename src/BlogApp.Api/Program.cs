using BlogApp.Api.Extensions;
using BlogApp.Application.Extensions;
using BlogApp.Core.Logging.Extensions;
using BlogApp.Domain.Extensions;
using BlogApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSettingFiles();

builder.Host.UseSerilog();

builder.Services
    .AddDomainServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddApiServices();

var app = builder.Build();

app.UseSerilog();

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseTimeZone();

app.UseCorrelation();

app.UseAuthentication();

app.UseAuthorization();

app.UseRequestLocalization();

app.MapEndpoints();

app.Run();