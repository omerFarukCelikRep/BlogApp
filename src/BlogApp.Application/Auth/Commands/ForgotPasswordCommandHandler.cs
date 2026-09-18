using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Auth.Commands;

public class ForgotPasswordCommandHandler(IPasswordResetService passwordResetService)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken = default)
    {
        return await passwordResetService.CreateForgotPasswordTokenAsync(request, cancellationToken);
    }
}