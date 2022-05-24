using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SO_72276645;

public static class DbSetExtension
{
    private const string SourceName = "$source";

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

    /// <summary>
    /// Perform:
    /// q.Select(s => new { $source = s, entity_1 = s.entity_1, ... })                                                     // select the entity and it's related entities
    ///  .Where(w => ... || predicate(w.entity.property) || ... || w.entityOfMany.Any(a => ... || predicate(a.property)))  // apply the predicate on all possible properties
    ///  .Select(e => e.$source);                                                                                          // extract the root entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMember"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="originalSet"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IQueryable<T> DeepSearch<T, TMember>(this IQueryable<T> queryable,
                                                       DbSet<T> originalSet,
                                                       Expression<Func<TMember, bool>> predicate)
        where T : class
    {
        // Ack to retrieve Enumerable.Any<>(IEnumerable<>, Func<,>)
        var anyMethod = typeof(Enumerable)
                       .GetMethods()
                       .Single(m => m.Name == "Any" &&
                                    m.GetParameters().Length == 2);

        var entityType = originalSet.EntityType;

        // {s}
        var selectParameter = Expression.Parameter(typeof(T), "s");

        // Selected entities informations.
        var entityDatas = entityType.GetDeclaredNavigations()
                                    .Select(n => new EntityData(Expression.Property(selectParameter, n.Name),
                                                                n.IsCollection,
                                                                n.Name,
                                                                n.ClrType,
                                                                n.TargetEntityType))
                                    .ToList();

        // Add the original entity in selected entities informations.
        entityDatas.Add(new EntityData(selectParameter,
                                       false,
                                       SourceName,
                                       entityType.ClrType,
                                       entityType));

        // Fields names and types of the `Anonymous type` used in the Select
        var selectOutputTypeFields = entityDatas.ToDictionary(o => o.Name, o => o.Type);

        // Actual `Anonymous type`
        var selectOutputType = LinqRuntimeTypeBuilder.GetDynamicType(selectOutputTypeFields);
        var selectOutputTypeConstructor = selectOutputType.GetConstructors().Single();

        // List of {entity = s.entity} and {$source = s} expressions
        var entityBindings = entityDatas.Select(n =>
                                         {
                                             var memberInfo = selectOutputType.GetMember(n.Name).First();
                                             return Expression.Bind(memberInfo, n.AccessExpression);
                                         })
                                        .ToList();

        // new <?> {entity = s.entity, ..., $source = s}
        var selectBody = Expression.MemberInit(Expression.New(selectOutputTypeConstructor), entityBindings);


        // {w}
        var whereParameter = Expression.Parameter(selectOutputType, "w");

        // {( ... || predicate(w.entity.property)}
        // {w.entityOfMany.Any(a => ... || predicate(a.property)}
        var constraints = entityDatas.SelectMany(n =>
        {
            var properties = n.EntityType
                              .GetProperties()
                              .Where(p => !p.IsKey() && p.ClrType == typeof(TMember));

            // {w.entity}
            var wDotEntity = Expression.Field(whereParameter, n.Name);

            if (!n.IsCollection)
            {
                return properties.Select(p =>
                {
                    // {w.entity.property}
                    var wDotEntityDotProperty = Expression.Property(wDotEntity, p.Name);

                    // {predicate(w.entity.property)}
                    return GetModifyedPredicateBody(predicate, wDotEntityDotProperty);
                });
            }

            var nType = n.EntityType.ClrType;

            // Enumerable.Any<NType>
            var genericAnyMethod = anyMethod.MakeGenericMethod(nType);

            // {a}
            var anyParameter = Expression.Parameter(nType, "a");

            // { ... || predicate(a.property) }
            var lambdaBody = properties.Select(p =>
                                        {
                                            // {a.property}
                                            var aDotName = Expression.Property(anyParameter, p.Name);

                                            // {predicate(a.property)}
                                            return GetModifyedPredicateBody(predicate, aDotName);
                                        })
                                       .Aggregate(Expression.OrElse);

            // { a => ... || predicate(a.property) }
            var lambda = Expression.Lambda(lambdaBody, anyParameter);

            // {w.entityOfMany.Any(a => ... || predicate(a.property))}
            return new[]
            {
                Expression.Call(null,
                                genericAnyMethod,
                                wDotEntity,
                                lambda)
            };
        });

        // { ... || predicate(w.entity.property) || ... || w.entityOfMany.Any(a => ... || predicate(a.property) }
        var whereBody = constraints.Aggregate(Expression.OrElse);

        // {e}
        var extractParameter = Expression.Parameter(selectOutputType, "e");

        // { e => e.$source}
        var extractBody = Expression.Field(extractParameter, SourceName);

        var h = Activator.CreateInstance(typeof(SelectWhereHelper<,>).MakeGenericType(typeof(T), selectOutputType));
        if (h is not ISelectWhereHelper<T> helper)
        {
            throw new Exception("oups");
        }

        return helper.SelectWhereSelect(queryable,
                                        selectBody, selectParameter,
                                        whereBody, whereParameter,
                                        extractBody, extractParameter);
    }

    private static Expression GetModifyedPredicateBody<TMember>(Expression<Func<TMember, bool>> predicate,
                                                                Expression extendedParam)
    {
        var result = ParameterReplacer.Replace(predicate, predicate.Parameters[0], extendedParam);
        return result.Body;
    }

    private interface ISelectWhereHelper<TInput>
    {
        IQueryable<TInput> SelectWhereSelect(IQueryable<TInput> source,
                                             Expression selectBody, ParameterExpression selectParameter,
                                             Expression whereBody, ParameterExpression whereParameter,
                                             Expression extractBody, ParameterExpression extractParameter);
    }

    private class SelectWhereHelper<TInput, TSelect> : ISelectWhereHelper<TInput>
    {
        /// <summary>
        /// Helper to write this :
        /// q.Select(s => new { $source = s, entity_1 = s.entity_1, ... })                                                     // select the entity and it's related entities
        ///  .Where(w => ... || predicate(w.entity.property) || ... || w.entityOfMany.Any(a => ... || predicate(a.property)))  // apply the predicate on all possible properties
        ///  .Select(e => e.$source);                                                                                          // extract the root entity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selectBody"></param>
        /// <param name="selectParameter"></param>
        /// <param name="whereBody"></param>
        /// <param name="whereParameter"></param>
        /// <param name="extractBody"></param>
        /// <param name="extractParameter"></param>
        /// <returns></returns>
        public IQueryable<TInput> SelectWhereSelect(IQueryable<TInput> source,
                                                    Expression selectBody, ParameterExpression selectParameter,
                                                    Expression whereBody, ParameterExpression whereParameter,
                                                    Expression extractBody, ParameterExpression extractParameter)
        {
            return source.Select(Expression.Lambda<Func<TInput, TSelect>>(selectBody, selectParameter))
                         .Where(Expression.Lambda<Func<TSelect, bool>>(whereBody, whereParameter))
                         .Select(Expression.Lambda<Func<TSelect, TInput>>(extractBody, extractParameter));
        }
    }

    private record EntityData(Expression AccessExpression,
                              bool IsCollection,
                              string Name,
                              Type Type,
                              IEntityType EntityType);
}