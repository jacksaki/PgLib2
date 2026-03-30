
using PgLib2.SSH;
using System.ComponentModel.DataAnnotations;
[assembly: CaptureConsole]
namespace PgLib2.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task WithTransactionAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var dbConf = conf.CreateDbConnectionConfig();
            var ct = new CancellationToken();
            var session = await PgSession.CreateAsync(dbConf, ct);

            var query = session.CreateQuery();
            var results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }

            await Task.Delay(1000, ct);
            await session.WithTransactionAsync(async (ct) =>
            {
                var query = session.CreateQuery();
                await query.ExecuteAsync("UPDATE language SET last_update = CURRENT_TIMESTAMP WHERE language_id=1", null, ct);
            }, System.Data.IsolationLevel.ReadCommitted, ct);

            query = session.CreateQuery();
            results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        [Fact]
        public async Task CommitAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var dbConf = conf.CreateDbConnectionConfig();
            var ct = new CancellationToken();
            var session = await PgSession.CreateAsync(dbConf, ct);

            var query = session.CreateQuery();
            var results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            await Task.Delay(1000, ct);
            await session.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
            query = session.CreateQuery();
            await query.ExecuteAsync("UPDATE language SET last_update = CURRENT_TIMESTAMP WHERE language_id=1", null, ct);
            await session.CommitAsync(ct);

            query = session.CreateQuery();
            results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        [Fact]
        public async Task RollbackAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var dbConf = conf.CreateDbConnectionConfig();
            var ct = new CancellationToken();
            var session = await PgSession.CreateAsync(dbConf, ct);

            var query = session.CreateQuery();
            var results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            await Task.Delay(1000, ct);
            await session.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, ct);
            query = session.CreateQuery();
            await query.ExecuteAsync("UPDATE language SET last_update = CURRENT_TIMESTAMP WHERE language_id=1", null, ct);
            await session.RollbackAsync(ct);

            query = session.CreateQuery();
            results = await query.QueryAsync("SELECT * FROM language WHERE language_id=1", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

        [Fact]
        public async Task ConnectAsync()
        {
            var conf = await ConnectionConfig.LoaAsync();
            var dbConf = conf.CreateDbConnectionConfig();
            var ct = new CancellationToken();
            var session = await PgSession.CreateAsync(dbConf, ct);
            var query = session.CreateQuery();
            var results = await query.QueryAsync("SELECT * FROM information_schema.tables", null, ct);
            foreach (var result in results)
            {
                foreach (KeyValuePair<string, object?> kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
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
