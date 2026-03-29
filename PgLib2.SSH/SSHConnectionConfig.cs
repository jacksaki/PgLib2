using System.Text.Json.Serialization;

namespace PgLib2.SSH;

public class SSHConnectionConfig:IConnectionConfig
{
    [JsonPropertyName("host_name")]
    public required string HostName { get; init; }

    [JsonPropertyName("user_name")]
    public required string UserName { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }

    [JsonPropertyName("port")]
    public int Port { get; init; }

    [JsonPropertyName("database_name")]
    public required string DatabaseName { get; init; }

    [JsonPropertyName("option")]
    public string? Option { get; init; }

    [JsonPropertyName("ssh")]
    public required SSHConfig SSH { get; init;  }

    public async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!this.SSH.LocalPort.HasValue)
        {
            throw new InvalidOperationException($"open SSH tunnel before build connection string.");
        }
        return await Task.FromResult($"Server=127.0.0.1;Port={this.SSH.LocalPort};Database={this.DatabaseName};User Id={this.UserName};Password=\"{this.Password}\";{this.Option}");
    }
}
