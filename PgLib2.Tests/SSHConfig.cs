using PgLib2.SSH;
using System.Text.Json.Serialization;

namespace PgLib2.Tests;

public class SSHConfig
{
    public SecureShellConfig CreateSecureShellConfig()
    {
        return new SecureShellConfig(new SecureShellConfigSettings()
        {
            SshPort = this.Port,
            SshHostName = this.HostName,
            SshUserName = this.UserName,
            SshPrivateKey = this.PrivateKey,
            SshPassword = this.Password,
        });
    }
    [JsonPropertyName("ssh_port")]
	public required int Port { get; init; }
    [JsonPropertyName("ssh_host_name")]
    public required string HostName { get; init; }
    [JsonPropertyName("ssh_user_name")]
    public required string UserName { get; init; }
    [JsonPropertyName("ssh_private_key")]
    public required string PrivateKey { get; init; }
    [JsonPropertyName("ssh_password")]
    public string? Password { get; init; }	
}
