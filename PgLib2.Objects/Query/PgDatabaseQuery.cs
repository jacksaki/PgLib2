using Npgsql;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects.Query;

public class PgDatabaseQuery
{
    internal static SQLSet GenerateSQLSet()
        => new SQLSet(SQL, new NpgsqlParameter[]
        {
            new NpgsqlParameter("database_name", NpgsqlTypes.NpgsqlDbType.Text),
        });

    private static readonly string SQL = @"SELECT
 d.oid
,d.datname
FROM
 pg_database d
WHERE
(@database_name IS NULL OR d.datname = @database_name::text)
ORDER BY
 d.datname";

    internal static async IAsyncEnumerable<PgDatabase> ListAsync(PgCatalog catalog, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["database_name"]!.Value = nameLike.Like(DBNull.Value);

        var q = catalog.Session.CreateQuery();
        await foreach (var db in q.StreamAsync<PgDatabase, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false))
        {
            yield return db;
        }
    }

    internal static async Task<PgDatabase?> GetAsync(PgCatalog catalog, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var sqlSet = GenerateSQLSet();
        sqlSet["database_name"]!.Value = name;

        var q = catalog.Session.CreateQuery();
        return await q.SelectSingleAsync<PgDatabase, PgCatalog>(catalog, sqlSet.SQL, sqlSet.Parameters, ct).ConfigureAwait(false);
    }
}
