using BlogApp.Core.Validations;
using BlogApp.Core.Validations.Extensions;

namespace BlogApp.Application.Admins.Commands;

public class AssignRoleCommandValidator : Validator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(nameof(AssignRoleCommand.UserId), x => x.UserId).NotNull().NotEmpty();
        RuleFor(nameof(AssignRoleCommand.Role), x => x.Role).NotNull().NotEmpty();
        RuleFor(nameof(AssignRoleCommand.Assign), x => x.Role).NotNull().NotEmpty();
    }
}