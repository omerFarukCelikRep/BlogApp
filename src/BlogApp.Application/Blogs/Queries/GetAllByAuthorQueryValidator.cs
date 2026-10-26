using BlogApp.Core.Validations;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByAuthorQueryValidator : Validator<GetAllByAuthorQuery>
{
    public GetAllByAuthorQueryValidator()
    {
        RuleFor(nameof(GetAllByAuthorQuery.Username), x => x.Username);
    }
}