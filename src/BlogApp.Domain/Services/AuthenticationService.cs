using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Utils;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Auth;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Domain.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IJwtProvider jwtProvider)
    : IAuthenticationService
{
    public async Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(x => x.Email.Equals(args.Email, StringComparison.OrdinalIgnoreCase),
            tracking: true, cancellationToken);
        if (user is null)
            return Result<LoginResult>.Failed(null, string.Empty, 401);

        var passwordVerified = PasswordHasher.VerifyPassword(args.Password, user.Password);
        if (!passwordVerified)
            return Result<LoginResult>.Failed(null, string.Empty, 401);

        var token = await jwtProvider.GenerateTokenAsync(user.Id, cancellationToken);
        LoginResult result = new()
        {
            Token = token
        };
        return Result<LoginResult>.Success(result, "Success", 200); //TODO: Resource magic string
    }

    public async Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default)
    {
        var userExist =
            await userRepository.AnyAsync(x => x.Email.ToLower().Equals(args.Email.ToLower()),
                cancellationToken);
        if (userExist)
            return Result.Failed("Email already in user", 400); //TODO : Resource Magic string

        var hashedPassword = PasswordHasher.HashPassword(args.Password);
        var role = await roleRepository.GetAsync(
            x => x.Name.Equals(nameof(Role.Author)), tracking: false,
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
            RoleId = role!.Id
        });

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success("Registered successfully", 200); //TODO:Resource Magic string 
    }
}