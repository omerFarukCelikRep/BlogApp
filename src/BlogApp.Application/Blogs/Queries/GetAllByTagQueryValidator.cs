using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByTagQueryValidator : Validator<GetAllByTagQuery>
{
    public GetAllByTagQueryValidator()
    {
        RuleFor(nameof(GetAllByTagQuery.Slug), x => x.Slug).NotNull().NotEmpty();
    }
}