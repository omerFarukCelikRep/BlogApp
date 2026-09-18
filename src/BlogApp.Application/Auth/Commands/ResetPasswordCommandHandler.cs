using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class ResetPasswordCommandHandler(IPasswordResetService passwordResetService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken = default)
    {
        return await passwordResetService.ResetPasswordAsync(request, cancellationToken);
    }
}