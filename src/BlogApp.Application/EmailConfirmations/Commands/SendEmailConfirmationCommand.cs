using BlogApp.Domain.Models.EmailConfirmations;

namespace BlogApp.Application.EmailConfirmations.Commands;

public record SendEmailConfirmationCommand() : IRequest<Result>;