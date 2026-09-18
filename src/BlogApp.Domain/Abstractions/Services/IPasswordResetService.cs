using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Domain.Abstractions.Services;

public interface IPasswordResetService
{
    Task<Result> CreateForgotPasswordTokenAsync(ForgotPasswordArgs args, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordArgs args, CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordWithOtpAsync(ResetPasswordWithOtpArgs args,
        CancellationToken cancellationToken = default);
}