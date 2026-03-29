
using PgLib2.SSH;
using System.ComponentModel.DataAnnotations;

namespace PgLib2.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task ConnectAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var dbConf = conf.CreateDbConnectionConfig();
            var ct = new CancellationToken();
            var session = await PgSession.CreateAsync(dbConf, ct);
        }

        [Fact]
        public async Task ConnectWithSSHAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var ssh = new SshTunnel(conf.SSH!.CreateSecureShellConfig(), conf.CreateDbConnectionConfig());
            var ct = new CancellationToken();
            await ssh.ConnectAsync(ct);
            var session = await PgSession.CreateAsync(ssh.DbConfig, ct);
            var query = session.CreateQuery();
            var results = await query.QueryAsync("SELECT * FROM information_schema.tables",null, ct);
            foreach(var result in results)
            {
                foreach(KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
    }
}
