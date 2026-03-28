using Npgsql;
using System.Data;

namespace PgLib2;

public sealed class PgSession : IAsyncDisposable
{
    private readonly IConnectionConfig _config;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private string? _connectionString;
    // Reader多重実行防止
    private bool _isReaderActive;

    public NpgsqlConnection Connection
        => _connection ?? throw new InvalidOperationException("Connection is not initialized.");

    public NpgsqlTransaction? Transaction => _transaction;

    public bool IsInTransaction => _transaction != null;

    public ConnectionState State => _connection?.State ?? ConnectionState.Closed;

    private PgSession(IConnectionConfig config)
    {
        _config = config;
    }


    public static async Task<PgSession> CreateAsync(
        IConnectionConfig config,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var session = new PgSession(config);
        session._connectionString = await session.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
        session._connection = new NpgsqlConnection(session._connectionString);
        return session;
    }

    // ===== Connection管理 =====
    private async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _config.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (this.State != ConnectionState.Open)
        {
            await this.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Connection.CloseAsync().ConfigureAwait(false);
    }

    // ===== Transaction管理 =====

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _transaction = await this.Connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
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
        finally
        {
            await this.RollbackAsync(cancellationToken).ConfigureAwait(false);
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
        return new PgQuery(this);
    }

    // ===== 内部用 =====

    internal NpgsqlCommand CreateCommand(
        string query,
        CommandType commandType,
        NpgsqlParameter[]? parameters)
    {
        var cmd = this.Connection.CreateCommand();
        cmd.CommandType = commandType;
        if (parameters != null)
        {
            foreach (var p in parameters)
            {
                cmd.Parameters.Add(p);
            }
        }
        return cmd;
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
        await this.Connection.DisposeAsync();
    }
}