using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Authors;

namespace BlogApp.Application.Authors;

public class GetByUsernameQueryHandler(IAuthorService authorService)
    : IRequestHandler<GetByUsernameQuery, Result<AuthorProfileResult>>
{
    public async Task<Result<AuthorProfileResult>> Handle(GetByUsernameQuery request,
        CancellationToken cancellationToken = default)
    {
        return await authorService.GetByUsernameAsync(request, cancellationToken);
    }
}