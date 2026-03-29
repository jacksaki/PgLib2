using Renci.SshNet;
using System.Collections.Concurrent;

namespace PgLib2.SSH;

public class SshTunnel : IAsyncDisposable
{
    private static readonly ConcurrentBag<SshTunnel> _instances = new();

    private SshClient? _sshClient;
    private ForwardedPortLocal? _forwardedPort;
    private SecureShellConfig _ssh;
    private DatabaseConnectionConfig _dbConfig;
    public DatabaseConnectionConfig DbConfig => _dbConfig;

    public uint LocalPort => _ssh.LocalPort ?? throw new InvalidOperationException("SSHトンネルが接続されていません。");
    public SshTunnel(SecureShellConfig sshConfig, DatabaseConnectionConfig dbConfig)
    {
        _ssh = sshConfig;
        _dbConfig = dbConfig;
        _instances.Add(this);
    }
    public async Task<string> GetConnectionStringAsync()
    {
        return await Task.FromResult($"Host=\"127.0.0.1\";Port={this.LocalPort};Username=\"{this.DbConfig.UserName}\";Password=\"{this.DbConfig.Password}\";Database=\"{this.DbConfig.DatabaseName}\";{this.DbConfig.Option}");
    }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (_sshClient?.IsConnected == true)
        {
            return;
        }

        _ssh.LocalPort = LocalPortAllocator.Allocate();

        var connectionInfo = !string.IsNullOrEmpty(_ssh.SshPrivateKey)
            ? new ConnectionInfo(_ssh.SshHostName, _ssh.SshPort, _ssh.SshUserName, new PrivateKeyAuthenticationMethod(_ssh.SshUserName, new PrivateKeyFile(_ssh.SshPrivateKey)))
            : new ConnectionInfo(_ssh.SshHostName, _ssh.SshPort, _ssh.SshUserName, new PasswordAuthenticationMethod(_ssh.SshUserName, _ssh.SshPassword));

        _sshClient = new SshClient(connectionInfo);

        await _sshClient.ConnectAsync(ct);
        _forwardedPort = new ForwardedPortLocal("127.0.0.1", (uint)_ssh.LocalPort.Value, _dbConfig.HostName, (uint)_dbConfig.Port);
        _sshClient.AddForwardedPort(_forwardedPort);
        this.DbConfig.Tunnel = this;
        _forwardedPort.Start();
    }

    // 個別の切断処理
    public async ValueTask DisposeAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                if (_forwardedPort != null && _forwardedPort.IsStarted)
                {
                    _forwardedPort.Stop();
                }
                if (_sshClient != null)
                {
                    if (_sshClient.IsConnected)
                    {
                        _sshClient.Disconnect();
                    }
                    _sshClient.Dispose();
                }
            }
            catch { /* ignore */ }
        });

        GC.SuppressFinalize(this);
    }

    // 全てを一括で切断する static メソッド
    public static async Task DisconnectAllAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var tasks = _instances.Select(i => i.DisposeAsync().AsTask());
        await Task.WhenAll(tasks).ConfigureAwait(false);
        // リストをクリア
        _instances.Clear();
    }
}