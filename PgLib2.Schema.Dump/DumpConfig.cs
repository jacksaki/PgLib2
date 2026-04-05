using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PgLib2.Schema.Dump;

internal class DumpConfig
{
    public string PgDumpPath =>
        System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
            this.ToolDirectory,
            "pg_dump.exe");

    public static async Task<DumpConfig> LoadAsync()
    {
        var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "PgLib2.Schema.Dump.conf");
        return await LoadAsync(path);
    }
    public static async Task<DumpConfig> LoadAsync(string path)
    {
        var json = await System.IO.File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<DumpConfig>(json)!;
    }

    [JsonPropertyName("tools_dir")]
    public required string ToolDirectory { get; init; }

    [JsonPropertyName("no_privileges")]
    public bool NoPrivileges { get; set; } = false;

    [JsonPropertyName("no_owner")]
    public bool NoOwner { get; set; } = true;

    [JsonPropertyName("no_comments")]
    public bool NoComments { get; set; } = false;

    [JsonPropertyName("no_data")]
    public bool NoData { get; set; } = true;
}
