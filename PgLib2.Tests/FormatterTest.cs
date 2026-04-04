using ObservableCollections;
using PgLib2.Formatter;
using System;
using System.Collections.Generic;
using System.Text;
using R3;

namespace PgLib2.Tests
{
    public class FormatterTest
    {
        [Fact]
        public async Task InstallTestAsync()
        {
            var ct = new CancellationToken();
            var installer = LibraryInstaller.Create();
            if (!installer.IsInstalled)
            {
                await installer.InstallAsync(ct);
            }
        }

        [Fact]
        public async Task FormatTestAsync()
        {
            var ct = new CancellationToken();
            var installer = LibraryInstaller.Create();
            if (!installer.IsInstalled)
            {
                await installer.InstallAsync(ct);
            }
            var formatter = SQLFluffFormatter.Create();
            formatter.Logs.ObserveAdd(ct).Subscribe(x => Console.WriteLine(x.Value.Message));
            var sql = @"SELECT
 c.oid
,nc.nspname::information_schema.sql_identifier AS table_schema
,c.relname::information_schema.sql_identifier AS table_name
,obj_description(c.oid) AS comment
,(pg_relation_is_updatable(c.oid::regclass, false) & 8) = 8  AS is_insertable_into
FROM
 pg_namespace nc
INNER JOIN pg_class c ON (nc.oid = c.relnamespace)
LEFT OUTER JOIN (pg_type t INNER JOIN pg_namespace nt ON t.typnamespace = nt.oid) ON (c.reloftype = t.oid)
WHERE
c.relkind = 'r'";
            await formatter.ExecuteAsync(sql, ct);
        }
    }
}
