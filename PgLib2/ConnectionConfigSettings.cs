namespace PgLib2;

public class ConnectionConfigSettings
{
    public required string HostName { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }

    public int Port { get; set; }

    public required string DatabaseName { get; set; }

    public string? Option { get; set; }
}
