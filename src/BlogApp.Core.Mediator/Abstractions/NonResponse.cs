namespace BlogApp.Core.Mediator.Abstractions;

public readonly record struct NonResponse
{
    public static readonly NonResponse Value = new();
}