using System.Linq.Expressions;
using System.Reflection;

namespace PgLib2;

internal static class ConstructorCache<T>
{
    private static readonly Func<T> _factory;
    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null
        )!;
        var newExpr = Expression.New(ctor);
        _factory = Expression.Lambda<Func<T>>(newExpr).Compile();
    }
    public static T CreateInstance() => _factory();
}

internal static class ConstructorCache<T, T0>
{
    private static readonly Func<T0, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var newExpr = Expression.New(ctor, p0);

        _factory = Expression.Lambda<Func<T0, T>>(newExpr, p0).Compile();
    }

    public static T CreateInstance(T0 param0)
        => _factory(param0);
}

internal static class ConstructorCache<T, T0, T1>
{
    private static readonly Func<T0, T1, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var newExpr = Expression.New(ctor, p0);

        _factory = Expression.Lambda<Func<T0, T1, T>>(newExpr, p0, p1).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1)
        => _factory(param0, param1);
}

internal static class ConstructorCache<T, T0, T1, T2>
{
    private static readonly Func<T0, T1, T2, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var newExpr = Expression.New(ctor, p0, p1, p2);

        _factory = Expression.Lambda<Func<T0, T1, T2, T>>(newExpr, p0, p1, p2).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2)
        => _factory(param0, param1, param2);
}

internal static class ConstructorCache<T, T0, T1, T2, T3>
{
    private static readonly Func<T0, T1, T2, T3, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var p3 = Expression.Parameter(typeof(T3), "p3");
        var newExpr = Expression.New(ctor, p0, p1, p2, p3);

        _factory = Expression.Lambda<Func<T0, T1, T2, T3, T>>(newExpr, p0, p1, p2, p3).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2, T3 param3)
        => _factory(param0, param1, param2, param3);
}

internal static class ConstructorCache<T, T0, T1, T2, T3, T4>
{
    private static readonly Func<T0, T1, T2, T3, T4, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var p3 = Expression.Parameter(typeof(T3), "p3");
        var p4 = Expression.Parameter(typeof(T4), "p4");
        var newExpr = Expression.New(ctor, p0, p1, p2, p3, p4);

        _factory = Expression.Lambda<Func<T0, T1, T2, T3, T4, T>>(newExpr, p0, p1, p2, p3, p4).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4)
        => _factory(param0, param1, param2, param3, param4);
}

internal static class ConstructorCache<T, T0, T1, T2, T3, T4, T5>
{
    private static readonly Func<T0, T1, T2, T3, T4, T5, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var p3 = Expression.Parameter(typeof(T3), "p3");
        var p4 = Expression.Parameter(typeof(T4), "p4");
        var p5 = Expression.Parameter(typeof(T5), "p5");
        var newExpr = Expression.New(ctor, p0, p1, p2, p3, p4, p5);

        _factory = Expression.Lambda<Func<T0, T1, T2, T3, T4, T5, T>>(newExpr, p0, p1, p2, p3, p4, p5).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        => _factory(param0, param1, param2, param3, param4, param5);
}

internal static class ConstructorCache<T, T0, T1, T2, T3, T4, T5, T6>
{
    private static readonly Func<T0, T1, T2, T3, T4, T5, T6, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var p3 = Expression.Parameter(typeof(T3), "p3");
        var p4 = Expression.Parameter(typeof(T4), "p4");
        var p5 = Expression.Parameter(typeof(T5), "p5");
        var p6 = Expression.Parameter(typeof(T6), "p6");
        var newExpr = Expression.New(ctor, p0, p1, p2, p3, p4, p5, p6);

        _factory = Expression.Lambda<Func<T0, T1, T2, T3, T4, T5, T6, T>>(newExpr, p0, p1, p2, p3, p4, p5, p6).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        => _factory(param0, param1, param2, param3, param4, param5, param6);
}

internal static class ConstructorCache<T, T0, T1, T2, T3, T4, T5, T6, T7>
{
    private static readonly Func<T0, T1, T2, T3, T4, T5, T6, T7, T> _factory;

    static ConstructorCache()
    {
        var ctor = typeof(T).GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) },
            null
        )!;

        var p0 = Expression.Parameter(typeof(T0), "p0");
        var p1 = Expression.Parameter(typeof(T1), "p1");
        var p2 = Expression.Parameter(typeof(T2), "p2");
        var p3 = Expression.Parameter(typeof(T3), "p3");
        var p4 = Expression.Parameter(typeof(T4), "p4");
        var p5 = Expression.Parameter(typeof(T5), "p5");
        var p6 = Expression.Parameter(typeof(T6), "p6");
        var p7 = Expression.Parameter(typeof(T7), "p7");
        var newExpr = Expression.New(ctor, p0, p1, p2, p3, p4, p5, p6, p7);

        _factory = Expression.Lambda<Func<T0, T1, T2, T3, T4, T5, T6, T7, T>>(newExpr, p0, p1, p2, p3, p4, p5, p6, p7).Compile();
    }

    public static T CreateInstance(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        => _factory(param0, param1, param2, param3, param4, param5, param6, param7);
}