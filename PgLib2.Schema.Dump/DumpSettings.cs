using System.Data;

namespace PgLib2.Schema.Dump;

public class DumpSettings
{
    public static DumpSettings Create()
    {
        return new DumpSettings();
    }
    private DumpSettings()
    {

    }

    internal static DumpSettings Load(DatabaseConnectionConfig connectionConf, DumpConfig dumpConf)
    {
        var settings = DumpSettings.Create();

        settings.HostName = connectionConf.HostName;
        settings.UserName = connectionConf.UserName;
        settings.DatabaseName = connectionConf.DatabaseName;
        settings.Password = connectionConf.Password;
        settings.Port = connectionConf.Port;
        settings.NoData = dumpConf.NoData;
        settings.NoOwner = dumpConf.NoOwner;
        settings.NoComments = dumpConf.NoComments;
        settings.NoPrivileges = dumpConf.NoPrivileges;
        return settings;
    }

    [PgDumpArgument("-h")]
    public string HostName { get; set; } = string.Empty;

    [PgDumpArgument("-U")]
    public string UserName { get; set; } = string.Empty;

    [PgDumpArgument("-d")]
    public string DatabaseName { get; set; } = string.Empty;

    [PgDumpArgument("-p")]
    public int Port { get; set; }

    public string Password { get; set; } = string.Empty;

    [PgDumpArgument("--schema",true)]
    public string SchemaName { get; set; } = string.Empty;

    [PgDumpArgument("--table", true)]
    public string TableName { get; set; } = string.Empty;


    [PgDumpArgument("--no-privileges")]
    public bool NoPrivileges { get; set; } = false;

    [PgDumpArgument("--no-owner")]
    public bool NoOwner { get; set; } = true;

    [PgDumpArgument("--no-comments")]
    public bool NoComments { get; set; } = false;

    [PgDumpArgument("--no-data")]
    public bool NoData { get; set; } = true;
}
