using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Categories.Commands;

public class DeleteCategoryCommandHandler(ICategoryService categoryService)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken = default)
    {
        return await categoryService.DeleteAsync(request, cancellationToken);
    }
}