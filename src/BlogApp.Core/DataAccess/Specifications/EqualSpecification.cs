using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Specifications;

public class EqualSpecification<T>(Specification<T> left, Specification<T> right) : BinarySpecification<T>(left, right, Expression.Equal)
{
}