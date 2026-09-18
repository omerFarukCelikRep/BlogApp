using BlogApp.Core.Results;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Domain.Abstractions.Services;

public interface IAuthenticationService
{
    Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default);
    Task<Result> Send2FAOtpAsync(Send2FAOtpArgs args, CancellationToken cancellationToken = default);
    Task<Result<LoginResult>> Verify2FAOtpAsync(Verify2FAArgs args, CancellationToken cancellationToken = default);
    Task<Result> Enable2FAAsync(SendEnable2FAArgs args, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEnable2FAAsync(ConfirmEnable2FAArgs args, CancellationToken cancellationToken = default);
    Task<Result> Disable2FAAsync(CancellationToken cancellationToken = default);
    Task<Result> ConfirmDisable2FAAsync(Verify2FAArgs args, CancellationToken cancellationToken = default);
}