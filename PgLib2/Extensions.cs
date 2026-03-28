using Npgsql;
using System.Collections.Concurrent;
using System.Reflection;

namespace PgLib2;

internal static class Extensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var list = new List<T>();
        await foreach (var item in source.WithCancellation(ct).ConfigureAwait(false))
        {
            list.Add(item);
        }
        return list;
    }

    public static NpgsqlParameter[] ToParameters(this object? param)
    {
        if (param == null)
        {
            return Array.Empty<NpgsqlParameter>();
        }

        if (param is NpgsqlParameter[] npgsqlParams)
        {
            return npgsqlParams;
        }

        if (param is IEnumerable<NpgsqlParameter> enumerableParams)
        {
            return enumerableParams.ToArray();
        }

        if (param is IDictionary<string, object?> dict)
        {
            return dict
                .Select(kvp => CreateParameter(kvp.Key, kvp.Value))
                .ToArray();
        }

        // 匿名オブジェクト / DTO
        return CreateFromObject(param);
    }

    private static ConcurrentDictionary<Type, PropertyInfo[]> _propDict = new ConcurrentDictionary<Type, PropertyInfo[]>();
    private static NpgsqlParameter[] CreateFromObject(this object obj)
    {
        if (!_propDict.TryGetValue(obj.GetType(), out var props))
        {
            props = obj.GetType().GetProperties();
        }

        var result = new NpgsqlParameter[props.Length];

        for (int i = 0; i < props.Length; i++)
        {
            var p = props[i];
            var value = p.GetValue(obj);
            result[i] = CreateParameter(p.Name, value);
        }

        return result;
    }

    private static NpgsqlParameter CreateParameter(string name, object? value)
    {
        return new NpgsqlParameter(name, value ?? DBNull.Value);
    }
}
