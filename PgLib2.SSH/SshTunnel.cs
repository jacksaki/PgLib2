using Renci.SshNet;
using System.Collections.Concurrent;

namespace PgLib2.SSH;

public class SshTunnel : IAsyncDisposable
{
    private static readonly ConcurrentBag<SshTunnel> _instances = new();

    private SshClient? _sshClient;
    private ForwardedPortLocal? _forwardedPort;
    private IConnectionConfig _config;

    public SshTunnel(IConnectionConfig config)
    {
        _config = config;
        _instances.Add(this);
    }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_config is not SSHConnectionConfig)
        {
            return;
        }

        if (_sshClient?.IsConnected == true)
        {
            return;
        }

        var conf = (SSHConnectionConfig)_config;
        var ssh = conf.SSH;
        ssh.LocalPort = LocalPortAllocator.Allocate();

        var connectionInfo = !string.IsNullOrEmpty(ssh.SshPrivateKey)
            ? new ConnectionInfo(ssh.SshHostName, ssh.SshPort, ssh.SshUserName, new PrivateKeyAuthenticationMethod(ssh.SshUserName, new PrivateKeyFile(ssh.SshPrivateKey)))
            : new ConnectionInfo(ssh.SshHostName, ssh.SshPort, ssh.SshUserName, new PasswordAuthenticationMethod(ssh.SshUserName, ssh.SshPassword));

        _sshClient = new SshClient(connectionInfo);

        await _sshClient.ConnectAsync(ct);
        _forwardedPort = new ForwardedPortLocal("127.0.0.1", (uint)ssh.LocalPort.Value, conf.HostName, (uint)conf.Port);
        _sshClient.AddForwardedPort(_forwardedPort);
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