using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Helpers;

public static class BlogTestHelper
{
    public static async Task<Category> SeedCategoryAsync(BlogAppDbContext db)
    {
        var category = new Category
        {
            Name        = $"Category_{Guid.NewGuid():N}",
            Slug        = $"category-{Guid.NewGuid():N}",
            CreatedBy   = "seed",
            CreatedDate = DateTime.UtcNow,
            Status      = Status.Added,
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    public static async Task AssignRoleAsync(
        BlogAppDbContext db,
        string email,
        string roleName)
    {
        var user = await db.Users
            .FirstAsync(u => u.Email == email);

        var role = await db.Roles
            .FirstAsync(r => r.Name == roleName);

        db.UserRoles.Add(new UserRole
        {
            UserId      = user.Id,
            RoleId      = role.Id,
            CreatedBy   = "seed",
            CreatedDate = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
    }
}