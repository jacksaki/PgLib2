using PgLib2.Formatter;
using PgLib2.Schema.Dump;

namespace PgLib2.Tests;

public class SchemaDumpTest
{
    [Fact]
    public async Task DumpTestAsync()
    {
        var conf = await ConnectionConfig.LoaAsync();
        var dbConf = conf.CreateDbConnectionConfig();
        var ct = new CancellationToken();
        var dump = DumpGenerator.Create(dbConf);
        var result = await dump.GenerateDefinitionAsync("public", "actor", false, ct);
        Console.WriteLine(result);
    }
    [Fact]
    public async Task DumpTestWithFormatAsync()
    {
        var ct = new CancellationToken();
        var installer = LibraryInstaller.Create();
        if (!installer.IsInstalled)
        {
            await installer.InstallAsync(ct);
        }
        var conf = await ConnectionConfig.LoaAsync();
        var dbConf = conf.CreateDbConnectionConfig();
        var dump = DumpGenerator.Create(dbConf);
        var result = await dump.GenerateDefinitionAsync("public", "actor", true, ct);
        Console.WriteLine(result);
    }
}
