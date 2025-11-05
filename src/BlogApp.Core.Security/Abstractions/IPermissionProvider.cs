using BlogApp.Core.Security.Enums;

namespace BlogApp.Core.Security.Abstractions;

public interface IPermissionProvider
{
    IReadOnlyCollection<Permission> GetAllPermissions();
}