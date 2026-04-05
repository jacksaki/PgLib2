using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Schema.Query;

internal class PgMaterializedViewQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("mview_schema", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("mview_name", NpgsqlTypes.NpgsqlDbType.Text),
        });
    private static readonly string SQL = @"SELECT
 c.oid
,nc.nspname::information_schema.sql_identifier AS mview_schema
,c.relname::information_schema.sql_identifier AS mview_name
,obj_description(c.oid) AS comment
,pg_get_viewdef(c.oid, true) AS view_definition
,(pg_relation_is_updatable(c.oid::regclass, false) & 8) = 8  AS is_insertable_into
FROM
 pg_namespace nc
INNER JOIN pg_class c ON (nc.oid = c.relnamespace)
LEFT OUTER JOIN (pg_type t INNER JOIN pg_namespace nt ON t.typnamespace = nt.oid) ON (c.reloftype = t.oid)
WHERE
c.relkind = 'm'
--AND NOT pg_is_other_temp_schema(nc.oid)
AND nc.nspname = @mview_schema
AND (@mview_name IS NULL OR c.relname ILIKE @mview_name::text)
ORDER BY
 nc.nspname
,c.relname";

    internal static async Task<PgMaterializedView?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["mview_schema"]!.Value = schemaName;
        sqlSet["mview_name"]!.Value = name;

        var q = catalog.Session.CreateQuery();
        return await q.SelectSingleAsync<PgMaterializedView, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false);
    }

    internal static async IAsyncEnumerable<PgMaterializedView> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["mview_schema"]!.Value = schemaName;
        sqlSet["mview_name"]!.Value = nameLike.Like(DBNull.Value);

        var q = catalog.Session.CreateQuery();
        await foreach (var mview in q.StreamAsync<PgMaterializedView, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return mview;
        }
    }
}
