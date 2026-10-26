namespace BlogApp.Domain.Models.Categories;

public record CreateCategoryArgs(string Name, int? ParentId = null);