using System.Globalization;
using BlogApp.Core.DataAccess.Enums;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Admins;

namespace BlogApp.Domain.Services;

public class AdminService(
    IBlogRepository blogRepository,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    ITagRepository tagRepository,
    ICategoryRepository categoryRepository,
    IRoleManager roleManager) : IAdminService
{
    public async Task<Result<DashboardStatsResult>> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);
        var monthAgo = now.AddDays(-30);

        var tasks = (
            totalUsers: userRepository.CountAsync(cancellationToken),
            totalBlogs: blogRepository.CountAsync(cancellationToken),
            totalComments: commentRepository.CountAsync(cancellationToken),
            totalTags: tagRepository.CountAsync(cancellationToken),
            totalCategories: categoryRepository.CountAsync(cancellationToken),
            newUsersThisWeek: userRepository.CountAsync(u => u.CreatedDate >= weekAgo, cancellationToken),
            newBlogsThisWeek: blogRepository.CountAsync(b => b.CreatedDate >= weekAgo, cancellationToken),
            newCommentsThisWeek: commentRepository.CountAsync(c => c.CreatedDate >= weekAgo, cancellationToken),
            publishedBlogs: blogRepository.CountAsync(b => b.PostStatus == PostStatus.Published, cancellationToken),
            draftBlogs: blogRepository.CountAsync(b => b.PostStatus == PostStatus.Draft, cancellationToken),
            archivedBlogs: blogRepository.CountAsync(b => b.PostStatus == PostStatus.Archived, cancellationToken),
            totalLikes: blogRepository.SumAsync(b => b.Likes.Count, cancellationToken)
        );

        await Task.WhenAll(
            tasks.totalUsers,
            tasks.totalBlogs,
            tasks.totalComments,
            tasks.totalTags,
            tasks.totalCategories,
            tasks.newUsersThisWeek,
            tasks.newBlogsThisWeek,
            tasks.newCommentsThisWeek,
            tasks.publishedBlogs,
            tasks.draftBlogs,
            tasks.archivedBlogs,
            tasks.totalLikes
        );

        var totalUsers = await tasks.totalUsers;
        var totalBlogs = await tasks.totalBlogs;
        var totalComments = await tasks.totalComments;
        var totalTags = await tasks.totalTags;
        var totalCategories = await tasks.totalCategories;
        var newUsersThisWeek = await tasks.newUsersThisWeek;
        var newBlogsThisWeek = await tasks.newBlogsThisWeek;
        var newCommentsThisWeek = await tasks.newCommentsThisWeek;
        var publishedBlogs = await tasks.publishedBlogs;
        var draftBlogs = await tasks.draftBlogs;
        var archivedBlogs = await tasks.archivedBlogs;
        var totalLikes = await tasks.totalLikes;

        var blogsPerDay = await blogRepository.GetDailyCountAsync(monthAgo, cancellationToken);
        var usersPerDay = await userRepository.GetDailyCountAsync(monthAgo, cancellationToken);

        var recentBlogs = await blogRepository.GetRecentAsync(5, cancellationToken);
        var recentUsers = await userRepository.GetRecentAsync(5, cancellationToken);

        var recentActivity = recentBlogs
            .Select(b => new RecentActivityResult(
                Type: "blog",
                Message: $"New blog: \"{b.Title}\"",
                Actor: b.Author?.Username ?? "unknown",
                Date: b.CreatedDate.DateTime))
            .Concat(recentUsers.Select(u => new RecentActivityResult(
                Type: "user",
                Message: "New user registered",
                Actor: u.Username,
                Date: u.CreatedDate.DateTime)))
            .OrderByDescending(a => a.Date)
            .Take(10)
            .ToList();

        return Result<DashboardStatsResult>.Success(data: new DashboardStatsResult(
            TotalUsers: totalUsers,
            TotalBlogs: totalBlogs,
            TotalComments: totalComments,
            TotalLikes: (int)totalLikes,
            TotalTags: totalTags,
            TotalCategories: totalCategories,
            NewUsersThisWeek: newUsersThisWeek,
            NewBlogsThisWeek: newBlogsThisWeek,
            NewCommentsThisWeek: newCommentsThisWeek,
            PublishedBlogs: publishedBlogs,
            DraftBlogs: draftBlogs,
            ArchivedBlogs: archivedBlogs,
            RecentActivity: recentActivity,
            BlogsPerDay: blogsPerDay,
            UsersPerDay: usersPerDay));
    }

    public async Task<PaginatedResult<AdminUserResult>> GetAllAsync(GetAllAdminsArgs args,
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAdminsAsync(args, cancellationToken);
        var data = users.Items.Select(u => new AdminUserResult(
            Id: u.Id,
            FullName: u.FullName,
            Username: u.Username,
            Email: u.Email,
            EmailConfirmed: u.EmailConfirmed,
            TwoFactorEnabled: u.TwoFactorEnabled,
            IsBanned: u.Status == Status.Deleted,
            PhoneNumber: u.PhoneNumber,
            JoinDate: u.CreatedDate.DateTime,
            PostCount: u.Blogs.Count,
            Roles:
            [
                .. u.UserRoles
                    .Select(ur => ur.Role!.Name)
            ])).ToList();

        return PaginatedResult<AdminUserResult>.Success(data: data, users.Index, users.Size, users.Pages);
    }

    public async Task<Result> BanUserAsync(BanUserArgs args, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(args.UserId, true, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        user.ChangeBanStatus(args.Ban);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AssignRoleAsync(AssignRoleArgs args, CancellationToken cancellationToken = default)
    {
        if (args.Assign)
            await roleManager.AssignRoleAsync(args.UserId, args.Role, cancellationToken);
        else
            await roleManager.RevokeRoleAsync(args.UserId, args.Role.ToString(), cancellationToken);

        return Result.Success();
    }
}