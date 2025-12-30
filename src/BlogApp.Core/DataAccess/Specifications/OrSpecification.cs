using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Specifications;

public class OrSpecification<T>(Specification<T> left, Specification<T> right) : BinarySpecification<T>(left, right, Expression.OrElse)
{
}