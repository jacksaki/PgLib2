using Npgsql;
using System.Data;
using System.Data.Common;

namespace PgLib2;

public sealed class PgSession : IAsyncDisposable
{
    internal string? ConnectionString { get; private set; }
    private readonly DatabaseConnectionConfig _config;
    public NpgsqlConnection PrimaryConnection { get; private set; }
    public NpgsqlTransaction? Transaction { get; private set; }

    // Reader多重実行防止
    private bool _isReaderActive;

    public bool IsInTransaction => this.Transaction != null;

    public ConnectionState State => this.PrimaryConnection?.State ?? ConnectionState.Closed;

    private PgSession(DatabaseConnectionConfig config)
    {
        _config = config;
        this.PrimaryConnection = new NpgsqlConnection();
    }


    public static async Task<PgSession> CreateAsync(
        DatabaseConnectionConfig config,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var session = new PgSession(config);
        session.ConnectionString = await session.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
        session.PrimaryConnection.ConnectionString = session.ConnectionString;
        await session.OpenAsync(cancellationToken).ConfigureAwait(false);
        return session;
    }
    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await this.PrimaryConnection.CloseAsync().ConfigureAwait(false);
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (this.PrimaryConnection.State != ConnectionState.Open)
        {
            await this.PrimaryConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    // ===== Connection管理 =====
    internal async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _config.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
    }

    // ===== Transaction管理 =====

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this.Transaction = await this.PrimaryConnection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!this.IsInTransaction)
        {
            throw new Exception("No active transaction to commit.");
        }
        await this.Transaction!.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!this.IsInTransaction)
        {
            throw new Exception("No active transaction to commit.");
        }
        await this.Transaction!.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task WithTransactionAsync(
        Func<CancellationToken, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        await this.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        try
        {
            await Task.Run(() => action(cancellationToken), cancellationToken).ConfigureAwait(false);
            await this.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch 
        {
            await this.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<T> WithTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        await this.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        try
        {
            var result = await Task.Run(() => action(cancellationToken), cancellationToken).ConfigureAwait(false);
            await this.CommitAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        finally
        {
            await this.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    // ===== Query生成 =====

    public PgQuery CreateQuery()
    {
        return PgQuery.Create(this);
    }

    internal void EnsureNoActiveReader()
    {
        if (this._isReaderActive)
        {
            throw new InvalidOperationException($"A data reader is already active on this session. Multiple readers cannot be used simultaneously.");
        }
    }

    internal void SetReaderActive(bool active)
    {
        this._isReaderActive = active;
    }

    public async ValueTask DisposeAsync()
    {
        await this.PrimaryConnection.DisposeAsync();
    }
}