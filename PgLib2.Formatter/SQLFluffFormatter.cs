using Cysharp.Diagnostics;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace PgLib2.Formatter;

public class SQLFluffFormatter
{
    public delegate void LoggedEventHandler(object sender, LogItem e);
    public event LoggedEventHandler Logged = delegate { };
    internal FormatterConfig Config { get; }
    private SQLFluffFormatter(FormatterConfig config)
    {
        Config = config;
    }
    public string BaseDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public string PythonDir => Path.Combine(this.BaseDir, this.Config.PythonDir);
    public string SQLFluffDir => Path.Combine(this.PythonDir, "Scripts");
    public string SQLFluffExe => Path.Combine(this.SQLFluffDir, "sqlfluff.exe");

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
        var (_, stdOut, stdError) = ExtendedProcessX.GetDualAsyncEnumerableEx(psi, stdIn);

        var consumeStdOut = Task.Run(async () =>
        {
            await foreach (var item in stdOut)
            {
                Logged(this, new LogItem(InstallLogItemType.Output, item));
            }
        }, ct);

        var errorBuffered = new List<string>();
        var consumeStdError = Task.Run(async () =>
        {
            await foreach (var item in stdError)
            {
                Logged(this, new LogItem(InstallLogItemType.Error, item));
                errorBuffered.Add(item);
            }
        }, ct);
        try
        {
            await Task.WhenAll(consumeStdOut, consumeStdError).ConfigureAwait(false);
        }
        catch (ProcessErrorException ex)
        {
            throw new Exception(ex.ExitCode.ToString());
        }
    }
}
