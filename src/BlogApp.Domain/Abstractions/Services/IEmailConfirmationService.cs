using BlogApp.Core.Results;
using BlogApp.Domain.Models.EmailConfirmations;

namespace BlogApp.Domain.Abstractions.Services;

public interface IEmailConfirmationService
{
    Task<Result> SendConfirmationAsync(CancellationToken cancellationToken = default);
    Task<Result> ConfirmAsync(ConfirmEmailConfirmationArgs args, CancellationToken cancellationToken = default);

    Task<Result> ConfirmWithOtpAsync(ConfirmEmailConfirmationWithOtpArgs args,
        CancellationToken cancellationToken = default);
}