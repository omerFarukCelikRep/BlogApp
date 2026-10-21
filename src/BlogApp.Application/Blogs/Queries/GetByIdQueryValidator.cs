using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Queries;

public class GetByIdQueryValidator : Validator<GetByIdQuery>
{
    public GetByIdQueryValidator()
    {
        RuleFor(nameof(GetByIdQuery.Id), x => x.Id).NotNull().NotEmpty();
    }
}