using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Comments.Queries;

public class GetBlogCommentsQueryValidator : Validator<GetBlogCommentsQuery>
{
    public GetBlogCommentsQueryValidator()
    {
        RuleFor(nameof(GetBlogCommentsQuery.BlogId), x => x.BlogId).NotNull().NotEmpty();
    }
}