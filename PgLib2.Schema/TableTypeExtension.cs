using System.Reflection;
using ZLinq;

namespace PgLib2.Schema;

internal static class TableTypeExtension
{
    public static string[] GetArray(this TableTypes t)
    {
        if (t == 0)
            return Array.Empty<string>();

        var type = typeof(TableTypes);

        return Enum.GetValues<TableTypes>().AsValueEnumerable<TableTypes>()
            .Where(flag => flag != 0 && t.HasFlag(flag))
            .Select(flag =>
            {
                var member = type.GetMember(flag.ToString())[0];
                var attr = member.GetCustomAttribute<TableTypeAttribute>();
                return attr?.RelKind;
            })
            .Where(x => x != null)
            .Distinct()
            .ToArray()!;
    }
}

