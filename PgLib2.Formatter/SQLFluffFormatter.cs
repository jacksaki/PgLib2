using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using R3;
using ObservableCollections;
using YamaKit.Process;

namespace PgLib2.Formatter;

public class SQLFluffFormatter
{
    internal FormatterConfig Config { get; }
    
    private SQLFluffFormatter(FormatterConfig config)
    {
        Config = config;
    }
    public string BaseDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public string PythonDir => Path.Combine(this.BaseDir, this.Config.PythonDir);
    public string SQLFluffDir => Path.Combine(this.PythonDir, "Scripts");
    public string SQLFluffExe => Path.Combine(this.SQLFluffDir, "sqlfluff.exe");
    public ObservableList<LogItem> Logs { get; } = new ObservableList<LogItem>();
    public static SQLFluffFormatter Create(string configPath)
    {
        var conf = JsonSerializer.Deserialize<FormatterConfig>(File.ReadAllText(configPath))!;
        return new SQLFluffFormatter(conf);
    }
    public static SQLFluffFormatter Create()
    {
        var confPath = System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".conf");
        return SQLFluffFormatter.Create(confPath);
    }

    public async Task FixSQLFileAsync(string sqlPath,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var psi = new ProcessStartInfo
        {
            FileName = this.SQLFluffExe,
            WorkingDirectory = this.SQLFluffDir,
            Arguments = this.Config.FixFileArguments.Replace("$FILE_PATH$", sqlPath),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await ExecuteCommandAsync(psi, null, ct);
    }

    public async Task ExecuteAsync(string sql,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var psi = new ProcessStartInfo
        {
            FileName = this.SQLFluffExe,
            WorkingDirectory = this.SQLFluffDir,
            Arguments = this.Config.Arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await ExecuteCommandAsync(psi, sql,ct);
    }

    private async Task ExecuteCommandAsync(ProcessStartInfo psi, string? stdIn = null,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var p = new ProcessXExtension();
        p.OutputList.ObserveAdd().Subscribe(x => this.Logs.Add(new LogItem(InstallLogItemType.Output, x.Value)));
        p.ErrorList.ObserveAdd().Subscribe(x => this.Logs.Add(new LogItem(InstallLogItemType.Error, x.Value)));
        await p.ExecuteAsync(psi,stdIn,ct);
    }
}
