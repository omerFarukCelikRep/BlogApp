using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record ConfirmDisable2FACommand(string Code) : Verify2FAArgs(Code), IRequest<Result>;