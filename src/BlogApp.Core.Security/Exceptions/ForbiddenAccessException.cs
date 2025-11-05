namespace BlogApp.Core.Security.Exceptions;

public sealed class ForbiddenAccessException(string message)
    : Exception(message);