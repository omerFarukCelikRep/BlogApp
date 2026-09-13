using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.EmailConfirmations;

namespace BlogApp.Application.EmailConfirmations.Commands;

[Authorize]
public record ConfirmEmailConfirmationWithOtpCommand(string OtpCode)
    : ConfirmEmailConfirmationWithOtpArgs(OtpCode), IRequest<Result>;