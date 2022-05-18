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

        // Ack to retrieve Enumerable.Any<>(IEnumerable<>, Func<,>)
        var anyMethod = typeof(Enumerable)
                       .GetMethods()
                       .Single(m => m.Name == "Any" &&
                                    m.GetParameters().Length == 2);

        // {x}
        var xParam = Expression.Parameter(typeof(T), "x");

        // {search}
        var constExpr = Expression.Constant(search);

        // {x.Name.Contains(search)} list
        var xDotNames = entityType.GetProperties()
                                  .Where(p => p.ClrType == typeof(string))
                                  .Select(p => Expression.Property(xParam, p.Name))
                                  .Select(e => (Expression)Expression.Call(e, containsMethod, constExpr));

        // {x.Navigation.Name.Contains(search)} list
        var xDotOtherDotNames = entityType.GetDeclaredNavigations()
                                          .Where(n => !n.IsCollection)
                                          .SelectMany(n => n.TargetEntityType
                                                            .GetProperties()
                                                            .Where(p => p.ClrType == typeof(string))
                                                            .Select(p => NestedProperty(xParam, n.Name, p.Name)))
                                          .Select(e => Expression.Call(e, containsMethod, constExpr));

        // {x.Navigations.Any(n => n.Name.Contains(search))} list
        var xDotOthersDotNames = entityType.GetDeclaredNavigations()
                                           .Where(n => n.IsCollection)
                                           .SelectMany(n =>
                                            {
                                                var nType = n.TargetEntityType.ClrType;

                                                // Enumerable.Any<NType>
                                                var genericAnyMethod = anyMethod.MakeGenericMethod(nType);

                                                // {n}
                                                var nParam = Expression.Parameter(nType, "n");

                                                // {x.Navigations}
                                                var xDotNavigations = Expression.Property(xParam, n.Name);

                                                return n.TargetEntityType
                                                        .GetProperties()
                                                        .Where(p => p.ClrType == typeof(string))
                                                        .Select(p =>
                                                         {
                                                             // {n.Name}
                                                             var nDotName = Expression.Property(nParam, p.Name);

                                                             // {n.Name.Contains(search)}
                                                             var contains = Expression.Call(nDotName, containsMethod, constExpr);

                                                             // {n => n.Name.Contains(search)}
                                                             var lambda = Expression.Lambda(contains, nParam);

                                                             // {Enumerable.Any(x.Navigations, n => n.Name.Contains(search))
                                                             return Expression.Call(null, genericAnyMethod, xDotNavigations, lambda);
                                                         });
                                            })
                                           .ToList();

        // { || ... }
        var orExpression = xDotNames.Concat(xDotOtherDotNames)
                                    .Concat(xDotOthersDotNames)
                                    .Aggregate(Expression.Or);

        var lambda = Expression.Lambda<Func<T, bool>>(orExpression, xParam);

        return queryable.Where(lambda);
    }

    private static Expression NestedProperty(Expression expression, params string[] propertyNames)
    {
        return propertyNames.Aggregate(expression, Expression.PropertyOrField);
    }
}