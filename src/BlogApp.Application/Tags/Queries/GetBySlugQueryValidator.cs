using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Tags.Queries;

public class GetBySlugQueryValidator : Validator<GetBySlugQuery>
{
    public GetBySlugQueryValidator()
    {
        RuleFor(nameof(GetBySlugQuery.Slug), x => x.Slug).NotNull().NotEmpty();
    }
}