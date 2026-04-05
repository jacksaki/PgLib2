using PgLib2.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace PgLib2.Tests;

public class PgTableTest
{
    [Fact]
    public async Task ListAsync()
    {
        var conf = await ConnectionConfig.LoaAsync();
        var dbConf = conf.CreateDbConnectionConfig();
    
        var ct = new CancellationToken();
        
        var session = await PgSession.CreateAsync(dbConf, ct);
        var catalog = PgCatalog.Create(session);
        await foreach(var schema in catalog.ListSchemasAsync(ct))
        {
            await foreach (var table in catalog.ListTablesAsync(schema.Name, null, ct))
            {
                Console.WriteLine($"{table.SchemaName}.{table.Name}");
                await foreach(var column in table.ListColumnsAsync(ct))
                {
                    Console.WriteLine($"  {column.ColumnName} {column.DataType}");
                }
            }
        }
    }

}
