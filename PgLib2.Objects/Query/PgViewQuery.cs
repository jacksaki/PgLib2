using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

internal class PgViewQuery
{
    public static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("view_schema", NpgsqlTypes.NpgsqlDbType.Text),
            new NpgsqlParameter("view_name", NpgsqlTypes.NpgsqlDbType.Text),
        });

    private static readonly string SQL = @"SELECT
 c.oid
,nc.nspname::information_schema.sql_identifier AS view_schema
,c.relname::information_schema.sql_identifier AS view_name
,pg_get_viewdef(c.oid, true) AS view_definition
,obj_description(c.oid) AS comment
,(pg_relation_is_updatable(c.oid::regclass, false) & 8) = 8  AS is_insertable_into
FROM
 pg_namespace nc
INNER JOIN pg_class c ON (nc.oid = c.relnamespace)
LEFT OUTER JOIN (pg_type t INNER JOIN pg_namespace nt ON t.typnamespace = nt.oid) ON (c.reloftype = t.oid)
WHERE
c.relkind = 'v'
--AND NOT pg_is_other_temp_schema(nc.oid)
AND nc.nspname = @view_schema
AND (@view_name IS NULL OR c.relname ILIKE @view_name::text)
ORDER BY
 nc.nspname
,c.relname";

    internal static async Task<PgView?> GetAsync(PgCatalog catalog, string schemaName, string name, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["view_schema"]!.Value = schemaName;
        sqlSet["view_name"]!.Value = name;

        var q = catalog.Session.CreateQuery();
        return await q.SelectSingleAsync<PgView, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false);
    }

    internal static async IAsyncEnumerable<PgView> ListAsync(PgCatalog catalog, string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["view_schema"]!.Value = schemaName;
        sqlSet["view_name"]!.Value = nameLike.Like(DBNull.Value);

        var q = catalog.Session.CreateQuery();
        await foreach (var view in q.StreamAsync<PgView, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return view;
        }
    }
}
