using System.Linq.Expressions;
using System.Reflection;

namespace PgLib2;

internal static class PostProcessCache<T>
{
    public static readonly Action<T>? Invoke;

    static PostProcessCache()
    {
        var attr = typeof(T).GetCustomAttribute<DbClassAttribute>();
        if (attr == null)
        {
            Invoke = null;
            return;
        }

        var method = typeof(T).GetMethod(
            attr.PostProcessMethodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            Type.EmptyTypes,
            modifiers: null);

        if (method == null)
            throw new InvalidOperationException(
                $"Method '{attr.PostProcessMethodName}' not found on {typeof(T)}");

        if (method.ReturnType != typeof(void))
            throw new InvalidOperationException(
                $"Post process method must return void: {method}");

        // (T instance) => instance.RefreshItems();
        var instance = Expression.Parameter(typeof(T), "instance");
        var call = Expression.Call(instance, method);

        Invoke = Expression
            .Lambda<Action<T>>(call, instance)
            .Compile();
    }
}