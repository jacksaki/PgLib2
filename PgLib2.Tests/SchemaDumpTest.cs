using PgLib2.Schema.Dump;
using System;
using System.Collections.Generic;
using System.Text;

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
        var result = await dump.GenerateDefinitionAsync("public", "actor", ct);
        Console.WriteLine(result);
    }
}
