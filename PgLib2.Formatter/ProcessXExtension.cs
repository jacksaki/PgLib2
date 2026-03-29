using Cysharp.Diagnostics;
using System.Diagnostics;
using System.Threading.Channels;

namespace PgLib2.Formatter;

class ProcessAsyncEnumerator : IAsyncEnumerator<string>
{
    readonly Process? process;
    readonly ChannelReader<string> channel;
    readonly CancellationToken cancellationToken;
    readonly CancellationTokenRegistration cancellationTokenRegistration;
    string? current;
    bool disposed;

    public ProcessAsyncEnumerator(Process? process, ChannelReader<string> channel, CancellationToken cancellationToken)
    {
        // process is not null, kill when canceled.
        this.process = process;
        this.channel = channel;
        this.cancellationToken = cancellationToken;
        if (cancellationToken.CanBeCanceled)
        {
            cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                _ = DisposeAsync();
            });
        }
    }

#pragma warning disable CS8603
    // when call after MoveNext, current always not null.
    public string Current => current;
#pragma warning restore CS8603

    public async ValueTask<bool> MoveNextAsync()
    {
        if (channel.TryRead(out current))
        {
            return true;
        }
        else
        {
            if (await channel.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (channel.TryRead(out current))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public ValueTask DisposeAsync()
    {
        if (!disposed)
        {
            disposed = true;
            try
            {
                cancellationTokenRegistration.Dispose();
                if (process != null)
                {
                    process.EnableRaisingEvents = false;
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
            }
            finally
            {
                if (process != null)
                {
                    process.Dispose();
                }
            }
        }

        return default;
    }
}
public class ProcessAsyncEnumerable : IAsyncEnumerable<string>
{
    readonly Process? process;
    readonly ChannelReader<string> channel;

    internal ProcessAsyncEnumerable(Process? process, ChannelReader<string> channel)
    {
        this.process = process;
        this.channel = channel;
    }

    public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ProcessAsyncEnumerator(process, channel, cancellationToken);
    }

    /// <summary>
    /// Consume all result and wait complete asynchronously.
    /// </summary>
    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var _ in this.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
        }
    }

    /// <summary>
    /// Returning first value and wait complete asynchronously.
    /// </summary>
    public async Task<string> FirstAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? data = null;
        await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (data == null)
            {
                data = (item ?? "");
            }
        }

        if (data == null)
        {
            throw new InvalidOperationException("Process does not return any data.");
        }
        else
        {
            return data;
        }
    }

    /// <summary>
    /// Returning first value or null and wait complete asynchronously.
    /// </summary>
    public async Task<string?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? data = null;
        await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (data == null)
            {
                data = (item ?? "");
            }
        }
        return data;
    }

    public async Task<string[]> ToTask(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var list = new List<string>();
        await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            list.Add(item);
        }
        return list.ToArray();
    }

    /// <summary>
    /// Write the all received data to console.
    /// </summary>
    public async Task WriteLineAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await foreach (var item in this.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            Console.WriteLine(item);
        }
    }
}
public static class ExtendedProcessX
{
    public static IReadOnlyList<int> AcceptableExitCodes { get; set; } = ProcessX.AcceptableExitCodes;
    static bool IsInvalidExitCode(Process process)
    {
        return !AcceptableExitCodes.Any(x => x == process.ExitCode);
    }

    static Process SetupRedirectableProcess(ref ProcessStartInfo processStartInfo, bool redirectStandardInput)
    {
        // override setings.
        processStartInfo.UseShellExecute = false;
        processStartInfo.CreateNoWindow = true;
        processStartInfo.ErrorDialog = false;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardInput = redirectStandardInput;

        var process = new Process()
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true,
        };

        return process;
    }

    public static (Process Process, ProcessAsyncEnumerable StdOut, ProcessAsyncEnumerable StdError) GetDualAsyncEnumerableEx(ProcessStartInfo processStartInfo, string? stdIn = null)
    {
        Process process = SetupRedirectableProcess(ref processStartInfo, redirectStandardInput: true);
        Channel<string> outputChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });
        Channel<string> errorChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });
        TaskCompletionSource<object?> waitOutputDataCompleted = new TaskCompletionSource<object?>();
        process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                outputChannel.Writer.TryWrite(e.Data);
            }
            else
            {
                waitOutputDataCompleted.TrySetResult(null);
            }
        };
        TaskCompletionSource<object?> waitErrorDataCompleted = new TaskCompletionSource<object?>();
        process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                errorChannel.Writer.TryWrite(e.Data);
            }
            else
            {
                waitErrorDataCompleted.TrySetResult(null);
            }
        };
        process.Exited += async delegate
        {
            await waitErrorDataCompleted.Task.ConfigureAwait(continueOnCapturedContext: false);
            await waitOutputDataCompleted.Task.ConfigureAwait(continueOnCapturedContext: false);
            if (IsInvalidExitCode(process))
            {
                errorChannel.Writer.TryComplete();
                outputChannel.Writer.TryComplete(new ProcessErrorException(process.ExitCode, Array.Empty<string>()));
            }
            else
            {
                errorChannel.Writer.TryComplete();
                outputChannel.Writer.TryComplete();
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException("Can't start process. FileName:" + processStartInfo.FileName + ", Arguments:" + processStartInfo.Arguments);
        }

        if (stdIn != null)
        {
            process.StandardInput.Write(stdIn);
            process.StandardInput.Close();
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return (Process: process, StdOut: new ProcessAsyncEnumerable(process, outputChannel.Reader), StdError: new ProcessAsyncEnumerable(null, errorChannel.Reader));
    }

}
