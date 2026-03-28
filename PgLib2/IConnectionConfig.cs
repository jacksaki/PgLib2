namespace PgLib2;

public interface IConnectionConfig
{
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }
    public int Port { get; }
    public string DatabaseName { get; }
    public string? Option { get; }
    public Task<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);
}
