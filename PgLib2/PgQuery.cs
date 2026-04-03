using Npgsql;
using Renci.SshNet;
using System.Data;
using System.Runtime.CompilerServices;

namespace PgLib2;

public class PgQuery
{
    private readonly PgSession _session;
    private NpgsqlConnection _connection;
    internal static PgQuery Create(PgSession session)
    {
        var q = new PgQuery(session);
        q._connection.ConnectionString = session.ConnectionString;
        return q;
    }
    private PgQuery(PgSession session)
    {
        _session = session;
        _connection = new NpgsqlConnection();
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _connection.CloseAsync().ConfigureAwait(false);
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var cmd = await this.CreateCommandAsync(sql, CommandType.Text, param.ToParameters(), cancellationToken);
        return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<Dictionary<string, object?>> QueryStreamAsync(string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _session.SetReaderActive(true);
        try
        {
            await using var dr = await this.GetDataReaderAsync(sql, param, cancellationToken).ConfigureAwait(false);

            var result = new List<Dictionary<string, object?>>();
            var fieldCount = dr.FieldCount;
            while (await dr.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var item = new Dictionary<string, object?>();
                var values = new object[fieldCount];
                dr.GetValues(values);
                for (var i = 0; i < fieldCount; i++)
                {
                    item.Add(dr.GetName(i), values[i] == DBNull.Value ? null : values[i]);
                }
                yield return item;
            }
        }
        finally
        {
            _session.SetReaderActive(false);
        }
    }

    public async Task<List<Dictionary<string, object?>>> QueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.QueryStreamAsync(sql, param, cancellationToken).ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, object?>> QuerySingleAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dr = await this.GetSingleDataReaderAsync(sql, param, cancellationToken).ConfigureAwait(false);
        var result = new Dictionary<string, object?>();
        if (await dr.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var fieldCount = dr.FieldCount;

            var values = new object[fieldCount];
            dr.GetValues(values);
            for (var i = 0; i < fieldCount; i++)
            {
                result.Add(dr.GetName(i), values[i] == DBNull.Value ? null : values[i]);
            }
        }
        return result;
    }
    internal async Task<NpgsqlCommand> CreateCommandAsync(
    string query,
    CommandType commandType,
    NpgsqlParameter[]? parameters,
    CancellationToken ct)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandType = commandType;
        cmd.CommandText = query;
        if (parameters != null)
        {
            foreach (var p in parameters)
            {
                cmd.Parameters.Add(p);
            }
        }
        if(_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct).ConfigureAwait(false);
        }
        return cmd;
    }

    #region SelectSingleAsync
    public async Task<T?> SelectSingleAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T>(dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0>(T0 t0, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0>(t0, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1>(T0 t0, T1 t1, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1>(t0, t1, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2>(T0 t0, T1 t1, T2 t2, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2>(t0, t1, t2, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2, T3>(t0, t1, t2, t3, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2, T3, T4>(t0, t1, t2, t3, t4, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5>(t0, t1, t2, t3, t4, t5, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5, T6>(t0, t1, t2, t3, t4, t5, t6, dr.Map);
    }
    public async Task<T?> SelectSingleAsync<T, T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var dr = await this.SelectSingleAsync(sql, param, cancellationToken).ConfigureAwait(false);
        return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5, T6, T7>(t0, t1, t2, t3, t4, t5, t6, t7, dr.Map);
    }
    #endregion

    #region SelectAsync
    public async Task<List<T>> SelectAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T>(sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0>(T0 t0, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0>(t0, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1>(T0 t0, T1 t1, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1>(t0, t1, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2>(T0 t0, T1 t1, T2 t2, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2>(t0, t1, t2, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2, T3>(t0, t1, t2, t3, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2, T3, T4>(t0, t1, t2, t3, t4, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2, T3, T4, T5>(t0, t1, t2, t3, t4, t5, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2, T3, T4, T5, T6>(t0, t1, t2, t3, t4, t5, t6, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<T>> SelectAsync<T, T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await this.StreamAsync<T, T0, T1, T2, T3, T4, T5, T6, T7>(t0, t1, t2, t3, t4, t5, t6, t7, sql, param, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region StreamAsync
    public async IAsyncEnumerable<T> StreamAsync<T>(string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T>(dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0>(T0 t0, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0>(t0, dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1>(T0 t0, T1 t1, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1>(t0, t1, dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2>(T0 t0, T1 t1, T2 t2, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2>(t0, t1, t2, dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2, T3>(t0, t1, t2, t3, dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2, T3, T4>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2, T3, T4>(t0, t1, t2, t3, t4, dr.Map);
        }
    }
    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2, T3, T4, T5>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5>(t0, t1, t2, t3, t4, t5, dr.Map);
        }
    }

    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2, T3, T4, T5, T6>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5, T6>(t0, t1, t2, t3, t4, t5, t6, dr.Map);
        }
    }

    public async IAsyncEnumerable<T> StreamAsync<T, T0, T1, T2, T3, T4, T5, T6, T7>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var dr in StreamAsync(sql, param, cancellationToken).ConfigureAwait(false))
        {
            yield return dr.Reader.Create<T, T0, T1, T2, T3, T4, T5, T6, T7>(t0, t1, t2, t3, t4, t5, t6, t7, dr.Map);
        }
    }
    #endregion

    private async Task<NpgsqlDataReader> GetDataReaderAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var cmd = await this.CreateCommandAsync(sql, CommandType.Text, param.ToParameters(), cancellationToken);
        return await cmd.ExecuteReaderAsync(
            CommandBehavior.Default,
            cancellationToken
        ).ConfigureAwait(false);
    }

    private async Task<NpgsqlDataReader> GetSingleDataReaderAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var cmd = await this.CreateCommandAsync(sql, CommandType.Text, param.ToParameters(), cancellationToken);
        return await cmd.ExecuteReaderAsync(
            CommandBehavior.SingleRow,
            cancellationToken
        ).ConfigureAwait(false);
    }

    private async Task<(NpgsqlDataReader Reader, Dictionary<string, int> Map)> SelectSingleAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var dr = await GetSingleDataReaderAsync(sql, param, cancellationToken).ConfigureAwait(false);
        if (await dr.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var map = dr.BuildOrdinalMap();
            return new(dr, map);
        }

        return default;
    }

    private async IAsyncEnumerable<(NpgsqlDataReader Reader, Dictionary<string, int> Map)> StreamAsync(string sql, object? param = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _session.SetReaderActive(true);
        try
        {
            await using var dr = await GetDataReaderAsync(sql, param, cancellationToken).ConfigureAwait(false);
            var map = dr.BuildOrdinalMap();
            while (await dr.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return new(dr, map);
            }
        }
        finally
        {
            _session.SetReaderActive(false);
        }
    }
}