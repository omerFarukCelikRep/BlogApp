using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.EmailConfirmations.Commands;

public class ConfirmEmailConfirmationCommandHandler(IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<ConfirmEmailConfirmationCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailConfirmationCommand request,
        CancellationToken cancellationToken = default)
    {
        return await emailConfirmationService.ConfirmAsync(request, cancellationToken);
    }
}