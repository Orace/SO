using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SO_72276645;

public static class DbSetExtension
{
    public static IQueryable<T> DeepSearch<T>(this DbSet<T> dbSet, string search)
        where T : class
    {
        return DeepSearch(dbSet, dbSet, search);
    }

    public static IQueryable<T> DeepSearch<T>(this IQueryable<T> queryable, DbContext dbContext, string search)
        where T : class
    {
        var set = dbContext.Set<T>();
        return DeepSearch(queryable, set, search);
    }

    public static IQueryable<T> DeepSearch<T>(this IQueryable<T> queryable, DbSet<T> originalSet, string search)
        where T : class
    {
        var entityType = originalSet.EntityType;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        // {x}
        var xParam = Expression.Parameter(typeof(T), "x");

        // {search}
        var constExpr = Expression.Constant(search);

        // {x.Name} list
        var xDotNames = entityType.GetProperties()
                                  .Where(p => p.ClrType == typeof(string))
                                  .Select(p => Expression.Property(xParam, p.Name));

        // {x.Navigation.Name} list
        var xDotOtherDotNames = entityType.GetDeclaredNavigations()
                                          .Where(n => !n.IsCollection)
                                          .SelectMany(n => n.TargetEntityType
                                                            .GetProperties()
                                                            .Where(p => p.ClrType == typeof(string))
                                                            .Select(p => NestedProperty(xParam, n.Name, p.Name)));

        // { || x.{}.Name.Contains(search) }
        var orExpression = xDotNames.Concat(xDotOtherDotNames)
                                    .Select(e => (Expression)Expression.Call(e, containsMethod, constExpr))
                                    .Aggregate(Expression.OrElse);

        var lambda = Expression.Lambda<Func<T, bool>>(orExpression, xParam);

        return queryable.Where(lambda);
    }

    private static Expression NestedProperty(Expression expression, params string[] propertyNames)
    {
        return propertyNames.Aggregate(expression, Expression.PropertyOrField);
    }
}