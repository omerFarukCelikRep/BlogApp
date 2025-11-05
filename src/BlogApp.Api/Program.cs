using BlogApp.Api.Extensions;
using BlogApp.Api.Handlers;
using BlogApp.Core.Logging.Extensions;
using BlogApp.Core.Logging.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapControllers();

app.MapEndpoints();

app.Run();