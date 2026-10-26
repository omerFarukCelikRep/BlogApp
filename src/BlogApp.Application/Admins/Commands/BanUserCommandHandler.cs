using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Admins.Commands;

public class BanUserCommandHandler(IAdminService adminService) : IRequestHandler<BanUserCommand,Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken = default)
    {
        return await adminService.BanUserAsync(request, cancellationToken);
    }
}