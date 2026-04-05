using Cysharp.Diagnostics;
using System.Diagnostics;
using R3;
using ObservableCollections;

namespace YamaKit.Process;

public class ProcessXExtension
{
    public ObservableList<string> OutputList = new ObservableList<string>();
    public ObservableList<string> ErrorList = new ObservableList<string>();
    public int[] AcceptableExitCodes { get; set; } = new int[1];
    public async Task ExecuteAsync(ProcessStartInfo psi, string? input, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        ProcessX.AcceptableExitCodes = this.AcceptableExitCodes.Length > 0 ? this.AcceptableExitCodes : new int[1];

        var (process, stdOut, stdError) = ProcessX.GetDualAsyncEnumerable(psi);
        if (!string.IsNullOrEmpty(input))
        {
            await process.StandardInput.WriteAsync(input);
        }

        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        var consumeStdOut = Task.Run(async () =>
        {
            await foreach (var item in stdOut)
            {
                this.OutputList.Add(item);
            }
        });

        var errorBuffered = new List<string>();
        var consumeStdError = Task.Run(async () =>
        {
            await foreach (var item in stdError)
            {
                this.ErrorList.Add(item);
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

    public async Task ExecuteAsync(string filePath, string? args, string? input, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = filePath,
            Arguments = args,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        await ExecuteAsync(psi, input, ct);
    }
}