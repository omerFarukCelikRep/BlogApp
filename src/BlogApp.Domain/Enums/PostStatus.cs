namespace BlogApp.Domain.Enums;

[Flags]
public enum PostStatus
{
    Draft = 1,
    Published = 2,
    Deleted = 4
}