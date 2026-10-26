using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Commands;

[Authorize(Role.Admin)]
public record CreateTagCommand(string Name) : CreateTagArgs(Name), IRequest<Result<TagResult>>;