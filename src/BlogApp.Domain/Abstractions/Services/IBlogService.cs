using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Domain.Abstractions.Services;

public interface IBlogService
{
    Task<Result<List<BlogSummaryResult>>> GetTopReadAsync(TopReadBlogArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<List<BlogSummaryResult>>> GetRandomAsync(RandomBlogArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<CreateBlogResult>> CreateAsync(CreateBlogArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<BlogDetailsResult>> GetByIdAsync(GetBlogByIdArgs args, CancellationToken cancellationToken = default);
    Task<Result> IncrementReadCountAsync(IncrementReadCountArgs args, CancellationToken cancellationToken = default);
    Task<Result<ToggleLikeResult>> ToggleLikeAsync(ToggleLikeArgs args, CancellationToken cancellationToken = default);

    Task<Result<BlogForUpdateResult>> GetForUpdateAsync(GetBlogByIdArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<BlogDetailsResult>> UpdateAsync(UpdateBlogArgs args, CancellationToken cancellationToken = default);

    Task<PaginatedResult<BlogSummaryResult>> GetAllByTagAsync(GetBlogByTagArgs args,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<BlogSummaryResult>> GetAllByCategoryAsync(GetBlogByCategoryArgs args,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<BlogSummaryResult>> GetAllByAuthorAsync(GetBlogByAuthorArgs args,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<AdminBlogSummaryResult>> GetAllForAdminAsync(GetAllForAdminsArgs args,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteForAdminAsync(DeleteBlogForAdminArgs args, CancellationToken cancellationToken = default);
}