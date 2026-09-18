namespace BlogApp.Api.Endpoints.Auth.Requests;

public record ConfirmEnable2FARequest(string Code, string PhoneNumber);