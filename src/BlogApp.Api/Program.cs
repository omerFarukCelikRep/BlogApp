using BlogApp.Api.Extensions;
using BlogApp.Api.Handlers;
using BlogApp.Application.Extensions;
using BlogApp.Core.Logging.Extensions;
using BlogApp.Domain.Extensions;
using BlogApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services
    .AddDomainServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddApiServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapEndpoints();

app.Run();