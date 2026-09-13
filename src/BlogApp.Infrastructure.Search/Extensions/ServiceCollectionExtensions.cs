using BlogApp.Core.Search.Abstractions;
using BlogApp.Core.Search.Options;
using BlogApp.Infrastructure.Search.Indexing;
using BlogApp.Infrastructure.Search.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlogApp.Infrastructure.Search.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSearch(IConfiguration configuration)
        {
            services.Configure<SearchOptions>(configuration.GetSection(SearchOptions.Section));
            
            var searchOptions = configuration.GetSection(SearchOptions.Section).Get<SearchOptions>() ?? new SearchOptions();

            var elasticsearchSettings = new ElasticsearchClientSettings(new Uri(searchOptions.Elasticsearch.Uri));
            if (!string.IsNullOrEmpty(searchOptions.Elasticsearch.Username))
                elasticsearchSettings.Authentication(new BasicAuthentication(searchOptions.Elasticsearch.Username,
                    searchOptions.Elasticsearch.Password));

            services.AddSingleton(new ElasticsearchClient(elasticsearchSettings))
                .AddScoped<ElasticsearchIndexer>()
                .AddScoped<PostgresSearchService>()
                .AddScoped<ElasticsearchSearchService>()
                .AddScoped<ISearchService>(sp => searchOptions.Provider.ToLowerInvariant() switch
                {
                    "elasticsearch" => sp.GetRequiredService<ElasticsearchSearchService>(),
                    _ => sp.GetRequiredService<PostgresSearchService>()
                });
            
            return services;
        }
    }
}