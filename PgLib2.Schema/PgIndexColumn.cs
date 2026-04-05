using System.Text.Json.Serialization;

namespace PgLib2.Schema;

public class PgIndexColumn
{
    [JsonPropertyName("column_name")]
    [JsonInclude]
    public string ColumnName { get; private set; } = string.Empty;
    [JsonPropertyName("order")]
    [JsonInclude]
    public string Order { get; private set; } = string.Empty;
}
