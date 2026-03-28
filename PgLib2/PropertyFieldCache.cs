using Npgsql;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace PgLib2;

internal static class PropertyFieldSetterCache<T>
{
    private static readonly Action<T, NpgsqlDataReader, IReadOnlyDictionary<string, int>> _setter;

    internal static IReadOnlyDictionary<string, int> BuildOrdinalMap(NpgsqlDataReader reader)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
        {
            dict[reader.GetName(i)] = i;
        }
        return dict;
    }

    static PropertyFieldSetterCache()
    {
        var target = Expression.Parameter(typeof(T), "target");
        var reader = Expression.Parameter(typeof(NpgsqlDataReader), "reader");
        var ordinals = Expression.Parameter(typeof(IReadOnlyDictionary<string, int>), "ordinals");

        var body = new List<Expression>();

        var getValueMethod = typeof(NpgsqlDataReader)
            .GetMethod(nameof(NpgsqlDataReader.GetValue))!;

        var tryGetValueMethod =
            typeof(IReadOnlyDictionary<string, int>)
                .GetMethod(nameof(IReadOnlyDictionary<string, int>.TryGetValue))!;

        foreach (var member in typeof(T)
            .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var attr = member.GetCustomAttribute<DbColumnAttribute>();
            if (attr == null)
                continue;

            Type memberType;
            MethodInfo? setMethod = null;
            bool isField = false;

            switch (member)
            {
                case PropertyInfo p:
                    if (p.GetIndexParameters().Length != 0)
                        continue;

                    setMethod = p.GetSetMethod(true);
                    if (setMethod == null)
                        continue;

                    memberType = p.PropertyType;
                    break;

                case FieldInfo f:
                    if (f.IsInitOnly)
                        continue;

                    memberType = f.FieldType;
                    isField = true;
                    break;

                default:
                    continue;
            }

            // int ordinal;
            var ordinalVar = Expression.Variable(typeof(int), "ordinal");

            // ordinals.TryGetValue("column", out ordinal)
            var hasColumn = Expression.Call(
                ordinals,
                tryGetValueMethod,
                Expression.Constant(attr.ColumnName),
                ordinalVar
            );

            // reader.GetValue(ordinal)
            var rawValue = Expression.Call(reader, getValueMethod, ordinalVar);

            var dbNull = Expression.ReferenceEqual(
                rawValue,
                Expression.Constant(DBNull.Value)
            );

            Expression valueExpr;

            var isNullableDateTime =
                memberType == typeof(DateTime?) ||
                Nullable.GetUnderlyingType(memberType) == typeof(DateTime);

            // ---- DateFormat 対応 ----
            if (attr.DateFormat != null &&
                (memberType == typeof(DateTime) || isNullableDateTime))
            {
                var tryParseExact = typeof(DateTime).GetMethod(
                    nameof(DateTime.TryParseExact),
                    new[]
                    {
                        typeof(string),
                        typeof(string),
                        typeof(IFormatProvider),
                        typeof(DateTimeStyles),
                        typeof(DateTime).MakeByRefType()
                    })!;

                var tmpVar = Expression.Variable(typeof(DateTime), "tmp");

                var tryParse = Expression.Call(
                    tryParseExact,
                    Expression.Convert(rawValue, typeof(string)),
                    Expression.Constant(attr.DateFormat),
                    Expression.Constant(CultureInfo.InvariantCulture),
                    Expression.Constant(DateTimeStyles.None),
                    tmpVar
                );

                Expression parsedValue =
                    isNullableDateTime
                        ? Expression.Convert(tmpVar, memberType)
                        : tmpVar;

                valueExpr =
                    Expression.Condition(
                        hasColumn,
                        Expression.Condition(
                            dbNull,
                            Expression.Default(memberType),
                            Expression.Block(
                                new[] { tmpVar },
                                Expression.Condition(
                                    tryParse,
                                    parsedValue,
                                    Expression.Default(memberType)
                                )
                            )
                        ),
                        Expression.Default(memberType) // ← 列が無い
                    );
            }
            else
            {
                valueExpr =
                    Expression.Condition(
                        hasColumn,
                        Expression.Condition(
                            dbNull,
                            Expression.Default(memberType),
                            Expression.Convert(rawValue, memberType)
                        ),
                        Expression.Default(memberType) // ← 列が無い
                    );
            }

            Expression assign =
                isField
                    ? Expression.Assign(
                        Expression.Field(target, (FieldInfo)member),
                        valueExpr)
                    : Expression.Call(
                        target,
                        setMethod!,
                        valueExpr);

            body.Add(
                Expression.Block(
                    new[] { ordinalVar },
                    assign
                )
            );
        }

        _setter = Expression
            .Lambda<Action<T, NpgsqlDataReader, IReadOnlyDictionary<string, int>>>(
                Expression.Block(body),
                target,
                reader,
                ordinals
            )
            .Compile();
    }

    public static void Set(
        T instance,
        NpgsqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals)
        => _setter(instance, reader, ordinals);
}
