namespace BlogApp.Domain.Models.Blogs;

public record RandomBlogArgs(int Count = 6, List<int>? Categories = null);