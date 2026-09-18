using BlogApp.Core.Security.Attributes;
using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

[Authorize]
public record SendEnable2FACommand(string PhoneNumber) : SendEnable2FAArgs(PhoneNumber), IRequest<Result>;