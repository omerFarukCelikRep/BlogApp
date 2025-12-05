using BlogApp.Core.Exceptions;
using BlogApp.Core.Validations;

namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandValidator : Validator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
    }
}