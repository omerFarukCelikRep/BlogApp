namespace BlogApp.Core.Security.Abstractions;

public interface IDomainPrincipal
{
    public Guid UserId { get; }
    bool IsAuthenticated { get; }
}