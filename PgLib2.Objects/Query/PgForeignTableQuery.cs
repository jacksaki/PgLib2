using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgForeignTableQuery
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
,s.srvname AS server_name
,ft.ftoptions
FROM
 pg_namespace nc
INNER JOIN pg_class c ON (nc.oid = c.relnamespace)
INNER JOIN pg_foreign_table ft ON(c.oid = ft.ftrelid)
INNER JOIN pg_foreign_server s ON (s.oid = ft.ftserver)
LEFT OUTER JOIN (pg_type t INNER JOIN pg_namespace nt ON t.typnamespace = nt.oid) ON (c.reloftype = t.oid)
WHERE
c.relkind = 'f'
--AND NOT pg_is_other_temp_schema(nc.oid)
AND nc.nspname = @table_schema
AND (@table_name IS NULL OR c.relname ILIKE @table_name::text)
ORDER BY
 nc.nspname
,c.relname";

    internal static async IAsyncEnumerable<PgForeignTable> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_schema"]!.Value = schemaName;
        sqlSet["table_name"]!.Value = nameLike.Like(DBNull.Value);

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        await foreach (var table in q.StreamAsync<PgForeignTable, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return table;
        }
    }
    internal static async Task<PgForeignTable?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_schema"]!.Value = schemaName;
        sqlSet["table_name"]!.Value = name;

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        return await q.SelectSingleAsync<PgForeignTable, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false);
    }
}
