using BlogApp.Domain.Models.Auth;

namespace BlogApp.Application.Auth.Commands;

public record ForgotPasswordCommand(string Email) : ForgotPasswordArgs(Email), IRequest<Result>;