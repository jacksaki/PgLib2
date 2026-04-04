using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using PgLib2.ConsoleTest;
using ZLogger;

namespace ConsoleAppFrameworkTemplate1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = ConsoleApp.Create();
            //.ConfigureLogging(x=>
            //{
            //    x.ClearProviders();
            //    x.SetMinimumLevel(LogLevel.Trace);
            //    x.AddZLoggerConsole();
            //});
            app.Add<FormatterCommand>();
            app.Run(args);
        }
    }
}
