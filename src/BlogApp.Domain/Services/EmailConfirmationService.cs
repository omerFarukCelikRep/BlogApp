using System.Security.Cryptography;
using System.Text;
using BlogApp.Core.Email.Abstractions;
using BlogApp.Core.Email.Models;
using BlogApp.Core.Email.Options;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.EmailConfirmations;
using Microsoft.Extensions.Options;

namespace BlogApp.Domain.Services;

public class EmailConfirmationService(
    IEmailService emailService,
    IEmailTemplateService emailTemplateService,
    IEmailConfirmationRepository emailConfirmationRepository,
    IUserRepository userRepository,
    IOptions<EmailOptions> options,
    IDomainPrincipal domainPrincipal) : IEmailConfirmationService
{
    private readonly EmailOptions _options = options.Value;

    private async Task<Result> ConfirmAsync(EmailConfirmation? emailConfirmation,
        CancellationToken cancellationToken = default)
    {
        if (emailConfirmation is null || !emailConfirmation.IsValid())
            return Result.Failed(400, Error.Create(Errors.EmailConfirmation.InvalidConfirmationToken));


        var user = await userRepository.GetByIdAsync(emailConfirmation.UserId, cancellationToken: cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        if (user.EmailConfirmed)
            return Result.Failed(400, Error.Create(Errors.User.EmailAlreadyConfirmed));

        emailConfirmation.IsUsed = true;
        user.ConfirmEmail();

        await emailConfirmationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SendConfirmationAsync(CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(domainPrincipal.UserId, false, cancellationToken);
        if (user is null)
            return Result.Failed(404, Error.Create(Errors.User.NotFound));

        if (user.EmailConfirmed)
            return Result.Failed(400, Error.Create(Errors.User.EmailAlreadyConfirmed));

        await emailConfirmationRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-")
            .Replace("/", "-")
            .TrimEnd("=")
            .ToString();

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var emailConfirmation = new EmailConfirmation()
        {
            UserId = user.Id,
            Token = tokenHash,
            OtpCode = otpCode,
            ExpireDate = DateTime.UtcNow.AddMinutes(30),
            IsUsed = false
        };

        await emailConfirmationRepository.AddAsync(emailConfirmation, cancellationToken);
        await emailConfirmationRepository.SaveChangesAsync(cancellationToken);

        var confirmUrl = $"{_options.BaseUrl}/confirm-email?token={rawToken}";
        var html = emailTemplateService.Build(user.FirstName, confirmUrl, otpCode);
        const string subject = "Confirm your email — DevLog";

        await emailService.SendAsync(new EmailMessage(
            To: user.Email,
            Subject: subject,
            html,
            IsBodyHtml: true,
            PlainTextBody: $"Your OTP: {otpCode}  |  Link: {confirmUrl}"), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ConfirmAsync(ConfirmEmailConfirmationArgs args,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(args.Token)));

        var confirmation =
            await emailConfirmationRepository.GetAsync(x => x.Token.Equals(tokenHash),
                cancellationToken: cancellationToken);

        return await ConfirmAsync(confirmation, cancellationToken);
    }

    public async Task<Result> ConfirmWithOtpAsync(ConfirmEmailConfirmationWithOtpArgs args,
        CancellationToken cancellationToken = default)
    {
        var confirmation =
            await emailConfirmationRepository.GetAsync(x => x.OtpCode.Equals(args.OtpCode),
                cancellationToken: cancellationToken);

        return await ConfirmAsync(confirmation, cancellationToken);
    }
}