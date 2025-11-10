namespace BlogApp.Application.Auth.Commands;

public class RegisterCommandValidator : IValidator<RegisterCommand>
{
    public Task<IEnumerable<string>> ValidateAsync(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}