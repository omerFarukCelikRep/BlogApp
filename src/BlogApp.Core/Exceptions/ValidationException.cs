using BlogApp.Core.Validations;

namespace BlogApp.Core.Exceptions;

public class ValidationException(List<ValidationError> exceptions) : Exception
{
    public List<ValidationError> PropertyExceptions => exceptions;
}