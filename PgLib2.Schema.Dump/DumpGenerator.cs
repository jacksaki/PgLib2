using System.Diagnostics;
using YamaKit.Process;
using R3;
using ObservableCollections;
using PgLib2.Formatter;
namespace PgLib2.Schema.Dump;

public class DumpGenerator
{
    private DatabaseConnectionConfig _conf;
    private DumpGenerator(DatabaseConnectionConfig conf)
    {
        _conf = conf;
    }
    public static DumpGenerator Create(DatabaseConnectionConfig conf)
    {
        return new DumpGenerator(conf);
    }

    public async Task<string> GenerateDefinitionAsync(string schemaName,string tableName,bool withFormat, CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        if (withFormat)
        {
            var installer = LibraryInstaller.Create();
            if (!installer.IsInstalled)
            {
                throw new Exception("Formatter not installed.");
            }
        }

        var dumpConf = await DumpConfig.LoadAsync();
        var settings = DumpSettings.Load(_conf, dumpConf);
        settings.SchemaName =schemaName;
        settings.TableName =tableName;
        var orgPassword = System.Environment.GetEnvironmentVariable("PGPASSWORD");
        System.Environment.SetEnvironmentVariable("PGPASSWORD", settings.Password);
        var psi = new ProcessStartInfo()
        {
            FileName = dumpConf.PgDumpPath,
            Arguments = settings.ToArgument(),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        var p = new ProcessXExtension();
        p.AcceptableExitCodes = new int[] { 0, 1 };
        var list = new List<string>();
        var errors = new List<string>();
        var outputs=new List<string>();
        p.OutputList.ObserveAdd().Subscribe(x => {
            if (x.Value.IsValidLine())
            {
                list.Add(x.Value);
            }
            outputs.Add(x.Value);
        });
        p.ErrorList.ObserveAdd().Subscribe(x =>
        {
            errors.Add(x.Value);
            outputs.Add(x.Value);
        });
        try
        {
            await p.ExecuteAsync(psi, null, ct);

            var sql  =string.Join("\r\n", list);
            if (withFormat)
            {
                var formatter = SQLFluffFormatter.Create();
                return await formatter.ExecuteAsync(sql);
            }
            else
            {
                return sql;
            }
        }
        catch 
        {
            throw new Exception(string.Join("\r\n",p.OutputList));
        }
        finally
        {
            System.Environment.SetEnvironmentVariable("PGPASSWORD", orgPassword);
        }
    }
}