using System.Text.Json.Serialization;

namespace PgLib2.SSH;

public class SSHConfig
{
    [JsonIgnore]
    public uint? LocalPort { get; set; }

    [JsonPropertyName("ssh_port")]
    public int SshPort { get; set; }
    [JsonPropertyName("ssh_host_name")]
    public string SshHostName { get; set; } = string.Empty;
    [JsonPropertyName("ssh_user_name")]
    public string SshUserName { get; set; } = string.Empty;
    [JsonPropertyName("ssh_private_key")]
    public string? SshPrivateKey { get; set; }
    [JsonPropertyName("ssh_password")]
    public string? SshPassword { get; set; }
}
