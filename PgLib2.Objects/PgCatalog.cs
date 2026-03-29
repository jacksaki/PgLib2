using PgLib2.Objects.Query;
using System.Runtime.CompilerServices;

namespace PgLib2.Objects;

public sealed class PgCatalog
{
    private readonly DatabaseConnectionConfig _config;

    public PgCatalog(DatabaseConnectionConfig config)
    {
        _config = config;
    }

    internal async Task<PgSession> CreateSessionAsync(CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        return await PgSession.CreateAsync(_config, cancellation);
    }
    public async IAsyncEnumerable<PgColumn> ListColumnsAsync(uint oid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var col in PgColumnQuery.ListColumnsAsync(this, oid, ct).ConfigureAwait(false))
        {
            yield return col;
        }
    }
    public async IAsyncEnumerable<PgConstraint> ListConstraintsAsync(uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var con in PgConstraintQuery.ListAsync(this, tableOid, ct).ConfigureAwait(false))
        {
            yield return con;
        }
    }

    public async Task<PgConstraint?> GetConstraintAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgConstraintQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }
    public async Task<IPgObject?> GetAsync(Type t, string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (t == typeof(PgConstraint))
        {
            return await this.GetConstraintAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgForeignTable))
        {
            return await this.GetForeignTableAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgFunction))
        {
            return await this.GetFunctionAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgIndex))
        {
            return await this.GetIndexAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgMaterializedView))
        {
            return await this.GetMaterializedViewAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgPartitionTable))
        {
            return await this.GetPartitionTableAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgProcedure))
        {
            return await this.GetProcedureAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgSequence))
        {
            return await this.GetSequenceAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgTable))
        {
            return await this.GetTableAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgTrigger))
        {
            return await this.GetTriggerAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else if (t == typeof(PgView))
        {
            return await this.GetViewAsync(schemaName, name, ct).ConfigureAwait(false);
        }
        else
        {
            throw new NotSupportedException($"{t.Name} is not supported.");
        }
    }

    public async IAsyncEnumerable<PgConstraint> ListConstraintsAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var con in PgConstraintQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return con;
        }
    }

    public async Task<PgDatabase?> GetDatabaseAsync(string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgDatabase.GetAsync(this, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgDatabase> ListDatabasesAsync(string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var db in PgDatabase.ListAsync(this, nameLike, ct).ConfigureAwait(false))
        {
            yield return db;
        }
    }


    public async Task<PgForeignTable?> GetForeignTableAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgForeignTableQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgForeignTable> ListForeignTablesAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var ftable in PgForeignTableQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return ftable;
        }
    }

    public async Task<PgFunction?> GetFunctionAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgFunctionQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgFunction> ListFunctionsAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var function in PgFunctionQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return function;
        }
    }

    public async IAsyncEnumerable<PgIndex> ListIndexesAsync(uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var ind in PgIndexQuery.ListAsync(this, tableOid, ct).ConfigureAwait(false))
        {
            yield return ind;
        }
    }
    public async Task<PgIndex?> GetIndexAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgIndexQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgIndex> ListIndexesAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var ind in PgIndexQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return ind;
        }
    }
    public async Task<PgMaterializedView?> GetMaterializedViewAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgMaterializedViewQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgMaterializedView> ListMaterializedViewsAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var mview in PgMaterializedViewQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return mview;
        }
    }
    public async Task<PgPartitionTable?> GetPartitionTableAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgPartitionTableQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgPartitionTable> ListPartitionTablesAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var ptable in PgPartitionTableQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return ptable;
        }
    }

    public async Task<PgProcedure?> GetProcedureAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgProcedureQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgProcedure> ListProceduresAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var proc in PgProcedureQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return proc;
        }
    }

    public async IAsyncEnumerable<PgSchema> ListSchemasAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var schema in PgSchemaQuery.ListAsync(this, ct).ConfigureAwait(false))
        {
            yield return schema;
        }
    }

    public async Task<PgSequence?> GetSequenceAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgSequenceQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgSequence> ListSequencesAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var seq in PgSequenceQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return seq;
        }
    }
    public async IAsyncEnumerable<PgSequence> ListSequencesAsync(uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var seq in PgSequenceQuery.ListAsync(this, tableOid, ct).ConfigureAwait(false))
        {
            yield return seq;
        }
    }

    public async Task<PgTable?> GetTableAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgTableQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgTable> ListTablesAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var table in PgTableQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return table;
        }
    }

    public async Task<PgTrigger?> GetTriggerAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgTriggerQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<PgTrigger> ListTriggersAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var trigger in PgTriggerQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return trigger;
        }
    }
    public async IAsyncEnumerable<PgTrigger> ListTriggersAsync(uint tableOid, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var trigger in PgTriggerQuery.ListAsync(this, tableOid, ct).ConfigureAwait(false))
        {
            yield return trigger;
        }
    }
    public async Task<PgView?> GetViewAsync(string schemaName, string name, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return await PgViewQuery.GetAsync(this, schemaName, name, ct).ConfigureAwait(false);
    }
    public async IAsyncEnumerable<PgView> ListViewsAsync(string schemaName, string? nameLike, [EnumeratorCancellation] CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        await foreach (var view in PgViewQuery.ListAsync(this, schemaName, nameLike, ct).ConfigureAwait(false))
        {
            yield return view;
        }
    }
}