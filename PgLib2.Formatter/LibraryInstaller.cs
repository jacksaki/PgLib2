using Cysharp.Diagnostics;
using R3;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using YamaKit.Process;
using ObservableCollections;
using System.Collections.ObjectModel;
namespace PgLib2.Formatter;

public class LibraryInstaller
{
    public static LibraryInstaller Create()
    {
        var confPath = System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".conf");
        return LibraryInstaller.Create(confPath);
    }

    public string BaseDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public string PythonDir => Path.Combine(this.BaseDir, this.Config.PythonDir);
    public string PythonExePath => Path.Combine(this.PythonDir, "python.exe");
    public string ScriptDir => Path.Combine(this.PythonDir, "Scripts");
    public string SQLFluffExePath => Path.Combine(this.ScriptDir, "sqlfluff.exe");
    public string PipPath => Path.Combine(this.ScriptDir, "pip.exe");
    internal FormatterConfig Config { get; private set; }
    public bool IsInstalled => File.Exists(this.SQLFluffExePath);
    public ObservableList<LogItem> Logs { get; }
    private LibraryInstaller(FormatterConfig config)
    {
        this.Config = config;
        this.Logs = new ObservableList<LogItem>();
    }

    public static LibraryInstaller Create(string confPath)
    {
        if (!File.Exists(confPath))
        {
            throw new FileNotFoundException("Formatter config not found", confPath);
        }
        var installer = new LibraryInstaller(JsonSerializer.Deserialize<FormatterConfig>(File.ReadAllText(confPath))!);
        return installer;
    }

    private async Task RunCommandAsync(string filePath, string args,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var psi = new ProcessStartInfo
        {
            FileName = filePath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        var p = new ProcessXExtension();
        p.OutputList.ObserveAdd().Subscribe(x => this.Logs.Add(new LogItem(InstallLogItemType.Output, x.Value)));
        p.ErrorList.ObserveAdd().Subscribe(x => this.Logs.Add(new LogItem(InstallLogItemType.Error, x.Value)));
        await p.ExecuteAsync(psi, null, ct);
    }

    public async Task InstallAsync(CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();

        if (this.IsInstalled)
        {
            return;
        }

        Directory.CreateDirectory(this.PythonDir);

        if (!File.Exists(this.PythonExePath))
        {
            await InstallEmbeddedPythonAsync(this.Config.PythonUrl, this.PythonDir, ct);
            EnableSite(this.PythonDir);
            await InstallPipAsync(this.PythonExePath, this.PythonDir, ct);
        }

        await RunCommandAsync(this.PipPath, "install --upgrade pip", ct);

        await RunCommandAsync(this.PipPath, "install sqlfluff", ct);

        var sqlFluffConfPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
            ".sqlfluff");

        System.IO.File.Copy(sqlFluffConfPath, Path.Combine(this.PythonDir, "Scripts", ".sqlfluff"));
    }

    private static async Task InstallEmbeddedPythonAsync(string url, string targetDir,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var zipPath = Path.Combine(targetDir, "python_embed.zip");

        using var wc = new HttpClient();
        var bytes = await wc.GetByteArrayAsync(url).ConfigureAwait(false);
        await File.WriteAllBytesAsync(zipPath, bytes).ConfigureAwait(false);

        ZipFile.ExtractToDirectory(zipPath, targetDir, overwriteFiles: true);
        File.Delete(zipPath);
    }

    private static void EnableSite(string pythonDir)
    {
        var pth = Directory.GetFiles(pythonDir, "python*._pth").Single();

        var lines = File.ReadAllLines(pth)
            .Select(l => l.Trim())
            .ToList();

        if (!lines.Any(l => l == "import site"))
        {
            lines.Add("import site");
            File.WriteAllLines(pth, lines);
        }
    }

    private async Task InstallPipAsync(string pythonExe, string pythonDir,CancellationToken ct=default)
    {
        ct.ThrowIfCancellationRequested();
        var getPipPath = Path.Combine(pythonDir, "get-pip.py");

        using var wc = new HttpClient();
        var bytes = await wc.GetByteArrayAsync("https://bootstrap.pypa.io/get-pip.py").ConfigureAwait(false);
        await File.WriteAllBytesAsync(getPipPath, bytes).ConfigureAwait(false);

        await RunCommandAsync(pythonExe, $"\"{getPipPath}\"", ct);
    }
}
