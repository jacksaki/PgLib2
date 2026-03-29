using PgLib2.SSH;

namespace PgLib2;

public class DatabaseConnectionConfig(ConnectionConfigSettings settings)
{
    public string HostName => settings.HostName;

    public string UserName => settings.UserName;

    public string Password => settings.Password;

    public int Port => settings.Port;

    public string DatabaseName => settings.DatabaseName;

    public string? Option => settings.Option;

    public SshTunnel? Tunnel { get; internal set; }
    public bool UseSSH => this.Tunnel != null;
    public async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (this.Tunnel != null)
        {
            return await this.Tunnel.GetConnectionStringAsync();
        }
        else
        {
            return await Task.FromResult($"Host=\"{this.HostName}\";Port={this.Port};Username=\"{this.UserName}\";Password=\"{this.Password}\";Database=\"{this.DatabaseName}\";{this.Option}");
        }
    }
}
