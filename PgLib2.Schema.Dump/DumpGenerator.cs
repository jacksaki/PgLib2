using System.Diagnostics;
using YamaKit.Process;
using R3;
using ObservableCollections;
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

    public async Task<string> GenerateDefinitionAsync(string schemaName,string tableName,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
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
        var list = new List<string>();
        var errors = new List<string>();
        var outputs=new List<string>();
        p.OutputList.ObserveAdd().Subscribe(x => {
            list.Add(x.Value);
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
            return string.Join("\r\n", list);
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