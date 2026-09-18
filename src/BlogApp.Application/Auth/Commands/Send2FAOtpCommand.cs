using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record Send2FAOtpCommand(Guid? UserId = null, TwoFactorPurpose Purpose = TwoFactorPurpose.Login)
    : Send2FAOtpArgs(UserId, Purpose), IRequest<Result>;