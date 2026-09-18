using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Email.Abstractions;
using BlogApp.Core.Email.Models;
using BlogApp.Core.Email.Options;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Utils;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Auth;
using Microsoft.Extensions.Options;

namespace BlogApp.Domain.Services;

public class PasswordResetService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IEmailTemplateService emailTemplateService,
    IEmailService emailService,
    IOptions<EmailOptions> emailOptions) : IPasswordResetService
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public async Task<Result> CreateForgotPasswordTokenAsync(ForgotPasswordArgs args,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(x => x.Email.Equals(args.Email), true, cancellationToken);
        if (user is null)
            return Result.Success(statusCode: 200);

        await passwordResetTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var otpCode = RandomNumberGenerator
            .GetInt32(100000, 999999)
            .ToString();

        await passwordResetTokenRepository.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            Token = tokenHash,
            OtpCode = otpCode,
            ExpireDate = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false,
        }, cancellationToken);

        await passwordResetTokenRepository.SaveChangesAsync(cancellationToken);

        var resetUrl = $"{_emailOptions.BaseUrl}/reset-password?token={rawToken}";
        var emailArgs = new Dictionary<string, string>()
        {
            ["FIRST_NAME"] = user.FirstName,
            ["RESET_URL"] = resetUrl,
            ["OTP_CODE"] = otpCode,
            ["EXPIRY_MINUTES"] = "5"
        };
        var html = emailTemplateService.Build(emailArgs, nameof(EmailTemplates.PasswordReset));
        const string subject = "Reset your password — DevLog";

        await emailService.SendAsync(new EmailMessage(
                To: user.Email,
                Subject: subject,
                HtmlBody: html,
                IsBodyHtml: true,
                PlainTextBody: $"Reset code: {otpCode} | Link: {resetUrl}"),
            cancellationToken);

        return Result.Success(statusCode: 200);
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordArgs args, CancellationToken cancellationToken = default)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(args.Token)));

        var resetToken =
            await passwordResetTokenRepository.GetAsync(x => x.Token == tokenHash, true, cancellationToken);
        if (resetToken is null || !resetToken.IsValid())
            return Result.Failed(400, Error.Create(Errors.PasswordResetToken.InvalidToken));

        resetToken.IsUsed = true;
        resetToken.User.Password = PasswordHasher.HashPassword(args.NewPassword);

        await passwordResetTokenRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(statusCode: 200);
    }

    public async Task<Result> ResetPasswordWithOtpAsync(ResetPasswordWithOtpArgs args,
        CancellationToken cancellationToken = default)
    {
        var resetToken =
            await passwordResetTokenRepository.GetAsync(x => x.User.Email == args.Email && x.OtpCode == args.OtpCode,
                true, cancellationToken);
        if (resetToken is null || !resetToken.IsValid())
            return Result.Failed(400, Error.Create(Errors.PasswordResetToken.InvalidOtpCode));

        resetToken.IsUsed = true;
        resetToken.User.Password = PasswordHasher.HashPassword(args.NewPassword);

        await passwordResetTokenRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(statusCode: 200);
    }
}