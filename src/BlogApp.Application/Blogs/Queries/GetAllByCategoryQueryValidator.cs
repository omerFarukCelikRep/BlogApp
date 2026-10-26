using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByCategoryQueryValidator : Validator<GetAllByCategoryQuery>
{
    public GetAllByCategoryQueryValidator()
    {
        RuleFor(nameof(GetAllByCategoryQuery.Category), x => x.Category).NotNull().NotEmpty();
    }
}