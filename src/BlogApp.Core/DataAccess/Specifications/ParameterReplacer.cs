using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Specifications;

public class ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) => node == oldParameter
        ? newParameter
        : base.VisitParameter(node);
}
