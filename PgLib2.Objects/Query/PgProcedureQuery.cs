using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgProcedureQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("schema_name", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("proc_name", NpgsqlTypes.NpgsqlDbType.Text),
        });

    private static readonly string SQL = @"SELECT
 n.nspname AS routine_schema
,p.proname AS routine_name
,COALESCE(type_map.dst, dt.raw_data_type) AS data_type
,CASE WHEN l.lanname = 'sql' THEN 'SQL' ELSE 'EXTERNAL' END AS routine_body,
CASE WHEN pg_has_role(p.proowner, 'USAGE') THEN p.prosrc ELSE NULL END AS routine_definition
,CASE WHEN l.lanname = 'c' THEN p.prosrc ELSE NULL END AS external_name
,upper(l.lanname) AS external_language
,p.provolatile = 'i' AS is_deterministic
FROM
 pg_namespace n
INNER JOIN pg_proc p ON (n.oid = p.pronamespace)
INNER JOIN pg_language l ON (p.prolang = l.oid)
LEFT OUTER JOIN pg_type t ON (p.prorettype = t.oid AND p.prokind <> 'p')
LEFT OUTER JOIN pg_namespace nt ON (t.typnamespace = nt.oid)
CROSS JOIN LATERAL (
SELECT
 CASE WHEN p.prokind = 'p' THEN NULL
      WHEN t.typelem <> 0 AND t.typlen = -1 THEN 'ARRAY'
      WHEN nt.nspname = 'pg_catalog' THEN format_type(t.oid, NULL)
      ELSE 'USER-DEFINED' END AS raw_data_type
) dt
LEFT OUTER JOIN (
    VALUES
     ('character varying', 'varchar')
    ,('character', 'char')
    ,('double precision','float8')
    ,('real', 'float4')
--    ,('timestamp without time zone', 'timestamp')
--    ,('timestamp with time zone', 'timestamptz')
--    ,('time without time zone', 'time')
--    ,('time with time zone', 'timetz')
) AS type_map(src, dst)
ON (type_map.src = dt.raw_data_type)
WHERE
n.nspname = @schema_name
AND p.prokind = 'p'
AND (@proc_name IS NULL OR p.proname ILIKE @proc_name::text)
ORDER BY
 n.nspname
,p.proname";

    internal static async Task<PgProcedure?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["proc_name"]!.Value = name;
        var q = catalog.Session.CreateQuery();
        return await q.SelectSingleAsync<PgProcedure, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct);
    }
    internal static async IAsyncEnumerable<PgProcedure> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var sqlSet = GenerateSQLSet();
        sqlSet["schema_name"]!.Value = schemaName;
        sqlSet["proc_name"]!.Value = nameLike.Like(DBNull.Value);
        var q = catalog.Session.CreateQuery();
        await foreach (var f in q.StreamAsync<PgProcedure, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return f;
        }
    }
}