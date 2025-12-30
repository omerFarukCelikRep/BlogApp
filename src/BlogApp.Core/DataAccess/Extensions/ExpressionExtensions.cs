using BlogApp.Core.DataAccess.Specifications;
using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Extensions;

public static class ExpressionExtensions
{
    extension<T>(Expression<Func<T, bool>> expression)
    {
        public Expression<Func<T, bool>> Combine(Expression<Func<T, bool>> rightExpression, Func<Expression, Expression, BinaryExpression> binaryExpression)
        {
            var parameter = expression.Parameters[0];

            var rightBody = new ParameterReplacer(rightExpression.Parameters[0], parameter).Visit(rightExpression.Body);
            var body = binaryExpression(expression.Body, rightBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
