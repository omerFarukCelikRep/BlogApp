using BlogApp.Infrastructure.Search.Indexing;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Search;

public static class ReindexEndpoint
{
    extension(RouteGroupBuilder builder)
    {
        public RouteGroupBuilder ReindexEndpoint()
        {
            builder.MapPost("reindex",
                    async (CancellationToken cancellationToken, [FromServices] ElasticsearchIndexer indexer) =>
                    {
                        await indexer.ReIndexAllAsync(cancellationToken);
                        return Results.NoContent();
                    })
                .RequireAuthorization()
                .WithName("Reindex");

            return builder;
        }
    }
}