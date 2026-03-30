using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgIndexQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("table_oid", NpgsqlTypes.NpgsqlDbType.Oid),
            new NpgsqlParameter("schema_name", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("index_name", NpgsqlTypes.NpgsqlDbType.Text),
        });

    private static readonly string SQL = @"SELECT
 i.oid AS index_oid
,t.oid AS table_oid
,nt.nspname AS table_schema
,t.relname AS table_name
,ni.nspname AS index_schema
,i.relname AS index_name
,ix.indisprimary AS is_primary_key
,ix.indisunique  AS is_unique
,json_agg(
    json_build_object(
         'column_name',regexp_replace(pg_get_indexdef(i.oid, k + 1, true), '\s+(ASC|DESC)\s*$', '', 'i')
        ,'order', COALESCE((regexp_match(pg_get_indexdef(i.oid, k + 1, true), '\s+(ASC|DESC)\s*$', 'i'))[1], 'ASC')
    )
    ORDER BY k
) AS columns
,pg_get_indexdef(i.oid) AS definition
FROM
 pg_class i
INNER JOIN pg_index ix ON (ix.indexrelid = i.oid)
INNER JOIN pg_class t ON (t.oid = ix.indrelid)
INNER JOIN pg_namespace nt ON (nt.oid = t.relnamespace)
INNER JOIN pg_namespace ni ON (ni.oid = t.relnamespace)
INNER JOIN generate_subscripts(ix.indkey, 1) AS k ON true
WHERE
i.relkind IN('i', 'I')
AND (@table_oid IS NULL OR t.oid = @table_oid)
AND (@schema_name IS NULL OR ni.nspname = @schema_name::text)
AND (@index_name IS NULL OR i.relname ILIKE @index_name::text)
GROUP BY
 i.oid
,t.oid
,nt.nspname
,ni.nspname
,ix.indisprimary
,ix.indisunique
,i.relname
ORDER BY
 nt.nspname
,ni.nspname
,t.relname
,i.relname";
    internal static async IAsyncEnumerable<PgIndex> ListAsync(PgCatalog catalog, uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = tableOid;
        sqlSet["schema_name"]!.Value = DBNull.Value;
        sqlSet["index_name"]!.Value = DBNull.Value;

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        await foreach (var ind in q.StreamAsync<PgIndex, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return ind;
        }
    }
    internal static async Task<PgIndex?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = DBNull.Value;
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["index_name"]!.Value = name;

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        return await q.SelectSingleAsync<PgIndex, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct);
    }

    internal static async IAsyncEnumerable<PgIndex> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = DBNull.Value;
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["index_name"]!.Value = nameLike.Like(DBNull.Value);

        await using var session = await catalog.CreateSessionAsync(ct).ConfigureAwait(false);
        var q = session.CreateQuery();
        await foreach (var ind in q.StreamAsync<PgIndex, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return ind;
        }
    }
}
