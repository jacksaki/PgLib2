using System.Runtime.CompilerServices;

namespace PgLib2.Schema;

public abstract class PgRelationBase: IPgObject
{
    public abstract Task<string> GenerateDDLAsync(DDLOptions options, CancellationToken ct = default);
    protected uint _oid;
    internal PgRelationBase(PgCatalog catalog)
    {
        _catalog = catalog;
    }
    protected PgCatalog _catalog;

    public abstract string SchemaName { get; protected set; } 
    public abstract string Name { get; protected set; } 
    public abstract string? Comment { get; protected set; }

    public async IAsyncEnumerable<PgColumn> ListColumnsAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var col in _catalog.ListColumnsAsync(_oid, ct).ConfigureAwait(false))
        {
            yield return col;
        }
    }
    public async IAsyncEnumerable<PgIndex> ListIndexesAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var index in _catalog.ListIndexesAsync(_oid, ct).ConfigureAwait(false))
        {
            yield return index;
        }
    }
    public async IAsyncEnumerable<PgConstraint> ListConstraintsAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var con in _catalog.ListConstraintsAsync(_oid, ct).ConfigureAwait(false))
        {
            yield return con;
        }
    }
    public async IAsyncEnumerable<PgTrigger> ListTriggersAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var trigger in _catalog.ListTriggersAsync(_oid, ct).ConfigureAwait(false))
        {
            yield return trigger;
        }
    }
    public async IAsyncEnumerable<PgSequence> ListSequencesAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var seq in _catalog.ListSequencesAsync(_oid, ct).ConfigureAwait(false))
        {
            yield return seq;
        }
    }
}
