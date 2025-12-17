using BlogApp.Core.Validations.Results;

namespace BlogApp.Core.Validations.Exceptions;

public class ValidationException(List<ValidationError> exceptions) : Exception
{
    public List<ValidationError> PropertyExceptions => exceptions;
}