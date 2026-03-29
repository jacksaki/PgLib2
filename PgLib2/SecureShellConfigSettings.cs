using System;
using System.Collections.Generic;
using System.Text;

namespace PgLib2;

public class SecureShellConfigSettings
{
    public int SshPort { get; set; }
    public string SshHostName { get; set; } = string.Empty;
    public string SshUserName { get; set; } = string.Empty;
    public string? SshPrivateKey { get; set; }
    public string? SshPassword { get; set; }
}
