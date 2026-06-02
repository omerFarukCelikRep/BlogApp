using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Models;
using BlogApp.Core.Security.Options;
using BlogApp.Core.Security.Utils;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Auth;
using BlogApp.Domain.Options;
using Microsoft.Extensions.Options;
using Role = BlogApp.Core.Security.Enums.Role;

namespace BlogApp.Domain.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IJwtProvider jwtProvider,
    IRefreshTokenProvider refreshTokenProvider,
    IOptions<LoginOptions> loginOptions,
    IOptions<JwtOptions> jwtOptions)
    : IAuthenticationService
{
    private readonly LoginOptions _loginOptions = loginOptions.Value;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(x => x.Email.Equals(args.Email, StringComparison.OrdinalIgnoreCase),
            tracking: true, cancellationToken);
        if (user is null)
            return Result<LoginResult>.Failed(401, Errors.Auth.LoginFailed);

        if (user.IsLockedOut())
            return Result<LoginResult>.Failed(401, Errors.Auth.AccountLocked);

        var passwordVerified = PasswordHasher.VerifyPassword(args.Password, user.Password);
        if (!passwordVerified)
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= _loginOptions.FailLimit)
                user.LockoutEnd = DateTimeOffset.Now.AddMinutes(_loginOptions.FailLimit);

            await userRepository.SaveChangesAsync(cancellationToken);
            return Result<LoginResult>.Failed(401, Errors.Auth.InvalidCredentials);
        }

        user.AccessFailedCount = 0;
        user.LockoutEnd = null;

        var tokenArgs = new TokenArgs()
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            Roles = [..user.UserRoles.Select(x => x.Role!.Name)],
            Permissions =
                [..user.UserRoles.SelectMany(x => x.Role!.RolePermissions.Select(p => p.Permission!.ToString()))]
        };
        var token = await jwtProvider.GenerateTokenAsync(tokenArgs, cancellationToken);
        var refreshToken = await refreshTokenProvider.GenerateAsync(user.Id, cancellationToken);

        await userRepository.SaveChangesAsync(cancellationToken);
        LoginResult result = new(token, refreshToken, DateTimeOffset.Now.AddMinutes(_jwtOptions.ExpirationMinutes));
        return Result<LoginResult>.Success(result);
    }

    public async Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default)
    {
        var userExist =
            await userRepository.AnyAsync(x => x.Email.ToLower().Equals(args.Email.ToLower()),
                cancellationToken);
        if (userExist)
            return Result.Failed(Errors.Auth.EmailAlreadyExist, 400);

        var role = await roleRepository.GetAsync(
            x => x.Name.Equals(nameof(Role.Author)), tracking: false,
            cancellationToken);
        if (role is null)
            return Result.Failed(Errors.Role.NotFound, 400);

        var hashedPassword = PasswordHasher.HashPassword(args.Password);
        User user = new()
        {
            FirstName = args.FirstName,
            LastName = args.LastName,
            Email = args.Email,
            Username = args.Username,
            Password = hashedPassword,
            EmailConfirmed = false
        };
        user.UserRoles.Add(new()
        {
            User = user,
            RoleId = role.Id
        });

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}