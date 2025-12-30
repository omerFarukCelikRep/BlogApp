using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Specifications;

public abstract class BinarySpecification<T>(Specification<T> left, Specification<T> right, Func<Expression, Expression, BinaryExpression> binaryExpression) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression() => Combine(left, right, binaryExpression);
}
