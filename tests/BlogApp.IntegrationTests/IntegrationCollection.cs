using BlogApp.IntegrationTests.Fixtures;

namespace BlogApp.IntegrationTests;

[CollectionDefinition("Integration")]
public class IntegrationCollection : ICollectionFixture<BlogAppFactory>
{
}