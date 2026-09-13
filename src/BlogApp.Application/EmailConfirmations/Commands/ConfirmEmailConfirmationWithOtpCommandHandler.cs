using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.EmailConfirmations.Commands;

public class ConfirmEmailConfirmationWithOtpCommandHandler(IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<ConfirmEmailConfirmationWithOtpCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailConfirmationWithOtpCommand request,
        CancellationToken cancellationToken = default)
    {
        return await emailConfirmationService.ConfirmWithOtpAsync(request, cancellationToken);
    }
}