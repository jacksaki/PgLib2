using PgLib2.Schema.Query;

namespace PgLib2.Schema;


public sealed class PgSchema
{
    public static SQLSet GetSQLSet() => PgSchemaQuery.GenerateSQLSet();
    internal PgSchema(PgCatalog catalog)
    {
        _catalog = catalog;
    }

    private readonly PgCatalog _catalog;

    [DbColumn("schema_name")]
    public string Name { get; private set; } = string.Empty;

    public IAsyncEnumerable<PgTable> ListTablesAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListTablesAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgView> ListViewsAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListViewsAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgMaterializedView> ListMaterializedViewsAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListMaterializedViewsAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgForeignTable> ListForeignTablesAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListForeignTablesAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgPartitionTable> ListPartitionTablesAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListPartitionTablesAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgIndex> ListIndexesAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListIndexesAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgProcedure> ListProceduresAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListProceduresAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgFunction> ListFunctionsAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListFunctionsAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgConstraint> ListConstraintsAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListConstraintsAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgSequence> ListSequencesAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListSequencesAsync(this.Name, nameLike, ct);
    }
    public IAsyncEnumerable<PgTrigger> ListTriggersAsync(string? nameLike, CancellationToken ct = default)
    {
        return _catalog.ListTriggersAsync(this.Name, nameLike, ct);
    }
}