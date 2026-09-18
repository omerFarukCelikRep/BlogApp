using System.Security.Cryptography;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Models;
using BlogApp.Core.Security.Options;
using BlogApp.Core.Security.Utils;
using BlogApp.Core.Sms.Abstractions;
using BlogApp.Core.Telemetry.Abstractions;
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
    ITwoFactorCodeRepository twoFactorCodeRepository,
    IJwtProvider jwtProvider,
    IRefreshTokenProvider refreshTokenProvider,
    ITelemetryService telemetryService,
    ISmsService smsService,
    IDomainPrincipal domainPrincipal,
    IOptions<LoginOptions> loginOptions,
    IOptions<JwtOptions> jwtOptions)
    : IAuthenticationService
{
    private readonly LoginOptions _loginOptions = loginOptions.Value;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    private static string GenerateOtpCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();

    public async Task<Result<LoginResult>> LoginAsync(LoginArgs args, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(x => x.Email.Equals(args.Email),
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
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(_loginOptions.FailLimit);

            await userRepository.SaveChangesAsync(cancellationToken);

            telemetryService.RecordLoginFailure(args.Email, "invalid_credentials");

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
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            Roles = [.. user.UserRoles.Select(x => x.Role!.Name)],
            Permissions =
                [.. user.UserRoles.SelectMany(x => x.Role!.RolePermissions.Select(p => p.Permission!.ToString()))]
        };
        var token = await jwtProvider.GenerateTokenAsync(tokenArgs, cancellationToken);
        var refreshToken = await refreshTokenProvider.GenerateAsync(user.Id, cancellationToken);

        await userRepository.SaveChangesAsync(cancellationToken);

        telemetryService.RecordLoginSuccess(args.Email);

        LoginResult result = new(token, refreshToken, DateTime.Now.AddMinutes(_jwtOptions.ExpirationMinutes));
        return Result<LoginResult>.Success(data: result);
    }

    public async Task<Result> RegisterAsync(RegisterArgs args, CancellationToken cancellationToken = default)
    {
        var userExist =
            await userRepository.AnyAsync(x => x.Email.ToLower().Equals(args.Email.ToLower()),
                cancellationToken);
        if (userExist)
            return Result<Guid>.Failed(400, Errors.Auth.EmailAlreadyExists);

        var role = await roleRepository.GetAsync(
            x => x.Name.Equals(nameof(Role.Author)), tracking: false,
            cancellationToken);
        if (role is null)
            return Result<Guid>.Failed(400, Error.Create(Errors.Role.NotFound));

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

        telemetryService.RecordRegister();

        return Result<Guid>.Success(201, data: user.Id);
    }

    public async Task<Result> Send2FAOtpAsync(Send2FAOtpArgs args, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(args.UserId ?? domainPrincipal.UserId, false, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        if (string.IsNullOrEmpty(user.PhoneNumber))
            return Result.Failed(400, Error.Create(Errors.User.PhoneNumberNotFound));

        await twoFactorCodeRepository.RevokeAllAsync(user.Id, args.Purpose, cancellationToken);

        var code = GenerateOtpCode();

        var twoFactorCode = new TwoFactorCode()
        {
            UserId = user.Id,
            Code = code,
            Phone = user.PhoneNumber,
            ExpireDate = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            Purpose = args.Purpose
        };

        await twoFactorCodeRepository.AddAsync(twoFactorCode, cancellationToken);
        await twoFactorCodeRepository.SaveChangesAsync(cancellationToken);

        var message = $"Your devlog verification code: {code}. Valid for 5 minutes.";
        await smsService.SendAsync(user.PhoneNumber, message, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<LoginResult>> Verify2FAOtpAsync(Verify2FAArgs args,
        CancellationToken cancellationToken = default)
    {
        if (domainPrincipal.Scope != TokenScope.TwoFactorChallenge)
            return Result<LoginResult>.Failed(403,
                Error.Create(Errors.TwoFactorCode.InvalidScope));

        var code = await twoFactorCodeRepository.GetAsync(x => x.Code == args.Code
                                                               && x.UserId == domainPrincipal.UserId
                                                               && x.Purpose == TwoFactorPurpose.Login
                                                               && x.ExpireDate > DateTime.UtcNow
                                                               && !x.IsUsed,
            true, cancellationToken);
        if (code is null)
            return Result<LoginResult>.Failed(404, Error.Create(Errors.TwoFactorCode.InvalidCode));

        code.IsUsed = true;

        var user = await userRepository.GetByIdAsync(domainPrincipal.UserId, false, cancellationToken);
        if (user is null)
            return Result<LoginResult>.Failed(404, Error.Create(Errors.User.NotFound));

        var tokenArgs = new TokenArgs()
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            EmailConfirmed = user.EmailConfirmed,
            Scope = TokenScope.FullAccess,
            TwoFactorEnabled = user.TwoFactorEnabled,
            Roles = [.. user.UserRoles.Select(r => r.Role!.Name)],
            Permissions =
                [.. user.UserRoles.SelectMany(x => x.Role!.RolePermissions).Select(x => x.Permission!.ToString())]
        };

        var accessToken = await jwtProvider.GenerateTokenAsync(tokenArgs, cancellationToken);
        var refreshToken = await refreshTokenProvider.GenerateAsync(user.Id, cancellationToken);

        return Result<LoginResult>.Success(data: new(
            Token: accessToken,
            RefreshToken: refreshToken,
            ExpireDate: DateTime.UtcNow.AddHours(1),
            Scope: TokenScope.FullAccess));
    }

    public async Task<Result> Enable2FAAsync(SendEnable2FAArgs args, CancellationToken cancellationToken = default)
    {
        var userId = domainPrincipal.UserId;
        await twoFactorCodeRepository.RevokeAllAsync(userId, TwoFactorPurpose.Enable,
            cancellationToken);

        var code = GenerateOtpCode();
        await twoFactorCodeRepository.AddAsync(new TwoFactorCode()
        {
            UserId = userId,
            Code = code,
            Phone = args.PhoneNumber,
            ExpireDate = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            Purpose = TwoFactorPurpose.Enable
        }, cancellationToken);

        await twoFactorCodeRepository.SaveChangesAsync(cancellationToken);

        await smsService.SendAsync(
            args.PhoneNumber,
            $"Your devlog 2FA setup code: {code}. Valid for 10 minutes.",
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ConfirmEnable2FAAsync(ConfirmEnable2FAArgs args,
        CancellationToken cancellationToken = default)
    {
        var userId = domainPrincipal.UserId;
        var code = await twoFactorCodeRepository.GetAsync(x => x.UserId == userId
                                                               && x.Code == args.Code
                                                               && x.Phone == args.PhoneNumber
                                                               && x.Purpose == TwoFactorPurpose.Enable
                                                               && x.ExpireDate > DateTime.UtcNow
                                                               && !x.IsUsed, true, cancellationToken);
        if (code is null)
            return Result.Failed(400, Error.Create(Errors.TwoFactorCode.InvalidCode));

        code.IsUsed = true;

        var user = await userRepository.GetByIdAsync(userId, true, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        user.EnableTwoFactorAuthentication(args.PhoneNumber);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Disable2FAAsync(CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(domainPrincipal.UserId, true, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        if (!user.TwoFactorEnabled)
            return Result.Failed(400, Error.Create(Errors.TwoFactorCode.NotEnabled));

        await twoFactorCodeRepository.RevokeAllAsync(user.Id, TwoFactorPurpose.Disable, cancellationToken);

        var code = GenerateOtpCode();
        await twoFactorCodeRepository.AddAsync(new TwoFactorCode()
        {
            UserId = user.Id,
            Code = code,
            Phone = user.PhoneNumber!,
            ExpireDate = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            Purpose = TwoFactorPurpose.Disable
        }, cancellationToken);
        await twoFactorCodeRepository.SaveChangesAsync(cancellationToken);

        await smsService.SendAsync(
            user.PhoneNumber!,
            $"Your DevLog 2FA disable code: {code}. Valid for 10 minutes.",
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ConfirmDisable2FAAsync(Verify2FAArgs args, CancellationToken cancellationToken = default)
    {
        var userId = domainPrincipal.UserId;
        var code = await twoFactorCodeRepository.GetAsync(x => x.UserId == userId
                                                               && x.Code == args.Code
                                                               && x.Purpose == TwoFactorPurpose.Disable
                                                               && x.ExpireDate > DateTime.UtcNow
                                                               && !x.IsUsed, true, cancellationToken);
        if (code is null)
            return Result.Failed(400, Error.Create(Errors.TwoFactorCode.InvalidCode));

        code.IsUsed = true;

        var user = await userRepository.GetByIdAsync(userId, true, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        user.DisableTwoFactorAuthentication();
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}