using BlogApp.Api.Endpoints.Blogs.Requests;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Api.Extensions;
using BlogApp.Api.Filters;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Mediator.Abstractions;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs;

public static class CreateEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder CreateEndpoint()
        {
            builder.MapPost("/",
                async ([FromBody] CreateBlogRequest request, CancellationToken cancellationToken,
                    [FromServices] IMediator mediator) =>
                {
                    var command = (CreateBlogCommand)request;
                    var result =
                        await mediator.Send<CreateBlogCommand, Result<CreateBlogResult>>(command, cancellationToken);

                    return result.ToResponse();
                })
                .RequireAuthorization()
                .Produces<Response<CreateBlogResult>>()
                .WithName("BlogCreate")
                .WithTags("Create")
                .AddEndpointFilter<XssProtectionEndpointFilter>();

            return builder;
        }
    }
}