using BlogApp.Core.DataAccess.Extensions;
using System.Linq.Expressions;

namespace BlogApp.Core.DataAccess.Specifications;

public abstract class Specification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    private readonly List<Expression<Func<T, object>>> _includes = [];
    public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDesc { get; private set; }

    protected Expression<Func<T, bool>> AddCriteria(Expression<Func<T, bool>> criteria, Func<Expression, Expression, BinaryExpression>? binaryExpression = null)
    {
        Criteria = Criteria is null
            ? criteria
            : Criteria.Combine(criteria, binaryExpression ?? Expression.AndAlso);

        return Criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> include) => _includes.Add(include);

    protected void AddOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDesc) => OrderByDesc = orderByDesc;

    protected static Expression<Func<T, bool>> Combine(Specification<T> left, Specification<T> right, Func<Expression, Expression, BinaryExpression> binaryExpression) => left.ToExpression().Combine(right.ToExpression(), binaryExpression);

    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);

    public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);
    public Expression<Func<T, bool>> And(Expression<Func<T, bool>> criteria) => AddCriteria(criteria, Expression.AndAlso);

    public Specification<T> Or(Specification<T> specification) => new OrSpecification<T>(this, specification);
    public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> criteria) => AddCriteria(criteria, Expression.OrElse);

    public Specification<T> NotEqual(Specification<T> specification) => new NotEqualSpecification<T>(this, specification);
    public Expression<Func<T, bool>> NotEqual(Expression<Func<T, bool>> criteria) => AddCriteria(criteria, Expression.NotEqual);

    public Specification<T> Equal(Specification<T> specification) => new EqualSpecification<T>(this, specification);
    public Expression<Func<T, bool>> Equal(Expression<Func<T, bool>> criteria) => AddCriteria(criteria, Expression.Equal);
}

public abstract class Specification<T, TResult> : Specification<T>
{
    public Expression<Func<T, int, TResult>> Selector { get; private set; } = null!;

    public void Select(Expression<Func<T, int, TResult>> selector) => Selector = selector;
}