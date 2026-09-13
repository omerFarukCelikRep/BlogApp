using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.EmailConfirmations.Commands;

public class SendEmailConfirmationCommandHandler(IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<SendEmailConfirmationCommand, Result>
{
    public async Task<Result> Handle(SendEmailConfirmationCommand request,
        CancellationToken cancellationToken = default)
    {
        return await emailConfirmationService.SendConfirmationAsync(cancellationToken);
    }
}