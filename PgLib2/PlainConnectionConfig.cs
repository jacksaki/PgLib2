namespace PgLib2;

internal class PlainConnectionConfig(string hostName, int port, string userName, string password, string databaseName, string? option) : IConnectionConfig
{

    public string HostName => hostName;

    public string UserName => userName;

    public string Password => password;

    public int Port => port;

    public string DatabaseName => databaseName;

    public string? Option => option;

    public async Task<string> GetConnectionStringAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult($"Host=\"{this.HostName}\";Port={this.Port};Username=\"{this.UserName}\";Password=\"{this.Password}\";Database=\"{this.DatabaseName}\";{this.Option}");
    }
}
