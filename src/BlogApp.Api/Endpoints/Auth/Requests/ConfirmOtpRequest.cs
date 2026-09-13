using BlogApp.Application.EmailConfirmations.Commands;

namespace BlogApp.Api.Endpoints.Auth.Requests;

public record ConfirmOtpRequest(string OtpCode)
{
    public static explicit operator ConfirmEmailConfirmationWithOtpCommand(ConfirmOtpRequest request) =>
        new(request.OtpCode);
}