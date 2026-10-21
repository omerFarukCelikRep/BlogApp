using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Commands;

public class CreateBlogCommandValidator : Validator<CreateBlogCommand>
{
    public CreateBlogCommandValidator()
    {
        RuleFor(nameof(CreateBlogCommand.Title), x => x.Title)
            .NotNull()
            .NotEmpty()
            .MinLength(3)
            .MaxLength(512);

        RuleFor(nameof(CreateBlogCommand.Content), x => x.Content)
            .NotNull()
            .NotEmpty()
            .MinLength(50);

        RuleFor(nameof(CreateBlogCommand.Thumbnail), x => x.Thumbnail)
            .MaxLength(1024)
            .When(x => x.Thumbnail is not null);

        RuleFor(nameof(CreateBlogCommand.Categories), x => x.Categories)
            .NotNull()
            .NotEmpty();

        RuleFor(nameof(CreateBlogCommand.Tags), x => x.Tags)
            .NotNull()
            .NotEmpty();
    }
}