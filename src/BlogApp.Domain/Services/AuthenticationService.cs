using BlogApp.Core.Results;
using BlogApp.Core.Security.Utils;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Domain.Services;

public class AuthenticationService(IUserRepository userRepository, IRoleRepository roleRepository)
    : IAuthenticationService
{
    public Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default)
    {
        var userExist =
            await userRepository.AnyAsync(x => x.Email.Equals(args.Email, StringComparison.OrdinalIgnoreCase),
                cancellationToken);
        if (userExist)
            return Result.Failed("Email already in user", 400); //TODO : Magic string

        var hashedPassword = PasswordHasher.HashPassword(args.Password);
        var role = await roleRepository.GetAsync(
            x => x.Name.Equals(nameof(Role.Author), StringComparison.OrdinalIgnoreCase), tracking: false,
            cancellationToken);
        User user = new()
        {
            FirstName = args.FirstName,
            LastName = args.LastName,
            Email = args.Email,
            Username = args.Username,
            Password = hashedPassword,
            EmailConfirmed = false
        };
        user.Roles.Add(new()
        {
            User = user,
            Role = role
        });

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success("Registered successfully", 200);
    }
}