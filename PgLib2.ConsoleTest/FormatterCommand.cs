using ConsoleAppFramework;
using PgLib2.Formatter;
using System;
using System.Collections.Generic;
using System.Text;
using Kokuban;
using ObservableCollections;
using R3;

namespace PgLib2.ConsoleTest;

public class FormatterCommand
{
    /// <summary>
    /// Install Formatter.
    /// </summary>
    /// <returns></returns>
    [Command("install")]
    public async Task InstallAsync()
    {
        var ct = new CancellationToken();
        var installer = LibraryInstaller.Create();
        if (installer.IsInstalled)
        {
            Console.WriteLine(Chalk.Yellow + $"Already installed.");
            return;
        }
        installer.Logs.ObserveAdd().Subscribe(x =>
        {
            var log = x.Value;
            if(log.Type== InstallLogItemType.Output)
            {
                Console.WriteLine($"{log.Date:HH:mm:ss}\t{log.Message}");
            }
            else
            {
                Console.Error.WriteLine(Chalk.Yellow + $"{log.Date:HH:mm:ss}\t{log.Message}");
            }
        });
        await installer.InstallAsync(ct);
    }

    /// <summary>
    /// Format SQL.
    /// </summary>
    /// <param name="sql">-s, SQL</param>
    /// <returns></returns>
    [Command("format")]
    public async Task FormatAsync(string sql)
    {
        var ct = new CancellationToken();
        var installer = LibraryInstaller.Create();
        if (!installer.IsInstalled)
        {
            Console.Error.WriteLine(Chalk.Yellow + $"formatter not installed.");
            return;
        }
        var formatter = SQLFluffFormatter.Create();
        formatter.Logs.ObserveAdd().Subscribe(x =>
        {
            var log = x.Value;
            if (log.Type == InstallLogItemType.Output)
            {
                Console.WriteLine($"{log.Date:HH:mm:ss}\t{log.Message}");
            }
            else
            {
                Console.WriteLine(Chalk.Yellow + $"{log.Date:HH:mm:ss}\t{log.Message}");
            }
        });
        await formatter.ExecuteAsync(sql, ct);
    }

    /// <summary>
    /// Format SQL from file.
    /// </summary>
    /// <param name="path">-s, SQL file path.</param>
    /// <returns></returns>
    [Command("format-file")]
    public async Task FormatFileAsync(string path)
    {
        var sql = await System.IO.File.ReadAllTextAsync(path);
        await FormatAsync(sql);
    }

}
