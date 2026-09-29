using BlogApp.Core.Validations;

namespace BlogApp.Application.Authors;

public class GetByUsernameQueryValidator : Validator<GetByUsernameQuery>
{
    public GetByUsernameQueryValidator()
    {
        RuleFor(nameof(GetByUsernameQuery.Username), x => x.Username);
    }
}