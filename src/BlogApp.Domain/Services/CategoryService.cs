using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Domain.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private static CategoryDetailsResult BuildTree(Category category, List<Category> categories)
    {
        var children = categories
            .Where(c => c.ParentId == category.Id)
            .Select(c => BuildTree(c, categories))
            .ToList();

        var blogCount = category.BlogCategories.Count(bc => bc.Blog!.IsPublished());

        var totalCount = blogCount + children.Sum(c => c.BlogCount);

        return new(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            BlogCount: totalCount,
            Children: children);
    }

    public async Task<Result<List<CategoryResult>>> GetTopAsync(TopCategoryArgs args,
        CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetTopAsync(args.Count, cancellationToken);

        List<CategoryResult> result = [.. categories.Select(x => (CategoryResult)x)];
        return Result<List<CategoryResult>>.Success(data: result);
    }

    public async Task<Result<List<CategoryDetailsResult>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        if (categories is not { Count: > 0 })
            return Result<List<CategoryDetailsResult>>.Failed(404, Error.Create(Errors.Category.NotFound));

        var tree = categories
            .Where(c => c.ParentId is null)
            .Select(c => BuildTree(c, categories))
            .ToList();

        return Result<List<CategoryDetailsResult>>.Success(data: tree);
    }

    public async Task<Result<CategoryDetailsResult>> GetBySlug(GetCategoryBySlugArgs args,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetBySlugAsync(args, cancellationToken);
        if (category is null)
            return Result<CategoryDetailsResult>.Failed(404, Error.Create(Errors.Category.NotFound));

        var blogCount = category.BlogCategories.Count(x => x.Blog!.IsPublished());

        var children = category.SubCategories
            .Select(c => new CategoryDetailsResult(
                Id: c.Id,
                Name: c.Name,
                Slug: c.Slug,
                BlogCount: c.BlogCategories.Count(x => x.Blog!.IsPublished()),
                Children: []))
            .ToList();

        var parent = category.Parent is null
            ? null
            : new CategoryResult(
                Id: category.Parent.Id,
                Name: category.Parent.Name,
                Slug: category.Parent.Slug);

        var result = new CategoryDetailsResult(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            BlogCount: blogCount,
            Parent: parent,
            Children: children);

        return Result<CategoryDetailsResult>.Success(data: result);
    }

    public async Task<Result<CategoryResult>> CreateAsync(CreateCategoryArgs args,
        CancellationToken cancellationToken = default)
    {
        var slug = args.Name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Trim();

        Category? parent = null;
        if (args.ParentId.HasValue)
        {
            parent = await categoryRepository.GetByIdAsync(args.ParentId.Value, tracking: false, cancellationToken);
            if (parent is null)
                return Result<CategoryResult>.Failed(404, Error.Create(Errors.Category.NotFound));
        }

        var category = new Category
        {
            Name = args.Name,
            Slug = slug,
            ParentId = parent?.Id,
        };

        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CategoryResult>.Success(data: (CategoryResult)category);
    }

    public async Task<Result> DeleteAsync(DeleteCategoryArgs args, CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(args.CategoryId, tracking: true, cancellationToken);
        if (category is null)
            return Result.Failed(404, Error.Create(Errors.Category.NotFound));

        await categoryRepository.DeleteAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<PaginatedResult<CategoryResult>> GetAllForAdminAsync(GetAllCategoriesForAdminArgs args,
        CancellationToken cancellationToken = default)
    {
        var categories =
            await categoryRepository.GetAllAsPaginateAsync(args.Page!.Value, args.PageSize!.Value, false,
                cancellationToken);

        var data = categories.Items.Select(x => (CategoryResult)x);
        return PaginatedResult<CategoryResult>.Success(data: data, categories.Index, categories.Size, categories.Pages);
    }
}