using System.Text.Json.Serialization;

namespace PgLib2.Schema;

internal class PgConstraintColumn
{
    [JsonPropertyName("column_name")]
    [JsonInclude]
    public string ColumnName { get; private set; } = string.Empty;
}
