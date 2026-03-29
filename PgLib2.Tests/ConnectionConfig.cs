using System.Text.Json;
using System.Text.Json.Serialization;

namespace PgLib2.Tests;

public class ConnectionConfig
{
    public static async Task<ConnectionConfig> LoaAsync()
    {
        var path = System.Environment.GetEnvironmentVariable("connection_config");
        var json = await File.ReadAllTextAsync(path ?? throw new InvalidOperationException("Failed to load connection config."));
        return JsonSerializer.Deserialize<ConnectionConfig>(json) ?? throw new InvalidOperationException("Failed to deserialize connection config.");
    }

    public DatabaseConnectionConfig CreateDbConnectionConfig()
    {
        return new DatabaseConnectionConfig(new ConnectionConfigSettings
        {
            HostName = this.HostName,
            DatabaseName = this.DatabaseName,
            Port = this.Port,
            UserName = this.UserName,
            Password = this.Password,
            Option = this.Option
        });
    }

    [JsonPropertyName("db_host")]
    public required string HostName { get; init; }

    [JsonPropertyName("db_port")]
    public required int Port { get; init; }

    [JsonPropertyName("db_name")]
    public required string DatabaseName { get; init; }

    [JsonPropertyName("db_user_name")]
    public required string UserName { get; init; }

    [JsonPropertyName("db_password")]
    public required string Password { get; init; }

    [JsonPropertyName("option")]
    public string? Option { get; init; }

    [JsonPropertyName("ssh")]
    public SSHConfig? SSH { get; init; }
}