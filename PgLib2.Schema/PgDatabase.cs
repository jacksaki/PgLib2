using PgLib2.Schema.Query;

namespace PgLib2.Schema;

public sealed class PgDatabase
{
    internal PgDatabase(PgCatalog catalog)
    {
        _catalog = catalog;
    }

    private readonly PgCatalog _catalog;
    public static async Task<PgDatabase?> GetAsync(PgCatalog catalog, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgDatabaseQuery.GetAsync(catalog, name, ct).ConfigureAwait(false);
    }

    public IAsyncEnumerable<PgSchema> ListSchemaAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return this._catalog.ListSchemasAsync(ct);
    }

    public static IAsyncEnumerable<PgDatabase> ListAsync(PgCatalog catalog, string? nameLike, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return PgDatabaseQuery.ListAsync(catalog, nameLike, ct);
    }
}
