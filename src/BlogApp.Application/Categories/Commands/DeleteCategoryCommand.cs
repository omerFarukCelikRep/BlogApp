using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Commands;

[Authorize(Role.Admin)]
public record DeleteCategoryCommand(int CategoryId) : DeleteCategoryArgs(CategoryId), IRequest<Result>;