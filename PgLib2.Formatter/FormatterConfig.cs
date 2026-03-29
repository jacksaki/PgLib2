using System.Text.Json.Serialization;

namespace PgLib2.Formatter;

public class FormatterConfig
{
    [JsonPropertyName("python_url")]
    [JsonInclude]
    public string PythonUrl { get; private set; } = "https://www.python.org/ftp/python/3.11.8/python-3.11.8-embed-amd64.zip";

    [JsonPropertyName("python_dir")]
    [JsonInclude]
    public string PythonDir { get; private set; } = "python";

    [JsonPropertyName("sqlfluff_version")]
    [JsonInclude]
    public string SqlfluffVersion { get; private set; } = "3.0.5";

    [JsonPropertyName("arguments")]
    [JsonInclude]
    public string Arguments { get; private set; } = "fix - --dialect postgres --disable-progress-bar";

    [JsonPropertyName("fix_file_arguments")]
    [JsonInclude]
    public string FixFileArguments { get; private set; } = "fix --dialect postgres --disable-progress-bar";
}
