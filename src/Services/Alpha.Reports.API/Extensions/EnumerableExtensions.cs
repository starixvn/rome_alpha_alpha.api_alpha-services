using System.Linq.Expressions;
using System.Reflection;

namespace Alpha.Reports.API.Extensions;

public static class EnumerableExtensions
{
    public static string JoinWithComma<T>(this IEnumerable<T> source)
        => string.Join(",", source);

    public static IQueryable<T> OrderBy<T>(
               this IEnumerable<T> source,
               string propertyName,
               bool descending)
               => descending
                   ? ApplyOrder(source.AsQueryable(), propertyName, "OrderByDescending")
                   : ApplyOrder(source.AsQueryable(), propertyName, "OrderBy");

    public static IQueryable<T> OrderBy<T>(
        this IEnumerable<T> source,
        string propertyName)
    {
        return OrderBy(source, propertyName, true);
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
    {
        var props = propertyName.Split('.');
        var type = typeof(T);
        var arg = Expression.Parameter(type, "x");
        Expression expr = arg;

        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(prop);
            if (pi == null)
            {
                return (IOrderedQueryable<T>)source;
            }

            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }

        var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        var lambda = Expression.Lambda(delegateType, expr, arg);

        var result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });

        return (IOrderedQueryable<T>)result;
    }

}