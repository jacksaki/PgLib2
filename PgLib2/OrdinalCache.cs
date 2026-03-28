using Npgsql;

namespace PgLib2;

internal static class OrdinalCache
{
    internal static Dictionary<string, int> BuildOrdinalMap(NpgsqlDataReader reader)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
        {
            dict[reader.GetName(i)] = i;
        }
        return dict;
    }
}
