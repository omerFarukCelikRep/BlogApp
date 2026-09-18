using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class ResetPasswordWithOtpCommandHandler(IPasswordResetService passwordResetService)
    : IRequestHandler<ResetPasswordWithOtpCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordWithOtpCommand request, CancellationToken cancellationToken = default)
    {
        return await passwordResetService.ResetPasswordWithOtpAsync(request, cancellationToken);
    }
}