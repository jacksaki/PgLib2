using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgSequenceQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("table_oid", NpgsqlTypes.NpgsqlDbType.Oid),
            new NpgsqlParameter("schema_name", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("sequence_name", NpgsqlTypes.NpgsqlDbType.Text),
        });

    private static readonly string SQL = @"SELECT
 tbl.oid AS table_oid
,ns.nspname AS sequence_schema
,seq.relname AS sequence_name
,CASE dep.deptype WHEN 'a' THEN 'serial' WHEN 'i' THEN 'identity' ELSE 'standalone' END AS sequence_type
,typ.typname AS data_type
,s.seqstart AS start_value
,s.seqincrement AS increment_by
,s.seqmin AS min_value
,s.seqmax AS max_value
,s.seqcache AS cache_size
,s.seqcycle AS is_cycled
,tbl_ns.nspname AS owned_table_schema
,tbl.relname AS owned_table_name
,col.attname AS owned_column
FROM
pg_class seq
INNER JOIN pg_namespace ns ON (ns.oid = seq.relnamespace)
INNER JOIN pg_sequence s ON (s.seqrelid = seq.oid)
INNER JOIN pg_type typ ON (typ.oid = s.seqtypid)
LEFT OUTER JOIN pg_depend dep ON (dep.objid = seq.oid AND dep.deptype IN ('a','i'))
LEFT OUTER JOIN pg_class tbl ON (tbl.oid = dep.refobjid)
LEFT OUTER JOIN pg_namespace tbl_ns ON (tbl_ns.oid = tbl.relnamespace)
LEFT OUTER JOIN pg_attribute col ON (col.attrelid = tbl.oid AND col.attnum   = dep.refobjsubid)
WHERE
seq.relkind = 'S'
AND (@table_oid   IS NULL OR tbl.oid = @table_oid)
AND (@schema_name IS NULL OR ns.nspname = @schema_name::text)
AND (@sequence_name IS NULL OR seq.relname ILIKE @sequence_name::text)
ORDER BY
 sequence_schema
,sequence_name";

    internal static async IAsyncEnumerable<PgSequence> ListAsync(PgCatalog catalog, uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = tableOid;
        sqlSet["schema_name"]!.Value = DBNull.Value;
        sqlSet["sequence_name"]!.Value = DBNull.Value;

        var q = catalog.Session.CreateQuery();
        await foreach (var seq in q.StreamAsync<PgSequence, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return seq;
        }
    }

    internal static async Task<PgSequence?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = DBNull.Value;
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["sequence_name"]!.Value = name;

        var q = catalog.Session.CreateQuery();
        return await q.SelectSingleAsync<PgSequence, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct);
    }

    internal static async IAsyncEnumerable<PgSequence> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["table_oid"]!.Value = DBNull.Value;
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["sequence_name"]!.Value = nameLike.Like(DBNull.Value);

        var q = catalog.Session.CreateQuery();
        await foreach (var seq in q.StreamAsync<PgSequence, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return seq;
        }
    }
}
