using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Authors;

namespace BlogApp.Application.Authors;

public record GetByUsernameQuery(string Username)
    : GetByUsernameArgs(Username), IRequest<Result<AuthorProfileResult>>, ICacheable
{
    public string Key => $"authors:username:{Username}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(10);
}