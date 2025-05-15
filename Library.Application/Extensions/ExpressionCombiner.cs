using System.Linq.Expressions;

namespace Library.Application.Extensions;

public static class ExpressionCombiner
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T));

        var leftBody = Expression.Invoke(left, param);
        var rightBody = Expression.Invoke(right, param);

        var body = Expression.AndAlso(leftBody, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}