using BlogApp.Domain.Models.Categories;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Categories.Queries;

public record GetTopCategoriesQuery(int Count = 10) : TopCategoryArgs(Count), IRequest<Result<List<CategoryResult>>>;