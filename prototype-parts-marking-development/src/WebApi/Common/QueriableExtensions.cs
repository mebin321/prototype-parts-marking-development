namespace WebApi.Common
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Utilities;
    using WebApi.Common.Sorting;
    using WebApi.Data;

    public static class QueriableExtensions
    {
        private static readonly MethodInfo OrderByMethod = typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
            .Single(m => m.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethodDefinition)
            .Single(m => m.GetParameters().Length == 2);

        public static IQueryable<TEntity> FilterWith<TEntity>(
            this IQueryable<TEntity> queriable,
            string value,
            string propertyName)
        {
            Guard.NotNull(queriable, nameof(queriable));
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName));

            if (string.IsNullOrWhiteSpace(value))
            {
                return queriable;
            }

            var lambdaParameter = Expression.Parameter(typeof(TEntity));
            var parameterProperty = Expression.Property(lambdaParameter, typeof(TEntity), propertyName);

            var lambdaBody = Expression.Equal(parameterProperty, Expression.Constant(value));
            var predicate = Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParameter);

            return queriable.Where(predicate);
        }

        public static IQueryable<TEntity> FilterWith<TEntity>(this IQueryable<TEntity> queryable, bool? isActive)
            where TEntity : IAuditableEntity
        {
            Guard.NotNull(queryable, nameof(queryable));

            return isActive switch
            {
                true => queryable.Where(u => u.DeletedAt == null),
                false => queryable.Where(u => u.DeletedAt != null),
                _ => queryable,
            };
        }

        public static IOrderedQueryable<TEntity> SortWith<TEntity>(
            this IQueryable<TEntity> queryable,
            string propertyName,
            string sortDirection,
            Expression<Func<TEntity, int>> secondarySelector,
            ISortColumnMapping sortColumnMapping)
        {
            Guard.NotNull(queryable, nameof(queryable));

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return queryable.OrderBy(secondarySelector);
            }

            var mappedPropertyName = sortColumnMapping[propertyName];
            var property = typeof(TEntity).GetProperty(mappedPropertyName) ?? throw Invalid(propertyName);

            var lambdaParameter = Expression.Parameter(typeof(TEntity));
            var parameterProperty = Expression.Property(lambdaParameter, typeof(TEntity), mappedPropertyName);
            var selector = Expression.Lambda(parameterProperty, lambdaParameter);

            var method = sortDirection switch
            {
                SortDirection.Descending => OrderByDescendingMethod.MakeGenericMethod(typeof(TEntity), property.PropertyType),
                _ => OrderByMethod.MakeGenericMethod(typeof(TEntity), property.PropertyType),
            };
            var methodParameters = new object[]
            {
                queryable,
                selector,
            };

            return ((IOrderedQueryable<TEntity>)method.Invoke(null, methodParameters))?.ThenBy(secondarySelector);
        }

        private static InvalidSortPropertyException Invalid(string propertyName)
            => new($"[{propertyName}] is not a valid sorting property.");
    }
}