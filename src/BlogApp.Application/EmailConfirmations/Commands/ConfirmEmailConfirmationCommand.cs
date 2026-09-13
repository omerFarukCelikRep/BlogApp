using BlogApp.Domain.Models.EmailConfirmations;

namespace BlogApp.Application.EmailConfirmations.Commands;

public record ConfirmEmailConfirmationCommand(string Token) : ConfirmEmailConfirmationArgs(Token), IRequest<Result>;