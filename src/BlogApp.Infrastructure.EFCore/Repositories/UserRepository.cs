using System.Globalization;
using BlogApp.Core.DataAccess.Enums;
using BlogApp.Core.DataAccess.Models;
using BlogApp.Core.EFCore.Extensions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Admins;
using BlogApp.Domain.Models.Authors;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.Infrastructure.EFCore.Repositories;

public class UserRepository(BlogAppDbContext context) : EFBaseRepository<User, Guid>(context), IUserRepository
{
    public async Task<User?> GetAuthorProfileAsync(GetByUsernameArgs args,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.Username.Equals(args.Username))
            .Include(x => x.Blogs.Where(b => b.IsPublished()))
            .ThenInclude(x => x.BlogTags)
            .ThenInclude(x => x.Tag)
            .Include(x => x.Blogs)
            .ThenInclude(x => x.Likes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<User>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .OrderByDescending(x => x.CreatedDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DailyCountResult>> GetDailyCountAsync(DateTime from,
        CancellationToken cancellationToken = default)
    {
        return await GetAll(false)
            .Where(x => x.CreatedDate >= from)
            .OrderBy(x => x.CreatedDate)
            .GroupBy(x => x.CreatedDate.Date)
            .Select(x => new DailyCountResult(x.Key.ToString(CultureInfo.CurrentCulture), x.Count()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IPaginate<User>> GetAllAdminsAsync(GetAllAdminsArgs args,
        CancellationToken cancellationToken = default)
    {
        var query = GetAll(false)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Blogs
                .Where(b => b.PostStatus == PostStatus.Published))
            .AsQueryable();

        if (!string.IsNullOrEmpty(args.Search))
            query = query.Where(u => EF.Functions.ILike(u.Username, $"%{args.Search}%")
                                     || EF.Functions.ILike(u.Email, $"%{args.Search}%")
                                     || EF.Functions.ILike(u.FullName, $"%{args.Search}%"));

        if (!string.IsNullOrEmpty(args.Role))
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role!.Name == args.Role));

        if (args.IsBanned.HasValue)
            query = query.Where(u => args.IsBanned.Value
                ? u.Status == Status.Deleted
                : u.Status == Status.Added);

        query = query.OrderByDescending(u => u.CreatedDate);

        return await query.ToPaginateAsync(args.Page!.Value, args.PageSize!.Value, cancellationToken);
    }
}