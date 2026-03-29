using System.Text.Json.Serialization;

namespace PgLib2.SSH;

public class SecureShellConfig(SecureShellConfigSettings settings)
{
    public uint? LocalPort { get; internal set; }
    public int SshPort => settings.SshPort;
    public string SshHostName =>settings.SshHostName;
    public string SshUserName =>settings.SshUserName;
    public string? SshPrivateKey => settings.SshPrivateKey;
    public string? SshPassword => settings.SshPassword;
}
