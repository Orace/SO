using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SO_72276645;

public static class DbSetExtension
{
    public static IQueryable<T> DeepSearch<T>(this DbSet<T> dbSet, string search)
        where T : class
    {
        return DeepSearch<T, string>(dbSet, dbSet, s => s.Contains(search));
    }

    public static IQueryable<T> DeepSearch<T, TMember>(this DbSet<T> dbSet,
                                                       Expression<Func<TMember, bool>> predicate)
        where T : class
    {
        return DeepSearch(dbSet, dbSet, predicate);
    }

    public static IQueryable<T> DeepSearch<T>(this IQueryable<T> queryable, DbContext dbContext, string search)
        where T : class
    {
        var set = dbContext.Set<T>();
        return DeepSearch<T, string>(queryable, set, s => s.Contains(search));
    }

    public static IQueryable<T> DeepSearch<T, TMember>(this IQueryable<T> queryable,
                                                       DbSet<T> originalSet,
                                                       Expression<Func<TMember, bool>> predicate)
        where T : class
    {
        var entityType = originalSet.EntityType;

        // Ack to retrieve Enumerable.Any<>(IEnumerable<>, Func<,>)
        var anyMethod = typeof(Enumerable)
                       .GetMethods()
                       .Single(m => m.Name == "Any" &&
                                    m.GetParameters().Length == 2);

        // {x}
        var xParam = Expression.Parameter(typeof(T), "x");

        // {predicate(x.Name)} list
        var xDotNames = entityType.GetProperties()
                                  .Where(p => p.ClrType == typeof(TMember))
                                  .Select(p => Expression.Property(xParam, p.Name))
                                  .Select(e => GetModifyedPredicateBody(predicate, e));

        // {predicate(x.Navigation.Name)} list
        var xDotOtherDotNames = entityType.GetDeclaredNavigations()
                                          .Where(n => !n.IsCollection)
                                          .SelectMany(n => n.TargetEntityType
                                                            .GetProperties()
                                                            .Where(p => p.ClrType == typeof(TMember))
                                                            .Select(p => NestedProperty(xParam, n.Name, p.Name)))
                                          .Select(e => GetModifyedPredicateBody(predicate, e));

        // {x.Navigations.Any(n => predicate(n.Name))} list
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
                                                        .Where(p => p.ClrType == typeof(TMember))
                                                        .Select(p =>
                                                         {
                                                             // {n.Name}
                                                             var nDotName = Expression.Property(nParam, p.Name);

                                                             // {n => predicate(n.Name)}
                                                             var lambda = Expression.Lambda(GetModifyedPredicateBody(predicate, nDotName), nParam);

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

    private static Expression GetModifyedPredicateBody<TMember>(Expression<Func<TMember, bool>> predicate,
                                                                Expression extendedParam)
    {
        var result = ParameterReplacer.Replace(predicate, predicate.Parameters[0], extendedParam);
        return result.Body;
    }

    private static Expression NestedProperty(Expression expression, params string[] propertyNames)
    {
        return propertyNames.Aggregate(expression, Expression.PropertyOrField);
    }
}