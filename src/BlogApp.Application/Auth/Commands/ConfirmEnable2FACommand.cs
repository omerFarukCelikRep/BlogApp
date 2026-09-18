using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public record ConfirmEnable2FACommand(string Code,string PhoneNumber): ConfirmEnable2FAArgs(Code,PhoneNumber),IRequest<Result>;