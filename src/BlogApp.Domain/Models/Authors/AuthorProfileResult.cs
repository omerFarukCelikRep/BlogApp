using BlogApp.Domain.Models.Tags;

namespace BlogApp.Domain.Models.Authors;

public record AuthorProfileResult(
    Guid Id,
    string FullName,
    string Username,
    string? Bio,
    string? ProfilePicture,
    string? WebSiteUrl,
    string? TwitterHandle,
    string? GithubHandle,
    DateTime JoinedDate,
    int BlogCount,
    int TotalReads,
    int TotalLikes,
    List<TagResult> TopTags)
{
    public static explicit operator AuthorProfileResult(User user) => new(
        Id: user.Id,
        FullName: user.FullName,
        Username: user.Username,
        Bio: user.Bio,
        ProfilePicture: user.ProfilePicture,
        WebSiteUrl: user.WebSiteUrl,
        TwitterHandle: user.TwitterHandle,
        GithubHandle: user.GithubHandle,
        JoinedDate: user.CreatedDate.DateTime,
        BlogCount: 0,
        TotalReads: 0,
        TotalLikes: 0,
        TopTags: []);
}