using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Queries;

public class GetForUpdateQueryValidator : Validator<GetForUpdateQuery>
{
    public GetForUpdateQueryValidator()
    {
        RuleFor(nameof(GetByIdQuery.Id), x => x.Id).NotNull().NotEmpty();
    }
}