using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Blogs.Commands;

public class UpdateBlogCommandValidator : Validator<UpdateBlogCommand>
{
    public UpdateBlogCommandValidator()
    {
        RuleFor(nameof(UpdateBlogCommand.Id), x => x.Id).NotNull().NotEmpty();

        RuleFor(nameof(UpdateBlogCommand.Title), x => x.Title)
            .NotNull()
            .NotEmpty()
            .MinLength(3)
            .MaxLength(512);

        RuleFor(nameof(UpdateBlogCommand.Content), x => x.Content)
            .NotNull()
            .NotEmpty()
            .MinLength(50);

        RuleFor(nameof(UpdateBlogCommand.Thumbnail), x => x.Thumbnail)
            .MaxLength(1024)
            .When(x => x.Thumbnail is not null);

        RuleFor(nameof(UpdateBlogCommand.Tags), x => x.Tags)
            .NotNull()
            .NotEmpty()!
            .LessThanOrEqual(10);

        RuleFor(nameof(UpdateBlogCommand.Categories), x => x.Categories)
            .NotNull()
            .NotEmpty()!
            .LessThanOrEqual(10);
    }
}