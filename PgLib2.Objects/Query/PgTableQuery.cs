using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgTableQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("table_schema", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("table_name", NpgsqlTypes.NpgsqlDbType.Text),
        });
    private static readonly string SQL = @"SELECT
 c.oid
,nc.nspname::information_schema.sql_identifier AS table_schema
,c.relname::information_schema.sql_identifier AS table_name
,(pg_relation_is_updatable(c.oid::regclass, false) & 8) = 8  AS is_insertable_into
FROM
 pg_namespace nc
INNER JOIN pg_class c ON (nc.oid = c.relnamespace)
LEFT OUTER JOIN (pg_type t INNER JOIN pg_namespace nt ON t.typnamespace = nt.oid) ON (c.reloftype = t.oid)
WHERE
c.relkind = 'r'
--AND NOT pg_is_other_temp_schema(nc.oid)
AND nc.nspname = @table_schema
AND (@table_name IS NULL OR c.relname ILIKE @table_name::text)";
    internal static async Task<PgTable?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_schema"]!.Value = schemaName;
        sqlSet["table_name"]!.Value = name;

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        return await q.SelectSingleAsync<PgTable, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false);
    }

    internal static async IAsyncEnumerable<PgTable> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_schema"]!.Value = schemaName;
        sqlSet["table_name"]!.Value = nameLike.Like(DBNull.Value);

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        await foreach (var table in q.StreamAsync<PgTable, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return table;
        }
    }
}
