using Npgsql;

namespace PgLib2;

public static class PgExtension
{
    internal static Dictionary<string, int> BuildOrdinalMap(this NpgsqlDataReader reader)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            map[name] = i;
        }

        return map;
    }

    public static T Create<T>(this NpgsqlDataReader row, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T>.CreateInstance();
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }

    public static T Create<T, T0>(this NpgsqlDataReader row, T0 param0, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0>.CreateInstance(param0);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
    public static T Create<T, T0, T1>(this NpgsqlDataReader row, T0 param0, T1 param1, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1>.CreateInstance(param0, param1);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
    public static T Create<T, T0, T1, T2>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2>.CreateInstance(param0, param1, param2);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
    public static T Create<T, T0, T1, T2, T3>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, T3 param3, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2, T3>.CreateInstance(param0, param1, param2, param3);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }

    public static T Create<T, T0, T1, T2, T3, T4>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2, T3, T4>.CreateInstance(param0, param1, param2, param3, param4);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
    public static T Create<T, T0, T1, T2, T3, T4, T5>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2, T3, T4, T5>.CreateInstance(param0, param1, param2, param3, param4, param5);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
    public static T Create<T, T0, T1, T2, T3, T4, T5, T6>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2, T3, T4, T5, T6>.CreateInstance(param0, param1, param2, param3, param4, param5, param6);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }

    public static T Create<T, T0, T1, T2, T3, T4, T5, T6, T7>(this NpgsqlDataReader row, T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Dictionary<string, int> map)
    {
        var obj = ConstructorCache<T, T0, T1, T2, T3, T4, T5, T6, T7>.CreateInstance(param0, param1, param2, param3, param4, param5, param6, param7);
        PropertyFieldSetterCache<T>.Set(obj, row, map);
        PostProcessCache<T>.Invoke?.Invoke(obj);
        return obj;
    }
}
